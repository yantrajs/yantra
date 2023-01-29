using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Generator;
using YantraJS.Extensions;

namespace YantraJS
{

    public struct ClrEnumerableElementEnumerator : IElementEnumerator
    {
        private IEnumerator<JSValue> en;

        public ClrEnumerableElementEnumerator(in IEnumerable<JSValue> en)
        {
            this.en = en.GetEnumerator();
        }
        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (en.MoveNext())
            {
                hasValue = true;
                index = 0;
                value = en.Current;
                return true;
            }
            hasValue = false;
            index = 0;
            value = null;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            if (en.MoveNext())
            {
                value = en.Current;
                return true;
            }
            value = null;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (en.MoveNext())
            {
                value = en.Current;
                return true;
            }
            value = @default;
            return false;
        }
        public JSValue NextOrDefault(JSValue @default)
        {
            if (en.MoveNext())
            {
                return en.Current;
            }
            return @default;
        }

    }


    public readonly struct ListElementEnumerator : IElementEnumerator
    {
        private readonly List<JSValue>.Enumerator en;

        public ListElementEnumerator(List<JSValue>.Enumerator en)
        {
            this.en = en;
        }
        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (en.MoveNext()) {
                hasValue = true;
                index = 0;
                value = en.Current;
                return true;
            }
            hasValue = false;
            index = 0;
            value = null;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            if (en.MoveNext())
            {
                value = en.Current;
                return true;
            }
            value = null;
            return false;
        }


        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (en.MoveNext())
            {
                value = en.Current;
                return true;
            }
            value = @default;
            return false;
        }
        public JSValue NextOrDefault(JSValue @default)
        {
            if (en.MoveNext())
            {
                return en.Current;
            }
            return @default;
        }

    }

    public struct ClrObjectEnumerator<T> : IEnumerator<T>
    {
        private IElementEnumerator en;

        public ClrObjectEnumerator(IElementEnumerator en)
        {
            this.en = en;
            Current = default;
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            if (en.MoveNext(out var c)) { 
                if(c.ConvertTo(typeof(T), out var v))
                {
                    Current = (T)v;
                }
                throw JSContext.Current.NewTypeError($"Failed to convert {c} to type {typeof(T).Name}");
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public readonly struct ClrObjectEnumerable<T> : IEnumerable<T>
    {

        readonly JSValue value;
        public ClrObjectEnumerable(JSValue value)
        {
            this.value = value;
        }



        public IEnumerator<T> GetEnumerator()
        {
            return new ClrObjectEnumerator<T>(this.value.GetElementEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct EnumerableElementEnumerable : IElementEnumerator
    {

        private readonly IEnumerator en;
        uint index;
        public EnumerableElementEnumerable(IEnumerator en)
        {
            this.en = en;
            index = uint.MaxValue;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (en.MoveNext())
            {
                value = ClrProxy.Marshal(en.Current);
                this.index = this.index == uint.MaxValue ? 0 : this.index + 1;
                index = this.index;
                hasValue = true;
                return true;
            }
            value = JSUndefined.Value;
            index = this.index;
            hasValue = false;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            if (en.MoveNext())
            {
                value = ClrProxy.Marshal(en.Current);
                this.index = this.index == uint.MaxValue ? 0 : this.index + 1;
                return true;
            }
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (en.MoveNext())
            {
                value = ClrProxy.Marshal(en.Current);
                this.index = this.index == uint.MaxValue ? 0 : this.index + 1;
                return true;
            }
            value = @default;
            return false;
        }

        public JSValue NextOrDefault(JSValue @default)
        {
            if (en.MoveNext())
            {
                this.index = this.index == uint.MaxValue ? 0 : this.index + 1;
                return ClrProxy.Marshal(en.Current);
            }
            return @default;
        }

    }

    /// <summary>
    /// Struct implementing interface is marginally faster than ElementEnumerator being a class.
    /// https://dotnetfiddle.net/EbegMo
    /// </summary>
    public interface IElementEnumerator
    {
        // bool MoveNext();

        bool MoveNext(out bool hasValue, out JSValue value, out uint index);

        
        bool MoveNext(out JSValue value);

        bool MoveNextOrDefault(out JSValue value, JSValue @default);

        JSValue NextOrDefault(JSValue @default);

        // JSValue Current { get; }

        ///// <summary>
        ///// Returns false if it has a hole (only applicable for an Array)
        ///// </summary>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //bool TryGetCurrent(out JSValue value);

        //bool TryGetCurrent(out JSValue value, out uint index);

        // uint Index { get; }

    }

    //public ref struct OwnElementEnumeratorWithoutHoles
    //{
    //    JSObject value;
    //    int length;
    //    IEnumerator<(uint Key, JSProperty Value)> en;
    //    UInt32Trie<JSProperty> elements;

    //    public int CurrentIndex;
    //    public JSValue Current => CurrentProperty.IsValue 
    //        ? CurrentProperty.value 
    //        : CurrentProperty.get.InvokeFunction(new Arguments(this.value));
    //    public JSProperty CurrentProperty;

    //    public OwnElementEnumeratorWithoutHoles(JSValue value)
    //    {
    //        this.value = value as JSObject;
    //        this.elements = this.value.elements;
    //        CurrentIndex = -1;
    //        CurrentProperty = new JSProperty();
    //        if (value is JSArray a)
    //        {
    //            this.en = null;
    //            this.length = (int)a._length;
    //        } else
    //        {
    //            this.en = this.elements?.AllValues?.GetEnumerator();
    //            this.length = -1;
    //        }
    //    }

    //    public bool MoveNext()
    //    {
    //        if(en != null)
    //        {
    //            if (en.MoveNext())
    //            {
    //                var c = en.Current;
    //                this.CurrentIndex = (int)c.Key;
    //                this.CurrentProperty = c.Value;
    //                return true;
    //            }
    //            return false;
    //        }
    //        CurrentIndex++;
    //        while(CurrentIndex < length)
    //        {
    //            if(this.elements.TryGetValue((uint)CurrentIndex, out CurrentProperty)){
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}

    //public ref struct OwnEntriesEnumerator
    //{

    //    JSObject value;
    //    IEnumerator<(uint Key, JSProperty Value)> elements;
    //    private PropertySequence.Enumerator properties;

    //    public bool IsUint;

    //    public uint Index;

    //    public JSValue Current;

    //    public JSProperty CurrentProperty;


    //    public OwnEntriesEnumerator(JSValue value)
    //    {
    //        this.value = value as JSObject;
    //        IsUint = false;
    //        Index = 0;
    //        Current = null;
    //        CurrentProperty = new JSProperty();
    //        if (this.value == null)
    //        {
    //            this.elements = null;
    //            this.properties = new PropertySequence.Enumerator();
    //        }
    //        else
    //        {
    //            this.elements = this.value.elements?.AllValues?.GetEnumerator();
    //            if (this.value.ownProperties != null)
    //            {
    //                this.properties = new PropertySequence.Enumerator(this.value.ownProperties);
    //            } else
    //            {
    //                this.properties = new PropertySequence.Enumerator();
    //            }
                
    //        }
    //    }

    //    public bool MoveNext()
    //    {
    //        if(elements != null)
    //        {
    //            if (elements.MoveNext())
    //            {
    //                var c = elements.Current;
    //                IsUint = true;
    //                Index = c.Key;
    //                CurrentProperty = c.Value;
    //                this.Current = c.Value.value;
    //                return true;
    //            }
    //        }
    //        if (properties.MoveNext())
    //        {
    //            IsUint = false;
    //            Index = 0;
    //            CurrentProperty = properties.Current;
    //            Current = this.value.GetValue(CurrentProperty);
    //            return true;
    //        }
    //        return false;
    //    }


    //}
}
