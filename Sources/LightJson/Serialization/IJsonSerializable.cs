namespace LightJson;

public interface IJsonDeserializable 
{
    void Deserialize(JsonObject obj);
}

public interface IJsonSerializable 
{
    JsonObject Serialize();
}