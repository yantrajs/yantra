using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core.Set
{
    public class JSSet: JSObject {

        private LinkedList<JSValue> entries = new LinkedList<JSValue>();
        private BinaryCharMap<LinkedListNode<JSValue>> cache = new BinaryCharMap<LinkedListNode<JSValue>>();

        [Constructor]
        public static JSSet Constructor(JSValue t, JSValue[] a)
        {
            return new JSSet();
        }

        [GetProperty("size")]
        public static JSValue GetSize(JSValue t, JSValue[] a)
        {
            return new JSNumber(t.ToSet().entries.Count);
        }

        [Prototype("add")]
        public static JSValue Add(JSValue t, JSValue[] a)
        {
            var s = t.ToSet();
            var first = a.Get1();
            var key = first.ToUniqueID();
            s.cache[key] = s.entries.AddFirst(first);
            return t;
        }

        [Prototype("delete")]
        public static JSValue Delete(JSValue t, JSValue[] a)
        {
            var s = t.ToSet();
            var key = a.Get1().ToUniqueID();
            if(s.cache.TryGetValue(key, out  var i))
            {
                s.entries.Remove(i);
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Prototype("clear")]
        public static JSValue Clear(JSValue t, JSValue[] a)
        {
            var m = t.ToSet();
            m.entries.Clear();
            m.cache = new BinaryCharMap<LinkedListNode<JSValue>>();
            return JSUndefined.Value;
        }

        [Prototype("entries")]
        public static JSValue Entries(JSValue t, JSValue[] a)
        {
            return new JSArray(t.ToSet().entries.Select(x => new JSArray(x)));
        }


        [Prototype("forEach")]
        public static JSValue ForEach(JSValue t, JSValue[] a)
        {
            var fx = a.GetAt(0);
            if (!fx.IsFunction)
                throw JSContext.Current.NewTypeError($"Function parameter expected");
            var m = t.ToSet();
            foreach (var e in m.entries)
            {
                fx.InvokeFunction(t, e, m);
            }
            return JSUndefined.Value;
        }

        [Prototype("has")]
        public static JSValue Has(JSValue t, JSValue[] a)
        {
            var m = t.ToSet();
            var key = a.Get1().ToUniqueID();
            if (m.cache.TryGetValue(key, out var _))
                return JSBoolean.True;
            return JSBoolean.False;
        }

        [Prototype("values")]
        public static JSValue Values(JSValue t, JSValue[] a)
        {
            return new JSArray(t.ToSet().entries);
        }


    }

    internal static class JSSetStatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSSet ToSet(this JSValue t)
        {
            if (!(t is JSSet m))
                throw JSContext.Current.NewTypeError($"Receiver is not a set");
            return m;
        }
    }
}
