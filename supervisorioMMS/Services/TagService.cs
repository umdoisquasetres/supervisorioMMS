using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace supervisorioMMS.Services
{
    public enum ModbusDataType { Coil, HoldingRegister }

    public class ModbusTag : INotifyPropertyChanged
    {
        private object _value;

        public string Name { get; set; }
        public int Address { get; set; }
        public ModbusDataType DataType { get; set; }
        
        public object Value
        {
            get => _value;
            set
            {
                if (!object.Equals(_value, value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TagService
    {
        private static readonly Lazy<TagService> _instance = new Lazy<TagService>(() => new TagService());
        public static TagService Instance => _instance.Value;

        public ObservableCollection<ModbusTag> Tags { get; }
        private readonly DispatcherTimer _pollingTimer;

        private TagService()
        {
            Tags = new ObservableCollection<ModbusTag>
            {
                new ModbusTag { Name = "Motor M-101 Status", Address = 0, DataType = ModbusDataType.Coil, Value = false },
                new ModbusTag { Name = "Nível Tanque 1", Address = 0, DataType = ModbusDataType.HoldingRegister, Value = 0 }
            };

            _pollingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _pollingTimer.Tick += PollingTimer_Tick;
            _pollingTimer.Start();
        }

        private void PollingTimer_Tick(object sender, EventArgs e)
        {
            if (!ModbusService.Instance.IsConnected) return;

            foreach (var tag in Tags)
            {
                try
                {
                    switch (tag.DataType)
                    {
                        case ModbusDataType.Coil:
                            bool[] coilValues = ModbusService.Instance.ReadCoils(tag.Address, 1);
                            if (coilValues != null)
                            {
                                tag.Value = coilValues[0];
                            }
                            break;
                        case ModbusDataType.HoldingRegister:
                            int[] registerValues = ModbusService.Instance.ReadHoldingRegisters(tag.Address, 1);
                            if (registerValues != null)
                            {
                                tag.Value = registerValues[0];
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao ler tag '{tag.Name}': {ex.Message}");
                }
            }
        }

        public void WriteTagValue(string tagName, object value)
        {
            if (!ModbusService.Instance.IsConnected)
            {
                Console.WriteLine("Não é possível escrever o valor. Modbus não está conectado.");
                return;
            }

            var tag = Tags.FirstOrDefault(t => t.Name == tagName);
            if (tag == null)
            {
                Console.WriteLine($"Tag '{tagName}' não encontrada.");
                return;
            }

            try
            {
                switch (tag.DataType)
                {
                    case ModbusDataType.Coil:
                        if (value is bool boolValue)
                        {
                            ModbusService.Instance.WriteSingleCoil(tag.Address, boolValue);
                        }
                        break;
                    case ModbusDataType.HoldingRegister:
                        if (value is int intValue || (value is string s && int.TryParse(s, out intValue)))
                        {
                            ModbusService.Instance.WriteSingleRegister(tag.Address, intValue);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever na tag '{tag.Name}': {ex.Message}");
            }
        }
    }
}
