using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using supervisorioMMS.Services;

namespace supervisorioMMS.Models
{
    public class SynopticItem : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private string _tagName = string.Empty;
        private ModbusTag? _linkedTag;

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

        public string TagName
        {
            get => _tagName;
            set
            {
                if (_tagName != value)
                {
                    _tagName = value;
                    OnPropertyChanged();
                    UpdateLinkedTag();
                }
            }
        }

        public ModbusTag? LinkedTag
        {
            get => _linkedTag;
            private set
            {
                if (_linkedTag != value)
                {
                    _linkedTag = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public SynopticItem()
        {
            TagService.Instance.Tags.CollectionChanged += (sender, e) => UpdateLinkedTag();
            UpdateLinkedTag();
        }

        private void UpdateLinkedTag()
        {
            LinkedTag = TagService.Instance.Tags.FirstOrDefault(t => t.Name == TagName);
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

        public void TurnOn()
        {
            if (LinkedTag != null && LinkedTag.DataType == ModbusDataType.Coil)
            {
                _ = TagService.Instance.WriteTagValueAsync(TagName, true);
            }
        }

        public void TurnOff()
        {
            if (LinkedTag != null && LinkedTag.DataType == ModbusDataType.Coil)
            {
                _ = TagService.Instance.WriteTagValueAsync(TagName, false);
            }
        }
    }

    public class SynopticSensor : SynopticItem
    {
        public string Label { get; set; } = "Sensor";
    }

    public class SynopticValve : SynopticItem
    {
        public string Label { get; set; } = "Válvula";

        public void Open()
        {
            if (LinkedTag != null && LinkedTag.DataType == ModbusDataType.Coil)
            {
                _ = TagService.Instance.WriteTagValueAsync(TagName, true);
            }
        }

        public void Close()
        {
            if (LinkedTag != null && LinkedTag.DataType == ModbusDataType.Coil)
            {
                _ = TagService.Instance.WriteTagValueAsync(TagName, false);
            }
        }
    }

    public class SynopticValueDisplay : SynopticItem
    {
        public string Label { get; set; } = "Display";
        public string Unit { get; set; } = "%";
    }
}
