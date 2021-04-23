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
        protected override Exp VisitTaggedTemplateExpression(AstTaggedTemplateExpression astTaggedTemplateExpression)
        {
            var template = astTaggedTemplateExpression.Template;
            var parts = pool.AllocateList<Expression>(template.Parts.Length);
            var raw = pool.AllocateList<Expression>(template.Parts.Length);
            var expressions = pool.AllocateList<Expression>(template.Parts.Length);
            try
            {
                foreach(var p in template.Parts)
                {
                    if(p.Type == FastNodeType.Literal)
                    {
                        var l = p as AstLiteral;
                        if(l.TokenType == TokenTypes.TemplatePart)
                        {
                            raw.Add(Expression.Constant(l.Start.Span.Value));
                            parts.Add(Expression.Constant(l.StringValue));
                            continue;
                        }
                    }
                    expressions.Add(VisitExpression(p));
                }
                var a = JSTemplateArrayBuilder.New(parts, raw, expressions);
                return a;
            } finally {
                parts.Clear();
                raw.Clear();
                expressions.Clear();
            }
        }
    }
}
