using Esprima;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.LinqExpressions
{
    public class TypeHelper<T>
    {

        protected static PropertyInfo IndexProperty<T1>()
        {
            return typeof(T)
                .GetProperties()
                .FirstOrDefault(x => x.GetIndexParameters().Length > 0 &&
                x.GetIndexParameters()[0].ParameterType == typeof(T1));
        }

        protected static PropertyInfo Property(string name)
        {
            var a = typeof(T).GetProperty(name);
            if (a == null)
                throw new NullReferenceException($"Property {name} not found on {typeof(T).FullName}");
            return a;
        }

        protected static ConstructorInfo Constructor<T1>()
        {
            var a = typeof(T).GetConstructor(new Type[] { typeof(T1) });
            return a;
        }

        protected static ConstructorInfo Constructor<T1, T2>()
        {
            return typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2) });
        }
        protected static ConstructorInfo Constructor<T1, T2, T3>()
        {
            var c = typeof(T).GetConstructor(new Type[] { typeof(T1), typeof(T2), typeof(T3) });
            if (c != null)
                return c;
            var list = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);
            return list.FirstOrDefault(x => 
                        x.GetParameters().Length == 3
                        && x.GetParameters()[0].ParameterType == typeof(T1)
                        && x.GetParameters()[1].ParameterType == typeof(T2)
                        && x.GetParameters()[2].ParameterType == typeof(T3));
        }

        protected static MethodInfo Method(string name)
        {
            return typeof(T).GetMethod(name);
        }

        protected static MethodInfo Method<T1>(string name)
        {
            var a = typeof(T).GetMethod(name, new Type[] { typeof(T1) });
            return a;
        }

        protected static MethodInfo InternalMethod<T1>(string name)
        {
            var a = typeof(T)
                .GetMethod(name, 
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.Static
                    , null, new Type[] { typeof(T1) }, null);
            return a;
        }

        protected static MethodInfo InternalMethod<T1,T2>(string name)
        {
            var a = typeof(T)
                .GetMethod(name,
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance
                    , null, new Type[] { typeof(T1), typeof(T2) }, null);
            return a;
        }

        protected static MethodInfo Method<T1, T2>(string name)
        {
            return typeof(T).GetMethod(name, new Type[] { typeof(T1), typeof(T2) });
        }

        protected static MethodInfo StaticMethod<T1, T2>(string name)
        {
            return typeof(T)
                .GetMethod(name, 
                new Type[] { typeof(T1), typeof(T2) });
        }

        protected static MethodInfo InternalStaticMethod<T1, T2>(string name)
        {
            var a = typeof(T)
                .GetMethod(name,
                    BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Static
                    , null, new Type[] { typeof(T1), typeof(T2) }, null);
            return a;
        }

        protected static FieldInfo InternalField(string name)
        {
            return typeof(T).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected static FieldInfo Field(string name)
        {
            return typeof(T).GetField(name);
        }

    }

    public static class ExpHelper
    {
        public class Object: TypeHelper<System.Object>
        {
            private static MethodInfo _ToString
                = typeof(System.Object).GetMethod("ToString", new Type[] { });

            public static Expression ToString(Expression value)
            {
                return Expression.Call(value, _ToString);
            }

            private static MethodInfo _ReferenceEquals
                = Method<object, object>("ReferenceEquals");

            public static Expression RefEquals(Expression left, Expression right)
            {
                return Expression.Call(null, _ReferenceEquals, left, right);
            }
        }

        public class String: TypeHelper<System.String>
        {
            private static MethodInfo _Compare =
                StaticMethod<string,string>("Compare");

            public static Expression Compare(Expression left, Expression right)
            {
                return Expression.Call(null, _Compare, left, right);
            }

            private static MethodInfo _Equals =
                StaticMethod<string, string>("Equals");

            public static Expression Equals(Expression left, Expression right)
            {
                return Expression.Call(null, _Equals, left, right);
            }

            private static MethodInfo _Concat =
                StaticMethod<string, string>("Concat");

            public static Expression Concat(Expression left, Expression right)
            {
                return Expression.Call(null, _Concat, left, right);
            }

        }

        public class IDisposable : TypeHelper<System.IDisposable>
        {
            private static MethodInfo _Dispose
                = Method("Dispose");

            public static Expression Dispose(Expression exp)
            {
                return Expression.Call(exp, _Dispose);
            }
        }

        public class Exception: TypeHelper<Exception>
        {
            private static MethodInfo _ToString =
                Method("ToString");
            public static Expression ToString(Expression target)
            {
                return Expression.Call(target, _ToString);
            }
        }

        internal class KeyStrings
        {
            private static MethodInfo _GetOrAdd =
                typeof(Core.KeyStrings).GetMethod("GetOrCreate");

            public static Expression GetOrCreate(Expression text)
            {
                return Expression.Call(null, _GetOrAdd, text);
            }
        }

        public class JSDebugger: TypeHelper<Debugger.JSDebugger>
        {
            private static MethodInfo _RaiseBreak
                = Method("RaiseBreak");

            public static Expression RaiseBreak()
            {
                return Expression.Call(null, _RaiseBreak);
            }
        }

        public class JSContext: TypeHelper<Core.JSContext> {
            public static Expression Current =>
                Expression.Property(null, Property("Current"));

            public static Expression CurrentScope =>
                Expression.Field(Current, InternalField("Scope"));

            public static Expression True =
                Expression.Field(Current, Field("True"));

            public static Expression False =
                Expression.Field(Current, Field("False"));

            public static Expression NaN =
                Expression.Field(Current, Field("NaN"));

            private static PropertyInfo _Index =
                IndexProperty<Core.KeyString>();
            public static Expression Index(Expression key)
            {
                return Expression.MakeIndex(Current, _Index, new Expression[] { key });
            }
        }

        public class LexicalScope: TypeHelper<Core.LexicalScope>
        {
            private static PropertyInfo _Index =
                IndexProperty<Core.KeyString>();

            public static Expression Index(Expression exp)
            {
                return Expression.MakeIndex(JSContext.CurrentScope, _Index , new Expression[] { exp });
            }

            private static MethodInfo _Push
                = typeof(LinkedStack<Core.LexicalScope>)
                .GetMethod(nameof(Core.LinkedStack<Core.LexicalScope>.Push));

            private static ConstructorInfo _New
                = typeof(Core.LexicalScope)
                .GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance, 
                    null, 
                    new Type[] { 
                        typeof(string),
                        typeof(string),
                        typeof(int),
                        typeof(int)
                    }, null);

            public static Expression NewScope(
                Expression fileName, 
                string function, 
                int line, 
                int column)
            {
                return Expression.Call(
                    JSContext.CurrentScope, 
                    _Push,
                    Expression.New(_New,
                    fileName,
                    Expression.Constant(function),
                    Expression.Constant(line),
                    Expression.Constant(column)));
            }

            private static FieldInfo _Position =
                Field(nameof(Core.LexicalScope.Position));

            private static ConstructorInfo _NewPosition =
                TypeHelper<Position>.Constructor<int, int>();

            public static Expression SetPosition(Expression exp, int line, int column)
            {
                return Expression.Assign(
                    Expression.Field(exp, _Position),
                    Expression.New(_NewPosition, Expression.Constant(line), Expression.Constant(column)
                    ));
            }

        }

        public class JSNull: TypeHelper<Core.JSNull>
        {
            public static Expression Value = 
                Expression.Field(null, 
                    Field(nameof(JSNull.Value)));
        }

        public class JSUndefined : TypeHelper<Core.JSUndefined>
        {
            public static Expression Value = 
                Expression.Field(null, 
                    Field(nameof(Core.JSUndefined.Value)));
        }


        public class JSNumber: TypeHelper<Core.JSNumber>
        {

            private static FieldInfo _Value =
                InternalField(nameof(Core.JSNumber.value));

            public static Expression Value(Expression ex)
            {
                return Expression.Field(ex, _Value);
            }

            private static ConstructorInfo _NewDouble 
                = Constructor<double>();

            public static Expression New(Expression exp)
            {
                return Expression.New(_NewDouble, exp);
            }

            private static MethodInfo _AddValue =
                            Method<Core.JSValue>(nameof(Core.JSValue.AddValue));

            public static Expression AddValue(Expression target, Expression value)
            {
                return Expression.Call(target, _AddValue, value);
            }


        }

        public class JSString : TypeHelper<Core.JSString>
        {
            private static FieldInfo _Value =
                InternalField(nameof(Core.JSString.value));

            public static Expression Value(Expression ex)
            {
                return Expression.Field(ex, _Value);
            }

            private static ConstructorInfo _New = Constructor<string>();

            public static Expression New(Expression exp)
            {
                return Expression.New(_New, exp);
            }

            public static Expression ConcatBasicStrings(Expression left, Expression right)
            {
                return Expression.New(_New, String.Concat(left, right));
            }

        }
        public class JSRegExp : TypeHelper<Core.JSRegExp>
        {
            private static ConstructorInfo _New = Constructor<string, string>();

            public static Expression New(Expression exp, Expression exp2)
            {
                return Expression.New(_New, exp, exp2);
            }

        }

        public class JSException: TypeHelper<Core.JSException>
        {
            private static MethodInfo _Throw = 
                InternalMethod<Core.JSValue>(nameof(Core.JSException.Throw));

            public static Expression Throw(Expression value)
            {
                return Expression.Call(null, _Throw, value);
            }

            private static PropertyInfo _Error =
                Property("Error");

            public static Expression Error(Expression target)
            {
                return Expression.Property(target, _Error);
            }
        }

        public class JSVariable: TypeHelper<Core.JSVariable>
        {
            private static ConstructorInfo _New
                = Constructor<Core.JSValue, string>();

            public static Expression New(Expression value, string name)
            {
                return Expression.New(_New, value, Expression.Constant(name, typeof(string)));
            }

            public static Expression FromArgument(Expression args, Expression length, int i, string name)
            {
                var ie = Expression.Constant(i);
                var lessThan = Expression.LessThan(ie, length);
                var ai = Expression.ArrayAccess(args, ie);
                var undefined = JSUndefined.Value;
                var c = Expression.Condition(
                        lessThan,
                        ai,
                        undefined);
                return Expression.New(_New, 
                     c,
                    Expression.Constant(name));
            }


            public static Expression New(string name)
            {
                return Expression.New(_New, ExpHelper.JSUndefined.Value, Expression.Constant(name));
            }

        }

        public class JSValue: TypeHelper<Core.JSValue>
        {
            private static PropertyInfo _DoubleValue =
                Property(nameof(Core.JSValue.DoubleValue));
            public static Expression DoubleValue(Expression exp)
            {
                return Expression.Property(exp, _DoubleValue);
            }

            private static PropertyInfo _BooleanValue =
                Property("BooleanValue");
            public static Expression BooleanValue(Expression exp)
            {
                return Expression.Property(exp, _BooleanValue);
            }


            private static MethodInfo _CreateInstance =
                Method<Core.JSArguments>(nameof(Core.JSValue.CreateInstance));

            public static Expression CreateInstance(Expression target, Expression paramList)
            {
                return Expression.Call(target, _CreateInstance, paramList);
            }

            private static MethodInfo _InvokeMethod =
                Method<Core.KeyString, Core.JSArguments>(nameof(Core.JSValue.InvokeMethod));

            public static Expression InvokeMethod(Expression target, Expression keyString, Expression args)
            {
                return Expression.Call(target, _InvokeMethod, keyString, args);
            }

            private static MethodInfo _InvokeFunction =
                Method<Core.JSValue, Core.JSArguments>(nameof(Core.JSValue.InvokeFunction));

            public static Expression InvokeFunction(Expression target, Expression t, Expression args)
            {
                return Expression.Call(target, _InvokeFunction, t, args);
            }

            private static MethodInfo _Add =
                Method<Core.JSValue>(nameof(Core.JSValue.AddValue));

            public static Expression Add(Expression target, Expression value)
            {
                return Expression.Call(target, _Add, value);
            }

            private static MethodInfo _InstanceOf =
                Method<Core.JSValue>(nameof(Core.JSValue.InstanceOf));

            public static Expression InstanceOf(Expression target, Expression value)
            {
                return Expression.Call(target, _InstanceOf, value);
            }

            private static MethodInfo _IsIn =
                Method<Core.JSValue>(nameof(Core.JSValue.IsIn));

            public static Expression IsIn(Expression target, Expression value)
            {
                return Expression.Call(target, _IsIn, value);
            }

            private static MethodInfo _Delete =
                Method<Core.JSValue>("Delete");

            public static Expression Delete(Expression target, Expression value)
            {
                return Expression.Call(target, _Delete, value);
            }

            private static PropertyInfo _TypeOf =
                Property("TypeOf");

            public static Expression TypeOf(Expression target)
            {
                return Expression.Property(target, _TypeOf);
            }

            private static PropertyInfo _KeyStringIndex =
                IndexProperty<Core.KeyString>();

            private static PropertyInfo _UIntIndex =
                IndexProperty<uint>();


            public static Expression KeyStringIndex(Expression target, Expression property)
            {
                return Expression.MakeIndex(target, _KeyStringIndex, new Expression[] { property });
            }

            private static PropertyInfo _Index =
                IndexProperty<Core.JSValue>();

            public static Expression Index(Expression target, Expression property)
            {
                return Expression.MakeIndex(target, _Index, new Expression[] { property });
            }

            public static Expression Index(Expression target, uint i)
            {
                return Expression.MakeIndex(target, _UIntIndex, new Expression[] { Expression.Constant(i) });
            }
            internal static MethodInfo StaticEquals
                = InternalStaticMethod<Core.JSValue,Core.JSValue>(nameof(Core.JSValue.StaticEquals));


            private static MethodInfo _Equals
                = Method<Core.JSValue>(nameof(Core.JSValue.Equals));

            public static Expression Equals(Expression target, Expression value)
            {
                return Expression.Call(target, _Equals, value);
            }

            public static Expression NotEquals(Expression target, Expression value)
            {
                return 
                    ExpHelper.JSBoolean.NewFromCLRBoolean(
                        Expression.Not(
                        ExpHelper.JSValue.BooleanValue(Expression.Call(target, _Equals, value))
                    ));
            }


            private static MethodInfo _StrictEquals
                = Method<Core.JSValue>(nameof(Core.JSValue.StrictEquals));

            public static Expression StrictEquals(Expression target, Expression value)
            {
                return Expression.Call(target, _StrictEquals, value);
            }

            public static Expression NotStrictEquals(Expression target, Expression value)
            {
                return 
                    ExpHelper.JSBoolean.NewFromCLRBoolean(
                    Expression.Not( 
                    ExpHelper.JSValue.BooleanValue(Expression.Call(target, _StrictEquals, value))));
            }

            private static MethodInfo _Less
                = InternalMethod<Core.JSValue>(nameof(Core.JSValue.Less));

            public static Expression Less(Expression target, Expression value)
            {
                return Expression.Call(target, _Less, value);
            }

            private static MethodInfo _LessOrEqual
                = InternalMethod<Core.JSValue>(nameof(Core.JSValue.LessOrEqual));

            public static Expression LessOrEqual(Expression target, Expression value)
            {
                return Expression.Call(target, _LessOrEqual, value);
            }

            public static Expression Greater(Expression target, Expression value)
            {
                return Expression.Not(Expression.Call(target, _LessOrEqual, value));
            }

            public static Expression GreaterOrEqual(Expression target, Expression value)
            {
                return Expression.Not(Expression.Call(target, _Less, value));
            }


            public static Expression LogicalAnd(Expression target, Expression value)
            {
                return Expression.Condition(JSValue.BooleanValue(target), value, target);
            }

            public static Expression LogicalOr(Expression target, Expression value)
            {
                return Expression.Condition(
                    JSValue.BooleanValue(target),
                    target,
                    value);
            }
        }

        public class JSObject: TypeHelper<Core.JSValue>
        {
            private static FieldInfo _ownProperties =
                InternalField(nameof(Core.JSValue.ownProperties));

            private static PropertyInfo _Index =
                typeof(BaseMap<uint,Core.JSProperty>)
                    .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.GetIndexParameters().Length > 0);

            private static FieldInfo _Key =
                TypeHelper<KeyString>.Field(nameof(Core.KeyString.Key));

            public static Expression New(IEnumerable<(Expression key, Expression value)> keyValues)
            {
                var pe = Expression.Parameter(typeof(Core.JSValue));
                var fe = Expression.Parameter(typeof(BinaryUInt32Map<Core.JSProperty>));
                var list = new List<Expression>() {
                    Expression.Assign(pe, Expression.New(typeof(Core.JSObject))),
                    Expression.Assign(fe, Expression.Field(pe, _ownProperties))
                };

                foreach(var (k,v) in keyValues)
                {
                    list.Add(Expression.Assign( 
                        Expression.Property(fe, _Index, Expression.Field(k, _Key)  ), v ));
                }
                list.Add(pe);
                return Expression.Block(new ParameterExpression[] { pe, fe }, list );
            }

        }

        public class JSProperty: TypeHelper<Core.JSProperty>
        {
            private static ConstructorInfo _New =
                typeof(Core.JSProperty).GetConstructors().FirstOrDefault();

            private static FieldInfo _Attributes =
                Field(nameof(Core.JSProperty.Attributes));

            private static FieldInfo _Key =
                Field(nameof(Core.JSProperty.key));

            private static FieldInfo _Get =
                Field(nameof(Core.JSProperty.get));

            private static FieldInfo _Value =
                Field(nameof(Core.JSProperty.value));

            public static Expression Value(Expression key, Expression value)
            {
                return Expression.MemberInit(Expression.New(typeof(Core.JSProperty)),
                    Expression.Bind(_Key, key),
                    Expression.Bind(_Value, value),
                    Expression.Bind(_Attributes, Expression.Constant(JSPropertyAttributes.EnumerableConfigurableValue))
                    );
            }

            public static Expression Property(Expression key, Expression getter, Expression setter)
            {
                getter = getter == null ? (Expression)Expression.Constant(null, typeof(Core.JSFunction)) : Expression.Convert(getter, typeof(Core.JSFunction));
                setter = setter == null ? (Expression)Expression.Constant(null, typeof(Core.JSFunction)) : Expression.Convert(setter, typeof(Core.JSFunction));
                return Expression.MemberInit(Expression.New(typeof(Core.JSProperty)),
                    Expression.Bind(_Key, key),
                    Expression.Bind(_Get, getter),
                    Expression.Bind(_Value, setter),
                    Expression.Bind(_Attributes, Expression.Constant(JSPropertyAttributes.EnumerableConfigurableReadonlyProperty))
                    );
            }

        }

        public class JSArray: TypeHelper<Core.JSArray>
        {
            private static ConstructorInfo _New =
                typeof(Core.JSArray).GetConstructor(new Type[] { });
            public static Expression New(IEnumerable<Expression> list)
            {
                return Expression.ListInit(
                    Expression.New(_New),
                    list);
            }

        }

        public class JSArguments: TypeHelper<Core.JSArguments>
        {

            private static FieldInfo _Elements =
                InternalField(nameof(Core.JSArguments.elements));

            public static Expression Elements(Expression target)
            {
                return Expression.Field(target, _Elements);
            }

            private static ConstructorInfo _New =
                Constructor<Core.JSValue[]>();

            public static Expression New(IEnumerable<Expression> list)
            {
                if (!list.Any())
                {
                    return JSArguments.Empty();
                }
                return Expression.New(_New, Expression.NewArrayInit(typeof(Core.JSValue),list));
            }

            private static FieldInfo _Empty =
                Field("Empty");

            public static Expression Empty()
            {
                return Expression.Field(null, _Empty);
            }


            private static PropertyInfo _Index
                = IndexProperty<uint>();

            public static Expression Index(Expression target, uint value)
            {
                return Expression.MakeIndex(target, _Index, new Expression[] {
                    Expression.Constant(value)
                });
            }
        }

        public class JSBoolean: TypeHelper<Core.JSBoolean>
        {

            private static FieldInfo _Value =
                InternalField(nameof(Core.JSBoolean._value));

            public static Expression Value(Expression target)
            {
                return Expression.Field(target, _Value);
            }

            public static Expression NewFromCLRBoolean(Expression target)
            {
                return Expression.Condition(target, JSContext.True, JSContext.False);
            }


            public static Expression Not(Expression value)
            {
                return Expression.Condition(
                    JSValue.BooleanValue(value),
                    JSContext.False,
                    JSContext.True
                    );
            }
        }
        public class JSFunction: TypeHelper<Core.JSFunction>
        {
            private static ConstructorInfo _New =
                Constructor<JSFunctionDelegate, string, string>();

            public static Expression New(Expression del, string name, string code)
            {
                return Expression.New(_New , del, 
                    Expression.Constant(name), 
                    Expression.Constant(code));
            }
        }

    }
}
