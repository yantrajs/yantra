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
using System.Linq;
using YantraJS.Expressions;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        private Exp CreateClass(AstIdentifier id, AstExpression super, AstClassExpression body)
        {

            var scope = pool.NewScope();
            var tempVar = this.scope.Top.GetTempVariable(typeof(JSClass));

            var prototypeElements = new Sequence<YElementInit>();
            var staticElements = new Sequence<YBinding>();

            Dictionary<string, string> added = new Dictionary<string, string>();


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

            var superVar = Exp.Parameter(typeof(JSFunction));
            var superPrototypeVar = Exp.Parameter(typeof(JSObject));

            var stmts = new Sequence<Exp>(body.Members.Count);


            stmts.Add(Exp.Assign(superVar, Exp.TypeAs(superExp, typeof(JSFunction))));
            stmts.Add(Exp.Assign(superPrototypeVar, JSFunctionBuilder.Prototype(superVar)));

            Exp retValue = tempVar.Variable;

            Exp GetName(AstExpression exp, bool computed)
            {
                switch ((exp.Type, exp))
                {
                    case (FastNodeType.Identifier, AstIdentifier id):
                        if (computed)
                            return VisitIdentifier(id);
                        return KeyOfName(id.Name);
                    case (FastNodeType.Literal, AstLiteral l):
                        return KeyOfName(l.StringValue);
                    default:
                        return Visit(exp);
                }
            }

            //var cnstr = body.Members.FirstOrDefault(x => x.Kind == AstPropertyKind.Constructor);
            //if (cnstr != null)
            //{
            //    stmts.Add(
            //        Expression.Assign(
            //            retValue,
            //            CreateFunction(cnstr.Init as AstFunctionExpression, superVar, true, id?.Name.Value)));
            //}
            //else
            //{

            //    stmts.Add(
            //        Expression.Assign(
            //            retValue,
            //            JSClassBuilder.New(this.scope.Top.ScriptInfo, null, constructor, superVar, id?.Name.Value ?? "Unnamed")));
            //}
                

            foreach (var property in body.Members)
            {
                var name = GetName(property.Key, property.Computed);
                // var el = property.IsStatic ? staticElements : prototypeElements;
                switch (property.Kind)
                {
                    case AstPropertyKind.Get:
                        //if (!cache.TryGetValue(name, out expHolder))
                        //{
                        //    expHolder = new ExpressionHolder()
                        //    {
                        //        Key = nameExp
                        //    };
                        //    cache[name] = expHolder;
                        //    members.Add(expHolder);
                        //    expHolder.Static = property.IsStatic;
                        //}
                        //expHolder.Getter = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                        var fx = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                        if (property.IsStatic)
                        {
                            staticElements.Add(JSObjectBuilder.AddGetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        }
                        else
                        {
                            prototypeElements.Add(JSObjectBuilder.AddGetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        }
                        break;
                    case AstPropertyKind.Set:
                        //if (!cache.TryGetValue(name, out expHolder))
                        //{
                        //    expHolder = new ExpressionHolder()
                        //    {
                        //        Key = nameExp
                        //    };
                        //    cache[name] = expHolder;
                        //    members.Add(expHolder);
                        //    expHolder.Static = property.IsStatic;
                        //}
                        //expHolder.Setter = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                        fx = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                        if (property.IsStatic)
                        {
                            staticElements.Add(JSObjectBuilder.AddSetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        } else
                        {
                            prototypeElements.Add(JSObjectBuilder.AddSetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        }
                        break;
                    case AstPropertyKind.Constructor:
                        fx = CreateFunction(property.Init as AstFunctionExpression, superVar, true);
                        staticElements.Add(JSClassBuilder.AddConstructor(fx));
                        break;
                    case AstPropertyKind.Method:
                        //members.Add(new ExpressionHolder()
                        //{
                        //    Key = nameExp,
                        //    Value = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar),
                        //    Static = property.IsStatic
                        //});
                        fx = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                        if (property.IsStatic)
                        {
                            staticElements.Add(JSObjectBuilder.AddValue(name, fx, JSPropertyAttributes.ConfigurableValue));
                        }
                        else
                        {
                            prototypeElements.Add(JSObjectBuilder.AddValue(name, fx, JSPropertyAttributes.ConfigurableValue));
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            //foreach (var exp in members)
            //{
            //    if (exp.Value != null)
            //    {
            //        retValue = exp.Static
            //            ? JSClassBuilder.AddStaticValue(retValue, exp.Key, exp.Value)
            //            : JSClassBuilder.AddValue(retValue, exp.Key, exp.Value);
            //        continue;
            //    }
            //    retValue = exp.Static
            //        ? JSClassBuilder.AddStaticProperty(retValue, exp.Key, exp.Getter, exp.Setter)
            //        : JSClassBuilder.AddProperty(retValue, exp.Key, exp.Getter, exp.Setter);
            //}
            // stmts.Add(retValue);

            var _new = JSClassBuilder.New(scriptInfo, null, null, superVar, id?.Name.Value ?? "Unnamed");

            if (prototypeElements.Any())
            {
                staticElements.Add(new YMemberElementInit(JSFunctionBuilder._prototype, prototypeElements));
            }

            YExpression retVal = staticElements.Any() 
                ? YExpression.MemberInit(_new, staticElements)
                : _new;

            stmts.Add(
                Expression.Assign(
                    retValue,
                    retVal
                        ));


            if (id?.Name != null)
            {
                var v = this.scope.Top.CreateVariable(id.Name);
                stmts.Add(Exp.Assign(v.Expression, retValue));
            }
            else
            {
                stmts.Add(retValue);
            }

            var result = Exp.Block(new Sequence<ParameterExpression> { superVar, superPrototypeVar }, stmts);
            scope.Dispose();
            return result;
        }
    }
}
