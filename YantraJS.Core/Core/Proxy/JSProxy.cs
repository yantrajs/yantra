using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    [JSBaseClass("Object")]
    [JSFunctionGenerator("Proxy")]
    public partial class JSProxy : JSObject
    {
        readonly JSObject target;
        private readonly JSObject handler;

        protected JSProxy((JSObject target, JSObject handler) p) : base(JSContext.Current.ObjectPrototype)
        {
            var (target, handler) = p;
            if (target == null || handler == null)
            {
                throw JSContext.Current.NewTypeError("Cannot create proxy with a non-object as target or handler");
            }
            this.target = target;
            this.handler = handler;
        }

        public override bool BooleanValue => target.BooleanValue;


        public override bool Equals(JSValue value)
        {
            return target.Equals(value);
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            var fx = handler[KeyStrings.apply];
            if (fx is JSFunction fxFunction)
            {
                var args = new JSArray(a.ToArray());
                return fxFunction.Call(this, this.target, a.This, args);
            }
            return target.InvokeFunction(a);
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            var fx = handler[KeyStrings.constructor];
            if (fx is JSFunction fxFunction)
            {
                var args = new JSArray(a.ToArray());
                return fxFunction.Call(this, this.target, args);
            }
            return target.CreateInstance(a);
        }

        public override JSValue DefineProperty(JSValue key, JSObject propertyDescription)
        {
            var fx = handler[KeyStrings.defineProperty];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(target, target, key, propertyDescription));
            }
            return target.DefineProperty(key, propertyDescription);
        }

        public override JSValue Delete(JSValue index)
        {
            var fx = handler[KeyStrings.deleteProperty];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(target, target, index));
            }
            return target.Delete(index);
        }

        internal protected override JSValue GetValue(JSSymbol key, JSValue receiver, bool throwError = true)
        {
            var fx = handler[KeyStrings.get];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(target, target, key, receiver));
            }
            return target.GetValue(key, receiver, throwError);
        }

        internal protected override JSValue GetValue(KeyString key, JSValue receiver, bool throwError = true)
        {
            var fx = handler[KeyStrings.get];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(target,target, key.ToJSValue(), receiver));
            }
            return target.GetValue(key, receiver, throwError);
        }

        internal protected override JSValue GetValue(uint key, JSValue receiver, bool throwError = true)
        {
            var fx = handler[KeyStrings.get];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(target, target, new JSNumber(key), receiver));
            }
            return target.GetValue(key, receiver, throwError);
        }

        internal protected override bool SetValue(JSSymbol name, JSValue value, JSValue receiver, bool throwError = true)
        {
            var fx = handler[KeyStrings.set];
            if (fx is JSFunction fxFunction)
            {
                fxFunction.InvokeFunction(new Arguments(target, target, name, receiver));
                return true;
            }
            return target.SetValue(name, value, receiver, false);
        }

        internal protected override bool SetValue(KeyString name, JSValue value, JSValue receiver, bool throwError = true)
        {
            var fx = handler[KeyStrings.set];
            if (fx is JSFunction fxFunction)
            {
                fxFunction.InvokeFunction(new Arguments(target, target, name.ToJSValue(), receiver));
                return true;
            }
            return target.SetValue(name, value, receiver, false);
        }

        internal protected override bool SetValue(uint name, JSValue value, JSValue receiver, bool throwError = true)
        {
            var fx = handler[KeyStrings.set];
            if (fx is JSFunction fxFunction)
            {
                fxFunction.InvokeFunction(new Arguments(target, target, new JSNumber(name), receiver));
                return true;
            }
            return target.SetValue(name, value, receiver, false);
        }

        public override JSValue GetPrototypeOf()
        {
            var fx = handler[KeyStrings.getPrototypeOf];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(target));
            }
            return target.GetPrototypeOf();
        }

        public override void SetPrototypeOf(JSValue proto)
        {
            var fx = handler[KeyStrings.setPrototypeOf];
            if (fx is JSFunction fxFunction)
            {
                fxFunction.InvokeFunction(new Arguments(this.target, proto));
                return;
            }
            this.target.SetPrototypeOf(proto);
        }

        public override IElementEnumerator GetAllKeys(bool showEnumerableOnly = true, bool inherited = true)
        {

            var fx = handler[KeyStrings.ownKeys];
            if (fx is JSFunction fxFunction)
            {
                return fxFunction.InvokeFunction(new Arguments(this.target)).GetElementEnumerator();
            }
            return target.GetAllKeys(showEnumerableOnly, inherited);
        }

        public override bool StrictEquals(JSValue value)
        {
            return target.StrictEquals(value);
        }

        public override JSValue TypeOf()
        {
            return target.TypeOf();
        }

        internal override PropertyKey ToKey(bool create = false)
        {
            return target.ToKey();
        }

        [JSExport(IsConstructor = true)]
        public new static JSValue Constructor(in Arguments a)
        {
            var (f, s) = a.Get2();
            return new JSProxy((f as JSObject, s as JSObject));
        }
    }
}
