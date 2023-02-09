using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static LightJson.Serialization.JsonSerializationException;

namespace LightJson.Serialization;


public class JsonBinaryWriter : JsonWriter
{
    private Stack<long> positionStack = new Stack<long>();

    public BinaryWriter InnerWriter;

    public JsonBinaryWriter(BinaryWriter writer) 
    {
        InnerWriter = writer;
    }

    public override void WriteEntry(JsonValue value)
    {
        switch (value.Type) 
        {
            case JsonValueType.Null:
                WriteNull();
                break;
            case JsonValueType.Boolean:
                WriteValue(value.AsBoolean);
                break;
            case JsonValueType.Number:
                switch (value.NumberType) 
                {
                    case JsonNumberType.Int:
                        WriteValue(value.AsInteger);
                        break;
                    case JsonNumberType.Double:
                        WriteValue(value.AsNumber);
                        break;
                    case JsonNumberType.Float:
                        WriteValue(value.AsNumberReal);
                        break;
                    case JsonNumberType.Long:  
                        WriteValue(value.AsLong);
                        break;
                }
                break;
            case JsonValueType.String:
                WriteValue(value.AsString);
                break;

            case JsonValueType.Object:
                Render((JsonObject)value);
                break;

            case JsonValueType.Array:
                Render((JsonArray)value);
                break;

            default:
                throw new JsonSerializationException(ErrorType.InvalidValueType);
        }
    }

    public override void Render(JsonArray value)
    {
        WriteArrayFirst();
        using var enumerator = value.GetEnumerator();
        var next = enumerator.MoveNext();
        while (next) 
        {
            WriteEntry(enumerator.Current);
            next = enumerator.MoveNext();
        }
        WriteArrayLast();
    }

    public override void Render(JsonObject value)
    {
        WriteObjectFirst();
        using var enumerator = value.GetEnumerator();
        var next = enumerator.MoveNext();
        while (next) 
        {
            WriteKey(enumerator.Current.Key);
            WriteEntry(enumerator.Current.Value);
            next = enumerator.MoveNext();
        }
        WriteObjectLast();
    }

    public void WriteOnContainerBegin(BinaryToken token) 
    {
        InnerWriter.Write((byte)token);
        positionStack.Push(InnerWriter.BaseStream.Position);
        InnerWriter.Write((uint)0);
    }

    public void WriteOnContainerEnd(BinaryToken token) 
    {
        InnerWriter.Write((byte)token);
        var current = InnerWriter.BaseStream.Position;
        var offset = positionStack.Pop();

        InnerWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
        InnerWriter.Write((uint)(current - offset - 4));
        InnerWriter.BaseStream.Seek(current, SeekOrigin.Begin);
    }

    public void WriteObjectFirst()
    {
        WriteOnContainerBegin(BinaryToken.ObjectFirst);
    }

    public void WriteObjectLast()
    {
        WriteOnContainerEnd(BinaryToken.ObjectLast);
    }

    public void WriteArrayFirst()
    {
        WriteOnContainerBegin(BinaryToken.ArrayFirst);
    }

    public void WriteArrayLast()
    {
        WriteOnContainerEnd(BinaryToken.ArrayLast);
    }

    public void WriteNull() 
    {
        InnerWriter.Write((byte)BinaryToken.Null);
    }

    public void WriteKey(string name) 
    {
        InnerWriter.Write((byte)BinaryToken.ObjectKey);
        InnerWriter.Write(name);
    }

    public void WriteValue(bool value) 
    {
        InnerWriter.Write((byte)BinaryToken.Boolean);
        InnerWriter.Write(value);
    }

    public void WriteValue(string value) 
    {
        InnerWriter.Write((byte)BinaryToken.String);
        InnerWriter.Write(value ?? "");
    }

    public void WriteValue(int value) 
    {
        InnerWriter.Write((byte)BinaryToken.Int);
        InnerWriter.Write(value);
    }

    public void WriteValue(long value) 
    {
        InnerWriter.Write((byte)BinaryToken.Long);
        InnerWriter.Write(value);
    }

    public void WriteValue(float value) 
    {
        InnerWriter.Write((byte)BinaryToken.Float);
        InnerWriter.Write(value);
    }

    public void WriteValue(double value) 
    {
        InnerWriter.Write((byte)BinaryToken.Double);
        InnerWriter.Write(value);
    }

    public void Write(char value) 
    {
        InnerWriter.Write((byte)BinaryToken.Char);
        InnerWriter.Write(value);
    }

    public void Write(ReadOnlySpan<byte> value) 
    {
        InnerWriter.Write((byte)BinaryToken.Raw);
        InnerWriter.Write(value);
    }

    public static void Serialize(JsonValue value, string path)
    {
        using var binaryWriter = new BinaryWriter(File.Create(path), Encoding.UTF8);
        var jsonWriter = new JsonBinaryWriter(binaryWriter);

        jsonWriter.WriteEntry(value);
    }

    public static byte[] Serialize(JsonValue value) 
    {
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
        var jsonWriter = new JsonBinaryWriter(binaryWriter);

        jsonWriter.WriteEntry(value);
        return memoryStream.GetBuffer();
    }
}


public enum BinaryToken : byte
{
    Null,

    Boolean,
    String,
    Number,

    ObjectFirst,
    ObjectLast,
    ObjectKey,
    ArrayFirst,
    ArrayLast,

    Int,
    Float,
    Double,
    Char,
    Long,

    Raw = 32
}