using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using YantraJS.Core.Runtime;
using YantraJS;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    [JSRuntime(typeof(JSArrayStatic), typeof(JSArrayPrototype))]
    public partial class JSArray: JSObject
    {
        internal uint _length;

        public JSArray(): base(JSContext.Current.ArrayPrototype)
        {
            
        }

        public JSArray(params JSValue[] items): this((IEnumerable<JSValue>)items)
        {

        }

        public JSArray(IElementEnumerator en): base(JSContext.Current.ArrayPrototype)
        {
            ref var elements = ref GetElements(true);
            while (en.MoveNextOrDefault(out var v, JSUndefined.Value))
                elements[_length++] = JSProperty.Property(v);
        }

        public JSArray(IEnumerable<JSValue> items): this()
        {
            ref var elements = ref GetElements(true);
            foreach (var item in items)
                elements[_length++] = JSProperty.Property(item);
        }

      
        internal IElementEnumerator GetEntries()
        {
            return new EntryEnumerator(this);
        }

        

        public JSArray(uint count): base(JSContext.Current.ArrayPrototype)
        {
            CreateElements(count);
            _length = count;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for(uint i =0; i<_length;i++)
            {
                if (i > 0)
                    sb.Append(',');
                var item = this[i];
                if (item != null && !item.IsNullOrUndefined)
                    sb.Append(item);
            }
            return sb.ToString();
        }

        public override string ToDetailString()
        {
            return $"[{this.ToString()}]"; ;
        }

        public override JSValue this[uint name]
        {
            get => this.GetValue(GetInternalProperty(name));
            set
            {
                var p = GetInternalProperty(name);
                if (p.IsProperty)
                {
                    if (p.set != null)
                    {
                        p.set.f(new Arguments(this, value));
                        return;
                    }
                    return;
                }
                if (this.IsSealedOrFrozen())
                    throw JSContext.Current.NewTypeError($"Cannot modify property {name} of {this}");
                if (this._length <= name)
                    this._length = name + 1;
                ref var elements = ref CreateElements();
                elements[name] = JSProperty.Property(value);
                Dirty();
            }
        }

        public override bool IsArray => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(uint index, JSValue value)> GetArrayElements(bool withHoles = true)
        {
            var elements = GetElements();
            uint l = this._length;
            for (uint i = 0; i < l; i++)
            {
                if(elements.TryGetValue(i, out var p))
                {
                    yield return (i, p.value);
                    continue;
                }
                if (withHoles)
                    yield return (i, JSUndefined.Value);
            }
        }

        public override int Length { 
            get => (int)_length; 
            set => _length = (uint)value; 
        }


        public JSArray Add(JSValue item)
        {
            if (item == null)
            {
                this._length++;
            }
            else
            {
                ref var elements = ref CreateElements();
                elements[this._length++] = JSProperty.Property(item);
            }
            return this;
        }

        //internal override bool TryRemove(uint i, out JSProperty p)
        //{
        //    ref var elements = ref GetElements();
        //    return elements.TryRemove(i, out p);
        //}

        public override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }


        private struct ElementEnumerator: IElementEnumerator
        {
            uint length;
            uint index;
            JSArray array;
            public ElementEnumerator(JSArray array)
            {
                this.length = array._length;
                this.array = array;
                index = uint.MaxValue;
            }

            public bool MoveNext(out JSValue value)
            {
                ref var elements = ref array.GetElements();
                if ((this.index = (this.index == uint.MaxValue) ? 0 : (this.index + 1)) < length)
                {
                    if (elements.TryGetValue(index, out var property))
                    {
                        value = property.IsEmpty
                            ? null
                            : (property.IsValue
                            ? property.value
                            : (property.set.InvokeFunction(new Arguments(this.array))));
                    }
                    else
                    {
                        value = JSUndefined.Value;
                    }
                    return true;
                }
                value = JSUndefined.Value;
                return false;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                ref var elements = ref array.GetElements();
                if((this.index = (this.index == uint.MaxValue) ? 0 : (this.index + 1)) < length)
                {
                    index = this.index;
                    if(elements.TryGetValue(index, out var property))
                    {
                        value = property.IsEmpty 
                            ? null 
                            : (property.IsValue
                            ? property.value
                            : (property.set.InvokeFunction(new Arguments(this.array))));
                        hasValue = true;
                    } else
                    {
                        hasValue = false;
                        value = JSUndefined.Value;
                    }
                    return true;
                }
                index = 0;
                value = JSUndefined.Value;
                hasValue = false;
                return false;
            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                ref var elements = ref array.GetElements();
                if ((this.index = (this.index == uint.MaxValue) ? 0 : (this.index + 1)) < length)
                {
                    if (elements.TryGetValue(index, out var property))
                    {
                        value = property.IsEmpty
                            ? null
                            : (property.IsValue
                            ? property.value
                            : (property.set.InvokeFunction(new Arguments(this.array))));
                    }
                    else
                    {
                        value = @default;
                    }
                    return true;
                }
                value = @default;
                return false;
            }
        }

        internal void AddRange(JSValue iterator)
        {
            ref var et = ref CreateElements();
            // var et = this.elements;
            var el = this._length;
            if (iterator is JSArray ary)
            {
                var l = ary._length;
                ref var e = ref ary.GetElements();
                for (uint i = 0; i < l; i++)
                {
                    et[el++] = JSProperty.Property(ary[i]);
                }
                this._length = el;
                return;
            }

            var en = iterator.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var  index))
            {
                if (hasValue)
                {
                    et[el++] = JSProperty.Property(item);
                } else
                {
                    el++;
                }
            }
            this._length = el;
        }
    }


    struct EntryEnumerator : IElementEnumerator
    {
        private JSArray array;
        private int index;

        public EntryEnumerator(JSArray typedArray)
        {
            this.array = typedArray;
            this.index = -1;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (++this.index < array.Length)
            {
                hasValue = true;
                index = (uint)this.index;
                value = new JSArray(new JSNumber(index), array[index]);
                return true;
            }

            hasValue = false;
            index = 0;
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            if (++this.index < array.Length)
            {
                value = new JSArray(new JSNumber(index), array[(uint)index]);
                return true;
            }

            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (++this.index < array.Length)
            {
                value = new JSArray(new JSNumber(index), array[(uint)index]);
                return true;
            }

            value = @default;
            return false;
        }
    }
}
