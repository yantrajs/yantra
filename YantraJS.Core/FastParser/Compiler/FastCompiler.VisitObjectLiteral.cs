using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using YantraJS.Utils;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Expression VisitObjectLiteral(AstObjectLiteral objectExpression)
        {
            var keys = new List<ExpressionHolder>(objectExpression.Properties.Count);
            var properties = new Dictionary<string, ExpressionHolder>(objectExpression.Properties.Count);
            foreach (AstNode pn in objectExpression.Properties)
            {
                if(pn.Type == FastNodeType.SpreadElement)
                {
                    throw new NotImplementedException();
                }

                AstClassProperty p = pn as AstClassProperty;

                Exp key = null;
                Exp value = null;
                string name = null;
                var pKey = p.Key;
                switch ((pKey.Type, pKey))
                {
                    case (FastNodeType.Identifier, AstIdentifier id):
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
                    case (FastNodeType.Literal, AstLiteral l):
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
                //if (p.Shorthand)
                //{
                //    value = this.scope.Top[name];
                //}
                //else
                //{
                    value = VisitExpression(p.Init);
                // }
                if (p.Kind == AstPropertyKind.Get || p.Kind == AstPropertyKind.Set)
                {
                    if (!properties.TryGetValue(name, out var m))
                    {
                        m = new ExpressionHolder
                        {
                            Key = key,
                            Getter = Exp.Constant(null, typeof(JSFunction)),
                            Setter = Exp.Constant(null, typeof(JSFunction))
                        };
                        properties[name] = m;
                        keys.Add(m);
                    }
                    if (p.Kind == AstPropertyKind.Get)
                    {
                        m.Getter = value;
                    }
                    else
                    {
                        m.Setter = value;
                    }
                    // m.Value = ExpHelper.JSPropertyBuilder.Property(key, m.Getter, m.Setter);
                    continue;
                }
                else
                {
                    // value = ExpHelper.JSPropertyBuilder.Value(key, value);
                    keys.Add(new ExpressionHolder
                    {
                        Key = key,
                        Value = value
                    });
                }
                // keys.Add(new ExpressionHolder { Value = value });
            }

            return ExpHelper.JSObjectBuilder.New(keys);
        }
    }
}
