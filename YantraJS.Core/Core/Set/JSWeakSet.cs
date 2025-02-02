using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core.Set
{
    [JSClassGenerator("WeakSet")]
    public partial class JSWeakSet : JSObject
    {

        private StringMap<WeakReference<WeakValue>> index;

        public JSWeakSet(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
            if (a[0] is JSArray array)
            {
                var en = array.GetElementEnumerator();
                while (en.MoveNext(out var value))
                    Add((JSObject)value);
            }
        }

        [JSExport("add")]
        public JSValue Add(JSObject a)
        {
            HashedString key = a.ToUniqueID();
            lock (this)
            {
                if (!index.TryGetValue(key, out var w))
                {
                    index.Put(key) = new(new (key, a, Unregister));
                }
            }
            return a;
        }

        private void Unregister(in HashedString key)
        {
            index.RemoveAt(key.Value);
        }

        [JSExport("delete")]
        public JSValue Delete(in Arguments a)
        {
            var key = a.Get1().ToUniqueID();
            lock (this)
            {
                if (index.TryRemove(key, out var w))
                {
                    if (w.TryGetTarget(out var target))
                    {
                        GC.SuppressFinalize(target);
                    }
                    return JSBoolean.True;
                }
            }

            return JSBoolean.False;
        }

        [Prototype("has")]
        public JSValue Has(in Arguments a)
        {
            var key = a.Get1().ToUniqueID();
            lock (this)
            {
                if (index.TryGetValue(key, out var v))
                {
                    if (v.TryGetTarget(out var target))
                    {
                        return JSBoolean.True;
                    }
                }
            }

            return JSUndefined.Value;
        }
    }
}
