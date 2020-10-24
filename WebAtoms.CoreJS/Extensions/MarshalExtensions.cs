using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Storage;

namespace WebAtoms.CoreJS.Core
{
    public static class MarshalExtensions
    {

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
                        var key = keys[0];
                        var value = keys[1];

                        bool UnmarshalList(JSObject @object, out object result)
                        {
                            var list = (result = c.Invoke(new object[] { })) as System.Collections.IDictionary;
                            var en = @object.GetAllEntries().GetEnumerator();
                            while (en.MoveNext())
                            {
                                var item = en.Current;
                                list.Add(item.Key.ForceConvert(key), item.Value.ForceConvert(value));
                            }
                            return true;
                        }

                        return UnmarshalList;
                    }

                }
                

                var properties = type.GetProperties().Where(x => x.CanWrite).ToArray();
                bool Unmarshal(JSObject @object, out object result)
                {
                    result = c.Invoke(new object[] { });
                    foreach(var p in properties)
                    {
                        var v = @object[p.Name];
                        if (v.IsNullOrUndefined)
                        {
                            v = @object[p.Name.ToCamelCase()];
                            if (v.IsNullOrUndefined)
                                continue;
                        }
                        p.SetValue(result, v.ForceConvert(p.PropertyType));
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
