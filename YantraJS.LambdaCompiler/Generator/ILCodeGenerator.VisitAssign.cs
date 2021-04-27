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
        protected override CodeInfo VisitAssign(YAssignExpression yAssignExpression)
        {
            // we need to investigate each type of expression on the left...
            Visit(yAssignExpression.Right);
            return Assign(yAssignExpression.Left);
        }

        private CodeInfo Assign(YExpression left)
        {
            switch (left.NodeType)
            {
                case YExpressionType.Parameter:
                    return AssignParameter(left as YParameterExpression);
                case YExpressionType.Field:
                    return AssignField(left as YFieldExpression);
                case YExpressionType.Property:
                    return AssignProperty(left as YPropertyExpression);
                case YExpressionType.Assign:
                    il.Emit(OpCodes.Dup);
                    return VisitAssign(left as YAssignExpression);
                case YExpressionType.ArrayIndex:
                    return AssignArrayIndex(left as YArrayIndexExpression);
                case YExpressionType.Index:
                    return AssignIndex(left as YIndexExpression);
            }

            throw new NotImplementedException();
        }

        private CodeInfo AssignIndex(YIndexExpression yIndexExpression)
        {
            var temp = tempVariables[yIndexExpression.Type];
            il.EmitSaveLocal(temp.LocalIndex);
            using (this.addressScope.Push(true))
            {
                Visit(yIndexExpression.Target);
                il.EmitLoadLocal(temp.LocalIndex);
                il.EmitCall(yIndexExpression.SetMethod);
            }
            return true;
        }

        private CodeInfo AssignProperty(YPropertyExpression yPropertyExpression)
        {
            var temp = tempVariables[yPropertyExpression.Type];
            il.EmitSaveLocal(temp.LocalIndex);
            using (this.addressScope.Push(true))
            {
                Visit(yPropertyExpression.Target);
                il.EmitLoadLocal(temp.LocalIndex);
                il.EmitCall(yPropertyExpression.SetMethod);
            }
            return true;
        }

        private CodeInfo AssignField(YFieldExpression yFieldExpression)
        {
            var temp = tempVariables[yFieldExpression.Type];
            il.EmitSaveLocal(temp.LocalIndex);
            using (this.addressScope.Push(true))
            {
                Visit(yFieldExpression.Target);
                il.EmitLoadLocal(temp.LocalIndex);
                il.Emit(OpCodes.Stfld, yFieldExpression.FieldInfo);
            }
            return true;
        }
    }
}
