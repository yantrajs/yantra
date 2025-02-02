using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Generator;

namespace YantraJS.Core.Typed
{

    [JSClassGenerator("TypedArray")]
    public partial class JSTypedArray: JSObject
    {


        [JSExport]
        private static JSValue From(in Arguments a)
        {
            return a.This.InvokeMethod(Names.from, a);
        }

        [JSExport]
        private static JSValue Of(in Arguments a)
        {
            return a.This.InvokeMethod(Names.of, a);
        }

        [JSExport]
        internal readonly JSArrayBuffer buffer;
        [JSExport]
        public readonly int byteOffset;

        [JSExport("BYTES_PER_ELEMENT")]
        internal readonly int bytesPerElement;

        [JSExport]
        internal readonly int length;

        [JSExport]
        internal int ByteLength => buffer.buffer.Length;
        

        public override int Length { get => length; set => throw new NotSupportedException(); }


        public JSTypedArray(in Arguments a): this(JSContext.NewTargetPrototype)
        {
            throw new NotSupportedException();
        }

        public JSTypedArray(in TypedArrayParameters p): this(p.prototype) {
            buffer = p.buffer;
            length = p.length;
            byteOffset = p.byteOffset;
            bytesPerElement = p.bytesPerElement;
            if (p.copyFrom == null)
            {
                if (buffer == null)
                {
                    buffer = new JSArrayBuffer(length * bytesPerElement);
                } else {
                    var l = length;
                    if (l == -1)
                    {
                        l = buffer.buffer.Length - byteOffset;
                        length = l / bytesPerElement;
                    }
                    else
                    {
                        length = l / bytesPerElement;
                    }

                    if (l < 0 ||
                        ((byteOffset + l) > buffer.buffer.Length))
                        throw JSContext.Current.NewRangeError($"Start offset {byteOffset} is outside the bounds of the buffer");

                    if (((l - byteOffset) % bytesPerElement) != 0)
                    {
                        throw JSContext.Current.NewRangeError($"byte length of TypedArray should be multiple of {bytesPerElement}");
                    }

                }
                return;
            }

            if(p.copyFrom == null)
            {
                return;
            }

            var source = p.copyFrom;

            // copy..
            length = -1;
            switch (source)
            {
                case JSArray array:
                    length = array.Length;
                    break;
                case JSString @string:
                    length = @string.Length;
                    break;
                case JSTypedArray typed:
                    length = typed.Length;
                    break;
            }
            IElementEnumerator en2;
            /*
             * If length is unknown, create a List and get its count
             * 
             */
            if (length == -1)
            {
                var en = source.GetElementEnumerator();
                var elements = new List<JSValue>();
                while (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    elements.Add(item);
                }
                length = elements.Count;
                en2 = new ListElementEnumerator(elements.GetEnumerator());
            }
            else
            {
                en2 = source.GetElementEnumerator();
            }

            buffer = new JSArrayBuffer(length * bytesPerElement);

            if (p.map == null || p.map.IsUndefined)
            {
                uint i = 0;
                while (en2.MoveNext(out var item))
                {
                    this[i++] = item;
                }
            } else
            {
                uint i = 0;
                while (en2.MoveNext(out var item))
                {
                    this[i] = p.map.Call(p.thisArg, item, new JSNumber(i));
                    i++;
                }
            }
        }

