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

            var k = key.ToKey();
            if (k.IsSymbol)
            {
                JSObject.InternalAddProperty(targetObject, k.Symbol, pd);
            }
            else
            {
                if (!k.IsUInt)
                {
                    JSObject.InternalAddProperty(targetObject, k.KeyString, pd);
                }
                else
                {
                    JSObject.InternalAddProperty(targetObject, k.Index, pd);
                }
            }
            return JSBoolean.True;
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
    }
}
