using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiaryBot
{
    public class Error : Singleton<Error>, INotifyPropertyChanged
    {
        private string _message = "";

        public string Message
        {
            get => _message;
            set
            {
                _message = FormatMessage(value);
                NotifyPropertyChanged(nameof(Message));
            }
        }

        private Error() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
