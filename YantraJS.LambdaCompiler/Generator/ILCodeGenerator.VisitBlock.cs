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
            foreach (var statement in yBlockExpression.Expressions) {
                Visit(statement);
            }
            return true;
        }
    }
}
