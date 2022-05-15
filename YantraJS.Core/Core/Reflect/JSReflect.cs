using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Internal
{
    public class JSReflectStatic
    {

        [Static("apply")]
        public static JSValue Apply(in Arguments a)
        {
            if(!(a[0] is JSFunction fx))
            {
                throw JSContext.Current.NewTypeError("Function expected");
            }
            var @this = a[1];
            var args = a[2];
            return fx.InvokeFunction(Arguments.ForApply(@this, args));
        }

        [Static("construct")]
        public static JSValue Construct(in Arguments a)
        {
            if (!(a[0] is JSFunction fx))
            {
                throw JSContext.Current.NewTypeError("Function expected");
            }
            var args = a[1];
            var newTarget = a[2];
            return fx.CreateInstance(Arguments.ForApply(JSUndefined.Value, args, newTarget));

        }

        [Static("defineProperty")]
        public static JSValue DefineProperty(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            var key = a[1];
            var desc = a[2];
            if (!targetObject.IsExtensible())
                return JSBoolean.False;
            if (!(desc is JSObject pd))
                return JSBoolean.False;
            return targetObject.DefineProperty(key, pd);
        }

        [Static("deleteProperty")]
        public static JSValue DeleteProperty(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            if (targetObject.IsSealedOrFrozen())
                return JSBoolean.False;
            var key = a[1];
            var propertyKey = key.ToKey(false);
            switch(propertyKey.Type)
            {
                case KeyType.Empty:
                    return JSBoolean.False;
                case KeyType.Symbol:
                    targetObject.GetSymbols().RemoveAt(propertyKey.Symbol.Key);
                    return JSBoolean.True;
                case KeyType.String:
                    targetObject.GetOwnProperties(false).RemoveAt(propertyKey.KeyString.Key);
                    return JSBoolean.True;
                case KeyType.UInt:
                    targetObject.GetElements(false).RemoveAt(propertyKey.Index);
                    return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [Static("get")]
        public static JSValue Get(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            var key = a[1];
            var receiver = a[2];
            if (receiver == null || receiver == JSUndefined.Value)
            {
                return targetObject[key];
            }
            var propertyKey = key.ToKey(false);
            ref var property = ref JSProperty.Empty;
            switch (propertyKey.Type)
            {
                case KeyType.Empty:
                    return JSUndefined.Value;
                case KeyType.Symbol:
                    property = ref targetObject.GetSymbols().GetRefOrDefault(propertyKey.Symbol.Key, ref JSProperty.Empty);
                    return receiver.GetValue(property);
                case KeyType.String:
                    property = ref targetObject.GetOwnProperties(false).GetValue(propertyKey.KeyString.Key);
                    return receiver.GetValue(property);
                case KeyType.UInt:
                    property = ref targetObject.GetElements(false).Get(propertyKey.Index);
                    return receiver.GetValue(property);
            }
            return JSUndefined.Value;
        }

        [Static("getOwnPropertyDescriptor")]
        public static JSValue GetOwnPropertyDescriptor(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            return targetObject.GetOwnProperty(a[1]);
        }
        
        [Static("getPrototypeOf")]
        public static JSValue GetPrototypeOf(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            return targetObject.prototypeChain?.@object ?? JSNull.Value;
        }

        [Static("has")]
        public static JSValue Has(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            return a[1].IsIn(targetObject);
        }

        [Static("isExtensible")]
        public static JSValue IsExtensible(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            return targetObject.IsExtensible() ? JSBoolean.True : JSBoolean.False;
        }

        [Static("ownKeys")]
        public static JSValue OwnKeys(in Arguments a)
        {
            return JSObjectStatic.GetOwnPropertyNames(a);
        }

        [Static("preventExtensions")]
        public static JSValue PreventExtensions(in Arguments a)
        {
            return JSObjectStatic.PreventExtensions(a);
        }

        [Static("set")]
        public static JSValue Set(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            var key = a[1];
            var receiver = a[2];
            var value = a[3];
            if (receiver == JSUndefined.Value)
            {
                receiver = null;
            }
            return targetObject.SetValue(key, value, receiver, false) ? JSBoolean.True : JSBoolean.False;
        }

        [Static("setPrototypeOf")]
        public static JSValue SetPrototypeOf(in Arguments a)
        {
            if (a[0] is not JSObject targetObject)
                throw JSContext.Current.NewTypeError($"Object expected");
            var p = a[1];
            if (p == JSNull.Value)
            {
                targetObject.BasePrototypeObject = null;
                return JSBoolean.True;
            }
            if (p is not JSObject prototype )
                throw JSContext.Current.NewTypeError($"Prototype must be an object or null");
            targetObject.BasePrototypeObject = prototype;
            return JSBoolean.True;
        }
    }
}
