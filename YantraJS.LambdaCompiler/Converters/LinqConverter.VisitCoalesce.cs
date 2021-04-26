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
        private YExpression VisitCoalesce(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Method != null)
                throw new NotSupportedException();
            return YExpression.Coalesce(Visit(binaryExpression.Left), Visit(binaryExpression.Right));
        }

    }
}
