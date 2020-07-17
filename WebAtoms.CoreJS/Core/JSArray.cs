using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArray: JSObject
    {
        internal BinaryUInt32Map<JSValue> elements = new BinaryUInt32Map<JSValue>();

        internal uint _length;

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

        internal static JSProperty length = JSProperty.Property(
            get: (t, a) => new JSNumber(((JSArray)t)._length),
            set: (t, a) =>
            {
                var ta = (JSArray)t;
                ta._length = (uint)a[0].IntValue;
                return new JSNumber(ta._length);
            }
        );

        internal static JSProperty push = JSProperty.Function((t, a) => {
            var ta = (JSArray)t;
            foreach(var item in a.All)
            {
                ta.elements[ta._length] = item;
                ta._length++;
            }
            return new JSNumber(ta._length);
        });

        internal static JSProperty pop = JSProperty.Function((t, a) => {
            var ta = (JSArray)t;
            var n = ta._length - 1;
            var r = ta.elements[n];
            ta._length = n;
            return r;
        });

        public JSArray Slice(int value)
        {
            JSArray a = new JSArray();
            for (uint i = (uint)value; i < _length; i++)
            {
                var item = elements[i];
                if (item == null) continue;
                a.elements[i] = item;
            }
            return a;
        }
    }
}
