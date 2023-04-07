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
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
                new Task(async () => { Bot.Instance.LastMessage = await Bot.Instance.SendMessage(text); }).Start();
        }

        private void EditLastMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
                new Task(async () => { Bot.Instance.LastMessage = await Bot.Instance.EditLastMessage(text); }).Start();
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Serializer.Save(Config.ConfigPath, Config.Instance);
        }
    }
}
