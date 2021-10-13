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
        protected override CodeInfo VisitBlock(YBlockExpression yBlockExpression)
        {
            using var tvs = tempVariables.Push();
            foreach(var p in yBlockExpression.FlattenVariables)
            {
                variables.Create(p, tvs);
            }
            var expressions = yBlockExpression.FlattenExpressions;
            foreach(var (exp, last) in expressions)
            {
                VisitSave(exp, last);
            }
            return true;
        }

        private CodeInfo VisitSave(YExpression exp, bool save)
        {
            if(exp.NodeType == YExpressionType.Assign)
            {
                if (!save)
                {
                    return VisitAssign(exp as YAssignExpression, -1);
                }
            }
            Visit(exp);
            if (!save)
            {
                if (exp.Type != typeof(void))
                {
                    il.Emit(OpCodes.Pop);
                }
            }
            return true;
        }
    }
}
