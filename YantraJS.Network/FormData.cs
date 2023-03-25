#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    public class FormData : KeyValueStore
    {
        public FormData(in Arguments a) : base(a)
        {
        }

        internal FormData(JSValue? first) : base(first)
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
