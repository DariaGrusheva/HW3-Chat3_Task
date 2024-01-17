using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chat3_Task
{
    internal class Server
    {
        private static bool exitRequested = false;
        static private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        static private CancellationToken ct = cancellationTokenSource.Token;

        public static async Task AcceptMsg()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(5051);
            Console.WriteLine("Сервер ожидает сообщения. Для завершения нажмите клавишу...");

            // Запустим задачу для ожидания нажатия клавиши
            Task exitTask = Task.Run(() =>
            {
                Console.ReadKey();
                exitRequested = true;
            });

            while (!exitRequested)
            {
               
                var data = udpClient.Receive(ref ep);
                //byte[] buffer = data.Buffer;
                string data1 = Encoding.UTF8.GetString(data);

                Task taskClient = new Task(async () =>
                {
                    Message msg = Message.FromJson(data1);
                    Console.WriteLine(msg.ToString());
                    Message responseMsg = new Message("Server", "Message accept on serv!");
                    if (msg.Text?.ToLower()?.Trim() == "exit")
                    {
                        cancellationTokenSource.Cancel();
                        responseMsg = new Message("Server", "Exit");
                    }
                   
                    string responseMsgJs = responseMsg.ToJson();
                    byte[] responseData = Encoding.UTF8.GetBytes(responseMsgJs);
                    await udpClient.SendAsync(responseData, responseData.Length, ep);
                }, ct);

                if (!taskClient.IsCanceled)
                { 
                    taskClient.Start(); 
                }
                else 
                { 
                    Environment.Exit(0); 
                }
            }

            // Дождитесь завершения задачи по нажатию клавиши
            exitTask.Wait();
        }
    }
}
