using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    // [JSRuntime(typeof(JSMapStatic), typeof(JSMap.JSMapPrototype))]
    [JSClassGenerator("Map")]
    public partial class JSMap: JSObject
    {

        private LinkedList<(JSValue key,JSValue value)> store = new LinkedList<(JSValue,JSValue)>();
        private StringMap<LinkedListNode<(JSValue key,JSValue value)>> index
            = new StringMap<LinkedListNode<(JSValue, JSValue)>>();

        [JSExport]
        public int Size => store.Count;

        public JSMap(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            if (a[0] is JSArray array)
            {
                var en = array.GetElementEnumerator();
                while (en.MoveNext(out var item))
                {
                    this.Set(item[0], item[1]);
                }
            }
        }

        [JSExport("set")]
        public JSValue Set(JSValue key, JSValue value)
        {
            HashedString uk = key.ToUniqueID();
            if (index.TryGetValue(in uk, out var i))
            {
                i.Value = (key, value);
            } else
            {
                var node = store.AddLast((key, value));
                index.Put(in uk) = node;
            }
            return value;
        }

        [JSExport("clear")]
        public JSValue Set(in Arguments a)
        {
            index = new();
            store.Clear();
            return JSUndefined.Value;
        }

        [JSExport("delete")]
        public JSValue Delete(in Arguments a)
        {
            var f = a[0];
            HashedString uk = f.ToUniqueID();
            if (index.TryGetValue(in uk, out var i))
            {
                store.Remove(i);
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [JSExport("entries")]
        public IEnumerable<JSValue> GetEntries()
        {
            if (store == null)
            {
                yield break;
            }
            foreach (var entry in store)
            {
                yield return new JSArray(entry.key, entry.value);
            }
        }

        [JSExport("forEach")]
        public JSValue ForEach(in Arguments a)
        {
            var fx = a.Get1();
            if (!fx.IsFunction)
                throw JSContext.Current.NewTypeError($"Function parameter expected");
            var @this = a.This ?? this;
            if (store == null)
            {
                return JSUndefined.Value;
            }
            foreach (var e in store)
            {
                fx.Call(@this, e.key, e.value, this);
            }
            return JSUndefined.Value;
        }

        [JSExport("has")]
        public JSValue Has(in Arguments a)
        {
            var f = a.Get1();
            HashedString uk = f.ToUniqueID();
            if (index.TryGetValue(in uk, out var i))
            {
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }


        [JSExport("get")]
        public JSValue Get(JSValue key)
        {
            HashedString uk = key.ToUniqueID();
            if (index.TryGetValue(in uk, out var i))
            {
                return i.Value.value;
            }
            return JSUndefined.Value;
        }

        [JSExport("keys")]
        public IEnumerable<JSValue> Keys()
        {
            if (store == null)
            {
                yield break;
            }
            foreach (var entry in store)
            {
                yield return entry.key;
            }
        }


        [JSExport("values")]
        public IEnumerable<JSValue> Values()
        {
            if (store == null)
            {
                yield break;
            }
            foreach (var entry in store)
            {
                yield return entry.value;
            }
        }


    }
}
