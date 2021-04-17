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

        protected override Exp VisitImportStatement(AstImportStatement importStatement)
        {
            var tempRequire = Exp.Parameter(typeof(JSValue));
            var require = this.scope.Top.GetVariable("require");
            var source = VisitExpression(importStatement.Source);
            var args = ArgumentsBuilder.New(JSUndefinedBuilder.Value, source);
            var stmts = pool.AllocateList<Exp>();
            stmts.Add(Exp.Assign(tempRequire, JSFunctionBuilder.InvokeFunction(require.Expression, args) ));
            FastFunctionScope.VariableScope imported;

            var all = importStatement.Declaration;

            if (all != null)
            {
                switch (all.Type) {
                    case FastNodeType.Identifier:
                        var id = all as AstIdentifier;
                        imported = this.scope.Top.CreateVariable(id.Name);
                        stmts.Add(Exp.Assign(imported.Expression, tempRequire));
                        break;
                    case FastNodeType.VariableDeclaration:
                        var vd = all as AstVariableDeclaration;
                        stmts.Add(VisitVariableDeclaration(vd));
                        var d = vd.Declarators[0];
                        stmts.Add(CreateAssignment(d.Identifier, tempRequire));
                        break;
                    default:
                        throw new NotImplementedException();

                }
            }

            if(importStatement.Default != null)
            {
                imported = this.scope.Top.CreateVariable(importStatement.Default.Name);
                var prop = JSValueBuilder.Index(tempRequire, KeyOfName("default"));
                stmts.Add(Exp.Assign(imported.Expression, prop));

            }

            //foreach (var d in importDeclaration.Specifiers)
            //{
            //    switch (d.Type)
            //    {
            //        case Nodes.ImportDefaultSpecifier:
            //            ImportDefaultSpecifier ids = d as ImportDefaultSpecifier;
            //            imported = this.scope.Top.CreateVariable(ids.Local.Name);
            //            prop = JSValueBuilder.Index(tempRequire, KeyOfName("default"));
            //            stmts.Add(Exp.Assign(imported.Expression, prop));
            //            break;
            //        case Nodes.ImportNamespaceSpecifier:
            //            ImportNamespaceSpecifier ins = d as ImportNamespaceSpecifier;
            //            imported = this.scope.Top.CreateVariable(ins.Local.Name);
            //            stmts.Add(Exp.Assign(imported.Expression, tempRequire));
            //            break;
            //        case Nodes.ImportSpecifier:
            //            ImportSpecifier iss = d as ImportSpecifier;
            //            imported = this.scope.Top.CreateVariable(iss.Local.Name);
            //            prop = JSValueBuilder.Index(tempRequire, KeyOfName(iss.Imported.Name));
            //            stmts.Add(Exp.Assign(imported.Expression, prop));
            //            break;
            //    }
            //}
            return Exp.Block(
                new ParameterExpression[] { tempRequire },
                stmts);
        }

    }
}
