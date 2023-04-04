using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System;

namespace DiaryBot
{
    public static class Serializer
    {
        public static T? Load<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using FileStream stream = File.OpenRead(filePath);
                try
                {
                    return JsonSerializer.Deserialize<T>(stream);
                }
                catch(JsonException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally { stream.Close(); }
            }
            return default(T);
        }

        public static void Save<T>(string filePath, T obj)
        {
            using FileStream stream = File.Open(filePath, FileMode.Create);
            JsonSerializer.Serialize<T>(stream, obj);
        }
    }
}
