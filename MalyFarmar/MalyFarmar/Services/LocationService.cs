using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;

namespace MalyFarmar.Services;

public class LocationService : ILocationService
{
    public async Task<LocationResult> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }


            if (status != PermissionStatus.Granted)
            {
                return new LocationResult
                {
                    ErrorMessage = LocationStrings.LocationPermissionDeniedMessage
                };
            }

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var cancelTokenSource = new CancellationTokenSource();
            Location location = await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);

            if (location == null)
            {
                return new LocationResult
                {
                    ErrorMessage = LocationStrings.UnableToRetrieveLocationMessage
                };
            }

            return new LocationResult
            {
                Location = location
            };
        }
        catch (FeatureNotSupportedException)
        {
            return new LocationResult()
            {
                ErrorMessage = LocationStrings.LocationNotSupportedMessage
            };
        }
        catch (FeatureNotEnabledException)
        {
            return new LocationResult()
            {
                ErrorMessage = LocationStrings.LocationServicesNotEnabledMessage
            };
        }
        catch (PermissionException)
        {
            return new LocationResult()
            {
                ErrorMessage = LocationStrings.LocationPermissionNotGrantedMessage
            };
        }
        catch (Exception e)
        {
            return new LocationResult()
            {
                ErrorMessage = $"{CommonStrings.Error}: {e.Message}"
            };
        }
    }
}