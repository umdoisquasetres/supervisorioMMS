using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace supervisorioMMS.Views
{
    public partial class GraficosView : UserControl
    {
        public GraficosView()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Temperatura",
                    Values = new ChartValues<double> { 75, 76, 78, 77, 79, 80, 79 }
                },
                new LineSeries
                {
                    Title = "Pressão",
                    Values = new ChartValues<double> { 1.2, 1.3, 1.2, 1.4, 1.3, 1.2, 1.3 }
                }
            };

            Labels = new[] { "10:00", "10:01", "10:02", "10:03", "10:04", "10:05", "10:06" };

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
    }
}