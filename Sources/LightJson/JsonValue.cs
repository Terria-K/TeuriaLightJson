using System;
using System.Diagnostics;
using System.Collections.Generic;
using LightJson.Serialization;

namespace LightJson
{
	/// <summary>
	/// A wrapper object that contains a valid JSON value.
	/// </summary>
	[DebuggerDisplay("{ToString(),nq}", Type = "JsonValue({Type})")]
	[DebuggerTypeProxy(typeof(JsonValueDebugView))]
	public struct JsonValue
	{
		private readonly JsonValueType type;
		private readonly JsonNumberType numberType;
		private readonly object reference;
		private readonly double value;

		/// <summary>
		/// Represents a null JsonValue.
		/// </summary>
		public static readonly JsonValue Null = new JsonValue(JsonValueType.Null, default(double), null);

		/// <summary>
		/// Gets the type of this JsonValue.
		/// </summary>
		public JsonValueType Type => type;
		public JsonNumberType NumberType => numberType;
		

		/// <summary>
		/// Gets a value indicating whether this JsonValue is Null.
		/// </summary>
		public bool IsNull
		{
			get
			{
				return this.Type == JsonValueType.Null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue is a Boolean.
		/// </summary>
		public bool IsBoolean
		{
			get
			{
				return this.Type == JsonValueType.Boolean;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue is an Integer.
		/// </summary>
		public bool IsInteger
		{
			get
			{
				if (!this.IsNumber)
				{
					return false;
				}

				var value = this.value;

				return (value >= Int32.MinValue) && (value <= Int32.MaxValue) && unchecked((Int32)value) == value;
			}
		}
		public bool IsLong
		{
			get
			{
				if (!this.IsNumber)
				{
					return false;
				}

				var value = this.value;

				return (value >= Int64.MinValue) && (value <= Int64.MaxValue) && unchecked((Int64)value) == value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue is a Number.
		/// </summary>
		public bool IsNumber
		{
			get
			{
				return this.Type == JsonValueType.Number;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue is a String.
		/// </summary>
		public bool IsString
		{
			get
			{
				return this.Type == JsonValueType.String;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue is a JsonObject.
		/// </summary>
		public bool IsJsonObject
		{
			get
			{
				return this.Type == JsonValueType.Object;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue is a JsonArray.
		/// </summary>
		public bool IsJsonArray
		{
			get
			{
				return this.Type == JsonValueType.Array;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this JsonValue represents a DateTime.
		/// </summary>
		public bool IsDateTime
		{
			get
			{
				return this.AsDateTime != null;
			}
		}

		/// <summary>
		/// Gets this value as a Boolean type.
		/// </summary>
		public bool AsBoolean
		{
			get
			{
				switch (this.Type)
				{
					case JsonValueType.Boolean:
						return (this.value == 1);

					case JsonValueType.Number:
						return (this.value != 0);

					case JsonValueType.String:
						return ((string)this.reference != "");

					case JsonValueType.Object:
					case JsonValueType.Array:
						return true;

					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Gets this value as an Integer type.
		/// </summary>
		public int AsInteger
		{
			get
			{
				var value = this.AsNumber;

				// Prevent overflow if the value doesn't fit.
				if (value >= int.MaxValue)
				{
					return int.MaxValue;
				}
				if (value <= int.MinValue)
				{
					return int.MinValue;
				}

				return (int)value;
			}
		}

		/// <summary>
		/// Gets this value as a Number type.
		/// </summary>
		public double AsNumber
		{
			get
			{
				switch (this.Type)
				{
					case JsonValueType.Boolean:
						return (this.value == 1)
							? 1
							: 0;

					case JsonValueType.Number:
						return this.value;

					case JsonValueType.String:
						double number;
						if (double.TryParse((string)this.reference, out number))
						{
							return number;
						}
						else
						{
							goto default;
						}

					default:
						return 0;
				}
			}
		}

		public long AsLong 
		{
			get
			{
				var value = this.AsNumber;

				// Prevent overflow if the value doesn't fit.
				if (value >= long.MaxValue)
				{
					return int.MaxValue;
				}
				if (value <= long.MinValue)
				{
					return int.MinValue;
				}

				return (long)value;
			}
		}

		public float AsNumberReal
		{
			get
			{
				switch (this.Type)
				{
					case JsonValueType.Boolean:
						return (this.value == 1)
							? 1
							: 0;

					case JsonValueType.Number:
						return (float)this.value;

					case JsonValueType.String:
						float number;
						if (float.TryParse((string)this.reference, out number))
						{
							return number;
						}
						else
						{
							goto default;
						}

					default:
						return 0;
				}
			}
		}

		/// <summary>
		/// Gets this value as a String type.
		/// </summary>
		public string AsString
		{
			get
			{
				return Type switch 
				{
					JsonValueType.Boolean => value == 1 ? "true" : "false",
					JsonValueType.Number => value.ToString(),
					JsonValueType.String => (string)reference,
					_ => null,
				};
			}
		}


		/// <summary>
		/// Gets this value as an JsonObject.
		/// </summary>
		public JsonObject AsJsonObject
		{
			get
			{
				return (this.IsJsonObject)
					? (JsonObject)this.reference
					: null;
			}
		}

		/// <summary>
		/// Gets this value as an JsonArray.
		/// </summary>
		public JsonArray AsJsonArray
		{
			get
			{
				return (this.IsJsonArray)
					? (JsonArray)this.reference
					: null;
			}
		}

		/// <summary>
		/// Gets this value as a system.DateTime.
		/// </summary>
		public DateTime? AsDateTime
		{
			get
			{
				if (this.IsString && DateTime.TryParse((string)this.reference, out DateTime value))
				{
					return value;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets this (inner) value as a System.object.
		/// </summary>
		public object AsObject
		{
			get
			{
				switch (this.Type)
				{
					case JsonValueType.Boolean:
					case JsonValueType.Number:
						return this.value;

					case JsonValueType.String:
					case JsonValueType.Object:
					case JsonValueType.Array:
						return this.reference;

					default:
						return null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown when this JsonValue is not a JsonObject.
		/// </exception>
		public JsonValue this[string key]
		{
			get
			{
				if (this.IsJsonObject)
				{
					return ((JsonObject)this.reference)[key];
				}
				else
				{
					throw new InvalidOperationException("This value does not represent a JsonObject.");
				}
			}
			set
			{
				if (this.IsJsonObject)
				{
					((JsonObject)this.reference)[key] = value;
				}
				else
				{
					throw new InvalidOperationException("This value does not represent a JsonObject.");
				}
			}
		}

		/// <summary>
		/// Gets or sets the value at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the value to get or set.</param>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown when this JsonValue is not a JsonArray
		/// </exception>
		public JsonValue this[int index]
		{
			get
			{
				if (this.IsJsonArray)
				{
					return ((JsonArray)this.reference)[index];
				}
				else
				{
					throw new InvalidOperationException("This value does not represent a JsonArray.");
				}
			}
			set
			{
				if (this.IsJsonArray)
				{
					((JsonArray)this.reference)[index] = value;
				}
				else
				{
					throw new InvalidOperationException("This value does not represent a JsonArray.");
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the JsonValue struct.
		/// </summary>
		/// <param name="type">The Json type of the JsonValue.</param>
		/// <param name="value">
		/// The internal value of the JsonValue.
		/// This is used when the Json type is Number or Boolean.
		/// </param>
		/// <param name="reference">
		/// The internal value reference of the JsonValue.
		/// This value is used when the Json type is String, JsonObject, or JsonArray.
		/// </param>
		private JsonValue(JsonValueType type, double value, object reference)
		{
			this.type      = type;
			this.value     = value;
			this.reference = reference;
		}

		public JsonValue(bool value)
		{
			this.reference = null;
			this.type = JsonValueType.Boolean;
			this.value = value ? 1 : 0;
			return;
		}
		public JsonValue(int value)
		{
			this.reference = null;
			this.type = JsonValueType.Number;
			numberType = JsonNumberType.Int;	
			this.value = value;
			return;
		}
		public JsonValue(long value)
		{
			this.reference = null;
			this.type = JsonValueType.Number;
			numberType = JsonNumberType.Long;	
			this.value = value;
			return;
		}
		public JsonValue(double value)
		{
			this.reference = null;
			this.type = JsonValueType.Number;
			numberType = JsonNumberType.Double;
			this.value = value;
			return;
		}
		public JsonValue(float value)
		{
			this.reference = null;
			this.type = JsonValueType.Number;
			numberType = JsonNumberType.Float;
			this.value = value;
			return;
		}
		public JsonValue(string value)
		{
			if (value != null)
			{
				this.value = default(double);
				this.type = JsonValueType.String;
				this.reference = value;
				return;
			}
			this = JsonValue.Null;
		}
		public JsonValue(JsonObject value)
		{
			if (value != null)
			{
				this.value = default(double);
				this.type = JsonValueType.Object;
				this.reference = value;
			}
			else
			{
				this = JsonValue.Null;
			}
		}
		public JsonValue(JsonArray value)
		{
			if (value != null)
			{
				this.value = default(double);
				this.type = JsonValueType.Array;
				this.reference = value;
				return;
			}
			this = JsonValue.Null;
		}

        public static implicit operator JsonValue(bool value) => new JsonValue(value);
        public static implicit operator JsonValue(int value) => new JsonValue(value);
        public static implicit operator JsonValue(long value) => new JsonValue(value);
        public static implicit operator JsonValue(double value) => new JsonValue(value);
        public static implicit operator JsonValue(float value) => new JsonValue(value);
        public static implicit operator JsonValue(string value) => new JsonValue(value);
        public static implicit operator JsonValue(JsonObject value) => new JsonValue(value);
        public static implicit operator JsonValue(JsonArray value) => new JsonValue(value);
        public static implicit operator JsonValue(DateTime? value)
		{
			if (value == null)
			{
				return JsonValue.Null;
			}

			return new JsonValue(value.Value.ToString("o"));
		}
		public static implicit operator int(JsonValue jsonValue)
		{
			if (jsonValue.IsInteger)
			{
				return jsonValue.AsInteger;
			}
			else
			{
				return 0;
			}
		}
		public static implicit operator long(JsonValue jsonValue)
		{
			if (jsonValue.IsInteger)
			{
				return jsonValue.AsLong;
			}
			else
			{
				return 0;
			}
		}
		public static implicit operator bool(JsonValue jsonValue)
		{
			if (jsonValue.IsBoolean)
			{
				return (jsonValue.value == 1);
			}
			else
			{
				return false;
			}
		}
		public static implicit operator double(JsonValue jsonValue)
		{
			if (jsonValue.IsNumber)
			{
				return jsonValue.value;
			}
			else
			{
				return double.NaN;
			}
		}
		public static implicit operator float(JsonValue jsonValue)
		{
			if (jsonValue.IsNumber)
			{
				return (float)jsonValue.value;
			}
			else
			{
				return float.NaN;
			}
		}
		public static implicit operator string(JsonValue jsonValue)
		{
			if (jsonValue.IsString || jsonValue.IsNull)
			{
				return jsonValue.reference as string;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Converts the given JsonValue into a JsonObject.
		/// </summary>
		/// <param name="jsonValue">The JsonValue to be converted.</param>
		public static implicit operator JsonObject(JsonValue jsonValue)
		{
			if (jsonValue.IsJsonObject || jsonValue.IsNull)
			{
				return jsonValue.reference as JsonObject;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Converts the given JsonValue into a JsonArray.
		/// </summary>
		/// <param name="jsonValue">The JsonValue to be converted.</param>
		public static implicit operator JsonArray(JsonValue jsonValue)
		{
			if (jsonValue.IsJsonArray || jsonValue.IsNull)
			{
				return jsonValue.reference as JsonArray;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Converts the given JsonValue into a DateTime.
		/// </summary>
		/// <param name="jsonValue">The JsonValue to be converted.</param>
		public static implicit operator DateTime(JsonValue jsonValue)
		{
			var dateTime = jsonValue.AsDateTime;

			if (dateTime.HasValue)
			{
				return dateTime.Value;
			}
			else
			{
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// Converts the given JsonValue into a nullable DateTime.
		/// </summary>
		/// <param name="jsonValue">The JsonValue to be converted.</param>
		public static implicit operator DateTime?(JsonValue jsonValue)
		{
			if (jsonValue.IsDateTime || jsonValue.IsNull)
			{
				return jsonValue.AsDateTime;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a value indicating whether the two given JsonValues are equal.
		/// </summary>
		/// <param name="a">A JsonValue to compare.</param>
		/// <param name="b">A JsonValue to compare.</param>
		public static bool operator ==(JsonValue a, JsonValue b)
		{
			return (a.Type == b.Type)
				&& (a.value == b.value)
				&& Equals(a.reference, b.reference);
		}

		/// <summary>
		/// Returns a value indicating whether the two given JsonValues are unequal.
		/// </summary>
		/// <param name="a">A JsonValue to compare.</param>
		/// <param name="b">A JsonValue to compare.</param>
		public static bool operator !=(JsonValue a, JsonValue b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Returns a JsonValue by parsing the given string.
		/// </summary>
		/// <param name="text">The JSON-formatted string to be parsed.</param>
		public static JsonValue Parse(string text)
		{
			return JsonTextReader.Parse(text);
		}

		/// <summary>
		/// Returns a value indicating whether this JsonValue is equal to the given object.
		/// </summary>
		/// <param name="obj">The object to test.</param>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return this.IsNull;
			}

			var jsonValue = obj as JsonValue?;

			if (jsonValue.HasValue)
			{
				return (this == jsonValue.Value);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a hash code for this JsonValue.
		/// </summary>
		public override int GetHashCode()
		{
			if (this.IsNull)
			{
				return this.Type.GetHashCode();
			}
			else
			{
				return this.Type.GetHashCode()
					^ this.value.GetHashCode()
					^ EqualityComparer<object>.Default.GetHashCode(this.reference);
			}
		}

		/// <summary>
		/// Returns a JSON string representing the state of the object.
		/// </summary>
		/// <remarks>
		/// The resulting string is safe to be inserted as is into dynamically
		/// generated JavaScript or JSON code.
		/// </remarks>
		public override string ToString()
		{
			return ToString(false);
		}

		/// <summary>
		/// Returns a JSON string representing the state of the object.
		/// </summary>
		/// <remarks>
		/// The resulting string is safe to be inserted as is into dynamically
		/// generated JavaScript or JSON code.
		/// </remarks>
		/// <param name="pretty">
		/// Indicates whether the resulting string should be formatted for human-readability.
		/// </param>
		public string ToString(bool pretty)
		{
			return JsonTextWriter.Serialize(this, pretty);
		}

		private class JsonValueDebugView
		{
			private JsonValue jsonValue;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public JsonObject ObjectView
			{
				get
				{
					if (jsonValue.IsJsonObject)
					{
						return (JsonObject)jsonValue.reference;
					}
					else
					{
						return null;
					}
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public JsonArray ArrayView
			{
				get
				{
					if (jsonValue.IsJsonArray)
					{
						return (JsonArray)jsonValue.reference;
					}
					else
					{
						return null;
					}
				}
			}

			public JsonValueType Type
			{
				get
				{
					return jsonValue.Type;
				}
			}

			public object Value
			{
				get
				{
					if (jsonValue.IsJsonObject)
					{
						return (JsonObject)jsonValue.reference;
					}
					else if (jsonValue.IsJsonArray)
					{
						return (JsonArray)jsonValue.reference;
					}
					else
					{
						return jsonValue;
					}
				}
			}

			public JsonValueDebugView(JsonValue jsonValue)
			{
				this.jsonValue = jsonValue;
			}
		}
	}
}
