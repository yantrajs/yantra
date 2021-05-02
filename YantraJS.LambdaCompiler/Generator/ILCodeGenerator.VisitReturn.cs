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
        private int ReturnLocal = -1;

        protected override CodeInfo VisitReturn(YReturnExpression yReturnExpression)
        {
            var label = labels[yReturnExpression.Target];
            var def = yReturnExpression.Default;
            if(def != null)
            {
                var temp = tempVariables[def.Type];
                ReturnLocal = temp.LocalIndex;
                //if (def.NodeType == YExpressionType.Assign)
                //{
                //    var be = def as YAssignExpression;
                //    Visit(be.Right);
                //    il.Emit(OpCodes.Dup);
                //    il.EmitSaveLocal(temp.LocalIndex);
                //    Assign(be.Left);
                //}
                //else
                //{

                //    Visit(def);
                //    il.EmitSaveLocal(temp.LocalIndex);
                //}

                Visit(def);
                il.EmitSaveLocal(temp.LocalIndex);
                ReturnLocal = -1;
                il.Branch(label, temp.LocalIndex);
                return true;
            }
            il.Branch(label);
            return true;
        }
    }
}
