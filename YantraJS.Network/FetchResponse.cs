#nullable enable
using System.Net.Http;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Debugger;

namespace YantraJS.Network
{
    [JSClassGenerator]
    public partial class FetchResponse : JSObject
    {
        private readonly HttpResponseMessage message;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private FetchResponse(in Arguments a): base(JSContext.NewTargetPrototype)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        internal FetchResponse(Request request, HttpResponseMessage message) : this()
        {
            Ok = message.IsSuccessStatusCode;
            Status = (int)message.StatusCode;
            Type = "basic";
            Url = request.Url;
            this.message = message;
            var h = new Headers(Arguments.Empty);
            foreach(var kvp in message.Headers)
            {
                h.Append(kvp.Key, string.Join(",", kvp.Value));
            }
            this.Headers = h;
        }

        [JSExport]
        public bool Ok { get; }

        [JSExport]
        public int Status { get; }

        [JSExport]
        public bool Redirected { get; }
        [JSExport]
        public string Type { get; }
            
        [JSExport]
        public string Url { get; }

        [JSExport]
        public Headers Headers { get; }

        [JSExport]
        public JSValue Json()
        {
            return new JSPromise(JsonAsync());
        }

        [JSExport]
        public JSValue Text()
        {
            return new JSPromise(TextAsync());
        }

        private async Task<JSValue> JsonAsync() {
            var text = await message.Content.ReadAsStringAsync();
            return JSJSON.Parse(new Arguments(JSUndefined.Value, new JSString(text)));
        }

        private async Task<JSValue> TextAsync()
        {
            var text = await message.Content.ReadAsStringAsync();
            return new JSString(text);
        }
    }

}
