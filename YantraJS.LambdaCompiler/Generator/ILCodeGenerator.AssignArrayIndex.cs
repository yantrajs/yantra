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
        private CodeInfo AssignArrayIndex(DataSource exp, YArrayIndexExpression yArrayIndexExpression, int savedIndex = -1)
        {
            Visit(yArrayIndexExpression.Target);
            Visit(yArrayIndexExpression.Index);

            VisitSave(exp, savedIndex);

            var code = Type.GetTypeCode(yArrayIndexExpression.Type);
            switch (code)
            {
                case TypeCode.Boolean:
                    il.Emit(OpCodes.Stelem_I1);
                    return true;
                case TypeCode.Byte:
                    il.Emit(OpCodes.Stelem_I1);
                    return true;
                case TypeCode.Char:
                    il.Emit(OpCodes.Stelem_I2);
                    return true;
                case TypeCode.DateTime:
                    il.Emit(OpCodes.Stelem);
                    return true;
                case TypeCode.DBNull:
                    il.Emit(OpCodes.Stelem);
                    return true;
                case TypeCode.Decimal:
                    il.Emit(OpCodes.Stelem);
                    return true;
                case TypeCode.Double:
                    il.Emit(OpCodes.Stelem_R8);
                    return true;
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    il.Emit(OpCodes.Stelem_I2);
                    return true;
                case TypeCode.Int32:
                    il.Emit(OpCodes.Stelem_I4);
                    return true;
                case TypeCode.Int64:
                    il.Emit(OpCodes.Stelem_I8);
                    return true;
                case TypeCode.Object:
                    il.Emit(OpCodes.Stelem_Ref);
                    return true;
                case TypeCode.SByte:
                    il.Emit(OpCodes.Stelem_I1);
                    return true;
                case TypeCode.Single:
                    il.Emit(OpCodes.Stelem_R4);
                    return true;
                case TypeCode.String:
                    il.Emit(OpCodes.Stelem_Ref);
                    return true;
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Stelem_I2);
                    return true;
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Stelem_I4);
                    return true;
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Stelem_I8);
                    return true;
            }

            return true;
        }


    }
}
