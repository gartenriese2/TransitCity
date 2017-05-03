using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shapes;

namespace WpfDrawing.Converter
{
    public class ElementToXConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rectangle)
            {
                return 100;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
