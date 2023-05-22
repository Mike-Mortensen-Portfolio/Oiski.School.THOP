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
        private QuickAction _quickAction;

        [ObservableProperty]
        private GraphFilter _filter = new()
        {
            StartDate = DateTime.Now.AddHours(-24),
            EndDate = DateTime.Now
        };

        [ObservableProperty]
        private ISeries[] _series =
        {
            new LineSeries<ObservablePoint>
            {
                Values = new ObservablePoint[]
                {
                    new ObservablePoint(0, 4),
                    new ObservablePoint(1, 3),
                    new ObservablePoint(3, 8),
                    new ObservablePoint(18, 6),
                    new ObservablePoint(20, 12)
                }
            },
            new LineSeries<ObservablePoint>
            {
                Values = new ObservablePoint[]
                {
                    new ObservablePoint(0, 4),
                    new ObservablePoint(2, 67),
                    new ObservablePoint(18, 33),
                    new ObservablePoint(20, 2),
                    new ObservablePoint(43, 65)
                }
            },
            new LineSeries<ObservablePoint>
            {
                Values = new ObservablePoint[]
                {
                    new ObservablePoint(3, 0),
                    new ObservablePoint(12, 98),
                    new ObservablePoint(21, 8),
                    new ObservablePoint(45, 6),
                    new ObservablePoint(66, 87)
                }
            }
        };

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
                TextSize = 30
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
            QuickAction = action;

            switch (QuickAction)
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
