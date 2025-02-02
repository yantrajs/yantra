using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.CodeGen;
using YantraJS.Core.LightWeight;
using YantraJS.Core.LinqExpressions.GeneratorsV2;

namespace YantraJS.Core.Generator
{
    [JSClassGenerator("Generator")]
    public partial class JSGenerator : JSObject
    {
        readonly IElementEnumerator en;
        private ClrGeneratorV2 cg;
        private readonly string name;

        internal JSValue value;
        internal bool done;

        public JSGenerator(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            throw new NotImplementedException();
        }

        public JSGenerator(IElementEnumerator en, string name): this() {
            this.en = en;
            this.name = name;
        }

        public JSGenerator(ClrGeneratorV2 g): this()
        {
            this.cg = g;
            value = JSUndefined.Value;
        }

        public override string ToString()
        {
            return $"[object {name}]";
        }

        [JSExport("toString")]
        public new JSValue ToString(in Arguments a)
        {
            return new JSString(ToString());
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
            cg.InjectException(JSException.FromValue(value));
            return value;
        }

        public JSValue ValueObject => (JSObject.NewWithProperties())
                    .AddProperty(KeyStrings.value, this.value)
                    .AddProperty(KeyStrings.done, done ? JSBoolean.True : JSBoolean.False);

        public bool MoveNext(JSValue replaceOld, out JSValue item)
        {

            var c = JSContext.Current;
            var top = c.Top;
            try
            {
                // c.Top = cg.StackItem;
                cg.Next(replaceOld, out item, out this.done);
                this.value = item;
                if (!this.done) {
                    return true;
                }
                this.value = item;
                this.done = true;
                return false;
            }
            finally
            {
                c.Top = top;
            }
        }

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
                // c.Top = cg.StackItem;
                cg.Next(replaceOld, out this.value, out this.done);
                //if(this.done == true && this.value == null)
                //{
                //    throw new ArgumentNullException();
                //}
                return ValueObject;
            }finally
            {
                c.Top = top; 
            }
        }

        public override IElementEnumerator GetElementEnumerator()
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

            public JSValue NextOrDefault(JSValue @default)
            {
                generator.Next();
                if (!generator.done)
                {
                    this.index++;
                    return this.generator.value;
                }
                return @default;
            }

        }

        [JSExport("next", Length = 1)]
        public JSValue Next(in Arguments a)
        {
            return Next(a.Length == 0 ? null : a.Get1());
        }

        [JSExport("return", Length = 1)]
        public JSValue Return(in Arguments a)
        {
            return Return(a.Get1());
        }

        [JSExport("throw", Length = 1)]
        public JSValue Throw(in Arguments a)
        {
            return Throw(a.Get1());
        }

    }
}
