using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace YantraJS.Core.LinqExpressions.Generators
{


    public class YieldFinder : ExpressionVisitor
    {

        class ENode: LinkedStackItem<ENode>
        {
            public Expression Node;

            public ENode BlockNode;

            public ENode(Expression node, ENode parent)
            {
                this.Node = node;
                if (node is BlockExpression blockExpression)
                {
                    this.BlockNode = this;
                    this.Block = blockExpression;
                    this.vars = new List<ParameterExpression>(blockExpression.Variables);
                    this.statements = new List<Expression>(blockExpression.Expressions);
                }
                else
                {
                    this.BlockNode = parent.BlockNode;
                    this.Block = parent.Block;
                    this.Current = parent.Current;
                    this.vars = parent.vars;
                    this.statements = parent.statements;
                }
            }

            public ENode()
            {

            }

            public List<ParameterExpression> vars;
            public List<Expression> statements;

            // current expression within the last block...
            public Expression Current;
            // current block...
            public BlockExpression Block;

            public bool Modified = false;

            internal void Insert(Expression n)
            {
                var i = statements.IndexOf(Current);
                statements.Insert(i, n);
                BlockNode.Modified = true;
            }
        }

        private bool found = false;

        private LinkedStack<ENode> stack = new LinkedStack<ENode>();

        public static Expression MarkYield(Expression exp)
        {
            var yf = new YieldFinder();
            exp = yf.Visit(exp);
            // do again to mark gotos...
            return yf.Visit( yf.Visit(exp));
        }

        protected override Expression VisitExtension(Expression node)
        {
            found = found || node is YieldExpression;
            if (found)
            {
                var top = stack.Top;
                while(top != null)
                {
                    top.Node.UpdateExtendedValue((e) => {
                        e.HasYield = true;
                    });
                    top = top.Parent;
                }
            }
            node.UpdateExtendedValue((e) => {
                e.HasYield = true;
            });
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.HasYield())
            {
                var top = stack.Top;

                // break...
                List<Expression> args = new List<Expression>();
                foreach(var arg in node.Arguments)
                {
                    var p = Expression.Parameter(arg.Type);
                    top.vars.Add(p);
                    args.Add(p);
                    top.Insert(Expression.Assign(p,Visit(arg)));
                }
                var ev = node.GetExtendedValue();
                node = node.Update(node.Object, args);
                // node.SetExtendedValue(ev);
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if (node.HasYield())
            {
                var top = stack.Top;

                // break...
                List<Expression> args = new List<Expression>();
                foreach (var arg in node.Arguments)
                {
                    var p = Expression.Parameter(arg.Type);
                    top.vars.Add(p);
                    args.Add(p);
                    top.Insert(Expression.Assign(p, Visit(arg)));
                }
                var ev = node.GetExtendedValue();
                node = node.Update(args);
                // node.SetExtendedValue(ev);

            }
            return base.VisitNew(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            var top = stack.Top;
            LinkedStackItem<ENode> @yield = null;
            while (top != null)
            {
                if(top.Node.GetExtendedValue().HasYield)
                {
                    yield = top;
                    break;
                }
                top = top.Parent;
            }
            if(yield != null)
            {
                top = stack.Top;
                while(top != null)
                {
                    top.Node.UpdateExtendedValue(e => {
                        e.ForceBreak = true;
                    });
                    top = top.Parent;
                    if (top == yield)
                        break;
                }
            }
            node.UpdateExtendedValue(e => e.ForceBreak = true);
            return base.VisitGoto(node);
        }

        public override Expression Visit(Expression node)
        {
            if (node is LambdaExpression)
                return node;
            using (var e = stack.Push(new ENode(node, stack.Top)))
            {
                var r = base.Visit(node);
                return r;
            }
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var top = stack.Top;
            var ev = node.GetExtendedValue();
            foreach (var stmt in node.Expressions)
            {
                stack.Top.Current = stmt;
                var r = Visit(stmt);
                if (r != stmt)
                {
                    var i = top.statements.IndexOf(stmt);
                    top.statements[i] = r;
                    top.Modified = true;
                }
            }
            if (top.Modified)
            {
                node  = node.Update(top.vars, top.statements);
                node.SetExtendedValue(ev);

                // visit again...
                // return VisitBlock(node);
            }
            return node;
        }
    }
}
