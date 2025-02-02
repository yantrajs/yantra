using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using YantraJS.Core;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Core.FastParser;
using YantraJS.Expressions;
using YantraJS.Core.LambdaGen;
using YantraJS.Core.Core;

namespace YantraJS.ExpHelper
{


    public class JSValueBuilder
    {
        private static readonly Type type = typeof(JSValue);

        //private static MethodInfo _EqualsLiteralDouble
        //    = type.PublicMethod(nameof(JSValue.EqualsLiteral), typeof(double));

        //private static MethodInfo _EqualsLiteralString
        //    = type.PublicMethod(nameof(JSValue.EqualsLiteral), typeof(string));
        //private static MethodInfo _StrictEqualsLiteralDouble
        //    = type.PublicMethod(nameof(JSValue.StrictEqualsLiteral), typeof(double));
        //private static MethodInfo _StrictEqualsLiteralString
        //    = type.PublicMethod(nameof(JSValue.StrictEqualsLiteral), typeof(string));

        //private static MethodInfo _Negate
        //    = type.PublicMethod(nameof(JSValue.Negate));

        public static Expression AddString(Expression target, Expression @string)
        {
            // return Expression.Call(target, _AddString, @string);
            return target.CallExpression<JSValue, string, JSValue>(() => (x, a) => x.AddValue(a), @string);
        }
        public static Expression AddDouble(Expression target, Expression @double)
        {
            // return Expression.Call(target, _AddDouble, @double);
            return target.CallExpression<JSValue, double, JSValue>(() => (x, a) => x.AddValue(a), @double);
        }

        public static Expression ToKey(Expression exp)
        {
            // return Expression.Call(exp, _ToKey, Expression.Constant(true));
            return exp.CallExpression<JSValue, PropertyKey>(() => (x) => x.ToKey(true), Expression.Constant(true));
        }

        public static Expression IsNumber(Expression exp)
        {
            return exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsNumber);
        }

