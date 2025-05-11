namespace MalyFarmar.Services.Interfaces;

public interface IPreferencesService
{
    public void SetCurrentUserId(int userId);
    public int? GetCurrentUserId();
    public void UnsetCurrentUserId();
}