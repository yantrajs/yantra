#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        protected override Exp VisitCallExpression(AstCallExpression callExpression)
        {
            var ce = VisitCallExpression(callExpression.Callee, callExpression.Arguments, callExpression.Coalesce);
            return ce;
        }

        protected (IFastEnumerable<Expression> args, bool hasSpread) VisitArguments(IFastEnumerable<AstExpression> arguments)
        {

            var args = new Sequence<Exp>(arguments.Count);
            bool hasSpread = false;
            //try
            //{
                var e = arguments.GetFastEnumerator();
                while (e.MoveNext(out var ae))
                {
                    if (ae.Type != FastNodeType.SpreadElement)
                    {
                        args.Add(Visit(ae));
                        continue;
                    }
                    // spread....
                    var sae = (ae as AstSpreadElement)!.Argument;
                    args.Add(JSSpreadValueBuilder.New(Visit(sae)));
                    hasSpread = true;
                }

                var result = args.Any()
                    ? (args, hasSpread)
                    : (Sequence<Exp>.Empty, false);
                // args.Clear();
                return result;
            //}
            //finally
            //{
            //    args.Clear();
            //}
        }

        protected Expression VisitArguments(
            Expression? thisArg,
            IFastEnumerable<AstExpression> arguments,
            Expression? newTarget = null) {

            var args = new Sequence<Exp>(arguments.Count);
            bool hasSpread = false;
            // try {
                var e = arguments.GetFastEnumerator();
                while(e.MoveNext(out var ae)) {
                    if (ae.Type != FastNodeType.SpreadElement) {
                        args.Add(Visit(ae));
                        continue;
                    }
                    // spread....
                    var sae = (ae as AstSpreadElement)!.Argument;
                    args.Add(JSSpreadValueBuilder.New(Visit(sae)));
                    hasSpread = true;
                }

                if(!args.Any())
                {
                    if (thisArg == null)
                    {
                        return ArgumentsBuilder.Empty();
                    }
                    return ArgumentsBuilder.NewEmpty(thisArg);
                }
                thisArg ??= JSUndefinedBuilder.Value;
                if(hasSpread)
                {
                    var r = ArgumentsBuilder.Spread(thisArg, args);
                    // args.Clear();
                    return r;
                }

                var result = ArgumentsBuilder.New(thisArg, args);
                // args.Clear();
                return result;
            //} finally {
            //    args.Clear();
            //}
        }

        protected Exp VisitCallExpression(
            AstExpression callee,
            IFastEnumerable<AstExpression> arguments
            , bool coalesce = false)
        {

            if (callee.Type == FastNodeType.MemberExpression && callee is AstMemberExpression me)
            {
                // invoke method...


                Exp name;

                switch (me.Property.Type)
                {
                    case FastNodeType.Identifier:
                        var id = (me.Property as AstIdentifier)!;
                        name = me.Computed ? VisitExpression(id) : KeyOfName(id.Name);
                        // name = KeyOfName(id.Name);
                        break;
                    case FastNodeType.Literal:
                        var l = (me.Property as AstLiteral)!;
                        if (l.TokenType == TokenTypes.String)
                            name = KeyOfName(l.Start.CookedText);
                        else if (l.TokenType == TokenTypes.Number)
                            name = Exp.Constant((uint)l.NumericValue);
                        else
                            throw new NotImplementedException();
                        break;
                    case FastNodeType.MemberExpression:
                        name = VisitMemberExpression(me.Property as AstMemberExpression);
                        break;
                    default:
                        name = Visit(me.Property);
                        break;
                }

                // var id = me.Property.As<Esprima.Ast.Identifier>();
                bool isSuper = me.Object.Type == FastNodeType.Super;
                var super = isSuper ? this.scope.Top.Super : null;
                var target = isSuper
                    ? this.scope.Top.ThisExpression
                    : VisitExpression(me.Object);

                if (isSuper)
                {

                    var paramArray = VisitArguments(
                                            isSuper ? target : null,
                                            arguments);

                    var superMethod = JSValueBuilder.Index(super, name, me.Coalesce);
                    return JSFunctionBuilder.InvokeFunction(superMethod, paramArray, me.Coalesce);
                }
                var (args, spread) = VisitArguments(arguments);
                using var te = this.scope.Top.GetTempVariable(typeof(JSValue));
                using var te2 = this.scope.Top.GetTempVariable(typeof(JSValue));
                return JSValueBuilder.InvokeMethod(te.Variable, te2.Variable, target, name, args, spread, me.Coalesce || coalesce);

            }
            else
            {

                bool isSuper = callee.Type == FastNodeType.Super;

                var @this = this.scope.Top.ThisExpression;
                if (isSuper)
                {

                    // check if there are pending member inits...
                    var paramArray1 = VisitArguments(@this, arguments);
                    FastFunctionScope top = this.scope.Top;
                    // var newTarget = top.NewTarget;
                    var members = top.MemberInits;
                    var super = top.Super;
                    // we need to set this to null
                    // to inform function creator that we have
                    // initialized members.. and super has been called...
                    if (members?.Any() ?? false) {
                        var initList = new Sequence<Exp>() {
                            JSFunctionBuilder.InvokeSuperConstructor(
                            super,
                            @this, paramArray1)
                        };
                        InitMembers(initList, top);
                        top.MemberInits = null;
                        return Exp.Block(initList);
                    }
                    return JSFunctionBuilder.InvokeSuperConstructor(
                        super,
                        @this, paramArray1);
                }

                var paramArray = VisitArguments(null, arguments);
                var target = VisitExpression(callee);
                return JSFunctionBuilder.InvokeFunction(target, paramArray, coalesce);
            }
        }
    }
}
