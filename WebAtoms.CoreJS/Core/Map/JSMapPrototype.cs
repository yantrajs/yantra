using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public partial class JSMap
    {

        internal static class JSMapPrototype
        {
            [Constructor]
            public static JSValue Constructor(JSValue t, JSValue[] args)
            {
                return new JSMap();
            }

            private static JSMap ToMap(JSValue t)
            {
                if (!(t is JSMap m))
                    throw JSContext.Current.NewTypeError($"Receiver is not a map");
                return m;
            }


            [GetProperty("size")]
            public static JSValue GetSize(JSValue t, JSValue[] a)
            {
                return new JSNumber(ToMap(t).entries.Count);
            }

            [Prototype("clear")]
            public static JSValue Clear(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                m.entries.Clear();
                m.cache = new BinaryCharMap<Entry>();
                return JSUndefined.Value;
            }

            [Prototype("delete")]
            public static JSValue Delete(JSValue t, JSValue[] a)
            {
                var m = ToMap(t);
                var key = a.Get1().ToUniqueID();
                if(m.cache.TryRemove(key, out var e))
                {
                    m.entries.RemoveAt(e.Index);
                }
                return JSUndefined.Value;
            }


            [Prototype("entries")]
            public static JSValue Entries(JSValue t, JSValue[] a)
            {
                return new JSArray(ToMap(t).entries.Select(x => new JSArray(x.key, x.value)));
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
                    fx.InvokeFunction(t, e.value, e.key, m);
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
                    return e.value;
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
                    return entry.value;
                }
                var index = m.entries.Count;
                entry = new Entry { 
                    Index = index,
                    key = first,
                    value = second
                };
                m.entries.Add(entry);
                m.cache.Save(key, entry);
                return second;
            }

            [Prototype("values")]
            public static JSValue Values(JSValue t, JSValue[] a)
            {
                return new JSArray(ToMap(t).entries.Select(x => x.value));
            }
        }
    }
}
