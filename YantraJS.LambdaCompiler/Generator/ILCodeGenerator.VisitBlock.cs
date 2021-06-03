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
            foreach(var p in yBlockExpression.Variables)
            {
                variables.Create(p);
            }
            var expressions = yBlockExpression.Expressions;
            var l = expressions.Length;
            var last = l - 1;
            for (int i = 0; i < l; i++)
            {
                var exp = expressions[i];
                VisitSave(exp, i == last);
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
