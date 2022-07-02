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
        AsyncQueue<IncomingMessage> messages = new AsyncQueue<IncomingMessage>();
        CancellationTokenSource cancellationTokenSource;
        private TaskCompletionSource<int> started;

        public V8InspectorProtocolProxy(Uri uri)
        {
            this.uri = uri;
            client = new System.Net.WebSockets.ClientWebSocket();

            cancellationTokenSource = new CancellationTokenSource();

            this.started = new TaskCompletionSource<int>();
            lastTask = this.started.Task;
        }

        

        public override async Task ConnectAsync()
        {
            await client.ConnectAsync(uri, cancellationTokenSource.Token);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => this.ReadMessagesAsync());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            started.TrySetResult(1);

            await foreach (var item in messages.Process())
            {
                await OnMessageReceived(item);
            }
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
                        messages.Enqueue(msg);
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

        public override void SendMessage(string message)
        {
            lock (this)
            {
                var p = lastTask;
                lastTask = Task.Run(async () =>
                {
                    if (p != null)
                        await p;
                    // System.Diagnostics.Debug.WriteLine($"Sent {message}");
                    var bytes = System.Text.Encoding.UTF8.GetBytes(message);
                    var buffer = new ArraySegment<byte>(bytes);
                    await client.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, cancellationTokenSource.Token);
                });
            }
        }
    }
}
