using System;
using System.Globalization;
using Xamarin.Forms;

namespace PiEarManager.Helpers
{
    public class Conterters
    {
        public class BoolNot : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return !(bool)value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return !(bool)value;
            }
        }
    }
}