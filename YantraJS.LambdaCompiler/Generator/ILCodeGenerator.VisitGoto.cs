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

        protected override CodeInfo VisitGoto(YGoToExpression yGoToExpression)
        {
            PushBranch(() =>
            {
                if (yGoToExpression.Default != null)
                {
                    Visit(yGoToExpression.Default);
                }
                il.Emit(OpCodes.Br, labels[yGoToExpression.Target]);
            });
            return true;
        }

    }
}
