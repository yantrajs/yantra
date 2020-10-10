using Esprima.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using WebAtoms.CoreJS.Core.Runtime;
using WebAtoms.CoreJS;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    [JSRuntime(typeof(JSArrayStatic), typeof(JSArrayPrototype))]
    public partial class JSArray: JSObject
    {
        internal uint _length;

        public JSArray(): base(JSContext.Current.ArrayPrototype)
        {
            elements = new UInt32Trie<JSProperty>();
        }

        public JSArray(params JSValue[] items): this((IEnumerable<JSValue>)items)
        {

        }

        public JSArray(IEnumerable<JSValue> items): this()
        {
            foreach (var item in items)
                elements[_length++] = JSProperty.Property(item);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            bool first = true;
            var en = new ElementEnumerator(this);
            while(en.MoveNext())
            {
                var item = en.Current;
                if (!first)
                    sb.Append(',');
                if (item != null && !item.IsUndefined)
                    sb.Append(item);
                first = false;
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
                elements = elements ?? (elements = new UInt32Trie<JSProperty>());
                elements[name] = JSProperty.Property(value);
            }
        }

        public override bool IsArray => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<(uint index, JSValue value)> GetArrayElements(bool withHoles = true)
        {
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
                this.elements[this._length++] = JSProperty.Property(item);
            }
            return this;
        }

        internal override bool TryRemove(uint i, out JSProperty p)
        {
            return elements.TryRemove(i, out p);
        }

        internal override IElementEnumerator GetElementEnumerator()
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

            public JSValue Current => !array.elements.TryGetValue(Index, out var property)
                ? JSUndefined.Value
                : (property.IsValue
                    ? property.value
                    : array.GetValue(property));

            public uint Index => index;

            public uint Length => array._length;

            public bool MoveNext()
            {
                return (index = (index == uint.MaxValue) ? 0 : (index + 1)) < length;
            }

            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                if((this.index = (this.index == uint.MaxValue) ? 0 : (this.index + 1)) < length)
                {
                    index = this.index;
                    if(array.elements.TryGetValue(index, out var property))
                    {
                        value = property.IsValue
                            ? property.value
                            : (property.set.InvokeFunction(new Arguments(this.array)));
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

            public bool TryGetCurrent(out JSValue value)
            {
                if(array.elements.TryGetValue(Index, out var property))
                {
                    value = property.IsEmpty
                        ? JSUndefined.Value
                        : (property.IsValue
                            ? property.value
                            : property.get.InvokeFunction(new Arguments(array)));
                    return true;
                }
                value = null;
                return false;
            }

            public bool TryGetCurrent(out JSValue value, out uint index)
            {
                if (array.elements.TryGetValue(Index, out var property))
                {
                    value = property.IsEmpty
                        ? JSUndefined.Value
                        : (property.IsValue
                            ? property.value
                            : property.get.InvokeFunction(new Arguments(array)));
                    index = this.index;
                    return true;
                }
                value = JSUndefined.Value;
                index = this.index;
                return false;
            }

        }

        internal void AddRange(JSValue iterator)
        {
            var et = this.elements;
            var el = this._length;
            if (iterator is JSArray ary)
            {
                var l = ary._length;
                var e = ary.elements;
                for (uint i = 0; i < l; i++)
                {
                    if(e.TryGetValue(i, out var v))
                    {
                        et[el++] = v;
                    }
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
}
