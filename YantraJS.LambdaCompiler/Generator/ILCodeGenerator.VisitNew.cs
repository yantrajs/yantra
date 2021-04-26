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
        protected override CodeInfo VisitNew(YNewExpression newExpression)
        {
            EmitParameters(newExpression.constructor, newExpression.args);     
            il.Emit(OpCodes.Newobj, newExpression.constructor);
            return true;
        }

        protected override CodeInfo VisitNewArray(YNewArrayExpression yNewArrayExpression)
        {

            var ea = yNewArrayExpression.Elements;

            // store length...
            il.EmitConstant(yNewArrayExpression.Elements?.Length ?? 0);

            il.Emit(OpCodes.Newarr, yNewArrayExpression.ElementType);

            if (ea == null)
                return true;

            for (int i = 0; i < ea.Length; i++)
            {
                il.Emit(OpCodes.Dup);
                il.EmitConstant(i);
                Visit(ea[i]);
                il.Emit(OpCodes.Stelem_Ref);
            }

            return true;
        }

        protected override CodeInfo VisitNewArrayBounds(YNewArrayBoundsExpression yNewArrayBoundsExpression)
        {
            il.EmitConstant(yNewArrayBoundsExpression.Size);
            il.Emit(OpCodes.Newarr, yNewArrayBoundsExpression.ElementType);
            return true;
        }
    }
}
