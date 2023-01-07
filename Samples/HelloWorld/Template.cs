using System;
using System.Collections.Generic;
using LightJson;
using LightJson.Serialization;

namespace HelloWorld;



[JsonSerializable]
public partial class Test 
{
    [JName("text")]
    public string Text { get; set; }
    [JName("numbers")]
    public int Number { get; set; }
    [JName("texture")]
    public Texture Texture { get; set; }

    [JName("array")]
    [JArray(SupportedTypes.Int)]
    public int[] ArrayInt { get; set; }

    [JName("textures")]
    [JArray(SupportedTypes.Other)]
    public Texture[] Textures { get; set; }


    [JName("array2D")]
    [JArray(SupportedTypes.Boolean2D)]
    public bool[,] ArrayBool2D { get; set; }

    [JName("dict")]
    [JDictionary()]
    public Dictionary<string, Texture> Dict { get; set; }

    [JName("dynamicDict")]
    [JDictionary(Dynamic = true)]
    public Dictionary<string, JsonValue> DynamicDict { get; set; }

    public Template Template { get; set; }
}

// Manual Attaching
public record Template : IJsonDeserializable
{
    
    public int Number { get; set; }
    public string Text { get; set; }
    public int[] Frames { get; set; }
    public int[,] Map { get; set; }
    public Texture Texture { get; set; }
    public float[] Positions { get; set; }

    public void Deserialize(JsonObject obj)
    {
        Number = obj["number"];
        Text = obj["text"];
        Frames = obj["frames"].ConvertToArrayInt();
        Console.WriteLine();
        var jsonMap = obj["map"].AsJsonArray;
        var jsonMap2 = jsonMap[0].AsJsonArray;
        Map ??= new int[jsonMap.Count, jsonMap2.Count];
        for (int i = 0; i < jsonMap.Count; i++) 
        {
            for (int j = 0; j < jsonMap2.Count; j++) 
            {

                Map[i, j] = jsonMap2[j];
                Console.Write(Map[i, j] + ", ");
            }
            Console.WriteLine();
        }
        Texture = JsonConvert.Deserialize<Texture>(obj["texture"]);
        Positions = obj["positions"].ConvertToArrayFloat();
    }
}

[JsonSerializable]
public partial class Texture 
{
    [JName("x")]
    public float X { get; set; }
    public float Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    [JObject]
    public int Field;
    public int UnserializedField;
    [JObject]
    [JArray(SupportedTypes.Int)]
    public int[] FieldArray;
}