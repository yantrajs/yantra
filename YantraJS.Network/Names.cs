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
