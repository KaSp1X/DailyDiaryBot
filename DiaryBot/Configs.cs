using System;
using System.Collections.Generic;

namespace DiaryBot
{
    public class Configs
    {
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

        private const string _path = "config.json";

        private static Configs? instance;

        public static Configs Instance
        {
            get
            {
                if (instance == null)
                {
                    List<Config> ex = Serializer.Load<List<Config>>(_path) ?? new();
                    instance = new();
                    instance.configsList = ex;
                    instance.SelectedConfig = instance.configsList.Count > 0 ? instance.configsList[^1] : default;
                    if (string.IsNullOrWhiteSpace(instance.SelectedConfig.Token))
                        Error.Instance.Message = "Fill fields in config, save and restart the app";
                }
                return instance;
            }
        }

        private Configs() { }

        private List<Config> configsList;

        public List<Config> ConfigsList { get { return configsList; } set { configsList = value; } }

        public Config SelectedConfig { get; set; }

        public static void AddConfig(Config newConfig)
        {
            Instance.configsList.Add(newConfig);
            Instance.SelectedConfig = Instance.configsList[^1];
            Serializer.Save(_path, Instance.configsList);
        }

        public static void UpdateConfig(Config selectedConfig, Config updatedConfig)
        {
            int index = Instance.configsList.IndexOf(selectedConfig);
            if (index != -1)
            {
                Instance.configsList[index] = updatedConfig;
                Instance.SelectedConfig = updatedConfig;
                Serializer.Save(_path, Instance.configsList);
            }
        }

        public static void RemoveConfig(Config selectedConfig)
        {
            int index = Instance.configsList.IndexOf(selectedConfig);
            if (index != -1)
            {
                Instance.configsList.RemoveAt(index);
                Instance.SelectedConfig = Instance.configsList.Count > 0 ? Instance.configsList[^1] : default;
                Serializer.Save(_path, Instance.configsList);
            }
        }
    }
}