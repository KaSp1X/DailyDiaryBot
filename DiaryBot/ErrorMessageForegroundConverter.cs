using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DiaryBot
{
    public class ErrorMessageForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture) => string.IsNullOrEmpty((string?)value) ? Brushes.Transparent : value.Equals("Success!") ? Brushes.Green : Brushes.Red;

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture) => Binding.DoNothing;
    }

}
