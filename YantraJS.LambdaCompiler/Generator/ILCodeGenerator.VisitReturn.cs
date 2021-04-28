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

                if(il.IsTryBlock)
                {
                    var temp = tempVariables[yReturnExpression.Default.Type];
                    il.Branch(label, temp.LocalIndex);
                    return true;
                }
            }
            Goto(label);
            return true;
        }
    }
}
