using System;
using System.Globalization;
using CommunityToolkit.Maui.Converters;
using MalyFarmar.Clients;            

namespace MalyFarmar.Converters
{
    public class IsOrderStatusCreatedConverter : BaseConverterOneWay<OrderStatusEnum, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; } = false;

        public override bool ConvertFrom(OrderStatusEnum value, CultureInfo? culture)
        {
            return value == OrderStatusEnum.Created;
        }
    }
}