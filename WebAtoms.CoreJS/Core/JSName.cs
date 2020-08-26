using System;
using System.Runtime.CompilerServices;

namespace WebAtoms.CoreJS.Core
{
    //public struct JSName
    //{
    //    public readonly KeyString Key;
    //    public readonly bool IsSymbol;

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static implicit operator JSName(JSSymbol input)
    //    {
    //        return new JSName(input.Key, true);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static implicit operator JSName(string input)
    //    {
    //        return new JSName(input);
    //    }

    //    private JSName(KeyString key, bool isSymbol = false)
    //    {
    //        this.IsSymbol = isSymbol;
    //        this.Key = key;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is JSName jn)
    //            return Key.Key == jn.Key.Key;
    //        if (obj is KeyString k)
    //            return Key.Key == k.Key;
    //        if (obj is String sv)
    //            return Key.Value == sv;
    //        return false;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return (int)Key.Key;
    //    }

    //    public override string ToString()
    //    {
    //        return this.Key.Value;
    //    }

    //    public JSValue ToJSValue()
    //    {
    //        if (IsSymbol)
    //        {
    //            return new JSSymbol(Key);
    //        }
    //        return new JSString(Key.Value);
    //    }
    //}
}
