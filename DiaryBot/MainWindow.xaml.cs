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
                new Task(async () =>
                {
                    await Bot.Instance.SendMessage(text);
                }).Start();
        }

        private void EditLastMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
                new Task(async () =>
                {
                    await Bot.Instance.EditPickedMessage(text);
                }).Start();
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Serializer.Save(Config.Path, Config.Instance);
        }

        private void RecentButton_Click(object sender, RoutedEventArgs e)
        {
            RecentMessages recentMessages = new();
            recentMessages.Show();
        }
    }
}
