using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Typed
{
    [JSRuntime(typeof(TypedArrayStatic),typeof(TypedArrayPrototype))]
    internal class Int8Array : TypedArray
    {
        public Int8Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length) 
            : base(buffer, type, byteOffset, length, JSContext.Current.Int8ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Int8ArrayPrototype, TypedArrayType.Int8Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a) {

            return TypedArrayStatic.From(JSContext.Current.Int8ArrayPrototype, TypedArrayType.Int8Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Int8ArrayPrototype, TypedArrayType.Int8Array, a);
        }

    }



    internal class Uint8Array : TypedArray
    {
        public Uint8Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Uint8ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Uint8ArrayPrototype, TypedArrayType.Uint8Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Uint8ArrayPrototype, TypedArrayType.Uint8Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Uint8ArrayPrototype, TypedArrayType.Uint8Array, a);
        }

    }


    internal class Int16Array : TypedArray
    {
        public Int16Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Int16ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Int16ArrayPrototype, TypedArrayType.Int16Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Int16ArrayPrototype, TypedArrayType.Int16Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Int16ArrayPrototype, TypedArrayType.Int16Array, a);
        }

    }

    internal class Uint16Array : TypedArray
    {
        public Uint16Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Uint16ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Uint16ArrayPrototype, TypedArrayType.Uint16Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Uint16ArrayPrototype, TypedArrayType.Uint16Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Uint16ArrayPrototype, TypedArrayType.Uint16Array, a);
        }

    }

    internal class Int32Array : TypedArray
    {
        public Int32Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Int32ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Int32ArrayPrototype, TypedArrayType.Int32Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Int32ArrayPrototype, TypedArrayType.Int32Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Int32ArrayPrototype, TypedArrayType.Int32Array, a);
        }

    }

    internal class Uint32Array : TypedArray
    {
        public Uint32Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Uint32ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Uint32ArrayPrototype, TypedArrayType.Uint32Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Uint32ArrayPrototype, TypedArrayType.Uint32Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Uint32ArrayPrototype, TypedArrayType.Uint32Array, a);
        }

    }

    internal class Float32Array : TypedArray
    {
        public Float32Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Float32ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Float32ArrayPrototype, TypedArrayType.Float32Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Float32ArrayPrototype, TypedArrayType.Float32Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Float32ArrayPrototype, TypedArrayType.Float32Array, a);
        }

    }

    internal class Float64Array : TypedArray
    {
        public Float64Array(JSArrayBuffer buffer, TypedArrayType type, int byteOffset, int length)
            : base(buffer, type, byteOffset, length, JSContext.Current.Float64ArrayPrototype)
        {
        }

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {

            return TypedArrayStatic.Constructor(JSContext.Current.Float64ArrayPrototype, TypedArrayType.Float64Array, a);
        }

        [Static("from", Length = 1)]
        public static JSValue From(in Arguments a)
        {

            return TypedArrayStatic.From(JSContext.Current.Float64ArrayPrototype, TypedArrayType.Float64Array, a);
        }


        [Static("of", Length = 1)]
        public static JSValue Of(in Arguments a)
        {

            return TypedArrayStatic.Of(JSContext.Current.Float64ArrayPrototype, TypedArrayType.Float64Array, a);
        }

    }


    internal static class TypedArrayStatic
    {
        internal static TypedArray AsTypedArray(this JSValue v,
        [CallerMemberName] string helper = null)
        {
            if (!(v is TypedArray array))
                throw JSContext.Current.NewTypeError($"TypedArray.prototype.{helper} called on non TypedArray");
            return array;
        }

        public static JSValue Constructor(JSObject prototype, TypedArrayType type, in Arguments a) {

            if (a.Length == 0) {
                return new TypedArray(null, type, 0, 0, prototype);
            }
            var (a1, a2, a3) = a.Get3();
            if (a1.IsNumber) {
                return new TypedArray(null, type, 0, a1.IntValue, prototype);
            }
            if (a1 is JSArrayBuffer arrayBuffer) {
                int byteOffset = a2.AsInt32OrDefault();
                int bufferLength = a3.AsInt32OrDefault(arrayBuffer.Length);
                return new TypedArray(arrayBuffer, type, byteOffset,bufferLength, prototype);
            }
            return CopyArray(prototype, type, a1, JSUndefined.Value, null);

            
        }


        // [Static("from", Length = 1)]
        public static JSValue From(JSObject prototype, TypedArrayType type, in Arguments a)
        {
            var (f, map, mapThis) = a.Get3();
            var t = a.This;
            return CopyArray(prototype, type, f, map, mapThis);

        }

        private static JSValue CopyArray(JSObject prototype, TypedArrayType type, JSValue f, JSValue map, JSValue mapThis)
        {
            int length = -1;

            switch (f)
            {
                case JSArray array:
                    length = array.Length;
                    break;
                case JSString @string:
                    length = @string.value.Length;
                    break;
                case TypedArray typed:
                    length = typed.Length;
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
            else
            {
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

        public static JSValue Of(JSObject prototype, TypedArrayType type, in Arguments a)
        {
            var length = a.Length;
            var r = new TypedArray(null,type,0,length,prototype);
            for (uint ai = 0; ai < length; ai++)
            {
                r[ai] = a.GetAt((int)ai);
            }
            return r;
        }
    }

}
