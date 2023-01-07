using System;
namespace LightJson.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class JNameAttribute : Attribute 
{
    public string JsonName { get; set; }

    public JNameAttribute(string name) 
    {
        JsonName = name;
    }
}