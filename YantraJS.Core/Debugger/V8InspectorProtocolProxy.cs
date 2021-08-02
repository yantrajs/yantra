using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger
{
    public class IncomingMessage
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public JObject Params { get; set; }
    }

    internal class V8InspectorProtocolProxy : V8InspectorProtocol
    {

        readonly Uri uri;
        System.Net.WebSockets.ClientWebSocket client;
        CancellationTokenSource cancellationTokenSource;
        public V8InspectorProtocolProxy(Uri uri)
        {
            this.uri = uri;
            client = new System.Net.WebSockets.ClientWebSocket();

            cancellationTokenSource = new CancellationTokenSource();
        }

        public override async Task ConnectAsync()
        {
            var t = client.ConnectAsync(uri, cancellationTokenSource.Token);
            lastTask = t;
            await t;

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

                        var msg = JsonConvert.DeserializeObject<IncomingMessage>(sb.ToString());
                        ProcessMessage(msg);
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

        private Task lastMsg;
        private void ProcessMessage(IncomingMessage msg)
        {
            var p = lastMsg;
            lastMsg = Task.Run(async () => {
                if (p != null)
                    await p;
                await OnMessageReceived(msg);
            });
        }

        public override void Dispose()
        {
            client?.Dispose();
        }

        private Task lastTask;

        public override void SendMessage(string message)
        {
            var p = lastTask;
            lastTask = Task.Run(async () =>
            {
                if (p != null)
                    await p;
                var bytes = System.Text.Encoding.UTF8.GetBytes(message);
                var buffer = new ArraySegment<byte>(bytes);
                await client.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, cancellationTokenSource.Token);
            });
        }
    }
}
