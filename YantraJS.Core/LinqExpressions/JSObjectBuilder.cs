using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YantraJS.Core;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

namespace YantraJS.ExpHelper
{
    public class JSObjectBuilder
    {
        readonly static Type type = typeof(JSObject);

        readonly static Type typeExtensions = typeof(JSObjectExtensions);

        //private static FieldInfo _ownProperties =
        //    type.InternalField(nameof(Core.JSObject.ownProperties));

        //readonly static PropertyInfo _Index =
        //    typeof(BaseMap<uint, Core.JSProperty>)
        //        .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
        //        .FirstOrDefault(x => x.GetIndexParameters().Length > 0);

        readonly static MethodInfo _NewWithProperties =
            type.PublicMethod(nameof(JSObject.NewWithProperties));

        readonly static MethodInfo _NewWithElements =
            type.PublicMethod(nameof(JSObject.NewWithElements));

        readonly static MethodInfo _NewWithPropertiesAndElements =
            type.PublicMethod(nameof(JSObject.NewWithPropertiesAndElements));

        readonly static MethodInfo _AddElement =
            type.PublicMethod(nameof(JSObject.AddElement), new Type[] { typeof(uint), typeof(JSValue) });

        readonly static MethodInfo _Spread =
            type.PublicMethod(nameof(JSObject.Merge), new Type[] { typeof(JSValue) });

        readonly static MethodInfo _AddValueString =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), KeyStringsBuilder.RefType, typeof(JSValue), typeof(JSPropertyAttributes) );

        readonly static MethodInfo _AddValueUInt =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(uint), typeof(JSValue), typeof(JSPropertyAttributes));

        readonly static MethodInfo _AddValue =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(JSValue), typeof(JSValue), typeof(JSPropertyAttributes));

        readonly static MethodInfo _AddPropertyString =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _AddPropertyUInt =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(uint), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _AddProperty =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(JSValue), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));


        //readonly static MethodInfo _AddExpressionProperty =
        //    type.PublicMethod(nameof(JSObject.AddProperty), new Type[] { typeof(JSValue), typeof(JSValue) });

        //readonly static MethodInfo _AddPropertyAccessors =
        //    type.PublicMethod(nameof(JSObject.AddProperty), new Type[] { KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction) });


        public static Expression AddValue(
            Expression target, 
            Expression key, 
            Expression value,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            if(key.Type == typeof(JSValue))
            {
                return Expression.Call(null, _AddValue, target, key, value, Expression.Constant(attributes));
            }
            if(key.Type== typeof(uint))
            {
                return Expression.Call(null, _AddValueUInt, target, key, value, Expression.Constant(attributes));
            }
            if (key.Type == typeof(int))
            {
                return Expression.Call(null, _AddValueUInt, target, Expression.Convert(key, typeof(uint)), value, Expression.Constant(attributes));
            }
            return Expression.Call(null, _AddValueString, target, key, value, Expression.Constant(attributes));
        }

        public static Expression AddProperty(
            Expression target, 
            Expression key, 
            Expression getter, 
            Expression setter, 
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            if (key.Type == typeof(JSValue))
            {
                return Expression.Call(null, _AddProperty, target, key, getter, setter, Expression.Constant(attributes));
            }
            if (key.Type == typeof(uint))
            {
                return Expression.Call(null, _AddPropertyUInt, target, key, getter, setter, Expression.Constant(attributes));
            }
            if (key.Type == typeof(int))
            {
                return Expression.Call(null, _AddPropertyUInt, target, Expression.Convert(key, typeof(uint)), getter, setter, Expression.Constant(attributes));
            }
            return Expression.Call(null, _AddPropertyString, target, key, getter, setter, Expression.Constant(attributes));
        }


        public static Expression New(IList<ExpressionHolder> keyValues)
        {
            // let the default be NewWithProperties
            bool addProperties = false;
            bool addElements = false;
            // choose best create method...
            foreach (var px in keyValues)
            {
                if (px.Spread)
                {
                    addElements = true;
                    addProperties = true;
                    break;
                }
                if (px.Key.Type == typeof(uint))
                {
                    addElements = true;
                    continue;
                }
                if (px.Key.Type == typeof(KeyString))
                {
                    addProperties = true;
                    continue;
                }
            }

            MethodInfo _new;
            if (addProperties && addElements)
            {
                _new = _NewWithPropertiesAndElements;
            }
            else
            {
                if (addProperties)
                {
                    _new = _NewWithProperties;
                }
                else
                {
                    _new = _NewWithElements;
                }
            }

            Expression _newObj = Expression.Call(null, _new);

            foreach (var px in keyValues)
            {
                if (px.Spread)
                {
                    _newObj = Expression.Call(_newObj, _Spread, px.Value);
                    continue;
                }
                if (px.Key.Type == typeof(uint))
                {
                    if (px.Value != null)
                    {
                        _newObj = Expression.Call(null, _AddValueUInt, _newObj, px.Key, px.Value);
                    } else
                    {
                        _newObj = Expression.Call(null, _AddPropertyUInt,_newObj, px.Key, px.Getter, px.Setter);
                    }
                    continue;
                }
                if (px.Key.Type == typeof(KeyString))
                {
                    if (px.Value != null)
                    {
                        _newObj = Expression.Call(null, _AddValueString, _newObj, px.Key, px.Value);
                    }
                    else
                    {
                        _newObj = Expression.Call(null, _AddPropertyString, _newObj, px.Key, px.Getter, px.Setter);
                    }
                    continue;
                }
                if (px.Value != null)
                {
                    _newObj = Expression.Call(null, _AddValue, _newObj, px.Key, px.Value);
                }
                else
                {
                    _newObj = Expression.Call(null, _AddProperty, _newObj, px.Key, px.Getter, px.Setter);
                }
            }
            return _newObj;
        }

    }
}
