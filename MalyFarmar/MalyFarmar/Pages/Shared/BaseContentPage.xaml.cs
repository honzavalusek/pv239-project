using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.Pages.Shared;

public partial class BaseContentPage : ContentPage
{
    public BaseContentPage(BaseViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BaseViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }
}