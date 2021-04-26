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
                if (RequiresAddress)
                {
                    if (!v.IsReference)
                    {
                        il.EmitLoadArgAddress(v.Index);
                        return true;
                    }
                }
                il.EmitLoadArg(v.Index);
                return true;
            }

            if (RequiresAddress)
            {
                il.EmitLoadLocalAddress(v.LocalBuilder.LocalIndex);
                return true;
            }
            il.EmitLoadLocal(v.LocalBuilder.LocalIndex);
            return true;
        }
    }
}
