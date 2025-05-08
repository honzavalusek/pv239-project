// todo použít preferences na currentUserId
using System.Globalization;
using MalyFarmar.Services.Interfaces;

namespace MalyFarmar.Services;

public class PreferencesService(IPreferences preferences)
    : IPreferencesService
{
    public const string CurrentUserKey = "CurrentUserId";

    public string CurrentUserId
    {
        get => preferences.Get(CurrentUserKey, string.Empty);

        set => preferences.Set(CurrentUserKey, value);
    }
}