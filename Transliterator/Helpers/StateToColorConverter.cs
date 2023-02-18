using System;
using System.Windows.Data;

namespace Transliterator.Helpers;

// TODO: Rewrite to BoolToColorConvertor
public class StateToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string val = value.ToString();
        if (val == "On")
        {
            return "Green";
        }
        else
        {
            return "Red";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}