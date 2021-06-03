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
        protected override CodeInfo VisitMemberInit(YMemberInitExpression memberInitExpression)
        {
            Visit(memberInitExpression.Target);
            foreach(var b in memberInitExpression.Bindings)
            {
                il.Emit(OpCodes.Dup);
                Visit(b.Value);
                switch(b.Member)
                {
                    case FieldInfo field:
                        il.Emit(OpCodes.Stfld, field);
                        break;
                    case PropertyInfo property:
                        il.Emit(OpCodes.Callvirt, property.SetMethod);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return true;
        }
    }
}
