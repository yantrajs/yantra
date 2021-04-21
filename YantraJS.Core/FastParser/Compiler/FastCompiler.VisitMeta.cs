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
        protected override Exp VisitMeta(AstMeta astMeta)
        {
            // only new.target is supported....
            if (!(astMeta.Identifier.Name.Equals("new") 
                &&  astMeta.Property.Name.Equals("target")))
                throw JSContext.Current.NewSyntaxError($"{astMeta.Identifier.Name}.{astMeta.Property} not supported");

            return ArgumentsBuilder.NewTarget(scope.Top.Arguments);
        }
    }
}