        public override JSValue GetOwnPropertyDescriptor(JSValue name)
        {
            var key = name.ToKey(false);
            switch (key.Type)
            {
                case KeyType.String:
                    if (key.KeyString.Key == KeyStrings.length.Key)
                    {
                        var l = new JSObject();
                        l.FastAddValue(KeyStrings.value, new JSNumber(this.length), JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.writable, JSBoolean.False, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.enumerable, JSBoolean.True, JSPropertyAttributes.ConfigurableValue);
                        return l;
                    }
                    break;
                case KeyType.UInt:
                    if (key.Index < (uint)this.length)
                    {
                        var l = new JSObject();
                        var v = GetValue(key.Index, this, false);
                        l.FastAddValue(KeyStrings.value, v, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.writable, JSBoolean.True, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.enumerable, JSBoolean.True, JSPropertyAttributes.ConfigurableValue);
                        l.FastAddValue(KeyStrings.configurable, JSBoolean.False, JSPropertyAttributes.ConfigurableValue);
                        return l;

                    }
                    return JSUndefined.Value;
            }
            return base.GetOwnPropertyDescriptor(name);
        }
        public override bool BooleanValue => true;
        public override double DoubleValue => double.NaN;
        public override bool Equals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this} is not a function");
        }

        public override bool StrictEquals(JSValue value)
        {
            return Object.ReferenceEquals(this, value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    sb.Append(',');
                }

                sb.Append(this[(uint)i].ToString());
            }
            return sb.ToString();
        }

        public override string ToDetailString()
        {
            return this.ToString();
        }

        public override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        internal IElementEnumerator GetElementEnumerator(int startIndex)
        {
            return new ElementEnumerator(this, startIndex);
        }

        internal IElementEnumerator GetEntries()
        {
            return new EntryEnumerator(this);
        }

        public override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {
            return new KeyEnumerator(this.length);
        }

        internal JSGenerator GetKeys()
        {
            return new JSGenerator(new KeyEnumerator(this.length), "Array Iterator");
        }



        struct ElementEnumerator : IElementEnumerator
        {
            private JSTypedArray typedArray;
            private int index;

            public ElementEnumerator(JSTypedArray typedArray, int startIndex = 0)
            {
                this.typedArray = typedArray;
                this.index = startIndex - 1;

            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if (++this.index < typedArray.length)
                {
                    hasValue = true;
                    index = (uint)this.index;
                    value = typedArray[index];
                    return true;
                }

                hasValue = false;
                index = 0;
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (++this.index < typedArray.length)
                {
                    value = typedArray[(uint)index];
                    return true;
                }

                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    value = typedArray[(uint)index];
                    return true;
                }

                value = @default;
                return false;
            }

            public JSValue NextOrDefault(JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    return typedArray[(uint)index];
                }

                return @default;
            }

        }

        struct EntryEnumerator : IElementEnumerator
        {
            private JSTypedArray typedArray;
            private int index;

            public EntryEnumerator(JSTypedArray typedArray)
            {
                this.typedArray = typedArray;
                this.index = -1;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if (++this.index < typedArray.length)
                {
                    hasValue = true;
                    index = (uint)this.index;
                    value = new JSArray(new JSNumber(index), typedArray[index]);
                    return true;
                }

                hasValue = false;
                index = 0;
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNext(out JSValue value)
            {
                if (++this.index < typedArray.length)
                {
                    value = new JSArray(new JSNumber(index), typedArray[(uint)index]);
                    return true;
                }

                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    value = new JSArray(new JSNumber(index), typedArray[(uint)index]);
                    return true;
                }

                value = @default;
                return false;
            }
            public JSValue NextOrDefault(JSValue @default)
            {
                if (++this.index < typedArray.length)
                {
                    return new JSArray(new JSNumber(index), typedArray[(uint)index]);
                }

                return @default;
            }
        }


    }

    internal struct KeyEnumerator : IElementEnumerator
    {
        private int length;
        private int index;

        public KeyEnumerator(int length)
        {
            this.length = length;
            this.index = -1;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (++this.index < this.length)
            {
                hasValue = true;
                index = (uint)this.index;
                value = new JSNumber(index);
                return true;
            }
            hasValue = false;
            index = 0;
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            if (++this.index < this.length)
            {
                value = new JSNumber(index);
                return true;
            }
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (++this.index < this.length)
            {
                value = new JSNumber(index);
                return true;
            }
            value = @default;
            return false;
        }

        public JSValue NextOrDefault(JSValue @default)
        {
            if (++this.index < this.length)
            {
                return new JSNumber(index);
            }
            return @default;
        }

    }

}