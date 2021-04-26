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
        protected override CodeInfo VistiAssign(YAssignExpression yAssignExpression)
        {
            // we need to investigate each type of expression on the left...
            Visit(yAssignExpression.Right);
            il.Emit(OpCodes.Dup);

            Assign(yAssignExpression.Left);
            return true;
        }

        private void Assign(YExpression left)
        {
            switch (left.NodeType)
            {
                case YExpressionType.Parameter:
                    AssignParameter(left as YParameterExpression);
                    return;

            }
        }

        private void AssignParameter(YParameterExpression yParameterExpression)
        {
            var pType = yParameterExpression.Type;
            var varInfo = variables[yParameterExpression];

            if(pType.IsByRef)
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
}
