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
            return true;
        }

        protected override CodeInfo VisitNewArray(YNewArrayExpression yNewArrayExpression)
        {

            using (tempVariables.Push())
            {
                var ea = yNewArrayExpression.Elements;

                var elementType = yNewArrayExpression.ElementType;

                // store length...
                il.EmitConstant(ea.Count);

                il.Emit(OpCodes.Newarr, elementType);

                var ve = yNewArrayExpression.Elements.GetFastEnumerator();
                while(ve.MoveNext(out var e, out var i))
                {
                    il.Emit(OpCodes.Dup);
                    il.EmitConstant(i);
                    Visit(e);
                    il.Emit(OpCodes.Stelem, elementType);
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
