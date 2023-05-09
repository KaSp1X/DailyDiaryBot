using DiaryBot.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace DiaryBot.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Tab Methods and Events

        #region UpdateTabMethods

        private void UpdateRecentGrid()
        {
            RecentGrid.Children.Clear();
            for (int i = 0; i < MessagesModel.Instance.Items.Count; i++)
            {
                string text = MessagesModel.Instance[i]?.Text.FixXamlKeys() ?? string.Empty;

                var xaml = $"""
                    <Button xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            Name="Recent_{i}"
                            xml:space="preserve"
                            Margin="5"
                            Padding="5"
                            VerticalContentAlignment="Stretch"
                            HorizontalContentAlignment="Stretch"
                            Grid.Row="{i / 2}"
                            Grid.Column="{i % 2}"
                            Background="{(MessagesModel.Instance[i].Equals(MessagesModel.Instance.SelectedItem) ?
                            Brushes.LightGoldenrodYellow : Brushes.LightGray)}">
                        <TextBlock TextWrapping="Wrap">{text.ToXaml()}</TextBlock>
                    </Button>
                    """;
                Button? button = XamlReader.Parse(xaml) as Button;
                if (button != null)
                {
                    button.Style = null;
                    button.Click += RecentButton_Click;
                    RecentGrid.Children.Add(button);
                }
            }
        }

        private void UpdateConfigsPanel()
        {
            ConfigsStackPanel.Children.Clear();
            for (int i = ConfigsModel.Instance.Items.Count - 1; i >= 0; i--)
            {
                string name = ConfigsModel.Instance.Items[i].Name.FixXamlKeys();
                string token = ConfigsModel.Instance.Items[i].Token.FixXamlKeys();
                string chatId = ConfigsModel.Instance.Items[i].ChatId.FixXamlKeys();
                string replyMessageId = ConfigsModel.Instance.Items[i].ReplyMessageId.ToString() ?? string.Empty;

                var xaml = $"""
                    <Button xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            Name="Profile_{i}"
                            xml:space="preserve"
                            Height="75"
                            Padding="5"
                            VerticalContentAlignment="Stretch"
                            HorizontalContentAlignment="Stretch"
                            Background="{(ConfigsModel.Instance.Items[i].Equals(ConfigsModel.Instance.SelectedItem) ?
                            Brushes.LightGoldenrodYellow : Brushes.LightGray)}">
                        <StackPanel>
                            <TextBlock FontStyle="Italic" FontWeight="Medium">{name}</TextBlock>
                            <TextBlock><TextBlock FontWeight="Medium">T: </TextBlock>{token.Split(':')[0]}</TextBlock>
                            <TextBlock><TextBlock FontWeight="Medium">CId: </TextBlock>{chatId}</TextBlock>
                            <TextBlock><TextBlock FontWeight="Medium">RMId: </TextBlock>{replyMessageId}</TextBlock>
                        </StackPanel>
                    </Button>
                    """;
                Button? button = XamlReader.Parse(xaml) as Button;
                if (button != null)
                {
                    button.Click += ProfileButton_Click;
                    ConfigsStackPanel.Children.Add(button);
                }
            }

            NameConfigTextBox.Text = ConfigsModel.Instance.SelectedItem?.Name;
            TokenConfigTextBox.Text = ConfigsModel.Instance.SelectedItem?.Token;
            ChatIdConfigTextBox.Text = ConfigsModel.Instance.SelectedItem?.ChatId;
            ReplyMessageIdConfigTextBox.Text = ConfigsModel.Instance.SelectedItem?.ReplyMessageId.ToString() ?? string.Empty;
        }

        private void UpdatePresetsPanel()
        {
            PresetsStackPanel.Children.Clear();
            for (int i = 0; i < PresetsModel.Instance.Items.Count; i++)
            {
                string name = PresetsModel.Instance.Items[i].Name.FixXamlKeys();
                string text = PresetsModel.Instance.Items[i].Text.FixXamlKeys();

                var xaml = $"""
                    <Button xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            Name="Preset_{i}"
                            xml:space="preserve"
                            Height="75"
                            Padding="5"
                            VerticalContentAlignment="Stretch"
                            HorizontalContentAlignment="Stretch"
                            Background="{(PresetsModel.Instance.Items[i].Equals(PresetsModel.Instance.SelectedItem) ?
                            Brushes.LightGoldenrodYellow : Brushes.LightGray)}">
                        <StackPanel>
                            <TextBlock FontStyle="Italic" FontWeight="Medium">{name}</TextBlock>
                            <TextBlock>{text.ToXaml()}</TextBlock>
                        </StackPanel>
                    </Button>
                    """;
                Button? button = XamlReader.Parse(xaml) as Button;
                if (button != null)
                {
                    button.Click += PresetButton_Click;
                    button.MouseDoubleClick += PresetButton_MouseDoubleClick;
                    PresetsStackPanel.Children.Add(button);
                }
            }

            NamePresetTextBox.Text = PresetsModel.Instance.SelectedItem?.Name;
            new TextRange(TextPresetRichTextBox.Document.ContentStart,
                TextPresetRichTextBox.Document.ContentEnd).Text = PresetsModel.Instance.SelectedItem?.Text ?? string.Empty;
        }
        
        #endregion

        #region ClearTabMethods

        private void ClearRecentTab()
        {
            RecentGrid.Children.Clear();
        }

        private void ClearConfigsTab()
        {
            ConfigsStackPanel.Children.Clear();
            NameConfigTextBox.Clear();
            TokenConfigTextBox.Clear();
            ChatIdConfigTextBox.Clear();
            ReplyMessageIdConfigTextBox.Clear();
            Controller.SetDeleteButtonIdle(DeleteConfigButton);
        }

        private void ClearPresetsTab()
        {
            PresetsStackPanel.Children.Clear();
            NamePresetTextBox.Clear();
            new TextRange(TextPresetRichTextBox.Document.ContentStart,
                TextPresetRichTextBox.Document.ContentEnd).Text = string.Empty;
            PresetsModel.Instance.SelectedItem = null;
            Controller.SetDeleteButtonIdle(DeletePresetButton);
        }
        
        #endregion

        #region ItemClickEvents

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in ConfigsStackPanel.Children)
            {
                if (obj is Button button)
                {
                    if (!sender.Equals(button))
                        button.Background = Brushes.LightGray;
                    else
                    {
                        button.Background = Brushes.LightGoldenrodYellow;
                        if (int.TryParse(button.Name.Split('_')[1], out int index) &&
                            index >= 0 && index < ConfigsModel.Instance.Items.Count)
                        {
                            if (long.TryParse(ConfigsModel.Instance.SelectedItem?.Token.Split(':')[0], out long token) &&
                                !token.Equals(Bot.GetToken()))
                            {
                                Bot.ClearInstance();
                            }

                            ConfigsModel.Instance.SelectedItem = ConfigsModel.Instance.Items[index];
                            NameConfigTextBox.Text = ConfigsModel.Instance.SelectedItem.Name;
                            TokenConfigTextBox.Text = ConfigsModel.Instance.SelectedItem.Token;
                            ChatIdConfigTextBox.Text = ConfigsModel.Instance.SelectedItem.ChatId;
                            string replyMessageId = ConfigsModel.Instance.SelectedItem?.ReplyMessageId?.ToString() ?? string.Empty;
                            ReplyMessageIdConfigTextBox.Text = replyMessageId;
                        }
                    }
                }
            }
        }

        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in PresetsStackPanel.Children)
            {
                if (obj is Button button)
                {
                    if (button != sender)
                        button.Background = Brushes.LightGray;
                    else
                    {
                        button.Background = Brushes.LightGoldenrodYellow;
                        if (int.TryParse(button.Name.Split('_')[1], out int index) &&
                            index >= 0 && index < PresetsModel.Instance.Items.Count)
                        {
                            PresetsModel.Instance.SelectedItem = PresetsModel.Instance.Items[index];
                            NamePresetTextBox.Text = PresetsModel.Instance.SelectedItem.Name;
                            new TextRange(TextPresetRichTextBox.Document.ContentStart,
                                TextPresetRichTextBox.Document.ContentEnd).Text = PresetsModel.Instance.SelectedItem.Text;
                        }
                    }
                }
            }
        }

        private void RecentButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement obj in RecentGrid.Children)
            {
                if (obj is Button button)
                {
                    if (button != sender)
                        button.Background = Brushes.LightGray;
                    else
                    {
                        button.Background = Brushes.LightGoldenrodYellow;
                        if (int.TryParse(button.Name.Split('_')[1], out int index) &&
                            index >= 0 && index < MessagesModel.Instance.Items.Count)
                        {
                            MessagesModel.Instance.SelectedItem = MessagesModel.Instance.Items[index];
                            new TextRange(MessageRichTextBox.Document.ContentStart,
                                MessageRichTextBox.Document.ContentEnd).Text = MessagesModel.Instance.SelectedItem?.Text;
                        }
                    }
                }
            }
        }

        private void PresetButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageRichTextBox.Selection.Text = PresetsModel.Instance.SelectedItem?.Text.TrimEnd('\n').TrimEnd('\r');
            MessageTab.IsSelected = true;
            RichTextBox_TextChanged(MessageRichTextBox);
        }

        #endregion

        private void MainWindowTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecentTab.IsSelected)
                UpdateRecentGrid();
            else
                ClearRecentTab();

            if (ConfigsTab.IsSelected)
                UpdateConfigsPanel();
            else
                ClearConfigsTab();

            if (PresetsTab.IsSelected)
                UpdatePresetsPanel();
            else
                ClearPresetsTab();
        }

        #endregion

        #region Message Sending Events

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.SendMessage(text);
            }
            else
                StatusModel.Instance.Message = "Text to send can't be empty";
        }

        private async void EditLastMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(MessageRichTextBox.Document.ContentStart, MessageRichTextBox.Document.ContentEnd).Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.EditSelectedMessage(text);
            }
            else
                StatusModel.Instance.Message = "Edited version of text can't be empty";
        }

        #endregion

        #region Commands

        private void FormattingCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                e.Handled = true;
                if (richTextBox.Selection.Text.Length > 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        private void SetBoldCmd_Executed(object sender, ExecutedRoutedEventArgs e) => Controller.WrapSelectedText(sender, Core.Tag.Bold);

        private void SetItalicCmd_Executed(object sender, ExecutedRoutedEventArgs e) => Controller.WrapSelectedText(sender, Core.Tag.Italic);

        private void SetUnderlineCmd_Executed(object sender, ExecutedRoutedEventArgs e) => Controller.WrapSelectedText(sender, Core.Tag.Underline);

        private void SetStrikethroughCmd_Executed(object sender, ExecutedRoutedEventArgs e) => Controller.WrapSelectedText(sender, Core.Tag.Strikethrough);

        private void SetSpoilerCmd_Executed(object sender, ExecutedRoutedEventArgs e) => Controller.WrapSelectedText(sender, Core.Tag.Spoiler);

        #endregion

        #region RichTextBox Events

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs? e = null)
        {
            if (sender is RichTextBox richTextBox)
            {
                // to prevent overflow, we temporary remove event from our RichTextBox
                richTextBox.TextChanged -= RichTextBox_TextChanged;

                var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                ScrollViewer scrollViewer = GetScrollViewerByRichTextBox(richTextBox);

                // a timer to make less load on the system when editing large text
                Controller.UpdateTextTimer.DisposeTimer();
                Controller.UpdateTextTimer = Controller.CreateTimer(Controller.UpdateInterval, (s, e) =>
                {
                    // highlighting tags in richtextbox
                    Controller.HighLightText(range);

                    // updating preview window
                    Controller.UpdatePreviewWindow(scrollViewer, range.Text.FixXamlKeys());
                });
                Controller.UpdateTextTimer.Start();

                // after everything is done return event to our RichTextBox
                richTextBox.TextChanged += RichTextBox_TextChanged;

            }
        }

        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                TextRange tr = new(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                if (tr.Text.ClearLinebrakes().Length >= Bot.MaxTextLength)
                {
                    e.Handled = true;
                }
            }
        }

        private void RichTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                if (e.FormatToApply == "Bitmap")
                {
                    e.CancelCommand();
                }
                else
                {
                    TextRange tr = new(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    string text = tr.Text.ClearLinebrakes();
                    if (text.Length == Bot.MaxTextLength)
                    {
                        e.CancelCommand();
                    }
                    else
                    {
                        string pasteText = (string)e.DataObject.GetData("UnicodeText");
                        string fulltext = text + pasteText.ClearLinebrakes();
                        if (fulltext.Length > Bot.MaxTextLength)
                        {
                            int lengthToCap = fulltext.Length - Bot.MaxTextLength;
                            DataObject d = new();
                            d.SetData(DataFormats.Text, pasteText[..^lengthToCap]);
                            e.DataObject = d;
                        }
                    }
                }
            }
        }

        private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                foreach (RoutedUICommand command in Controller.Commands)
                {
                    richTextBox.CommandBindings.Add(new CommandBinding(command, null, (sender, e) =>
                    {
                        e.CanExecute = false;
                        e.Handled = true;
                    }));
                }
            }
        }

        #endregion

        #region Control Button Events

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(TokenConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(ChatIdConfigTextBox.Text) ||
                ConfigsModel.Instance.Items.Any(x => x.Name == NameConfigTextBox.Text && NameConfigTextBox.Text != ConfigsModel.Instance.SelectedItem?.Name))
                return;

            // Update
            Config config = new(NameConfigTextBox.Text, TokenConfigTextBox.Text, ChatIdConfigTextBox.Text,
                int.TryParse(ReplyMessageIdConfigTextBox.Text, out int replyMessageId) ? replyMessageId : null);
            ConfigsModel.Instance.Update(config);
            UpdateConfigsPanel();
        }

        private void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(TokenConfigTextBox.Text) ||
                string.IsNullOrWhiteSpace(ChatIdConfigTextBox.Text) ||
                ConfigsModel.Instance.Items.Any(x => x.Name == NameConfigTextBox.Text))
                return;

            // Add
            Config config = new(NameConfigTextBox.Text, TokenConfigTextBox.Text, ChatIdConfigTextBox.Text,
                int.TryParse(ReplyMessageIdConfigTextBox.Text, out int replyMessageId) ? replyMessageId : null);
            ConfigsModel.Instance.Add(config);
            UpdateConfigsPanel();
        }

        private void DeleteConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Controller.IsDeleteButtonInIdleState(sender))
            {
                Controller.SetDeleteButtonIdle(sender);
                // Delete
                ConfigsModel.Instance.Remove();
                UpdateConfigsPanel();
            }
        }

        private void UpdatePresetButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(NamePresetTextBox.Text) ||
                string.IsNullOrWhiteSpace(text) ||
                PresetsModel.Instance.Items.Any(x => x.Name == NamePresetTextBox.Text && NamePresetTextBox.Text != PresetsModel.Instance.SelectedItem?.Name))
                return;

            // Update
            var newPreset = new Preset(NamePresetTextBox.Text, text);
            PresetsModel.Instance.Update(newPreset);
            UpdatePresetsPanel();
        }

        private void AddPresetButton_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(TextPresetRichTextBox.Document.ContentStart, TextPresetRichTextBox.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(NamePresetTextBox.Text) ||
                string.IsNullOrWhiteSpace(text) ||
                PresetsModel.Instance.Items.Any(x => x.Name == NamePresetTextBox.Text))
                return;

            //// Add
            var newPreset = new Preset(NamePresetTextBox.Text, text);
            PresetsModel.Instance.Add(newPreset);
            UpdatePresetsPanel();
        }

        private void DeletePresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Controller.IsDeleteButtonInIdleState(sender))
            {
                Controller.SetDeleteButtonIdle(sender);
                // Delete
                PresetsModel.Instance.Remove();
                UpdatePresetsPanel();
            }
        }

        #endregion

        #region Utilities

        public ScrollViewer GetScrollViewerByRichTextBox(RichTextBox richTextBox) => richTextBox.Name switch
        {
            "TextPresetRichTextBox" => PreviewPresetWindow,
            "MessageRichTextBox" => PreviewMessageWindow,
            _ => throw new ArgumentException("ScrollViewer is not found by this RichTextBox", nameof(richTextBox))
        };

        #endregion
    }
}