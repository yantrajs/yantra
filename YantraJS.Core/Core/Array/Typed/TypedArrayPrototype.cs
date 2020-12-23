using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using YantraJS.Core.Generator;

namespace YantraJS.Core.Typed
{
    public static class TypedArrayPrototype
    {
        [GetProperty("length")]
        public static JSValue Length(in Arguments a) {
            var @this = a.This.AsTypedArray();
            return new JSNumber(@this.Length);
        }

        [SetProperty("length")]
        public static JSValue SetLength(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            throw JSContext.Current.NewTypeError($"Cannot set property length of {@this} which has only a getter");
        }

        [GetProperty("byteOffset")]
        public static JSValue ByteOffset(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            return new JSNumber(@this.byteOffset);
        }

        [GetProperty("byteLength")]
        public static JSValue ByteLength(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            return new JSNumber(@this.buffer.buffer.Length);
        }

        [GetProperty("buffer")]
        public static JSValue Buffer(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            return @this.buffer;
        }


        [Prototype("toString")]
        public static JSValue ToString(in Arguments a) {
            var @this = a.This.AsTypedArray();
            return new JSString(@this.ToString());
        }


        /// <summary>
        /// Copies the sequence of array elements within the array to the position starting at
        /// target. The copy is taken from the index positions of the second and third arguments
        /// start and end. The end argument is optional and defaults to the length of the array.
        /// This method has the same algorithm as Array.prototype.copyWithin.
        /// </summary>
        /// <param name="target"> Target start index position where to copy the elements to. </param>
        /// <param name="start"> Source start index position where to start copying elements from. </param>
        /// <param name="end"> Optional. Source end index position where to end copying elements from. </param>
        /// <returns> The array that is being operated on. </returns>
        [Prototype("copyWithin", Length = 2)]
        public static JSValue CopyWithin(in Arguments a) {
            var(t, s) = a.Get2();
            var target = t.IntValue;
            var start = s.IntValue;
            var end = a.TryGetAt(2, out var e) ? e.IntValue : int.MaxValue;
            var @this = a.This.AsTypedArray();
            // Negative values represent offsets from the end of the array.
            target = target < 0 ? Math.Max(@this.Length + target, 0) : Math.Min(target, @this.Length);
            start = start < 0 ? Math.Max(@this.Length + start, 0) : Math.Min(start, @this.Length);
            end = end < 0 ? Math.Max(@this.Length + end, 0) : Math.Min(end, @this.Length);

            // Calculate the number of values to copy.
            int count = Math.Min(end - start, @this.Length - target);

            // Check if we need to copy in reverse due to an overlap.
            int direction = 1;
            if (start < target && target < start + count)
            {
                direction = -1;
                start += count - 1;
                target += count - 1;
            }

            while (count > 0)
            {
                // Get the value of the array element.
                var elementValue = @this[(uint)start];

                // Copy the value to the new position.
                @this[(uint)target] = elementValue;

                // Progress to the next element.
                start += direction;
                target += direction;
                count--;
            }

            return @this;
            //throw new NotImplementedException();
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
           
            var startIndex = a.Length == 2 ? fromIndex.IntValue : int.MaxValue;
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
            ref var rElements = ref r.CreateElements();
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                {
                    r._length++;
                    continue;
                }
                var itemArgs = new Arguments(thisArg, item, new JSNumber(index), @this);
                rElements[r._length++] = JSProperty.Property(fn.f(itemArgs));
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
                var itemArgs = new Arguments(JSUndefined.Value, initialValue, item, new JSNumber(index), @this);
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
                var itemArgs = new Arguments(JSUndefined.Value, initialValue, item, new JSNumber(i), @this);
                initialValue = fn.f(itemArgs);
            }
            return initialValue;
        }


        [Prototype("reverse", Length = 0)]
        public static JSValue Reverse(in Arguments a) {
            var @this = a.This.AsTypedArray();

            var src = @this.buffer.buffer;
            var temp = new byte[src.Length];
            Array.Copy(src, temp, src.Length);
            int bytesPerElement = @this.bytesPerElement;
            int length = @this.Length;
            for (int i = 0; i < length; i++)
            {
                var y = length - i - 1;
                Array.Copy(temp, @this.byteOffset + (i * bytesPerElement), 
                    src, 
                    @this.byteOffset + (y * bytesPerElement), 
                    bytesPerElement);
            }
            // Array.Copy(temp, src,src.Length);
            return @this;
        }

