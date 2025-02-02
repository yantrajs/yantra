#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Debugger;

namespace YantraJS.Network
{
    [JSClassGenerator]
    public partial class Request : JSObject
    {
        public Request(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
            var first = a[0] ?? throw new ArgumentNullException();
            if (first.IsString)
            {
                this.Url = first.ToString();
            } else {
                if(!first.ConvertTo<Request>(out var r))
                    throw new ArgumentException();
                this.Url = r.Url;
            }

            var options = a[1];
            JSValue? v;
            this.Method = options.TryGetProperty(Names.method, out var p) ? p.ToString() : "GET";
            this.Headers = new Headers(options.TryGetProperty(Names.headers, out v) ? v : null);
            if(options.TryGetProperty(Names.body, out v))
            {
                this.Body = v;
            }
            this.Mode = options.TryGetProperty(Names.mode, out v) ? v.ToString() : "cors";
            this.Credentials = options.TryGetProperty(Names.credentials, out v) ? v.ToString() : "same-origin";
            this.Cache = options.TryGetProperty(Names.cache, out v) ? v.ToString() : null;
            this.Redirect = options.TryGetProperty(Names.redirect, out v) ? v.ToString() : "follow";
            this.Referrer = options.TryGetProperty(Names.referrer, out v) ? v.ToString() : "about:client";
            if (options.TryGetProperty(Names.referrerPolicy, out v))
            {
                this.ReferrerPolicy = v.ToString();
            }
            if (options.TryGetProperty(Names.integrity, out v))
                this.Integrity = v.ToString();

            this.KeepAlive = options.TryGetProperty(Names.keepalive, out v) ? v.BooleanValue : false;
            if (options.TryGetProperty(Names.signal, out v))
            {
                if(v.ConvertTo<AbortSignal>(out var s)) {
                    this.Signal = s;
                }
            }
        }

        [JSExport]
        public string Url { get; }

        [JSExport]
        public string Method { get; }

        [JSExport]
        public Headers Headers { get; }

        [JSExport]
        public JSValue? Body { get; set; }

        [JSExport]
        public string? Mode { get; set; }

        [JSExport]
        public string? Credentials { get; set; }

        [JSExport]
        public string? Cache { get; set; }

        [JSExport]
        public string? Redirect { get; set; }

        [JSExport]
        public string? Referrer { get; set; }

        [JSExport]
        public string? ReferrerPolicy { get; set; }

        [JSExport]
        public string? Integrity { get; set; }

        [JSExport]
        public bool KeepAlive { get; set; }

        [JSExport]
        public AbortSignal? Signal { get; set; }

        internal HttpRequestMessage Build(HttpClient client)
        {
            var request = new HttpRequestMessage(new HttpMethod(this.Method), this.Url);

            foreach (var header in this.Headers.GetEnumerable())
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            SetBody(request);

            return request;
        }

        private void SetBody(HttpRequestMessage request)
        {
            if (Body == null)
                return;

            if (Body.IsString)
            {
                request.Content = new StringContent(Body.ToString());
                return;
            }

            // try each type....
            if (Body.ConvertTo<KeyValueStore>(out var fd))
            {
                request.Content = new FormUrlEncodedContent(fd.GetEnumerable());
                return;
            }
        }
    }
}
