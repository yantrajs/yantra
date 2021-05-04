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
                var c = Visit(exp);

                // do not pop for last item...
                if(i > last)
                {
                    if (c.Stack)
                    {
                        il.Emit(OpCodes.Pop);
                    }
                } 
            }
            return true;
        }
    }
}