        [Prototype("set", Length = 1)]
        public static JSValue Set(in Arguments a) {
            var @this = a.This.AsTypedArray();
            var (source, offset) = a.Get2();
            int length = @this.Length;
            if (length == 0)
            {
                return JSNumber.MinusOne;
            }
            
            var relativeStart = offset.AsInt32OrDefault();
            if (relativeStart < 0)
                throw JSContext.Current.NewRangeError("Offset is out of bounds");
            var targetArrayLength = source.Length + relativeStart;
            if (targetArrayLength > length)
                throw JSContext.Current.NewRangeError("Offset is out of bounds");
            if (source is TypedArray typedArray) {
                var src = typedArray.buffer.buffer;
                var target = @this.buffer.buffer;
                int sourceBytesPerElement = typedArray.bytesPerElement;
                int targetBytesPerElement = @this.bytesPerElement;
                // var maxLength = source.Length - (relativeStart * sourceBytesPerElement);
                if (src == target && (relativeStart * targetBytesPerElement) >= typedArray.byteOffset) { 
                    for (int i = source.Length - 1; i >= 0 ; i--)
                    {
                        var y = relativeStart + i;
                        Array.Copy(src, typedArray.byteOffset + (i * sourceBytesPerElement), 
                            target, 
                            @this.byteOffset + (y * targetBytesPerElement), 
                            sourceBytesPerElement);
                    }
                }else { 
                for (int i = 0; i < source.Length; i++)
                {
                    var y = relativeStart + i;
                    Array.Copy(src, typedArray.byteOffset + (i * sourceBytesPerElement),
                        target,
                        @this.byteOffset + (y * targetBytesPerElement),
                        sourceBytesPerElement);
                }
                }

                return @this;
            }

            var rs = (uint)relativeStart;
            var en = source.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var value, out var index)) {
                @this[index + rs] = value;
            }

            return @this;
         
        }

        [Prototype("slice", Length = 2)]
        public static JSValue Slice(in Arguments a) {
            var begin = a.TryGetAt(0, out var a1) ? a1.IntValue : 0;
            var end = a.TryGetAt(1, out var a2) ? a2.IntValue : int.MaxValue;
            
            int newLength;
            var @this = a.This.AsTypedArray();

            begin = begin < 0 ? Math.Max(@this.Length + begin, 0) : Math.Min(begin, @this.Length);
            end = end < 0 ? Math.Max(@this.Length + end, 0) : Math.Min(end, @this.Length);
            newLength = Math.Max(end - begin, 0);


            var src = @this.buffer.buffer;
            var r = new TypedArray(null, @this.type, 0, newLength, @this.prototypeChain);
            var target = r.buffer.buffer;
            int bytesPerElement = @this.bytesPerElement;

            for (int i = begin; i < end; i++)
            {
                var y = i - begin;
                Array.Copy(src,@this.byteOffset + (i * bytesPerElement), target, y * bytesPerElement, bytesPerElement);
            }

            
            return r;
            
        }

        [Prototype("some", Length = 1)]
        public static JSValue Some(in Arguments a) {
            var array = a.This.AsTypedArray();
            var (callback,thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            var en = array.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                var itemArgs = new Arguments(thisArg, item, new JSNumber(index), array);
                if (fn.f(itemArgs).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }


        [Prototype("sort", Length = 1)]
        public static JSValue Sort(in Arguments a) {
            var fx = a.Get1();
            var @this = a.This.AsTypedArray();
            Comparison<JSValue> cx = null;
            if (fx is JSFunction fn)
            {
                cx = (l, r) => {
                    var arg = new Arguments(@this, l, r);
                    return (int)(fn.f(arg).DoubleValue);
                };
            }
            else
            {
                if (!fx.IsUndefined)
                    throw JSContext.Current.NewTypeError($"Argument is not a function");
                cx = (l, r) =>
                {
                    var x = l.DoubleValue;
                    var y = r.DoubleValue;
                    if (x < y)
                        return -1;
                    if (x > y)
                        return 1;
                    if (double.IsNaN(x) && double.IsNaN(y))
                        return 0;
                    if (double.IsNaN(x))
                        return 1;
                    if (double.IsNaN(y))
                        return -1;
                    if (JSNumber.IsNegativeZero(x) && JSNumber.IsPositiveZero(y))
                        return -1;
                    if (JSNumber.IsPositiveZero(x) && JSNumber.IsNegativeZero(y))
                        return 1;
                    return 0;



                };
            }

            var list = new List<JSValue>();
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (hasValue)
                {
                    list.Add(item);
                }
            }

            list.Sort(cx);

            return new JSArray(list);
        }



        [Prototype("subarray", Length = 2)]
        public static JSValue SubArray(in Arguments a) {
            var begin = a.TryGetAt(0, out var a1) ? a1.IntValue : 0;
            var end = a.TryGetAt(1, out var a2) ? a2.IntValue : int.MaxValue;

            int newLength;
            var @this = a.This.AsTypedArray();

            begin = begin < 0 ? Math.Max(@this.Length + begin, 0) : Math.Min(begin, @this.Length);
            end = end < 0 ? Math.Max(@this.Length + end, 0) : Math.Min(end, @this.Length);
            newLength = Math.Max(end - begin, 0);

            var r = new TypedArray(@this.buffer, 
                @this.type, 
                @this.byteOffset + begin * @this.bytesPerElement, 
                newLength * @this.bytesPerElement, 
                @this.prototypeChain);
            return r;

        }

        [Prototype("values", Length = 2)]
        public static JSValue Values(in Arguments a) {
            var array = a.This.AsTypedArray();
            return new JSGenerator(array.GetElementEnumerator(), "Array Iterator");
        }


        [Prototype("toLocaleString", Length = 0)]
        internal static JSValue ToLocaleString(in Arguments a)
        {
            var @this = a.This.AsTypedArray();
            var (locale, format) = a.Get2();
            StringBuilder sb = new StringBuilder();

            var def = "N0";
            switch (@this.type)
            {
                case TypedArrayType.Float32Array:
                case TypedArrayType.Float64Array:
                    def = "N";
                    break;
            }

            string strFormat = format.IsNullOrUndefined ? def : (format.IsString ? format.ToString() :
                throw JSContext.Current.NewTypeError("Options not supported, use .Net String Formats")
                );

            CultureInfo culture = locale.IsNullOrUndefined ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(locale.ToString());


            bool first = true;
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var n)) {
                if (!first)
                {
                    sb.Append(',');
                }
                first = false;
                sb.Append(n.ToLocaleString(strFormat, culture));
            }
            return new JSString(sb.ToString());
            

        }
    }
}
