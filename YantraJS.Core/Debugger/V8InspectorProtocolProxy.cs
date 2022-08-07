using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger
{
    public class IncomingMessage
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        public System.Text.Json.Nodes.JsonNode Params { get; set; }
    }

    internal class V8InspectorProtocolProxy : V8InspectorProtocol
    {

        readonly Uri uri;
        System.Net.WebSockets.ClientWebSocket client;
        private TaskCompletionSource<int> started;

        public V8InspectorProtocolProxy(Uri uri)
        {
            this.uri = uri;
            client = new System.Net.WebSockets.ClientWebSocket();

        }

        

        protected override async Task ConnectAsync(CancellationToken token)
        {
            await client.ConnectAsync(uri, token);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => this.ReadMessagesAsync());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        private async Task ReadMessagesAsync()
        {
            var buffer = new byte[1024];
            var sb = new StringBuilder();
            try
            {
                while (true)
                {
                    
                    var r = await this.client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (r.MessageType != System.Net.WebSockets.WebSocketMessageType.Text)
                        break;
                    sb.Append(System.Text.Encoding.UTF8.GetString(buffer,0, r.Count));
                    if (r.EndOfMessage)
                    {
                        var text = sb.ToString();
                        var msg = JsonSerializer.Deserialize<IncomingMessage>(text);
                        // System.Diagnostics.Debug.WriteLine($"Received {text}");
                        Enqueue(msg);
                        sb.Length = 0;
                    }
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex);
                // disconnect...
                client.Dispose();
                client = null;
            }
        }

        public override void Dispose()
        {
            client?.Dispose();
        }

        private Task lastTask;

        protected override Task SendAsync(string message, CancellationToken token)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(bytes);
            return client.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }
    }
}
