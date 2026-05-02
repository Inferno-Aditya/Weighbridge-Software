using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VinayakEnterprises.Core.Services.Interfaces;

namespace VinayakEnterprises.Core.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly ISessionManager _sessionManager;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _licenseStatus = "Licensed"; // Placeholder

    public LoginViewModel(IAuthService authService, INavigationService navigationService, ISessionManager sessionManager)
    {
        _authService = authService;
        _navigationService = navigationService;
        _sessionManager = sessionManager;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        var result = await _authService.LoginAsync(Username, Password);

        IsLoading = false;

        if (result.Success)
        {
            if (_sessionManager.CurrentUser?.ForcePasswordChange == true)
            {
                _navigationService.NavigateTo<ChangePasswordViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<DashboardViewModel>();
            }
        }
        else
        {
            ErrorMessage = result.Message;
        }
    }
}
