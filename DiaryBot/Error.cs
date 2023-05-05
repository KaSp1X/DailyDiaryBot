using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiaryBot
{
    public class Error : INotifyPropertyChanged
    {
        private string _message = "";

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = FormatMessage(value);
                NotifyPropertyChanged(nameof(Message));
            }
        }

        private static Error? _instance;

        public static Error Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Error();
                }
                return _instance;
            }
        }

        private Error() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static string FormatMessage(string message) => message switch
        {
            "Exception during making request" => "No connection to the server. Check your network connection and try again!",
            "Unauthorized" => "Your token is invalid. Change in configs and restart the app.",
            "Not Found" => "Your token is invalid. Change in configs and restart the app.",
            "Bad Request: chat not found" => "Your chat id is invalid. Check your configs is it trully correct in there.",
            _ => $"{message}!"
        };
    }
}
