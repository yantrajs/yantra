#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSClassGenerator]
    public partial class FormData : KeyValueStore
    {
        public FormData(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
        }

        internal FormData(JSValue? first) : this()
        {
        }

        [JSExport]
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var pair in this.GetEnumerable())
            {
                sb.Append(Uri.EscapeDataString(pair.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(pair.Value));
                sb.Append('&');
            }
            return sb.ToString();
        }
    }
}
