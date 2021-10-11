using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Expressions;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitArrayExpression(AstArrayExpression arrayExpression)
        {
            var e = arrayExpression.Elements.GetFastEnumerator();
            var list = new Sequence<YElementInit>();
            //try
            //{
                while (e.MoveNext(out var item))
                {
                    if (item == null)
                    {
                        list.Add(YExpression.ElementInit(JSArrayBuilder._Add, new Exp[] { Expression.Null }));
                        // _new = JSArrayBuilder.Add(_new, Expression.Null);
                        continue;
                    }
                    if (item.Type == FastNodeType.SpreadElement)
                    {
                        var i = (item as AstSpreadElement).Argument;
                        list.Add(YExpression.ElementInit(JSArrayBuilder._AddRange, new Exp[] { Visit(i) }));
                        //_new = JSArrayBuilder.AddRange(_new, Visit(i));
                        continue;
                    }
                    // _new = JSArrayBuilder.Add(_new, Visit(item));
                    list.Add(YExpression.ElementInit(JSArrayBuilder._Add, new Exp[] { Visit(item) }));
                }

                if (list.Count > 0)
                {
                    // var r = list.Release();
                    // list.Dispose();
                    return Expression.ListInit(Expression.New(JSArrayBuilder._New), list);
                }

                // list.Dispose();
                return Expression.New(JSArrayBuilder._New);
            //} finally
            //{
            //    list.Dispose();
            //}
        }
    }
}
