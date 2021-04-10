using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitIdentifier(AstIdentifier identifier)
        {
            if (identifier.Equals("undefined"))
                return JSUndefinedBuilder.Value;
            if (identifier.Equals("this"))
                return this.scope.Top.ThisExpression;
            if (identifier.Equals("arguments"))
            {
                var functionScope = this.scope.Top.TopScope;
                var vs = functionScope.CreateVariable("arguments",
                    JSArgumentsBuilder.New(functionScope.ArgumentsExpression));
                return vs.Expression;

            }
            var var = this.scope.Top.GetVariable(identifier.Name, true);
            if (var != null)
                return var.Expression;

            return ExpHelper.JSContextBuilder.Index(KeyOfName(identifier.Name));
        }
    }
}
