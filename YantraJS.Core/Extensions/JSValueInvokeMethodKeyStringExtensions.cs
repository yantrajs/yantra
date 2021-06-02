using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core
{
    public static partial class JSValueExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, in KeyString name)
        {
            var fx = @this.GetMethod(name)
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var a = new Arguments(@this);
            return fx(a);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, in KeyString name, JSValue arg0)
        {
            var fx = @this.GetMethod(name)
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var a = new Arguments(@this, arg0);
            return fx(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, in KeyString name, JSValue arg0, JSValue arg1)
        {
            var fx = @this.GetMethod(name)
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var a = new Arguments(@this, arg0, arg1);
            return fx(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, in KeyString name, JSValue arg0, JSValue arg1, JSValue arg2)
        {
            var fx = @this.GetMethod(name) 
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var a = new Arguments(@this, arg0, arg1, arg2);
            return fx(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, in KeyString name, JSValue arg0, JSValue arg1, JSValue arg2, JSValue arg3)
        {
            var fx = @this.GetMethod(name)
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var a = new Arguments(@this, arg0, arg1, arg2, arg3);
            return fx(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethod(this JSValue @this, in KeyString name, JSValue[] args)
        {
            var fx = @this.GetMethod(name)
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var a = new Arguments(@this, args);
            return fx(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue InvokeMethodSpread(this JSValue @this, in KeyString name, JSValue[] args)
        {
            var fx = @this.GetMethod(name)
                ?? throw JSContext.Current.NewTypeError($"Method {name} not found in {@this}");
            var length = 0;
            foreach ( var item in args)
            {
                length += item.IsSpread ? item.Length : 1;
            }
            var a = new Arguments(@this, args, length);
            return fx(a);
        }

    }
}
