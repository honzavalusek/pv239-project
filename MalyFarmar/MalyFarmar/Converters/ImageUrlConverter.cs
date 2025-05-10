// Converters/ImageUrlConverter.cs
using Microsoft.Maui.Controls;
using System;
using System.Globalization;

public class ImageUrlConverter : IValueConverter
{
    private const string ApiBaseUrl = "http://0.0.0.0:5138";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine("jsem v image url converteru");

        if (value is string relativeOrAbsoluteUrl && !string.IsNullOrWhiteSpace(relativeOrAbsoluteUrl))
        {
            if (relativeOrAbsoluteUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("vracím url :>" + ImageSource.FromUri(new Uri(relativeOrAbsoluteUrl)) + "<");
                return ImageSource.FromUri(new Uri(relativeOrAbsoluteUrl));
            }
            else
            {
                string baseUrl = ApiBaseUrl.TrimEnd('/');
                string relativeUrl = relativeOrAbsoluteUrl.TrimStart('/');
                string fullUrl = baseUrl + "/" + relativeUrl;
                Console.WriteLine("vracím url :>" + ImageSource.FromUri(new Uri(fullUrl)) + "<");
                return ImageSource.FromUri(new Uri(fullUrl));
            }
        }
        return "placeholder_product.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new object();
    }
}