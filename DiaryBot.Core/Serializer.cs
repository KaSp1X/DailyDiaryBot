using System.Text.Json;

namespace DiaryBot.Core
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
                finally { stream.Close(); }
            }
            return default;
        }

        public static void Save<T>(string filePath, T obj, JsonSerializerOptions? options = null)
        {
            using FileStream stream = File.Open(filePath, FileMode.Create);
            JsonSerializer.Serialize<T>(stream, obj, options);
        }
    }
}
