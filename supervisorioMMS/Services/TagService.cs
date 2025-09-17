using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                new ModbusTag { Name = "Motor_M101_Status", Address = 0, DataType = ModbusDataType.Coil, Value = false },
                new ModbusTag { Name = "Valvula_V101_Status", Address = 1, DataType = ModbusDataType.Coil, Value = false },
                new ModbusTag { Name = "Nivel_Tanque_T101", Address = 0, DataType = ModbusDataType.HoldingRegister, Value = 75 },
                new ModbusTag { Name = "Temperatura_Tanque_T101", Address = 1, DataType = ModbusDataType.HoldingRegister, Value = 25 },
                new ModbusTag { Name = "Sensor_Pressao_P101", Address = 2, DataType = ModbusDataType.HoldingRegister, Value = 1 }
            };

            _pollingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _pollingTimer.Tick += PollingTimer_Tick;
            _pollingTimer.Start();
        }

        private async void PollingTimer_Tick(object sender, EventArgs e)
        {
            if (!ModbusService.Instance.IsConnected) return;

            foreach (var tag in Tags.ToList())
            {
                try
                {
                    switch (tag.DataType)
                    {
                        case ModbusDataType.Coil:
                            bool[] coilValues = await ModbusService.Instance.ReadCoilsAsync(tag.Address, 1);
                            if (coilValues != null && coilValues.Length > 0)
                            {
                                tag.Value = coilValues[0];
                            }
                            break;
                        case ModbusDataType.HoldingRegister:
                            int[] registerValues = await ModbusService.Instance.ReadHoldingRegistersAsync(tag.Address, 1);
                            if (registerValues != null && registerValues.Length > 0)
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

        public async Task WriteTagValueAsync(string tagName, object value)
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
                            await ModbusService.Instance.WriteSingleCoilAsync(tag.Address, boolValue);
                        }
                        break;
                    case ModbusDataType.HoldingRegister:
                        if (value is int intValue || (value is string s && int.TryParse(s, out intValue)))
                        {
                            await ModbusService.Instance.WriteSingleRegisterAsync(tag.Address, intValue);
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