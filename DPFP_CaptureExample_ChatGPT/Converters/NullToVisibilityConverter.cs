using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DPFP_CaptureExample_ChatGPT.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        // Si el valor es null → Collapsed
        // Si el valor NO es null → Visible
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter?.ToString() == "Invert";

            bool isNull = value == null;

            if (invert)
                isNull = !isNull;

            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        // No lo usamos en este caso
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
