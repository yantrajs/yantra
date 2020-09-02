using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArray: JSObject, IList<JSValue>
    {
        internal BinaryUInt32Map<JSValue> elements = new BinaryUInt32Map<JSValue>();

        internal uint _length;

        public JSArray(): base(JSContext.Current.ArrayPrototype)
        {
            
        }

        public JSArray(params JSValue[] items): this((IEnumerable<JSValue>)items)
        {

        }

        public JSArray(IEnumerable<JSValue> items): base(JSContext.Current.ArrayPrototype)
        {
            foreach (var item in items)
                elements[_length++] = item;
        }

        public override string ToString()
        {
            return $"[{string.Join(",", All)}]";
        }

        public override string ToDetailString()
        {
            var all = All.Select(a => a.ToDetailString());
            return $"[{string.Join(", ", all)}]";
        }

        public IEnumerable<JSValue> All
        {
            get
            {
                int i = 0;
                foreach(var a in elements.AllValues())
                {
                    var index = a.Key;
                    while (index > i)
                    {
                        yield return JSUndefined.Value;
                        i++;
                    }
                    yield return a.Value;
                    i++;
                }
                while (i < _length)
                {
                    yield return JSUndefined.Value;
                    i++;
                }
            }
        }

        public override int Length { 
            get => (int)_length; 
            set => _length = (uint)value; 
        }

        int ICollection<JSValue>.Count => (int)this._length;

        bool ICollection<JSValue>.IsReadOnly => false;

        JSValue IList<JSValue>.this[int index] { get => this[(uint)index]; set => this[(uint)index] = value; }

        public override JSValue this[uint key] {
            get => key >= _length ? JSUndefined.Value : (elements[key] ?? JSUndefined.Value);
            set
            {
                if (key >= _length)
                    _length = key + 1;
                elements[key] = value;
            }
        }

        [Prototype("push")]
        public static JSValue Push (JSValue t, JSArguments a){
            var ta = (JSArray)t;
            foreach(var item in a.elements)
            {
                ta.elements[ta._length] = item;
                ta._length++;
            }
            return new JSNumber(ta._length);
        }

        [Prototype("pop")]
        public static JSValue Pop(JSValue t, JSArguments a)
        {
            var ta = (JSArray)t;
            if (ta._length == 0)
                return JSUndefined.Value;
            JSValue r;
            ta.elements.TryRemove(ta._length - 1, out r);
            ta._length--;
            return r ?? JSUndefined.Value;
        }

        [Prototype("slice")]
        public static JSArray Slice(JSValue t, JSArguments a){
            var ta = (JSArray)t;
            var a0 = a.elements[0]?.IntValue ?? 0;
            var a1 = a.elements[1]?.IntValue ?? -1;
            return ta.Slice(a0, a1);
        }

        public JSArray Slice(int value, int length = -1)
        {
            JSArray a = new JSArray();
            uint l = _length;
            if (length != -1)
            {
                l = (uint)length > _length ? _length : (uint)length;
            }
            for (uint i = (uint)value; i < l; i++)
            {
                var item = elements[i];
                if (item == null) continue;
                a.elements[i] = item;
            }
            return a;
        }

        [Static("from")]
        public static JSValue From(JSValue t, JSArguments a)
        {
            var r = new JSArray();
            var f = a[0];
            var map = a[1];
            switch (f) {
                case JSUndefined u:
                    throw JSContext.Current.Error("undefined is not iterable");
                case JSNull n:
                    throw JSContext.Current.Error("null is not iterable");
                case JSString str:
                    foreach(var ch in str.value)
                    {
                        JSValue item = new JSString(new string(ch, 1));
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(t, JSArguments.From(item));
                        }
                        r.elements[r._length++] = item;
                    }
                    return r;
                case JSArray array:
                    foreach (var ch in array.elements.AllValues())
                    {
                        JSValue item = ch.Value;
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(t, JSArguments.From(item));
                        }
                        r.elements[r._length++] = item;
                    }
                    return r;
            }
            return r;
        }

        [Static("isArray")]
        public static JSValue StaticIsArray(JSValue t, JSArguments a)
        {
            return a[0] is JSArray ? JSContext.Current.True : JSContext.Current.False;
        }

        [Static("of")]
        public static JSValue Of(JSValue t, JSArguments a)
        {
            var r = new JSArray();
            if (a.elements != null)
            {
                foreach (var e in a.elements)
                {
                    r.elements[r._length++] = e;
                }
            }
            return r;
        }

        [Prototype("length", MemberType.Get)]
        internal static JSValue GetLength(JSValue t, JSArguments a)
        {
            return new JSNumber(((JSArray)t)._length);
        }

        [Prototype("length", MemberType.Set)]
        internal static JSValue SetLength(JSValue t, JSArguments a)
        {
            return new JSNumber(((JSArray)t)._length = (uint)a[0].IntValue);
        }

        int IList<JSValue>.IndexOf(JSValue item)
        {
            if (this.elements.TryGetKeyOf(item, out var index))
                return (int)index;
            return -1;
        }

        void IList<JSValue>.Insert(int index, JSValue item)
        {
            var ui = (uint)index;
            var length = this._length;
            if (length >= ui)
            {
                this._length = ui + 1;
                this.elements[ui] = item;
                return;
            }

            var toInsert = item;

            while (ui > length - 1)
            {
                var nextIndex = ui + 1;
                this.elements[ui] = toInsert;
                toInsert = this.elements[nextIndex];
            }
        }

        void IList<JSValue>.RemoveAt(int index)
        {
            this.elements.RemoveAt((uint)index);
        }

        public void Add(JSValue item)
        {
            this.elements[this._length++] = item;
        }

        void ICollection<JSValue>.Clear()
        {
            this.elements = new BinaryUInt32Map<JSValue>();
            this._length = 0;
        }

        bool ICollection<JSValue>.Contains(JSValue item)
        {
            return this.elements.TryGetKeyOf(item, out var _);
        }

        void ICollection<JSValue>.CopyTo(JSValue[] array, int arrayIndex)
        {
            var en = this.All.GetEnumerator();
            while (arrayIndex-- > 0 && en.MoveNext()) ;
            while (en.MoveNext())
            {
                array[arrayIndex++] = en.Current;
            }
        }

        bool ICollection<JSValue>.Remove(JSValue item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<JSValue> IEnumerable<JSValue>.GetEnumerator()
        {
            foreach(var item in this.All)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<JSValue>)this).GetEnumerator();
        }
    }
}
