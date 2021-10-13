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
            // need to store variable in temp
            //il.Branch()


            //PushBranch(() =>
            //{
            //    if (yGoToExpression.Default != null)
            //    {
            //        Visit(yGoToExpression.Default);
            //    }
            //    il.Emit(OpCodes.Br, labels[yGoToExpression.Target]);
            //});
            il.Branch(labels[yGoToExpression.Target]);
            return true;
        }

    }
}
