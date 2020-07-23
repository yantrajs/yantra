using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArray: JSObject
    {
        internal BinaryUInt32Map<JSValue> elements = new BinaryUInt32Map<JSValue>();

        internal uint _length;

        public JSArray(): base(JSContext.Current.ArrayPrototype)
        {
            
        }

        public override string ToString()
        {
            return $"[{string.Join(",", All)}]";
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

        public override JSValue this[uint key] {
            get => elements[key] ?? JSUndefined.Value;
            set
            {
                if (key >= _length)
                    _length = key + 1;
                elements[key] = value;
            }
        }

        public static JSValue Push (JSValue t, JSArray a){
            var ta = (JSArray)t;
            foreach(var item in a.All)
            {
                ta.elements[ta._length] = item;
                ta._length++;
            }
            return new JSNumber(ta._length);
        }

        public static JSValue Pop(JSValue t, JSArray a)
        {
            var ta = (JSArray)t;
            if (ta._length == 0)
                return JSUndefined.Value;
            JSValue r;
            ta.elements.TryRemove(ta._length - 1, out r);
            ta._length--;
            return r ?? JSUndefined.Value;
        }

        public static JSArray Slice(JSValue t, JSArray a){
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

        public static JSArray From(JSValue t, JSArray a)
        {
            return a;
        }

        internal new static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty, "Array");
            var p = r.prototype;

            p.DefineProperty(KeyStrings.length, JSProperty.Property(
                get: (t, a) => new JSNumber(((JSArray)t)._length),
                set: (t, a) => new JSNumber(((JSArray)t)._length = (uint)a[0].IntValue)
            ));

            p.DefineProperty("slice", JSProperty.Function(Slice));
            p.DefineProperty("push", JSProperty.Function(Push));
            p.DefineProperty("pop", JSProperty.Function(Pop));

            r.DefineProperty("from", JSProperty.Function(From));


            return r;
        }

        internal static JSArray NewInstance(params JSValue[] list)
        {
            var a = new JSArray();
            uint i = 0;
            foreach(var item in list)
            {
                a[i++] = item;
            }
            return a;
        }
    }
}
