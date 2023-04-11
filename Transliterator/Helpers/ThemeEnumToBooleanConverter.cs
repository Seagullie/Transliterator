using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace Transliterator.Helpers;

public class ThemeEnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
            throw new ArgumentException("ExceptionThemeEnumToBooleanConverterParameterMustBeAnEnumName");

        if (!Enum.IsDefined(typeof(ThemeType), value))
            throw new ArgumentException("ExceptionThemeEnumToBooleanConverterValueMustBeAnEnum");

        var enumValue = Enum.Parse(typeof(ThemeType), enumString);

        return enumValue.Equals(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string enumString)
            throw new ArgumentException("ExceptionThemeEnumToBooleanConverterParameterMustBeAnEnumName");

        return Enum.Parse(typeof(ThemeType), enumString);
    }
}