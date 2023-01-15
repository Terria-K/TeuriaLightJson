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

#region Array2D
    public static JsonArray ConvertToJsonArray2D<T>(this T[,] array) 
    where T : IJsonSerializable
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        for (int i = 0; i < array.GetLength(0); i++) 
        {
            var jsonArray2 = new JsonArray();
            for (int j = 0; j < array.GetLength(1); j++) 
            {
                jsonArray2.Add(array[i, j].Serialize());
            }
            jsonArray.Add(jsonArray2);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray2D(this int[,] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        for (int i = 0; i < array.GetLength(0); i++) 
        {
            var jsonArray2 = new JsonArray();
            for (int j = 0; j < array.GetLength(1); j++) 
            {
                jsonArray2.Add(array[i, j]);
            }
            jsonArray.Add(jsonArray2);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray2D(this bool[,] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        for (int i = 0; i < array.GetLength(0); i++) 
        {
            var jsonArray2 = new JsonArray();
            for (int j = 0; j < array.GetLength(1); j++) 
            {
                jsonArray2.Add(array[i, j]);
            }
            jsonArray.Add(jsonArray2);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray2D(this string[,] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        for (int i = 0; i < array.GetLength(0); i++) 
        {
            var jsonArray2 = new JsonArray();
            for (int j = 0; j < array.GetLength(1); j++) 
            {
                jsonArray2.Add(array[i, j]);
            }
            jsonArray.Add(jsonArray2);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray2D(this char[,] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        for (int i = 0; i < array.GetLength(0); i++) 
        {
            var jsonArray2 = new JsonArray();
            for (int j = 0; j < array.GetLength(1); j++) 
            {
                jsonArray2.Add(array[i, j]);
            }
            jsonArray.Add(jsonArray2);
        }
        return jsonArray;
    }

    public static JsonArray ConvertToJsonArray2D(this float[,] array) 
    {
        if (array == null)
            return JsonValue.Null;
        var jsonArray = new JsonArray();
        for (int i = 0; i < array.GetLength(0); i++) 
        {
            var jsonArray2 = new JsonArray();
            for (int j = 0; j < array.GetLength(1); j++) 
            {
                jsonArray2.Add(array[i, j]);
            }
            jsonArray.Add(jsonArray2);
        }
        return jsonArray;
    }


#endregion

    public static JsonObject ToJsonObject<T>(this Dictionary<string, T> value)
    where T : IJsonSerializable
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value.Serialize());
        }
        return jsonObj;
    }

    public static JsonObject ToJsonObject(this Dictionary<string, JsonValue> value)
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value);
        }
        return jsonObj;
    }

    public static JsonObject ToJsonObject<T>(this Dictionary<string, int> value)
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value);
        }
        return jsonObj;
    }

    public static JsonObject ToJsonObject<T>(this Dictionary<string, bool> value)
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value);
        }
        return jsonObj;
    }

    public static JsonObject ToJsonObject<T>(this Dictionary<string, string> value)
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value);
        }
        return jsonObj;
    }

    public static JsonObject ToJsonObject<T>(this Dictionary<string, char> value)
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value);
        }
        return jsonObj;
    }

    public static JsonObject ToJsonObject<T>(this Dictionary<string, float> value)
    {
        if (value == null) 
            return null;
        var jsonObj = new JsonObject();
        foreach (var cObj in value) 
        {
            jsonObj.Add(cObj.Key, cObj.Value);
        }
        return jsonObj;
    }
}