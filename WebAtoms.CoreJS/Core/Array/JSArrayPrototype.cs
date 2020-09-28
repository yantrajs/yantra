using System;
using System.Drawing;
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

                foreach (var item in a.All)
                {
                    ta.elements[ta._length] = JSProperty.Property(item);
                    ta._length++;
                }
                return new JSNumber(ta._length);
            }
            var l = t[KeyStrings.length];
            uint ln = (uint)(l.IsNumber ? l.IntValue : 0);
            foreach (var item in a.All)
            {
                t[ln++] = item;
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

        [Prototype("reverse")]
        public static JSValue Reverse(in Arguments a)
        {
            var @this = a.This;
            var r = new JSArray();
            for (int i = @this.Length - 1 ; i >= 0; i--)
            {
                r.Add(@this[(uint)i]);
            }
            return JSUndefined.Value;

        }

        [Prototype("slice")]
        public static JSArray Slice(in Arguments a)
        {
            var ta = a.This as JSArray;
            var start = a.TryGetAt(0, out var a1) ? a1.IntValue : 0;
            var end = a.TryGetAt(0, out var a2) ? a2.IntValue : -1;
            return ta.Slice(start, end);
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
