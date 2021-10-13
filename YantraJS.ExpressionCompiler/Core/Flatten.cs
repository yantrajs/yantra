using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Core
{
    //public class FlattenVisitor: YantraJS.Expressions.YExpressionMapVisitor
    //{

    //    protected override YExpression VisitTypeAs(YTypeAsExpression node)
    //    {
    //        if(node.Type.IsAssignableFrom(node.Target.Type))
    //        {
    //            return Visit(node.Target);
    //        }
    //        return base.VisitTypeAs(node);
    //    }

    //    protected override YExpression VisitAssign(YAssignExpression yAssignExpression)
    //    {
    //        if (Flatten(yAssignExpression.Right, x => new YAssignExpression(yAssignExpression.Left, x, x.Type), out var block))
    //            return block;
    //        return base.VisitAssign(yAssignExpression);
    //    }

    //    protected override YExpression VisitReturn(YReturnExpression node)
    //    {
    //        if (Flatten(node.Default, x => YExpression.Return(node.Target, x), out var block))
    //            return block;
    //        return base.VisitReturn(node);
    //    }

    //    protected override YExpression VisitMemberInit(YMemberInitExpression memberInitExpression)
    //    {
    //        var meTarget = Visit(memberInitExpression.Target);
    //        if (meTarget.NodeType == YExpressionType.Block && meTarget is YBlockExpression block)
    //        {
    //            var list = new List<YExpression>();
    //            var length = block.Expressions.Length;
    //            var last = length - 1;
    //            for (int i = 0; i < length; i++)
    //            {
    //                var e = block.Expressions[i];
    //                if (i == last)
    //                {
    //                    list.Add(YExpression.MemberInit(e as YNewExpression, memberInitExpression.Bindings));
    //                    continue;
    //                }
    //                list.Add(e);
    //            }

    //            return YExpression.Block(list.ToArray());
    //        }
    //        return base.VisitMemberInit(memberInitExpression);
    //    }

    //    protected override YExpression VisitNew(YNewExpression node)
    //    {
    //        var vars = new List<YParameterExpression>();
    //        var args = new List<YExpression>();
    //        var list = new List<YExpression>();
    //        foreach (var a in node.args)
    //        {
    //            var e = Visit(a);
    //            if (e.NodeType == YExpressionType.Block && e is YBlockExpression block)
    //            {
    //                vars.AddRange(block.Variables);
    //                var p = YExpression.Parameter(e.Type);
    //                vars.Add(p);
    //                args.Add(p);
    //                var length = block.Expressions.Length;
    //                var last = length - 1;
    //                for (int i = 0; i < length; i++)
    //                {
    //                    var be = Visit(block.Expressions[i]);
    //                    if (i == last)
    //                    {
    //                        list.Add(YExpression.Assign(p, be));
    //                        continue;
    //                    }
    //                    list.Add(be);
    //                }
    //                continue;
    //            }
    //            args.Add(e);
    //        }

    //        if (!list.Any())
    //        {
    //            return node.Update(node.constructor, args.ToArray());
    //        }

    //        list.Add(node.Update(node.constructor, args.ToArray()));
    //        // return base.VisitNew(node);
    //        return YExpression.Block(vars, list.ToArray());
    //    }

    //    protected override YExpression VisitBlock(YBlockExpression node)
    //    {
    //        var vars = new List<YParameterExpression>(node.Variables);
    //        var list = new List<YExpression>();
    //        foreach (var e in node.Expressions)
    //        {
    //            var visited = Visit(e);
    //            if (visited.NodeType == YExpressionType.Block && visited is YBlockExpression block)
    //            {
    //                vars.AddRange(block.Variables);
    //                list.AddRange(block.Expressions);
    //                continue;
    //            }
    //            list.Add(visited);
    //        }
    //        return YExpression.Block(vars, list.ToArray());
    //    }

    //    private bool Flatten(YExpression target, Func<YExpression, YExpression> lastf, out YExpression result)
    //    {
    //        if (target.NodeType != YExpressionType.Block)
    //        {
    //            result = default;
    //            return false;
    //        }

    //        var block = target as YBlockExpression;

    //        var list = new List<YExpression>();
    //        var vars = new List<YParameterExpression>( block.Variables);
    //        var length = block.Expressions.Length;
    //        var last = length - 1;
    //        for (int i = 0; i < length; i++)
    //        {
    //            var e = block.Expressions[i];
    //            var visited = Visit(e);
    //            if (last == i)
    //            {
    //                visited = lastf(visited);
    //            }

    //            if(visited.NodeType == YExpressionType.Block && visited is YBlockExpression visitedBlock)
    //            {
    //                vars.AddRange(visitedBlock.Variables);
    //                list.AddRange(visitedBlock.Expressions);
    //                continue;
    //            }

    //            list.Add(visited);
    //        }
    //        result = YExpression.Block(vars, list.ToArray());
    //        return true;
    //    }

    //}
}
