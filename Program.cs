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

            string deviceName = args[0];

            UartCommunication communication = new UartCommunication(deviceName, 115200);
            communication.Open();
            communication.StartReading();

            Console.WriteLine("Digite 'exit' para encerrar o programa.");

            while (true)
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
