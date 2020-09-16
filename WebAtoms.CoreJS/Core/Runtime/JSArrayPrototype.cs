using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public class JSArrayPrototype
    {

        [Prototype("concat")]
        public static JSValue Concat(JSValue t, params JSValue[] a)
        {
            var r = new JSArray();
            var f = a.GetAt(0);
            foreach (var e in t.AllElements)
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

        [Prototype("push")]
        public static JSValue Push(JSValue t, params JSValue[] a)
        {

            if (t.IsArray)
            {

                var ta = (JSArray)t;
                foreach (var item in a)
                {
                    ta.elements[ta._length] = JSProperty.Property(item);
                    ta._length++;
                }
                return new JSNumber(ta._length);
            }
            var l = t[KeyStrings.length];
            uint ln = (uint)(l.IsNumber ? l.IntValue : 0);
            foreach (var item in a)
            {
                t[ln++] = item;
            }
            var n = new JSNumber(ln);
            t[KeyStrings.length] = n;
            return n;
        }

        [Prototype("pop")]
        public static JSValue Pop(JSValue t, params JSValue[] a)
        {
            var ta = (JSArray)t;
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
        public static JSArray Slice(JSValue t, params JSValue[] a)
        {
            var ta = (JSArray)t;
            var start = a.TryGetAt(0, out var a0) ? a0.IntValue : 0;
            var end = a.TryGetAt(1, out var a1) ? a1.IntValue : -1;
            return ta.Slice(start, end);
        }

        [GetProperty("length")]
        internal static JSValue GetLength(JSValue t, params JSValue[] a)
        {
            return new JSNumber(((JSArray)t)._length);
        }

        [SetProperty("length")]
        internal static JSValue SetLength(JSValue t, params JSValue[] a)
        {
            return new JSNumber(((JSArray)t)._length = (uint)a[0].IntValue);
        }

        [Prototype("toString")]
        internal new static JSValue ToString(JSValue t, params JSValue[] _)
            => new JSString(
                t is JSArray a
                    ? string.Join(",", a.All)
                    : "[object Object]");


        
    }
}
