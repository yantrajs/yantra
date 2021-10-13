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
        protected override CodeInfo VisitILOffset(YILOffsetExpression node)
        {
            il.EmitConstant(il.ILOffset);
            return true;
        }
    }
}
