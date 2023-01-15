using System;
using System.IO;
using System.Text;

namespace LightJson.Serialization;

public class JsonBinaryReader
{
    private BinaryReader reader;
    public object Value;
    public BinaryToken Token;
    private uint size;

    public JsonBinaryReader(BinaryReader reader) 
    {
        this.reader = reader;
    }

    public long Position => reader.BaseStream.Position;

    public static JsonValue Parse(string path) 
    {
        using var binaryReader = new BinaryReader(File.Open(path, FileMode.Open), Encoding.UTF8);
        var jsonBinaryReader = new JsonBinaryReader(binaryReader);
        return jsonBinaryReader.ReadObject();
    }

    public JsonValue ReadObject() 
    {
        var result = new JsonObject();
        var opened = false;

        while (Read() && Token != BinaryToken.ObjectLast) 
        {
            if (!opened && Token == BinaryToken.ObjectFirst) 
            {
                opened = true;
                continue;
            }

            if (Token != BinaryToken.ObjectKey)
                throw new Exception("Expected Object Key");

            var key = Value as string;

            result[key] = ReadValue();
        }
        return result;
    }

    public JsonValue ReadArray() 
    {
        var arr = new JsonArray();
        while (Read() && Token != BinaryToken.ArrayLast)
            arr.Add(GetValue());
        return arr;
    }

    public JsonValue ReadValue() 
    {
        Read();
        return GetValue();
    }

    private JsonValue GetValue() 
    {
        switch (Token) 
        {
            case BinaryToken.Null:
                return JsonValue.Null;
            case BinaryToken.Boolean:
                if (Value is bool Bool)
                    return Bool;
                break;
            case BinaryToken.String:
                if (Value is string String)
                    return String;
                break;
            case BinaryToken.ObjectFirst:
                return ReadObject();
            case BinaryToken.ArrayFirst:
                return ReadArray();
            case BinaryToken.Number:
                if (Value is int Int)
                    return Int;
                if (Value is long Long)
                    return Long;
                if (Value is float Float)
                    return Float;
                if (Value is double Double)
                    return Double;
                if (Value is char Char)
                    return Char;
                break;
            case BinaryToken.Raw:
                if (Value is byte[] Raw)
                    throw new NotImplementedException();
                break;
        }
        return JsonValue.Null;
    }

    public bool Read() 
    {
        if (Position < reader.BaseStream.Length) 
        {
            var token = (BinaryToken)reader.ReadByte();

            switch (token) 
            {
                case BinaryToken.Null:
                    Value = null;
                    Token = BinaryToken.Null;
                    break;
                case BinaryToken.Boolean:
                    Value = reader.ReadBoolean();
                    Token = BinaryToken.Boolean;
                    break;
                case BinaryToken.String:
                    Value = reader.ReadString();
                    Token = BinaryToken.String;
                    break;
                case BinaryToken.ObjectFirst:
                    ReadFirstContainer();
                    Token = BinaryToken.ObjectFirst;
                    break;
                case BinaryToken.ObjectKey:
                    Value = reader.ReadString();
                    Token = BinaryToken.ObjectKey;
                    break;
                case BinaryToken.ObjectLast:
                    Value = null;
                    Token = BinaryToken.ObjectLast;
                    break;
                case BinaryToken.ArrayFirst:
                    ReadFirstContainer();
                    Token = BinaryToken.ArrayFirst;
                    break;
                case BinaryToken.ArrayLast:
                    Value = null;
                    Token = BinaryToken.ArrayLast;
                    break;
                case BinaryToken.Int:
                    Value = reader.ReadInt32();
                    Token = BinaryToken.Number;
                    break;
                case BinaryToken.Float:
                    Value = reader.ReadSingle();
                    Token = BinaryToken.Number;
                    break;
                case BinaryToken.Double:
                case BinaryToken.Number:
                    Value = reader.ReadDouble();
                    Token = BinaryToken.Number;
                    break;
                case BinaryToken.Char:
                    Value = reader.ReadChar();
                    Token = BinaryToken.Number;
                    break;
                case BinaryToken.Long:
                    Value = reader.ReadInt64();
                    Token = BinaryToken.Number;
                    break;
                case BinaryToken.Raw:
                    var len = reader.ReadInt32();
                    Value = reader.ReadBytes(len);
                    Token = BinaryToken.Raw;
                    break;
            }

            return true;
        }
        return false;
    }
    
    private void ReadFirstContainer() 
    {
        size = reader.ReadUInt32();
        Value = null;
    }
}