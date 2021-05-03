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

        private CodeInfo Assign(YExpression left, int savedIndex = -1)
        {
            switch (left.NodeType)
            {
                case YExpressionType.Parameter:
                    return AssignParameter(left as YParameterExpression);
                case YExpressionType.Field:
                    return AssignField(left as YFieldExpression, savedIndex);
                case YExpressionType.Property:
                    return AssignProperty(left as YPropertyExpression, savedIndex);
                case YExpressionType.Assign:
                    var a = left as YAssignExpression;
                    if (savedIndex >= 0) {
                        il.EmitLoadLocal(savedIndex);
                        return Assign(a.Right, savedIndex);
                    }
                    Visit(a.Right);
                    return Assign(a.Left, savedIndex);
                case YExpressionType.ArrayIndex:
                    return AssignArrayIndex(left as YArrayIndexExpression, savedIndex);
                case YExpressionType.Index:
                    return AssignIndex(left as YIndexExpression, savedIndex);
            }

            throw new NotImplementedException();
        }

        private CodeInfo AssignIndex(YIndexExpression yIndexExpression, int savedIndex = -1)
        {
            if (savedIndex == -1)
            {
                var temp = tempVariables[yIndexExpression.Type];
                savedIndex = temp.LocalIndex;
                il.EmitSaveLocal(temp.LocalIndex);
            }
            Visit(yIndexExpression.Target);
            il.EmitLoadLocal(savedIndex);
            il.EmitCall(yIndexExpression.SetMethod);
            return true;
        }

        private CodeInfo AssignProperty(YPropertyExpression yPropertyExpression, int savedIndex = -1)
        {
            if (savedIndex == -1)
            {
                var temp = tempVariables[yPropertyExpression.Type];
                savedIndex = temp.LocalIndex;
                il.EmitSaveLocal(temp.LocalIndex);
            }
            // using (this.addressScope.Push(true))
            {
                if (!yPropertyExpression.IsStatic)
                    Visit(yPropertyExpression.Target);
                il.EmitLoadLocal(savedIndex);
                il.EmitCall(yPropertyExpression.SetMethod);
            }
            return true;
        }

        private CodeInfo AssignField(YFieldExpression yFieldExpression, int savedIndex = -1)
        {
            if (savedIndex == -1)
            {
                var temp = tempVariables[yFieldExpression.Type];
                savedIndex = temp.LocalIndex;
                il.EmitSaveLocal(temp.LocalIndex);
            }
            using (this.addressScope.Push(yFieldExpression.FieldInfo.FieldType.IsValueType))
            {
                if (!yFieldExpression.FieldInfo.IsStatic)
                    Visit(yFieldExpression.Target);
                il.EmitLoadLocal(savedIndex);
                il.Emit(OpCodes.Stfld, yFieldExpression.FieldInfo);
            }
            return true;
        }
    }
}
