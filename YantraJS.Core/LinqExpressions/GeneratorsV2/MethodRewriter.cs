﻿using System.Collections.Generic;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{

    public class MethodRewriter : YExpressionMapVisitor
    {

        public static Expression Rewrite(YExpression exp)
        {
            var rw = new MethodRewriter();
            return rw.Visit(exp);

        }


        protected override Exp VisitLambda(YLambdaExpression yLambdaExpression)
        {
            return yLambdaExpression;
        }

        protected override Exp VisitRelay(YRelayExpression relayExpression)
        {
            return relayExpression;
        }

        protected override Exp VisitAssign(YAssignExpression yAssignExpression)
        {
            var right = yAssignExpression.Right;
            // nested assign should be converted to block if it contains yield...
            if(right.NodeType == YExpressionType.Assign && right.HasYield())
            {
                // we need to break the right..
                right = BreakAssign(right as YAssignExpression);
                var bb = new YBlockBuilder();
                right = bb.ConvertToVariable(right);
                bb.AddExpression(YExpression.Assign(Visit(yAssignExpression.Left), right));
                return bb.Build();
            }

            return base.VisitAssign(yAssignExpression);
        }

        private Exp BreakAssign(YAssignExpression assign)
        {
            var bb = new YBlockBuilder();
            var right = assign.Right;
            if(right.NodeType == YExpressionType.Assign && right.HasYield())
            {
                right = BreakAssign(right as YAssignExpression);
                right = bb.ConvertToVariable(Visit(right));
                bb.AddExpression(YExpression.Assign(Visit(assign.Left), right));
                return bb.Build();
            }
            right = bb.ConvertToVariable(Visit(right));
            bb.AddExpression(YExpression.Assign(Visit(assign.Left), right));
            return bb.Build();
        }

        protected override Expression VisitNew(YNewExpression node)
        {
            if (node.HasYield())
            {
                var bb = new YBlockBuilder();
                var args = new List<YExpression>();
                foreach (var item in node.args)
                {
                    var a = Visit(item);
                    args.Add(bb.ConvertToVariable(a));
                }
                bb.AddExpression(Expression.New(node.constructor, args));
                return bb.Build();

            }
            return base.VisitNew(node);
        }

        protected override Exp VisitUnary(YUnaryExpression yUnaryExpression)
        {
            var target = yUnaryExpression.Target;
            if (target.HasYield())
            {
                // break...
                var bb = new YBlockBuilder();
                target = bb.ConvertToVariable(Visit(target));
                bb.AddExpression(new YUnaryExpression(target, yUnaryExpression.Operator));
                return bb.Build();
            }
            return base.VisitUnary(yUnaryExpression);
        }

        protected override Exp VisitConditional(YConditionalExpression yConditionalExpression)
        {
            var test = yConditionalExpression.test;
            if (test.HasYield())
            {
                var bb = new YBlockBuilder();
                test = bb.ConvertToVariable(Visit(test));
                bb.AddExpression(YExpression.Condition(test, 
                    Visit(yConditionalExpression.@true), 
                    Visit(yConditionalExpression.@false)));
                return bb.Build();
            }
            return base.VisitConditional(yConditionalExpression);
        }

        protected override Exp VisitField(YFieldExpression yFieldExpression)
        {
            if (yFieldExpression.Target == null)
                return yFieldExpression;
            var target = Visit(yFieldExpression.Target);
            if (target.HasYield())
            {
                var bb = new YBlockBuilder();
                target = bb.ConvertToVariable(target);
                bb.AddExpression(YExpression.Field(target, yFieldExpression.FieldInfo));
                return bb.Build();
            }
            return yFieldExpression;
        }

        protected override Exp VisitProperty(YPropertyExpression yPropertyExpression)
        {
            var target = yPropertyExpression.Target;
            if (target == null)
                return yPropertyExpression;
            if (target.HasYield())
            {
                var bb = new YBlockBuilder();
                target = bb.ConvertToVariable(Visit(target));
                bb.AddExpression(YExpression.Property(target, yPropertyExpression.PropertyInfo));
                return bb.Build();
            }
            return yPropertyExpression;
        }

        protected override Exp VisitIndex(YIndexExpression yIndexExpression)
        {
            var hasYield = yIndexExpression.HasYield();
            if(hasYield)
            {
                var bb = new YBlockBuilder();
                var target = Visit(yIndexExpression.Target);
                if (target.HasYield())
                {
                    target = bb.ConvertToVariable(target);
                }

                var args = new List<YExpression>();

                foreach(var item in yIndexExpression.Arguments)
                {
                    var e = Visit(item);
                    args.Add(bb.ConvertToVariable(e));
                }

                bb.AddExpression(YExpression.Index(target, yIndexExpression.Property, args.ToArray()));
                return bb.Build();
            }
            return yIndexExpression;
        }

        protected override Exp VisitArrayIndex(YArrayIndexExpression yArrayIndexExpression)
        {
            var target = Visit(yArrayIndexExpression.Target);
            var index = Visit(yArrayIndexExpression.Index);

            var targetHasYield = target.HasYield();
            var indexHasYield = index.HasYield();

            if(targetHasYield || indexHasYield)
            {
                var bb = new YBlockBuilder();
                    
                if (targetHasYield)
                {
                    target = bb.ConvertToVariable(target);
                }
                if (indexHasYield)
                {
                    index = bb.ConvertToVariable(index);
                }
                bb.AddExpression(YExpression.ArrayIndex(target, index));
                return bb.Build();
            }
            return yArrayIndexExpression;
        }

        protected override Expression VisitCall(YCallExpression node)
        {
            if(node.HasYield())
            {

                // rewrite...
                var bb = new YBlockBuilder();

                var target = Visit(node.Target);
                if (target?.HasYield() ?? false)
                {
                    target = bb.ConvertToVariable(target);
                }

                var args = new List<YExpression>();
                foreach(var item in node.Arguments)
                {
                    var a = Visit(item);
                    args.Add(bb.ConvertToVariable(a));
                }
                bb.AddExpression(Expression.Call(target, node.Method, args));
                return bb.Build();
            }

            return node;
        }
    }

}