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
        private CodeInfo AssignParameter(DataSource exp, YParameterExpression yParameterExpression, int savedIndex)
        {
            if (closureRepository.TryGet(yParameterExpression, out var ve))
            {
                InitializeClosure(yParameterExpression);
                return Assign(ve, exp, savedIndex);
            }

            VisitSave(exp, savedIndex);


            var pType = yParameterExpression.Type;
            var varInfo = variables[yParameterExpression];

            il.Comment($"save {varInfo.Name}");

            if (pType.IsByRef)
            {
                AssignRefParameter(varInfo, pType);
                return true;
            }

            if (varInfo.IsArgument)
            {
                // il.Emit(OpCodes.Starg_S, OpCodes.Starg, varInfo.Index);
                il.EmitSaveArg(varInfo.Index);
                return true;
            }
            var i = varInfo.LocalBuilder.LocalIndex;
            //switch (i) {
            //    case 0:
            //        il.Emit(OpCodes.Stloc_0);
            //        return true;
            //    case 1:
            //        il.Emit(OpCodes.Stloc_1);
            //        return true;
            //    case 2:
            //        il.Emit(OpCodes.Stloc_2);
            //        return true;
            //    case 3:
            //        il.Emit(OpCodes.Stloc_3);
            //        return true;
            //}
            // il.Emit(OpCodes.Stloc_S, OpCodes.Stloc, i);
            il.EmitSaveLocal(i);
            return true;
        }

        private void AssignRefParameter(Variable varInfo, Type pType)
        {
            if (!varInfo.IsArgument)
                throw new NotSupportedException();
            il.EmitLoadArg(varInfo.Index);
            var code = Type.GetTypeCode(pType);
            switch (code)
            {
                case TypeCode.Boolean:
                    il.Emit(OpCodes.Stind_I1);
                    return;
                case TypeCode.Byte:
                    il.Emit(OpCodes.Stind_I1);
                    return;
                case TypeCode.Char:
                    il.Emit(OpCodes.Stind_I2);
                    return;
                case TypeCode.DateTime:
                    il.Emit(OpCodes.Stobj);
                    return;
                case TypeCode.DBNull:
                    il.Emit(OpCodes.Stobj);
                    return;
                case TypeCode.Decimal:
                    il.Emit(OpCodes.Stobj);
                    return;
                case TypeCode.Double:
                    il.Emit(OpCodes.Stind_R8);
                    return;
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    il.Emit(OpCodes.Stind_I2);
                    return;
                case TypeCode.Int32:
                    il.Emit(OpCodes.Stind_I4);
                    return;
                case TypeCode.Int64:
                    il.Emit(OpCodes.Stind_I8);
                    return;
                case TypeCode.Object:
                    il.Emit(OpCodes.Stind_Ref);
                    return;
                case TypeCode.SByte:
                    il.Emit(OpCodes.Stind_I1);
                    return;
                case TypeCode.Single:
                    il.Emit(OpCodes.Stind_R4);
                    return;
                case TypeCode.String:
                    il.Emit(OpCodes.Stind_Ref);
                    return;
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Stind_I2);
                    return;
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Stind_I4);
                    return;
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Stind_I8);
                    return;
            }
            throw new NotImplementedException();
        }

    }
}
