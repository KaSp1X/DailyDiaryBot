using System.Text.Json;
using System.IO;

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
                catch
                {
                    // Ignoring JSONSerializerException as upon invalid format of file it will be recreated.
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
