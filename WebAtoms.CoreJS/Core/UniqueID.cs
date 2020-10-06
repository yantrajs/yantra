using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebAtoms.CoreJS.Core
{
    internal static class UniqueID
    {


        private static JSSymbol UniqueIDKey = new JSSymbol("UniqueID");

        internal static string ToUniqueID(this JSValue value)
        {
            switch(value)
            {
                case JSString @string:
                    return $"string:{@string}";
                case JSNumber n:
                    return $"number:{n.value}";
                case JSObject @object:
                    return GenerateID(@object);
            }
            return value.ToString();
        }

        private static long NextID = 1;

        private static string GenerateID(JSObject obj)
        {
            var op = obj.symbols ?? (obj.symbols = new CompactUInt32Trie<JSProperty>());
            if(op.TryGetValue(UniqueIDKey.Key.Key, out var px))
            {
                return $"ID:{px.value}";
            }
            var id = Interlocked.Increment(ref NextID);
            op[UniqueIDKey.Key.Key] = JSProperty.Property(new JSString(id.ToString()), JSPropertyAttributes.ConfigurableReadonlyValue);
            return $"ID:{id}";
        }

    }
}
