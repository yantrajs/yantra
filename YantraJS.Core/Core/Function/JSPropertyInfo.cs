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

namespace YantraJS.Core.Clr
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
            this.Property = property;
            var (name, export) = ClrTypeExtensions.GetJSName(namingConvention, property);
            this.Name = name;
            this.Export = export;
            this.PropertyType = property.PropertyType;
            this.GetMethod = property.GetMethod;
            this.SetMethod = property.SetMethod;
            this.CanRead = property.CanRead;
            this.CanWrite = property.CanWrite;
        }

        public JSFunction GetValue<TOwner, TValue>()
        {
            var name = $"get {Name}";
            var getter = (Func<TOwner, TValue>)Property.GetMethod.CreateDelegate(typeof(Func<TOwner, TValue>));
            var del = ClrProxy.GetDelegate<TValue>();
            return new JSFunction((in Arguments a) => {
                var owner = a.This;
                var value = getter((TOwner)owner.ForceConvert(typeof(TOwner)));
                return del(value);
            }, name);
        }

        public JSFunction GetStaticValue<TValue>()
        {
            var name = $"get {Name}";
            var getter = (Func<TValue>)Property.GetMethod.CreateDelegate(typeof(Func<TValue>));
            var del = ClrProxy.GetDelegate<TValue>();
            return new JSFunction( (in Arguments a) => {
                var value = getter();
                return del(value);
            }, name);
        }

        public JSFunction SetValue<TOwner, TValue>()
        {
            var name = $"set {Name}";
            var setter = (Action<TOwner, TValue>)Property.SetMethod.CreateDelegate(typeof(Action<TOwner, TValue>));
            return new JSFunction((in Arguments a) =>
            {
                var owner = a.This;
                var value = a.Get1();
                setter(
                    (TOwner)owner.ForceConvert(typeof(TOwner)),
                    (TValue)value.ForceConvert(typeof(TValue)));
                return JSUndefined.Value;
            }, name);
        }

        public JSFunction SetStaticValue<TValue>()
        {
            var name = $"set {Name}";
            var setter = (Action<TValue>)Property.SetMethod.CreateDelegate(typeof(Action<TValue>));
            return new JSFunction((in Arguments a) =>
            {
                var value = a.Get1();
                setter(
                    (TValue)value.ForceConvert(typeof(TValue)));
                return JSUndefined.Value;
            }, name);
        }

        internal JSFunction GeneratePropertySetter()
        {
            if (Property.SetMethod.IsStatic)
            {
                return this.InvokeAs(Property.PropertyType, SetStaticValue<object>);
            }
            // return (JSFunction)SetValueMethod
                // .MakeGenericMethod(property.DeclaringType, property.PropertyType).Invoke(null, new object[] { property });
            return this.InvokeAs(Property.DeclaringType, Property.PropertyType, SetValue<object,object>);
        }

        internal JSFunction GeneratePropertyGetter()
        {
            if (Property.GetMethod.IsStatic)
            {
                return this.InvokeAs(Property.PropertyType, GetStaticValue<object>);
            }
            // return (JSFunction)SetValueMethod
            // .MakeGenericMethod(property.DeclaringType, property.PropertyType).Invoke(null, new object[] { property });
            return this.InvokeAs(Property.DeclaringType, Property.PropertyType, GetValue<object, object>);
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
