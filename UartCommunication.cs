using System.IO.Ports;
using System.Text.RegularExpressions;

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
            ChecarIP(receivedData);
        }

        public void CommandsInit()
        {
            string[] commandSequence = new string[]
            {//WISUN TEST
                "atstart 1",
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

        public void ChecarIP(string message)
        {

            if (ValidateIPMessage(message))
            {
                string ipAddress = ExtractIpAddressFromMessage(message);
                Console.WriteLine(ipAddress);
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string udpstMessage = $"udpst {ipAddress} 20171 \"yes!\"";
                    Console.WriteLine(udpstMessage);
                    SendMessage(udpstMessage);
                }
            }
        }

        static bool ValidateIPMessage(string message)
        {
            string expectedMessage = "udprt <[^>]+> \"isb?\""; // Use um padrão regex flexível para capturar qualquer endereço IP entre < e >
            return Regex.IsMatch(message, expectedMessage);
        }

        static string ExtractIpAddressFromMessage(string message)
        {
            // Define o padrão de expressão regular para extrair o endereço IP entre os símbolos "<" e ">"
            string pattern = @"<([^>]*)>";

            // Cria uma instância da classe Regex
            Regex regex = new Regex(pattern);

            // Procura por correspondências na mensagem
            Match match = regex.Match(message);

            // Se encontrar uma correspondência, retorna o valor entre os símbolos "<" e ">"
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // Caso contrário, retorna uma string vazia indicando que o IP não foi encontrado
            return string.Empty;
        }
    }
}