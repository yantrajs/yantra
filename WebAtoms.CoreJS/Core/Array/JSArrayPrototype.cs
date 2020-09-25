using System.Drawing;
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
                r.elements[r._length++] = JSProperty.Property(e);
            }
            if (f.IsArray)
            {
                foreach (var e in f.AllElements)
                {
                    r.elements[r._length++] = JSProperty.Property(e);
                }
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
            int i = 0;
            foreach(var item in array.AllElements)
            {
                var itemArgs = new Arguments(a.This, item, new JSNumber(i++), array);
                if (!fn.f(itemArgs).BooleanValue)
                    return JSBoolean.False;
            }
            return JSBoolean.True;
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
            if (ta._length == 0)
                return JSUndefined.Value;
            JSProperty r;
            if (ta.elements.TryRemove(ta._length - 1, out r))
            {
                ta._length--;
                return r.value;
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
                    ? string.Join(",", a.All)
                    : "[object Object]");


        
    }
}
