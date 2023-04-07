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
                    dynamic? d = Serializer.Load<ExpandoObject>(ConfigPath);
                    _instance = new Config();

                    if (d is not null)
                    {
                        _instance.Token = d.Token is not null ? d.Token.ToString() : "";
                        _instance.ChatId = d.ChatId is not null ? d.ChatId.ToString() : "";
                    }

                    if (_instance == null)
                        Serializer.Save(ConfigPath, _instance);

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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}