using System.Globalization;
using CommunityToolkit.Maui.Converters;
using MalyFarmar.Options;
using Microsoft.Extensions.Options;

namespace MalyFarmar.Converters;

public class ImageUrlConverter : BaseConverterOneWay<string?, ImageSource>
{
    private readonly string _apiBaseUrl;
    public override ImageSource DefaultConvertReturnValue { get; set; } = "placeholder_product.png";

    public ImageUrlConverter(IOptions<ApiOptions> apiOptions)
    {
        _apiBaseUrl = apiOptions.Value.BaseUrl;
    }
    
    public override ImageSource ConvertFrom(string? value, CultureInfo? culture)
    {
        if (value == null || string.IsNullOrWhiteSpace(value))
        {
            return DefaultConvertReturnValue;
        }
        
        string baseUrl = _apiBaseUrl.TrimEnd('/');
        string relativeUrl = value.TrimStart('/');
        string fullUrl = baseUrl + "/" + relativeUrl;
        
        return ImageSource.FromUri(new Uri(fullUrl));
    }
}