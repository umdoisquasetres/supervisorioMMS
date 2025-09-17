using EasyModbus;
using System;

namespace supervisorioMMS.Services
{
    public class ModbusService
    {
        private static readonly Lazy<ModbusService> _instance = new Lazy<ModbusService>(() => new ModbusService());
        public static ModbusService Instance => _instance.Value;

        private ModbusClient _modbusClient;

        public bool IsConnected => _modbusClient?.Connected ?? false;

        private ModbusService()
        {
            _modbusClient = new ModbusClient();
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                if (IsConnected)
                {
                    _modbusClient.Disconnect();
                }
                _modbusClient.IPAddress = ipAddress;
                _modbusClient.Port = port;
                _modbusClient.Connect();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Erro ao conectar Modbus: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _modbusClient.Disconnect();
            }
        }

                public int[] ReadHoldingRegisters(int startingAddress, int quantity)
        {
            if (!IsConnected) return null;
            try
            {
                return _modbusClient.ReadHoldingRegisters(startingAddress, quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler Holding Registers: {ex.Message}");
                return null;
            }
        }

        public bool[] ReadCoils(int startingAddress, int quantity)
        {
            if (!IsConnected) return null;
            try
            {
                return _modbusClient.ReadCoils(startingAddress, quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler Coils: {ex.Message}");
                return null;
            }
        }

                public bool WriteSingleCoil(int startingAddress, bool value)
        {
            if (!IsConnected) return false;
            try
            {
                _modbusClient.WriteSingleCoil(startingAddress, value);
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
            if (!IsConnected) return false;
            try
            {
                _modbusClient.WriteSingleRegister(startingAddress, value);
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
