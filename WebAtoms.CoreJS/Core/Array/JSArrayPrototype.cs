using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public class JSArrayPrototype
    {

        [Prototype("concat")]
        public static JSValue Concat(in Arguments a)
        {
            var r = new JSArray();
            var f = a.Get1();
            foreach (var e in a.This.AllElements)
            {
                r.elements[e.index] = JSProperty.Property(e.value);
            }
            r._length = (uint)a.This.Length;
            if (f is JSArray ary)
            {
                var start = r._length;
                foreach (var e in ary.GetArrayElements(false))
                {
                    var ei = start + e.index;
                    r.elements[ei] = JSProperty.Property(e.value);
                }
                r._length = (uint)a.This.Length + ary._length;
            }
            else
            {
                r.elements[r._length++] = JSProperty.Property(f);
            }
            return r;
        }

        [Prototype("every")]
        public static JSValue Every(in Arguments a)
        {
            var array = a.This;
            var first = a.Get1();
            if (!(first is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            foreach(var item in array.AllElements)
            {
                var itemArgs = new Arguments(a.This, item.value, new JSNumber(item.index), array);
                if (!fn.f(itemArgs).BooleanValue)
                    return JSBoolean.False;
            }
            return JSBoolean.True;
        }

        [Prototype("copyWithIn")]
        public static JSValue CopyWithIn(in Arguments a)
        {
            var array = a.This;
            if (array.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError(JSTypeError.Cannot_convert_undefined_or_null_to_object);
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

        [Prototype("filter")]
        public static JSValue Filter(in Arguments a)
        {
            var @this = a.This;
            var callback = a.Get1();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.filter");
            var r = new JSArray();
            foreach(var item in @this.AllElements)
            {
                var itemParams = new Arguments(@this, item.value, new JSNumber(item.index), @this);
                if (fn.f(itemParams).BooleanValue)
                {
                    r.Add(item.value);
                }
            }
            return r;
        }

        [Prototype("find")]
        public static JSValue Find(in Arguments a)
        {
            var @this = a.This;
            var callback = a.Get1();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            foreach (var item in @this.AllElements)
            {
                var itemParams = new Arguments(@this, item.value, new JSNumber(item.index), @this);
                if (fn.f(itemParams).BooleanValue)
                {
                    return item.value;
                }
            }
            return JSUndefined.Value;

        }


        [Prototype("findIndex")]
        public static JSValue FindIndex(in Arguments a)
        {
            var @this = a.This;
            var callback = a.Get1();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            foreach (var item in @this.AllElements)
            {
                var index = new JSNumber(item.index);
                var itemParams = new Arguments(@this, item.value, index, @this);
                if (fn.f(itemParams).BooleanValue)
                {
                    return index;
                }
            }
            return JSNumber.MinusOne;
        }

        [Prototype("forEach")]
        public static JSValue ForEach(in Arguments a)
        {
            var @this = a.This;
            var callback = a.Get1();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            foreach (var item in @this.AllElements)
            {
                var n = new JSNumber(item.index);
                var itemParams = new Arguments(@this, item.value, n, @this);
                fn.f(itemParams);
            }
            return JSUndefined.Value;
        }

        [Prototype("includes")]
        public static JSValue Includes(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            foreach (var item in @this.AllElements)
            {
                if (item.value.Equals(first).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Prototype("indexOf")]
        public static JSValue IndexOf(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            foreach (var item in @this.AllElements)
            {
                if (first.Equals(item.value).BooleanValue)
                    return new JSNumber(item.index);
            }
            return JSNumber.MinusOne;
        }

        [Prototype("join")]
        public static JSValue Join(in Arguments a)
        {
            var @this = a.This;
            var first = a.Get1();
            var sep = first.IsUndefined ? "," : first.ToString();
            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var item in @this.AllElements)
            {
                if(!isFirst)
                {
                    sb.Append(sep);
                }
                isFirst = false;
                sb.Append(item.value.ToString());
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

        [Prototype("lastIndexOf")]
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
                if (item.Equals(first).BooleanValue)
                    return new JSNumber(i);
                if (i == 0)
                    break;
                i--;
            }
            return JSNumber.MinusOne;
        }

        [Prototype("map")]
        public static JSValue Map(in Arguments a)
        {
            var @this = a.This;
            var callback = a.Get1();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.find");
            var r = new JSArray();
            foreach (var item in @this.AllElements)
            {
                var itemArgs = new Arguments(@this, item.value, new JSNumber(item.index), @this);
                r.Add(fn.f(itemArgs));
            }
            return r;
        }

        [Prototype("push")]
        public static JSValue Push(in Arguments a)
        {
            var t = a.This;
            if (t is JSArray ta)
            {
                var i = ta._length;
                var en = new Arguments.ArgumentsEnumerator(a);
                while(en.MoveNext())
                {
                    ta.elements[i++] = JSProperty.Property(en.Current);
                    
                }
                ta._length = i;
                return new JSNumber(ta._length);
            }
            
            uint ln = (uint)t.Length;
            var en1 = new Arguments.ArgumentsEnumerator(a);
            while(en1.MoveNext())
            {
                t[ln++] = en1.Current;
            }
            var n = new JSNumber(ln);
            t[KeyStrings.length] = n;
            return n;
        }

        [Prototype("pop")]
        public static JSValue Pop(in Arguments a)
        {
            var ta = a.This as JSArray;
            if (ta == null || ta._length == 0)
                return JSUndefined.Value;
            JSProperty r;
            if (ta.elements.TryRemove(ta._length - 1, out r))
            {
                ta._length--;
                return r.value;
            }
            return JSUndefined.Value;
        }

        [Prototype("reduce")]
        public static JSValue Reduce(in Arguments a)
        {
            var r = new JSArray();
            var @this = a.This;
            var (callback, initialValue) = a.Get2();
            if (!(callback is JSFunction fn))
                throw JSContext.Current.NewTypeError($"{callback} is not a function in Array.prototype.reduce");
            var en = @this.AllElements.GetEnumerator();
            if (a.Length == 1)
            {
                if (!en.MoveNext())
                    throw JSContext.Current.NewTypeError($"No initial value provided and array is empty");
                initialValue = en.Current.value;
            }
            while (en.MoveNext())
            {
                var item = en.Current.value;
                var itemArgs = new Arguments(@this, initialValue, item, new JSNumber(en.Current.index), @this);
                initialValue = fn.f(itemArgs);
            }
            return initialValue;
        }

        [Prototype("reduceRight")]
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
                var en = ary.GetArrayElements(false).GetEnumerator();
                var elements = ary.elements;
                if (en.MoveNext())
                {
                    var item = en.Current;
                    if(item.index > 0)
                    {
                        // shift...
                        elements[item.index - 1] = elements[item.index];
                        elements.RemoveAt(item.index);
                    } else
                    {
                        first = item.value;
                        elements.RemoveAt(0);
                    }
                }
                while (en.MoveNext())
                {
                    var item = en.Current;
                    elements[item.index - 1] = elements[item.index];
                    elements.RemoveAt(item.index);
                }
                ary._length -= 1;
                return first;
            }

            if (!(@this is JSObject @object))
                return first;

            var n = @this.Length;
            if (n == 0)
                return first;
            var oe = @object.elements;
            if (oe == null)
                return first;
            for(uint i = 1; i < n - 1; i++)
            {
                if (oe.TryRemove(i, out var p))
                    oe[i - 1] = p;
            }
            @this.Length = n - 1;
            return first;

        }

        [Prototype("slice")]
        public static JSArray Slice(in Arguments a)
        {
            var start = a.TryGetAt(0, out var a1) ? a1.IntValue : 0;
            var end = a.TryGetAt(1, out var a2) ? a2.IntValue : -1;
            JSArray r = new JSArray();
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
                for (uint i = (uint)start; i < l; i++)
                {
                    if (ary.elements.TryGetValue(i, out var p))
                    {
                        r.elements[ni++] = p;
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

        [Prototype("some")]
        public static JSValue Some(in Arguments a)
        {
            var array = a.This;
            var first = a.Get1();
            if (!(first is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            foreach (var item in array.AllElements)
            {
                var itemArgs = new Arguments(a.This, item.value, new JSNumber(item.index), array);
                if (fn.f(itemArgs).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Prototype("sort")]
        public static JSValue Sort(in Arguments a)
        {

            var fx = a.Get1();
            Comparison<JSValue> cx = null;
            if (fx is JSFunction fn)
            {
                var @this = a.This;
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
            foreach (var item in a.This.AllElements)
            {
                list.Add(item.value);
            }

            list.Sort(cx);

            return new JSArray(list);
        }

        [Prototype("splice")]
        public static JSValue Splice(in Arguments a)
        {
            var r = new JSArray();
            var (startP, deleteCountP) = a.Get2();

            var start = startP.IsUndefined ? 0 : startP.IntValue;
            var length = a.This.Length;

            var @this = a.This;

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
                deleteCount = length;
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
                @this.MoveElements(start - deleteCount, start);
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

        [GetProperty("length")]
        internal static JSValue GetLength(in Arguments a)
        {
            return new JSNumber((a.This as JSArray)._length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(in Arguments a)
        {
            return new JSNumber((a.This as JSArray)._length = (uint)a.Get1().IntValue);
        }

        [Prototype("toString")]
        internal static JSValue ToString(in Arguments args)
            => new JSString(
                args.This is JSArray a
                    ? a.ToString()
                    : "[object Object]");


        
    }
}
