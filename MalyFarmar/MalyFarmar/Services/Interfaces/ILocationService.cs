namespace MalyFarmar.Services.Interfaces;

public class LocationResult
{
    public Location? Location { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface ILocationService
{
    public Task<LocationResult> GetCurrentLocationAsync();
}