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
using System.Reflection;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {


        private Exp GetName(AstClassProperty property)
        {
            var exp = property.Key;
            var computed = property.Computed;
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

            var memberInits = new Sequence<AstClassProperty>();
            AstFunctionExpression constructor = null;

            var en = body.Members.GetFastEnumerator();
            while(en.MoveNext(out var property))
            {
                Exp name;
                // var el = property.IsStatic ? staticElements : prototypeElements;
                switch (property.Kind)
                {
                    case AstPropertyKind.Data:
                        if (property.IsStatic)
                        {
                            name = GetName(property);
                            var value = property.Init == null ? JSUndefinedBuilder.Value : Visit(property.Init);
                            staticElements.Add(JSObjectBuilder.AddValue(name, value, JSPropertyAttributes.ConfigurableValue));
                            break;
                        }
                        memberInits.Add(property);
                        break;
                    case AstPropertyKind.Get:
                        name = GetName(property);
                        if (property.IsStatic)
                        {
                            var fx = CreateFunction(property.Init as AstFunctionExpression, superVar);
                            staticElements.Add(JSObjectBuilder.AddGetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                            break;
                        }
                        else
                        {
                            var fx = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                            prototypeElements.Add(JSObjectBuilder.AddGetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        }
                        break;
                    case AstPropertyKind.Set:
                        name = GetName(property);
                        if (property.IsStatic)
                        {
                            var fx = CreateFunction(property.Init as AstFunctionExpression, superVar);
                            staticElements.Add(JSObjectBuilder.AddSetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        } else
                        {
                            var fx = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
                            prototypeElements.Add(JSObjectBuilder.AddSetter(name, fx, JSPropertyAttributes.ConfigurableProperty));
                        }
                        break;
                    case AstPropertyKind.Constructor:
                        constructor = property.Init as AstFunctionExpression;
                        break;
                    case AstPropertyKind.Method:
                        name = GetName(property);
                        if (property.IsStatic)
                        {
                            var fx = CreateFunction(property.Init as AstFunctionExpression, superVar);
                            staticElements.Add(JSObjectBuilder.AddValue(name, fx, JSPropertyAttributes.ConfigurableValue));
                        }
                        else
                        {
                            var fx = CreateFunction(property.Init as AstFunctionExpression, superPrototypeVar);
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

            var className = id?.Name.Value ?? "Unnamed";
            if (constructor != null)
            {
                var fx = CreateFunction(constructor, superVar, true, className, memberInits);
                staticElements.Add(JSClassBuilder.AddConstructor(fx));
            }
            else
            {
                if (memberInits.Any())
                {
                    using var s = this.scope.Push(new FastFunctionScope(null, null, memberInits: memberInits));
                    var args = s.Arguments;
                    var @this = s.ThisExpression;
                    var inits = new Sequence<Exp>() {
                    };
                    inits.AddRange(s.InitList);
                    inits.Add(Exp.Assign(@this, JSFunctionBuilder.InvokeFunction(superVar, args)));
                    InitMembers(inits, s);
                    inits.Add(@this);
                    var lambda = Exp.Lambda<JSFunctionDelegate>(className,
                        Exp.Block(s.VariableParameters.AsSequence(), inits),
                        args);
                    var fx = JSFunctionBuilder.New(
                        lambda,
                        StringSpanBuilder.New(className),
                        StringSpanBuilder.Empty,
                        1
                        );
                    staticElements.Add(JSClassBuilder.AddConstructor(fx));
                }
            }

            var _new = JSClassBuilder.New(null, superVar, className);

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
