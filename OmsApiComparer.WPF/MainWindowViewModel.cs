using System.Collections.Immutable;
using System.Linq;
using Rx;

namespace OmsApiComparer.WPF
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(ImmutableArray<RequestCategoryViewModel> requests)
        {
            RequestCategories = requests;

            SelectedRequestCategory = new ObservableProperty<RequestCategoryViewModel>(requests.First());
        }

        public ImmutableArray<RequestCategoryViewModel> RequestCategories { get; }

        public ObservableProperty<RequestCategoryViewModel> SelectedRequestCategory { get; }
    }
}
