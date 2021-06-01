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

        protected override CodeInfo VisitDebugInfo(YDebugInfoExpression node)
        {
            SequencePoints.Add(new (il.ILOffset, node.Start, node.End));
            return true;
        }

    }
}
