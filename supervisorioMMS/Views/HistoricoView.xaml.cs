using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class HistoricoView : UserControl
    {
        public HistoricoView()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(-1);
            EndDatePicker.SelectedDate = DateTime.Today;
            LoadHistory();
        }

        private void LoadHistory()
        {
            var history = new List<HistoryEntry>
            {
                new HistoryEntry { Timestamp = DateTime.Now.AddHours(-1), Tag = "Temperatura Tanque 1", Value = "75.2 °C" },
                new HistoryEntry { Timestamp = DateTime.Now.AddHours(-1).AddMinutes(5), Tag = "Pressão Linha", Value = "1.21 bar" },
                new HistoryEntry { Timestamp = DateTime.Now.AddHours(-2), Tag = "Motor M-101", Value = "Ligado" },
                new HistoryEntry { Timestamp = DateTime.Now.AddHours(-3), Tag = "Motor M-101", Value = "Desligado" },
                new HistoryEntry { Timestamp = DateTime.Now.AddHours(-4), Tag = "Nível Tanque 1", Value = "62%" }
            };

            HistoryGrid.ItemsSource = history;
        }
    }

    public class HistoryEntry
    {
        public DateTime Timestamp { get; set; }
        public string Tag { get; set; }
        public string Value { get; set; }
    }
}