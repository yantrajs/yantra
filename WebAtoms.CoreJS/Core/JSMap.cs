using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public class JSMap: JSObject
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
            ToMap(t).entries.Clear();
            return JSUndefined.Value;
        }

        [Prototype("delete")]
        public static JSValue Delete(JSValue t, JSValue[] a)
        {
            ToMap(t).entries.Remove(new Entry { key = a[0] });
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
                throw JSContext.Current.NewTypeError($"Function paramtere expected");
            var m = ToMap(t);
            foreach(var e in m.entries)
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
            foreach (var e in m.entries)
            {
                if (e.key.Equals(first).BooleanValue)
                    return e.value;
            }
            return JSUndefined.Value;
        }

        [Prototype("has")]
        public static JSValue Has(JSValue t, JSValue[] a)
        {
            var first = a.GetAt(0);
            var m = ToMap(t);
            foreach (var e in m.entries)
            {
                if (e.key.Equals(first).BooleanValue)
                    return JSBoolean.True;
            }
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
            m.entries.Remove(new Entry { key = first });
            m.entries.Add(new Entry { key = first, value = second });
            return second;
        }

        [Prototype("values")]
        public static JSValue Values(JSValue t, JSValue[] a)
        {
            return new JSArray(ToMap(t).entries.Select(x => x.value));
        }

    }
}
