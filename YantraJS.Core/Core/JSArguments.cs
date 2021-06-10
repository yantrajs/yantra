using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using YantraJS.Core.Generator;
using YantraJS.Core.Runtime;

namespace YantraJS.Core
{
    public class JSArguments: JSObject
    {
        public JSValue Callee(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"Cannot access callee in strict mode");
        }

        public JSValue Values(in Arguments a)
        {
            return new JSGenerator(this.GetElementEnumerator(), "Arguments");
        }

        public static JSValue[] Empty = new JSValue[] { };

        public override bool BooleanValue => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Arguments;
        }

        internal override KeyString ToKey(bool create = false)
        {
            return KeyStrings.arguments;
        }

        public JSArguments(in Arguments args)
        {
            // arguments = args;
            ref var properties = ref this.GetOwnProperties(true);
            properties[KeyStrings.length.Key] = JSProperty.Property(KeyStrings.length, new JSNumber(args.Length), JSPropertyAttributes.ConfigurableValue);
            properties[KeyStrings.callee.Key] = JSProperty.Property(KeyStrings.callee, (JSFunctionDelegate)Callee, Callee, JSPropertyAttributes.Property);

            ref var symbols = ref this.GetSymbols();
            symbols[JSSymbolStatic.iterator.Key.Key] = JSProperty.Property(new JSFunction(Values), JSPropertyAttributes.ConfigurableValue);
            ref var elements = ref this.CreateElements();
            for (int i = 0; i < args.Length; i++)
            {
                elements[(uint)i] = JSProperty.Property(args.GetAt(i));
            }
        }

        public override string ToString()
        {
            return "[object Arguments]";
        }
    }
}
