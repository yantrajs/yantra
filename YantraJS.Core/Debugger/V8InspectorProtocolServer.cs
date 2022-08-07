#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger
{
    public class V8InspectorProtocolServer : V8InspectorProtocol
    {
        private readonly int port;
        private WebSocket? webSocket;

        public V8InspectorProtocolServer(int port = 9222)
        {
            this.port = port;
        }

        protected override Task ConnectAsync(CancellationToken token)
        {
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            Task.Run(() =>
            {

                while (!token.IsCancellationRequested)
                {
                    var request = listener.GetContext();
                    if (request.Request.IsWebSocketRequest)
                    {
                        Task.Run(() => ProcessWebSocketRequestAsync(request));
                    }
                    else
                    {
                        Task.Run(() => ProcessRequestAsync(request), token);
                    }
                }
            });
            return Task.CompletedTask;
        }

        private async Task ProcessWebSocketRequestAsync(HttpListenerContext c)
        {
            var context = await c.AcceptWebSocketAsync(null);
            this.webSocket = context.WebSocket;

            try
            {
                //### Receiving
                // Define a receive buffer to hold data received on the WebSocket connection. The buffer will be reused as we only need to hold on to the data
                // long enough to send it back to the sender.
                byte[] receiveBuffer = new byte[1024];
                StringBuilder? sb = null;

                // While the WebSocket connection remains open run a simple loop that receives data and sends it back.
                while (webSocket.State == WebSocketState.Open)
                {

                    // The first step is to begin a receive operation on the WebSocket. `ReceiveAsync` takes two parameters:
                    //
                    // * An `ArraySegment` to write the received data to. 
                    // * A cancellation token. In this example we are not using any timeouts so we use `CancellationToken.None`.
                    //
                    // `ReceiveAsync` returns a `Task<WebSocketReceiveResult>`. The `WebSocketReceiveResult` provides information on the receive operation that was just 
                    // completed, such as:                
                    //
                    // * `WebSocketReceiveResult.MessageType` - What type of data was received and written to the provided buffer. Was it binary, utf8, or a close message?                
                    // * `WebSocketReceiveResult.Count` - How many bytes were read?                
                    // * `WebSocketReceiveResult.EndOfMessage` - Have we finished reading the data for this message or is there more coming?
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                    switch (receiveResult.MessageType) {
                        case WebSocketMessageType.Text:

                            if (receiveResult.EndOfMessage)
                            {
                                if (sb != null)
                                {
                                    var message = sb.ToString();
                                    var msg = System.Text.Json.JsonSerializer.Deserialize<IncomingMessage>(message);
                                    Enqueue(msg);
                                }
                                continue;
                            }
                            sb ??= new StringBuilder();
                            sb.Append(System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count));
                            break;
                        case WebSocketMessageType.Binary:
                            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary frame", CancellationToken.None);
                            break;
                        case WebSocketMessageType.Close:
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            break;
                    }

                }
            }
            catch (Exception e)
            {
                // Just log any exceptions to the console. Pretty much any exception that occurs when calling `SendAsync`/`ReceiveAsync`/`CloseAsync` is unrecoverable in that it will abort the connection and leave the `WebSocket` instance in an unusable state.
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                // Clean up by disposing the WebSocket once it is closed/aborted.
                if (webSocket != null)
                    webSocket.Dispose();
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        protected override Task SendAsync(string message, CancellationToken cancellationToken)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(bytes);
            return webSocket!.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, cancellationTokenSource.Token);

        }
    }
}
