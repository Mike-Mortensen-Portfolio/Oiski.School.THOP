using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Oiski.School.THOP.App.Models;
using Oiski.School.THOP.App.Services;
using Oiski.School.THOP.App.ViewModels;
using Oiski.School.THOP.Services.Models;
using System.Globalization;

namespace Oiski.School.THOP.App.Views;

public partial class GraphPage : ContentPage
{
    private readonly HumidexMauiService _service;

    public GraphPage(GraphViewModel viewModel, HumidexMauiService service)
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

        await ViewModel.QuickActionInputCommand.ExecuteAsync(QuickAction.Hours24);
    }
}