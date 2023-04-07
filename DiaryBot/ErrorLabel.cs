using System.Windows;
using System.Windows.Controls;

namespace DiaryBot
{
    public class ErrorLabel : Label
    {
        public static readonly RoutedEvent ContentChangedEvent = EventManager.RegisterRoutedEvent(
            "ContentChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ErrorLabel));

        public event RoutedEventHandler ContentChanged
        {
            add
            {
                AddHandler(ContentChangedEvent, value);
            }
            remove
            {
                RemoveHandler(ContentChangedEvent, value);
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
        }
    }
}