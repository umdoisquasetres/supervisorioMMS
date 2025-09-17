using System.Windows;
using supervisorioMMS.Views;

namespace supervisorioMMS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new PrincipalView();
        }

        private void NavPrincipal_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PrincipalView();
        }

        private void NavAlarmes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AlarmesView();
        }

        private void NavHistorico_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new HistoricoView();
        }

        private void NavGraficos_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new GraficosView();
        }

        private void NavConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ConfiguracoesView();
        }
    }
}
