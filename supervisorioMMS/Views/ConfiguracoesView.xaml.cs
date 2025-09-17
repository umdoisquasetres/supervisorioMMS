using supervisorioMMS.Services;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace supervisorioMMS.Views
{
    public partial class ConfiguracoesView : UserControl
    {
        public ConfiguracoesView()
        {
            InitializeComponent();
            Loaded += ConfiguracoesView_Loaded;
        }

        private void ConfiguracoesView_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateConnectionSettings();
            ConnectionTypeComboBox.SelectedIndex = 0;
            UpdateConnectionStatus();
        }

        private void PopulateConnectionSettings()
        {
            // Preenche a ComboBox de Portas COM
            ComPortComboBox.ItemsSource = SerialPort.GetPortNames();
            if (ComPortComboBox.Items.Count > 0)
            {
                ComPortComboBox.SelectedIndex = 0;
            }

            // Preenche as outras ComboBoxes com valores padrão
            BaudRateComboBox.ItemsSource = new int[] { 9600, 19200, 38400, 57600, 115200 };
            BaudRateComboBox.SelectedValue = 9600;

            DataBitsComboBox.ItemsSource = new int[] { 7, 8 };
            DataBitsComboBox.SelectedValue = 8;

            ParityComboBox.ItemsSource = Enum.GetNames(typeof(Parity));
            ParityComboBox.SelectedValue = Parity.None.ToString();

            StopBitsComboBox.ItemsSource = Enum.GetNames(typeof(StopBits));
            StopBitsComboBox.SelectedValue = StopBits.One.ToString();
        }

        private void UpdateConnectionStatus()
        {
            bool isConnected = ModbusService.Instance.IsConnected;

            if (isConnected)
            {
                ConnectButton.Content = "Desconectar";
                ConnectButton.Background = (SolidColorBrush)FindResource("ErrorColor");
                StatusIndicator.Fill = (SolidColorBrush)FindResource("SuccessColor");
                StatusText.Text = "Conectado";
            }
            else
            {
                ConnectButton.Content = "Conectar";
                ConnectButton.Background = (SolidColorBrush)FindResource("AccentColor");
                StatusIndicator.Fill = (SolidColorBrush)FindResource("ErrorColor");
                StatusText.Text = "Desconectado";
            }
            
            // Desabilita os painéis de configuração quando conectado
            RtuSettingsGroupBox.IsEnabled = !isConnected;
            TcpSettingsGroupBox.IsEnabled = !isConnected;
            ConnectionTypeComboBox.IsEnabled = !isConnected;
        }

        private void ConnectionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RtuSettingsGroupBox == null || TcpSettingsGroupBox == null) return;

            if (ConnectionTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string? connectionType = selectedItem.Content?.ToString();
                RtuSettingsGroupBox.Visibility = connectionType == "Modbus RTU (Serial)" ? Visibility.Visible : Visibility.Collapsed;
                TcpSettingsGroupBox.Visibility = connectionType == "Modbus TCP/IP" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Se já estiver conectado, desconecta
            if (ModbusService.Instance.IsConnected)
            {
                ModbusService.Instance.Disconnect();
                UpdateConnectionStatus();
                return;
            }

            ConnectButton.IsEnabled = false;
            ConnectButton.Content = "Conectando...";
            bool isConnected = false;

            if (ConnectionTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string? connectionType = selectedItem.Content?.ToString();

                if (connectionType == "Modbus RTU (Serial)")
                {
                    // Validação dos campos RTU
                    string? comPort = ComPortComboBox.SelectedValue as string;
                    if (string.IsNullOrEmpty(comPort))
                    {
                        MessageBox.Show("Por favor, selecione uma Porta COM.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!int.TryParse(BaudRateComboBox.SelectedValue?.ToString(), out int baudRate) ||
                        !int.TryParse(DataBitsComboBox.SelectedValue?.ToString(), out int dataBits) ||
                        !Enum.TryParse(ParityComboBox.SelectedValue?.ToString(), out Parity parity) ||
                        !Enum.TryParse(StopBitsComboBox.SelectedValue?.ToString(), out StopBits stopBits))
                    {
                        MessageBox.Show("Configurações seriais inválidas.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    isConnected = await ModbusService.Instance.ConnectRtuAsync(comPort, baudRate, dataBits, parity, stopBits);
                }
                else if (connectionType == "Modbus TCP/IP")
                {
                    // Validação dos campos TCP
                    string ipAddress = IpAddressTextBox.Text;
                    if (string.IsNullOrWhiteSpace(ipAddress) || !int.TryParse(PortTextBox.Text, out int port))
                    {
                        MessageBox.Show("Endereço IP ou Porta inválidos.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    isConnected = await ModbusService.Instance.ConnectTcpAsync(ipAddress, port);
                }
            }

            ConnectButton.IsEnabled = true;
            UpdateConnectionStatus();

            if (isConnected)
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
