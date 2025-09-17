using NModbus;
using System;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using NModbus.Serial; // Adicionado para SerialPortAdapter

namespace supervisorioMMS.Services
{
    public class ModbusService
    {
        private static readonly Lazy<ModbusService> _instance = new Lazy<ModbusService>(() => new ModbusService());
        public static ModbusService Instance => _instance.Value;

        private TcpClient? _tcpClient; // Para conexão TCP
        private SerialPort? _serialPort; // Para conexão Serial
        private IModbusMaster? _master;

        public bool IsConnected => (_tcpClient?.Connected ?? false) || (_serialPort?.IsOpen ?? false);

        private ModbusService() { }

        // Conexão TCP/IP (mantida para compatibilidade, se necessário)
        public bool Connect(string ipAddress, int port)
        {
            try
            {
                Disconnect();
                _tcpClient = new TcpClient(ipAddress, port);
                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_tcpClient);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar Modbus TCP: {ex.Message}");
                return false;
            }
        }

        // Nova Conexão Serial (Modbus RTU)
        public bool Connect(string comPort, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            try
            {
                Disconnect();
                _serialPort = new SerialPort(comPort);
                _serialPort.BaudRate = baudRate;
                _serialPort.DataBits = dataBits;
                _serialPort.Parity = parity;
                _serialPort.StopBits = stopBits;
                _serialPort.Open();

                var factory = new ModbusFactory();
                // Envolve SerialPort em um SerialPortAdapter para NModbus
                _master = factory.CreateRtuMaster(new SerialPortAdapter(_serialPort));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar Modbus RTU: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            _tcpClient?.Close();
            _serialPort?.Close();
        }

        public int[]? ReadHoldingRegisters(int startingAddress, int quantity)
        {
            if (!IsConnected || _master == null) return null;
            try
            {
                ushort[] result = _master.ReadHoldingRegisters(0, (ushort)startingAddress, (ushort)quantity);
                return Array.ConvertAll(result, val => (int)val);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler Holding Registers: {ex.Message}");
                return null;
            }
        }

        public bool[]? ReadCoils(int startingAddress, int quantity)
        {
            if (!IsConnected || _master == null) return null;
            try
            {
                return _master.ReadCoils(0, (ushort)startingAddress, (ushort)quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler Coils: {ex.Message}");
                return null;
            }
        }

        public bool WriteSingleCoil(int startingAddress, bool value)
        {
            if (!IsConnected || _master == null) return false;
            try
            {
                _master.WriteSingleCoil(0, (ushort)startingAddress, value);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao escrever Single Coil: {ex.Message}");
                return false;
            }
        }

        public bool WriteSingleRegister(int startingAddress, int value)
        {
            if (!IsConnected || _master == null) return false;
            try
            {
                _master.WriteSingleRegister(0, (ushort)startingAddress, (ushort)value);
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