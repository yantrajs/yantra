#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    public static class JSContextNetworkExtensions
    {

        public static void InstallNetworkServices(this JSContext context, HttpClient? client = null)
        {
            client ??= new HttpClient();

            context[Names.fetch] = new JSFunction((in Arguments a) => {
                return ClrProxy.Marshal(FetchApi.Fetch(context, client, a));
            });

            context[Names.Request] = ClrType.From(typeof(Request));

            context[Names.AbortController] = ClrType.From(typeof(AbortController));

            context[Names.AbortSignal] = ClrType.From(typeof(AbortSignal));

            context[Names.FormData] = ClrType.From(typeof(FormData));

            context[Names.Headers] = ClrType.From(typeof(Headers));
        }

    }
}
