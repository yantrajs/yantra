using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSArray: JSObject
    {
        internal BinaryUInt32Map<JSValue> elements = new BinaryUInt32Map<JSValue>();

        internal uint _length;

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
            ta[ta._length] = a[0];
            return new JSNumber(ta._length);
        });

        public JSValue Slice(int value)
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
