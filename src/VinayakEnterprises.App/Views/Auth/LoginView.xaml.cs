using System.Windows;
using System.Windows.Controls;
using VinayakEnterprises.Core.ViewModels.Auth;

namespace VinayakEnterprises.App.Views.Auth;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = TxtPassword.Password;
        }
    }
}
