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
            public JSValue key;
            public JSValue value;

            public override bool Equals(object obj)
            {
                if (obj is Entry entry)
                {
                    return key.Equals(entry.key).BooleanValue;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return key.GetHashCode();
            }
        }

        private List<Entry> entries = new List<Entry>();

        public JSMap(): base(JSContext.Current.MapPrototype)
        {
        }

    }
}
