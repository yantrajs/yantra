using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Converters
{

    public partial class LinqConverter
    {
        protected override YExpression VisitBlock(BlockExpression node)
        {
            var list = Register(node.Variables);
            var s = node.Expressions.Select(b => Visit(b)).ToArray();
            return YExpression.Block(list, s);
        }

    }
}
