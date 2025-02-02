using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.Core.CodeGen;
using YantraJS.ExpHelper;
using YantraJS.Extensions;
using YantraJS.LinqExpressions;
using YantraJS.Runtime;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using Yantra.Core;

namespace YantraJS.Core
{

    [JSBaseClass("Object")]
    [JSFunctionGenerator("Function", Register = false)]
    public partial class JSFunction : JSObject
    {

        internal static JSFunctionDelegate empty = (in Arguments a) => a.This;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public JSObject prototype;

        private StringSpan source;

        internal JSFunction constructor;

        public readonly StringSpan name;

        internal JSFunctionDelegate f;
        
        public override bool IsFunction => true;

        public override JSValue TypeOf()
        {
            return JSConstants.Function;
        }


        /// <summary>
        /// Used as specific type constructor
        /// </summary>
        /// <param name="clrDelegate"></param>
        /// <param name="type"></param>
        internal JSFunction(JSFunctionDelegate clrDelegate, ClrType type)
            : this()
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = clrDelegate;
            this.name = "clr-native";
            this.source = source.IsEmpty 
                ? $"function {type.name}() {{ [clr-native] }}"
                : source;
            prototype = type.prototype;
            prototype.FastAddValue(KeyStrings.constructor, type, JSPropertyAttributes.EnumerableConfigurableValue);
            ownProperties.Put(KeyStrings.prototype.Key) = JSProperty.Property(KeyStrings.prototype, prototype);

            this.FastAddValue(KeyStrings.name, name.IsEmpty
                ? new JSString("native")
                : new JSString(name), JSPropertyAttributes.EnumerableConfigurableValue);
            this.FastAddValue(KeyStrings.length, new JSNumber(0), JSPropertyAttributes.EnumerableConfigurableValue);
            constructor = this;
        }

        internal void Seal()
        {
            ref var ownProperties = ref this.GetOwnProperties();
            ownProperties.Update((uint key, ref JSProperty p) => {
                if(p.IsValue)
                    p = new JSProperty(key, p.get, p.set, p.value, JSPropertyAttributes.ReadonlyValue);
            });
            //for (int i = 0; i < ownProperties.properties.Length; i++)
            //{
            //    ref var p = ref ownProperties.properties[i];
            //    if (p.IsValue)
            //    {
            //        ownProperties.properties[i] = new JSProperty(p.key, p.get, p.set, p.value, JSPropertyAttributes.ReadonlyValue);
            //    }
            //}
        }

        protected JSFunction(StringSpan name, StringSpan source, JSObject _prototype)
            : this()
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = empty;
            this.name = name.IsEmpty ? "native" : name;
            this.source = source.IsEmpty 
                ? $"function {this.name}() {{ [native] }}"
                : source;
            prototype = _prototype;
            prototype.GetOwnProperties(true).Put(KeyStrings.constructor, this);
            ownProperties.Put(KeyStrings.prototype, prototype);

