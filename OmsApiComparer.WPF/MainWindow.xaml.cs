using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Windows;

namespace OmsApiComparer.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var ruRequests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

            DataContext = new MainWindowViewModel(
                ruRequests
                    .GroupBy(x => x.FullPath)
                    .Select(x => new RequestCategoryViewModel(x))
                    .ToImmutableArray()
                );
        }
    }
}
