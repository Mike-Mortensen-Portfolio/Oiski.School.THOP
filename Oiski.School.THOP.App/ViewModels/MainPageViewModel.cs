using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oiski.School.THOP.App.Services;
using Oiski.School.THOP.Services.Models;
using Polly;
using System.Diagnostics;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly HumidexMauiService _humidexService;
        private readonly PeripheralMauiService _peripheralService;
        private readonly CacheService _cacheService;
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool _isNotBusy = true;
        [ObservableProperty]
        private bool ventilationOn = false;
        [ObservableProperty]
        private bool _lightsOn = false;
        [ObservableProperty]
        private HumidexDto _humidex;
        [ObservableProperty]
        private DeviceDetails _deviceDetails;
        [ObservableProperty]
        private string _title;


        public MainPageViewModel(HumidexMauiService humidexService, PeripheralMauiService peripheralService, CacheService cacheService)
        {
            _humidexService = humidexService;
            _peripheralService = peripheralService;
            _cacheService = cacheService;
            _deviceDetails = humidexService.DeviceDetails;
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsBusy = true;
            IsNotBusy = !IsBusy;

            var attemptContainer = 0;
            try
            {

                var readings = await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider:
                attempt =>
                {
                    attemptContainer = attempt;
                    return TimeSpan.FromSeconds(Math.Pow(2, attempt));
                },
                onRetry: (ex, time) =>
                {
                    Debug.WriteLine($"An error occured (Attempt: {attemptContainer}- trying again in: {time}...): {ex}");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine("Fetching latest reading");

                    return await _humidexService.GetAllAsync(new HumidexOptions
                    {
                        LocationId = _humidexService.DeviceDetails.LocationId,
                        MaxCount = 1
                    });
                });

                Humidex = readings
                .FirstOrDefault();

                if (Humidex != null)
                    await _cacheService.CacheState(Humidex);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An error occured: {e.Message}");
            }

            Humidex ??= await _cacheService.GetCache<HumidexDto>();
            Title = $"Latest Readings ({Humidex.LocationId})";

            IsBusy = false;
            IsNotBusy = !IsBusy;

            RefreshCommand?.NotifyCanExecuteChanged();
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
                    return await _peripheralService.OpenVentsAsync(_humidexService.DeviceDetails.LocationId, _humidexService.DeviceDetails.DeviceId, value);
                });
        }

        [RelayCommand]
        async partial void OnLightsOnChanged(bool value)
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
                Debug.WriteLine("Sending light control data");
                return await _peripheralService.LightsOnAsync(_humidexService.DeviceDetails.LocationId, _humidexService.DeviceDetails.DeviceId, value);
            });
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        public async Task SetDeviceDetails()
        {
            _humidexService.DeviceDetails = this.DeviceDetails;

            await RefreshCommand?.ExecuteAsync(null);
        }
    }
}
