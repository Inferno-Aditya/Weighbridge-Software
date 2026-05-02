using System.Windows;
using System.Windows.Controls;
using VinayakEnterprises.Core.ViewModels.Auth;

namespace VinayakEnterprises.App.Views.Auth;

public partial class ChangePasswordView : UserControl
{
    public ChangePasswordView()
    {
        InitializeComponent();
    }

    private void TxtOldPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm) vm.OldPassword = TxtOldPassword.Password;
    }

    private void TxtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm) vm.NewPassword = TxtNewPassword.Password;
    }

    private void TxtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm) vm.ConfirmPassword = TxtConfirmPassword.Password;
    }
}
