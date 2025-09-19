using NModbus;
using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading.Tasks;
using NModbus.Serial;

namespace supervisorioMMS.Services
{
    public class ModbusService
    {
        private TcpClient? _tcpClient;
        private SerialPort? _serialPort;
        private IModbusMaster? _master;

        public bool IsConnected => (_tcpClient?.Connected ?? false) || (_serialPort?.IsOpen ?? false);

        public ModbusService() { }

        public async Task<bool> ConnectTcpAsync(string ipAddress, int port)
        {
            if (IsConnected) Disconnect();
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ipAddress, port);
                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_tcpClient);
                return _tcpClient.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar Modbus TCP: {ex.Message}");
                Disconnect(); // Garante a limpeza em caso de falha
                return false;
            }
        }

        public Task<bool> ConnectRtuAsync(string comPort, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            return Task.Run(() =>
            {
                if (IsConnected) Disconnect();
                try
                {
                    _serialPort = new SerialPort(comPort)
                    {
                        BaudRate = baudRate,
                        DataBits = dataBits,
                        Parity = parity,
                        StopBits = stopBits,
                        ReadTimeout = 500,
                        WriteTimeout = 500
                    };
                    _serialPort.Open();

                    var factory = new ModbusFactory();
                    _master = factory.CreateRtuMaster(new SerialPortAdapter(_serialPort));
                    return _serialPort.IsOpen;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao conectar Modbus RTU: {ex.Message}");
                    Disconnect(); // Garante a limpeza em caso de falha
                    return false;
                }
            });
        }

        public void Disconnect()
        {
            _master?.Dispose();
            _master = null;

            _tcpClient?.Close();
            _tcpClient = null;

            _serialPort?.Close();
            _serialPort = null;
        }

        public async Task<int[]?> ReadHoldingRegistersAsync(int startingAddress, int quantity)
        {
            if (!IsConnected || _master == null) return null;
            try
            {
                ushort[] result = await _master.ReadHoldingRegistersAsync(0, (ushort)startingAddress, (ushort)quantity);
                return Array.ConvertAll(result, val => (int)val);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler Holding Registers: {ex.Message}");
                return null;
            }
        }

        public async Task<bool[]?> ReadCoilsAsync(int startingAddress, int quantity)
        {
            if (!IsConnected || _master == null) return null;
            try
            {
                return await _master.ReadCoilsAsync(0, (ushort)startingAddress, (ushort)quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler Coils: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> WriteSingleCoilAsync(int startingAddress, bool value)
        {
            if (!IsConnected || _master == null) return false;
            try
            {
                await _master.WriteSingleCoilAsync(0, (ushort)startingAddress, value);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever Single Coil: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> WriteSingleRegisterAsync(int startingAddress, int value)
        {
            if (!IsConnected || _master == null) return false;
            try
            {
                await _master.WriteSingleRegisterAsync(0, (ushort)startingAddress, (ushort)value);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever Single Register: {ex.Message}");
                return false;
            }
        }
    }
}