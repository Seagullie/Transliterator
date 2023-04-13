using System;
using System.Windows.Data;

namespace Transliterator.Helpers;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        bool boolValue = (bool)value;

        return boolValue ? "Green" : "Red";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}