        public static Expression IsString(Expression exp)
        {
            return exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsString);
        }

        public static Expression IsBoolean(Expression exp)
        {
            return exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsBoolean);
        }

        public static Expression IsSymbol(Expression exp)
        {
            return exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsSymbol);
        }

        public static Expression IsFunction(Expression exp)
        {
            return exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsFunction);
        }

        public static Expression IsObjectType(Expression exp)
        {
            // return Expression.And(Expression.Property(exp, _IsObject), Expression.Not(Expression.Property(exp, _IsFunction)));

            return Expression.And(
                exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsObject),
                Expression.Not(exp.PropertyExpression<JSValue, bool>(() => (x) => x.IsFunction))
                );
        }

        public static Expression IsNullOrUndefined(Expression target)
        {
            if (target.Type == typeof(JSVariable))
                target = JSVariable.ValueExpression(target);
            // return Expression.Property(target, _IsNullOrUndefined);
            return target.PropertyExpression<JSValue, bool>(() => (x) => x.IsNullOrUndefined);
        }

        private static PropertyInfo _lengthProperty
            = type.Property(nameof(Core.JSValue.Length));

        public static Expression Length(Expression target)
        {
            return Expression.Property(target, _lengthProperty);
        }

        public static Expression DoubleValue(Expression exp)
        {
            // return Expression.Property(exp, _DoubleValue);
            return exp.PropertyExpression<JSValue, double>(() => (x) => x.DoubleValue);
        }

        public static Expression IntValue(Expression exp)
        {
            // return Expression.Property(exp, _IntValue);
            return exp.PropertyExpression<JSValue, int>(() => (x) => x.IntValue);
        }

        public static Expression BigIntValue(Expression exp)
        {
            // return Expression.Property(exp, _BigIntValue);
            return exp.PropertyExpression<JSValue, long>(() => (x) => x.BigIntValue);
        }

        public static Expression UIntValue(Expression exp)
        {
            /// return Expression.Property(exp, _UIntValue);
            return exp.PropertyExpression<JSValue, uint>(() => (x) => x.UIntValue);
        }

        public static Expression PrototypeChain(Expression exp)
        {
            return exp
                .FieldExpression<JSValue, JSPrototype>(() => (x) => x.prototypeChain)
                .FieldExpression<JSPrototype, JSObject>(() => (x) => x.@object);
        }

        public static Expression Negate(Expression exp)
        {
            // return Expression.Call(exp, _Negate);
            return exp.CallExpression<JSValue, JSValue>(() => (x) => x.Negate());
        }

        public static Expression Power(Expression left,Expression right) {
            // return Expression.Call(left, _Power, right);
            return left.CallExpression<JSValue, JSValue, JSValue>(() => (x, a) => x.Power(a), right);
        }

        //private static PropertyInfo _BooleanValue =
        //    type.Property(nameof(JSValue.BooleanValue));
        public static Expression BooleanValue(Expression exp)
        {
            if(exp.NodeType == Expressions.YExpressionType.Conditional && exp is YConditionalExpression ce)
            {
                if (ce.@true == JSBooleanBuilder.True && ce.@false == JSBooleanBuilder.False)
                    return ce.test;
                if (ce.@true == JSBooleanBuilder.False && ce.@false == JSBooleanBuilder.True)
                    return Expression.Not( ce.test);
            }
            if (exp == JSBooleanBuilder.True)
            {
                return YExpression.Constant(true);
            }
            if (exp == JSBooleanBuilder.False)
            {
                return YExpression.Constant(false);
            }
            // return Expression.Property(exp, _BooleanValue);
            return exp.PropertyExpression<JSValue, bool>(() => (x) => x.BooleanValue);
        }


        //private static MethodInfo _Add =
        //    type.InternalMethod(nameof(Core.JSValue.AddValue), typeof(JSValue));

        public static Expression Add(Expression target, Expression value)
        {
            // return Expression.Call(target, _Add, value);
            return target.CallExpression<JSValue, JSValue, JSValue>(() => (x, a) => x.AddValue(a), value);
        }

        private static MethodInfo _TypeOf =
            type.GetMethod(nameof(JSValue.TypeOf));

        public static Expression TypeOf(Expression target)
        {
            return Expression.Call(target, _TypeOf);
        }

        private static PropertyInfo _IndexKeyString =
                type.IndexProperty(typeof(KeyString));

        private static PropertyInfo _IndexUInt =
                type.IndexProperty(typeof(uint));

        private static PropertyInfo _Index =
                type.IndexProperty(typeof(JSValue));

        private static PropertyInfo _SuperIndexKeyString =
                    type.PublicIndex(typeof(JSObject), typeof(KeyString));

        private static PropertyInfo _SuperIndexUInt =
                    type.PublicIndex(typeof(JSObject), typeof(uint));

        private static PropertyInfo _SuperIndex =
                    type.PublicIndex(typeof(JSObject), typeof(JSValue));

        private static MethodInfo _PropertyOrUndefinedKeyString =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), KeyStringsBuilder.RefType);
        private static MethodInfo _PropertyOrUndefinedUInt =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(uint));
        private static MethodInfo _PropertyOrUndefined =
            type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSValue));
        //private static MethodInfo _SuperPropertyOrUndefinedKeyString =
        //    type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSObject), KeyStringsBuilder.RefType);
        //private static MethodInfo _SuperPropertyOrUndefinedUInt =
        //    type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSObject), typeof(uint));
        //private static MethodInfo _SuperPropertyOrUndefined =
        //    type.PublicMethod(nameof(JSValue.PropertyOrUndefined), typeof(JSObject), typeof(JSValue));

        //private static MethodInfo _GetOwnPropertyKeyString =
        //    type.PublicMethod(nameof(JSValue.GetOwnProperty), KeyStringsBuilder.RefType);

        //private static MethodInfo _GetOwnPropertyUInt =
        //    type.PublicMethod(nameof(JSValue.GetOwnProperty), typeof(uint));

        //private static MethodInfo _GetOwnProperty =
        //    type.PublicMethod(nameof(JSValue.GetOwnProperty), typeof(JSValue));


        public static Expression InvokeMethod(
            Expression targetTemp,
            Expression methodTemp, 
            Expression target, Expression name, IFastEnumerable<Expression> args, bool spread, bool coalesce)
        {
            if (!coalesce)
            {
                return JSValueExtensionsBuilder.InvokeMethod(target, name, args, spread);
            }


            var method = _Index;
            if (name.Type == typeof(KeyString))
            {
                method = _IndexKeyString;
            }
            else if (name.Type == typeof(uint))
            {
                method = _IndexUInt;
            }
            else if (name.Type == typeof(int))
            {
                method = _IndexUInt;
                name = Expression.Convert(name, typeof(uint));
            }

            return Expression.Block(
                Expression.Assign(targetTemp, target),
                Expression.Assign(methodTemp, Expression.MakeIndex(targetTemp, method, name)),
                Expression.Condition(
                    JSValueBuilder.IsNullOrUndefined(methodTemp),
                        JSUndefinedBuilder.Value,
                        JSFunctionBuilder.InvokeFunction(methodTemp, ArgumentsBuilder.New(targetTemp, args, spread))
                    )
                );
        }

        public static Expression Index(Expression target, Expression super, uint i, bool coalesce = false)
        {
            if (super == null)
            {
                return Index(target, i, coalesce);
            }
            return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, Expression.Constant(i) });
        }


        public static Expression Index(Expression target, uint i, bool coalesce = false)
        {

            return Expression.MakeIndex(target, _IndexUInt, new Expression[] { Expression.Constant(i) });
        }


        public static Expression Index(Expression target, Expression super, Expression property, bool coalesce = false)
        {
            if (super == null)
            {
                return Index(target, property, coalesce);
            }
            if (property.Type == typeof(KeyString))
            {
                //if (coalesce)
                //{
                //    return Expression.Call(target, _SuperPropertyOrUndefinedKeyString, super, property);
                //}
                return Expression.MakeIndex(target, _SuperIndexKeyString, new Expression[] { super, property });
            }
            if (property.Type == typeof(uint))
            {
                //if (coalesce)
                //{
                //    return Expression.Call(target, _SuperPropertyOrUndefinedUInt, super, property);
                //}
                return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, property });
            }
            if (property.Type == typeof(int))
            {
                //if (coalesce)
                //{
                //    return Expression.Call(target, _SuperPropertyOrUndefinedUInt, super, Expression.Convert(property, typeof(uint)));
                //}
                return Expression.MakeIndex(target, _SuperIndexUInt, new Expression[] { super, Expression.Convert(property, typeof(uint)) });
            }
            //if (coalesce)
            //{
            //    return Expression.Call(target, _SuperPropertyOrUndefined, super, Expression.Convert(property, typeof(uint)));
            //}
            return Expression.MakeIndex(target, _SuperIndex, new Expression[] { super, property });
        }


        public static Expression Index(Expression target, Expression property, bool coalesce = false)
        {
            if (property.Type == typeof(KeyString))
            {
                if(coalesce)
                {
                    return Expression.Call(target, _PropertyOrUndefinedKeyString, property);
                }
                return Expression.MakeIndex(target, _IndexKeyString, new Expression[] { property });
            }
            if (property.Type == typeof(uint))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _PropertyOrUndefinedUInt, property);
                }
                return Expression.MakeIndex(target, _IndexUInt, new Expression[] { property });
            }
            if (property.Type == typeof(int))
            {
                if (coalesce)
                {
                    return Expression.Call(target, _PropertyOrUndefinedUInt, Expression.Convert(property, typeof(uint)));
                }
                return Expression.MakeIndex(target, _IndexUInt, new Expression[] { Expression.Convert(property, typeof(uint)) });
            }
            if (coalesce)
            {
                // we need to use a block...
                var pes = Expression.Parameters(typeof(JSValue));
                var pe = pes[0];
                return Expression.Block(pes.AsSequence(),
                    Expression.Assign(pe, target),
                    Expression.Condition(
                        JSValueBuilder.IsNullOrUndefined(pe),
                        JSUndefinedBuilder.Value,
                        Expression.Call(target, _Index.GetMethod, property)
                        )
                    );

                // return Expression.Call(target, _PropertyOrUndefined, property);
            }
            return Expression.MakeIndex(target, _Index, new Expression[] { property });
        }

        private static MethodInfo _DeleteKeyString
            = type.InternalMethod(nameof(JSValue.Delete), KeyStringsBuilder.RefType);
        private static MethodInfo _DeleteUInt
            = type.InternalMethod(nameof(JSValue.Delete), typeof(uint));
        private static MethodInfo _DeleteJSValue
            = type.InternalMethod(nameof(JSValue.Delete), typeof(JSValue));

        public static Expression Delete(Expression target, Expression method)
        {
            if (method.Type == typeof(KeyString))
                return Expression.Call(target, _DeleteKeyString, method);
            if (method.Type == typeof(uint))
                return Expression.Call(target, _DeleteUInt, method);
            if (method.Type == typeof(int))
                return Expression.Call(target, _DeleteUInt, Expression.Convert(method, typeof(uint)));
            return Expression.Call(target, _DeleteJSValue, method);
        }

        internal static MethodInfo _CreateInstance
            = type.GetMethod(nameof(JSValue.CreateInstance));

        public static Expression CreateInstance(Expression target, Expression args)
        {
            return Expression.Call(target, _CreateInstance, args);
            // return target.CallExpression<JSValue, JSValue>(() => (x) => x.CreateInstance())
        }

        internal static MethodInfo StaticEquals
            = type.PublicMethod(nameof(Core.JSValue.StaticEquals), typeof(JSValue), typeof(JSValue));


        private static MethodInfo _Equals
            = type.PublicMethod(nameof(Core.JSValue.Equals), typeof(JSValue));

        public static Expression Equals(Expression target, Expression value)
        {
            if (value.Type == typeof(string))
                // return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(target, _EqualsLiteralString, value));
                return JSBooleanBuilder.NewFromCLRBoolean(
                    target.CallExpression<JSValue, string, bool>(() => (x, a) => x.EqualsLiteral(a), value)
                    );
            if (value.Type == typeof(double))
                // return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(target, _EqualsLiteralDouble, value));
                return JSBooleanBuilder.NewFromCLRBoolean(
                    target.CallExpression<JSValue,double, bool>(() =>(x, a) => x.EqualsLiteral(a), value)
                    );
            return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(target, _Equals, value));
        }

        public static Expression NotEquals(Expression target, Expression value)
        {
            if (value.Type == typeof(string))
                return JSBooleanBuilder.NewFromCLRBoolean(
                    // Expression.Not(Expression.Call(target, _EqualsLiteralString, value))
                    Expression.Not(target.CallExpression<JSValue, string, bool>(() => (x, a) => x.EqualsLiteral(a), value))
                    );
            if (value.Type == typeof(double))
                return JSBooleanBuilder.NewFromCLRBoolean(
                    // Expression.Not( Expression.Call(target, _EqualsLiteralDouble, value))
                    Expression.Not(target.CallExpression<JSValue, double, bool>(() => (x, a) => x.EqualsLiteral(a), value))
                    );
            return
                ExpHelper.JSBooleanBuilder.NewFromCLRBoolean(
                    Expression.Not(
                    Expression.Call(target, _Equals, value)
                ));
        }


        private static MethodInfo _StrictEquals
            = type.InternalMethod(nameof(Core.JSValue.StrictEquals), typeof(JSValue));

        public static Expression StrictEquals(Expression target, Expression value)
        {
            if (value.Type == typeof(string))
                return JSBooleanBuilder.NewFromCLRBoolean( target.CallExpression<JSValue, string, bool>(() => (x, a) => x.StrictEqualsLiteral(a), value));
            if (value.Type == typeof(double))
                return JSBooleanBuilder.NewFromCLRBoolean(target.CallExpression<JSValue, double, bool>(() => (x, a) => x.StrictEqualsLiteral(a), value));
            return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(target, _StrictEquals, value));
        }

        public static Expression NotStrictEquals(Expression target, Expression value)
        {
            if (value.Type == typeof(string))
                return JSBooleanBuilder.NewFromCLRBoolean(
                    Expression.Not(target.CallExpression<JSValue, string, bool>(() => (x, a) => x.StrictEqualsLiteral(a), value)));
            if (value.Type == typeof(double))
                return JSBooleanBuilder.NewFromCLRBoolean(
                    Expression.Not(target.CallExpression<JSValue, double, bool>(() => (x, a) => x.StrictEqualsLiteral(a), value)));
            return
                ExpHelper.JSBooleanBuilder.NewFromCLRBoolean(
                Expression.Not(Expression.Call(target, _StrictEquals, value)));
        }

        private static MethodInfo _Less
            = type.PublicMethod(nameof(Core.JSValue.Less), typeof(JSValue));

        public static Expression Less(Expression target, Expression value)
        {
            return JSBooleanBuilder.NewFromCLRBoolean( Expression.Call(ValueOf(target), _Less, ValueOf(value)));
        }

        private static MethodInfo _LessOrEqual
            = type.PublicMethod(nameof(Core.JSValue.LessOrEqual), typeof(JSValue));

        public static Expression LessOrEqual(Expression target, Expression value)
        {
            return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(ValueOf(target), _LessOrEqual, ValueOf(value)));
        }

        private static MethodInfo _Greater
            = type.PublicMethod(nameof(Core.JSValue.Greater), typeof(JSValue));
        public static Expression Greater(Expression target, Expression value)
        {
            return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(ValueOf(target), _Greater, ValueOf(value)));
        }

        private static MethodInfo _GreaterOrEqual
            = type.PublicMethod(nameof(Core.JSValue.GreaterOrEqual), typeof(JSValue));
        public static Expression GreaterOrEqual(Expression target, Expression value)
        {
            return JSBooleanBuilder.NewFromCLRBoolean(Expression.Call(ValueOf(target), _GreaterOrEqual, ValueOf(value)));
        }

        public static Expression ValueOf(Expression target) {
            // return Expression.Call(target, _ValueOf);
            return target.CallExpression<JSValue, JSValue>(() => (x) => x.ValueOf());
        }

        public static Expression LogicalAnd(Expression target, Expression value)
        {
            return Expression.Coalesce(JSValueExtensionsBuilder.NullIfTrue(target), value);
        }

        public static Expression LogicalOr(Expression target, Expression value)
        {
            return Expression.Coalesce(JSValueExtensionsBuilder.NullIfFalse(target), value);
        }

        private static MethodInfo _GetAllKeys =
            type.PublicMethod(nameof(JSValue.GetAllKeys), typeof(bool), typeof(bool));

        private static MethodInfo _GetEnumerator =
            typeof(IEnumerable<JSValue>).GetMethod(nameof(IEnumerable<JSValue>.GetEnumerator));

        public static Expression GetAllKeys(Expression target)
        {
            return
                    // Expression.Call(
                    Expression.Call(target, _GetAllKeys, Expression.Constant(true), Expression.Constant(false))
                    // ,
                    //_GetEnumerator);
                    ;
        }

        //private static MethodInfo _ConvertTo =
        //    type.InternalMethod(nameof(JSValue.TryConvertTo), typeof(Type), typeof(object).MakeByRefType());

        //private static MethodInfo _ForceConvert =
        //    type.PublicMethod(nameof(JSValue.ForceConvert), typeof(Type));

        //private static MethodInfo _Convert =
        //    type.InternalMethod(nameof(JSValue.Convert), typeof(Type), typeof(object));

        //public static Expression Convert(Expression jsValue, Type type, Expression defaultValue)
        //{
        //    return Expression.Convert(Expression.Call(jsValue, _Convert, Expression.Constant(type), defaultValue), type);
        //}


        //public static Expression ForceConvert(Expression jsValue, Type type)
        //{
        //    return Expression.Convert( Expression.Call(jsValue, _ForceConvert, Expression.Constant(type)), type);
        //}

        //public static Expression ConvertTo(Expression jsValue, Expression type, Expression outVar)
        //{
        //    return Expression.Call(jsValue, _ConvertTo, type, outVar);
        //}

        //public static Expression ConvertTo(Expression jsValue, Type type, Expression outVar)
        //{
        //    return ConvertTo(jsValue, Expression.Constant(type), outVar);
        //}

        public static Expression Coalesce(Expression target, Expression def)
        {
            return Expression.Condition(
                JSValueBuilder.IsNullOrUndefined(target), def, target);
        }

        //public static Expression Coalesce(
        //    Expression jsValue, 
        //    Type type, 
        //    Expression outVar,
        //    string memberName,
        //    [CallerMemberName] string function = null,
        //    [CallerFilePath] string filePath = null,
        //    [CallerLineNumber] int line = 0)
        //{
        //    return Expression.Condition(
        //        ConvertTo(jsValue, Expression.Constant(type), outVar), 
        //        // true
        //        outVar,
        //        // false
        //        JSExceptionBuilder.Throw(
        //            $"{type.Name}.prototype.{memberName} called with object not of type {type}", 
        //            type,
        //            function,
        //            filePath,
        //            line));
        //}

    }
}
