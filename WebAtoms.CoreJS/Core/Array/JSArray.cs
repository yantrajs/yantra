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

        internal override IEnumerable<(uint index, JSValue value)> AllElements  => this.GetArrayElements(false);

        internal override bool TryRemove(uint i, out JSProperty p)
        {
            return elements.TryRemove(i, out p);
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        internal override IElementEnumerator GetElementEnumeratorWithoutHoles()
        {
            return new ElementEnumeratorWithoutHoles(this);
        }

        private struct ElementEnumeratorWithoutHoles : IElementEnumerator
        {
            uint length;
            uint index;
            JSProperty property;
            JSArray array;
            public ElementEnumeratorWithoutHoles(JSArray array)
            {
                this.length = array._length;
                this.array = array;
                index = uint.MaxValue;
                property = new JSProperty();
            }

            public JSValue Current => property.IsEmpty 
                ? JSUndefined.Value 
                : (property.IsValue 
                    ? property.value
                    : array.GetValue(property));

            public uint Index => index;

            public bool MoveNext()
            {
                while ((index = (index == uint.MaxValue) ? 0 : (index + 1)) < length)
                {
                    if(array.elements.TryGetValue(Index, out property))
                    {
                        return true;
                    }
                }
                return false;
            }

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

            public bool MoveNext()
            {
                return (index = (index == uint.MaxValue) ? 0 : (index + 1)) < length;
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
            while (en.MoveNext())
            {
                et[el++] = JSProperty.Property(en.Current);
            }
            this._length = el;
        }
    }
}
