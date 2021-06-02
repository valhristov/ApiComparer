using System.Collections.Immutable;
using System.Linq;
using Rx;

namespace OmsApiComparer.WPF
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(ImmutableArray<RequestViewModel> requests)
        {
            Requests = requests;

            SelectedRequest = new ObservableProperty<RequestViewModel>(requests.First());
        }

        public ImmutableArray<RequestViewModel> Requests { get; }

        public ObservableProperty<RequestViewModel> SelectedRequest { get; }
    }
}
