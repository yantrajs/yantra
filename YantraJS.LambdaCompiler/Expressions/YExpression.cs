#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YantraJS.Expressions
{

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

        public abstract void Print(IndentedTextWriter writer);

        public string DebugView => ToString();

        public override string ToString()
        {
            using (var sw = new StringWriter())
            {
                using (var iw = new IndentedTextWriter(sw))
                {
                    Print(iw);
                    return sw.ToString();
                }
            }
        }

        public static YBinaryExpression Binary(YExpression left, YOperator @operator, YExpression right)
        {
            return new YBinaryExpression(left, @operator, right);
        }

        public static YCoalesceExpression Coalesce(YExpression left, YExpression right)
        {
            return new YCoalesceExpression(left, right);
        }

        public static YConstantExpression Constant(object value, Type? type = null)
        {
            if (value is YConstantExpression)
                throw new NotSupportedException();
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
            IEnumerable<YParameterExpression>? variables,
            IList<YExpression> expressions)
        {
            return new YBlockExpression(variables, expressions);
        }

        public static YBlockExpression Block(
            IEnumerable<YParameterExpression>? variables,
            params YExpression[] expressions)
        {
            return new YBlockExpression(variables, expressions);
        }

        public static YConvertExpression Convert(YExpression exp, Type type)
        {
            var method = YConvertExpression.GetConversionMethod(exp.Type, type);
            return new YConvertExpression(exp, type, method);
        }

        public static YConvertExpression Convert(YExpression exp, Type type, MethodInfo method)
        {
            return new YConvertExpression(exp, type, method);
        }


        public static YBinaryExpression Equal(YExpression left, YExpression right)
             => YExpression.Binary(left, YOperator.Equal, right);

        public static YBinaryExpression Or(YExpression left, YExpression right)
            => YExpression.Binary(left, YOperator.BooleanOr, right);   

        public static YBinaryExpression NotEqual(YExpression left, YExpression right)
             => YExpression.Binary(left, YOperator.NotEqual, right);

        public static YBinaryExpression Greater(YExpression left, YExpression right)
             => YExpression.Binary(left, YOperator.Greater, right);

        public static YBinaryExpression GreaterOrEqual(YExpression left, YExpression right)
             => YExpression.Binary(left, YOperator.GreaterOrEqual, right);

        public static YBinaryExpression Less(YExpression left, YExpression right)
             => YExpression.Binary(left, YOperator.Less, right);

        public static YBinaryExpression LessOrEqual(YExpression left, YExpression right)
             => YExpression.Binary(left, YOperator.LessOrEqual, right);

        public static YCallExpression Call(YExpression? target, MethodInfo method, IList<YExpression> args)
        {
            return new YCallExpression(target, method, args);
        }
        public static YCallExpression Call(YExpression? target, MethodInfo method, params YExpression[] args)
        {
            return new YCallExpression(target, method, args);
        }

        public static YNewExpression New(ConstructorInfo constructor, IList<YExpression> args)
        {
            return new YNewExpression(constructor, args);
        }
        public static YNewExpression New(Type type, IList<YExpression> args)
        {
            var constructor = type.GetConstructor(args.Select(x => x.Type).ToArray());
            return new YNewExpression(constructor, args);
        }
        public static YNewExpression New(ConstructorInfo constructor, params YExpression[] args)
        {
            return New(constructor, (IList<YExpression>)args);
        }
        public static YNewExpression New(Type type, params YExpression[] args)
        {
            return New(type, (IList<YExpression>)args);
        }

        public static YFieldExpression Field(YExpression target, FieldInfo field)
        {
            return new YFieldExpression(target, field);
        }

        public static YFieldExpression Field(YExpression target, string name)
        {
            var field = target.Type.GetField(name);
            return new YFieldExpression(target, field);
        }

        public static YInvokeExpression Invoke(YExpression target, Type type, params YExpression[] args)
        {
            return new YInvokeExpression(target, args, type);
        }

        public static YPropertyExpression Property(YExpression target, PropertyInfo field)
        {
            return new YPropertyExpression(target, field);
        }

        public static YNewArrayExpression NewArray(Type type, IList<YExpression> elements)
        {
            return new YNewArrayExpression(type, elements);
        }
        public static YNewArrayBoundsExpression NewArrayBounds(Type type, YExpression size)
        {
            return new YNewArrayBoundsExpression(type, size);
        }

        public static YLabelTarget Label(string? name = null, 
            Type? type = null)
        {
            return new YLabelTarget(name, type ?? typeof(void));
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

        public static YLambdaExpression Lambda(string name, YExpression body, YParameterExpression[] parameters)
        {
            return new YLambdaExpression(name, body, parameters);
        }

        public static YLambdaExpression Lambda(string name, YExpression body, List<YParameterExpression> parameters)
        {
            return new YLambdaExpression(name, body, parameters.ToArray());
        }

        public static YTypeAsExpression TypeAs(YExpression target, Type type)
        {
            return new YTypeAsExpression(target, type);
        }

        public static YTypeIsExpression TypeIs(YExpression target, Type type)
        {
            return new YTypeIsExpression(target, type);
        }

        public static YCatchBody Catch(YParameterExpression parameter, YExpression body)
        {
            return new YCatchBody(parameter, body);
        }
        public static YCatchBody Catch(YExpression body)
        {
            return new YCatchBody(null, body);
        }

        public static YTryCatchFinallyExpression TryCatchFinally(
            YExpression @try, 
            YCatchBody? catchBody,
            YExpression? @finally)
        {
            if (catchBody == null && @finally == null)
                throw new ArgumentNullException($"Both finally and catch cannot be null");
            return new YTryCatchFinallyExpression(@try, catchBody, @finally);
        }

        public static YArrayIndexExpression ArrayIndex(YExpression target, YExpression index)
        {
            return new YArrayIndexExpression(target, index);
        }

        public static YArrayLengthExpression ArrayLength(YExpression target)
        {
            return new YArrayLengthExpression(target);
        }

        public static YIndexExpression Index(YExpression target, params YExpression[] args)
        {
            var types = args.Select(x => x.Type).ToArray();
            PropertyInfo propertyInfo =
                target.GetType()
                    .GetProperties()
                    .FirstOrDefault(x => x.GetIndexParameters().Select(p => p.ParameterType).SequenceEqual(types));
            return new YIndexExpression(target, propertyInfo, args);
        }

        public static YUnaryExpression Not(YExpression exp)
        {
            return new YUnaryExpression(exp, YUnaryOperator.Not);
        }

        public static YUnaryExpression Negative(YExpression exp)
        {
            return new YUnaryExpression(exp, YUnaryOperator.Negative);
        }

        public static YUnaryExpression OnesComplement(YExpression exp)
        {
            return new YUnaryExpression(exp, YUnaryOperator.OnesComplement);
        }

        public static YTypeIsExpression TypeEqual(YExpression exp, Type type)
        {
            return new YTypeIsExpression(exp, type);
        }

        public static YThrowExpression Throw(YExpression exp)
        {
            return new YThrowExpression(exp);
        }
        internal static YLambdaExpression InlineLambda(
            string name, 
            YExpression body, 
            YParameterExpression[] parameters, 
            YExpression? repository)
        {
            return new YLambdaExpression(name, body, parameters, null, repository);
        }

        internal static YLambdaExpression InlineLambda(
            string name,
            YExpression body,
            List<YParameterExpression> parameters,
            YExpression? repository)
        {
            return new YLambdaExpression(name, body, parameters.ToArray(), null, repository);
        }
    }
}
