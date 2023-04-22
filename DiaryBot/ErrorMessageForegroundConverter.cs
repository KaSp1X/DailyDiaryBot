using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DiaryBot
{
    internal class ErrorMessageForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if((string)value == "Success!")
            {
                return Brushes.Green;
            }
            else
            {
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
