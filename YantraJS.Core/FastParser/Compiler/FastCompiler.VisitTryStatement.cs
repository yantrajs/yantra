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

        protected override Expression VisitTryStatement(AstTryStatement tryStatement)
        {
            var block = VisitStatement(tryStatement.Block);
            var cb = tryStatement.Catch;
            if (cb != null)
            {
                var id = tryStatement.Identifier;
                var pe = scope.Top.CreateException(id.Name.Value);
                var v = scope.Top.CreateVariable(id.Name);

                var catchBlock = Exp.Block(new ParameterExpression[] { v.Variable },
                    Exp.Assign(v.Variable, ExpHelper.JSVariableBuilder.NewFromException(pe.Variable, id.Name.Value)),
                    VisitStatement(cb));
                var cbExp = Exp.Catch(pe.Variable, catchBlock.ToJSValue());


                if (tryStatement.Finally != null)
                {
                    return Exp.TryCatchFinally(block.ToJSValue(), VisitStatement(tryStatement.Finally).ToJSValue(), cbExp);
                }

                return Exp.TryCatch(block.ToJSValue(), cbExp);
            }

            var @finally = tryStatement.Finally;
            if (@finally != null)
            {
                return Exp.TryFinally(block.ToJSValue(), VisitStatement(@finally).ToJSValue());
            }

            return JSUndefinedBuilder.Value;
        }
    }
}
