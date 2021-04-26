using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitReturn(YReturnExpression yReturnExpression)
        {
            var label = labels[yReturnExpression.Target];
            if(yReturnExpression.Default != null)
            {
                Visit(yReturnExpression.Default);
            }
            Goto(label);
            return true;
        }
    }
}
