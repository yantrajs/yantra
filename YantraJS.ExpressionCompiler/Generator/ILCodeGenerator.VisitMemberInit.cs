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
                switch (b.BindingType)
                {
                    case BindingType.MemberAssignment:
                        var ma = b as YMemberAssignment;
                        il.Emit(OpCodes.Dup);
                        Visit(ma.Value);
                        switch (b.Member)
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
                        break;
                    case BindingType.MemberListInit:
                        il.Emit(OpCodes.Dup);
                        switch (b.Member)
                        {
                            case FieldInfo field:
                                il.Emit(OpCodes.Ldfld, field);
                                break;
                            case PropertyInfo property:
                                il.Emit(OpCodes.Callvirt, property.GetMethod);
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        var la = b as YMemberElementInit;
                        foreach(var ei in la.Elements)
                        {
                            il.Emit(OpCodes.Dup);
                            foreach(var p in ei.Arguments)
                            {
                                Visit(p);
                            }
                            il.EmitCall(ei.AddMethod);
                        }
                        il.Emit(OpCodes.Pop);
                        break;
                    case BindingType.ElementInit:
                        il.Emit(OpCodes.Dup);
                        var el = b as YElementInit;
                        foreach(var item in el.Arguments)
                        {
                            Visit(item);
                        }
                        il.EmitCall(el.AddMethod);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return true;
        }
    }
}
