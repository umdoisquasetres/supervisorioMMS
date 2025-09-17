using supervisorioMMS.Services;
using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class ConfiguracoesView : UserControl
    {
        public ConfiguracoesView()
        {
            InitializeComponent();
            PopulateConnectionSettings();
            // Define o tipo de conexão padrão como RTU
            ConnectionTypeComboBox.SelectedIndex = 0;
        }

        private void PopulateConnectionSettings()
        {
            // Populate COM Port ComboBox
            ComPortComboBox.ItemsSource = SerialPort.GetPortNames();
            if (ComPortComboBox.Items.Count > 0)
            {
                ComPortComboBox.SelectedIndex = 0;
            }

            // Populate Baud Rate ComboBox
            BaudRateComboBox.ItemsSource = new int[] { 9600, 19200, 38400, 57600, 115200 };
            BaudRateComboBox.SelectedValue = 9600;

            // Populate Data Bits ComboBox
            DataBitsComboBox.ItemsSource = new int[] { 7, 8 };
            DataBitsComboBox.SelectedValue = 8;

            // Populate Parity ComboBox
            ParityComboBox.ItemsSource = Enum.GetNames(typeof(Parity));
            ParityComboBox.SelectedValue = Parity.None.ToString();

            // Populate Stop Bits ComboBox
            StopBitsComboBox.ItemsSource = Enum.GetNames(typeof(StopBits));
            StopBitsComboBox.SelectedValue = StopBits.One.ToString();
        }

        private void ConnectionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConnectionTypeComboBox.SelectedValue is ComboBoxItem selectedItem)
            {
                string? connectionType = selectedItem.Content?.ToString();

                if (connectionType == "Modbus RTU (Serial)")
                {
                    RtuSettingsGroupBox.Visibility = Visibility.Visible;
                    TcpSettingsGroupBox.Visibility = Visibility.Collapsed;
                }
                else if (connectionType == "Modbus TCP/IP")
                {
                    RtuSettingsGroupBox.Visibility = Visibility.Collapsed;
                    TcpSettingsGroupBox.Visibility = Visibility.Visible;
                }
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionTypeComboBox.SelectedValue is ComboBoxItem selectedItem)
            {
                string? connectionType = selectedItem.Content?.ToString();

                if (connectionType == "Modbus RTU (Serial)")
                {
                    string? comPort = ComPortComboBox.SelectedValue as string;
                    if (string.IsNullOrEmpty(comPort))
                    {
                        MessageBox.Show("Por favor, selecione uma Porta COM.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!int.TryParse(BaudRateComboBox.SelectedValue?.ToString(), out int baudRate))
                    {
                        MessageBox.Show("Baud Rate inválido.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!int.TryParse(DataBitsComboBox.SelectedValue?.ToString(), out int dataBits))
                    {
                        MessageBox.Show("Data Bits inválido.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!Enum.TryParse(ParityComboBox.SelectedValue?.ToString(), out Parity parity))
                    {
                        MessageBox.Show("Parity inválido.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!Enum.TryParse(StopBitsComboBox.SelectedValue?.ToString(), out StopBits stopBits))
                    {
                        MessageBox.Show("Stop Bits inválido.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    bool isConnected = ModbusService.Instance.Connect(comPort, baudRate, dataBits, parity, stopBits);

                    if (isConnected)
                    {
                        MessageBox.Show("Conectado com sucesso ao CLP!", "Conexão bem-sucedida", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Falha ao conectar com o CLP. Verifique as configurações da porta serial.", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (connectionType == "Modbus TCP/IP")
                {
                    string? ipAddress = IpAddressTextBox.Text;
                    if (!int.TryParse(PortTextBox.Text, out int port))
                    {
                        MessageBox.Show("A porta deve ser um número válido.", "Erro de Formato", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    bool isConnected = ModbusService.Instance.Connect(ipAddress, port);

                    if (isConnected)
                    {
                        MessageBox.Show("Conectado com sucesso ao CLP!", "Conexão bem-sucedida", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Falha ao conectar com o CLP. Verifique o endereço IP, a porta e a conexão de rede.", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}