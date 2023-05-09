using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiaryBot.Core
{
    public class StatusModel : Singleton<StatusModel>, INotifyPropertyChanged
    {
        private string _message = "";

        public string Message
        {
            get => _message;
            set
            {
                _message = value.ToStatus();
                NotifyPropertyChanged(nameof(Message));
            }
        }

        private StatusModel() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
