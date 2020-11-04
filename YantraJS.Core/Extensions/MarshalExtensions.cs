using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Enumerators;
using YantraJS.Core.Storage;

namespace YantraJS.Core
{
    public static class MarshalExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this object value)
        {
            return ClrProxy.Marshal(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this Type type)
        {
            return ClrType.From(type);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this string value)
        {
            return new JSString(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this bool value)
        {
            return value ? JSBoolean.True : JSBoolean.False;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this int value)
        {
            return new JSNumber(value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this uint value)
        {
            return new JSNumber(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this double value)
        {
            return new JSNumber(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this float value)
        {
            return new JSNumber(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this short value)
        {
            return new JSNumber(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue Marshal(this byte value)
        {
            return new JSNumber(value);
        }



        internal static bool TryUnmarshal(this JSObject @object, Type type, out object result)
        {
            return cache[type](@object, out result);
        }

        delegate bool UnmarshalDelegate(JSObject @object, out object result);

        static ConcurrentTypeTrie<UnmarshalDelegate> cache = new ConcurrentTypeTrie<UnmarshalDelegate>(UnmarshalDelegateFactory);

        static UnmarshalDelegate UnmarshalDelegateFactory (Type type)
        {

            var c = type.GetConstructor(new Type[] { typeof(JSValue) });
            if (c != null)
            {
                bool Unmarshal(JSObject @object, out object result)
                {
                    result = c.Invoke(new object[] { @object });
                    return true;
                }
                return Unmarshal;
            }

            c = type.GetConstructor(new Type[] { });
            if (c != null)
            {
                if (type.IsConstructedGenericType)
                {
                    var gt = type.GetGenericTypeDefinition();
                    // check if it is List<T> ....
                    if (gt == typeof(List<>)) {
                        // add all items...
                        var et = type.GetGenericArguments()[0];
                        // get enumerator...
                        bool UnmarshalList(JSObject @object, out object result) {
                            var list = (result = c.Invoke(new object[] { })) as System.Collections.IList;
                            var en = @object.GetElementEnumerator();
                            while(en.MoveNext(out var item))
                            {
                                list.Add(item.ForceConvert(et));
                            }
                            return true;
                        }

                        return UnmarshalList;
                    }

                    // check if it is a Dictionary<T>...

                    if (gt == typeof(Dictionary<,>)) {
                        var keys = gt.GetGenericArguments();
                        var keyType = keys[0];
                        var valueType = keys[1];

                        bool UnmarshalList(JSObject @object, out object result)
                        {
                            var list = (result = c.Invoke(new object[] { })) as System.Collections.IDictionary;
                            var en = new PropertyEnumerator(@object, true, true);
                            while (en.MoveNext(out var key, out var value))
                            {
                                list.Add(
                                    Convert.ChangeType(key.ToString(), keyType),
                                    value.ForceConvert(valueType));
                            }
                            return true;
                        }

                        return UnmarshalList;
                    }

                }
                


                // change this logic to support case insensitive property match
                var properties = type.GetProperties()
                    .Where(x => x.CanWrite)
                    .ToDictionary(x => x.Name.ToLower(), x => x);
                bool Unmarshal(JSObject @object, out object result)
                {
                    result = c.Invoke(new object[] { });
                    var en = new PropertyEnumerator(@object, true, true);
                    while(en.MoveNext(out var key, out var value))
                    {
                        if (properties.TryGetValue(key.ToString().ToLower(), out var p))
                        {
                            p.SetValue(result, value.ForceConvert(p.PropertyType));
                        }
                    }
                    return true;
                }
                return Unmarshal;
            }

            bool NotSupported(JSObject  @object, out object result)
            {
                result = null;
                return false;
            }
            return NotSupported;
        }
    }
}
