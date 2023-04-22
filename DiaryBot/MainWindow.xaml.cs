using System;
using System.Collections.Generic;
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
            string text = new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd).Text;
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
            string text = new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd).Text;
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
            var textRange = new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd);
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

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // to prevent overflow, we temporary remove event from our RichTextBox
            MessageTextBox.TextChanged -= MessageTextBox_TextChanged;

            // rendering preview window
            string @fixed = new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd).Text.Replace("&", "&amp;").Replace("<", "&lt;");
            var xaml = """
                    <TextBlock xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    Padding="2" Margin="5" FontSize="10" TextWrapping="Wrap" xml:space="preserve">
                    """ + @fixed.ToXaml() + "</TextBlock>";
            PreviewWindow.Content = XamlReader.Parse(xaml) as TextBlock;

            // after everything is done return event to our RichTextBox
            MessageTextBox.TextChanged += MessageTextBox_TextChanged;
        }

        private void FormattingCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            if (MessageTextBox.Selection.Text.Length > 0)
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
            var selectedTextRange = new TextRange(MessageTextBox.Selection.Start, MessageTextBox.Selection.End);

            string formatedMessage = FormattingTag.Insert(selectedTextRange, tag);
            if (!string.IsNullOrWhiteSpace(formatedMessage))
            {
                MessageTextBox.Selection.Text = formatedMessage;
            }
        }

        private void MessageTextBox_Loaded(object sender, RoutedEventArgs e)
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
                MessageTextBox.CommandBindings.Add(new CommandBinding(command, null, EdittingCmd_CantExecute));
            }
        }
    }
}