using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Oiski.School.THOP.App.Services;
using Oiski.School.THOP.App.ViewModels;
using System.Globalization;

namespace Oiski.School.THOP.App.Views;

public partial class GraphPage : ContentPage
{
    private readonly ApiService _service;

    public GraphPage(GraphViewModel viewModel, ApiService service)
    {
        InitializeComponent();

        ViewModel = viewModel;
        _service = service;
        BindingContext = ViewModel;
    }

    public GraphViewModel ViewModel { get; }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var readings = await _service.GetAllAsync();

        List<DateTimePoint> temperatureReadings = new List<DateTimePoint>();
        List<DateTimePoint> humidityReadings = new List<DateTimePoint>();

        foreach (var reading in readings)
        {
            temperatureReadings.Add(new DateTimePoint(reading.Time.Value, reading.Temperature));
            humidityReadings.Add(new DateTimePoint(reading.Time.Value, reading.Humidity));
        }

        ViewModel.Series = new LineSeries<DateTimePoint>[]
        {
            new LineSeries<DateTimePoint>
            {
                Name = "Temperature",
                Values = temperatureReadings,
            },
            new LineSeries<DateTimePoint>
            {
                Name = "Humidity",
                Values = humidityReadings
            }
        };
    }
}