using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using YantraJS;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public class JSArrayPrototype
    {

        [Constructor(Length = 1)]
        public static JSValue Constructor(in Arguments a)
        {
            // throw JSContext.Current.NewTypeError("Not supported");
            var @this = a.This;
            var arg = a.Get1();
            var result = new JSArray();
            if (a.Length == 0)
                return new JSArray();
            // If only length is specified
            if (a.Length == 1 && arg.IsNumber)
            {
                double val = arg.DoubleValue;
                if(double.IsNaN(val) || val < 0 || val > UInt32.MaxValue || Math.Floor(val) != val)
                    throw JSContext.Current.NewRangeError($"Invalid array length");
                return new JSArray(arg.IntValue);
            }
            // If elements are specified
            for (int i = 0; i < a.Length; i++)
            {
                var ele = a.GetAt(i);
                //if (ele == null)
                //    ele = JSUndefined.Value;
                result.Add(ele);
                
            }
            return result;

            
        }

        [Prototype("concat", Length = 1)]
        public static JSValue Concat(in Arguments a)
        {
            var r = new JSArray();
            r.AddRange(a.This);
            for (int i = 0; i < a.Length; i++)
            {
                var f = a.GetAt(i);
                if (f.IsArray)
                {
                    r.AddRange(f);
                }
                else
                {
                    r.Add(f);
                }
            }
            return r;
        }

        [Prototype("every", Length = 1)]
        public static JSValue Every(in Arguments a)
        {
            var array = a.This;
            var (first, thisArg) = a.Get2();
            if (!(first is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            var en = array.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
            {
                var itemArgs = new Arguments(thisArg, item, new JSNumber(index), array);
                if (!fn.f(itemArgs).BooleanValue)
                    return JSBoolean.False;
            }
            return JSBoolean.True;
        }

        [Prototype("copyWithIn", Length = 2)]
        public static JSValue CopyWithIn(in Arguments a)
        {
            var @this = a.This;
            if (@this.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
            var array = @this as JSObject;
            if (array.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot modify property length of {@this}");
            var len = array.Length;

            var (target, start, end) = a.Get3();

            var relativeTarget = target.IntValue;

            var to = relativeTarget < 0
                ? Math.Max(len + relativeTarget, 0)
                : Math.Min(relativeTarget, len);

            var relativeStart = start.IntValue;

            var from = relativeStart < 0
                ? Math.Max(len + relativeStart, 0)
                : Math.Min(relativeStart, len);

            var relativeEnd = end.IsUndefined ? len : end.IntValue;

            var final = relativeEnd < 0
                ? Math.Max(len + relativeEnd, 0)
                : Math.Min(relativeEnd, len);

            var count = Math.Min(final - from, len - to);


            throw new NotImplementedException();
        }

        [Prototype("filter", Length = 1)]
        public static JSValue Filter(in Arguments a)
        {
            var @this = a.This;
            var (callback, thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.filter");
            var r = new JSArray();
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
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
            var @this = a.This;
            var (callback, thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
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
        public static JSValue FindIndex(in Arguments a)
        {
            var @this = a.This;
            var (callback, thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var n))
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
        public static JSValue ForEach(in Arguments a)
        {
            var @this = a.This;
            var (callback,thisArg) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item , out var index))
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
        public static JSValue Includes(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (hasValue && item.Equals(first).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Prototype("indexOf", Length = 1)]
        public static JSValue IndexOf(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                if (first.StrictEquals(item).BooleanValue)
                    return new JSNumber(index);
            }
            return JSNumber.MinusOne;
        }

        [Prototype("join", Length = 1)]
        public static JSValue Join(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            var sep = first.IsUndefined ? "," : first.ToString();
            var sb = new StringBuilder();
            bool isFirst = true;
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var item))
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

        [Prototype("keys")]
        public static JSValue Keys(in Arguments a)
        {
            var @this = a.This;
            var r = new JSArray();
            for (int i = 0; i < @this.Length; i++)
            {
                r.Add(new JSNumber(i));
            }
            return r;

        }

        [Prototype("lastIndexOf", Length = 1)]
        public static JSValue LastIndexOf(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            var n = @this.Length;
            if (n == 0)
                return JSNumber.MinusOne;
            var i = (uint)(n - 1);
            while(i >= 0)
            {
                var item = @this[i];
                if (item.StrictEquals(first).BooleanValue)
                    return new JSNumber(i);
                if (i == 0)
                    break;
                i--;
            }
            return JSNumber.MinusOne;
        }

        [Prototype("map", Length = 1)]
        public static JSValue Map(in Arguments a)
        {
            var @this = a.This;
            var callback = a.Get1();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var r = new JSArray();
            ref var relements = ref r.GetElements();
            var en = @this.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                {
                    r._length++;
                    continue;
                }
                var itemArgs = new Arguments(@this, item, new JSNumber(index), @this);
                relements[r._length++] = JSProperty.Property(fn.f(itemArgs));
            }            
            return r;
        }

        [Prototype("push", Length = 1)]
        public static JSValue Push(in Arguments a)
        {
            var t = a.This as JSObject;
            if (t == null)
                return JSNumber.Zero;

            if (t.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot modify property length");

            int ai, al;
            if (t is JSArray ta)
            {

                var i = ta._length;
                al = a.Length;
                ref var taElements = ref ta.GetElements();
                for(ai = 0; ai < al; ai++)
                {
                    var item = a.GetAt(ai);
                    taElements[i++] = JSProperty.Property(item);
                    
                }
                ta._length = i;
                return new JSNumber(ta._length);
            }

            int ln1 = t.Length;
            uint ln = ln1 == -1 ? 0 : (uint)ln1;
            al = a.Length;
            for(ai = 0; ai <al; ai++)
            {
                t[ln++] = a.GetAt(ai);
            }
            var n = new JSNumber(ln);
            t[KeyStrings.length] = n;
            return n;
        }

        [Prototype("pop")]
        public static JSValue Pop(in Arguments a)
        {
            if (!(a.This is JSArray ta) || ta._length == 0)
                return JSUndefined.Value;
            if (ta.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError($"Cannot modify property length");
            ref var taElements = ref ta.GetElements();
            if (taElements.TryRemove(ta._length - 1, out JSProperty r))
            {
                ta._length--;
                return r.value;
            }
            return JSUndefined.Value;
        }

        [Prototype("reduce", Length = 1)]
        public static JSValue Reduce(in Arguments a)
        {
            var r = new JSArray();
            var @this = a.This;
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
            var @this = a.This;
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

        [Prototype("reverse")]
        public static JSValue Reverse(in Arguments a)
        {
            var @this = a.This;
            var r = new JSArray();
            for (int i = @this.Length - 1 ; i >= 0; i--)
            {
                r.Add(@this[(uint)i]);
            }
            return r;

        }

        [Prototype("shift")]
        public static JSValue Shift(in Arguments a)
        {
            var @this = a.This;
            JSValue first = JSUndefined.Value;

            if (@this is JSArray ary)
            {
                if (ary.IsSealedOrFrozen())
                    throw JSContext.Current.NewTypeError("Cannot modify property length");

                var en = ary.GetElementEnumerator();
                ref var elements = ref ary.GetElements();
                if (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    if(index > 0)
                    {
                        // shift...
                        elements[index - 1] = elements[index];
                        elements.RemoveAt(index);
                    } else
                    {
                        first = item;
                        elements.RemoveAt(0);
                    }
                }
                while (en.MoveNext(out hasValue, out item, out index))
                {
                    elements[index - 1] = elements[index];
                    elements.RemoveAt(index);
                }
                ary._length -= 1;
                return first;
            }

            if (!(@this is JSObject @object))
                return first;

            if (@object.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError("Cannot modify property length");

            var n = @this.Length;
            if (n == 0)
                return first;
            ref var oe = ref @object.GetElements();
            if (oe.IsNull)
                return first;
            for(uint i = 1; i < n - 1; i++)
            {
                if (oe.TryRemove(i, out var p))
                    oe[i - 1] = p;
            }
            @this.Length = n - 1;
            return first;

        }

        [Prototype("slice", Length = 2)]
        public static JSArray Slice(in Arguments a)
        {
            var start = a.TryGetAt(0, out var a1) ? a1.IntValue : 0;
            var end = a.TryGetAt(1, out var a2) ? a2.IntValue : -1;
            JSArray r = new JSArray();
            ref var rElements = ref r.CreateElements();
            uint l;
            uint ni;
            if (a.This is JSArray ary)
            {
                if (end >= 0)
                {
                    l = (uint)end >= ary._length ? ary._length - 1: (uint)end;
                }
                else 
                {
                    int n = ((int)ary._length) + end;
                    l = n >= 0 ? (uint)n + 1 : ary._length;
                }
                ni = 0;
                ref var aryElements = ref ary.GetElements();
                for (uint i = (uint)start; i < l; i++)
                {
                    if (aryElements.TryGetValue(i, out var p))
                    {
                        rElements[ni++] = p;
                    } else
                    {
                        ni++;
                    }
                }
                r._length = ni;
                return r;
            }
            var @object = a.This;
            // array like object..
            l = ((uint)@object.Length) >> 0;
            if (l == 0)
                return r;
            if (end >= 0)
            {
                l = (uint)end >= l ? l - 1 : (uint)end;
            }
            else
            {
                int n = ((int)l) + end;
                l = n >= 0 ? (uint)n + 1 : l;
            }
            ni = 0;
            for (uint i = (uint)start; i < l; i++)
            {
                r[ni++] = @object[i];
            }
            r._length = ni;

            return r;
        }

        [Prototype("some", Length = 1)]
        public static JSValue Some(in Arguments a)
        {
            var array = a.This;
            var first = a.Get1();
            if (!(first is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            var en = array.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (!hasValue)
                    continue;
                var itemArgs = new Arguments(a.This, item, new JSNumber(index), array);
                if (fn.f(itemArgs).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Prototype("sort", Length = 1)]
        public static JSValue Sort(in Arguments a)
        {

            var fx = a.Get1();
            var @this = a.This;
            Comparison<JSValue> cx = null;
            if (fx is JSFunction fn)
            {
                cx = (l, r) => {
                    var arg = new Arguments(@this, l, r);
                    return (int)(fn.f(arg).DoubleValue);
                };
            } else
            {
                if (!fx.IsUndefined)
                    throw JSContext.Current.NewTypeError($"Argument is not a function");

                // lets use map...
                //StringTrie<JSValue> trie = new StringTrie<JSValue>();
                //foreach(var item in a.This.AllElements)
                //{

                //}
                cx = (l, r) => (l.IsUndefined ? string.Empty : l.ToString())
                    .CompareTo(r.IsUndefined ? string.Empty : r.ToString());
            }

            var list = new List<JSValue>();
            var en = @this.GetElementEnumerator();
            while(en.MoveNext(out var hasValue, out var item, out var index))
            {
                if (hasValue)
                {
                    list.Add(item);
                }
            }

            list.Sort(cx);

            return new JSArray(list);
        }

        [Prototype("splice", Length = 2)]
        public static JSValue Splice(in Arguments a)
        {
            var r = new JSArray();
            var (startP, deleteCountP) = a.Get2();

            var start = startP.IsUndefined ? 0 : startP.IntValue;
            var length = a.This.Length;

            var @this = a.This as JSObject;
            if (@this == null)
                return r;

            if (@this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError("Cannot modify property length");


            if (start <0)
            {
                start = Math.Max(length + start, 0);
            }
            var deleteCount = 0;
            if (deleteCountP.IsUndefined)
            {
                // cut the array and return..
                if (start == 0)
                {
                    return r;
                }
                deleteCount = length - start;
            } else
            {
                deleteCount = deleteCountP.IntValue;
                if (deleteCount >= length - start) {
                    deleteCount = length - start;
                }
            }

            if (deleteCount > 0)
            {
                // copy items...
                var end = start + deleteCount;
                for (uint i = (uint)start, j = 0; i <end; i++, j++)
                {
                    if(@this.TryGetValue(i, out var p)) {
                        r[j] = p.value;
                    }

                }
                r._length = (uint)deleteCount;
                @this.MoveElements(end, start);
                // @this.Length -= deleteCount;
            }

            var insertLength = a.Length - 2;
            if (insertLength > 0)
            {
                // move items...
                @this.MoveElements(start, start + insertLength);
                for (int i = 2, j = start; i < a.Length; i++, j++)
                {
                    @this[(uint)j] = a.GetAt(i);
                }
            }

            return r;

        }

        [Prototype("unshift", Length = 1)]
        public static JSValue Unshift(in Arguments a)
        {
            var @this = a.This as JSObject;
            if (@this == null)
                return JSUndefined.Value;

            if (@this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError("Cannot modify property length");

            var l = a.This.Length;
            if (l > 0)
            {
                // move.. 
                @this.MoveElements(0, l - 1);

                for (uint i = 0; i < a.Length; i++)
                {
                    @this[i] = a.GetAt((int)i);
                }
            }
            return new JSNumber(a.This.Length);
        }

        [GetProperty("length")]
        internal static JSValue GetLength(in Arguments a)
        {
            return new JSNumber((a.This as JSArray)._length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(in Arguments a)
        {
            var @this = a.This as JSArray;
            if (@this.IsSealedOrFrozen())
                throw JSContext.Current.NewTypeError("Cannot modify property length");

            return new JSNumber(@this._length = (uint)a.Get1().IntValue);
        }

        [Prototype("toString")]
        internal static JSValue ToString(in Arguments args)
            => new JSString(
                args.This is JSArray a
                    ? a.ToString()
                    : "[object Object]");


        
    }
}
