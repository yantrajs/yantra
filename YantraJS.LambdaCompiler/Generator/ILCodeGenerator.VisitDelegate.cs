using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public partial class ILCodeGenerator
    {
        protected override CodeInfo VisitDelegate(YDelegateExpression delegateExpression)
        {
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ldftn, delegateExpression.Method);
            var cl = delegateExpression.Type.GetConstructors();
            var c = cl
                .FirstOrDefault(ct => ct.GetParameters().Length == 2);
            il.EmitNew(c);
            if(delegateExpression.Type != typeof(void))
                return true;
            return true;
        }
    }
}
