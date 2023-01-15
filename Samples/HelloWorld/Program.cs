using System;
using HelloWorld;
using LightJson;
using LightJson.Serialization;

var json = new JsonObject 
{
    ["number"] = 3,
    ["text"] = "Hello",
    ["frames"] = new JsonArray { 1, 2, 4 },
    ["map"] = new JsonArray {
        new JsonArray { 0, 1, 0 },
        new JsonArray { 0, 1, 0 },
        new JsonArray { 0, 1, 0 },
    },
    ["texture"] = new JsonObject {
        ["x"] = 4.2,
        ["y"] = 6.4,
        ["width"] = 12,
        ["height"] = 12
    },
    ["positions"] = new JsonArray { 3.5f, 4.2f, 2.1f }
}.ToString(pretty: true);


Console.WriteLine(json);
var reader = JsonTextReader.Parse(json);
var template = JsonConvert.Deserialize<Template>(reader);
Console.WriteLine(template);

var reader2 = JsonTextReader.Parse(new JsonObject 
{
   ["number"] = 3,
   ["text"] = "Hello" 
}.ToString());
var test = JsonConvert.Deserialize<Test>(reader2);
Console.WriteLine(test.Text);

var test2 = new Test() 
{
    Text = "Hello World",
    Number = 3,
    Texture = new () {
        X = 2,
        Y = 4,
        Width = 20,
        Height = 40
    },
    ArrayInt = new[] { 3, 2, 4},
    ArrayBool2D = new [,] { 
        { true, false, true }, 
        { true, false, true },  
        { true, false, false }, 
        { false, false, false }, 
    },
    Textures = new Texture[4] {
        new () { X = 2, Y = 4, Width = 20, Height = 40 },
        new () { X = 2, Y = 4, Width = 20, Height = 40 },
        new () { X = 2, Y = 4, Width = 20, Height = 40 },
        new () { X = 2, Y = 4, Width = 20, Height = 40 },
    },
    Dict = new () { 
        { "Player", new Texture() { X = 3, Y= 2, Width = 20, Height = 40} },
        { "Enemy", new Texture() { X = 4, Y= 10, Width = 20, Height = 40} }
    },
    DynamicDict = new() {
        {"Num", 2},
        {"Text", "Py"}
    }
};

var obj = test2.Serialize();

JsonTextWriter.Serialize(obj, "Output/Serialized.json", true);
JsonBinaryWriter.Serialize(obj, "Output/Serialized.bin");
var fromBinary = JsonBinaryReader.Parse("Output/Serialized.bin");
var reader3 = JsonConvert.Deserialize<Test>(fromBinary);

// Use Debugger
Console.WriteLine(reader3);