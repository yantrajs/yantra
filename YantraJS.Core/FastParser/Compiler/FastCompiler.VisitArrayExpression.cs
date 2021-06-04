using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitArrayExpression(AstArrayExpression arrayExpression)
        {
            var e = arrayExpression.Elements.GetEnumerator();
            var _new = JSArrayBuilder.New();
            while(e.MoveNext(out var item))
            {
                if(item == null)
                {
                    // list.Add(Expression.Null);
                    _new = JSArrayBuilder.Add(_new, Expression.Null);
                    continue;
                }
                if(item.Type == FastNodeType.SpreadElement)
                {
                    var i = (item as AstSpreadElement).Argument;
                    _new = JSArrayBuilder.AddRange(_new, Visit(i));
                    continue;
                }
                _new = JSArrayBuilder.Add(_new, Visit(item));
            }
            return _new;
        }
    }
}
