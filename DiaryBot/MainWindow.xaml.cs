using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace DiaryBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateRecentGrid()
        {
            RecentGrid.Children.Clear();
            for (int i = 0; i < Messages.Instance.MessagesList.Count; i++)
            {
                var button = new Button
                {
                    Margin = new Thickness(5),
                    Name = "RecentMessage" + i,
                    Background = Brushes.LightGray,
                    Padding = new(5.0),
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch
                };
                string @fixed = Messages.Instance.MessagesList[i].Text.Replace("&", "&amp;").Replace("<", "&lt;");
                var xaml = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\">"
        + @fixed.ToXaml() + " </TextBlock>";
                var textBlock = XamlReader.Parse(xaml) as TextBlock;
                if (textBlock != null)
                {
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    button.Content = textBlock;
                }
                if (Messages.Instance[i]?.Id == Messages.Instance.PickedMessage?.Id)
                {
                    button.Background = Brushes.LightGoldenrodYellow;
                }
                button.Click += RecentButton_Click;
                RecentGrid.Children.Add(button);
                Grid.SetColumn(button, i % 2);
                Grid.SetRow(button, i / 2);
            }
        }

        private void UpdateProfilesPanel()
        {
            ProfilesStackPanel.Children.Clear();
            for (int i = Configs.Instance.ConfigsList.Count - 1; i >= 0; i--)
            {
                string name = Configs.Instance.ConfigsList[i].Name.Replace("&", "&amp;").Replace("<", "&lt;");
                string token = Configs.Instance.ConfigsList[i].Token.Replace("&", "&amp;").Replace("<", "&lt;");
                string chatId = Configs.Instance.ConfigsList[i].ChatId.Replace("&", "&amp;").Replace("<", "&lt;");
                string replyMessageId = Configs.Instance.ConfigsList[i].ReplyMessageId.ToString();
                if (string.IsNullOrEmpty(replyMessageId))
                    replyMessageId = "Empty";

                var xaml = $"""
                    <Button xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            Name="Profile_{i}"
                            xml:space="preserve"
                            Height="75"
                            Padding="5"
                            VerticalContentAlignment="Stretch"
                            HorizontalContentAlignment="Stretch">
                        <StackPanel>
                            <TextBlock FontStyle="Italic" FontWeight="Medium">{name}</TextBlock>
                            <TextBlock><TextBlock FontWeight="Medium">T: </TextBlock>{token.Split(':')[0]}</TextBlock>
                            <TextBlock><TextBlock FontWeight="Medium">CId: </TextBlock>{chatId}</TextBlock>
                            <TextBlock><TextBlock FontWeight="Medium">RMId: </TextBlock>{replyMessageId}</TextBlock>
                        </StackPanel>
                    </Button>
                    """;
                var button = XamlReader.Parse(xaml) as Button;
                if (button != null)
                {
                    button.Click += ProfileButton_Click;
                    if (Configs.Instance.ConfigsList[i].Equals(Configs.Instance.SelectedConfig))
                    {
                        button.Background = Brushes.LightGoldenrodYellow;
                        NameTextBox.Text = Configs.Instance.SelectedConfig.Name;
                        TokenTextBox.Text = Configs.Instance.SelectedConfig.Token;
                        ChatIdTextBox.Text = Configs.Instance.SelectedConfig.ChatId;
                        ReplyMessageIdTextBox.Text = replyMessageId;
                    }
                    ProfilesStackPanel.Children.Add(button);
                }
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in ProfilesStackPanel.Children)
            {
                Button button = obj as Button;
                if (button != sender)
                    button.Background = Brushes.LightGray;
                else
                {
                    button.Background = Brushes.LightGoldenrodYellow;
                    int.TryParse(button.Name.Split('_')[1], out int index);
                    if (index >= 0 && index < Configs.Instance.ConfigsList.Count)
                    {
                        Configs.Instance.SelectedConfig = Configs.Instance.ConfigsList[index];
                        NameTextBox.Text = Configs.Instance.SelectedConfig.Name;
                        TokenTextBox.Text = Configs.Instance.SelectedConfig.Token;
                        if (long.TryParse(Configs.Instance.SelectedConfig.Token.Split(':')[0], out long token) && token != Bot.GetToken())
                        {
                            Bot.Instance = null;
                        }
                        ChatIdTextBox.Text = Configs.Instance.SelectedConfig.ChatId;
                        string replyMessageId = Configs.Instance.SelectedConfig.ReplyMessageId.ToString();
                        if (string.IsNullOrEmpty(replyMessageId))
                            replyMessageId = "Empty";
                        ReplyMessageIdTextBox.Text = replyMessageId;
                    }
                }
            }
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.SendMessage(text);
            }
            else
                Error.Instance.Message = "Text to send can't be empty";
        }

        private async void EditLastMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.EditPickedMessage(text);
            }
            else
                Error.Instance.Message = "Edited version of text can't be empty";
        }

        private void RecentButton_Click(object sender, RoutedEventArgs e)
        {
            Messages.Instance.PickedMessage = Messages.Instance[(Grid.GetRow((Button)sender) == 1 ? 2 : 0) + Grid.GetColumn((Button)sender)];
            var textRange = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd);
            textRange.Text = Messages.Instance.PickedMessage?.Text;
            foreach (UIElement obj in RecentGrid.Children)
            {
                if (obj is Button)
                {
                    if ((obj as Button) != sender)
                        (obj as Button).Background = Brushes.LightGray;
                    else
                        (obj as Button).Background = Brushes.LightGoldenrodYellow;
                }
            }
        }

        private void MainWindowTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecentTab.IsSelected)
            {
                UpdateRecentGrid();
            }
            else if (SettingsTab.IsSelected)
            {
                UpdateProfilesPanel();
            }
        }

        private void MessageRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // to prevent overflow, we temporary remove event from our RichTextBox
            MessageRichTextBox.TextChanged -= MessageRichTextBox_TextChanged;

            var textRange = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd);

            // highlighting tags in richtextbox
            Regex reg = new Regex(@"\[\\[bivus][0]{0,1}\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var start = MessageRichTextBox.Document.ContentStart;
            while (start != null && start.CompareTo(MessageRichTextBox.Document.ContentEnd) < 0)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var match = reg.Match(start.GetTextInRun(LogicalDirection.Forward));

                    var range = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                    new TextRange(range.End, textRange.End).ClearAllProperties();
                    start = range.End;
                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }

            // rendering preview window
            string @fixed = textRange.Text.Replace("&", "&amp;").Replace("<", "&lt;");
            var xaml = """
                    <TextBlock xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    Padding="2" Margin="5" FontSize="10" TextWrapping="Wrap" xml:space="preserve">
                    """ + @fixed.ToXaml() + "</TextBlock>";
            PreviewWindow.Content = XamlReader.Parse(xaml) as TextBlock;


            // after everything is done return event to our RichTextBox
            MessageRichTextBox.TextChanged += MessageRichTextBox_TextChanged;
        }

        private void FormattingCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            if (MessageRichTextBox.Selection.Text.Length > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void EdittingCmd_CantExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        private void SetBoldCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(FormattingTag.Bold);

        private void SetItalicCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(FormattingTag.Italic);

        private void SetUnderlineCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(FormattingTag.Underline);

        private void SetStrikethroughCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(FormattingTag.Strikethrough);

        private void SetSpoilerCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(FormattingTag.Spoiler);

        private void FormatText(string tag)
        {
            var selectedTextRange = new TextRange(MessageRichTextBox.Selection.Start, MessageRichTextBox.Selection.End);

            string formatedMessage = FormattingTag.Insert(selectedTextRange, tag);
            if (!string.IsNullOrWhiteSpace(formatedMessage))
            {
                MessageRichTextBox.Selection.Text = formatedMessage;
            }
        }

        private void MessageRichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<RoutedUICommand> commands = new()
            {
                EditingCommands.AlignCenter,
                EditingCommands.AlignLeft,
                EditingCommands.AlignRight,
                EditingCommands.AlignJustify,
                EditingCommands.DecreaseFontSize,
                EditingCommands.DecreaseIndentation,
                EditingCommands.IncreaseFontSize,
                EditingCommands.IncreaseIndentation,
                EditingCommands.ToggleBold,
                EditingCommands.ToggleBullets,
                EditingCommands.ToggleItalic,
                EditingCommands.ToggleNumbering,
                EditingCommands.ToggleSubscript,
                EditingCommands.ToggleSuperscript,
                EditingCommands.ToggleUnderline
            };

            foreach (RoutedUICommand command in commands)
            {
                MessageRichTextBox.CommandBindings.Add(new CommandBinding(command, null, EdittingCmd_CantExecute));
            }
        }

        private void MessageRichTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.FormatToApply == "Bitmap")
            {
                e.CancelCommand();
            }
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(TokenTextBox.Text) ||
                string.IsNullOrWhiteSpace(ChatIdTextBox.Text) ||
                Configs.Instance.ConfigsList.Any(x => x.Name == NameTextBox.Text))
                return;

            // Update
            Configs.Config config = new Configs.Config()
            {
                Name = NameTextBox.Text,
                Token = TokenTextBox.Text,
                ChatId = ChatIdTextBox.Text,
                ReplyMessageId = int.TryParse(ReplyMessageIdTextBox.Text, out int replyMessageId) ? replyMessageId : null,
            };
            Configs.UpdateConfig(Configs.Instance.SelectedConfig, config);
            UpdateProfilesPanel();
        }

        private void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(TokenTextBox.Text) ||
                string.IsNullOrWhiteSpace(ChatIdTextBox.Text) ||
                Configs.Instance.ConfigsList.Any(x => x.Name == NameTextBox.Text))
                return;

            // Add
            Configs.Config config = new Configs.Config()
            {
                Name = NameTextBox.Text,
                Token = TokenTextBox.Text,
                ChatId = ChatIdTextBox.Text,
                ReplyMessageId = int.TryParse(ReplyMessageIdTextBox.Text, out int replyMessageId) ? replyMessageId : null,
            };
            Configs.AddConfig(config);
            UpdateProfilesPanel();
        }

        private void DeleteConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteConfigButton.Content.ToString() == "Delete")
            {
                timer = new();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += TurnBackToNormalDeleteConfigButton;
                DeleteConfigButton.Content = "Sure?";
                DeleteConfigButton.Foreground = Brushes.Red;
                timer.Start();
            }
            else
            {
                if (timer != null && timer.IsEnabled)
                {
                    TurnBackToNormalDeleteConfigButton();
                    // Delete
                    Configs.RemoveConfig(Configs.Instance.SelectedConfig);
                    UpdateProfilesPanel();
                }
            }
        }

        private void TurnBackToNormalDeleteConfigButton(object? sender = null, EventArgs? e = null)
        {
            timer.Stop();
            DeleteConfigButton.Content = "Delete";
            DeleteConfigButton.Foreground = Brushes.Black;
            timer = null;
        }
    }
}