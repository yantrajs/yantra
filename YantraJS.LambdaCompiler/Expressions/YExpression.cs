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
        New
    }

    public enum YOperator
    {
        Add,
        Subtract,
        Multipley,
        Divide
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


    }
}
