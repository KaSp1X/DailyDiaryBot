using System.Windows.Input;

namespace DiaryBot
{
    public static class FormattingCommand
    {
        public static readonly RoutedCommand SetStrikethroughTextCmd = 
            new RoutedUICommand("Strikethrough", "Strikethrough", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.X, ModifierKeys.Control | ModifierKeys.Shift) });
        public static readonly RoutedCommand SetSpoilerTextCmd = 
            new RoutedUICommand("Spoiler", "Spoiler", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Shift) });
    }
}
