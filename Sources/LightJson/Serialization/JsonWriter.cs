namespace LightJson.Serialization;

// Work In Progress
public abstract class JsonWriter 
{
    public abstract void WriteEntry(JsonValue value);
    public abstract void Render(JsonArray value);
    public abstract void Render(JsonObject value);

    // protected void Render(JsonValue value) 
    // {
	// 		switch (value.Type)
	// 		{
	// 			case JsonValueType.Null:
	// 			case JsonValueType.Boolean:
	// 			case JsonValueType.Number:
	// 			case JsonValueType.String:
	// 				Write(value);
	// 				break;

	// 			case JsonValueType.Object:
	// 				Render((JsonObject)value);
	// 				break;

	// 			case JsonValueType.Array:
	// 				Render((JsonArray)value);
	// 				break;

	// 			default:
	// 				throw new JsonSerializationException(ErrorType.InvalidValueType);
	// 		}
    // }
}