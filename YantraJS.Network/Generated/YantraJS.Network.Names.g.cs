using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Extensions;
using YantraJS.Core;
namespace YantraJS.Network { 
partial class Names {
static Names() {
}
static private void RegisterAll(JSContext context) {
YantraJS.Network.AbortController.CreateClass(context);
YantraJS.Network.AbortSignal.CreateClass(context);
YantraJS.Network.Blob.CreateClass(context);
YantraJS.Network.Request.CreateClass(context);
YantraJS.Network.FetchResponse.CreateClass(context);
YantraJS.Network.KeyValueStore.CreateClass(context);
YantraJS.Network.URL.CreateClass(context);
YantraJS.Network.URLSearchParams.CreateClass(context);
YantraJS.Network.FormData.CreateClass(context);
YantraJS.Network.Headers.CreateClass(context);
}
}
}
