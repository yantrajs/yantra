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
            }

            return begin.Reset();

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
                if (ExpressionList(out var nodes, out var start, out var end, TokenTypes.SquareBracketEnd)) {
                    node = new AstArrayExpression(start, end, nodes);
                    return true;
                }
                return false;
            }

            bool ExpressionList(out ArraySpan<AstExpression> node, out FastToken start, out FastToken end, TokenTypes endType)
            {
                var begin = Location;
                start = begin.Token;
                stream.Consume();
                var nodes = Pool.AllocateList<AstExpression>();
                try
                {

                    while (!stream.CheckAndConsume(TokenTypes.BracketEnd))
                    {
                        if (!Expression(out var n))
                            throw stream.Unexpected();
                        if (stream.CheckAndConsume(TokenTypes.Comma))
                            continue;
                        if (stream.CheckAndConsume(TokenTypes.BracketEnd))
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
        }

    }

}
