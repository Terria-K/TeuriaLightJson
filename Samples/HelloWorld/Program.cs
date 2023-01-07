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
Console.ReadLine();