using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        /// <summary>
        /// While parsing expression, it can never start from same
        /// position of token, any nested Expression must consume
        /// the current token.
        /// </summary>
        private int lastExpressionIndex = 0;

        bool Expression(out AstExpression node)
        {
            if(lastExpressionIndex > 0)
            {
                if (lastExpressionIndex == stream.Position)
                    throw stream.Unexpected();
            }
            lastExpressionIndex = stream.Position;

            var begin = Location;
            var token = begin.Token;

            if (token.Type == TokenTypes.EOF)
                throw stream.Unexpected();

            if (!SinglePrefixPostfixExpression(out node, out var isAsync, out var isGenerator))
            {
                // lets check if we have expression sequence
                if(!ExpressionArray(out var nodes))
                    return begin.Reset();
                if(stream.CheckAndConsume(TokenTypes.Lambda))
                {
                    var scope = this.variableScope.Push(token, FastNodeType.FunctionExpression);
                    try {
                        var parameters = VariableDeclarator.From(Pool, in nodes);
                        // array function...
                        if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart)) {
                            if (!Block(out var block))
                                throw stream.Unexpected();
                            node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                                parameters, block);
                            return true;
                        }
                        if (!Expression(out var r))
                            throw stream.Unexpected();
                        node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                            parameters, new AstReturnStatement(r.Start, r.End, r));
                        return true;
                    } finally {
                        scope.Dispose();
                    }
                }
            }

            if(stream.CheckAndConsume(TokenTypes.Lambda))
            {
                var scope = this.variableScope.Push(token, FastNodeType.FunctionExpression);
                try {
                    // create parameters now...
                    var parameters = VariableDeclarator.From(Pool, node);
                    if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart)) {
                        if (!Block(out var block))
                            throw stream.Unexpected();
                        node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                            VariableDeclarator.From(Pool, node), block);
                        return true;
                    }
                    if (!Expression(out var r))
                        throw stream.Unexpected();
                    node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                        parameters, new AstReturnStatement(r.Start, r.End, r));
                    return true;
                } finally {
                    scope.Dispose();
                }

            }

            begin = Location;

            var current = stream.Current;
            var currentType = current.Type;
            if(NextExpression(ref node, ref currentType, out var next, out var nextToken))
            {
                if(next == null)
                {
                    return true;
                }
                node = node.Combine(currentType, next);
                return true;
            }

            return true;
        }

        
    }

}
