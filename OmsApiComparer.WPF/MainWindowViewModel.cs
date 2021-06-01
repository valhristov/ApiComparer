using System.Collections.Immutable;
using System.Linq;
using Rx;

namespace OmsApiComparer.WPF
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(ImmutableArray<RequestViewModel> requests)
        {
            RequestCategories = requests;

            SelectedRequestCategory = new ObservableProperty<RequestViewModel>(requests.First());
        }

        public ImmutableArray<RequestViewModel> RequestCategories { get; }

        public ObservableProperty<RequestViewModel> SelectedRequestCategory { get; }
    }
}
