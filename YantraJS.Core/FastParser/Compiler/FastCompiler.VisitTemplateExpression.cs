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
        protected override Expression VisitTemplateExpression(AstTemplateExpression templateExpression)
        {
            var items = new Sequence<Exp>(templateExpression.Parts.Count);
            try {
                var e = templateExpression.Parts.GetFastEnumerator();
                int size = 0;
                while(e.MoveNext(out  var item))
                {
                    if(item.Type == FastNodeType.Literal) {
                        var l = item as AstLiteral;
                        var txt = l.TokenType == TokenTypes.TemplatePart 
                            ? l.Start.CookedText
                            : l.StringValue;
                        size += txt.Length;
                        items.Add(Exp.Constant(txt));
                    } else {
                        items.Add(VisitExpression(item));
                    }
                }
                return JSTemplateStringBuilder.New(items, size);
            } finally {
                // items.Clear();
            }
        }
    }
}
