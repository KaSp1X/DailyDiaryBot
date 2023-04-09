using System.Windows;
using System.Windows.Controls;

namespace DiaryBot
{
    /// <summary>
    /// Interaction logic for RecentMessages.xaml
    /// </summary>
    public partial class RecentMessages : Window
    {
        public RecentMessages()
        {
            InitializeComponent();
            for (int i = 0; i < Messages.Instance.MessagesList.Count; i++)
            {
                var button = new Button
                {
                    Margin = new(5.0),
                    Content = Messages.Instance.MessagesList[i].Text
                };
                button.Click += Button_Click;
                RecentMessagesGrid.Children.Add(button);
                Grid.SetRow(button, i);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Messages.Instance.PickedMessage = Messages.Instance[Grid.GetRow((Button)sender)];
        }
    }
}
