﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using YantraJS.Core.Clr;
using YantraJS.ExpHelper;
using YantraJS.Extensions;
using YantraJS.LinqExpressions;

namespace YantraJS.Core
{
    public partial class JSFunction : JSObject
    {

        internal static JSFunctionDelegate empty = (in Arguments a) => a.This;

        internal JSObject prototype;

        readonly string source;

        public readonly string name;

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
            this.source = source
                ?? $"function {type.name}() {{ [clr-native] }}";
            prototype = type.prototype;
            prototype[KeyStrings.constructor] = type;
            ownProperties[KeyStrings.prototype.Key] = JSProperty.Property(KeyStrings.prototype, prototype);

            this[KeyStrings.name] = name != null
                ? new JSString(name)
                : new JSString("native");
            this[KeyStrings.length] = new JSNumber(0);
        }

        protected JSFunction(string name, string source, JSObject _prototype)
            : base(JSContext.Current?.FunctionPrototype)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = empty;
            this.name = name ?? "native";
            this.source = source
                ?? $"function {name ?? "native"}() {{ [native] }}";
            prototype = _prototype;
            prototype[KeyStrings.constructor] = this;
            ownProperties[KeyStrings.prototype.Key] = JSProperty.Property(KeyStrings.prototype, prototype);

            this[KeyStrings.name] = name != null
                ? new JSString(name)
                : new JSString("native");
            this[KeyStrings.length] = new JSNumber(0);
        }

        public JSFunction(
            JSFunctionDelegate f,
            string name = null,
            string source = null,
            int length = 0): base(JSContext.Current?.FunctionPrototype)
        {
            ref var ownProperties = ref this.GetOwnProperties();
            this.f = f;
            this.name = name ?? "native";
            this.source = source 
                ?? $"function {name ?? "native"}() {{ [native] }}";
            prototype = new JSObject();
            prototype[KeyStrings.constructor] = this;
            ownProperties[KeyStrings.prototype.Key] = JSProperty.Property(KeyStrings.prototype, prototype);

            this[KeyStrings.name] = name != null
                ? new JSString(name)
                : new JSString("native");
            this[KeyStrings.length] = new JSNumber(length);

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

        public override string ToString()
        {
            return name;
        }

        public override string ToDetailString()
        {
            return this.source;
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            JSValue obj = new JSObject
            {
                prototypeChain = prototype
            };
            var a1 = a.OverrideThis(obj);
            var r = f(a1);
            if (!r.IsUndefined)
                return r;
            return obj;
        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            return f(a);
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
            var fx = new JSFunction((in Arguments a2) =>
            {
                if (a2.Length == 0)
                {
                    // for constructor...
                    return fOriginal.f(original.CopyForCall());
                }
                return fOriginal.f(a2.OverrideThis(original.Get1()));
            })
            {
                // need to set prototypeChain...
                prototypeChain = fOriginal
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