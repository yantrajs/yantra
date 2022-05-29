using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        bool Function(out AstStatement statement, bool isAsync = false)
        {
            if (!FunctionExpression(out var expression, isAsync, isStatement: true))
            {
                statement = default;
                return false;
            }
            statement = new AstExpressionStatement(expression);
            return true;
        }


        bool FunctionExpression(out AstExpression node, bool isAsync = false, bool isStatement = false)
        {
            bool isRootAsync = this.isAsync;
            var begin = stream.Current;
            node = default;
            stream.Consume();
            var generator = false;
            if (stream.CheckAndConsume(TokenTypes.Multiply))
            {
                generator = true;
            }

            if(Identitifer(out var id)) {
                this.variableScope.Top.AddVariable(id.Start, id.Name, isStatement ? FastVariableKind.Let : FastVariableKind.Var);
            }

            stream.Expect(TokenTypes.BracketStart);
            var scope = variableScope.Push(begin, FastNodeType.FunctionExpression);
            if (!Parameters(out var declarators, TokenTypes.BracketEnd, false, FastVariableKind.Let))
                throw stream.Unexpected();

            if (!stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                throw stream.Unexpected();
            try
            {
                if(!Block(out var body))
                    throw stream.Unexpected();

                node = new AstFunctionExpression(begin, PreviousToken, false, isAsync, generator, id, declarators, body);
            } finally
            {
                scope.Dispose();
            }

            this.isAsync = isRootAsync;

            return true;
        }


    }
}
