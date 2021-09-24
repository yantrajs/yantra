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
            switch (node.BooleanMember)
            {
                case FieldInfo field:
                    il.Emit(OpCodes.Ldfld, field);
                    break;
                case PropertyInfo property:
                    il.EmitCall(property.GetMethod);
                    break;
                case MethodInfo method:
                    il.EmitCall(method);
                    break;
                case null:
                    // check if it is null... 
                    break;
            }
            // check if it is false ...

            // goto end...

            var falseEnd = il.DefineLabel("falseEnd", il.Top);
            il.Emit(OpCodes.Brfalse, falseEnd);
            var a = EmitParameters(node.Method, node.Arguments, node.Method.ReturnType);
            il.EmitCall(node.Method);
            a();
            il.MarkLabel(falseEnd);
            return true;
        }
    }
}
