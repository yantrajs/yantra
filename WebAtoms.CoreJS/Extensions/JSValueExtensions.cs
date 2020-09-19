using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Extensions
{
    public static class JSValueExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue NullIfTrue(JSValue value)
        {
            if (value.BooleanValue)
                return null;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSValue NullIfFalse(JSValue value)
        {
            if (!value.BooleanValue)
                return null;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<(JSValue Key, JSValue Value)> 
            GetAllEntries(this JSValue value, bool showEnumerableOnly = true)
        {
            if (!(value is JSObject @object))
                yield break;
            var elements = @object.elements;
            if (elements != null)
            {
                foreach (var p in elements.AllValues)
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return ( new JSNumber(p.Key), value.GetValue(p.Value));
                }
            }

            var ownProperties = @object.ownProperties;
            if (ownProperties != null)
            {
                foreach (var p in ownProperties.AllValues())
                {
                    if (showEnumerableOnly)
                    {
                        if (!p.Value.IsEnumerable)
                            continue;
                    }
                    yield return (p.Value.ToJSValue(), value.GetValue(p.Value));
                }
            }

            var @base = value.prototypeChain;
            if (@base != value && @base != null) {
                foreach(var bp in @base.GetAllEntries(showEnumerableOnly))
                {
                    yield return bp;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<(int index, KeyString key, JSValue value)> GetOwnEntries(this JSValue value)
        {
            if (!(value is JSObject @object))
                yield break;
            var elements = @object.elements;
            if (elements != null)
            {
                foreach (var p in elements.AllValues)
                {
                    yield return ((int)p.Key, KeyString.Empty, value.GetValue(p.Value));
                }
            }

            var ownProperties = @object.ownProperties;
            if (ownProperties != null)
            {
                foreach (var p in ownProperties.AllValues())
                {
                    yield return (-1, p.Value.key, value.GetValue(p.Value));
                }
            }
        }

        public static JSBoolean InstanceOf(this JSValue target, JSValue value)
        {
            if (value.IsUndefined)
                throw JSContext.Current.NewTypeError("Right side of instanceof is undefined");
            if (value.IsNull)
                throw JSContext.Current.NewTypeError("Right side of instanceof is null");
            if(!value.IsObject)
                throw JSContext.Current.NewTypeError("Right side of instanceof is not an object");
            var p = target.prototypeChain;
            if (p == null)
                return JSBoolean.False;
            var c = p[KeyStrings.constructor];
            if (c.IsUndefined)
                return JSBoolean.False;
            if (c.StrictEquals(value).BooleanValue)
                return JSBoolean.True;
            return c.InstanceOf(value);
        }

        public static JSBoolean IsIn(this JSValue target, JSValue value)
        {
            //var target = this;
            //while(target != null)
            //{
            //    target.prototypeChain 
            //}
            var tx = value as JSObject;
            if (tx == null)
                return JSBoolean.False;
            foreach(var a in tx.GetAllKeys())
            {
                if (a.Equals(target).BooleanValue)
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        //public static JSValue InvokeMethod(this JSValue target, KeyString key, JSValue[] args)
        //{
        //    var property = target.GetProperty(key);
        //    if (property.IsUndefined)
        //        throw new NotImplementedException($"Cannot invoke {key}, it is undefined");
        //    if (!(property is JSFunction function))
        //        throw new NotImplementedException($"Cannot invoke {key}, {property} is not a function");
        //    return function.f(target, args);
        //}

        //public static JSValue InvokeMethod(this JSValue target, uint key, JSValue[] args)
        //{
        //    var property = target.GetProperty(key);
        //    if (property.IsUndefined)
        //        throw new NotImplementedException($"Cannot invoke {key}, it is undefined");
        //    if (!(property is JSFunction function))
        //        throw new NotImplementedException($"Cannot invoke {key}, {property} is not a function");
        //    return function.f(target, args);
        //}

        //public static JSValue InvokeMethod(this JSValue target, JSValue key, JSValue[] args)
        //{
        //    var property = target.GetProperty(key);
        //    if (property.IsUndefined)
        //        throw new NotImplementedException($"Cannot invoke {key}, it is undefined");
        //    if (!(property is JSFunction function))
        //        throw new NotImplementedException($"Cannot invoke {key}, {property} is not a function");
        //    return function.f(target, args);
        //}

    //    internal static JSValue Delete(this JSValue target, KeyString ks)
    //    {
    //        if (target.IsUndefined)
    //        {
    //            throw JSContext.Current.NewTypeError($"Unable to set {ks} of undefined");
    //        }
    //        if (target.IsNull)
    //        {
    //            throw JSContext.Current.NewTypeError($"Unable to set {ks} of null");
    //        }
    //        if (!(target is JSObject @object))
    //            return JSBoolean.False;
    //        var ownProperties = @object.ownProperties;
    //        if (ownProperties == null)
    //            return JSBoolean.False;
    //        var px = ownProperties[ks.Key];
    //        if (px.IsEmpty)
    //            return JSBoolean.False;
    //        // only in strict mode...
    //        if (!px.IsConfigurable)
    //            throw JSContext.Current.NewTypeError("Cannot delete property of sealed object");
    //        ownProperties.RemoveAt(ks.Key);
    //        return JSBoolean.True;
    //    }

    //    internal static JSValue Delete(this JSValue target, uint ks)
    //    {
    //        if (target.IsUndefined)
    //        {
    //            throw JSContext.Current.NewTypeError($"Unable to set {ks} of undefined");
    //        }
    //        if (target.IsNull)
    //        {
    //            throw JSContext.Current.NewTypeError($"Unable to set {ks} of null");
    //        }
    //        if (!(target is JSObject @object))
    //            return JSBoolean.False;
    //        var ownProperties = @object.elements;
    //        if (ownProperties == null)
    //            return JSBoolean.False;
    //        var px = ownProperties[ks];
    //        if (px.IsEmpty)
    //            return JSBoolean.False;
    //        // only in strict mode...
    //        if (!px.IsConfigurable)
    //            throw JSContext.Current.NewTypeError("Cannot delete property of sealed object");
    //        ownProperties.RemoveAt(ks);
    //        return JSBoolean.True;
    //    }
    }
}
