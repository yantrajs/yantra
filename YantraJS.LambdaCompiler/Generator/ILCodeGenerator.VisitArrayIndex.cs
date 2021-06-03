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

        protected override CodeInfo VisitArrayIndex(YArrayIndexExpression yArrayIndexExpression)
        {
            Visit(yArrayIndexExpression.Target);
            Visit(yArrayIndexExpression.Index);

            var type = yArrayIndexExpression.Type;

            //var requiresAddress = this.RequiresAddress;
            //if (requiresAddress)
            //{
            //    if (type.IsValueType)
            //    {
            //        il.Emit(OpCodes.Ldelema);
            //        return true;
            //    }
            //    switch (Type.GetTypeCode(type))
            //    {
            //        case TypeCode.Object:
            //        case TypeCode.String:
            //            il.Emit(OpCodes.Ldelem_Ref);
            //            return true;
            //    }
            //    il.Emit(OpCodes.Ldelema);
            //    return true;
            //}

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    il.Emit(OpCodes.Ldelem_I1);
                    break;
                case TypeCode.Int16:
                    il.Emit(OpCodes.Ldelem_I2);
                    break;
                case TypeCode.Int32:
                    il.Emit(OpCodes.Ldelem_I4);
                    break;
                case TypeCode.Int64:
                    il.Emit(OpCodes.Ldelem_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Ldelem_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Ldelem_R8);
                    break;
                case TypeCode.SByte:
                    il.Emit(OpCodes.Ldelem_U1);
                    break;
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Ldelem_U2);
                    break;
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldelem_U4);
                    break;
                case TypeCode.String:
                    il.Emit(OpCodes.Ldelem_Ref);
                    break;
                default:
                    il.Emit(OpCodes.Ldelem, yArrayIndexExpression.Type);
                    break;
            }

            return true;
        }

    }
}
