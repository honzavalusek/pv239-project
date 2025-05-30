﻿using MalyFarmar.Clients;
using MalyFarmar.Converters;
using MalyFarmar.Options;
using MalyFarmar.Pages;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels;
using Microsoft.Extensions.Options;

namespace MalyFarmar;

public partial class App : Application
{
    private Window? _mainWindow = null;
    private readonly AppShell _appShell;
    private readonly LoginPage _loginPage;

    public App(IOptions<ApiOptions> apiOptions, AppShell appShell, LoginPage loginPage)
    {
        InitializeComponent();

        var imageToUrlConverter = new ImageUrlConverter(apiOptions);
        Resources.Add("ImageUrlConverter", imageToUrlConverter);

        _appShell = appShell;
        _loginPage = loginPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        _mainWindow = new Window(new NavigationPage(_loginPage));

        return _mainWindow;
    }

    public void SwitchToShell()
    {
        if (_mainWindow != null)
        {
            _mainWindow.Page = _appShell;
        }
    }
}