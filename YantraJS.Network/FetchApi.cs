#nullable enable
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    internal partial class FetchApi
    {

        public static async Task<JSValue> Fetch(JSContext window, HttpClient client, Arguments a)
        {
            var first = a[0] ?? throw new ArgumentNullException();
            if (!first.ConvertTo<Request>(out var request))
            {
                // build request ...
                request = new Request(a);
            }
            EnsureUrlIsSafe(request.Url);
            CancellationToken token;
            if (request.Signal != null)
            {
                var ct = new CancellationTokenSource();
                token = ct.Token;
                request.Signal.AbortedEvent += (s, e) => {
                    ct.Cancel();
                };
            }
            var response = await client.SendAsync(request.Build(client), token);
            return new FetchResponse(request, response);
        }

        // Prevent SSRF by blocking requests to loopback, private, link-local
        // and cloud metadata addresses (e.g. 169.254.169.254).
        private static void EnsureUrlIsSafe(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Fetch URL must be an absolute http/https URL");

            var addresses = IPAddress.TryParse(uri.Host, out var single)
                ? new[] { single }
                : Dns.GetHostAddresses(uri.Host);

            foreach (var ip in addresses)
            {
                if (IsBlockedAddress(ip))
                    throw new ArgumentException("Fetch URL targets a restricted network address");
            }
        }

        private static bool IsBlockedAddress(IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip) || ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal)
                return true;
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                var b = ip.GetAddressBytes();
                if (b[0] == 10) return true;
                if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
                if (b[0] == 192 && b[1] == 168) return true;
                if (b[0] == 169 && b[1] == 254) return true; // link-local & cloud metadata
            }
            return false;
        }

    }
}
