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
            // check if it is marked as a closure...

            if (closureRepository.TryGet(yParameterExpression, out var ve))
            {
                this.InitializeClosure(yParameterExpression);
                return Visit(ve);
            }

            var v = variables[yParameterExpression];
            il.Comment($"Load {v.Name}");
            var isValueType = yParameterExpression.Type.IsValueType;
            if (isValueType)
            {
                if (v.IsArgument)
                {
                    //if (RequiresAddress)
                    //{
                    //    if (!v.IsReference)
                    //    {
                    //        il.EmitLoadArgAddress(v.Index);
                    //        return true;
                    //    }
                    //}
                    il.EmitLoadArg(v.Index);
                    return true;
                }

                //if (RequiresAddress)
                //{
                //    il.EmitLoadLocalAddress(v.LocalBuilder.LocalIndex);
                //    return true;
                //}
                il.EmitLoadLocal(v.LocalBuilder.LocalIndex);
                return true;
            }
            if (v.IsArgument)
            {
                // irrespective of RequiresAddress
                // retype always load ref...
                il.EmitLoadArg(v.Index);
                //if (RequiresAddress && v.IsReference)
                //{
                //    il.Emit(OpCodes.Ldind_Ref);
                //    return true;
                //}
                return true;
            }

            il.EmitLoadLocal(v.LocalBuilder.LocalIndex);
            if (v.IsReference)
            {
                throw new NotSupportedException();
            }

            return true;
        }
    }
}
