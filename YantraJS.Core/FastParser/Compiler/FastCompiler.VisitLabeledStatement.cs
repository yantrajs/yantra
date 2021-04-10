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
        protected override Expression VisitLabeledStatement(AstLabeledStatement labeledStatement)
        {
            switch (labeledStatement.Body.Type)
            {
                case FastNodeType.ForStatement:
                    return VisitForStatement(labeledStatement.Body as AstForStatement, labeledStatement.Label.Span.Value);
                case FastNodeType.ForOfStatement:
                    return VisitForOfStatement(labeledStatement.Body as AstForOfStatement, labeledStatement.Label.Span.Value);
                case FastNodeType.ForInStatement:
                    return VisitForInStatement(labeledStatement.Body as AstForInStatement, labeledStatement.Label.Span.Value);
                case FastNodeType.WhileStatement:
                    return VisitWhileStatement(labeledStatement.Body as AstWhileStatement, labeledStatement.Label.Span.Value);
                case FastNodeType.DoWhileStatement:
                    return VisitDoWhileStatement(labeledStatement.Body as AstDoWhileStatement, labeledStatement.Label.Span.Value);
                default:
                    throw JSContext.Current.NewSyntaxError($"Label can only be used for loops");
            }
            throw new NotImplementedException();
        }
    }
}
