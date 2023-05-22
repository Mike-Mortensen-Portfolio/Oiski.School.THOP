using CommunityToolkit.Mvvm.ComponentModel;

namespace Oiski.School.THOP.App.Models
{
    public partial class GraphFilter : ObservableObject
    {
        [ObservableProperty]
        private DateTime _startDate;
        [ObservableProperty]
        private DateTime _endDate;
    }
}
