using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class AlarmesView : UserControl
    {
        public AlarmesView()
        {
            InitializeComponent();
            LoadAlarms();
        }

        private void LoadAlarms()
        {
            var alarms = new ObservableCollection<Alarm>
            {
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-5), Description = "Falha no motor M-101", Priority = "Alta", Status = "Ativo" },
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-10), Description = "Pressão alta no tanque 1", Priority = "Média", Status = "Ativo" },
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-15), Description = "Nível baixo no tanque 2", Priority = "Baixa", Status = "Reconhecido" },
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-20), Description = "Falha de comunicação com CLP", Priority = "Crítica", Status = "Ativo" }
            };

            AlarmsGrid.ItemsSource = alarms;
        }

        private void AcknowledgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmsGrid.SelectedItem is Alarm selectedAlarm)
            {
                if (selectedAlarm.Status == "Ativo")
                {
                    selectedAlarm.Status = "Reconhecido";
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um alarme para reconhecer.", "Nenhum Alarme Selecionado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class Alarm : INotifyPropertyChanged
    {
        private string _status = string.Empty;

        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
