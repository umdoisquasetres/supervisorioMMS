using System;
using System.Collections.Generic;
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
            var alarms = new List<Alarm>
            {
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-5), Description = "Falha no motor M-101", Priority = "Alta", Status = "Ativo" },
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-10), Description = "Pressão alta no tanque 1", Priority = "Média", Status = "Ativo" },
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-15), Description = "Nível baixo no tanque 2", Priority = "Baixa", Status = "Reconhecido" },
                new Alarm { Timestamp = DateTime.Now.AddMinutes(-20), Description = "Falha de comunicação com CLP", Priority = "Crítica", Status = "Ativo" }
            };

            AlarmsGrid.ItemsSource = alarms;
        }
    }

    public class Alarm
    {
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}