using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace DiaryBot
{
    public class Config : INotifyPropertyChanged
    {
        public const string ConfigPath = "config.json";

        private static Config? _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    dynamic? dynConfig = Serializer.Load<ExpandoObject>(ConfigPath);
                    IDictionary<string, object> dictConfig = dynConfig as IDictionary<string, object> ?? new Dictionary<string, object>();

                    if (dictConfig.Count == 0)
                    {
                        _instance = new Config();
                        Serializer.Save(ConfigPath, _instance);
                    }
                    else
                    {
                        _instance = new Config
                        {
                            Token = dictConfig.TryGetValue("Token", out object tok) ? tok.ToString() : "",
                            ChatId = dictConfig.TryGetValue("ChatId", out object chId) ? chId.ToString() : "",
                            ReplyMessageId = dictConfig.TryGetValue("ReplyMessageId", out object obMId) ? (Int32.TryParse(obMId.ToString(), out int intMId) ? intMId : null) : null
                        };
                    }

                    if (string.IsNullOrWhiteSpace(_instance?.Token) || string.IsNullOrWhiteSpace(_instance?.ChatId))
                        Error.Instance.Message = "Fill fields in config, save and restart the app";
                }
                return _instance;
            }
        }

        private Config() { }

        private string _token = "";

        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                NotifyPropertyChanged(nameof(Token));
            }
        }

        private string _chatId = "";

        public string ChatId
        {
            get
            {
                return _chatId;
            }
            set
            {
                _chatId = value;
                NotifyPropertyChanged(nameof(ChatId));
            }
        }

        private int? _replyMessageId = null;

        public int? ReplyMessageId
        {
            get
            {
                return _replyMessageId;
            }
            set
            {
                _replyMessageId = value;
                NotifyPropertyChanged(nameof(ReplyMessageId));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}