using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;

using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;
using YantraJS.Expressions;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitObjectLiteral(AstObjectLiteral objectExpression)
        {
            // var keys = new List<ExpressionHolder>(objectExpression.Properties.Count);
            // var properties = new Dictionary<string, ExpressionHolder>(objectExpression.Properties.Count);

            var elements = new Sequence<YElementInit>();

            var en = objectExpression.Properties.GetFastEnumerator();
            while(en.MoveNext(out var pn))
            {

                switch (pn.Type)
                {
                    case FastNodeType.SpreadElement:
                        var spread = pn as AstSpreadElement;
                        elements.Add(new YElementInit( JSObjectBuilder._FastAddRange, Visit(spread.Argument)));
                        continue;
                    case FastNodeType.ClassProperty:
                        break;
                    default:
                        throw new FastParseException(pn.Start, $"Invalid token {pn.Start} in object literal");
                }

                AstClassProperty p = pn as AstClassProperty;

                Exp key = null;
                Exp value = null;
                string name = null;
                var pKey = p.Key;

                value = VisitExpression(p.Init);


                if (p.Computed)
                {
                // there is a possibility of numeric index
                var keyExp = pKey.IsUIntLiteral(out var num) ? Exp.Constant(num) : Visit(pKey);

                    if (p.Kind == AstPropertyKind.Get)
                    {
                        elements.Add(JSObjectBuilder.AddGetter(keyExp, value));
                        continue;
                    }
                    if (p.Kind == AstPropertyKind.Set)
                    {
                        elements.Add(JSObjectBuilder.AddSetter(keyExp, value));
                        continue;
                    }
                    elements.Add(JSObjectBuilder.AddValue(keyExp, value));

                    //keys.Add(new ExpressionHolder
                    //{
                    //    Key = VisitExpression(p.Key),
                    //    Value = value
                    //});

                    continue;
                }

                switch (pKey.Type)
                {
                    case FastNodeType.Identifier:
                        var id = pKey as AstIdentifier;
                        if (!p.Computed)
                        {
                            key = KeyOfName(id.Name);
                            name = id.Name.Value;
                        }
                        else
                        {
                            key = this.scope.Top.GetVariable(id.Name).Expression;
                            name = id.Name.Value;
                        }
                        break;
                    case FastNodeType.Literal:
                        var l = pKey as AstLiteral;
                        if (l.TokenType == TokenTypes.String)
                        {
                            if (NumberParser.TryCoerceToUInt32(l.StringValue, out var ui))
                            {
                                key = Exp.Constant(ui);

                            }
                            else
                            {
                                key = KeyOfName(l.StringValue);
                                name = l.StringValue;
                            }
                        }
                        else if (l.TokenType == TokenTypes.Number)
                        {
                            key = Exp.Constant((uint)l.NumericValue);
                        }
                        else
                            throw new NotSupportedException();
                        break;
                    default:
                        throw new NotSupportedException();
                }

                switch (p.Kind)
                {
                    case AstPropertyKind.Get:
                        elements.Add(JSObjectBuilder.AddGetter(key, value));
                        break;
                    case AstPropertyKind.Set:
                        elements.Add(JSObjectBuilder.AddSetter(key, value));
                        break;
                    default:
                        elements.Add(JSObjectBuilder.AddValue(key, value));
                        break;
                }
            }

            if (elements.Any()) {
                var r = ExpHelper.JSObjectBuilder.New(elements);
                return r;
            }
            return JSObjectBuilder.New();
        }
    }
}
