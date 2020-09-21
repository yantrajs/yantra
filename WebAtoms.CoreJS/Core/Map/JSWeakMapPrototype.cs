using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSWeakMap
    {

        internal static class JSMapPrototype
        {
            [Constructor]
            public static JSValue Constructor(JSValue t, JSValue[] args)
            {
                return new JSMap();
            }

            private static JSWeakMap ToMap(JSValue t)
            {
                if (!(t is JSWeakMap m))
                    throw JSContext.Current.NewTypeError($"Receiver is not a map");
                return m;
            }


            [Prototype("clear")]
            public static JSValue Clear(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                m.entries.Clear();
                m.cache = new BinaryCharMap<LinkedListNode<(JSValue key, WeakReference<JSValue> value)>>();
                return JSUndefined.Value;
            }

            [Prototype("delete")]
            public static JSValue Delete(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                var key = a.Get1().ToUniqueID();
                if(m.cache.TryRemove(key, out var e))
                {
                    m.entries.Remove(e);
                }
                return JSUndefined.Value;
            }


            [Prototype("entries")]
            public static JSValue Entries(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                var r = new JSArray();
                foreach(var entry in m.entries)
                {
                    r.Add(new JSArray(entry.key, entry.value.TryGetTarget(out var v) ? v : JSUndefined.Value));
                }
                return r;
            }


            [Prototype("forEach")]
            public static JSValue ForEach(JSValue t, JSValue[] a)
            {
                var fx = a.GetAt(0);
                if (!fx.IsFunction)
                    throw JSContext.Current.NewTypeError($"Function parameter expected");
                var m = ToMap(t);
                foreach (var e in m.entries)
                {
                    fx.InvokeFunction(t, 
                        e.value.TryGetTarget(out var v) ? v : JSUndefined.Value
                        , e.key, m);
                }
                return JSUndefined.Value;
            }

            [Prototype("get")]
            public static JSValue Get(JSValue t, JSValue[] a)
            {
                var first = a.GetAt(0);
                var m = ToMap(t);
                var key = first.ToUniqueID();
                if (m.cache.TryGetValue(key, out var e))
                    if (e.Value.value.TryGetTarget(out var v))
                        return v;
                return JSUndefined.Value;
            }

            [Prototype("has")]
            public static JSValue Has(JSValue t, JSValue[] a)
            {
                var first = a.GetAt(0);
                var m = ToMap(t);
                var key = first.ToUniqueID();
                if (m.cache.TryGetValue(key, out var _))
                    return JSBoolean.True;
                return JSBoolean.False;
            }

            [Prototype("keys")]
            public static JSValue Keys(JSValue t, JSValue[] a)
            {
                return new JSArray(ToMap(t).entries.Select(x => x.key));
            }


            [Prototype("set")]
            public static JSValue Set(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                var first = a.GetAt(0);
                var second = a.GetAt(1);
                var key = first.ToUniqueID();
                if(m.cache.TryGetValue(key, out var entry))
                {
                    if (entry.Value.value.TryGetTarget(out var v))
                        return v;
                }
                m.cache.Save(key, m.entries.AddLast((first, new WeakReference<JSValue>(second))));
                return second;
            }

            [Prototype("values")]
            public static JSValue Values(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                var r = new JSArray();
                foreach (var entry in m.entries)
                {
                    r.Add(entry.value.TryGetTarget(out var v) ? v : JSUndefined.Value);
                }
                return r;
            }
        }
    }
}
