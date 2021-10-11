using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public class YCoalesceCallExpression : YExpression
    {
        public readonly YExpression Target;
        public readonly MemberInfo BooleanMember;
        public readonly MethodInfo Method;
        public readonly IFastEnumerable<YExpression> Arguments;

        public YCoalesceCallExpression(
            YExpression target, 
            MemberInfo booleanMember,
            MethodInfo method,
            IFastEnumerable<YExpression> arguments) : base(YExpressionType.CoalesceCall, method.ReturnType)
        {
            this.Target = target;
            this.BooleanMember = booleanMember;
            this.Method = method;
            this.Arguments = arguments;
        }

        public override void Print(IndentedTextWriter writer)
        {
            
        }
    }
}
