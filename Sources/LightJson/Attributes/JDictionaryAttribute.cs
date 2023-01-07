using System;

namespace LightJson.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class JDictionaryAttribute : Attribute 
{
    public bool Dynamic { get; set; }
}