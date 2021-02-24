using System;
using System.Collections.Generic;
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

namespace YantraJS.Core
{
    public class JSClassFunction: JSFunction
    {

        public JSClassFunction(
            JSFunctionDelegate @delegate,
            in StringSpan name,
            in StringSpan source,
            int length = 0) : base(@delegate, name, source, length)
        {

        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this.name} cannot be invoked directly");
        }
    }

    public class JSClosureFunction: JSFunction
    {
        private readonly ScriptInfo script;
        private readonly JSVariable[] closures;
        internal readonly JSClosureFunctionDelegate cf;

        public JSClosureFunction(
            ScriptInfo script,
            JSVariable[] closures,
            JSClosureFunctionDelegate cf,
            in StringSpan name,
            in StringSpan source,
            int length = 0): base(Closure(script, closures,cf), name, source, length)
        {
            this.script = script;
            this.closures = closures;
            this.cf = cf;
        }

        private static JSFunctionDelegate Closure(ScriptInfo script, JSVariable[] closures, JSClosureFunctionDelegate cf)
        {
            return (in Arguments a) => {
                return cf(script, closures, in a);
            };
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            JSValue obj = new JSObject
            {
                BasePrototypeObject = prototype
            };
            var a1 = a.OverrideThis(obj, this);
            var r = cf(script, closures, in a1);
            if (!r.IsUndefined)
                return r;
            return obj;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return cf(script, closures, in a);
        }
    }


    public partial class JSFunction : JSObject
    {

        internal static JSFunctionDelegate empty = (in Arguments a) => a.This;

        internal static JSClosureFunctionDelegate emptyCF = (ScriptInfo s, JSVariable[] closures, in Arguments a) => a.This;

        internal JSObject prototype;

        readonly StringSpan source;

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
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = clrDelegate;
            this.name = "clr-native";
            this.source = source.IsEmpty 
                ? $"function {type.name}() {{ [clr-native] }}"
                : source;
            prototype = type.prototype;
            prototype[KeyStrings.constructor] = type;
            ownProperties[KeyStrings.prototype.Key] = JSProperty.Property(KeyStrings.prototype, prototype);

            this[KeyStrings.name] = name.IsEmpty
                ? new JSString("native")
                : new JSString(name);
            this[KeyStrings.length] = new JSNumber(0);
            constructor = this;
        }

        internal void Seal()
        {
            ref var ownProperties = ref this.GetOwnProperties();
            for (int i = 0; i < ownProperties.properties.Length; i++)
            {
                ref var p = ref ownProperties.properties[i];
                if (p.IsValue)
                {
                    ownProperties.properties[i] = new JSProperty(p.key, p.get, p.set, p.value, JSPropertyAttributes.ReadonlyValue);
                }
            }
        }

        protected JSFunction(StringSpan name, StringSpan source, JSObject _prototype)
            : base(JSContext.Current?.FunctionPrototype)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = empty;
            this.name = name.IsEmpty ? "native" : name;
            this.source = source.IsEmpty 
                ? $"function {this.name}() {{ [native] }}"
                : source;
            prototype = _prototype;
            prototype[KeyStrings.constructor] = this;
            ownProperties[KeyStrings.prototype.Key] = JSProperty.Property(KeyStrings.prototype, prototype);

            this[KeyStrings.name] = name.IsEmpty 
                ? new JSString("native")
                : new JSString(name);
            this[KeyStrings.length] = new JSNumber(0);
            constructor = this;
        }

        public JSFunction(JSFunctionDelegate f)
            : this(f, StringSpan.Empty, StringSpan.Empty)
        {

        }

        public JSFunction(
            JSFunctionDelegate f,
            in StringSpan name,
            int length = 0): this(f, name, StringSpan.Empty, length)
        {

        }

        public JSFunction(
            JSFunctionDelegate f,
            in StringSpan name,
            in StringSpan source,
            int length = 0): base(JSContext.Current?.FunctionPrototype)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = f;
            this.name = name.IsEmpty ? "native" : name;
            this.source = source.IsEmpty
                ? $"function {this.name}() {{ [native] }}"
                : source;
            prototype = new JSObject();
            // prototype[KeyStrings.constructor] = this;
            prototype.DefineProperty(KeyStrings.constructor, JSProperty.Property(this, JSPropertyAttributes.ConfigurableValue));
            // ref var opp = ref prototype.GetOwnProperties(true);
            // opp[KeyStrings.constructor.Key] = JSProperty.Property(this, JSPropertyAttributes.ConfigurableReadonlyValue);
            ownProperties[KeyStrings.prototype.Key] = JSProperty.Property(KeyStrings.prototype, prototype);

            this[KeyStrings.name] = name.IsEmpty
                ? new JSString("native")
                : new JSString(name);
            this[KeyStrings.length] = new JSNumber(length);
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
            JSValue obj = new JSObject
            {
                BasePrototypeObject = prototype
            };
            var a1 = a.OverrideThis(obj, constructor);
            var r = f(a1);
            if (!r.IsUndefined)
                return r;
            return obj;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return f(a);
        }

        [Prototype("valueOf", Length = 1)]
        public static JSValue ValueOf(in Arguments a)
        {
            return a.This;
        }

        [Prototype("toString")]
        public static JSValue ToString(in Arguments a)
        {
            var f = a.This as JSFunction;
            return new JSString(f.source);
        }

        public override string ToString()
        {
            return source.Value;
        }

        [Prototype("call", Length = 1)]
        public static JSValue Call(in Arguments a)
        {
            var a1 = a.CopyForCall();
            return a.This.InvokeFunction(a1);
        }

        [Prototype("apply", Length = 2)]
        public static JSValue Apply(in Arguments a){
            var ar = a.CopyForApply();
            return a.This.InvokeFunction(ar);
        }

        [Prototype("bind", Length = 1)]
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

        internal static JSValue InvokeSuperConstructor(JSValue super, in Arguments a)
        {
            var @this = a.This;
            var r = (super as JSFunction).f(a);
            return r.IsUndefined ? @this : r;
        }

        [Constructor]
        internal static JSValue Constructor(in Arguments args)
        {
            var len = args.Length;
            if (len == 0)
                throw JSContext.Current.NewTypeError("No arguments were supplied to Function constructor");
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
            var fx = new JSFunction(JSFunction.empty, "internal", bodyText);


            // parse and create method...
            var fx1 = CoreScript.Compile(bodyText, "internal", sargs);
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
            var veList = new List<ParameterExpression>(pa.Length + 1);
            var peList = new List<ParameterExpression>(pa.Length);
            var stmts = new List<Expression>();
            foreach (var p in method.GetParameters())
            {
                var inP = Expression.Parameter(p.ParameterType, p.Name);
                peList.Add(inP);

                var jsV = Expression.Parameter(typeof(JSValue), "js" + p.Name);
                veList.Add(jsV);

                stmts.Add(Expression.Assign(jsV, ClrProxyBuilder.Marshal(inP)));
            }
            var retVar = Expression.Parameter(method.ReturnType == typeof(void) ? typeof(object) : method.ReturnType);
            veList.Add(retVar);
            var @delegate = function.f;
            var d = Expression.Constant(@delegate);
            var @this = Expression.Constant(function);
            var nargs = ArgumentsBuilder.New(@this, veList.ToList<Expression>());


            stmts.Add(JSValueBuilder.Coalesce(Expression.Invoke(d, nargs), rtt, retVar, ""));
            stmts.Add(retVar);

            return Expression.Lambda(Expression.Block(veList, stmts), peList).Compile();
        }
    }
}
