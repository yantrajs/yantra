using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSMapStatic), typeof(JSMap.JSMapPrototype))]
    public partial class JSMap: JSObject
    {

        struct Entry
        {
            public int Index;
            public JSValue key;
            public JSValue value;
        }

        private List<Entry> entries = new List<Entry>();
        private BinaryCharMap<Entry> cache = new BinaryCharMap<Entry>();

        public JSMap(): base(JSContext.Current.MapPrototype)
        {
        }

    }
}
