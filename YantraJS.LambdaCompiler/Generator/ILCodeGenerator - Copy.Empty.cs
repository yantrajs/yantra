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
        protected override CodeInfo VisitParameter(YParameterExpression yParameterExpression)
        {
            var v = variables[yParameterExpression];
            if (v.IsArgument)
            {
                switch (v.Index)
                {
                    case 0:
                        il.Emit(OpCodes.Ldarg_0);
                        return true;
                    case 1:
                        il.Emit(OpCodes.Ldarg_1);
                        return true;
                    case 2:
                        il.Emit(OpCodes.Ldarg_2);
                        return true;
                    case 3:
                        il.Emit(OpCodes.Ldarg_3);
                        return true;
                }
                il.Emit(OpCodes.Ldarg_S, v.Index);
                return true;
            }
            return true;
        }
    }
}
