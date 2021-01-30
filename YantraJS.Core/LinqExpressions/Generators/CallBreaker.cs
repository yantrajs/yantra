using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public class CallBreaker: ExpressionVisitor
    {

        public CallBreaker()
        {

        }

        class ENode : LinkedStackItem<ENode>
        {
            public Expression Node;

            public ENode(Expression node)
            {
                this.Node = node;
            }

            public List<ParameterExpression> vars = new List<ParameterExpression>();
            public List<Expression> statements = new List<Expression>();
        }

        private LinkedStack<ENode> stack = new LinkedStack<ENode>();

        protected override Expression VisitBlock(BlockExpression node)
        {
            // we will reduce the block with broken calls

            if (!node.HasYield())
                return node;

            using (var top = stack.Push(new ENode(node)))
            {
                var statements = top.statements;
                var vars = top.vars;
                foreach (var exp in node.Expressions)
                {
                    if (!exp.HasYield())
                    {
                        statements.Add(exp);
                        continue;
                    }

                    this.Lift(exp, vars, statements);
                }

                return base.VisitBlock(node);
            }
        }

        private void Lift(Expression exp, List<ParameterExpression> vars, List<Expression> statements)
        {
            throw new NotImplementedException();
        }
    }
}
