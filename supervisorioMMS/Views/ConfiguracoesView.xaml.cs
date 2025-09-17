using supervisorioMMS.Services;
using System.Windows;
using System.Windows.Controls;

namespace supervisorioMMS.Views
{
    public partial class ConfiguracoesView : UserControl
    {
        public ConfiguracoesView()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var ipAddress = IpAddressTextBox.Text;
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