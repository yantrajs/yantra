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
            var be = memberInitExpression.Bindings.GetFastEnumerator();
            while(be.MoveNext(out var b))
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
                            var en = ei.Arguments.GetFastEnumerator();
                            while(en.MoveNext(out var p))
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
                        {
                            var en = el.Arguments.GetFastEnumerator();
                            while(en.MoveNext(out var item))
                            {
                                Visit(item);
                            }
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
