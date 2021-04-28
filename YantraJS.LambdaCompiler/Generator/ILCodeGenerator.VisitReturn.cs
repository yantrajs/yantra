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
        protected override CodeInfo VisitReturn(YReturnExpression yReturnExpression)
        {
            var label = labels[yReturnExpression.Target];
            var def = yReturnExpression.Default;
            if(def != null)
            {
                var temp = tempVariables[def.Type];
                if (def.NodeType == YExpressionType.Assign)
                {
                    var be = def as YAssignExpression;
                    Visit(be.Right);
                    il.Emit(OpCodes.Dup);
                    il.EmitSaveLocal(temp.LocalIndex);
                    Assign(be.Left);
                }
                else
                {
                    Visit(def);
                    il.EmitSaveLocal(temp.LocalIndex);
                }

                il.Branch(label, temp.LocalIndex);
                return true;
            }
            il.Branch(label);
            return true;
        }
    }
}
