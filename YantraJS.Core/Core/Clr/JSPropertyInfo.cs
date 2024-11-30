using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

using System;
using System.Reflection;
using System.Text;
using YantraJS.ExpHelper;
using YantraJS.LinqExpressions;
using YantraJS.Runtime;
using static YantraJS.Core.Clr.ClrType;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Core.Clr
{

    internal class JSPropertyInfo
    {
        public readonly PropertyInfo Property;
        public readonly string Name;
        public readonly bool Export;

        public readonly Type PropertyType;
        public readonly MethodInfo GetMethod;
        public readonly MethodInfo SetMethod;

        public readonly bool CanRead;
        public readonly bool CanWrite;

        public JSPropertyInfo(ClrMemberNamingConvention namingConvention, PropertyInfo property)
        {
            Property = property;
            var (name, export) = ClrTypeExtensions.GetJSName(namingConvention, property);
            Name = name;
            Export = export;
            PropertyType = property.PropertyType;
            GetMethod = property.GetMethod;
            SetMethod = property.SetMethod;
            CanRead = property.CanRead;
            CanWrite = property.CanWrite;
        }

        public JSFunction GeneratePropertyGetter()
        {
            var name = $"get {Name}";
            return new JSFunction(Property.GetMethod.CompileToJSFunctionDelegate(name), name);
        }

        public JSFunction GeneratePropertySetter()
        {
            var name = $"set {Name}";
            return new JSFunction(Property.SetMethod.CompileToJSFunctionDelegate(name), name);
        }

        internal Func<object, uint, JSValue> GenerateIndexedGetter()
        {
            var @this = Expression.Parameter(typeof(object));
            var index = Expression.Parameter(typeof(uint));
            var indexParameter = Property.GetMethod.GetParameters()[0];
            Expression indexAccess = index.Type != indexParameter.ParameterType
                ? Expression.Convert(index, indexParameter.ParameterType)
                : index as Expression;
            Expression indexExpression;
            Expression convertThis = Expression.TypeAs(@this, Property.DeclaringType);
            if (Property.DeclaringType.IsArray)
            {
                // this is direct array.. cast and get.. 
                indexExpression = Expression.ArrayIndex(convertThis, indexAccess);
            }
            else
            {
                indexExpression = Expression.MakeIndex(convertThis, Property, new Expression[] { indexAccess });
            }
            Expression body = JSExceptionBuilder.Wrap(ClrProxyBuilder.Marshal(indexExpression));
            var lambda = Expression.Lambda<Func<object, uint, JSValue>>($"set {Property.Name}", body, @this, index);
            return lambda.Compile();
        }

        internal Func<object, uint, object, JSValue> GenerateIndexedSetter()
        {
            if (!Property.CanWrite)
                return null;

            var type = Property.DeclaringType;
            var elementType = type.GetElementTypeOrGeneric() ?? Property.PropertyType;

            var @this = Expression.Parameter(typeof(object));
            var index = Expression.Parameter(typeof(uint));
            var value = Expression.Parameter(typeof(object));
            var indexParameter = Property.SetMethod.GetParameters()[0];
            Expression indexAccess = index.Type != indexParameter.ParameterType
                ? Expression.Convert(index, indexParameter.ParameterType)
                : index as Expression;
            Expression indexExpression;
            Expression convertThis = Expression.TypeAs(@this, Property.DeclaringType);
            if (Property.DeclaringType.IsArray)
            {
                // this is direct array.. cast and get.. 
                indexExpression = Expression.ArrayIndex(convertThis, indexAccess);
            }
            else
            {
                indexExpression = Expression.MakeIndex(convertThis, Property, new Expression[] { indexAccess });
            }


            Expression body = Expression.Block(
                JSExceptionBuilder.Wrap(
                    Expression.Assign(indexExpression, Expression.TypeAs(value, elementType)).ToJSValue()));
            var lambda = Expression.Lambda<Func<object, uint, object, JSValue>>("get " + Property.Name, body, @this, index, value);
            return lambda.Compile();
        }

    }
}
