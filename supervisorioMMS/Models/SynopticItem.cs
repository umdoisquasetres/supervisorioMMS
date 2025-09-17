using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace supervisorioMMS.Models
{
    public class SynopticItem : INotifyPropertyChanged
    {
        private double _x;
        private double _y;

        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SynopticMotor : SynopticItem
    {
        public string Label { get; set; } = "Motor";
    }

    public class SynopticSensor : SynopticItem
    {
        public string Label { get; set; } = "Sensor";
        public int Value { get; set; } = 123; // Valor de exemplo
    }
}
