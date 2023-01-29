using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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

namespace YantraJS.Core.FastParser.Compiler
{
    partial class FastCompiler
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ExtractName(Sequence<StringSpan> list, AstNode node) {
            switch (node.Type)
            {
                case FastNodeType.VariableDeclaration:
                    var vd = node as AstVariableDeclaration;
                    var ve = vd.Declarators.GetFastEnumerator();
                    while (ve.MoveNext(out var d))
                        ExtractName(list, d.Identifier);
                    return;
                case FastNodeType.Identifier:
                    var id = node as AstIdentifier;
                    list.Add(id.Start.Span);
                    return;
                case FastNodeType.ArrayPattern:
                    var ap = node as AstArrayPattern;
                    var ae = ap.Elements.GetFastEnumerator();
                    while (ae.MoveNext(out var aitem))
                        ExtractName(list, aitem);
                    return;
                case FastNodeType.ObjectPattern:
                    var op = node as AstObjectPattern;
                    var oe = op.Properties.GetFastEnumerator();
                    while(oe.MoveNext(out var oitem))
                    {
                        ExtractName(list, oitem.Value);
                    }
                    return;
            }
        }


        IFastEnumerable<StringSpan> Names(AstNode expression)
        {

            var list = new Sequence<StringSpan>();
            ExtractName(list, expression);
            // var result = list.ToSpan();
            // list.Clear();
            return list;
        }

        protected override Exp VisitExportStatement(AstExportStatement exportStatement)
        {
            var exports = this.scope.Top.GetVariable("exports");
            var top = this.scope.Top;
            var declaration = exportStatement.Declaration;
            Exp left;
            if (exportStatement.IsDefault)
            {
                var defExports = JSValueBuilder.Index(exports.Expression, KeyOfName("default"));
                return Exp.Assign(defExports, Visit(declaration));
            }

            var list = new Sequence<Exp>();

            try
            {
                
                switch (exportStatement.Declaration.Type)
                {
                    case FastNodeType.VariableDeclaration:
                        var vd = this.Visit(declaration);
                        var names = Names(declaration);
                        var en = names.GetFastEnumerator();
                        list.Add(vd);
                        while(en.MoveNext(out var name))
                        {
                            left = JSValueBuilder.Index(exports.Expression, KeyOfName(name));
                            var right = top.GetVariable(name);
                            list.Add(Exp.Assign(left, right.Expression));
                        }
                        return Exp.Block(list);
                    case FastNodeType.Identifier:
                        var id = exportStatement.Declaration as AstIdentifier;
                        return JSValueBuilder.Index(exports.Expression, KeyOfName(id.Name));
                    case FastNodeType.FunctionExpression:
                        var fe = this.Visit(declaration);
                        var fd = declaration as AstFunctionExpression;
                        if(fd.Id != null)
                        {
                            left = JSValueBuilder.Index(exports.Expression, KeyOfName(fd.Id.Name));
                            return Exp.Assign(left, fe);
                        }
                        break;
                    case FastNodeType.ClassStatement:
                        var ce = this.Visit(declaration);
                        var cd = declaration as AstFunctionExpression;
                        if (cd.Id != null)
                        {
                            left = JSValueBuilder.Index(exports.Expression, KeyOfName(cd.Id.Name));
                            return Exp.Assign(left, ce);
                        }
                        break;
                }


                throw new FastParseException(exportStatement.Start, $"Unexpected export type {exportStatement.Declaration.Type}");
            }finally
            {
                // list.Clear();
            }
        }
    }
}
