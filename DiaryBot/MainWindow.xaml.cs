using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            UpdateRecentGrid();
        }

        private void UpdateRecentGrid()
        {
            Label recentLabel = RecentLabel;
            RecentGrid.Children.Clear();
            RecentGrid.Children.Add(recentLabel);
            for (int i = 0; i < Messages.Instance.MessagesList.Count; i++)
            {
                var button = new Button
                {
                    Margin = new(1.0),
                    Name = "recentMessage" + i,
                    Background = Brushes.LightGray,
                    Padding = new(2.0)
                };
                var textBlock = new TextBlock
                {
                    Text = Messages.Instance.MessagesList[i].Text,
                    TextWrapping = TextWrapping.Wrap
                };
                button.Content = textBlock;
                if (Messages.Instance[i]?.Id == Messages.Instance.PickedMessage?.Id)
                {
                    button.Background = Brushes.LightGreen;
                }
                button.Click += RecentButton_Click;
                RecentGrid.Children.Add(button);
                Grid.SetRow(button, i + 1);
            }
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.SendMessage(text);
                UpdateRecentGrid();
            }
        }

        private async void EditLastMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.EditPickedMessage(text);
                UpdateRecentGrid();
            }
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Serializer.Save(Config.Path, Config.Instance);
        }

        private void RecentButton_Click(object sender, RoutedEventArgs e)
        {
            Messages.Instance.PickedMessage = Messages.Instance[Grid.GetRow((Button)sender) - 1];
            MessageTextBox.Text = Messages.Instance.PickedMessage?.Text;
            foreach (UIElement obj in RecentGrid.Children)
            {
                if (obj is Button)
                {
                    if ((obj as Button) != sender)
                    {
                        (obj as Button).Background = Brushes.LightGray;
                    }
                    else
                    {
                        (obj as Button).Background = Brushes.LightGreen;
                    }
                }
            }
        }
    }
}
