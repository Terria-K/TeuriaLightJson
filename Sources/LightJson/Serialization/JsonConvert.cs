using LightJson.Serialization;

namespace LightJson;

public class JsonConvert 
{
    public static T DeserializeFromFile<T>(string path) 
    where T : IJsonDeserializable, new()
    {
        var converter = JsonReader.ParseFile(path);
        return Deserialize<T>(converter.AsJsonObject);
    }
    public static T Deserialize<T>(JsonObject jsObj)
    where T : IJsonDeserializable, new() 
    {
        var obj = new T();
        obj.Deserialize(jsObj);
        return obj;
    }

    public static string Serialize(IJsonSerializable serializable) 
    {
        return serializable.Serialize();
    }
}