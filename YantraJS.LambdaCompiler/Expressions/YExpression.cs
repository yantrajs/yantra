#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace YantraJS.Expressions
{

    public enum YExpressionType
    {
        Binary,
        Constant,
        Conditional,
        Assign,
        Parameter,
        Block,
        Call,
        New,
        Field,
        Property,
        NewArray,
        GoTo,
        Return,
        Loop,
        TypeAs,
        Lambda,
        Label
    }

    public enum YOperator
    {
        Add,
        Subtract,
        Multipley,
        Divide,
        Mod,
        Power,

        Xor,
        BitwiseAnd,
        BitwiseOr,
        BooleanAnd,
        BooleanOr,

        BooleanNot,
        BitwiseNot,

        TypeAs,
        TypeIs,

        Coalesc
    }

    /// <summary>
    /// System.Linq.Expressions.Expression is very complex and it allows
    /// various complex operations such as += etc.
    /// 
    /// We need simpler operations to build IL easily without automatically
    /// assuming or supporting nullability etc.
    /// 
    /// Simple IL Generator does not allow += operators etc. It does not 
    /// allow Nullable types as well. Expression creator must take care of it.
    /// </summary>
    public abstract class YExpression
    {
        public readonly YExpressionType NodeType;

        public readonly Type Type;

        protected YExpression(YExpressionType nodeType, Type type)
        {
            this.NodeType = nodeType;
            this.Type = type;
        }

        public static YBinaryExpression Binary(YExpression left, YOperator @operator, YExpression right)
        {
            return new YBinaryExpression(left, @operator, right);
        }

        public static YConstantExpression Constant(object value, Type? type = null)
        {
            return new YConstantExpression(value, type ?? value.GetType());
        }

        public static YConditionalExpression Conditional(
            YExpression test, 
            YExpression @true, 
            YExpression @false,
            Type? type = null)
        {
            return new YConditionalExpression(test, @true, @false, type);
        }

        public static YAssignExpression Assign(YExpression left, YExpression right, Type? type = null)
        {
            return new YAssignExpression(left, right, type);
        }

        public static YParameterExpression Parameter(Type type, string? name = null)
        {
            return new YParameterExpression(type, name);
        }

        public static YBlockExpression Block(
            IEnumerable<YParameterExpression> variables,
            IList<YExpression> expressions)
        {
            return new YBlockExpression(variables, expressions);
        }

        public static YCallExpression Call(YExpression target, MethodInfo method, IList<YExpression> args)
        {
            return new YCallExpression(target, method, args);
        }

        public static YNewExpression New(ConstructorInfo constructor, IList<YExpression> args)
        {
            return new YNewExpression(constructor, args);
        }

        public static YFieldExpression Field(YExpression target, FieldInfo field)
        {
            return new YFieldExpression(target, field);
        }

        public static YPropertyExpression Property(YExpression target, PropertyInfo field)
        {
            return new YPropertyExpression(target, field);
        }

        public static YNewArrayExpression NewArray(Type type, IList<YExpression> elements)
        {
            return new YNewArrayExpression(type, elements.Count, elements);
        }
        public static YNewArrayExpression NewArray(Type type, int size, IList<YExpression> elements)
        {
            return new YNewArrayExpression(type, size, elements);
        }
        public static YNewArrayExpression NewArray(Type type, int size)
        {
            return new YNewArrayExpression(type, size);
        }

        public static YLabelTarget Label(string? name = null, 
            Type? type = null)
        {
            return new YLabelTarget(name ?? "unnamed", type ?? typeof(void));
        }

        public static YLabelExpression Label(YLabelTarget target, YExpression? defaultValue = null)
        {
            return new YLabelExpression(target, defaultValue);
        }

        public static YGoToExpression GoTo(YLabelTarget target, YExpression? defaultValue = null)
        {
            return new YGoToExpression(target, defaultValue);
        }

        public static YReturnExpression Return(YLabelTarget target, YExpression? defaultValue = null)
        {
            return new YReturnExpression(target, defaultValue);
        }

        public static YLoopExpression Loop(YExpression body, YLabelTarget @break, YLabelTarget? @continue = null) {
            return new YLoopExpression(body, @break, @continue ?? Label("continue", @break.LabelType));
        }

        public static YLambdaExpression Lambda(string name, YExpression body, IList<YParameterExpression> parameters)
        {
            return new YLambdaExpression(name, body, parameters);
        }
    }
}
