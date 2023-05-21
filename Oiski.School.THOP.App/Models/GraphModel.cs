using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;

namespace Oiski.School.THOP.App.Models
{
    public partial class GraphModel : ObservableObject
    {
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

        [ObservableProperty]
        private SolidColorPaint _legendTextColor = new SolidColorPaint
        {
            Color = SKColors.DarkCyan
        };
    }
}
