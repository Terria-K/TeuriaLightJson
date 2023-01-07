using System.Collections.Generic;

namespace LightJson;

public static class JsonSerializeUtility 
{
#region Array
    public static JsonArray ConvertToJsonArray(this int[] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        var arrayCount = array.Length;
        for (int i = 0; i < arrayCount; i++) 
        {
            jsonArray.Add(array[i]);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this string[] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        var arrayCount = array.Length;
        for (int i = 0; i < arrayCount; i++) 
        {
            jsonArray.Add(array[i]);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this char[] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        var arrayCount = array.Length;
        for (int i = 0; i < arrayCount; i++) 
        {
            jsonArray.Add(array[i]);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this bool[] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        var arrayCount = array.Length;
        for (int i = 0; i < arrayCount; i++) 
        {
            jsonArray.Add(array[i]);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this float[] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        var arrayCount = array.Length;
        for (int i = 0; i < arrayCount; i++) 
        {
            jsonArray.Add(array[i]);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray<T>(this T[] array) 
    where T : IJsonSerializable
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        var arrayCount = array.Length;
        for (int i = 0; i < arrayCount; i++) 
        {
            jsonArray.Add(array[i].Serialize());
        }
        return jsonArray;
    }
#endregion
#region Array
    public static JsonArray ConvertToJsonArray(this List<int> array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        foreach (int v in array) 
        {
            jsonArray.Add(v);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this List<string> array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        foreach (string v in array) 
        {
            jsonArray.Add(v);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this List<char> array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        foreach (char v in array) 
        {
            jsonArray.Add(v);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this List<bool> array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        foreach (bool v in array) 
        {
            jsonArray.Add(v);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray(this List<float> array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        foreach (float v in array) 
        {
            jsonArray.Add(v);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray<T>(this List<T> array) 
    where T : IJsonSerializable
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        foreach (T v in array) 
        {
            jsonArray.Add(v.Serialize());
        }
        return jsonArray;
    }
#endregion

}