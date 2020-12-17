using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public delegate void GeneratorDelegate();

    public class Generator
    {

        public Generator()
        {

        }

    }

    public class YieldExpression: Expression
    {
        public YieldExpression New(Expression argument)
        {
            return new YieldExpression(argument);
        }

        private YieldExpression(Expression argument)
        {
            Argument = argument;
        }

        public Expression Argument { get; }

        public override Type Type => Argument.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;
    }

    public class YieldRewriter: ExpressionVisitor
    {
        public static Expression Rewrite(Expression body)
        {
            return (new YieldRewriter()).Visit(body);
        }

        public YieldRewriter()
        {

        }

        //protected override Expression VisitBlock(BlockExpression node)
        //{
        //    List<Expression> body = new List<Expression>();
        //    List<Expression> afterYield = null;
        //    foreach(var child in node.Expressions)
        //    {
        //        if (YieldFinder.ContainsYield(child))
        //        {
        //            // we need to break here...
        //            afterYield = afterYield ?? new List<Expression>();
        //            break;
        //        }
        //        body.Add(child);
        //    }
        //}
    }

    public class YieldFinder: ExpressionVisitor {

        private bool found = false;

        public static bool ContainsYield(Expression node)
        {
            var finder = new YieldFinder();
            finder.Visit(node);
            return finder.found;
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node is YieldExpression)
            {
                found = found || true;
            }
            return node;
        }

        public override Expression Visit(Expression node)
        {
            if (node is LambdaExpression)
                return node;
            return base.Visit(node);
        }
    }
}
