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

        private Exp CreateClass(AstIdentifier id, AstExpression super, AstClassExpression body)
        {

            var scope = pool.NewScope();
            try
            {

                // need to save super..
                // create a super variable...
                Exp superExp;
                if (super != null)
                {
                    superExp = VisitExpression(super);
                }
                else
                {
                    superExp = JSContextBuilder.Object;
                }

                Exp constructor = null;
                Dictionary<string, ExpressionHolder> cache = new Dictionary<string, ExpressionHolder>(body.Members.Count);
                var members = scope.AllocateList<ExpressionHolder>(body.Members.Count);
                ExpressionHolder expHolder;

                var superVar = Exp.Parameter(typeof(JSFunction));
                var superPrototypeVar = Exp.Parameter(typeof(JSObject));

                var stmts = scope.AllocateList<Exp>();
                stmts.Add(Exp.Assign(superVar, Exp.TypeAs(superExp, typeof(JSFunction))));
                stmts.Add(Exp.Assign(superPrototypeVar, JSFunctionBuilder.Prototype(superVar)));

                Exp retValue = null;

                (string, Exp) GetName(AstExpression exp, bool computed)
                {
                    switch ((exp.Type, exp))
                    {
                        case (FastNodeType.Identifier, AstIdentifier id):
                            if (computed)
                                return (id.Name.Value, VisitIdentifier(id));
                            return (id.Name.Value, KeyOfName(id.Name));
                        case (FastNodeType.Literal, AstLiteral l):
                            return (l.StringValue, KeyOfName(l.StringValue));
                        default:
                            throw new NotImplementedException($"{exp.GetType().Name}");
                    }
                }

                foreach (var property in body.Members)
                {
                    var (name, nameExp) = GetName(property.Key, property.Computed);
                    switch (property.Kind)
                    {
                        case AstPropertyKind.Get:
                            if (!cache.TryGetValue(name, out expHolder))
                            {
                                expHolder = new ExpressionHolder()
                                {
                                    Key = nameExp
                                };
                                cache[name] = expHolder;
                                members.Add(expHolder);
                                expHolder.Static = property.IsStatic;
                            }
                            expHolder.Getter = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                            break;
                        case AstPropertyKind.Set:
                            if (!cache.TryGetValue(name, out expHolder))
                            {
                                expHolder = new ExpressionHolder()
                                {
                                    Key = nameExp
                                };
                                cache[name] = expHolder;
                                members.Add(expHolder);
                                expHolder.Static = property.IsStatic;
                            }
                            expHolder.Setter = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                            break;
                        case AstPropertyKind.Constructor:
                            retValue = CreateFunction(property.Init as AstFunctionExpression, superVar, true, id?.Name.Value);
                            break;
                        case AstPropertyKind.Method:
                            members.Add(new ExpressionHolder()
                            {
                                Key = nameExp,
                                Value = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar),
                                Static = property.IsStatic
                            });
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                retValue = retValue ?? JSClassBuilder.New(this.scope.Top.ScriptInfo, null, constructor, superVar, id?.Name.Value ?? "Unnamed");
                foreach (var exp in members)
                {
                    if (exp.Value != null)
                    {
                        retValue = exp.Static
                            ? JSClassBuilder.AddStaticValue(retValue, exp.Key, exp.Value)
                            : JSClassBuilder.AddValue(retValue, exp.Key, exp.Value);
                        continue;
                    }
                    retValue = exp.Static
                        ? JSClassBuilder.AddStaticProperty(retValue, exp.Key, exp.Getter, exp.Setter)
                        : JSClassBuilder.AddProperty(retValue, exp.Key, exp.Getter, exp.Setter);
                }
                // stmts.Add(retValue);

                if (id?.Name != null)
                {
                    var v = this.scope.Top.CreateVariable(id.Name);
                    stmts.Add(Exp.Assign(v.Expression, retValue));
                }
                else
                {
                    stmts.Add(retValue);
                }

                return Exp.Block(new ParameterExpression[] { superVar, superPrototypeVar }, stmts);
            } finally
            {
                scope.Dispose();
            }
        }
    }
}
