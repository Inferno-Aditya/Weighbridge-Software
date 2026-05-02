using CommunityToolkit.Mvvm.ComponentModel;

namespace VinayakEnterprises.Core.Services.Interfaces;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
}
