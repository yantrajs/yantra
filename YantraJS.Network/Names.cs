#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSRegistrationGenerator]
    public static partial class Names
    {
        internal static readonly KeyString keepalive;
        internal static readonly KeyString fetch;
        internal static readonly KeyString method;
        internal static readonly KeyString headers;
        internal static readonly KeyString body;
        internal static readonly KeyString mode;
        internal static readonly KeyString credentials;
        internal static readonly KeyString cache;
        internal static readonly KeyString redirect;
        internal static readonly KeyString referrer;
        internal static readonly KeyString referrerPolicy;
        internal static readonly KeyString integrity;
        internal static readonly KeyString signal;

        public static void InstallNetworkServices(this JSContext context, HttpClient? client = null)
        {
            client ??= new HttpClient();
            RegisterAll(context);
            context[Names.fetch] = new JSFunction((in Arguments a) => {
                return ClrProxy.Marshal(FetchApi.Fetch(context, client, a));
            });

        }

    }
}
