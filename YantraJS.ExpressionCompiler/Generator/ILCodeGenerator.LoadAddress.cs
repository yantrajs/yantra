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

        protected override CodeInfo VisitAddressOf(YAddressOfExpression node)
        {
            return LoadAddress(node.Target);
        }

        private CodeInfo LoadAddress(YExpression exp)
        {
            switch (exp.NodeType)
            {
                case YExpressionType.Parameter:
                    return LoadParameterAddress(exp as YParameterExpression);
                case YExpressionType.Field:
                    return LoadFieldAddress(exp as YFieldExpression);
                case YExpressionType.ArrayIndex:
                    return LoadArrayIndexAddress(exp as YArrayIndexExpression);

            }
            var temp = tempVariables[exp.Type];
            Visit(exp);
            il.EmitSaveLocal(temp.LocalIndex);
            il.EmitLoadLocalAddress(temp.LocalIndex);
            return true;
        }

        private CodeInfo LoadArrayIndexAddress(YArrayIndexExpression yArrayIndexExpression)
        {
            Visit(yArrayIndexExpression.Target);
            Visit(yArrayIndexExpression.Index);

            var type = yArrayIndexExpression.Type;

            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldelema, type);
                return true;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                case TypeCode.String:
                    il.Emit(OpCodes.Ldelem_Ref);
                    return true;
            }
            il.Emit(OpCodes.Ldelema, type);
            return true;

        }

        private CodeInfo LoadFieldAddress(YFieldExpression yFieldExpression)
        {
            var field = yFieldExpression.FieldInfo;
            if (field.IsStatic)
            {
                if (field.IsLiteral)
                {
                    throw new InvalidOperationException();
                }
                il.Emit(OpCodes.Ldsflda, field);
                return true;
            }

            Visit(yFieldExpression.Target);
            il.Emit(OpCodes.Ldflda, field);
            return true;
        }

        private CodeInfo LoadParameterAddress(YParameterExpression yParameterExpression)
        {
            var varInfo = variables[yParameterExpression];
            if (varInfo.IsArgument) {
                if(varInfo.IsReference)
                {
                    il.EmitLoadArg(varInfo.Index);
                    return true;
                }
                il.EmitLoadArgAddress(varInfo.Index);
                return true;
            }
            il.EmitLoadLocalAddress(varInfo.LocalBuilder.LocalIndex);
            return true;
        }
    }
}
