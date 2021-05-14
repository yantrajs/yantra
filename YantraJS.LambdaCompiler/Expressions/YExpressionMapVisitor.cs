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
            list = statements;
            if (statements.Length == 0)
            {
                return false;
            }
            bool dirty = false;
            var r = new T[statements.Length];
            for (int i = 0; i < statements.Length; i++)
            {
                ref var item = ref statements[i];
                var visitedItem = Visit(item);
                var visited = visitedItem as T;
                if (visited == null)
                    throw new ArgumentNullException();
                if (visited != item)
                    dirty = true;
                r[i] = visited;
            }
            if (!dirty)
            {
                return false;
            }
            list = r;
            return true;
        }

        private bool Modified<T>(in T[] statements, Func<T, T> visitor, out T[] list)
        {
            list = statements;
            if (statements.Length == 0)
            {
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
                return false;
            }
            list = r;
            return true;
        }

        protected override YExpression VisitArrayLength(YArrayLengthExpression arrayLengthExpression)
        {
            if (Modified(arrayLengthExpression.Target, out var target))
                return new YArrayLengthExpression(target);
            return arrayLengthExpression;
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

        protected override YExpression VisitConvert(YConvertExpression convertExpression)
        {
            if (Modified(convertExpression.Target, out var target))
                return YExpression.Convert(target, convertExpression.Type);
            return convertExpression;
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

        protected override YExpression VisitDelegate(YDelegateExpression yDelegateExpression)
        {
            return yDelegateExpression;
        }

        protected override YExpression VisitEmpty(YEmptyExpression exp)
        {
            return exp;
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

        protected override YExpression VisitInvoke(YInvokeExpression invokeExpression)
        {
            var tm = Modified(invokeExpression.Target, out var target);
            var am = Modified(invokeExpression.Arguments, out var args);
            if (tm || am)
                return new YInvokeExpression(target, args, invokeExpression.Type);
            return invokeExpression;
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
            var tm = Modified(yLambdaExpression.This, out var @this);
            if (pm || bm || tm)
                return new YLambdaExpression(yLambdaExpression.Type, yLambdaExpression.Name, body, @this, parameters, yLambdaExpression.ReturnType, yLambdaExpression.Repository);
            return yLambdaExpression;

        }

        protected override YExpression VisitLoop(YLoopExpression yLoopExpression)
        {
            if (Modified(yLoopExpression.Body, out var body))
                return new YLoopExpression(body, yLoopExpression.Break, yLoopExpression.Continue);
            return yLoopExpression;
        }

        protected override YExpression VisitMemberInit(YMemberInitExpression memberInitExpression)
        {
            var ne = Modified(memberInitExpression.Target, out var target);
            var be = Modified(memberInitExpression.Bindings, VisitMemberAssignment, out var bindings);
            if (ne || be)
                return new YMemberInitExpression(target, bindings);
            return memberInitExpression;
        }

        protected virtual  YMemberAssignment VisitMemberAssignment(YMemberAssignment a)
        {
            if (Modified(a.Value, out var v))
                return new YMemberAssignment(a.Member, v);
            return a;
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
                return new YNewArrayExpression(yNewArrayExpression.ElementType, elements);
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

        protected override YExpression VisitRelay(YRelayExpression relayExpression)
        {
            var cm = Modified(relayExpression.Closures, out var closures);
            var lm = Modified(relayExpression.InnerLambda, out var lambda);
                if(cm || lm ) 
                return new YRelayExpression(closures, lambda);
            return relayExpression;
        }

        protected override YExpression VisitReturn(YReturnExpression yReturnExpression)
        {
            if (Modified(yReturnExpression.Default, out var @default))
                return new YReturnExpression(yReturnExpression.Target, @default);
            return yReturnExpression;
        }

        protected override YExpression VisitSwitch(YSwitchExpression node)
        {
            var tOrdModified = Modified(node.Target, node.Default, out var target, out  var @default);
            var casesModified = Modified(node.Cases, VisitSwitchCase, out var cases);
            if(tOrdModified || casesModified)
            {
                return new YSwitchExpression(target, node.CompareMethod, @default, cases);
            }
            return node;
        }

        protected virtual YSwitchCaseExpression VisitSwitchCase(YSwitchCaseExpression @case)
        {
            var tvm = Modified(@case.TestValues, out var tv);
            var bm = Modified(@case.Body, out var body);
            if (tvm || bm)
                return new YSwitchCaseExpression(body, tv);
            return @case;
        }

        protected override YExpression VisitAssign(YAssignExpression yAssignExpression)
        {
            if (Modified(yAssignExpression.Left, yAssignExpression.Right, out var left, out var right))
                return new YAssignExpression(left, right, right?.Type);
            return yAssignExpression;
        }

        protected override YExpression VisitTypeIs(YTypeIsExpression yTypeIsExpression)
        {
            if (Modified(yTypeIsExpression.Target, out var target))
                return new YTypeIsExpression(target, yTypeIsExpression.TypeOperand);
            return yTypeIsExpression;
        }

        protected override YExpression VisitTypeAs(YTypeAsExpression yTypeAsExpression)
        {
            if (Modified(yTypeAsExpression.Target, out var target))
                return new YTypeAsExpression(target, yTypeAsExpression.Type);
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

        protected override YExpression VisitThrow(YThrowExpression throwExpression)
        {
            if (Modified(throwExpression.Expression, out var exp))
                return new YThrowExpression(exp);
            return throwExpression;
        }

        protected override YExpression VisitTryCatchFinally(YTryCatchFinallyExpression tryCatchFinallyExpression)
        {
            var tf = Modified(tryCatchFinallyExpression.Try, tryCatchFinallyExpression.Finally,
                out var @try, out var @finally);
            YCatchBody @catch = tryCatchFinallyExpression.Catch;
            bool cf = false;
            if(tryCatchFinallyExpression.Catch != null)
            {
                cf = Modified(tryCatchFinallyExpression.Catch.Body, out var cb);
                if (cf)
                    @catch = new YCatchBody(tryCatchFinallyExpression.Catch.Parameter, cb);
                
            }
            if(cf || tf)
                return YExpression.TryCatchFinally(@try, @catch, @finally);
            return tryCatchFinallyExpression;
        }
    }
}
