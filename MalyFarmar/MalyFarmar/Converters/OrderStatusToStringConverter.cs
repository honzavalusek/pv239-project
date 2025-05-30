using System.Globalization;
using CommunityToolkit.Maui.Converters;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.Converters;

public class OrderStatusToStringConverter : BaseConverterOneWay<OrderStatusEnum, string>
{
    public override string DefaultConvertReturnValue { get; set; } = MyReservationsPageStrings.OrderStatusUnknown;

    public override string ConvertFrom(OrderStatusEnum value, CultureInfo? culture)
    {
        switch (value)
        {
            case OrderStatusEnum.Created:
                return MyReservationsPageStrings.OrderStatusCreated;
            case OrderStatusEnum.PickUpSet:
                return MyReservationsPageStrings.OrderStatusPickUpSet;
            case OrderStatusEnum.Completed:
                return MyReservationsPageStrings.OrderStatusCompleted;
            default:
                return DefaultConvertReturnValue + $" ({value})";
        }
    }
}