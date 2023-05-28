using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.App.Services;
using Oiski.School.THOP.Services.Models;
using Polly;
using SkiaSharp;
using System.Diagnostics;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class GraphViewModel : ObservableObject
    {
        private readonly HumidexMauiService _service;
        [ObservableProperty]
        private QuickAction _quickActionFlag;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool _isNotBusy = false;
        [ObservableProperty]
        private DateTime _startDate;
        [ObservableProperty]
        private DateTime _endDate;
        [ObservableProperty]
        private TimeSpan _startTime;
        [ObservableProperty]
        private TimeSpan _endTime;

        #region Series Configuration
        [ObservableProperty]
        private ISeries[] _seriesCollection = new LineSeries<DateTimePoint>[]
        {
            new LineSeries<DateTimePoint>
            {
                Name = "Temperature"
            },
            new LineSeries<DateTimePoint>
            {
                Name = "Humidity"
            }
        };
        #endregion

        #region Axis Configuration
        [ObservableProperty]
        private Axis[] _xAxis =
        {
            new Axis
            {
                Name = "Timestamp",
                NamePaint = new SolidColorPaint (SKColors.Cyan),
                LabelsPaint = new SolidColorPaint (SKColors.DarkCyan),
                NameTextSize = 40,
                TextSize = 30,
                Labeler = (value) => new DateTime ((long)value).ToString("yy/MM/dd HH:mm"),
                LabelsRotation = 45
            }
        };

        [ObservableProperty]
        private Axis[] _yAxis =
        {
            new Axis
            {
                Name = "Values",
                NamePaint = new SolidColorPaint (SKColors.Cyan),
                LabelsPaint = new SolidColorPaint (SKColors.DarkCyan),
                NameTextSize = 40,
                TextSize = 30
            }
        };
        #endregion

        [ObservableProperty]
        private SolidColorPaint _legendTextColor = new SolidColorPaint
        {
            Color = SKColors.DarkCyan
        };

        public GraphViewModel(HumidexMauiService service)
        {
            _service = service;
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        public async Task QuickActionInputAsync(QuickAction action)
        {
            QuickActionFlag = action;

            var date = DateTime.Now;
            if (action != QuickAction.None)
            {
                EndDate = date.Date;
                EndTime = date.TimeOfDay;
            }

            switch (QuickActionFlag)
            {
                case QuickAction.Minutes60:
                    date = date.AddMinutes(-60);
                    StartDate = date.Date;
                    StartTime = date.TimeOfDay;
                    break;
                case QuickAction.Hours24:
                    date = date.AddHours(-24);
                    StartDate = date.Date;
                    StartTime = date.TimeOfDay;
                    break;
                case QuickAction.Days7:
                    date = date.AddDays(-7);
                    StartDate = date.Date;
                    StartTime = date.TimeOfDay;
                    break;
            }
            await UpdateChartAsync();

            QuickActionInputCommand.NotifyCanExecuteChanged();
        }

        public async Task UpdateChartAsync()
        {
            IsBusy = true;
            IsNotBusy = !IsBusy;
            var filter = new HumidexOptions
            {
                LocationId = _service.DeviceDetails.LocationId,
                EndTime = EndDate.Add(EndTime),
                StartTime = StartDate.Add(StartTime)
            };

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
                    Debug.WriteLine("Fetching Graph data");
                    return await _service.GetAllAsync(filter);
                });

            List<DateTimePoint> temperatureReadings = new List<DateTimePoint>();
            List<DateTimePoint> humidityReadings = new List<DateTimePoint>();

            foreach (var reading in readings)
            {
                temperatureReadings.Add(new DateTimePoint(reading.Time.Value.ToLocalTime(), reading.Temperature));
                humidityReadings.Add(new DateTimePoint(reading.Time.Value.ToLocalTime(), reading.Humidity));
            }

            SeriesCollection[0].Values = temperatureReadings;
            SeriesCollection[1].Values = humidityReadings;

            XAxis[0].MinLimit = XAxis[0].MaxLimit = null;
            YAxis[0].MinLimit = YAxis[0].MaxLimit = null;

            IsBusy = false;
            IsNotBusy = !IsBusy;
        }
    }
}
