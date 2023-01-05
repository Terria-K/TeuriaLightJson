//Source Generated Code
using System;
using LightJson;

namespace HelloWorld;
public partial class Test : IJsonDeserializable
{
public void Deserialize(JsonObject obj)
{
Text = obj["text"];
Number = obj["numbers"];
Texture = obj["texture"].Convert<HelloWorld.Texture>();
ArrayInt = obj["array"].ConvertToArrayInt();
Textures = obj["textures"].ConvertToArray<HelloWorld.Texture>();
ArrayBool2D = obj["array2D"].ConvertToArrayBoolean2D();
Dict = obj["dict"].ToDictionary<HelloWorld.Texture>();
DynamicDict = obj["dynamicDict"].ToDictionary();
}
}
