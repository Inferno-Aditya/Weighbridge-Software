using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VinayakEnterprises.Core.Services.Interfaces;

namespace VinayakEnterprises.Core.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly ISessionManager _sessionManager;

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    public DashboardViewModel(IAuthService authService, INavigationService navigationService, ISessionManager sessionManager)
    {
        _authService = authService;
        _navigationService = navigationService;
        _sessionManager = sessionManager;

        if (_sessionManager.CurrentUser != null && _sessionManager.CurrentRole != null)
        {
            WelcomeMessage = $"Welcome, {_sessionManager.CurrentUser.Username} ({_sessionManager.CurrentRole.Name})";
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        _navigationService.NavigateTo<Auth.LoginViewModel>();
    }
}
