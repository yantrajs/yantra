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
        protected override CodeInfo VisitBox(YBoxExpression node)
        {
            Visit(node.Target);
            il.Emit(OpCodes.Box, node.Target.Type);
            return true;
        }
    }
}
