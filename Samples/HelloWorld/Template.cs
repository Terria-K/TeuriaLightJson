using System;
using LightJson;

public record Template : IJsonSerializable
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

public record Texture : IJsonSerializable
{
    public float X;
    public float Y;
    public int Width;
    public int Height;

    public void Deserialize(JsonObject obj)
    {
        X = (float)obj["x"].AsNumber;
        Y = (float)obj["y"].AsNumber;
        Width = obj["width"];
        Height = obj["height"];
    }

    public string Serialize()
    {
        throw new NotImplementedException();
    }
}