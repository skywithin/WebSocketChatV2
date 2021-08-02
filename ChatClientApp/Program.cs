using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketsConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StartChat().GetAwaiter().GetResult();
        }

        public static async Task StartChat()
        {
            var client = new ClientWebSocket();
            var serverAddres = "ws://localhost:5000/Chat";

            Console.WriteLine($"Connecting to {serverAddres}...");

            await client.ConnectAsync(new Uri(serverAddres), CancellationToken.None);

            Console.WriteLine($"Web socket connection established at {DateTime.Now}");

            var send = Task.Run(async () =>
            {
                string message;

                while (!string.IsNullOrEmpty(message = Console.ReadLine()))
                {
                    var bytes = Encoding.UTF8.GetBytes(message);
                    var endOfMessage = true;

                    await client.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        endOfMessage,
                        CancellationToken.None);
                }

                await client.CloseOutputAsync(
                    WebSocketCloseStatus.NormalClosure,
                    statusDescription: "",
                    CancellationToken.None);
            });

            var receive = Receive(client);

            await Task.WhenAll(send, receive);
        }

        public static async Task Receive(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                var incomingMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Console.WriteLine(incomingMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(
                        WebSocketCloseStatus.NormalClosure,
                        statusDescription: "",
                        CancellationToken.None);

                    break;
                }
            }
        }
    }
}

