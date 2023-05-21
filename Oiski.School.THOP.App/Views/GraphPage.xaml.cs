using CommunityToolkit.Mvvm.ComponentModel;
using Oiski.School.THOP.App.ViewModels;

namespace Oiski.School.THOP.App.Views;

public partial class GraphPage : ContentPage
{
    private readonly HttpClient _client;

    public GraphPage(GraphViewModel viewModel, HttpClient client)
    {
        InitializeComponent();

        _client = client;

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }

    public GraphViewModel ViewModel { get; }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}