﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        /// <summary>
        /// Single expression is,
        ///     Identifier
        ///     ( Expression )
        ///     Literal
        ///     Array
        ///     Object
        ///     Function
        ///     Class
        ///     `fdfsd${singleExpression}dfsd`
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool SingleExpression(out AstExpression node)
        {
            if (Literal(out node))
                return true;
            var begin = Location;
            var token = begin.Token;

            switch (token.Keyword)
            {
                case FastKeywords.async:
                    stream.Consume();
                    if (!Expression(out var fx))
                        throw stream.Unexpected();
                    if (!(fx is AstFunctionExpression func))
                        throw stream.Unexpected();
                    func.Async = true;
                    node = func;
                    return true;
                case FastKeywords.function:
                    return FunctionExpression(out node);
                case FastKeywords.@class:
                    return ClassExpression(out node);
                case FastKeywords.yield:
                    return YieldExpression(out node);
            }
            if (Identitifer(out var id))
            {
                node = id;
                return true;
            }
            switch (token.Type) {
                case TokenTypes.BracketStart:
                    return BracketExpression(out node);
                case TokenTypes.SquareBracketStart:
                    return ArrayExpression(out node);
                case TokenTypes.CurlyBracketStart:
                    return ObjectLiteral(out node);
                case TokenTypes.TemplateBegin:
                    return Template(out node);
                case TokenTypes.TemplateEnd:
                    stream.Consume();
                    node = new AstTemplateExpression(token, token, ArraySpan<AstExpression>.From(node));
                    return true;
                default:
                    throw stream.Unexpected();
            }

            bool Template(out AstExpression node)
            {
                var begin = Location;
                stream.Consume();
                var nodes = Pool.AllocateList<AstExpression>();
                try
                {
                    nodes.Add(new AstLiteral(TokenTypes.String, begin.Token));
                    while (!stream.CheckAndConsume(TokenTypes.TemplateEnd))
                    {
                        if (stream.CheckAndConsume(TokenTypes.TemplatePart, out var token))
                        {
                            nodes.Add(new AstLiteral(TokenTypes.String, token));
                        }
                        if (SingleExpression(out var exp))
                        {
                            nodes.Add(exp);
                            continue;
                        }
                        throw stream.Unexpected();
                    }
                    node = new AstTemplateExpression(begin.Token, PreviousToken, nodes);
                } finally
                {
                    nodes.Clear();
                }
                return true;
            }

            bool BracketExpression(out AstExpression node)
            {
                node = default;
                if(ExpressionList(out var nodes, out var start, out var end, TokenTypes.BracketEnd)) {
                    if(nodes.Length == 0)
                    {
                        node = new AstEmptyExpression(PreviousToken);
                    } else if(nodes.Length == 1)
                    {
                        node = nodes[0];
                    } else
                    {
                        node = new AstSequenceExpression(start, end, nodes);
                    }
                    return true;
                }
                return false;
            }

            bool ArrayExpression(out AstExpression node)
            {
                node = default;
                if (ExpressionList(
                    out var nodes, 
                    out var start, 
                    out var end, TokenTypes.SquareBracketEnd, true)) {
                    node = new AstArrayExpression(start, end, nodes);
                    return true;
                }
                return false;
            }

            bool ExpressionList(
                out ArraySpan<AstExpression> node, 
                out FastToken start, 
                out FastToken end, 
                TokenTypes endType,
                bool allowEmpty = false)
            {
                var begin = Location;
                start = begin.Token;
                stream.Consume();
                var nodes = Pool.AllocateList<AstExpression>();
                try
                {

                    while (!stream.CheckAndConsumeAny(endType, TokenTypes.EOF))
                    {
                        if(stream.CheckAndConsume(TokenTypes.Comma))
                        {
                            if (allowEmpty)
                            {
                                nodes.Add(null);
                                continue;
                            }
                            throw stream.Unexpected();
                        }
                        if (!Expression(out var n))
                            throw stream.Unexpected();
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                        if (stream.CheckAndConsumeAny(endType, TokenTypes.EOF))
                            break;
                        throw stream.Unexpected();
                    }
                    node = nodes;
                    end = PreviousToken;
                    return true;

                } finally
                {
                    nodes.Clear();
                }
            }

            bool YieldExpression(out AstExpression statement)
            {
                var begin = Location;
                stream.Consume();
                bool star = false;
                if (stream.CheckAndConsume(TokenTypes.Multiply))
                {
                    star = true;
                }
                if (Expression(out var target))
                {
                    statement = new AstYieldExpression(begin.Token, PreviousToken, target, star);
                    EndOfStatement();
                    return true;
                }
                throw stream.Unexpected();
            }

        }

    }

}