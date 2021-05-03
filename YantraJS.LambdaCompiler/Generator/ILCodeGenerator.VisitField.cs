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
        protected override CodeInfo VisitField(YFieldExpression yFieldExpression)
        {

            var field = yFieldExpression.FieldInfo;
            if (field.IsStatic)
            {

                if (field.IsLiteral)
                {
                    il.EmitConstant( field.GetRawConstantValue());
                    return true;
                }

                //if (RequiresAddress && field.FieldType.IsValueType)
                //{
                //    il.Emit(OpCodes.Ldsflda, field);
                //    return true;
                //}
                il.Emit(OpCodes.Ldsfld, field);
                return true;
            }

            Visit(yFieldExpression.Target);

            //if (RequiresAddress && field.FieldType.IsValueType)
            //{
            //    il.Emit(OpCodes.Ldflda, field);
            //    return true;
            //}

            il.Emit(OpCodes.Ldfld, field);
            return true;
        }
    }
}
