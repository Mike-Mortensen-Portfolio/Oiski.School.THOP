using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Oiski.School.THOP.App.Models;
using SkiaSharp;

namespace Oiski.School.THOP.App.ViewModels
{
    public partial class GraphViewModel : ObservableObject
    {
        [ObservableProperty]
        private QuickAction _quickActionFlag;

        [ObservableProperty]
        private GraphFilter _filter = new()
        {
            StartDate = DateTime.Now.AddHours(-24),
            EndDate = DateTime.Now
        };

        #region Series Configuration
        [ObservableProperty]
        private ISeries[] _series =
        {
            new LineSeries<DateTimePoint>
            {
                Name = "Dummy",
                Values = new DateTimePoint[]
                {
                    new DateTimePoint(DateTime.Now.AddDays (-5), 1),
                    new DateTimePoint(DateTime.Now.AddDays (-4), 2),
                    new DateTimePoint(DateTime.Now.AddDays (-3), 3),
                    new DateTimePoint(DateTime.Now.AddDays (-2), 4),
                    new DateTimePoint(DateTime.Now.AddDays (-1), 5)
                }
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
                Labeler = (value) => new DateTime ((long)value).ToString("yy/MM/dd HH:mm")
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

        [RelayCommand]
        public void QuickActionInput(QuickAction action)
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
        }
    }
}
