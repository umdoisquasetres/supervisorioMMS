using System.Windows;
using supervisorioMMS.Views;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using supervisorioMMS.Models;

namespace supervisorioMMS
{
    public partial class MainWindow : Window
    {
        private PrincipalView? _principalView;
        private const string LayoutFileName = "synoptic_layout.json";

        public MainWindow()
        {
            InitializeComponent();
            _principalView = new PrincipalView();
            MainContent.Content = _principalView;
        }

        private void NavPrincipal_Click(object sender, RoutedEventArgs e)
        {
            _principalView = new PrincipalView();
            MainContent.Content = _principalView;
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

        private void NavUsuarios_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UsuariosView();
        }

        private void NavTags_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TagConfigView();
        }

        private async void SaveSynoptic_Click(object sender, RoutedEventArgs e)
        {
            if (_principalView == null) return;

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(_principalView.SynopticItems, options);
                await File.WriteAllTextAsync(LayoutFileName, jsonString);
                MessageBox.Show("Layout salvo com sucesso!", "Salvar", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar o layout: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadSynoptic_Click(object sender, RoutedEventArgs e)
        {
            if (_principalView == null) return;

            try
            {
                if (!File.Exists(LayoutFileName))
                {
                    MessageBox.Show("Nenhum layout salvo encontrado.", "Carregar", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var jsonString = await File.ReadAllTextAsync(LayoutFileName);
                var loadedItems = JsonSerializer.Deserialize<ObservableCollection<SynopticItem>>(jsonString);

                if (loadedItems != null)
                {
                    _principalView.SynopticItems.Clear();
                    foreach (var item in loadedItems)
                    {
                        _principalView.SynopticItems.Add(item);
                    }
                    MessageBox.Show("Layout carregado com sucesso!", "Carregar", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o layout: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
