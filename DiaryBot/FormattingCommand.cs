using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiaryBot
{
    public static class FormattingCommand
    {
        public static readonly RoutedCommand SetBoldTextCmd = 
            new RoutedCommand("Bold", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.B, ModifierKeys.Control) });
        public static readonly RoutedCommand SetItalicTextCmd = 
            new RoutedCommand("Italic", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.I, ModifierKeys.Control) });
        public static readonly RoutedCommand SetUnderlineTextCmd = 
            new RoutedCommand("Underline", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.U, ModifierKeys.Control) });
        public static readonly RoutedCommand SetStrikethroughTextCmd = 
            new RoutedCommand("Strikethrough", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.X, ModifierKeys.Control | ModifierKeys.Shift) });
        public static readonly RoutedCommand SetSpoilerTextCmd = 
            new RoutedCommand("Spoiler", typeof(FormattingCommand), new InputGestureCollection { new KeyGesture(Key.P, ModifierKeys.Control | ModifierKeys.Shift) });
    }
}
