// todo použít preferences na currentUserId
using System.Globalization;
using MalyFarmar.Services.Interfaces;

namespace MalyFarmar.Services;

public class PreferencesService(IPreferences preferences) : IPreferencesService
{
    private const string currentUserKey = "CurrentUserId";

    public void SetCurrentUserId(int userId)
    {
        preferences.Set(currentUserKey, userId);
    }

    public int? GetCurrentUserId()
    {
        var result = preferences.Get(currentUserKey, -1);

        if (result == -1)
        {
            return null;
        }

        return result;
    }

    public void UnsetCurrentUserId()
    {
        preferences.Remove(currentUserKey);
    }
}