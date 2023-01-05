using System;

namespace LightJson.Serialization;

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
}
