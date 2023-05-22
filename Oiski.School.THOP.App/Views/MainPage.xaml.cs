using Oiski.School.THOP.App.ViewModels;

namespace Oiski.School.THOP.App.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}