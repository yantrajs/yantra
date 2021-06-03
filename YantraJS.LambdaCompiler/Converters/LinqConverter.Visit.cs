using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Converters
{

    public partial class LinqConverter
    {
        private YMemberAssignment Visit(MemberBinding binding)
        {
            switch (binding)
            {
                case MemberAssignment ma:
                    return YExpression.Bind(ma.Member, Visit(ma.Expression));
            }
            throw new NotSupportedException();
        }

        private YExpression VisitOld(Expression exp)
        {
            if (exp == null)
                return null;
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                    var be = exp as BinaryExpression;
                    return YExpression.Binary( Visit(be.Left), YOperator.Add, Visit(be.Right) );
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.BitwiseAnd, Visit(be.Right));
                case ExpressionType.AndAlso:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.BooleanAnd, Visit(be.Right));
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.ArrayIndex:
                    var iexp = exp as BinaryExpression;
                    return YExpression.ArrayIndex(Visit(iexp.Left), Visit(iexp.Right));
                case ExpressionType.ArrayLength:
                    var ue = exp as UnaryExpression;
                    return YExpression.ArrayLength(Visit(ue.Operand));
                case ExpressionType.Assign:
                    be = exp as BinaryExpression;
                    if (be.Conversion != null)
                        throw new NotSupportedException();
                    return YExpression.Assign(Visit(be.Left), Visit(be.Right));
                case ExpressionType.Block:
                    return VisitBlock(exp as BlockExpression);
                case ExpressionType.Call:
                    return VisitCall(exp as MethodCallExpression);
                case ExpressionType.Coalesce:
                    return VisitCoalesce(exp as BinaryExpression);
                case ExpressionType.Conditional:
                    var ce = exp as ConditionalExpression;
                    return YExpression.Conditional(Visit(ce.Test), Visit(ce.IfTrue), Visit(ce.IfFalse));
                case ExpressionType.Constant:
                    var cnt = exp as ConstantExpression;
                    return YExpression.Constant( cnt.Value, cnt.Type );
                case ExpressionType.Convert:
                    ue = exp as UnaryExpression;
                    return YExpression.Convert(Visit(ue.Operand), ue.Type, true);
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Default:
                    var de = exp as DefaultExpression;
                    if(exp.Type == typeof(void))
                    {
                        return YExpression.Empty;
                    }
                    return YExpression.Null;
                case ExpressionType.Divide:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Divide, Visit(be.Right));
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Equal:
                    be = exp as BinaryExpression;
                    return YExpression.Equal(Visit(be.Left), Visit(be.Right));
                case ExpressionType.ExclusiveOr:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Xor, Visit(be.Right));
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    var ge = exp as GotoExpression;
                    switch (ge.Kind)
                    {
                        case GotoExpressionKind.Break:
                        case GotoExpressionKind.Continue:
                        case GotoExpressionKind.Goto:
                            return YExpression.GoTo(labels[ge.Target], Visit(ge.Value));
                        case GotoExpressionKind.Return:
                            return YExpression.Return(labels[ge.Target], Visit(ge.Value));
                    }
                    break;
                case ExpressionType.GreaterThan:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Greater, Visit(be.Right));
                case ExpressionType.GreaterThanOrEqual:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.GreaterOrEqual, Visit(be.Right));
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    var ie = exp as IndexExpression;
                    return YExpression.Index(Visit(ie.Object), ie.Indexer , VisitList(ie.Arguments));
                case ExpressionType.Invoke:
                    var invoke = exp as InvocationExpression;
                    return YExpression.Invoke(Visit(invoke.Expression), VisitList(invoke.Arguments));
                case ExpressionType.IsFalse:
                    
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.Label:
                    var le = exp as LabelExpression;
                    return YExpression.Label(labels[le.Target], Visit(le.DefaultValue));
                case ExpressionType.Lambda:
                    return VisitLambda(exp as LambdaExpression);
                case ExpressionType.LeftShift:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.LeftShift, Visit(be.Right));
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.LessThan:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Less, Visit(be.Right));
                case ExpressionType.LessThanOrEqual:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.LessOrEqual, Visit(be.Right));
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.Loop:
                    var loop = exp as LoopExpression;
                    return YExpression.Loop(Visit(loop.Body), labels[loop.BreakLabel], loop.ContinueLabel != null ? labels[loop.ContinueLabel] : null);
                case ExpressionType.MemberAccess:
                    var member = exp as MemberExpression;
                    if (member.Member is FieldInfo field)
                        return YExpression.Field(Visit(member.Expression), field);
                    if (member.Member is PropertyInfo property)
                        return YExpression.Property(Visit(member.Expression), property);
                    break;
                case ExpressionType.MemberInit:
                    var mie = exp as MemberInitExpression;

                    return YExpression.MemberInit(Visit(mie.NewExpression) as YNewExpression, 
                        mie.Bindings.Select(m => Visit(m)).ToArray());
                case ExpressionType.Modulo:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Mod, Visit(be.Right));
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.Multiply:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Multipley, Visit(be.Right));
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.MultiplyChecked:
                    
                    break;
                case ExpressionType.Negate:
                    ue = exp as UnaryExpression;
                    return YExpression.Negative(Visit(ue.Operand));
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    var newExp = exp as NewExpression;
                    return YExpression.New(newExp.Constructor, VisitList(newExp.Arguments));
                case ExpressionType.NewArrayBounds:
                    var nab = exp as NewArrayExpression;
                    return YExpression.NewArrayBounds(nab.Type.GetElementType(), Visit(nab.Expressions.First()));
                case ExpressionType.NewArrayInit:
                    nab = exp as NewArrayExpression;
                    return YExpression.NewArray(nab.Type.GetElementType(), VisitList(nab.Expressions));
                case ExpressionType.Not:
                    var notExp = exp as UnaryExpression;
                    return YExpression.Not(Visit(notExp.Operand));
                case ExpressionType.NotEqual:
                    be = exp as BinaryExpression;
                    return YExpression.NotEqual(Visit(be.Left), Visit(be.Right));
                case ExpressionType.OnesComplement:
                    ue = exp as UnaryExpression;
                    return YExpression.OnesComplement(Visit(ue.Operand));
                case ExpressionType.Or:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.BitwiseOr, Visit(be.Right));
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.OrElse:
                    be = exp as BinaryExpression;
                    return YExpression.OrElse(Visit(be.Left), Visit(be.Right));
                case ExpressionType.Parameter:
                    return parameters[exp as ParameterExpression];
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.Power:
                    return VisitPower(exp as BinaryExpression);
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.RightShift , Visit(be.Right));
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Subtract:
                    be = exp as BinaryExpression;
                    return YExpression.Binary(Visit(be.Left), YOperator.Subtract, Visit(be.Right));
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.Switch:
                    return VisitSwitch(exp as SwitchExpression);
                case ExpressionType.Throw:
                    ue = exp as UnaryExpression;
                    return YExpression.Throw(Visit(ue.Operand));
                case ExpressionType.Try:
                    var te = exp as TryExpression;
                    YCatchBody cb = null;
                    if (te.Handlers.Count > 0)
                    {
                        var first = te.Handlers.First();
                        cb = first.Variable != null 
                            ? YExpression.Catch(parameters[first.Variable], Visit(first.Body))
                            : YExpression.Catch(Visit(first.Body));
                    }
                    return YExpression.TryCatchFinally(Visit(te.Body), cb, Visit(te.Finally));
                case ExpressionType.TypeAs:
                    ue = exp as UnaryExpression;
                    return YExpression.TypeAs(Visit(ue.Operand), ue.Type);
                case ExpressionType.TypeEqual:
                    var tbe = exp as TypeBinaryExpression;
                    return YExpression.TypeIs(Visit(tbe.Expression), tbe.TypeOperand);
                case ExpressionType.TypeIs:
                    tbe = exp as TypeBinaryExpression;
                    return YExpression.TypeIs(Visit(tbe.Expression), tbe.TypeOperand);
                case ExpressionType.UnaryPlus:
                    // ue = exp as UnaryExpression;
                    // i have no ideay why this exists !!
                    return Visit(exp);
                case ExpressionType.Unbox:
                    
                    break;
            }
            throw new NotSupportedException($"Expression of type {exp.NodeType} is not yet supported");
        }
    }
}
