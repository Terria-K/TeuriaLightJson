using System;

namespace LightJson.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class JIgnoreAttribute : Attribute {}