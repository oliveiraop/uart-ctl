using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Device.Gpio.Drivers;

namespace UartCommunication
{
    public class LoraUart
    {
        private SerialPort _serialPort;

        
        public LoraUart(string portName, int baudRate, Action<string, string> onMessageReceived)
        {
            _serialPort = new SerialPort(portName, baudRate);
            OnMessageReceived = onMessageReceived;
            // @TODO Setar GPIO aqui (gpipchip0 line 6 = 1)
            
        }

        public void TurnWisunOn() 
        {
            System.Device.Gpio.GpioDriver driver = System.Device.Gpio.Drivers.UnixDriver.Create();
            System.Device.Gpio.GpioController controller = new System.Device.Gpio.GpioController(System.Device.Gpio.PinNumberingScheme.Logical, driver);
            controller.OpenPin(6, System.Device.Gpio.PinMode.Output, System.Device.Gpio.PinValue.High);
        }

        public Action<string, string> OnMessageReceived { get; set; }


        public void Config() // Comando para inicializar as configurações do módulo
        {
            Thread.Sleep(1000);
            SendMessage("atstart 1");
            SendMessage("save");
            SendMessage("reset");
            Thread.Sleep(1000);
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

        public void SendCommand(string command, string ipAddress) // @TODO Usar esse comando para enviar os dados, precisa formatar a mensagem
        {
            SendMessage($"udpst {ipAddress} 20171 \"{command}\"");
        }

        public void SendMessage(string message) // send message de fato, está funcionando
        {
            if (_serialPort.IsOpen)
            {
                Console.WriteLine($"Lora Message to send: {message}");
                _serialPort.WriteLine(message);
            }
        }

        public void StartReading()
        {
            Thread readThread = new Thread(SerialPortDataReceived);
            readThread.Start();
        }

        public void SerialPortDataReceived(object sender) // Código de recebimento, ele já valida algumas perguntas e responde automaticamente
        {
            while (true)
            {
                string receivedData = string.Empty;

                while (_serialPort.BytesToRead > 0)
                {
                    receivedData += _serialPort.ReadExisting(); // Leitura do canal serial, received data funciona
                }

                if (!string.IsNullOrEmpty(receivedData))
                {
                    Console.WriteLine(receivedData);
                        OnMessageReceived("lora", receivedData);
                }
                Thread.Sleep(1);
            }
        }




        // 
        /*private void Receive() // @TODO: precisa ser validado, foi feita por jorge com base no UDP Socket
    {
        String msgReceived = "";
        string ipAddress;
        int domain = 0;
        int unit = 0;
        int area = 0;
        int command = 0;
        int payload = 0;

        while (_serial.BytesToRead > 0)
        {
            msgReceived += _serial.ReadExisting(); //Recebe mensagem
        }

        if (!string.IsNullOrEmpty(msgReceived))
        {
            Console.WriteLine(msgReceived);
            if (IsUdprtMessageFormatValid(msgReceived)) //checa se é udp  exemplo: udprt <ipv6> "4 16 2 111 107"
            {
                ipAddress = ExtractIpAddressFromMessage(msgReceived); // extrai ip de quem enviou o udp da mensagem

                string pattern = @"udprt <[^>]*> ""(\d+) (\d+) (\d+) (\d+) (\d+)"""; //extrai os dados do udp
                Regex regex = new Regex(pattern);
                Match match = regex.Match(msgReceived);

                if (match.Success)
                {
                    domain = int.Parse(match.Groups[1].Value);
                    unit = int.Parse(match.Groups[2].Value);
                    area = int.Parse(match.Groups[3].Value);
                    command = int.Parse(match.Groups[4].Value);
                    payload = int.Parse(match.Groups[5].Value);
                }

                int clientDomain = (domain & DOMAIN_MASK) >> 5; //Verificar isto!!!
                int clientUnity = (unit & UNITY_MASK);
                int clientArea = (area & AREA_MASK) >> 2;
                int command_field = (command & COMMAND_MASK);

                Console.WriteLine($"Extracted data: {ipAddress}, Domain={clientDomain}, Unit={clientUnity}, Area={clientArea}, Command={command_field}, Payload={payload}");
            }
            else if(ValidateIPMessage(msgReceived))
            {
                    ipAddress = ExtractIpAddressFromMessage(msgReceived); // extrai ip de quem enviou o udp da mensagem
                    string udpstMessage = $"udpst {ipAddress} 20171 \"yes!\"";
                    //SendMessage(udpstMessage);  Função de envio da udp para que o outro wisun pegue o ip do border, n sabia se era pra deixar aqui tb 
            }
        }

        }*/


        private bool ChecarMensagem(string message) // @TODO estou pensando em retornar um booleano pra facilitar
        {
            if (IsUdprtMessage(message))
            {
                string ipAddress = ExtractIpAddressFromMessage(message);
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    if (ValidateIPMessage(message))
                    {
                        Console.WriteLine($"{ipAddress} validated and integrated to the list");
                        string udpstMessage = $"udpst {ipAddress} 20171 \"yes!\""; // @TODO colocar callback para colocar na lista de dispositivos
                        SendMessage(udpstMessage);
                        return false;
                    }
                }
                return true; // Se a mensagem não for de cadastro
            }
            return false; // @TODO Verificar se é necessário rotear outras mensagens sem ser udprt
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

    

            private string ExtractDataFromMessage(string message) // @TODO utilizar essa função pra extrair a mensagem recebida "4 16 17 3 111 107"
        {
            string pattern = @"udprt <[^>]*> (.*)";
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