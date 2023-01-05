using System;

namespace LightJson.Serialization;

public sealed class JDictionaryAttribute : Attribute 
{
    public bool Dynamic { get; set; }
}