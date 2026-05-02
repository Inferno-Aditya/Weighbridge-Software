using System.Windows;
using VinayakEnterprises.Core.ViewModels;

namespace VinayakEnterprises.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}