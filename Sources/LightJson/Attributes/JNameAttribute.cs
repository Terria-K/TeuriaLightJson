using System;
namespace LightJson.Serialization;

public class JNameAttribute : Attribute 
{
    public string JsonName { get; set; }

    public JNameAttribute(string name) 
    {
        JsonName = name;
    }
}