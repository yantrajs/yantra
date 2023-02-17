using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace YantraJS.Core
{
    internal static class UniqueID
    {


        // private static JSSymbol UniqueIDKey = new JSSymbol("UniqueID");

        internal static string ToUniqueID(this JSValue value)
        {
            switch(value)
            {
                case JSString @string:
                    return $"string:{@string}";
                case JSNumber n:
                    return $"number:{n.value}";
                case JSObject @object:
                    return $"id:{@object.UniqueID}";
                case JSSymbol symbol:
                    return $"symbol:{symbol.Key}";
            }
            return value.ToString();
        }

        //private static long NextID = 1;

        //private static string GenerateID(JSObject obj)
        //{
        //    ref var op = ref obj.GetSymbols();
        //    if(op.TryGetValue(UniqueIDKey.Key, out var px))
        //    {
        //        return $"ID:{px.value}";
        //    }
        //    var id = Interlocked.Increment(ref NextID);
        //    op.Put(UniqueIDKey.Key) = JSProperty.Property(new JSString(id.ToString()), JSPropertyAttributes.ConfigurableReadonlyValue);
        //    return $"ID:{id}";
        //}

    }
}
