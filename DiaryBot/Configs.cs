using System.Collections.Generic;

namespace DiaryBot
{
    public sealed class Configs : Singleton<Configs>
    {
        private const string _path = "config.json";

        public struct Config
        {
            public string Name { get; set; }
            public string Token { get; set; }
            public string ChatId { get; set; }
            public int? ReplyMessageId { get; set; }

            public Config(string name, string token, string chatId, int? replyMessageId = null)
            {
                Name = name;
                Token = token;
                ChatId = chatId;
                ReplyMessageId = replyMessageId;
            }
        }

        private Configs()
        {
            ConfigsList = Serializer.Load<List<Config>>(_path) ?? new();
            SelectedConfig = ConfigsList.Count > 0 ? ConfigsList[^1] : default;
            if (string.IsNullOrWhiteSpace(SelectedConfig.Token))
                Error.Instance.Message = "Fill fields in config, save and restart the app";
        }

        public List<Config> ConfigsList { get; init; }

        public Config SelectedConfig { get; set; }

        public static void AddConfig(Config newConfig)
        {
            Instance.ConfigsList.Add(newConfig);
            Instance.SelectedConfig = Instance.ConfigsList[^1];
            Serializer.Save(_path, Instance.ConfigsList);
        }

        public static void UpdateConfig(Config selectedConfig, Config updatedConfig)
        {
            int index = Instance.ConfigsList.IndexOf(selectedConfig);
            if (index != -1)
            {
                Instance.ConfigsList[index] = updatedConfig;
                Instance.SelectedConfig = updatedConfig;
                Serializer.Save(_path, Instance.ConfigsList);
            }
        }

        public static void RemoveConfig(Config selectedConfig)
        {
            int index = Instance.ConfigsList.IndexOf(selectedConfig);
            if (index != -1)
            {
                Instance.ConfigsList.RemoveAt(index);
                Instance.SelectedConfig = Instance.ConfigsList.Count > 0 ? Instance.ConfigsList[^1] : default;
                Serializer.Save(_path, Instance.ConfigsList);
            }
        }
    }
}