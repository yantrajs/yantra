using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using YantraJS.Core.CodeGen;
using YantraJS.Core.LightWeight;
using YantraJS.Core.LinqExpressions.Generators;
using YantraJS.Core.LinqExpressions.GeneratorsV2;

namespace YantraJS.Core.Generator
{
    public class JSGenerator : JSObject, IDisposable
    {
        readonly IElementEnumerator en;
        private ClrGeneratorV2 cg;
        private readonly string name;

        private JSContext context;
        internal JSValue value;
        internal bool done;

        public JSGenerator(IElementEnumerator en, string name) {
            this.BasePrototypeObject = JSContext.Current.GeneratorPrototype;
            this.en = en;
            this.name = name;
        }

        public JSGenerator(ClrGeneratorV2 g)
        {
            this.BasePrototypeObject = JSContext.Current.GeneratorPrototype;
            this.cg = g;
            value = JSUndefined.Value;
        }

        public override string ToString()
        {
            return $"[object {name}]";
        }

        [Prototype("toString")]
        public static JSValue ToString(in Arguments a)
        {
            var a1 = a.This as JSGenerator;
            return new JSString(a1.ToString());
        }

        ~JSGenerator()
        {
            OnDispose(false);
        }


        // Thread thread;

        public JSValue Return(JSValue value)
        {
            this.done = true;
            this.value = JSUndefined.Value;
            return (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);
        }

        public JSValue Throw(JSValue value)
        {
            //yield?.Dispose();
            //wait?.Dispose();
            //yield = null;
            //wait = null;
            return ValueObject;
        }

        public JSValue ValueObject => (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, this.value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);

        public JSValue Next(JSValue replaceOld = null)
        {
            JSValue item;
            if (en != null) {
                if (en.MoveNext(out item)) {
                    this.value = item;
                    return ValueObject;
                }
                this.done = true;
                this.value = JSUndefined.Value;
                return ValueObject;
            }
            var c = JSContext.Current;
            var top = c.Top;
            try
            {
                c.Top = cg.StackItem;
                if (cg.Next(replaceOld, out item))
                {
                    this.done = false;
                    this.value = item;
                    return ValueObject;
                }
            }finally
            {
                c.Top = top; 
            }

            this.done = true;
            // this.value = JSUndefined.Value;
            return ValueObject;
        }

        internal override IElementEnumerator GetElementEnumerator()
        {
            return new ElementEnumerator(this);
        }

        private struct ElementEnumerator: IElementEnumerator
        {
            readonly JSGenerator generator;
            int index;
            public ElementEnumerator(JSGenerator generator)
            {
                this.generator = generator;
                index = -1;
            }

            public bool MoveNext(out JSValue value)
            {
                generator.Next();
                if (!generator.done)
                {
                    this.index++;
                    value = this.generator.value;
                    return true;
                }
                value = JSUndefined.Value;
                return false;

            }


            public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
            {
                generator.Next();
                if (!generator.done)
                {
                    this.index++;
                    index = (uint)this.index;
                    hasValue = true;
                    value = this.generator.value;
                    return true;
                }
                index = 0;
                value = JSUndefined.Value;
                hasValue = false;
                return false;

            }

            public bool MoveNextOrDefault(out JSValue value, JSValue @default)
            {
                generator.Next();
                if (!generator.done)
                {
                    this.index++;
                    value = this.generator.value;
                    return true;
                }
                value = @default;
                return false;

            }
        }

        [Prototype("next", Length = 1)]
        public static JSValue Next(in Arguments a)
        {
            if(!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Next(a.Length == 0 ? null : a.Get1());
        }

        [Prototype("return", Length = 1)]
        public static JSValue Return(in Arguments a)
        {
            if (!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Return(a.Get1());
        }

        [Prototype("throw", Length = 1)]
        public static JSValue Throw(in Arguments a)
        {
            if (!(a.This is JSGenerator generator))
            {
                throw JSContext.Current.NewTypeError($"receiver for Generator.prototype.next should be generator");
            }
            return generator.Return(a.Get1());
        }

        private void OnDispose(bool supress = true)
        {
            //threadTop?.Pop(context);
            //threadTop = null;
            //yield?.Dispose();
            //wait?.Dispose();
            //yield = null;
            //wait = null;
            if (supress)
            {
                GC.SuppressFinalize(this);
            }
        }

        void IDisposable.Dispose()
        {
            OnDispose();
        }
    }
}
