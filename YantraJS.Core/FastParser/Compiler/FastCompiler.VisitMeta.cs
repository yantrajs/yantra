using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

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

            return JSContextBuilder.NewTarget();
        }
    }
}
