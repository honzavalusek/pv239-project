using CommunityToolkit.Mvvm.ComponentModel;

namespace MalyFarmar.ViewModels.Shared;

public abstract class BaseViewModel : ObservableObject
{
    protected bool ForceDataRefresh = true;

    public virtual async Task OnAppearingAsync()
    {
        if (ForceDataRefresh)
        {
            await LoadDataAsync();

            ForceDataRefresh = false;
        }
    }

    protected virtual Task LoadDataAsync()
        => Task.CompletedTask;
}