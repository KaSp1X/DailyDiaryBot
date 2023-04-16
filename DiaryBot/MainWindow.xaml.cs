using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

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
                var xaml = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"> "
        + HtmlElement.ToXaml(Messages.Instance.MessagesList[i].Text) + " </TextBlock>";
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
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.SendMessage(text);
            }
        }

        private async void EditLastMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string text = MessageTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await Bot.Instance.EditPickedMessage(text);
            }
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
                        (obj as Button).Background = Brushes.LightGoldenrodYellow;
                    }
                }
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.Key == Key.B || e.Key == Key.I || e.Key == Key.U) && (Keyboard.Modifiers & ModifierKeys.Control) > 0) ||
                    ((e.Key == Key.X || e.Key == Key.P) && (Keyboard.Modifiers & ModifierKeys.Control) > 0 && (Keyboard.Modifiers & ModifierKeys.Shift) > 0))
            {
                if (MessageTextBox.SelectionLength > 0)
                {
                    e.Handled = true;
                    string formatedSelectedText = "";
                    switch (e.Key)
                    {
                        case Key.B:
                            formatedSelectedText = HtmlElement.Insert(MessageTextBox.SelectedText, HtmlElement.Bold);
                            break;
                        case Key.I:
                            formatedSelectedText = HtmlElement.Insert(MessageTextBox.SelectedText, HtmlElement.Italic);
                            break;
                        case Key.U:
                            formatedSelectedText = HtmlElement.Insert(MessageTextBox.SelectedText, HtmlElement.Underline);
                            break;
                        case Key.X:
                            formatedSelectedText = HtmlElement.Insert(MessageTextBox.SelectedText, HtmlElement.Strikethrough);
                            break;
                        case Key.P:
                            formatedSelectedText = HtmlElement.Insert(MessageTextBox.SelectedText, HtmlElement.Spoiler);
                            break;
                        default:
                            break;
                    }
                    int caretIndex = MessageTextBox.SelectionStart;
                    MessageTextBox.Text = MessageTextBox.Text[..MessageTextBox.SelectionStart] + formatedSelectedText +
                        MessageTextBox.Text[(MessageTextBox.SelectionStart + MessageTextBox.SelectionLength)..];
                    MessageTextBox.CaretIndex = caretIndex + 2;
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
            var xaml = "<TextBlock xmlns = \"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Padding=\"5\" TextWrapping=\"Wrap\" TextAlignment=\"Justify\" > "
                    + HtmlElement.ToXaml(MessageTextBox.Text) +
                    "</TextBlock>";
            PreviewWindow.Child = XamlReader.Parse(xaml,true) as TextBlock;
        }
    }
}