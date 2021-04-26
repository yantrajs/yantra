using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Expressions
{
    public class YExpressionMapVisitor : YExpressionVisitor<YExpression>
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T>(T node, out T r)
            where T : YExpression
        {
            r = Visit(node) as T;
            return r != node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T1, T2>(T1 node1, T2 node2, out T1 r1, out T2 r2)
            where T1 : YExpression
            where T2 : YExpression
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            return r1 != node1 || r2 != node2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T1, T2, T3>(T1 node1, T2 node2, T3 node3, out T1 r1, out T2 r2, out T3 r3)
            where T1 : YExpression
            where T2 : YExpression
            where T3 : YExpression
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            r3 = Visit(node3) as T3;
            return r1 != node1 || r2 != node2 || r3 != node3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Modified<T1, T2, T3, T4>(T1 node1, T2 node2, T3 node3, T4 node4, out T1 r1, out T2 r2, out T3 r3, out T4 r4)
            where T1 : YExpression
            where T2 : YExpression
            where T3 : YExpression
            where T4 : YExpression
        {
            r1 = Visit(node1) as T1;
            r2 = Visit(node2) as T2;
            r3 = Visit(node3) as T3;
            r4 = Visit(node4) as T4;
            return r1 != node1 || r2 != node2 || r3 != node3 || r4 != node4;
        }

        private bool Modified<T>(in T[] statements, out T[] list)
            where T : YExpression
        {
            if (statements.Length == 0)
            {
                list = Array.Empty<T>();
                return false;
            }
            bool dirty = false;
            var r = new T[statements.Length];
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visited = Visit(item);
                if (visited != item)
                    dirty = true;
                r[i] = visited as T;
            }
            if (!dirty)
            {
                list = Array.Empty<T>();
                return false;
            }
            list = r;
            return true;
        }

        private bool Modified<T>(in T[] statements, Func<T, T> visitor, out T[] list)
        {
            if (statements.Length == 0)
            {
                list = Array.Empty<T>();
                return false;
            }
            bool dirty = false;
            var r = new T[statements.Length];
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visited = visitor(item);
                if (!visited.Equals(item))
                    dirty = true;
                r[i] = visited;
            }
            if (!dirty)
            {
                list = Array.Empty<T>();
                return false;
            }
            list = r;
            return true;
        }
        protected override YExpression VisitBinary(YBinaryExpression yBinaryExpression)
        {
            if (Modified(yBinaryExpression.Left, yBinaryExpression.Right, out var left, out var right))
                return new YBinaryExpression(left, yBinaryExpression.Operator, right);
            return yBinaryExpression;
        }

        protected override YExpression VisitBlock(YBlockExpression yBlockExpression)
        {
            var pm = Modified(yBlockExpression.Variables, out var variables);
            var sm = Modified(yBlockExpression.Expressions, out var expressions);
            if (pm || sm)
                return new YBlockExpression(variables, expressions);
            return yBlockExpression;
        }

        protected override YExpression VisitCall(YCallExpression yCallExpression)
        {
            var tm = Modified(yCallExpression.Target, out var target);
            var am = Modified(yCallExpression.Arguments, out var arguments);
            if (tm || am)
                return new YCallExpression(target, yCallExpression.Method, arguments);
            return yCallExpression;
        }

        protected override YExpression VisitConditional(YConditionalExpression yConditionalExpression)
        {
            if (Modified(
                yConditionalExpression.test, yConditionalExpression.@true, yConditionalExpression.@false,
                out var test, out var @true, out var @false))
                return new YConditionalExpression(test, @true, @false);
            return yConditionalExpression;
        }

        protected override YExpression VisitCoalesce(YCoalesceExpression yCoalesceExpression)
        {
            if (Modified(yCoalesceExpression.Left, yCoalesceExpression.Right, out var left, out var right))
                return new YCoalesceExpression(left, right);
            return yCoalesceExpression;
        }

        protected override YExpression VisitConstant(YConstantExpression yConstantExpression)
        {
            return yConstantExpression;
        }

        protected override YExpression VisitField(YFieldExpression yFieldExpression)
        {
            if (Modified(yFieldExpression.Target, out var target))
                return new YFieldExpression(target, yFieldExpression.FieldInfo);
            return yFieldExpression;
        }

        protected override YExpression VisitGoto(YGoToExpression yGoToExpression)
        {
            if (Modified(yGoToExpression.Default, out var @default))
                return new YGoToExpression(yGoToExpression.Target, @default);
            return yGoToExpression;
        }

        protected override YExpression VisitLabel(YLabelExpression yLabelExpression)
        {
            if (Modified(yLabelExpression.Default, out var @default))
                return new YLabelExpression(yLabelExpression.Target, @default);
            return yLabelExpression;
        }

        protected override YExpression VisitLambda(YLambdaExpression yLambdaExpression)
        {
            var pm = Modified(yLambdaExpression.Parameters, out var parameters);
            var bm = Modified(yLambdaExpression.Body, out var body);
            if (pm || bm)
                return new YLambdaExpression(yLambdaExpression.Name, body, parameters);
            return yLambdaExpression;

        }

        protected override YExpression VisitLoop(YLoopExpression yLoopExpression)
        {
            if (Modified(yLoopExpression.Body, out var body))
                return new YLoopExpression(body, yLoopExpression.Break, yLoopExpression.Continue);
            return yLoopExpression;
        }

        protected override YExpression VisitNew(YNewExpression yNewExpression)
        {
            var am = Modified(yNewExpression.args, out var args);
            if (am)
                return new YNewExpression(yNewExpression.constructor, args);
            return yNewExpression;

        }

        protected override YExpression VisitNewArray(YNewArrayExpression yNewArrayExpression)
        {
            var am = Modified(yNewArrayExpression.Elements, out var elements);
            if (am)
                return new YNewArrayExpression(yNewArrayExpression.Type, elements);
            return yNewArrayExpression;
        }

        protected override YExpression VisitParameter(YParameterExpression yParameterExpression)
        {
            return yParameterExpression;
        }

        protected override YExpression VisitProperty(YPropertyExpression yPropertyExpression)
        {
            if (Modified(yPropertyExpression.Target, out var target))
                return new YPropertyExpression(target, yPropertyExpression.PropertyInfo);
            return yPropertyExpression;
        }

        protected override YExpression VisitReturn(YReturnExpression yReturnExpression)
        {
            if (Modified(yReturnExpression.Default, out var @default))
                return new YReturnExpression(yReturnExpression.Target, @default);
            return yReturnExpression;
        }

        protected override YExpression VistiAssign(YAssignExpression yAssignExpression)
        {
            if (Modified(yAssignExpression.left, yAssignExpression.right, out var left, out var right))
                return new YAssignExpression(left, right, right?.Type);
            return yAssignExpression;
        }

        protected override YExpression VisitTypeIs(YTypeIsExpression yTypeIsExpression)
        {
            if (Modified(yTypeIsExpression.Target, out var target))
                return new YTypeIsExpression(target, yTypeIsExpression.Type);
            return yTypeIsExpression;
        }

        protected override YExpression VisitTypeAs(YTypeAsExpression yTypeAsExpression)
        {
            if (Modified(yTypeAsExpression.Target, out var target))
                return new YTypeIsExpression(target, yTypeAsExpression.Type);
            return yTypeAsExpression;
        }

        protected override YExpression VisitIndex(YIndexExpression yIndexExpression)
        {
            var tm = Modified(yIndexExpression.Target, out var target);
            var am = Modified(yIndexExpression.Arguments, out var arguments);
            if (tm || am)
                return new YIndexExpression(target, yIndexExpression.Property, arguments);
            return yIndexExpression;
        }

        protected override YExpression VisitArrayIndex(YArrayIndexExpression yArrayIndexExpression)
        {
            if (Modified(yArrayIndexExpression.Target, yArrayIndexExpression.Index, out var target, out var index))
                return new YArrayIndexExpression(target, index);
            return yArrayIndexExpression;
        }

        protected override YExpression VisitNewArrayBounds(YNewArrayBoundsExpression yNewArrayBoundsExpression)
        {
            if (Modified(yNewArrayBoundsExpression.Size, out var size))
                return new YNewArrayBoundsExpression(yNewArrayBoundsExpression.ElementType, size);
            return yNewArrayBoundsExpression;
        }

        protected override YExpression VisitUnary(YUnaryExpression yUnaryExpression)
        {
            if (Modified(yUnaryExpression.Target, out var target))
                return new YUnaryExpression(target, yUnaryExpression.Operator);
            return yUnaryExpression;
        }
    }
}
