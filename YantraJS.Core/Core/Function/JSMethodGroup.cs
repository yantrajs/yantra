using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace YantraJS.Core.Clr
{
    internal class JSMethodGroup
    {
        public readonly string name;
        public readonly List<MethodInfo> Methods;
        public readonly MethodInfo JSMethod;
        private readonly Type type;
        private readonly (MethodInfo method, ParameterInfo[] parameters)[] all;

        public JSMethodGroup(ClrMemberNamingConvention namingConvention, Type type, IGrouping<string, MethodInfo> methods)
        {
            this.Methods = methods.ToList();
            this.JSMethod = this.Methods.FirstOrDefault(x => x.IsJSFunctionDelegate());

            var (n,e) = ClrTypeExtensions.GetJSName(namingConvention, this.JSMethod ?? this.Methods
                .OrderByDescending(x => x.GetCustomAttribute<JSExportAttribute>())
                .First());

            this.name = n;

            this.all = new (MethodInfo method, ParameterInfo[] parameters)[Methods.Count];
            for (int i = 0; i < this.all.Length; i++)
            {
                var m = this.Methods[i];
                this.all[i] = (m, m.GetParameters());
            }
            this.type = type;
        }

        internal JSValue Generate(bool isStatic)
        {
            return isStatic
                    ? new JSFunction(StaticInvoke, name)
                    : new JSFunction(Invoke, name);
        }

        private JSValue Invoke(in Arguments a)
        {
            if (!a.This.ConvertTo(type, out var target))
                throw JSContext.Current.NewTypeError($"{type.Name}.prototype.{name} called with object not of type {type.Name}");
            try
            {
                var (method, args) = all.Match(a, name);
                return ClrProxy.Marshal(method.Invoke(target, args));
            }
            catch (Exception ex)
            {
                throw JSException.From(ex);
            }
        }

        private JSValue StaticInvoke(in Arguments a)
        {
            try
            {
                var (method, args) = all.Match(a, name);
                return ClrProxy.Marshal(method.Invoke(null, args));
            }
            catch (Exception ex)
            {
                throw JSException.From(ex);
            }
        }

    }
}
