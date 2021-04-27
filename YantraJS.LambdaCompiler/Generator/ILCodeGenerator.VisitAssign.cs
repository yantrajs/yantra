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
                case YExpressionType.Field:
                    AssignField(left as YFieldExpression);
                    return;
            }

            throw new NotImplementedException();
        }

        private void AssignField(YFieldExpression yFieldExpression)
        {
            throw new NotImplementedException();
        }
    }
}
