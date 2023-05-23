using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.App.Services;
using SkiaSharp;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class GraphViewModel : ObservableObject
    {
        private readonly HumidexService _service;
        [ObservableProperty]
        private QuickAction _quickActionFlag;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool _isNotBusy = false;

        [ObservableProperty]
        private GraphFilter _filter = new()
        {
            StartDate = DateTime.Now.AddHours(-24),
            EndDate = DateTime.Now
        };

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

        public GraphViewModel(HumidexService service)
        {
            _service = service;
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        public async Task QuickActionInputAsync(QuickAction action)
        {
            QuickActionFlag = action;

            switch (QuickActionFlag)
            {
                case QuickAction.Minutes60:
                    Filter.StartDate = DateTime.Now.AddMinutes(-60);
                    break;
                case QuickAction.Hours24:
                    Filter.StartDate = DateTime.Now.AddHours(-24);
                    break;
                case QuickAction.Days7:
                    Filter.StartDate = DateTime.Now.AddDays(-7);
                    break;
            }

            Filter.EndDate = DateTime.Now;
            await UpdateChartAsync();

            QuickActionInputCommand.NotifyCanExecuteChanged();
        }

        public async Task UpdateChartAsync()
        {
            IsBusy = true;
            IsNotBusy = !IsBusy;
            var filter = new HumidexOptions
            {
                EndTime = Filter.EndDate,
                StartTime = Filter.StartDate
            };

            var readings = await _service.GetAllAsync(filter);

            List<DateTimePoint> temperatureReadings = new List<DateTimePoint>();
            List<DateTimePoint> humidityReadings = new List<DateTimePoint>();

            foreach (var reading in readings)
            {
                temperatureReadings.Add(new DateTimePoint(reading.Time.Value.ToLocalTime(), reading.Temperature));
                humidityReadings.Add(new DateTimePoint(reading.Time.Value.ToLocalTime(), reading.Humidity));
            }

            SeriesCollection[0].Values = temperatureReadings;
            SeriesCollection[1].Values = humidityReadings;

            IsBusy = false;
            IsNotBusy = !IsBusy;
        }
    }
}
