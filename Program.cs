using System;

namespace UartCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            /*if (args.Length < 1)
            {
                Console.WriteLine("Você precisa fornecer o nome do dispositivo como argumento de linha de comando.");
                return;
            }*/

            MQTT mqttClientSide = new MQTT();
            mqttClientSide.MQTTConnect();

            Thread mqttClientSideTask = new Thread(new ThreadStart(mqttClientSide!.MQTT_Task));

            


            //string deviceName = args[0]; // @TODO /dev/ttymxc0


            // ######################################     INTEGRAVEL   ########################################################
            UartCommunication wisun = new UartCommunication("/dev/ttymxc0", 115200, mqttClientSide.SendMessage);
            LoraUart lora = new LoraUart("/dev/ttyUSB1", 115200, mqttClientSide.SendMessage);
            mqttClientSide.sendWisunMessage = wisun.SendMessage;
            mqttClientSide.sendLoraMessage = lora.SendMessage;

            System.Console.WriteLine("Running MQTT Client side TASK...");
            mqttClientSideTask.Start();

            wisun.TurnWisunOn();
            wisun.Open(); // Abertura do canal serial, pode ser colocado no construtor se achar necessário
            wisun.StartReading();

            lora.Open();
            lora.StartReading();
            // Thread readThread = new Thread(communication.SerialPortDataReceived); // @TODO ese aqui é o start da thread que está em communication.StartReading();
            wisun.Config();
            // ################################################################################################################
            

            Console.WriteLine("Digite 'exit' para encerrar o programa.");

            while (true) // @TODO esse while pode ser removido, o send message deve ser integrado
            {
                // Recebe input
                string message = Console.ReadLine();

                // Fecha o programa se o comando for "exit"
                if (message.ToLower() == "exit")
                {
                    break;
                }

                // Envia a mensagem
                wisun.SendMessage(message);
            }

            wisun.Close();
        }
    }
}
