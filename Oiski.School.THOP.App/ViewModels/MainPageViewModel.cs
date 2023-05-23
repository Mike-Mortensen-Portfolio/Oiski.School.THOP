using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.App.Services;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly HumidexService _humidexService;
        private readonly PeripheralService _peripheralService;
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool ventilationOn = false;
        [ObservableProperty]
        private HumidexDto _humidex;


        public MainPageViewModel(HumidexService humidexService, PeripheralService peripheralService)
        {
            _humidexService = humidexService;
            _peripheralService = peripheralService;
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsBusy = true;

            var readings = await _humidexService.GetAllAsync(new HumidexOptions
            {
                EndTime = DateTime.Now,
                StartTime = DateTime.Now.AddMinutes(-1)
            });
            Humidex = readings
                .OrderBy(humidex => humidex.Time)
                .TakeLast(1)
                .FirstOrDefault();

            IsBusy = false;
        }

        [RelayCommand]
        async partial void OnVentilationOnChanged(bool value)
        {
            await _peripheralService.OpenVentsAsync("home", "oiski_1010", value);
        }
    }
}
