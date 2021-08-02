using ChatServerApp.SocketsManager;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerApp.Handlers
{
    public class WebSocketMessageHandler : SocketHandler
    {
        public WebSocketMessageHandler(IConnectionManager connectionManager) : base (connectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var connectiontId = ConnectionManager.GetConnectionId(socket);

            await SendMessageToAll($"{connectiontId} just joined the party");
        }

        public override async Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var connectiontId = ConnectionManager.GetConnectionId(socket);

            var messageReceived = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var broadcastMessage = $"{connectiontId} said: {messageReceived}";

            await SendMessageToAll(broadcastMessage);
        }
    }
}
