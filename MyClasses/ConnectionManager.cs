// From https://dotnetplaybook.com/which-is-best-websockets-or-signalr/

using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Linq;

namespace qeep
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task OnAcceptSocketAsync(WebSocket socket)
        {
            string connection_id = Guid.NewGuid().ToString();
            qp_util.log("OnAcceptSocketAsync " + connection_id);

            _sockets.TryAdd(connection_id, socket);

            await SendConnectionIdAsync(socket, connection_id); //Call to new method here

        }

        public async Task OnCloseSocketAsync(WebSocket socket, WebSocketReceiveResult result)
        {
            string connection_id = _sockets.FirstOrDefault(s => s.Value == socket).Key;

            qp_util.log("OnCloseSocketAsync " + connection_id);

            _sockets.TryRemove(connection_id, out WebSocket sock);

            qp_util.log("# of sockets " + _sockets.Count.ToString());

            await socket.CloseAsync(
                result.CloseStatus.Value,
                result.CloseStatusDescription,
                CancellationToken.None);

        }

        async Task SendConnectionIdAsync(WebSocket socket, string connection_id)
        {
            var buffer = Encoding.UTF8.GetBytes(
                    @"{""message_type"": ""id"", ""socket_connection_id"": """ + connection_id + @"""}");
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        public async Task NotifyOthersAsync(string connection_id, string timestamp)
        {
            qp_util.log("NotifyOthers " + connection_id);

            foreach (string key in _sockets.Keys)
            {
                if (key == connection_id)
                {
                    continue; // because this client already knows
                }
                else
                {
                    var socket = _sockets[key];
                    var buffer = Encoding.UTF8.GetBytes(
                        @"{""message_type"": ""update"", ""timestamp"": """ + timestamp + @"""}");
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }

            }
        }
    }
}