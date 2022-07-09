using System;
using System.Collections.Generic;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{
    public class FlattenBlocks: YExpressionMapVisitor
    {
        //protected override Exp VisitRelay(YRelayExpression relayExpression)
        //{
        //    return relayExpression;
        //}

        protected override Exp VisitLambda(YLambdaExpression yLambdaExpression)
        {
            return yLambdaExpression;
        }

        protected override Expression VisitBinary(YBinaryExpression node)
        {

            if(Flatten(node.Right, last => node.Update(node.Left, node.Operator, last), out var result))
            {
                return result;
            }

            return base.VisitBinary(node);
        }

        protected override Exp VisitAssign(YAssignExpression node)
        {
            if (Flatten(node.Right, last => new YAssignExpression(node.Left, last, null), out var result))
                return result;
            return base.VisitAssign(node);
        }

        protected override Exp VisitReturn(YReturnExpression node)
        {
            if (Flatten(node.Default, x => node.Update(node.Target, x), out var block))
                return block;
            return base.VisitReturn(node);
        }

        protected override Expression VisitNew(YNewExpression node)
        {
            var vars = new Sequence<ParameterExpression>();
            var args = new Sequence<Expression>();
            var list = new Sequence<Expression>();
            var argEn = node.args.GetFastEnumerator();
            while(argEn.MoveNext(out var a))
            {
                var e = Visit(a);
                if(e.NodeType == YExpressionType.Block && e is YBlockExpression block)
                {
                    vars.AddRange(block.Variables);
                    var p = Expression.Parameter(e.Type);
                    vars.Add(p);
                    args.Add(p);
                    var length = block.Expressions.Count;
                    var last = length - 1;
                    var en = block.Expressions.GetFastEnumerator();
                    while(en.MoveNext(out var exp, out var i))
                    {
                        var be = Visit(exp);
                        if(i == last)
                        {
                            list.Add(Expression.Assign(p, be));
                            continue;
                        }
                        list.Add(be);
                    }
                    continue;
                }
                args.Add(e);
            }
            if (vars.Count == 0)
            {
                return Expression.New(node.constructor, args);
            }
            list.Add(Expression.New(node.constructor, args));
            // return base.VisitNew(node);
            return Expression.Block(vars, list);
        }

        protected override Expression VisitBlock(YBlockExpression node)
        {
            var vars = new Sequence<ParameterExpression>( node.Variables);
            var list = new Sequence<Expression>(node.Expressions.Count);
            var en = node.Expressions.GetFastEnumerator();
            while(en.MoveNext(out var e))
            {
                var visited = Visit(e);
                if(visited.NodeType == YExpressionType.Block && visited is YBlockExpression block)
                {
                    vars.AddRange(block.Variables);
                    list.AddRange(block.Expressions);
                    continue;
                }
                list.Add(visited);
            }
            return Expression.Block(vars, list);
        }

        private bool Flatten(Expression exp, Func<Expression, Expression> p, out Expression result)
        {
            if (exp.NodeType != YExpressionType.Block) {
                result = null;
                return false;
            }
            var block = exp as YBlockExpression;
            
            var vars = block.Variables;
            var length = block.Expressions.Count;
            var list = new Sequence<Expression>(length);
            var last = length - 1;
            var en = block.Expressions.GetFastEnumerator();
            while(en.MoveNext(out var e, out var i))
            {
                // var e = block.Expressions[i];
                if (last == i)
                {
                    list.Add(p(Visit(e)));
                    continue;
                }
                list.Add(Visit(e));
            }
            result = Expression.Block(vars, list);
            return true;
            
        }
    }
}
