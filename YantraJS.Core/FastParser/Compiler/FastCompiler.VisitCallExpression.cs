#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var ce = VisitCallExpression(callExpression.Callee, in callExpression.Arguments, callExpression.Coalesce);
            return ce;
        }

        protected (ArraySpan<Expression> args, bool hasSpread) VisitArguments(in ArraySpan<AstExpression> arguments)
        {

            var args = pool.AllocateList<Exp>(arguments.Count);
            bool hasSpread = false;
            try
            {
                var e = arguments.GetEnumerator();
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

                return args.Any()
                    ? (args.ToSpan(), hasSpread)
                    : (ArraySpan<Expression>.Empty, false);
            }
            finally
            {
                args.Clear();
            }
        }

        protected Expression VisitArguments(
            Expression? thisArg,
            in ArraySpan<AstExpression> arguments,
            Expression? newTarget = null) {

            var args = pool.AllocateList<Exp>(arguments.Count);
            bool hasSpread = false;
            try {
                var e = arguments.GetEnumerator();
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
                    return ArgumentsBuilder.Spread(thisArg, args);
                }

                return ArgumentsBuilder.New(thisArg, args);
            } finally {
                args.Clear();
            }
        }

        protected Exp VisitCallExpression(
            AstExpression callee, 
            in ArraySpan<AstExpression> arguments
            , bool coalesce = false)
        {

            if (callee is AstMemberExpression me)
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
                        throw new NotImplementedException($"{me.Property}");
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
                                            in arguments);

                    var superMethod = JSValueBuilder.Index(super, name, me.Coalesce);
                    return JSFunctionBuilder.InvokeFunction(superMethod, paramArray, me.Coalesce);
                }
                var (args, spread) = VisitArguments(in arguments);
                using var te = this.scope.Top.GetTempVariable(typeof(JSValue));
                using var te2 = this.scope.Top.GetTempVariable(typeof(JSValue));
                return JSValueBuilder.InvokeMethod(te.Variable, te2.Variable, target, name, args, spread, me.Coalesce || coalesce);

            }
            else
            {

                bool isSuper = callee.Type == FastNodeType.Super;

                if (isSuper)
                {
                    var paramArray1 = VisitArguments(this.scope.Top.ThisExpression, in arguments);
                    return JSFunctionBuilder.InvokeSuperConstructor(
                        this.scope.Top.NewTarget, 
                        this.scope.Top.Super,
                        this.scope.Top.ThisExpression, paramArray1);
                }

                var paramArray = VisitArguments(JSUndefinedBuilder.Value, in arguments);
                var target = VisitExpression(callee);
                return JSFunctionBuilder.InvokeFunction(target, paramArray, coalesce);
            }
        }
    }
}
