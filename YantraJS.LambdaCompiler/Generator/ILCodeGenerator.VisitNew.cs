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
            using (tempVariables.Push())
            {
                var a = EmitParameters(newExpression.constructor, newExpression.args, newExpression.Type);
                if (newExpression.AsCall)
                {
                    il.Emit(OpCodes.Call, newExpression.constructor);
                }
                else
                {
                    il.Emit(OpCodes.Newobj, newExpression.constructor);
                }
                a();
            }
            if (RequiresAddress && newExpression.Type.IsValueType)
            {
                var t = tempVariables[newExpression.Type];
                il.EmitSaveLocal(t.LocalIndex);
                il.EmitLoadLocalAddress(t.LocalIndex);
            }
            return true;
        }

        protected override CodeInfo VisitNewArray(YNewArrayExpression yNewArrayExpression)
        {

            using (tempVariables.Push())
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
        }

        protected override CodeInfo VisitNewArrayBounds(YNewArrayBoundsExpression yNewArrayBoundsExpression)
        {
            Visit(yNewArrayBoundsExpression.Size);
            il.Emit(OpCodes.Newarr, yNewArrayBoundsExpression.ElementType);
            return true;
        }
    }
}
