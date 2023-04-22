using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        private bool isSettingsSelected = false;

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
                    Name = "recentMessage" + i,
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

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.SendMessage(text);
                Error.Instance.Message = "Success";
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
                Error.Instance.Message = "Success";
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
                    {
                        (obj as Button).Background = Brushes.LightGray;
                    }
                    else
                    {
                        (obj as Button).Background = Brushes.LightGoldenrodYellow;
                    }
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
                isSettingsSelected = true;
            }

            if (isSettingsSelected && !SettingsTab.IsSelected)
            {
                Serializer.Save(Config.Path, Config.Instance);
                isSettingsSelected = false;
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
    }
}