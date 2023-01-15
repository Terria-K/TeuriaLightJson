using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace LightJson.Serialization
{
	using ErrorType = JsonParseException.ErrorType;

	/// <summary>
	/// Represents a reader that can read JsonValues.
	/// </summary>
	public sealed class JsonTextReader
	{
		private TextScanner scanner;

		private JsonTextReader(TextReader reader)
		{
			this.scanner = new TextScanner(reader);
		}

		private string ReadJsonKey()
		{
			return ReadString();
		}

		private JsonValue ReadJsonValue()
		{
			this.scanner.SkipWhitespace();

			var next = this.scanner.Peek();

			if (char.IsNumber(next)) 
			{
				return ReadNumber();
			}


			return next switch 
			{
				'{' => ReadObject(),
				'[' => ReadArray(),
				'"' => ReadString(),
				'-' => ReadNumber(),
				't' or 'f' => ReadBoolean(),
				'n' => ReadNull(),
				_ => throw new JsonParseException(
						ErrorType.InvalidOrUnexpectedCharacter,
						this.scanner.Position)
			};
		}

		private JsonValue ReadNull()
		{
			this.scanner.Assert("null");
			return JsonValue.Null;
		}

		private JsonValue ReadBoolean()
		{
			switch (this.scanner.Peek())
			{
				case 't':
					this.scanner.Assert("true");
					return true;

				case 'f':
					this.scanner.Assert("false");
					return false;

				default:
					throw new JsonParseException(
						ErrorType.InvalidOrUnexpectedCharacter,
						this.scanner.Position
					);
			}
		}

		private void ReadDigits(StringBuilder builder)
		{
			while (this.scanner.CanRead && char.IsDigit(this.scanner.Peek()))
			{
				builder.Append(this.scanner.Read());
			}
		}

		private JsonValue ReadNumber()
		{
			var builder = new StringBuilder();

			if (this.scanner.Peek() == '-')
			{
				builder.Append(this.scanner.Read());
			}

			if (this.scanner.Peek() == '0')
			{
				builder.Append(this.scanner.Read());
			}
			else
			{
				ReadDigits(builder);
			}

			if (this.scanner.CanRead && this.scanner.Peek() == '.')
			{
				builder.Append(this.scanner.Read());
				ReadDigits(builder);
			}

			if (this.scanner.CanRead && char.ToLowerInvariant(this.scanner.Peek()) == 'e')
			{
				builder.Append(this.scanner.Read());

				var next = this.scanner.Peek();

				switch (next)
				{
					case '+':
					case '-':
						builder.Append(this.scanner.Read());
						break;
				}

				ReadDigits(builder);
			}

			return double.Parse(
				builder.ToString(),
				CultureInfo.InvariantCulture
			);
		}

		private string ReadString()
		{
			var builder = new StringBuilder();

			this.scanner.Assert('"');

			while (true)
			{
				var c = this.scanner.Read();

				if (c == '\\')
				{
					c = this.scanner.Read();

					switch (char.ToLower(c))
					{
						case '"':  // "
						case '\\': // \
						case '/':  // /
							builder.Append(c);
							break;
						case 'b':
							builder.Append('\b');
							break;
						case 'f':
							builder.Append('\f');
							break;
						case 'n':
							builder.Append('\n');
							break;
						case 'r':
							builder.Append('\r');
							break;
						case 't':
							builder.Append('\t');
							break;
						case 'u':
							builder.Append(ReadUnicodeLiteral());
							break;
						default:
							throw new JsonParseException(
								ErrorType.InvalidOrUnexpectedCharacter,
								this.scanner.Position
							);
					}
				}
				else if (c == '"')
				{
					break;
				}
				else
				{
                    /*
                     * According to the spec:
                     * 
                     * unescaped = %x20-21 / %x23-5B / %x5D-10FFFF
                     * 
                     * i.e. c cannot be < 0x20, be 0x22 (a double quote) or a 
                     * backslash (0x5C).
                     * 
                     * c cannot be a back slash or double quote as the above 
                     * would have hit. So just check for < 0x20.
                     * 
                     * > 0x10FFFF is unnecessary *I think* because it's obviously
                     * out of the range of a character but we might need to look ahead
                     * to get the whole utf-16 codepoint
                     */
                    if (c < '\u0020')
                    {
                        throw new JsonParseException(
                            ErrorType.InvalidOrUnexpectedCharacter,
                            this.scanner.Position
                        );
                    }
                    else
					{
						builder.Append(c);
					}
				}
			}

			return builder.ToString();
		}

		private int ReadHexDigit()
		{
			var scannerRead = char.ToUpper(scanner.Read());
			return scannerRead switch 
			{
				'0' => 0,
				'1' => 1,
				'2' => 2,
				'3' => 3,
				'4' => 4,
				'5' => 5,
				'6' => 6,
				'7' => 7,
				'8' => 8,
				'9' => 9,
				'A' => 10,
				'B' => 11,
				'C' => 12,
				'D' => 13,
				'E' => 14,
				'F' => 15,
				_ => throw new JsonParseException(
						ErrorType.InvalidOrUnexpectedCharacter,
						this.scanner.Position
					)
			};
		}

		private char ReadUnicodeLiteral()
		{
			int value = 0;

			value += ReadHexDigit() * 4096; // 16^3
			value += ReadHexDigit() * 256;  // 16^2
			value += ReadHexDigit() * 16;   // 16^1
			value += ReadHexDigit();        // 16^0

			return (char)value;
		}

		private JsonObject ReadObject()
		{
			return ReadObject(new JsonObject());
		}

		private JsonObject ReadObject(JsonObject jsonObject)
		{
			this.scanner.Assert('{');

			this.scanner.SkipWhitespace();

			if (this.scanner.Peek() == '}')
			{
				this.scanner.Read();
			}
			else
			{
				while (true)
				{
					this.scanner.SkipWhitespace();

					var key = ReadJsonKey();

					if (jsonObject.ContainsKey(key))
					{
						throw new JsonParseException(
							ErrorType.DuplicateObjectKeys,
							this.scanner.Position
						);
					}

					this.scanner.SkipWhitespace();

					this.scanner.Assert(':');

					this.scanner.SkipWhitespace();

					var value = ReadJsonValue();

					jsonObject.Add(key, value);

					this.scanner.SkipWhitespace();

					var next = this.scanner.Read();

					if (next == '}')
					{
						break;
					}
					else if (next == ',')
					{
						continue;
					}
					else
					{
						throw new JsonParseException(
							ErrorType.InvalidOrUnexpectedCharacter,
							this.scanner.Position
						);
					}
				}
			}

			return jsonObject;
		}

		private JsonArray ReadArray()
		{
			return ReadArray(new JsonArray());
		}

		private JsonArray ReadArray(JsonArray jsonArray)
		{
			this.scanner.Assert('[');

			this.scanner.SkipWhitespace();

			if (this.scanner.Peek() == ']')
			{
				this.scanner.Read();
			}
			else
			{
				while (true)
				{
					this.scanner.SkipWhitespace();

					var value = ReadJsonValue();

					jsonArray.Add(value);

					this.scanner.SkipWhitespace();

					var next = this.scanner.Read();

					if (next == ']')
					{
						break;
					}
					else if (next == ',')
					{
						continue;
					}
					else
					{
						throw new JsonParseException(
							ErrorType.InvalidOrUnexpectedCharacter,
							this.scanner.Position
						);
					}
				}
			}

			return jsonArray;
		}

		private JsonValue Parse()
		{
			this.scanner.SkipWhitespace();
			return ReadJsonValue();
		}

		/// <summary>
		/// Creates a JsonValue by using the given TextReader.
		/// </summary>
		/// <param name="reader">The TextReader used to read a JSON message.</param>
		public static JsonValue Parse(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			return new JsonTextReader(reader).Parse();
		}

		/// <summary>
		/// Creates a JsonValue by reader the JSON message in the given string.
		/// </summary>
		/// <param name="source">The string containing the JSON message.</param>
		public static JsonValue Parse(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			using (var reader = new StringReader(source))
			{
				return new JsonTextReader(reader).Parse();
			}
		}

		/// <summary>
		/// Creates a JsonValue by reading the given file.
		/// </summary>
		/// <param name="path">The file path to be read.</param>
		public static JsonValue ParseFile(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			

			// NOTE: FileAccess.Read is needed to be able to open read-only files
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			using (var reader = new StreamReader(stream))
			{
				return new JsonTextReader(reader).Parse();
			}
		}

		public static JsonValue ParseFile(FileStream fs) 
		{
			using var reader = new StreamReader(fs);

			return new JsonTextReader(reader).Parse();
		}
	}
}
