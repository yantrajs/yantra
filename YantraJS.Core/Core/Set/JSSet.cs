#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core.Set
{

    // [JSRuntime(typeof(JSMapStatic), typeof(JSMap.JSMapPrototype))]
    [JSClassGenerator("Set")]
    public partial class JSSet: JSObject {

        private LinkedList<JSValue> store = new LinkedList<JSValue>();
        private StringMap<LinkedListNode<JSValue>> index;

        [JSExport]
        public int Size => store?.Count ?? 0;

        public JSSet(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            if (a[0] is JSArray array)
            {
                var en = array.GetElementEnumerator();
                while (en.MoveNext(out var item))
                {
                    this.Add(item);
                }
            }

        }

        [JSExport("add")]
        public JSValue Add(JSValue key)
        {
            HashedString uk = key.ToUniqueID();
            if (!index.TryGetValue(in uk, out var i))
            {
                var node = store.AddLast(key);
                index.Put(in uk) = node;
            }
            return key;
        }

        [JSExport("clear")]
        public JSValue Set(in Arguments a)
        {
            index = new ();
            store.Clear();
            return JSUndefined.Value;
        }

        [JSExport("delete")]
        public JSValue Delete(in Arguments a)
        {
            var f = a[0];
            HashedString uk = f.ToUniqueID();
            if(index.TryGetValue(in uk, out var i))
            {
                store.Remove(i);
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [JSExport("entries")]
        public IEnumerable<JSValue> GetEntries() {
            if (store == null)
            {
                yield break;
            }
            foreach(var entry in store)
            {
                yield return new JSArray(entry, entry);
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
                fx.Call(@this, e, e, this);
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

        [JSExport("keys")]
        public IEnumerable<JSValue> Keys()
        {
            if (store == null)
            {
                yield break;
            }
            foreach (var entry in store)
            {
                yield return entry;
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
                yield return entry;
            }
        }
    }
}
