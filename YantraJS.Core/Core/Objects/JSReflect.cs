using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Core.Objects
{
    [JSClassGenerator("Reflect"), JSInternalObject]
    public partial class JSReflect: JSObject
    {

        [JSExport( Length = 3)]
        public static JSValue Apply(in Arguments a)
        {
            var (target, thisArgument, arguments) = a.Get3();
            var fx = target as JSFunction;
            return fx.InvokeFunction(Arguments.ForApply(thisArgument, arguments));
        }

        [JSExport( Length = 2)]
        public static JSValue Construct(in Arguments a)
        {
            var (target, arguments, newTarget) = a.Get3();
            newTarget = newTarget.IsUndefined ? target : newTarget;
            var fx = target as JSFunction;
            return fx.CreateInstance(Arguments.ForApply(new JSObject(), arguments));
        }

        [JSExport( Length = 3)]
        public new static JSValue DefineProperty(in Arguments a)
        {
            var (target, propertyKey, attributes) = a.Get3();
            if (!(target is JSObject targetObject))
                return JSBoolean.False;
            if (!targetObject.IsExtensible())
                return JSBoolean.False;
            if (!(attributes is JSObject pd))
                return JSBoolean.False;
            return targetObject.DefineProperty(propertyKey, pd);
            //var k = propertyKey.ToKey();
            //if (k.IsSymbol)
            //{
            //    // JSObject.InternalAddProperty(targetObject, k.Symbol, pd);
                
            //}
            //else
            //{
            //    if (!k.IsUInt)
            //    {
            //        JSObject.InternalAddProperty(targetObject, k.KeyString, pd);
            //    }
            //    else
            //    {
            //        JSObject.InternalAddProperty(targetObject, k.Index, pd);
            //    }
            //}
            //return JSBoolean.True;
        }

        [JSExport( Length = 2)]
        public static JSValue DeleteProperty(in Arguments a)
        {
            var (target, propertyKey) = a.Get2();
            if (!(target is JSObject @object))
                return JSBoolean.False;
            var key = propertyKey.ToKey();
            if (key.IsSymbol)
            {
                ref var symbols = ref @object.GetSymbols();
                return symbols.RemoveAt(key.Index) ? JSBoolean.True : JSBoolean.False;
            }
            if (key.IsUInt)
            {
                ref var elements = ref @object.GetElements();
                return elements.RemoveAt(key.Index) ? JSBoolean.True : JSBoolean.False;
            }
            ref var properties = ref @object.GetOwnProperties();
            return properties.RemoveAt(key.Index) ? JSBoolean.True : JSBoolean.False;
        }

        [JSExport( Length = 2)]
        public static JSValue Get(in Arguments a)
        {
            var (target, propertyKey, receiver) = a.Get3();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            receiver = receiver.IsUndefined ? target : receiver;
            return target.GetValue(propertyKey, receiver, false);
        }

        [JSExport( Length = 2)]
        public new static JSValue GetOwnPropertyDescriptor(in Arguments a)
        {
            var (target, propertyKey) = a.Get2();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            var key = propertyKey.ToKey();
            JSProperty p;
            if (key.IsSymbol)
            {
                p = @object.GetInternalProperty(key.Symbol);
            }
            else
            {
                if (key.IsUInt)
                {
                    p = @object.GetInternalProperty(key.Index);
                }
                else
                {
                    p = @object.GetInternalProperty(in key.KeyString);
                }
            }
            if (p.IsEmpty)
                return JSUndefined.Value;
            return p.ToJSValue();
        }

        [Static("getPrototypeOf")]
        public new static JSValue GetPrototypeOf(in Arguments a)
        {
            var target = a.Get1();
            if(!(target is JSObject))
                throw JSContext.Current.NewTypeError($"Not an object");
            var p = target.prototypeChain?.@object;
            if (p == target || p == null)
                return JSNull.Value;
            return p;
        }

        [JSExport( Length = 2)]
        public static JSValue Has(in Arguments a)
        {
            var (target, propertyKey, receiver) = a.Get3();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            var key = propertyKey.ToKey();
            JSProperty p;
            if (key.IsSymbol)
            {
                p = @object.GetInternalProperty(key.Symbol);
            }
            else
            {
                if (key.IsUInt)
                {
                    p = @object.GetInternalProperty(key.Index);
                }
                else
                {
                    p = @object.GetInternalProperty(in key.KeyString);
                }
            }
            if (p.IsEmpty)
                return JSBoolean.False;
            return JSBoolean.True;
        }

        [JSExport( Length = 1)]
        public new static JSValue IsExtensible(in Arguments a)
        {
            var target = a.Get1();
            if(!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            return @object.IsExtensible() ? JSBoolean.True : JSBoolean.False;
        }

        [JSExport( Length = 1)]
        public static JSValue OwnKeys(in Arguments a)
        {
            var target = a.Get1();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            // return @object.IsExtensible() ? JSBoolean.True : JSBoolean.False;
            var r = new JSArray();
            var een = @object.GetElementEnumerator();
            while(een.MoveNext(out var hasValue, out var value, out var index))
            {
                if (hasValue)
                    r.Add(new JSNumber(index));
            }
            var en = @object.GetOwnProperties(false).GetEnumerator();
            while (en.MoveNext(out var property))
            {
                r.Add(property.ToJSValue());
            }
            ref var symbols = ref @object.GetSymbols();
            foreach(var e in symbols.AllValues())
            {
                r.Add(e.Value.ToJSValue());
            }
            return r;
        }

        [JSExport( Length = 1)]
        public new static JSValue PreventExtensions(in Arguments a)
        {
            var target = a.Get1();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            @object.status |= ObjectStatus.NonExtensible;
            return JSBoolean.True;
        }

        [JSExport( Length = 2)]
        public static JSValue Set(in Arguments a)
        {
            var (target, propertyKey, value, receiver) = a.Get4();
            if (!(target is JSObject @object))
                throw JSContext.Current.NewTypeError($"Not an object");
            receiver = receiver.IsUndefined ? target : receiver;
            var key = propertyKey.ToKey();
            if (key.IsSymbol)
            {
                var symbol = key.Symbol;
                var p = @object.GetInternalProperty(symbol, false);
                if (p.IsProperty)
                {
                    p.set.InvokeFunction(new Arguments(receiver, value));
                    return JSBoolean.True;
                }
                ref var symbols = ref @object.GetSymbols();
                symbols.Save(symbol.Key, JSProperty.Property(value));
                return JSBoolean.True;
            }
            else
            {
                if (key.IsUInt)
                {
                    var p = @object.GetInternalProperty(key.Index, false);
                    if (p.IsProperty)
                    {
                        p.set.InvokeFunction(new Arguments(receiver, value));
                        return JSBoolean.True;
                    }
                    ref var elements = ref @object.GetElements(true);
                    elements.Put(key.Index, value);
                    return JSBoolean.True;
                }
                else
                {
                    var p = @object.GetInternalProperty(in key.KeyString, false);
                    if (p.IsProperty)
                    {
                        p.set.InvokeFunction(new Arguments(receiver, value));
                        return JSBoolean.True;
                    }
                    ref var properties = ref @object.GetOwnProperties(true);
                    properties.Put(in key.KeyString, value);
                    return JSBoolean.True;
                }
            }
        }

        [JSExport]
        public new static JSValue SetPrototypeOf(in Arguments a)
        {
            var (target,p) = a.Get2();
            if (!(target is JSObject))
                throw JSContext.Current.NewTypeError($"Not an object");
            if(!(p is JSObject prototype))
                throw JSContext.Current.NewTypeError($"Not an object");
            target.BasePrototypeObject = prototype;
            return p;
        }
    }

}
