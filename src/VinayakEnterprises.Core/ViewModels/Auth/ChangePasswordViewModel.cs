using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VinayakEnterprises.Core.Services.Interfaces;

namespace VinayakEnterprises.Core.ViewModels.Auth;

public partial class ChangePasswordViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly ISessionManager _sessionManager;

    [ObservableProperty]
    private string _oldPassword = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ChangePasswordViewModel(IAuthService authService, INavigationService navigationService, ISessionManager sessionManager)
    {
        _authService = authService;
        _navigationService = navigationService;
        _sessionManager = sessionManager;
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "New password and confirmation do not match.";
            return;
        }

        if (NewPassword.Length < 6)
        {
            ErrorMessage = "New password must be at least 6 characters.";
            return;
        }

        var currentUser = _sessionManager.CurrentUser;
        if (currentUser == null)
        {
            ErrorMessage = "Session expired. Please log in again.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        bool success = await _authService.ChangePasswordAsync(currentUser.Id, OldPassword, NewPassword);

        IsLoading = false;

        if (success)
        {
            // After successful password change, go to dashboard
            _navigationService.NavigateTo<DashboardViewModel>();
        }
        else
        {
            ErrorMessage = "Incorrect old password or failed to change password.";
        }
    }
}
