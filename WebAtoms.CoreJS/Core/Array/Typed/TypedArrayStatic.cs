using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Typed;

namespace WebAtoms.CoreJS.Core.Typed
{
    internal class Int8Array : TypedArray
    {
        public Int8Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length) 
            : base(buffer, type, byteOffset, length, JSContext.Current.Int8ArrayPrototype)
        {
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a) {

            return TypedArrayStatic.From(JSContext.Current.Int8ArrayPrototype, TypedArrayType.Int8Array, a);
        }
    }

    internal static class TypedArrayStatic
    {
        // [Static("from", Length = 1)]
        public static JSValue From(JSObject prototype, TypedArrayType type, in Arguments a) {
            var (f, map, mapThis) = a.Get3();
            var t = a.This;

            int length = -1;

            switch (f) {
                case JSArray array:
                    length = array.Length;
                    break;
                case JSString @string:
                    length = @string.value.Length;
                    break;
            }

            IElementEnumerator en2 = null;
            /*
             * If length is unknown, create a List and get its count
             * 
             */
            if (length == -1)
            {
                var en = f.GetElementEnumerator();
                var elements = new List<JSValue>();
                while (en.MoveNext(out var hasValue, out var item, out var index))
                {
                    elements.Add(item);
                }
                length = elements.Count;
                en2 = new ListElementEnumerator(elements.GetEnumerator());
            }
            else {
                en2 = f.GetElementEnumerator();
            }

            var typedArray = new TypedArray(null, type, 0, length, prototype);
            uint i = 0;
            if (map is JSFunction fx2)
            {
                var cb = fx2.f;
                while (en2.MoveNext(out var hasValue, out var item, out var index))
                {
                    typedArray[i++] = cb(new Arguments(mapThis, item));
                }
            }
            else
            {
                while (en2.MoveNext(out var hasValue, out var item, out var index))
                {
                    typedArray[i++] = item;
                }
            }
            return typedArray;




        }
    }

}
