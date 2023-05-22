using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool ventilationOn = false;

        [RelayCommand]
        private void Refresh()
        {
            IsBusy = true;

            IsBusy = false;
        }

        [RelayCommand]
        partial void OnVentilationOnChanged(bool value)
        {

        }
    }
}
