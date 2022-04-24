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
using YantraJS.Expressions;
using YantraJS.Core.LinqExpressions;

namespace YantraJS.ExpHelper
{
    public class JSObjectBuilder
    {
        readonly static Type type = typeof(JSObject);

        readonly static Type typeExtensions = typeof(JSObjectExtensions);

        readonly static ConstructorInfo _New = type.Constructor();

        //private static FieldInfo _ownProperties =
        //    type.InternalField(nameof(Core.JSObject.ownProperties));

        //readonly static PropertyInfo _Index =
        //    typeof(BaseMap<uint, Core.JSProperty>)
        //        .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
        //        .FirstOrDefault(x => x.GetIndexParameters().Length > 0);

        readonly static MethodInfo _FastAddSetterUInt =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.FastAddSetter), typeof(JSObject), typeof(uint), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddGetterUInt =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.FastAddGetter), typeof(JSObject), typeof(uint), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddSetterKeyString =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.FastAddSetter), typeof(JSObject), typeof(KeyString), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddGetterKeyString =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.FastAddGetter), typeof(JSObject), typeof(KeyString), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddSetterValue =
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.FastAddSetter), typeof(JSObject), typeof(JSValue), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddGetterValue=
            typeExtensions.PublicMethod(nameof(JSObjectExtensions.FastAddGetter), typeof(JSObject), typeof(JSValue), typeof(JSFunction), typeof(JSPropertyAttributes));


        readonly static MethodInfo _FastAddValueUInt =
            type.PublicMethod(nameof(JSObject.FastAddValue), typeof(uint), typeof(JSValue), typeof(JSPropertyAttributes));

        public readonly static MethodInfo _FastAddValueKeyString =
            type.PublicMethod(nameof(JSObject.FastAddValue), typeof(KeyString), typeof(JSValue), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddValueKeySymbol =
            type.PublicMethod(nameof(JSObject.FastAddValue), typeof(JSSymbol), typeof(JSValue), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddValueKeyValue =
            type.PublicMethod(nameof(JSObject.FastAddValue), typeof(JSValue), typeof(JSValue), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddPropertyUInt =
            type.PublicMethod(nameof(JSObject.FastAddProperty), typeof(uint), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddPropertyKeyString =
            type.PublicMethod(nameof(JSObject.FastAddProperty), typeof(KeyString), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddPropertySymbol =
            type.PublicMethod(nameof(JSObject.FastAddProperty), typeof(JSSymbol), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        readonly static MethodInfo _FastAddPropertyValue =
            type.PublicMethod(nameof(JSObject.FastAddProperty), typeof(JSValue), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        public readonly static MethodInfo _FastAddRange =
            type.PublicMethod(nameof(JSObject.FastAddRange), typeof(JSValue));


        readonly static MethodInfo _NewWithProperties =
            type.PublicMethod(nameof(JSObject.NewWithProperties));

        readonly static MethodInfo _NewWithElements =
            type.PublicMethod(nameof(JSObject.NewWithElements));

        readonly static MethodInfo _NewWithPropertiesAndElements =
            type.PublicMethod(nameof(JSObject.NewWithPropertiesAndElements));

        //readonly static MethodInfo _AddElement =
        //    type.PublicMethod(nameof(JSObject.FastAddValue), new Type[] { typeof(uint), typeof(JSValue) });

        //readonly static MethodInfo _Spread =
        //    type.PublicMethod(nameof(JSObject.Merge), new Type[] { typeof(JSValue) });

        //readonly static MethodInfo _AddValueString =
        //    typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), KeyStringsBuilder.RefType, typeof(JSValue), typeof(JSPropertyAttributes) );

        //readonly static MethodInfo _AddValueUInt =
        //    typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(uint), typeof(JSValue), typeof(JSPropertyAttributes));

        //readonly static MethodInfo _AddValue =
        //    typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(JSValue), typeof(JSValue), typeof(JSPropertyAttributes));

        //readonly static MethodInfo _AddPropertyString =
        //    typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        //readonly static MethodInfo _AddPropertyUInt =
        //    typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(uint), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));

        //readonly static MethodInfo _AddProperty =
        //    typeExtensions.PublicMethod(nameof(JSObjectExtensions.AddProperty), typeof(JSObject), typeof(JSValue), typeof(JSFunction), typeof(JSFunction), typeof(JSPropertyAttributes));


        //readonly static MethodInfo _AddExpressionProperty =
        //    type.PublicMethod(nameof(JSObject.AddProperty), new Type[] { typeof(JSValue), typeof(JSValue) });

        //readonly static MethodInfo _AddPropertyAccessors =
        //    type.PublicMethod(nameof(JSObject.AddProperty), new Type[] { KeyStringsBuilder.RefType, typeof(JSFunction), typeof(JSFunction) });


        public static YElementInit AddValue(
            Expression key, 
            Expression value,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableValue)
        {
            if(key.Type.IsJSValueType())
            {
                return new YElementInit(_FastAddValueKeyValue, key, value, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValue, target, key, value, Expression.Constant(attributes));
            }
            if(key.Type== typeof(uint))
            {
                return new YElementInit(_FastAddValueUInt, key, value, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValueUInt, target, key, value, Expression.Constant(attributes));
            }
            if (key.Type == typeof(int))
            {
                return new YElementInit(_FastAddValueUInt, Expression.Convert(key, typeof(uint)), value, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValueUInt, target, Expression.Convert(key, typeof(uint)), value, Expression.Constant(attributes));
            }
            return new YElementInit(_FastAddValueKeyString, key, value, Expression.Constant(attributes));
            // return Expression.Call(null, _AddValueString, target, key, value, Expression.Constant(attributes));
        }

        public static YElementInit AddSetter(
            // Expression target, 
            Expression key, 
            Expression setter, 
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            if (key.Type.IsJSValueType())
            {
                return new YElementInit(_FastAddSetterValue, key, setter, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValue, target, key, value, Expression.Constant(attributes));
            }
            if (key.Type == typeof(uint))
            {
                return new YElementInit(_FastAddSetterUInt, key, setter, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValueUInt, target, key, value, Expression.Constant(attributes));
            }
            if (key.Type == typeof(int))
            {
                return new YElementInit(_FastAddSetterUInt, Expression.Convert(key, typeof(uint)), setter, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValueUInt, target, Expression.Convert(key, typeof(uint)), value, Expression.Constant(attributes));
            }
            return new YElementInit(_FastAddSetterKeyString, key, setter, Expression.Constant(attributes));
        }

        public static YElementInit AddGetter(
            // Expression target, 
            Expression key,
            Expression getter,
            JSPropertyAttributes attributes = JSPropertyAttributes.EnumerableConfigurableProperty)
        {
            if (key.Type.IsJSValueType())
            {
                return new YElementInit(_FastAddGetterValue, key, getter, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValue, target, key, value, Expression.Constant(attributes));
            }
            if (key.Type == typeof(uint))
            {
                return new YElementInit(_FastAddGetterUInt, key, getter, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValueUInt, target, key, value, Expression.Constant(attributes));
            }
            if (key.Type == typeof(int))
            {
                return new YElementInit(_FastAddGetterUInt, Expression.Convert(key, typeof(uint)), getter, Expression.Constant(attributes));
                // return Expression.Call(null, _AddValueUInt, target, Expression.Convert(key, typeof(uint)), value, Expression.Constant(attributes));
            }
            return new YElementInit(_FastAddGetterKeyString, key, getter, Expression.Constant(attributes));
        }

        public static Expression New()
        {
            return Expression.New(_New);
        }


        public static Expression New(IFastEnumerable<YElementInit> elements)
        {
            return YExpression.ListInit(Expression.New(_New), elements);
        }


        public static Expression New(IList<ExpressionHolder> keyValues)
        {
            var list = new Sequence<YElementInit>();

            foreach(var v in keyValues)
            {
                if (v.Spread)
                {
                    list.Add(YExpression.ElementInit(_FastAddRange, v.Value));
                    continue;
                }
                if(v.Key.Type == typeof(uint))
                {
                    if(v.Value != null)
                    {
                        list.Add(YExpression.ElementInit(_FastAddValueUInt, v.Key, v.Value, JSPropertyAttributesBuilder.EnumerableConfigurableValue ));
                        continue;
                    }
                    list.Add(YExpression.ElementInit(_FastAddPropertyUInt, v.Key, v.Getter, v.Setter, JSPropertyAttributesBuilder.EnumerableConfigurableProperty));
                    continue;
                }
                if(v.Key.Type == typeof(KeyString))
                {
                    if (v.Value != null)
                    {
                        list.Add(YExpression.ElementInit(_FastAddValueKeyString, v.Key, v.Value, JSPropertyAttributesBuilder.EnumerableConfigurableValue));
                        continue;
                    }
                    list.Add(YExpression.ElementInit(_FastAddPropertyKeyString, v.Key, v.Getter, v.Setter, JSPropertyAttributesBuilder.EnumerableConfigurableProperty));
                    continue;
                }
                if (v.Value != null)
                {
                    list.Add(YExpression.ElementInit(_FastAddValueKeyValue, v.Key, v.Value, JSPropertyAttributesBuilder.EnumerableConfigurableValue));
                    continue;
                }
                list.Add(YExpression.ElementInit(_FastAddPropertyValue, v.Key, v.Getter, v.Setter, JSPropertyAttributesBuilder.EnumerableConfigurableProperty));
            }

            return YExpression.ListInit(YExpression.New(_New), list);

            // let the default be NewWithProperties
            //bool addProperties = false;
            //bool addElements = false;
            //// choose best create method...
            //foreach (var px in keyValues)
            //{
            //    if (px.Spread)
            //    {
            //        addElements = true;
            //        addProperties = true;
            //        break;
            //    }
            //    if (px.Key.Type == typeof(uint))
            //    {
            //        addElements = true;
            //        continue;
            //    }
            //    if (px.Key.Type == typeof(KeyString))
            //    {
            //        addProperties = true;
            //        continue;
            //    }
            //}

            //MethodInfo _new;
            //if (addProperties && addElements)
            //{
            //    _new = _NewWithPropertiesAndElements;
            //}
            //else
            //{
            //    if (addProperties)
            //    {
            //        _new = _NewWithProperties;
            //    }
            //    else
            //    {
            //        _new = _NewWithElements;
            //    }
            //}

            //Expression _newObj = Expression.Call(null, _new);

            //foreach (var px in keyValues)
            //{
            //    if (px.Spread)
            //    {
            //        _newObj = Expression.Call(_newObj, _Spread, px.Value);
            //        continue;
            //    }
            //    if (px.Key.Type == typeof(uint))
            //    {
            //        if (px.Value != null)
            //        {
            //            _newObj = Expression.Call(null, _AddValueUInt, _newObj, px.Key, px.Value);
            //        } else
            //        {
            //            _newObj = Expression.Call(null, _AddPropertyUInt,_newObj, px.Key, px.Getter, px.Setter);
            //        }
            //        continue;
            //    }
            //    if (px.Key.Type == typeof(KeyString))
            //    {
            //        if (px.Value != null)
            //        {
            //            _newObj = Expression.Call(null, _AddValueString, _newObj, px.Key, px.Value);
            //        }
            //        else
            //        {
            //            _newObj = Expression.Call(null, _AddPropertyString, _newObj, px.Key, px.Getter, px.Setter);
            //        }
            //        continue;
            //    }
            //    if (px.Value != null)
            //    {
            //        _newObj = Expression.Call(null, _AddValue, _newObj, px.Key, px.Value);
            //    }
            //    else
            //    {
            //        _newObj = Expression.Call(null, _AddProperty, _newObj, px.Key, px.Getter, px.Setter);
            //    }
            //}
            //return _newObj;
        }

    }
}
