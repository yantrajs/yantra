using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    internal delegate void UnregisterWeakValue(in HashedString key);

    internal class WeakValue
    {
        private readonly UnregisterWeakValue unregister;
        private readonly HashedString key;
        public readonly JSValue value;

        public WeakValue(HashedString key, JSValue value, UnregisterWeakValue unregister)
        {
            this.unregister = unregister;
            this.key = key;
            this.value = value;
        }

        ~WeakValue()
        {
            unregister(in key);
        }
    }

    [JSClassGenerator("WeakMap")]
    public partial class JSWeakMap: JSObject
    {


        private StringMap<WeakReference<WeakValue>> index;

        public JSWeakMap(in Arguments a) : base(JSContext.NewTargetPrototype)
        {
            if (a[0] is JSArray array)
            {
                var en = array.GetElementEnumerator();
                while (en.MoveNext(out var value))
                    Set((JSObject)value[0], value[1]);
            }

        }

        [JSExport("set")]
        public JSValue Set(JSObject key, JSValue value)
        {
            HashedString uk = key.ToUniqueID();
            lock (this)
            {
                if (!index.TryGetValue(uk, out var w))
                {
                    index.Put(uk) = new(new(uk, value, Unregister));
                }
            }
            return value;
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

        [JSExport("has")]
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


        [JSExport("get")]
        public JSValue Get(JSObject key)
        {
            var uk = key.ToUniqueID();
            lock (this)
            {
                if (index.TryGetValue(uk, out var v))
                {
                    if (v.TryGetTarget(out var target))
                    {
                        return target.value;
                    }
                }
            }

            return JSUndefined.Value;
        }
    }
}
