using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitCoalesceCall(YCoalesceCallExpression node)
        {
            Visit(node.Target);
            il.Emit(OpCodes.Dup);
            // check if it is false ...
            foreach(var arg in node.TestArguments)
            {
                Visit(arg);
            }

            // goto end...
            if (node.Test is PropertyInfo property)
            {
                il.EmitCall(property.GetMethod);
            }
            else
            {
                il.EmitCall(node.Test as MethodInfo);
            }

            var end = il.DefineLabel("end", il.Top);
            var falseStart = il.DefineLabel("falseStart", il.Top);
            il.Emit(OpCodes.Brfalse, falseStart);
            if (node.True != null)
            {
                foreach (var arg in node.TrueArguments)
                {
                    Visit(arg);
                }
                il.EmitCall(node.True);
            }
            il.Emit(OpCodes.Br, end);
            il.MarkLabel(falseStart);
            if (node.False != null)
            {
                foreach (var arg in node.FalseArguments)
                    Visit(arg);
                il.EmitCall(node.False);
            }
            il.MarkLabel(end);
            return true;
        }
    }
}
