using System;

namespace LightJson.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class JArrayAttribute : Attribute 
{
    public string SupportedTypes { get; set; }

    public JArrayAttribute(SupportedTypes type) 
    {
        SupportedTypes = type.ToString();
    }
}

public enum SupportedTypes 
{
    Int, Boolean, Float, Double, Char, String, Other,
    Int2D, Boolean2D, Float2D, Double2D, Char2D, String2D, Other2D,
    ListInt, ListBoolean, ListFloat, ListDouble, ListString, ListOther
}
