using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using supervisorioMMS.Models;
using supervisorioMMS.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace supervisorioMMS
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private PrincipalView? _principalView;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _principalView = _serviceProvider.GetRequiredService<PrincipalView>();
            MainContent.Content = _principalView;
        }

        private void NavPrincipal_Click(object sender, RoutedEventArgs e)
        {
            if (_principalView == null)
            {
                _principalView = _serviceProvider.GetRequiredService<PrincipalView>();
            }
            MainContent.Content = _principalView;
        }

        private void NavAlarmes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _serviceProvider.GetRequiredService<AlarmesView>();
        }

        private void NavHistorico_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _serviceProvider.GetRequiredService<HistoricoView>();
        }

        private void NavGraficos_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _serviceProvider.GetRequiredService<GraficosView>();
        }

        private void NavConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _serviceProvider.GetRequiredService<ConfiguracoesView>();
        }

        private void NavUsuarios_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _serviceProvider.GetRequiredService<UsuariosView>();
        }

        private void NavTags_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _serviceProvider.GetRequiredService<TagConfigView>();
        }

        private async void SaveSynoptic_Click(object sender, RoutedEventArgs e)
        {
            if (_principalView == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON File (*.json)|*.json",
                Title = "Salvar Layout Sinótico",
                DefaultExt = "json",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        Formatting = Formatting.Indented
                    };
                    var jsonString = JsonConvert.SerializeObject(_principalView.SynopticItems, settings);
                    await File.WriteAllTextAsync(saveFileDialog.FileName, jsonString);
                    MessageBox.Show("Layout salvo com sucesso!", "Salvar", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar o layout: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void LoadSynoptic_Click(object sender, RoutedEventArgs e)
        {
            if (_principalView == null)
            {
                _principalView = _serviceProvider.GetRequiredService<PrincipalView>();
                MainContent.Content = _principalView;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON File (*.json)|*.json",
                Title = "Carregar Layout Sinótico"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var jsonString = await File.ReadAllTextAsync(openFileDialog.FileName);
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    };
                    var loadedItems = JsonConvert.DeserializeObject<ObservableCollection<SynopticItem>>(jsonString, settings);

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
}
