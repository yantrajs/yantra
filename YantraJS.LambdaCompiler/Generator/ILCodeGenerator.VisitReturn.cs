using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
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
                return VisitReturn(def, label, temp.LocalIndex);
            }
            il.Branch(label);
            return true;
        }

        private CodeInfo VisitReturn(YExpression exp, ILWriterLabel label, int localIndex)
        {
            switch (exp.NodeType)
            {
                case YExpressionType.Assign:
                    return VisitReturnAssign(exp as YAssignExpression, label, localIndex);
                case YExpressionType.Block:
                    return VisitReturnBlock(exp as YBlockExpression, label, localIndex);
            }
            Visit(exp);
            il.EmitSaveLocal(localIndex);
            il.Branch(label, localIndex);
            return true;
        }

        private CodeInfo VisitReturnAssign(YAssignExpression assign, ILWriterLabel label, int localIndex)
        {
            Visit(assign.Right);
            il.EmitSaveLocal(localIndex);
            Assign(assign.Left, localIndex);
            il.Branch(label, localIndex);
            return true;
        }

        private CodeInfo VisitReturnBlock(YBlockExpression block, ILWriterLabel label, int localIndex)
        {
            var length = block.Expressions.Length;
            var last = length - 1;

            foreach (var p in block.Variables)
                variables.Create(p);

            for (int i = 0; i < length; i++)
            {
                var exp = block.Expressions[i];
                if(i < last)
                {
                    var r = Visit(exp);
                    if (r.Stack)
                    {
                        il.Emit(OpCodes.Pop);
                    }
                    continue;
                }

                // last item...
                return VisitReturn(exp, label, localIndex);
            }

            throw new InvalidOperationException($"This code is not reachable");
        }
    }
}
