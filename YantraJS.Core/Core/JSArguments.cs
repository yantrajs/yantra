using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using YantraJS.Core.Generator;

namespace YantraJS.Core
{
    public class JSArguments: JSObject
    {
        public JSValue Callee(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"Cannot access callee in strict mode");
        }

        public new JSValue Values(in Arguments a)
        {
            return new JSGenerator(this.GetElementEnumerator(), "Arguments");
        }

        public static JSValue[] Empty = new JSValue[] { };

        public override bool BooleanValue => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Arguments;
        }

        internal override PropertyKey ToKey(bool create = false)
        {
            return KeyStrings.arguments;
        }

        public JSArguments(in Arguments args)
        {
            // arguments = args;
            ref var properties = ref this.GetOwnProperties(true);
            properties.Put(KeyStrings.length, new JSNumber(args.Length), JSPropertyAttributes.ConfigurableValue);
            properties.Put(KeyStrings.callee, (JSFunctionDelegate)Callee, Callee, JSPropertyAttributes.Property);

            ref var symbols = ref this.GetSymbols();
            symbols.Put(JSSymbol.iterator.Key) = JSProperty.Property(new JSFunction(Values), JSPropertyAttributes.ConfigurableValue);
            ref var elements = ref this.CreateElements();
            for (int i = 0; i < args.Length; i++)
            {
                elements.Put((uint)i, args.GetAt(i));
            }
        }

        public override string ToString()
        {
            return "[object Arguments]";
        }
    }
}