            ownProperties.Put(KeyStrings.name, name.IsEmpty 
                ? new JSString("native")
                : new JSString(name));
            ownProperties.Put(KeyStrings.length, JSNumber.Zero);
            constructor = this;
        }

        public JSFunction(JSFunctionDelegate f)
            : this(f, StringSpan.Empty, StringSpan.Empty)
        {

        }

        public JSFunction(Func<JSFunctionDelegate> fx, in StringSpan name)
            : this(empty, in name, StringSpan.Empty)
        {
            this.f = (in Arguments a) =>
            {
                this.f = fx();
                return this.f(in a);
            };
        }

        public JSFunction(
            JSFunctionDelegate f,
            in StringSpan name,
            int length = 0): this(f, name, StringSpan.Empty, length)
        {

        }

        public JSFunction(
            JSObject basePrototype,
            JSFunctionDelegate f,
            in StringSpan name,
            in StringSpan source,
            int length = 0,
            bool createPrototype = true) : base(basePrototype)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = f;
            this.name = name.IsEmpty ? "native" : name;
            this.source = source.IsEmpty
                ? $"function {this.name}() {{ [native] }}"
                : source;
            if (createPrototype)
            {
                prototype = new JSObject();
                // prototype[KeyStrings.constructor] = this;
                prototype.FastAddValue(KeyStrings.constructor, this, JSPropertyAttributes.ConfigurableValue);
                // ref var opp = ref prototype.GetOwnProperties(true);
                // opp[KeyStrings.constructor.Key] = JSProperty.Property(this, JSPropertyAttributes.ConfigurableReadonlyValue);
                ownProperties.Put(KeyStrings.prototype, prototype, JSPropertyAttributes.ConfigurableValue);
            }

            //this[KeyStrings.name] = name.IsEmpty
            //    ? new JSString("native")
            //    : new JSString(name);
            // this[KeyStrings.length] = new JSNumber(length);
            ownProperties.Put(KeyStrings.name, name.IsEmpty
                ? new JSString("native")
                : new JSString(name));
            ownProperties.Put(KeyStrings.length, new JSNumber(length));
            constructor = this;
        }

        public JSFunction(
            JSFunctionDelegate f,
            in StringSpan name,
            in StringSpan source,
            int length = 0,
            bool createPrototype = true): base(JSContext.Current?.FunctionPrototype)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = f;
            this.name = name.IsEmpty ? "native" : name;
            this.source = source.IsEmpty
                ? $"function {this.name}() {{ [native] }}"
                : source;
            if (createPrototype)
            {
                prototype = new JSObject();
                // prototype[KeyStrings.constructor] = this;
                prototype.FastAddValue(KeyStrings.constructor, this, JSPropertyAttributes.ConfigurableValue);
                // ref var opp = ref prototype.GetOwnProperties(true);
                // opp[KeyStrings.constructor.Key] = JSProperty.Property(this, JSPropertyAttributes.ConfigurableReadonlyValue);
                ownProperties.Put(KeyStrings.prototype, prototype, JSPropertyAttributes.ConfigurableValue);
            }

            //this[KeyStrings.name] = name.IsEmpty
            //    ? new JSString("native")
            //    : new JSString(name);
            // this[KeyStrings.length] = new JSNumber(length);
            ownProperties.Put(KeyStrings.name, name.IsEmpty
                ? new JSString("native")
                : new JSString(name),
                JSPropertyAttributes.ConfigurableValue);
            ownProperties.Put(KeyStrings.length, new JSNumber(length),
                JSPropertyAttributes.ConfigurableValue);
            constructor = this;
        }

        public override JSValue this[KeyString name] { 
            get => base[name]; 
            set {
                if (name.Key == KeyStrings.prototype.Key)
                {
                    this.prototype = value as JSObject;
                }
                base[name] = value;
            }
        }

        //public override string ToString()
        //{
        //    return name.Value;
        //}

        public override string ToDetailString()
        {
            return this.source.Value;
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            if (prototype == null)
            {
                throw JSContext.Current.NewTypeError($"{name} is not a constructor");
            }
            JSValue obj = new JSObject
            {
                BasePrototypeObject = prototype
            };
            var a1 = a.OverrideThis(obj);
            JSContext.Current.CurrentNewTarget = this;
            var r = f(a1);
            if (r.IsObject)
            {
                r.BasePrototypeObject = this.prototype;
                return r;
            }
            return obj;
        }

        public JSValue InvokeSuper(in Arguments a)
        {
            var r = f(in a);
            if (r.IsObject)
            {
                return r;
            }
            return a.This;
            //var prototype = JSContext.NewTargetPrototype;
            //if (prototype == null)
            //{
            //    throw JSContext.Current.NewTypeError($"{name} is called as super outside constructor");
            //}
            //JSValue obj = new JSObject
            //{
            //    BasePrototypeObject = prototype
            //};
            //var a1 = a.OverrideThis(obj);
            //var r = f(a1);
            //if (r.IsObject)
            //{
            //    r.BasePrototypeObject = prototype;
            //    return r;
            //}
            //return obj;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return f(a);
        }

        [JSPrototypeMethod][JSExport("valueOf", Length = 1)]
        public new static JSValue ValueOf(in Arguments a)
        {
            return a.This;
        }


        //public override string ToString()
        //{
        //    var fx = this[KeyStrings.toString];
        //    if (fx.IsNullOrUndefined)
        //        return this.source.Value;
        //    return fx.InvokeFunction(Arguments.Empty).ToString();
        //}

        [JSPrototypeMethod][JSExport("call", Length = 1)]
        public static JSValue Call(in Arguments a)
        {
            var a1 = a.CopyForCall();
            return a.This.InvokeFunction(a1);
        }

        [JSPrototypeMethod][JSExport("apply", Length = 2)]
        public static JSValue Apply(in Arguments a){
            var ar = a.CopyForApply();
            return a.This.InvokeFunction(ar);
        }

        [JSPrototypeMethod][JSExport("bind", Length = 1)]
        public static JSValue Bind(in Arguments a) {
            var fOriginal = a.This as JSFunction;
            var original = a;
            var copy = a;
            var fx = new JSFunction((in Arguments a2) =>
            {
                return fOriginal.InvokeFunction(copy.CopyForBind(a2));
            })
            {
                // need to set prototypeChain...
                prototypeChain = (fOriginal as JSFunction).prototypeChain,
                prototype = (fOriginal as JSFunction).prototype,
                constructor = fOriginal.constructor
            };
            return fx;
        }

        [JSPrototypeMethod][JSExport("toString", Length = 0)]
        public new static JSValue ToString(in Arguments a)
        {
            if (!(a.This is JSFunction fx))
                throw JSContext.Current.NewTypeError($"Function.prototype.toString cannot be called with non function");
            var source = fx.source;
            if (source.IsEmpty)
            {
                return new JSString(string.Empty);
            }
            if (source.Source.Length != source.Length || source.Offset != 0)
            {
                source = source.Value;
            }
            return new JSString(source.Source);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static JSValue InvokeSuperConstructor(JSValue newTarget, JSValue super, in Arguments a)
        {
            var target = newTarget;

            var @this = a.This;
            var r = (super as JSFunction).CreateInstance(a.OverrideThis(a.This));
            return r.IsObject ? r : @this;
        }

        [JSExport(IsConstructor = true)]
        internal new static JSValue Constructor(in Arguments args)
        {

            var len = args.Length;
            if (len == 0) {
                return new JSFunction(JSFunction.empty, "anonymous", "function anonymous() {\n\n}");
            }
                
            JSValue body = null;
            var al = args.Length;
            var last = al - 1;
            var sargs = new List<string>();
            for (var ai = 0; ai < al; ai++)
            {
                var item = args.GetAt(ai);
                if (ai == last)
                {
                    body = item;
                }
                else
                {
                    sargs.Add(item.ToString());
                }
            }

            var bodyText = body is JSString @string ? @string.value : body.ToString();
            string location = null;
            var context = JSContext.Current;
            context.DispatchEvalEvent(ref bodyText, ref location);

            var fx = new JSFunction(JSFunction.empty, "internal", bodyText);


            // parse and create method...
            var fx1 = CoreScript.Compile(bodyText, "internal", sargs, codeCache: context.CodeCache);
            fx.f = fx1;
            return fx;
        }

        public override bool ConvertTo(Type type, out object value)
        {
            if (typeof(Delegate).IsAssignableFrom(type))
            {
                // create delegate....
                value = CreateClrDelegate(type, this);
                return true;
            }
            if(type.IsAssignableFrom(typeof(JSFunction)))
            {
                value = this;
                return true;
            }
            if(type == typeof(object))
            {
                value = this;
                return true;
            }
            return base.ConvertTo(type, out value);
        }
        static object CreateClrDelegate(Type type, JSFunction function)
        {
            var method = type.GetMethod("Invoke");
            var rt = method.ReturnType;
            var rtt = rt == typeof(void) ? typeof(object) : rt;
            var pa = method.GetParameters();
            var veList = new Sequence<ParameterExpression>(pa.Length + 1);
            var peList = new Sequence<ParameterExpression>(pa.Length);
            var stmts = new Sequence<Expression>();
            foreach (var p in method.GetParameters())
            {
                var inP = Expression.Parameter(p.ParameterType, p.Name);
                peList.Add(inP);

                var jsV = Expression.Parameter(typeof(JSValue), "js" + p.Name);
                veList.Add(jsV);

                stmts.Add(Expression.Assign(jsV, ClrProxyBuilder.Marshal(inP)));
            }
            // var retVar = Expression.Parameter(method.ReturnType == typeof(void) ? typeof(object) : method.ReturnType);
            // veList.Add(retVar);
            var @delegate = function.f;
            var d = Expression.Constant(@delegate);
            var @this = Expression.Constant(function);
            var nargs = ArgumentsBuilder.New(@this, veList.AsSequence<Expression>());

            if (rt == typeof(void) || rt == typeof(object))
            {
                stmts.Add(Expression.Invoke(d, nargs));
            } else
            {
                stmts.Add(JSValueToClrConverter.Get( Expression.Invoke(d, nargs), rt, ""));
            }
            // stmts.Add(JSValueToClrConverter.Coalesce(Expression.Invoke(d, nargs), rtt, retVar, ""));
            // stmts.Add(retVar);

            return Expression.Lambda( type, Expression.Block(veList, stmts), type.Name, peList.ToArray()).Compile();
        }
    }
}
