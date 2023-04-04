using System.Threading.Tasks;
using System.Windows;

namespace DiaryBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TokenTextBox.Text = Static.Config.token;
            ChatIdTextBox.Text = Static.Config.chatId;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            new Task(async () => { await Bot.Instance.SendMessage(message); }).Start();
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Serializer.Save(Static.ConfigPath, Static.Config);
        }

        private void TokenTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Static.Config.token = TokenTextBox.Text;
        }

        private void ChatIdTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Static.Config.chatId = ChatIdTextBox.Text;
        }

    }
}
