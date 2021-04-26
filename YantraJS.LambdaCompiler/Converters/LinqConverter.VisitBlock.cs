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
        private YExpression VisitBlock(BlockExpression blockExpression)
        {
            var list = Register(blockExpression.Variables);
            var s = blockExpression.Expressions.Select(b => Visit(b)).ToArray();
            return YExpression.Block(list, s);
        }

    }
}
