using System;
using System.Globalization;
using System.Windows.Data;

namespace SPTarkov.Launcher.Converters
{
    class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                return !b;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                return b;
            }

            return value;
        }
    }
}
