using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServerApp.SocketsManager
{
    public interface IConnectionManager
    {
        WebSocket GetSocketById(Guid connectionId);
        ConcurrentDictionary<Guid, WebSocket> GetAllConnections();
        Guid GetConnectionId(WebSocket socket);
        Task RemoveSocket(Guid connectionId);
        void AddSocket(WebSocket socket);
    }

    public class ConnectionManager : IConnectionManager
    {
        private ConcurrentDictionary<Guid, WebSocket> _connections = new ConcurrentDictionary<Guid, WebSocket>();

        public WebSocket GetSocketById (Guid connectionId)
        {
            //return _connections.FirstOrDefault(x => x.Key == id).Value;

            return _connections[connectionId];
        }

        public ConcurrentDictionary<Guid, WebSocket> GetAllConnections()
        {
            return _connections;
        }

        public Guid GetConnectionId (WebSocket socket)
        {
            return _connections.FirstOrDefault(x => x.Value == socket).Key;
        }

        public async Task RemoveSocket(Guid connectionId)
        {
            _connections.TryRemove(connectionId, out var socket);
            await socket.CloseAsync(
                closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Socket connection closed",
                cancellationToken: CancellationToken.None);
        }

        public void AddSocket(WebSocket socket)
        {
            var connectionId = Guid.NewGuid();

            _connections.TryAdd(connectionId, socket);
        }
    }
}
