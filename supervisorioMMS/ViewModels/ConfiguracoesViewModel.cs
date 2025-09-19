using supervisorioMMS.Services;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace supervisorioMMS.ViewModels
{
    public class ConfiguracoesViewModel : BaseViewModel
    {
        private bool _isRtuSelected = true;
        private string _selectedComPort;
        private int _selectedBaudRate;
        private int _selectedDataBits;
        private Parity _selectedParity;
        private StopBits _selectedStopBits;
        private string _ipAddress = "127.0.0.1";
        private string _port = "502";
        private bool _isConnected;
        private bool _isConnecting;

        public List<string> ConnectionTypes => new List<string> { "Modbus RTU (Serial)", "Modbus TCP/IP" };
        private string _selectedConnectionType = "Modbus RTU (Serial)";
        public string SelectedConnectionType
        {
            get => _selectedConnectionType;
            set
            {
                _selectedConnectionType = value;
                OnPropertyChanged();
                IsRtuSelected = value == "Modbus RTU (Serial)";
            }
        }

        public bool IsRtuSelected
        {
            get => _isRtuSelected;
            set { _isRtuSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsTcpSelected)); }
        }
        public bool IsTcpSelected => !IsRtuSelected;

        public List<string> ComPorts { get; }
        public List<int> BaudRates { get; } = new List<int> { 9600, 19200, 38400, 57600, 115200 };
        public List<int> DataBitsOptions { get; } = new List<int> { 7, 8 };
        public List<Parity> ParityOptions { get; }
        public List<StopBits> StopBitsOptions { get; }

        public string SelectedComPort { get => _selectedComPort; set { _selectedComPort = value; OnPropertyChanged(); } }
        public int SelectedBaudRate { get => _selectedBaudRate; set { _selectedBaudRate = value; OnPropertyChanged(); } }
        public int SelectedDataBits { get => _selectedDataBits; set { _selectedDataBits = value; OnPropertyChanged(); } }
        public Parity SelectedParity { get => _selectedParity; set { _selectedParity = value; OnPropertyChanged(); } }
        public StopBits SelectedStopBits { get => _selectedStopBits; set { _selectedStopBits = value; OnPropertyChanged(); } }

        public string IpAddress { get => _ipAddress; set { _ipAddress = value; OnPropertyChanged(); } }
        public string Port { get => _port; set { _port = value; OnPropertyChanged(); } }

        public bool IsConnected { get => _isConnected; set { _isConnected = value; OnPropertyChanged(); OnPropertyChanged(nameof(StatusText)); OnPropertyChanged(nameof(StatusIndicatorFill)); OnPropertyChanged(nameof(ConnectButtonContent)); OnPropertyChanged(nameof(IsSettingsEnabled)); } }
        public bool IsConnecting { get => _isConnecting; set { _isConnecting = value; OnPropertyChanged(); OnPropertyChanged(nameof(ConnectButtonContent)); } }
        public bool IsSettingsEnabled => !IsConnected;

        public string StatusText => IsConnected ? "Conectado" : "Desconectado";
        public Brush StatusIndicatorFill => IsConnected ? (SolidColorBrush)Application.Current.FindResource("SuccessColor") : (SolidColorBrush)Application.Current.FindResource("ErrorColor");
        public string ConnectButtonContent => IsConnecting ? "Conectando..." : (IsConnected ? "Desconectar" : "Conectar");

        public ICommand ConnectCommand { get; }

        public ConfiguracoesViewModel()
        {
            ComPorts = SerialPort.GetPortNames().ToList();
            ParityOptions = Enum.GetValues(typeof(Parity)).Cast<Parity>().ToList();
            StopBitsOptions = Enum.GetValues(typeof(StopBits)).Cast<StopBits>().ToList();

            if (ComPorts.Any())
                SelectedComPort = ComPorts[0];
            SelectedBaudRate = 9600;
            SelectedDataBits = 8;
            SelectedParity = Parity.None;
            SelectedStopBits = StopBits.One;

            ConnectCommand = new RelayCommand(async _ => await ToggleConnection());
            UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            IsConnected = ModbusService.Instance.IsConnected;
        }

        private async Task ToggleConnection()
        {
            if (IsConnected)
            {
                ModbusService.Instance.Disconnect();
                UpdateConnectionStatus();
                return;
            }

            IsConnecting = true;
            bool success = false;

            if (IsRtuSelected)
            {
                if (string.IsNullOrEmpty(SelectedComPort))
                {
                    MessageBox.Show("Por favor, selecione uma Porta COM.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsConnecting = false;
                    return;
                }
                success = await ModbusService.Instance.ConnectRtuAsync(SelectedComPort, SelectedBaudRate, SelectedDataBits, SelectedParity, SelectedStopBits);
            }
            else // TCP
            {
                if (string.IsNullOrWhiteSpace(IpAddress) || !int.TryParse(Port, out int port))
                {
                    MessageBox.Show("Endereço IP ou Porta inválidos.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsConnecting = false;
                    return;
                }
                success = await ModbusService.Instance.ConnectTcpAsync(IpAddress, port);
            }

            IsConnecting = false;
            UpdateConnectionStatus();

            if (success)
            {
                MessageBox.Show("Conectado com sucesso ao CLP!", "Conexão bem-sucedida", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Falha ao conectar com o CLP. Verifique as configurações e a conexão física.", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
