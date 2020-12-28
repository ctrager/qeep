// From https://dotnetplaybook.com/which-is-best-websockets-or-signalr/

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace qeep
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ConnectionManager _manager;

        private byte[] _buffer = new byte[1024 * 4];

        public WebSocketServerMiddleware(RequestDelegate next, ConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await _manager.OnAcceptSocketAsync(webSocket);

                // the "async (result, buffer) =>" is the "handleMessage" in Receive

                await Receive(webSocket, async (result, buffer) =>
                {
                    // if (result.MessageType == WebSocketMessageType.Text)
                    // {
                    //     Console.WriteLine($"Receive->Text");
                    //     Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                    //     return;
                    // }
                    // else 

                    qp_util.log("WebSocket Middleware Recieve " + Enum.GetName(typeof(WebSocketMessageType), result.MessageType));

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _manager.OnCloseSocketAsync(webSocket, result);
                    }
                });
            }
            else
            {
                // not a websocket request
                await _next(context);
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(
                        buffer: new ArraySegment<byte>(_buffer),
                        cancellationToken: CancellationToken.None);

                    handleMessage(result, _buffer);
                }
                catch (Exception e)
                {
                    // if the client crashes without sending us the close
                    qp_util.log(e.ToString() + ", " + e.Message);
                    await _manager.OnCloseSocketAsync(socket, null);
                }
            }
        }
    }
}