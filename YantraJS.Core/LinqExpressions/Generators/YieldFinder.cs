using System.Linq.Expressions;

namespace YantraJS.Core.LinqExpressions.Generators
{


    public class YieldFinder : ExpressionVisitor
    {

        class ENode: LinkedStackItem<ENode>
        {
            public Expression Node;

            public ENode(Expression node)
            {
                this.Node = node;
            }
        }

        private bool found = false;

        private LinkedStack<ENode> stack = new LinkedStack<ENode>();

        public static void MarkYield(Expression exp)
        {
            var yf = new YieldFinder();
            yf.Visit(exp);
            // do agin to mark gotos...
            yf.Visit(exp);
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
            using (var e = stack.Push(new ENode(node)))
            {
                return base.Visit(node);
            }
        }
    }
}
