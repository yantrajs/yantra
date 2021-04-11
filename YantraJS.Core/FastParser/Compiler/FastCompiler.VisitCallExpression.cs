#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Core.LinqExpressions;
using YantraJS.ExpHelper;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {
        protected override Exp VisitCallExpression(AstCallExpression callExpression)
        {
            var ce = VisitCallExpression(callExpression, callExpression);
            return ce;
        }

        protected Expression VisitArguments(
            Expression? thisArg,
            in ArraySpan<AstExpression> arguments) {

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
                    var sa = ae as AstSpreadElement;
                    args.Add(new ClrSpreadExpression(Visit(sa.Argument)));
                    hasSpread = true;
                }
                if(args.Any()) {
                    thisArg ??= JSUndefinedBuilder.Value;
                }
                return thisArg != null || args.Any()
                    ? ArgumentsBuilder.New(thisArg, args, hasSpread)
                    : ArgumentsBuilder.Empty();
            } finally {
                args.Clear();
            }
        }

        protected Exp VisitCallExpression(AstCallExpression callExpression, AstCallExpression node)
        {
            var calle = callExpression.Callee;

            if (calle is AstMemberExpression me)
            {
                // invoke method...


                Exp name;

                switch (me.Property.Type)
                {
                    case FastNodeType.Identifier:
                        var id = me.Property as AstIdentifier;
                        name = me.Computed ? VisitExpression(id) : KeyOfName(id.Name);
                        // name = KeyOfName(id.Name);
                        break;
                    case FastNodeType.Literal:
                        var l = me.Property as AstLiteral;
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
                bool isSuper = me.Object is AstSuper;
                var super = isSuper ? this.scope.Top.Super : null;
                var target = isSuper
                    ? this.scope.Top.ThisExpression
                    : VisitExpression(me.Object);

                var paramArray = VisitArguments(
                        isSuper ? target : null, 
                        in callExpression.Arguments);

                if (isSuper)
                {
                    var superMethod = JSValueBuilder.Index(super, name);
                    return JSFunctionBuilder.InvokeFunction(superMethod, paramArray);
                }

                return JSValueExtensionsBuilder.InvokeMethod(target, name, paramArray);

            }
            else
            {

                bool isSuper = callExpression.Callee is AstSuper;

                if (isSuper)
                {
                    var paramArray1 = VisitArguments(this.scope.Top.ThisExpression, in callExpression.Arguments);
                    var super = this.scope.Top.Super;
                    return JSFunctionBuilder.InvokeSuperConstructor(this.scope.Top.ThisExpression, super, paramArray1);
                }

                var paramArray = VisitArguments(JSUndefinedBuilder.Value, in callExpression.Arguments);
                var callee = VisitExpression(callExpression.Callee);
                return JSFunctionBuilder.InvokeFunction(callee, paramArray);
            }
        }
    }
}
