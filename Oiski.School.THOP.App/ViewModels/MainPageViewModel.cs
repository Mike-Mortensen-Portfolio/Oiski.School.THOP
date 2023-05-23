using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.App.Services;
using Polly;
using System.Diagnostics;

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

            var readings = await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider:
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (ex, time) =>
                {
                    Debug.WriteLine($"An error occured: {ex}, trying again...");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine("Fetching latest reading");

                    return await _humidexService.GetAllAsync(new HumidexOptions
                    {
                        MaxCount = 1
                    });
                });

            Humidex = readings
                .FirstOrDefault();

            IsBusy = false;
        }

        [RelayCommand]
        async partial void OnVentilationOnChanged(bool value)
        {
            var result = await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider:
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (ex, time) =>
                {
                    Debug.WriteLine($"An error occured: {ex}, trying again...");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine("Sending vent control data");
                    return await _peripheralService.OpenVentsAsync("home", "oiski_1010", value);
                });
        }
    }
}
