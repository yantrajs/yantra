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


        [Prototype("entries")]
        public static JSValue Entries(in Arguments a)
        {
            var array = a.This.AsTypedArray();
            return new JSGenerator(array.GetEntries(), "Array Iterator");
        }

        [Prototype("every", Length = 1)]
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

        [Prototype("fill", Length = 1)]
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

        [Prototype("filter", Length = 1)]
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


        [Prototype("find", Length = 1)]
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

        [Prototype("findIndex", Length = 1)]
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

        [Prototype("forEach", Length = 1)]
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

        [Prototype("includes", Length = 1)]
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
            var n = @this.Length;
            if (n == 0)
            {
                return JSNumber.MinusOne;
            }
            var startIndex = fromIndex.AsInt32OrDefault();
            if (startIndex >= n)
            {
                return JSNumber.MinusOne;
            }
            if (startIndex < 0)
            {
                startIndex = n + startIndex;
                if (startIndex < 0) {
                    startIndex = 0;
                }
            }
            var en = @this.GetElementEnumerator(startIndex);
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                if (searchElement.StrictEquals(item).BooleanValue)
                    return new JSNumber(index);
            }
            return JSNumber.MinusOne;
        }

        [Prototype("join", Length = 1)]
        public static JSValue Join(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var first = a.Get1();
            var sep = first.IsUndefined ? "," : first.ToString();
            var sb = new StringBuilder();
            bool isFirst = true;
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var item))
            {
                if (!isFirst)
                {
                    sb.Append(sep);
                }
                else
                {
                    isFirst = false;
                }
                if (item.IsUndefined)
                    continue;
                sb.Append(item.ToString());
            }
            return new JSString(sb.ToString());
        }

        [Prototype("keys", Length = 0)]
        public static JSValue Keys(in Arguments a) {
            var @this = a.This.AsTypedArray();
            return @this.GetKeys();
        }

        [Prototype("lastIndexOf", Length = 1)]
        public static JSValue LastIndexOf(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            var (element, fromIndex) = a.Get2();
            var n = @this.Length;
            if (n == 0)
            {
                return JSNumber.MinusOne;
            }
            var startIndex = fromIndex.AsInt32OrDefault(n - 1);
            if (startIndex >= n)
            {
                startIndex = n - 1;
            }
            if (startIndex < 0) {
                startIndex = n + startIndex;
            }

         
            var i = (uint)startIndex;
            
            while (i >= 0)
            {
                var item = @this[i];
                if (item.StrictEquals(element).BooleanValue)
                    return new JSNumber(i);
                if (i == 0)
                    break;
                i--;
            }
            return JSNumber.MinusOne;
        }



        [Prototype("map", Length = 1)]
        public static JSValue Map(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var (callback, thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var r = new JSArray();
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                {
                    r._length++;
                    continue;
                }
                var itemArgs = new Arguments(thisArg, item, new JSNumber(index), @this);
                r.elements[r._length++] = JSProperty.Property(fn.f(itemArgs));
            }
            return r;
        }

        [Prototype("reduce", Length = 1)]
        public static JSValue Reduce(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            var (callback, initialValue) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.reduce");
            var en = @this.GetElementEnumerator();
            uint index = 0;
            if (a.Length == 1)
            {
                if (!en.MoveNext(out initialValue))
                    throw JSContext.Current.NewTypeError($"No initial value provided and array is empty");
            }
            while (en.MoveNext(out var hasValue, out var item, out index))
            {
                if (!hasValue)
                    continue;
                var itemArgs = new Arguments(@this, initialValue, item, new JSNumber(index), @this);
                initialValue = fn.f(itemArgs);
            }
            return initialValue;
        }


        [Prototype("reduceRight", Length = 1)]
        public static JSValue ReduceRight(in Arguments a)
        {
            var r = new JSArray();
            var @this = a.This.AsTypedArray();
            var (callback, initialValue) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.reduce");
            var start = @this.Length - 1;
            if (a.Length == 1)
            {
                if (@this.Length == 0)
                    throw JSContext.Current.NewTypeError($"No initial value provided and array is empty");
                initialValue = @this[(uint)start];
                start--;
            }
            for (int i = start; i >= 0; i--)
            {
                var item = @this[(uint)i];
                var itemArgs = new Arguments(@this, initialValue, item, new JSNumber(i), @this);
                initialValue = fn.f(itemArgs);
            }
            return initialValue;
        }




    }
}
