using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private System.Windows.Threading.DispatcherTimer removeConfigTimer;
        private System.Windows.Threading.DispatcherTimer removePresetTimer;
        private System.Windows.Threading.DispatcherTimer previewAndHighlightTagsTimer;

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
                    Style = null,
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

        private void UpdateConfigsPanel()
        {
            ConfigsStackPanel.Children.Clear();
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
                        NameConfigTextBox.Text = Configs.Instance.SelectedConfig.Name;
                        TokenConfigTextBox.Text = Configs.Instance.SelectedConfig.Token;
                        ChatIdConfigTextBox.Text = Configs.Instance.SelectedConfig.ChatId;
                        ReplyMessageIdConfigTextBox.Text = replyMessageId;
                    }
                    ConfigsStackPanel.Children.Add(button);
                }
            }
        }
        
        private void UpdatePresetsPanel()
        {
            PresetsStackPanel.Children.Clear();
            for (int i = 0; i < Presets.Instance.PresetsList.Count; i++)
            {
                string name = Presets.Instance.PresetsList[i].Name.Replace("&", "&amp;").Replace("<", "&lt;");
                string text = Presets.Instance.PresetsList[i].Text.Replace("&", "&amp;").Replace("<", "&lt;");

                var xaml = $"""
                    <Button xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            Name="Preset_{i}"
                            xml:space="preserve"
                            Height="75"
                            Padding="5"
                            VerticalContentAlignment="Stretch"
                            HorizontalContentAlignment="Stretch">
                        <StackPanel>
                            <TextBlock FontStyle="Italic" FontWeight="Medium">{name}</TextBlock>
                            <TextBlock>{text.ToXaml()}</TextBlock>
                        </StackPanel>
                    </Button>
                    """;
                var button = XamlReader.Parse(xaml) as Button;
                if (button != null)
                {
                    button.Click += PresetButton_Click;
                    button.MouseDoubleClick += PresetButton_MouseDoubleClick;
                    if (Presets.Instance.PresetsList[i].Equals(Presets.Instance.SelectedPreset))
                    {
                        button.Background = Brushes.LightGoldenrodYellow;
                        NamePresetTextBox.Text = Presets.Instance.SelectedPreset.Name;
                        new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd).Text = Presets.Instance.SelectedPreset.Text;
                    }
                    PresetsStackPanel.Children.Add(button);
                }
            }
        }

        private void PresetButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageRichTextBox.Selection.Text = Presets.Instance.SelectedPreset.Text.TrimEnd('\n').TrimEnd('\r');
            MessageTab.IsSelected = true;
            RichTextBox_TextChanged(MessageRichTextBox, null);
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in ConfigsStackPanel.Children)
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
                        NameConfigTextBox.Text = Configs.Instance.SelectedConfig.Name;
                        TokenConfigTextBox.Text = Configs.Instance.SelectedConfig.Token;
                        if (long.TryParse(Configs.Instance.SelectedConfig.Token.Split(':')[0], out long token) && token != Bot.GetToken())
                        {
                            Bot.Instance = null;
                        }
                        ChatIdConfigTextBox.Text = Configs.Instance.SelectedConfig.ChatId;
                        string replyMessageId = Configs.Instance.SelectedConfig.ReplyMessageId.ToString();
                        if (string.IsNullOrEmpty(replyMessageId))
                            replyMessageId = "Empty";
                        ReplyMessageIdConfigTextBox.Text = replyMessageId;
                    }
                }
            }
        }
        
        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in PresetsStackPanel.Children)
            {
                Button button = obj as Button;
                if (button != sender)
                    button.Background = Brushes.LightGray;
                else
                {
                    button.Background = Brushes.LightGoldenrodYellow;
                    int.TryParse(button.Name.Split('_')[1], out int index);
                    if (index >= 0 && index < Presets.Instance.PresetsList.Count)
                    {
                        Presets.Instance.SelectedPreset = Presets.Instance.PresetsList[index];
                        NamePresetTextBox.Text = Presets.Instance.SelectedPreset.Name;
                        new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd).Text = Presets.Instance.SelectedPreset.Text;
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
            else
            {
                ClearRecentTab();
            }

            if (ConfigsTab.IsSelected)
            {
                UpdateConfigsPanel();
            }
            else
            {
                ClearConfigsTab();
            }
            
            if (PresetsTab.IsSelected)
            {
                UpdatePresetsPanel();
            }
            else
            {
                ClearPresetsTab();
            }
        }

        private void ClearRecentTab() => RecentGrid.Children.Clear();

        private void ClearConfigsTab()
        {
            ConfigsStackPanel.Children.Clear();
            NameConfigTextBox.Clear();
            TokenConfigTextBox.Clear();
            ChatIdConfigTextBox.Clear();
            ReplyMessageIdConfigTextBox.Clear();
        }

        private void ClearPresetsTab()
        {
            PresetsStackPanel.Children.Clear();
            NamePresetTextBox.Clear();
            _ = new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd)
            {
                Text = string.Empty
            };
            Presets.Instance.SelectedPreset = new(string.Empty, string.Empty);
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            // to prevent overflow, we temporary remove event from our RichTextBox
            richTextBox.TextChanged -= RichTextBox_TextChanged;

            var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            // a timer to make less load on the system when editing large text
            if (previewAndHighlightTagsTimer != null)
            {
                previewAndHighlightTagsTimer.Stop();
                previewAndHighlightTagsTimer = null;
            }

            if (previewAndHighlightTagsTimer == null)
            {
                previewAndHighlightTagsTimer = new();
                previewAndHighlightTagsTimer.Interval = TimeSpan.FromMilliseconds(200);
                previewAndHighlightTagsTimer.Tick += (sender, e) =>
                {
                    // highlighting tags in richtextbox
                    Regex reg = new Regex(@"\[\\[bivus][0]{0,1}\]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var start = range.Start;
                    var end = range.End;
                    range.ClearAllProperties();

                    while (start != null && start.CompareTo(end) < 0)
                    {
                        if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                        {
                            var match = reg.Match(start.GetTextInRun(LogicalDirection.Forward));
                            var subrange = new TextRange(start.GetPositionAtOffset(match.Index), start.GetPositionAtOffset(match.Index + match.Length));
                            subrange.ApplyPropertyValue(ForegroundProperty, Brushes.Blue);
                            new TextRange(subrange.End, end).ClearAllProperties();
                            start = subrange.End;
                        }
                        start = start.GetNextContextPosition(LogicalDirection.Forward);
                    }

                    // rendering preview window
                    ScrollViewer scrollViewer = richTextBox.Name == "TextPresetRichTextBox" ? PreviewPresetWindow : PreviewMessageWindow;

                    string @fixed = range.Text.Replace("&", "&amp;").Replace("<", "&lt;");
                    var xaml = """
                    <TextBlock xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    Padding="2" Margin="5" FontSize="10" TextWrapping="Wrap" xml:space="preserve">
                    """ + @fixed.ToXaml() + "</TextBlock>";
                    scrollViewer.Content = XamlReader.Parse(xaml);
                    previewAndHighlightTagsTimer.Stop();
                };
                previewAndHighlightTagsTimer.Start();
            }

            // after everything is done return event to our RichTextBox
            richTextBox.TextChanged += RichTextBox_TextChanged;
        }

        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            RichTextBox rtb = (RichTextBox)sender;
            TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            string text = tr.Text.Replace("\r", "").Replace("\n", "");
            if (text.Length >= Bot.MaxTextLength)
            {
                e.Handled = true;
            }
        }

        private void RichTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.FormatToApply == "Bitmap")
            {
                e.CancelCommand();
            }
            else
            {
                RichTextBox rtb = (RichTextBox)sender;
                TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                string text = tr.Text.Replace("\r", "").Replace("\n", "");
                if (text.Length == Bot.MaxTextLength)
                {
                    e.CancelCommand();
                }
                else
                {
                    string pasteText = (string)e.DataObject.GetData("UnicodeText");
                    string fulltext = text + pasteText.Replace("\r", "").Replace("\n", "");
                    if (fulltext.Length > Bot.MaxTextLength)
                    {
                        int lengthToCap = fulltext.Length - Bot.MaxTextLength;
                        DataObject d = new DataObject();
                        d.SetData(DataFormats.Text, pasteText[..^lengthToCap]);
                        e.DataObject = d;
                    }
                }
            }
        }

        private void FormattingCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            e.Handled = true;
            if (richTextBox.Selection.Text.Length > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void EdittingCmd_CantExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        private void SetBoldCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(sender, FormattingTag.Bold);

        private void SetItalicCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(sender, FormattingTag.Italic);

        private void SetUnderlineCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(sender, FormattingTag.Underline);

        private void SetStrikethroughCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(sender, FormattingTag.Strikethrough);

        private void SetSpoilerCmd_Executed(object sender, ExecutedRoutedEventArgs e) => FormatText(sender, FormattingTag.Spoiler);
        
        private void FormatText(object sender, string tag)
        {
            RichTextBox rtb = (RichTextBox)sender;
            var selectedTextRange = new TextRange(rtb.Selection.Start, rtb.Selection.End);

            string formatedMessage = FormattingTag.Insert(selectedTextRange, tag);
            if (!string.IsNullOrWhiteSpace(formatedMessage))
            {
                rtb.Selection.Text = formatedMessage;
            }
        }

        private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
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
                richTextBox.CommandBindings.Add(new CommandBinding(command, null, EdittingCmd_CantExecute));
            }
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(TokenConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(ChatIdConfigTextBox.Text) ||
                Configs.Instance.ConfigsList.Any(x => x.Name == NameConfigTextBox.Text))
                return;

            // Update
            Configs.Config config = new Configs.Config()
            {
                Name = NameConfigTextBox.Text,
                Token = TokenConfigTextBox.Text,
                ChatId = ChatIdConfigTextBox.Text,
                ReplyMessageId = int.TryParse(ReplyMessageIdConfigTextBox.Text, out int replyMessageId) ? replyMessageId : null,
            };
            Configs.UpdateConfig(Configs.Instance.SelectedConfig, config);
            UpdateConfigsPanel();
        }

        private void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(TokenConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(ChatIdConfigTextBox.Text) ||
                Configs.Instance.ConfigsList.Any(x => x.Name == NameConfigTextBox.Text))
                return;

            // Add
            Configs.Config config = new Configs.Config()
            {
                Name = NameConfigTextBox.Text,
                Token = TokenConfigTextBox.Text,
                ChatId = ChatIdConfigTextBox.Text,
                ReplyMessageId = int.TryParse(ReplyMessageIdConfigTextBox.Text, out int replyMessageId) ? replyMessageId : null,
            };
            Configs.AddConfig(config);
            UpdateConfigsPanel();
        }

        private void DeleteConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteConfigButton.Content.ToString() == "Delete")
            {
                removeConfigTimer = new();
                removeConfigTimer.Interval = TimeSpan.FromSeconds(3);
                removeConfigTimer.Tick += TurnBackToNormalDeleteConfigButton;
                DeleteConfigButton.Content = "Sure?";
                DeleteConfigButton.Foreground = Brushes.Red;
                removeConfigTimer.Start();
            }
            else
            {
                if (removeConfigTimer != null && removeConfigTimer.IsEnabled)
                {
                    TurnBackToNormalDeleteConfigButton();
                    // Delete
                    Configs.RemoveConfig(Configs.Instance.SelectedConfig);
                    UpdateConfigsPanel();
                }
            }
        }

        private void TurnBackToNormalDeleteConfigButton(object? sender = null, EventArgs? e = null)
        {
            removeConfigTimer.Stop();
            DeleteConfigButton.Content = "Delete";
            DeleteConfigButton.Foreground = Brushes.Black;
            removeConfigTimer = null;
        }

        private void UpdatePresetButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(NamePresetTextBox.Text) ||
                string.IsNullOrWhiteSpace(text) ||
                Presets.Instance.PresetsList.Any(x => x.Name == NamePresetTextBox.Text && NamePresetTextBox.Text != Presets.Instance.SelectedPreset.Name))
                return;

            // Update
            var newPreset = new Presets.Preset(NamePresetTextBox.Text, text);
            Presets.UpdatePreset(Presets.Instance.SelectedPreset, newPreset);
            UpdatePresetsPanel();
        }

        private void AddPresetButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(NamePresetTextBox.Text) ||
                string.IsNullOrWhiteSpace(text) ||
                Presets.Instance.PresetsList.Any(x => x.Name == NamePresetTextBox.Text))
                return;

            //// Add
            var newPreset = new Presets.Preset(NamePresetTextBox.Text, text);
            Presets.AddPreset(newPreset);
            UpdatePresetsPanel();
        }

        private void DeletePresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeletePresetButton.Content.ToString() == "Delete")
            {
                removePresetTimer = new();
                removePresetTimer.Interval = TimeSpan.FromSeconds(3);
                removePresetTimer.Tick += TurnBackToNormalDeletePresetButton;
                DeletePresetButton.Content = "Sure?";
                DeletePresetButton.Foreground = Brushes.Red;
                removePresetTimer.Start();
            }
            else
            {
                if (removePresetTimer != null && removePresetTimer.IsEnabled)
                {
                    TurnBackToNormalDeletePresetButton();
                    // Delete
                    Presets.RemovePreset(Presets.Instance.SelectedPreset);
                    UpdatePresetsPanel();
                }
            }
        }

        private void TurnBackToNormalDeletePresetButton(object? sender = null, EventArgs? e = null)
        {
            removePresetTimer.Stop();
            DeletePresetButton.Content = "Delete";
            DeletePresetButton.Foreground = Brushes.Black;
            removePresetTimer = null;
        }
    }
}