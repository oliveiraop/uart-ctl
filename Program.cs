using System;

namespace UartCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Você precisa fornecer o nome do dispositivo como argumento de linha de comando.");
                return;
            }


            string deviceName = args[0]; // @TODO /dev/ttymxc0


            // ######################################     INTEGRAVEL   ########################################################
            UartCommunication communication = new UartCommunication(deviceName, 115200);
            communication.Open(); // Abertura do canal serial, pode ser colocado no construtor se achar necessário
            communication.StartReading();
            // Thread readThread = new Thread(communication.SerialPortDataReceived); // @TODO ese aqui é o start da thread que está em communication.StartReading();
            communication.Config();
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
                communication.SendMessage(message);
            }

            communication.Close();
        }
    }
}
