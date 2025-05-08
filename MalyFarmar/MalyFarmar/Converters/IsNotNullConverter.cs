using System.Globalization;

namespace MalyFarmar.Converters
{
    
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null;
    }
}
