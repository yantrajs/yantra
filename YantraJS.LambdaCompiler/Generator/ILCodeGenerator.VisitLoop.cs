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
        protected override CodeInfo VisitLoop(YLoopExpression yLoopExpression)
        {
            var @continue = labels[yLoopExpression.Continue];
            var @break = labels[yLoopExpression.Break];
            il.MarkLabel(@continue);
            Visit(yLoopExpression.Body);
            il.EmptyStack();
            il.Branch(@continue);
            il.MarkLabel(@break);
            return true;
        }
    }
}
