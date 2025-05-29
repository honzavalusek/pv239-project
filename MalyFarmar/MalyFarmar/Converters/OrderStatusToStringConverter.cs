using System.Globalization;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.Converters
{
    public class OrderStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatusEnum status)
            {
                switch (status)
                {
                    case OrderStatusEnum._1:
                        return MyReservationsPageStrings.OrderStatusCreated;
                    case OrderStatusEnum._2:
                        return MyReservationsPageStrings.OrderStatusPickUpSet;
                    case OrderStatusEnum._3:
                        return MyReservationsPageStrings.OrderStatusCompleted;
                    default:
                        return MyReservationsPageStrings.OrderStatusUnknown + $" ({status})";
                }
            }
            return string.Empty; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Converting string back to OrderStatusEnum is not implemented.");
        }
    }
}
