using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.App.Services;
using System.Net.Http.Json;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ApiService _service;
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool ventilationOn = false;
        [ObservableProperty]
        private HumidexDto _humidex;


        public MainPageViewModel(ApiService service)
        {
            _service = service;
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsBusy = true;

            var readings = await _service.GetAllAsync();
            Humidex = readings
                .OrderBy(humidex => humidex.Time)
                .TakeLast(1)
                .FirstOrDefault();

            IsBusy = false;
        }

        [RelayCommand]
        partial void OnVentilationOnChanged(bool value)
        {

        }
    }
}
