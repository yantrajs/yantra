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
        protected override CodeInfo VisitReturn(YReturnExpression yReturnExpression)
        {
            var label = labels[yReturnExpression.Target];
            var def = yReturnExpression.Default;
            if(def != null)
            {

                if(!il.IsTryBlock)
                {
                    if(def.NodeType == YExpressionType.Call)
                    {
                        if(yReturnExpression.Type.IsAssignableFrom(def.Type))
                        {
                            // tail call....
                            if (VisitTailCall(def as YCallExpression))
                                return true;
                        }
                        Visit(yReturnExpression.Default);
                        il.Emit(OpCodes.Ret);
                        return true;
                    }
                }
                using var temp = il.NewTemp(def.Type);
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

                // tail call...
            }
            Visit(exp);
            if(!il.IsTryBlock)
            {
                il.Emit(OpCodes.Ret);
                // il.ClearStack();
                return true;
            }
            il.EmitSaveLocal(localIndex);
            il.Branch(label, localIndex);
            return true;
        }

        private CodeInfo VisitReturnAssign(YAssignExpression assign, ILWriterLabel label, int localIndex)
        {
            //Visit(assign.Right);
            //il.EmitSaveLocal(localIndex);
            //Assign(assign.Left, localIndex);
            VisitAssign(assign, localIndex);
            if (!il.IsTryBlock)
            {
                il.EmitLoadLocal(localIndex);
                il.Emit(OpCodes.Ret);
                // il.ClearStack();
                return true;
            }
            il.Branch(label, localIndex);
            return true;
        }

        private CodeInfo VisitReturnBlock(YBlockExpression block, ILWriterLabel label, int localIndex)
        {
            using var tvs = tempVariables.Push();

            foreach (var p in block.FlattenVariables)
                variables.Create(p, tvs);

            foreach(var (exp, last) in block.FlattenExpressions)
            {
                if(!last)
                {
                    VisitSave(exp, false);
                    continue;
                }

                // last item...
                return VisitReturn(exp, label, localIndex);
            }

            throw new InvalidOperationException($"This code is not reachable");
        }
    }
}
