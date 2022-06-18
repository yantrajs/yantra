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
        public readonly MemberInfo Test;
        public readonly IFastEnumerable<YExpression> TestArguments;
        public readonly MethodInfo True;
        public readonly IFastEnumerable<YExpression> TrueArguments;
        public readonly MethodInfo False;
        public readonly IFastEnumerable<YExpression> FalseArguments;

        public YCoalesceCallExpression(
            YExpression target, 
            MemberInfo test,
            IFastEnumerable<YExpression> testArguments,
            MethodInfo @true,
            IFastEnumerable<YExpression> trueArguments,
            MethodInfo @false,
            IFastEnumerable<YExpression> falseArguments
        ) : base(YExpressionType.CoalesceCall, @true?.ReturnType ?? @false.ReturnType)
        {
            this.Target = target;
            this.Test = test;
            this.TestArguments = testArguments;
            this.True = @true;
            this.TrueArguments = trueArguments;
            this.False = @false;
            this.FalseArguments = falseArguments;
        }

        public override void Print(IndentedTextWriter writer)
        {
            
        }
    }
}
