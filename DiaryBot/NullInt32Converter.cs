using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DiaryBot
{
    public class NullInt32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return null;
            if (!Int32.TryParse(value.ToString(), out int d))
                return Binding.DoNothing;
            return d;
        }
    }
}
