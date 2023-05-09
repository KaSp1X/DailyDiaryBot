using System.Windows.Input;

namespace DiaryBot.WPF
{
    public static class FormattingCommand
    {
        public static readonly RoutedUICommand SetBoldTextCmd =
            new("Bold", "Bold", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.B, ModifierKeys.Control) });
        public static readonly RoutedUICommand SetItalicTextCmd =
            new("Italic", "Italic", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.I, ModifierKeys.Control) });
        public static readonly RoutedUICommand SetUnderlineTextCmd =
            new("Underline", "Underline", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.U, ModifierKeys.Control) });
        public static readonly RoutedUICommand SetStrikethroughTextCmd =
            new("Strikethrough", "Strikethrough", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.X, ModifierKeys.Control | ModifierKeys.Shift) });
        public static readonly RoutedUICommand SetSpoilerTextCmd =
            new("Spoiler", "Spoiler", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Shift) });
    }
}
