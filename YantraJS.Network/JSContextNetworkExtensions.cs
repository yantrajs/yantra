#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Yantra.Core.Events;
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
            EventTarget.CreateClass(context);
            KeyValueStore.CreateClass(context);

            FormData.CreateClass(context);

            Headers.CreateClass(context);
            Request.CreateClass(context);
            AbortController.CreateClass(context);
            AbortSignal.CreateClass(context);
            Blob.CreateClass(context);
            URL.CreateClass(context);
            URLSearchParams.CreateClass(context);
        }

    }
}
