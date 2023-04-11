using System;
using System.Windows.Data;

namespace Transliterator.Helpers;

internal class BoolToStateStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        bool boolValue = (bool)value;

        if (boolValue)
        {
            return "On";
        }
        return "Off";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string stringValue = value.ToString();

        if (stringValue == "On")
        {
            return true;
        }
        return false;
    }
}