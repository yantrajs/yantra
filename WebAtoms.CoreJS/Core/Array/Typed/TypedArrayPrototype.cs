using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Generator;

namespace WebAtoms.CoreJS.Core.Typed
{
    public static class TypedArrayPrototype
    {
        [Prototype("toString")]
        public static JSValue ToString(in Arguments a) {
            var @this = a.This.AsTypedArray();
            return new JSString(@this.ToString());
        }

        [Prototype("copyWithin", Length = 2)]
        public static JSValue CopyWithin(in Arguments a) {
            var(target, start, end) = a.Get3();
            throw new NotImplementedException();
        }


        [Prototype("entries", Length = 0)]
        public static JSValue Entries(in Arguments a)
        {
            var array = a.This.AsTypedArray();
            return new JSGenerator(array.GetEntries(), "Array Iterator");
        }

        [Prototype("every", Length = 0)]
        public static JSValue Every(in Arguments a) {

            var array = a.This.AsTypedArray();
            var (first, thisArg) = a.Get2();
            if (!(first is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            var en = array.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                var itemArgs = new Arguments(thisArg, item, new JSNumber(index), array);
                if (!fn.f(itemArgs).BooleanValue)
                    return JSBoolean.False;
            }
            return JSBoolean.True;
        }

        [Prototype("fill", Length = 0)]
        public static JSValue Fill(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            var (value,start,end) = a.Get3();
           // JSArray r = new JSArray();
            var len = @this.Length;
            var relativeStart = start.AsInt32OrDefault();
            var relativeEnd = end.AsInt32OrDefault(len);
            // Negative values represent offsets from the end of the array.
            relativeStart = relativeStart < 0 ? Math.Max(len + relativeStart, 0) : Math.Min(relativeStart, len);
            relativeEnd = relativeEnd < 0 ? Math.Max(len + relativeEnd, 0) : Math.Min(relativeEnd, len);
            for (; relativeStart < relativeEnd; relativeStart++)
            {
                @this[(uint)relativeStart] = value;
            }
            return @this;
        }

        [Prototype("filter", Length = 0)]
        public static JSValue Filter(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            var (callback,thisArg) = a.Get2();

            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.filter");
            var r = new JSArray();
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue) continue;
                var itemParams = new Arguments(thisArg, item, new JSNumber(index), @this);
                if (fn.f(itemParams).BooleanValue)
                {
                    r.Add(item);
                }
            }
            return r;
        }


        [Prototype("find", Length = 0)]
        public static JSValue Find(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            var (callback, thisArg) = a.Get2();

            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.filter");

            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                // ignore holes...
                if (!hasValue)
                    continue;
                var itemParams = new Arguments(thisArg, item, new JSNumber(index), @this);
                if (fn.f(itemParams).BooleanValue)
                {
                    return item;
                }
            }
            return JSUndefined.Value;
        }

        [Prototype("findIndex", Length = 0)]
        public static JSValue FindIndex(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var (callback, thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var n))
            {
                // ignore holes...
                if (!hasValue)
                    continue;
                var index = new JSNumber(n);
                var itemParams = new Arguments(thisArg, item, index, @this);
                if (fn.f(itemParams).BooleanValue)
                {
                    return index;
                }
            }
            return JSNumber.MinusOne;

        }

        [Prototype("forEach", Length = 0)]
        public static JSValue ForEach(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var (callback, thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                // ignore holes...
                if (!hasValue)
                    continue;
                var n = new JSNumber(index);
                var itemParams = new Arguments(thisArg, item, n, @this);
                fn.f(itemParams);
            }
            return JSUndefined.Value;
        }

        [Prototype("includes", Length = 0)]
        public static JSValue Includes(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var (searchElement, fromIndex) = a.Get2();
            var startIndex = fromIndex.AsInt32OrDefault();
            if (startIndex < 0) {
                startIndex = 0;
            }
            var en = @this.GetElementEnumerator(startIndex);
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (hasValue && item.Equals(searchElement).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Prototype("indexOf", Length = 1)]
        public static JSValue IndexOf(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var (searchElement, fromIndex) = a.Get2();
            var startIndex = fromIndex.AsInt32OrDefault();
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            var en = @this.GetElementEnumerator(startIndex);
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                if (searchElement.Equals(item).BooleanValue)
                    return new JSNumber(index);
            }
            return JSNumber.MinusOne;
        }

    }
}
