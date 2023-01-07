//Source Generated Code
using System;
using LightJson;

namespace HelloWorld;
partial class Texture : IJsonDeserializable
{
public void Deserialize(JsonObject obj)
{
X = obj["x"];
Y = obj["Y"];
Width = obj["Width"];
Height = obj["Height"];
Field = obj["Field"];
FieldArray = obj["FieldArray"].ConvertToArrayInt();
}
}
