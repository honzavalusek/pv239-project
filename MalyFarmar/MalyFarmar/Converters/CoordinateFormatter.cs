using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace MalyFarmar.Converters;

public class CoordinateFormatter : BaseConverterOneWay<double, string>
{
    public override string DefaultConvertReturnValue { get; set; } = "0.00";

    public override string ConvertFrom(double value, CultureInfo? culture)
    {
        return value.ToString("F6", CultureInfo.InvariantCulture);
    }

}