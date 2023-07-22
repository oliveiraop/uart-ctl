using System;
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

        public void StartReading()
        {
            Thread readThread = new Thread(SerialPortDataReceived);
            readThread.Start();
        }

        public void SerialPortDataReceived(object sender)
        {
            while (true)
            {
                string receivedData = string.Empty;

                while (_serialPort.BytesToRead > 0)
                {
                    receivedData += _serialPort.ReadExisting();
                }

                if (!string.IsNullOrEmpty(receivedData))
                {
                    Console.WriteLine(receivedData);
                    ChecarIP(receivedData);
                }
            }
        }

        public void ChecarIP(string message)
        {
            if (IsUdprtMessage(message))
            {
                string ipAddress = ExtractIpAddressFromMessage(message);
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    if (ValidateIPMessage(message))
                    {
                        string udpstMessage = $"udpst {ipAddress} 20171 \"yes!\"";
                        SendMessage(udpstMessage);
                    }
                    else
                    {
                        
                    }
                }
            }
        }

        static bool IsUdprtMessage(string message)
        {
            return message.Trim().StartsWith("udprt", StringComparison.OrdinalIgnoreCase);
        }

        static bool ValidateIPMessage(string message)
        {
            int startIndex = message.IndexOf("udprt <");
            int endIndex = message.IndexOf("> \"isb?\"");

            // Verifica se a mensagem contém "udprt <", "> \"isb?\"" e se os índices estão na ordem correta
            return startIndex != -1 && endIndex != -1 && startIndex < endIndex;
        }


        private string ExtractIpAddressFromMessage(string message)
        {
            string pattern = @"<([^>]*)>";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(message);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }
    }
}