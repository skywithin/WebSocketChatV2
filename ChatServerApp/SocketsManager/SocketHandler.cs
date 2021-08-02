using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServerApp.SocketsManager
{
    public abstract class SocketHandler
    {
        public IConnectionManager ConnectionManager { get; }

        public SocketHandler(IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            await Task.Run(() => { ConnectionManager.AddSocket(socket); });
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var connectionId = ConnectionManager.GetConnectionId(socket);
            await ConnectionManager.RemoveSocket(connectionId);
        }

        public async Task SendMessage(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length);
            var endOfMessage = true;

            await socket.SendAsync(
                buffer, 
                WebSocketMessageType.Text, 
                endOfMessage, 
                CancellationToken.None);
        }

        public async Task SendMessage(Guid connectionId, string message)
        {
            var socket = ConnectionManager.GetSocketById(connectionId);

            await SendMessage(socket, message);
        }

        public async Task SendMessageToAll(string message)
        {
            var connections = ConnectionManager.GetAllConnections();

            foreach (var connection in connections)
            {
                await SendMessage(connection.Value, message);
            }
        }

        public abstract Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}
