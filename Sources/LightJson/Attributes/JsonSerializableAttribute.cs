using System;
namespace LightJson.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class JsonSerializableAttribute : Attribute {}