using DiaryBot.Core;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace DiaryBot.WPF
{
    public static class Controller
    {
        #region ReadOnly Fields

        public readonly static TimeSpan RemoveInterval = TimeSpan.FromSeconds(3);
        public readonly static TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(200);
        public readonly static Regex TagRegex = new(@"\[\\[bivus][0]{0,1}\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public readonly static RoutedUICommand[] Commands = new[]
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

        #endregion

        #region Properties

        public static DispatcherTimer? DeleteItemTimer { get; set; }

        public static DispatcherTimer? UpdateTextTimer { get; set; }

        public static bool IsTagged { get; internal set; }

        #endregion

        #region Methods

        public static void WrapSelectedText(object sender, string tag)
        {
            if (sender is RichTextBox richTextBox)
            {
                var selectedTextRange = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
                string selectedText = selectedTextRange.Text;
                string textBefore = selectedTextRange.Start.GetTextInRun(LogicalDirection.Backward);
                string textAfter = selectedTextRange.End.GetTextInRun(LogicalDirection.Forward);

                string formatedMessage = selectedText.WrapWithTags(tag, textBefore, textAfter);
                if (!string.IsNullOrWhiteSpace(formatedMessage))
                {
                    richTextBox.BeginChange();
                    richTextBox.Selection.Text = formatedMessage;
                    HighLightText(new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End));
                    richTextBox.EndChange();
                    IsTagged = true;
                }
            }
        }

        public static void SetDeleteButtonIdle(object sender)
        {
            if (sender is Button button)
            {
                button.Content = "Delete";
                button.Foreground = Brushes.Black;
                DeleteItemTimer?.Stop();
                DeleteItemTimer = null;
            }
        }

        public static bool IsDeleteButtonInIdleState(object sender)
        {
            if (sender is Button button && button.Content.ToString() == "Delete")
            {
                DeleteItemTimer = CreateTimer(RemoveInterval, (s, e) => SetDeleteButtonIdle(button));
                DeleteItemTimer.Start();

                button.Content = "Sure?";
                button.Foreground = Brushes.Red;
                return true;
            }

            return false;
        }

        public static void UpdatePreviewWindow(ScrollViewer scroll, string text)
        {
            var xaml = """
                    <TextBlock xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    Padding="2" Margin="5" FontSize="10" TextWrapping="Wrap" xml:space="preserve">
                    """ + text.ToXaml() + "</TextBlock>";
            scroll.Content = XamlReader.Parse(xaml);
        }
        public static void HighLightText(TextRange range)
        {
            var start = range.Start;
            var end = range.End;
            range.ClearAllProperties();

            while (start != null && start.CompareTo(end) < 0)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var match = Controller.TagRegex.Match(start.GetTextInRun(LogicalDirection.Forward));
                    var subrange = new TextRange(start.GetPositionAtOffset(match.Index),
                        start.GetPositionAtOffset(match.Index + match.Length));
                    subrange.ApplyPropertyValue(Control.ForegroundProperty, Brushes.Blue);
                    new TextRange(subrange.End, end).ClearAllProperties();
                    start = subrange.End;
                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        public static void DisposeTimer(this DispatcherTimer? timer)
        {
            timer?.Stop();
            timer = null;
        }

        public static DispatcherTimer CreateTimer(TimeSpan interval, EventHandler eventHandler)
        {
            DispatcherTimer timer = new()
            {
                Interval = interval
            };
            timer.Tick += eventHandler;
            timer.Tick += (sender, e) => timer.DisposeTimer();
            return timer;
        }

        #endregion
    }
}
