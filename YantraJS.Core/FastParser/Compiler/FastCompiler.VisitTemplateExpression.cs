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
        protected override Expression VisitTemplateExpression(AstTemplateExpression templateExpression)
        {
            var items = pool.AllocateList<Exp>();
            try {
                var e = templateExpression.Parts.GetEnumerator();
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
                items.Clear();
            }
        }
    }
}
