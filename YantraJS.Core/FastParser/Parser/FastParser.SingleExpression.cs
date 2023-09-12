using System;
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
        bool SingleExpression(out AstExpression node, bool afterDot = false)
        {
            var begin = stream.Current;
            var token = begin;

            if (afterDot)
            {
                // after .
                // even keywords are accepted as a member name
                switch (token.Type)
                {
                    case TokenTypes.Identifier:
                    case TokenTypes.In:
                    case TokenTypes.InstanceOf:
                    case TokenTypes.Null:
                    case TokenTypes.True:
                    case TokenTypes.False:
                        node = new AstIdentifier(token.AsString());
                        stream.Consume();
                        return true;
                }
            }

            if (Literal(out node))
                return true;

            switch (token.Keyword)
            {
                case FastKeywords.async:
                    stream.Consume();
                    if (!Expression(out var fx))
                        throw stream.Unexpected();
                    if (!(fx.IsFunction(out var func)))
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
                case FastKeywords.await:
                    return AwaitExpression(out node);
                case FastKeywords.super:
                    stream.Consume();
                    node = new AstSuper(token);
                    return true;
                //case FastKeywords.@new:
                //    stream.Consume();
                //    if (!Expression(out var target))
                //        throw stream.Unexpected();
                //    //stream.Expect(TokenTypes.BracketStart);
                //    //if (!ExpressionArray(out var arguments))
                //    //    throw stream.Unexpected();
                //    node = new AstNewExpression(begin.Token, target, arguments);
                //    return true;
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
                    node = Template();
                    return true;
                case TokenTypes.TemplateEnd:
                    stream.Consume();
                    node = new AstTemplateExpression(token, token, new Sequence<AstExpression>(1) { new AstLiteral(token.Type, token) });
                    return true;
                case TokenTypes.EOF:
                case TokenTypes.Comma:
                case TokenTypes.BracketEnd:
                case TokenTypes.SquareBracketEnd:
                case TokenTypes.CurlyBracketEnd:
                case TokenTypes.LineTerminator:
                case TokenTypes.SemiColon:
                    return false;

                default:
                    throw stream.Unexpected();
            }

            bool BracketExpression(out AstExpression node)
            {
                node = default;
                if(ExpressionList(out var nodes, out var start, out var end, TokenTypes.BracketEnd)) {
                    if(nodes.Count == 0)
                    {
                        node = new AstEmptyExpression(PreviousToken);
                    } else if(nodes.Count == 1)
                    {
                        node = nodes.First();
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
                out IFastEnumerable<AstExpression> node, 
                out FastToken start, 
                out FastToken end, 
                TokenTypes endType,
                bool allowEmpty = false)
            {
                var begin = stream.Current;
                start = begin;
                stream.Consume();
                var nodes = new Sequence<AstExpression>();
                try
                {

                    while (!stream.CheckAndConsume(endType))
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
                        var spread = stream.CheckAndConsume(TokenTypes.TripleDots, out var token);
                        if (!Expression(out var n))
                            throw stream.Unexpected();
                        if(spread)
                        {
                            n = new AstSpreadElement(token, n.End, n);
                        }
                        nodes.Add(n);
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                        if (stream.CheckAndConsume(endType))
                            break;
                        throw stream.Unexpected();
                    }
                    node = nodes;
                    end = PreviousToken;
                    return true;

                } finally
                {
                    // nodes.Clear();
                }
            }

            bool YieldExpression(out AstExpression statement)
            {
                var begin = stream.Current;
                stream.Consume();
                bool star = false;
                if (stream.CheckAndConsume(TokenTypes.Multiply))
                {
                    star = true;
                }
                if (Expression(out var target))
                {
                    statement = new AstYieldExpression(begin, PreviousToken, target, star);
                    EndOfStatement();
                    return true;
                }
                throw stream.Unexpected();
            }

            bool AwaitExpression(out AstExpression statement)
            {
                var begin = stream.Current;
                stream.Consume();

                if (Expression(out var target))
                {
                    this.isAsync = true;
                    statement = new AstAwaitExpression(begin, PreviousToken, target);
                    EndOfStatement();
                    return true;
                }
                throw stream.Unexpected();
            }

        }

    }

}
