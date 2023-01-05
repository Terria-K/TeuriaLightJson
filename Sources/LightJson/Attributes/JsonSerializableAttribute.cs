using System;
namespace LightJson.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class JsonSerializableAttribute : Attribute {}