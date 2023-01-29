using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PreventStackoverFlow(ref FastToken id) {
//            if (id != null) {
                if (id == stream.Current)
                    throw stream.Unexpected();
//            }
            id = stream.Current;
        }

        /// <summary>
        /// While parsing expression, it can never start from same
        /// position of token, any nested Expression must consume
        /// the current token.
        /// </summary>
        private FastToken lastExpressionIndex;

        bool Expression(out AstExpression node)
        {
            SkipNewLines();

            PreventStackoverFlow(ref lastExpressionIndex);

            var token = stream.Current;

            if (token.Type == TokenTypes.EOF)
                throw stream.Unexpected();

            if (!SinglePrefixPostfixExpression(out node, out var isAsync, out var isGenerator))
            {
                node = null;
                return false;
                // lets check if we have expression sequence
                //if(!ArrayExpression(out var nodes))
                //    return begin.Reset();
                //if(stream.CheckAndConsume(TokenTypes.Lambda))
                //{
                //    var scope = this.variableScope.Push(token, FastNodeType.FunctionExpression);
                //    try {
                //        var parameters = VariableDeclarator.From(Pool, in nodes);
                //        // array function...
                //        if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart)) {
                //            if (!Block(out var block))
                //                throw stream.Unexpected();
                //            node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                //                parameters, block);
                //            return true;
                //        }
                //        if (!Expression(out var r))
                //            throw stream.Unexpected();
                //        node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                //            parameters, new AstReturnStatement(r.Start, r.End, r));
                //        return true;
                //    } finally {
                //        scope.Dispose();
                //    }
                //}

                //if (nodes.Length == 0)
                //    return false;

                //node = new AstSequenceExpression(nodes);
            }

            if(stream.CheckAndConsume(TokenTypes.Lambda))
            {
                var scope = this.variableScope.Push(token, FastNodeType.FunctionExpression);
                // create parameters now...
                var parameters = VariableDeclarator.From(node);
                if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart)) {
                    if (!Block(out var block))
                        throw stream.Unexpected();
                    node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                        VariableDeclarator.From(node), block);
                    scope.Dispose();
                    return true;
                }
                if (!Expression(out var r))
                    throw stream.Unexpected();
                node = new AstFunctionExpression(token, PreviousToken, true, isAsync, isGenerator, null,
                    parameters, new AstReturnStatement(r.Start, r.End, r));
                scope.Dispose();
                return true;
            }

            if (node.End.Type == TokenTypes.SemiColon)
                return true;

            if (stream.Previous.Type == TokenTypes.SemiColon)
            {
                return true;
            }

            var m = stream.SkipNewLines();

            var current = stream.Current;
            var currentType = current.Type;

            switch (currentType)
            {
                case TokenTypes.Colon:
                case TokenTypes.CurlyBracketEnd:
                case TokenTypes.BracketEnd:
                case TokenTypes.TemplatePart:
                case TokenTypes.TemplateEnd:
                    return true;
            }

            if (!currentType.IsOperator())
            {
                if (m.LinesSkipped)
                {
                    m.Undo();
                    return true;
                }

                if (currentType == token.Type)
                    throw stream.Unexpected();
            }

            if(NextExpression(ref node, ref currentType, out var next, out var nextToken))
            {
                if(next == null)
                {
                    return true;
                }
                node = Combine(node, currentType, next);
                return true;
            }

            return true;
        }

        
    }

}
