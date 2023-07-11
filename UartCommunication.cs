using System.IO.Ports;

namespace UartCommunication
{
    public class UartCommunication
    {
        private SerialPort _serialPort;

        public UartCommunication(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.DataReceived += SerialPortDataReceived;
        }

        public void Open()
        {
            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening serial port: " + ex.Message);
            }
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void SendMessage(string message)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine(message);
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            string receivedData = string.Empty;
            
            while (serialPort.BytesToRead > 0)
            {
                receivedData += serialPort.ReadExisting();
            }
            
            // Process received data here
            Console.WriteLine(receivedData);
        }

        public void CommandsInit()
        {
            string[] commandSequence = new string[]
            {//WISUN TEST
                "atstart 1",
                "netname WISUN",
                "chan 33 59",
                "save",
                "reset"
            };

            foreach (string command in commandSequence)
            {
                SendMessage(command);

                // Aguardar o retorno do chip ou algum tempo de espera aqui, se necessário
                // Por exemplo, você pode usar Thread.Sleep para aguardar alguns milissegundos

                string receivedData = _serialPort.ReadExisting();
                Console.WriteLine(receivedData);
            }
        }
    }
}