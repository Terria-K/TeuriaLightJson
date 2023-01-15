using LightJson.Serialization;

namespace LightJson;

public class JsonConvert 
{
    public static T DeserializeFromFile<T>(string path) 
    where T : IJsonDeserializable, new()
    {
        var converter = JsonTextReader.ParseFile(path);
        return Deserialize<T>(converter.AsJsonObject);
    }
    public static JsonValue DeserializeFromFile(string path) 
    {
        var converter = JsonTextReader.ParseFile(path);
        return converter;
    }

    public static T DeserializeFromFileBinary<T>(string path) 
    where T : IJsonDeserializable, new()
    {
        var converter = JsonBinaryReader.Parse(path);
        return Deserialize<T>(converter.AsJsonObject);
    }
    public static T Deserialize<T>(JsonObject jsObj)
    where T : IJsonDeserializable, new() 
    {
        var obj = new T();
        obj.Deserialize(jsObj);
        return obj;
    }

    public static T Deserialize<T>(JsonValue jsObj)
    where T : IJsonDeserializable, new() 
    {
        var obj = new T();
        obj.Deserialize(jsObj);
        return obj;
    }

    public static string Serialize(IJsonSerializable serializable, bool pretty = false) 
    {
        return serializable.Serialize().ToString(pretty);
    }
}