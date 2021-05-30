using System.Collections.Immutable;
using System.Linq;
using Rx;

namespace OmsApiComparer.WPF
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(ImmutableArray<RequestComparisonViewModel> requests)
        {
            RequestCategories = requests;

            SelectedRequestCategory = new ObservableProperty<RequestComparisonViewModel>(requests.First());
        }

        public ImmutableArray<RequestComparisonViewModel> RequestCategories { get; }

        public ObservableProperty<RequestComparisonViewModel> SelectedRequestCategory { get; }
    }
}
