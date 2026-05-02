using CommunityToolkit.Mvvm.ComponentModel;

namespace VinayakEnterprises.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    public MainViewModel()
    {
    }
}
