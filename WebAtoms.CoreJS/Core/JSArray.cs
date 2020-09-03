using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public class JSArray: JSObject
    {
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
                elements[_length++] = JSProperty.Property(item);
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
                    yield return a.Value.value;
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


        [Prototype("push")]
        public static JSValue Push (JSValue t,params JSValue[] a){
            var ta = (JSArray)t;
            foreach(var item in a)
            {
                ta.elements[ta._length] = JSProperty.Property(item);
                ta._length++;
            }
            return new JSNumber(ta._length);
        }

        [Prototype("pop")]
        public static JSValue Pop(JSValue t,params JSValue[] a)
        {
            var ta = (JSArray)t;
            if (ta._length == 0)
                return JSUndefined.Value;
            JSProperty r;
            if (ta.elements.TryRemove(ta._length - 1, out r))
            {
                ta._length--;
                return r.value;
            }
            return JSUndefined.Value;
        }

        [Prototype("slice")]
        public static JSArray Slice(JSValue t,params JSValue[] a){
            var ta = (JSArray)t;
            var start = a.TryGetAt(0, out var a0) ? a0.IntValue : 0;
            var end = a.TryGetAt(1, out var a1) ? a1.IntValue : -1;
            return ta.Slice(start, end);
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
                if (item.IsEmpty) continue;
                a.elements[i] = item;
            }
            return a;
        }

        [Static("from")]
        public static JSValue From(JSValue t,params JSValue[] a)
        {
            var r = new JSArray();
            var f = a.GetAt(0);
            var map = a.GetAt(1);
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
                            item = fn.InvokeFunction(t, item);
                        }
                        r.elements[r._length++] = JSProperty.Property(item);
                    }
                    return r;
                case JSArray array:
                    foreach (var ch in array.elements.AllValues())
                    {
                        JSValue item = ch.Value.value;
                        if (map is JSFunction fn)
                        {
                            item = fn.InvokeFunction(t, item);
                        }
                        r.elements[r._length++] = JSProperty.Property(item);
                    }
                    return r;
            }
            return r;
        }

        [Static("isArray")]
        public static JSValue StaticIsArray(JSValue t,params JSValue[] a)
        {
            return a[0] is JSArray ? JSContext.Current.True : JSContext.Current.False;
        }

        [Static("of")]
        public static JSValue Of(JSValue t,params JSValue[] a)
        {
            var r = new JSArray();
            if (a != null)
            {
                foreach (var e in a)
                {
                    r.elements[r._length++] = JSProperty.Property(e);
                }
            }
            return r;
        }

        [Prototype("length", MemberType.Get)]
        internal static JSValue GetLength(JSValue t,params JSValue[] a)
        {
            return new JSNumber(((JSArray)t)._length);
        }

        [Prototype("length", MemberType.Set)]
        internal static JSValue SetLength(JSValue t,params JSValue[] a)
        {
            return new JSNumber(((JSArray)t)._length = (uint)a[0].IntValue);
        }


        public JSArray Add(JSValue item)
        {
            this.elements[this._length++] = JSProperty.Property(item);
            return this;
        }

    }
}
