using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Extensions;
using WebAtoms.CoreJS.Utils;

namespace WebAtoms.CoreJS.Core
{
    public static class JSValueExtensions
    {

        /// <summary>
        /// Returns .net string if it is not undefined
        /// </summary>
        /// <param name="target"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string AsStringOrDefault(this JSValue target, string def = null)
        {
            return target.IsUndefined ? def : target.ToString();
        }

        /// <summary>
        /// Returns .net int if it is not undefined
        /// </summary>
        /// <param name="target"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int AsInt32OrDefault(this JSValue target, int def = 0)
        {
            return target.IsUndefined ? def : target.IntValue;
        }

        /// <summary>
        /// Returns .net double if it is not undefined
        /// </summary>
        /// <param name="target"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static double AsDoubleOrDefault(this JSValue target, double def = 0)
        {
            return target.IsUndefined ? def : target.DoubleValue;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, KeyString name, in Arguments a)
        {
            var fx = @this[name];
            if (fx.IsUndefined)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {@this}");
            return fx.InvokeFunction(a.OverrideThis(@this));
        }

        public static JSValue InvokeMethod(this JSValue @this, uint name, in Arguments a)
        {
            var fx = @this[name];
            if (fx.IsUndefined)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {@this}");
            return fx.InvokeFunction(a.OverrideThis(@this));
        }

        public static JSValue InvokeMethod(this JSValue @this, JSValue name, in Arguments a)
        {
            var key = name.ToKey();
            if (key.IsUInt)
                return @this.InvokeMethod(key.Key, a);
            return @this.InvokeMethod(key, a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue NullIfUndefined(JSValue value)
        {
            if (value.IsUndefined)
                return null;
            return value;
        }

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
                foreach (var (Key, Value) in elements.AllValues)
                {
                    if (showEnumerableOnly)
                    {
                        if (!Value.IsEnumerable)
                            continue;
                    }
                    yield return ( new JSNumber(Key), value.GetValue(Value));
                }
            }

            var en = new PropertySequence.Enumerator(@object.GetOwnProperties(false));
            while(en.MoveNext())
            {
                var p = en.Current;
                if (showEnumerableOnly)
                {
                    if (!p.IsEnumerable)
                        continue;
                }
                yield return (p.ToJSValue(), value.GetValue(p));
            }

            var @base = value.prototypeChain;
            if (@base != value && @base != null) {
                foreach(var bp in @base.GetAllEntries(showEnumerableOnly))
                {
                    yield return bp;
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
            return p.InstanceOf(value);
        }

        public static JSBoolean IsIn(this JSValue target, JSValue value)
        {
            if (!(value is JSObject tx))
                return JSBoolean.False;
            var key = target.ToKey(false);
            if (key.IsUInt)
            {
                var p = tx.GetInternalProperty(key.Key);
                if (p.IsEnumerable)
                    return JSBoolean.True;
            }
            var p1 = tx.GetInternalProperty(key);
            if (p1.IsEnumerable)
                return JSBoolean.True;
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
