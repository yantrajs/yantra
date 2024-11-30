using Expression = YantraJS.Expressions.YExpression;
using System.Reflection;
using YantraJS.ExpHelper;
using YantraJS.LinqExpressions;
using YantraJS.Runtime;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Core.Clr
{
    internal readonly struct JSFieldInfo
    {
        private readonly FieldInfo field;

        public readonly string Name;
        public readonly bool Export;

        public JSFieldInfo(ClrMemberNamingConvention namingConvention, FieldInfo field)
        {
            this.field = field;
            var (name, export) = ClrTypeExtensions.GetJSName(namingConvention, field);
            Name = name;
            Export = export;
        }

        public JSFunction GenerateFieldGetter()
        {
            var name = $"get {Name}";
            var field = this.field;
            return new JSFunction(() =>
            {
                var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
                Expression convertedThis = field.IsStatic
                    ? null
                    : JSValueToClrConverter.Get(ArgumentsBuilder.This(args), field.DeclaringType, "this");
                var body =
                    ClrProxyBuilder.Marshal(
                        Expression.Field(
                            convertedThis, field));
                var lambda = Expression.Lambda<JSFunctionDelegate>(name, body, args);
                return lambda.Compile();
            }, name);

        }

        public JSFunction GenerateFieldSetter()
        {
            var name = $"set {Name}";
            var field = this.field;
            return new JSFunction(() =>
            {
                var args = Expression.Parameter(typeof(Arguments).MakeByRefType());
                var a1 = ArgumentsBuilder.Get1(args);
                var convert = field.IsStatic
                    ? null
                    : JSValueToClrConverter.Get(ArgumentsBuilder.This(args), field.DeclaringType, "this");

                var clrArg1 = JSValueToClrConverter.Get(a1, field.FieldType, "value");


                var fieldExp = Expression.Field(convert, field);

                // todo
                // not working for `char`
                var assign = Expression.Assign(fieldExp, clrArg1).ToJSValue();

                var body = assign;
                var lambda = Expression.Lambda<JSFunctionDelegate>(name, body, args);
                return lambda.Compile();
            }, name);
        }
    }
}
