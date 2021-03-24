using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        bool NextExpression(
            AstExpression previous, TokenTypes previousType,
            out AstExpression node, out TokenTypes type)
        {
            AstExpression right = null;
            TokenTypes rightType = TokenTypes.None;

            switch(previousType)
            {
                // Associate right...
                case TokenTypes.Assign:
                case TokenTypes.AssignAdd:
                case TokenTypes.AssignBitwideAnd:
                case TokenTypes.AssignBitwideOr:
                case TokenTypes.AssignDivide:
                case TokenTypes.AssignLeftShift:
                case TokenTypes.AssignMod:
                case TokenTypes.AssignMultiply:
                case TokenTypes.AssignPower:
                case TokenTypes.AssignRightShift:
                case TokenTypes.AssignSubtract:
                case TokenTypes.AssignUnsignedRightShift:
                case TokenTypes.AssignXor:
                    if (!Expression(out right))
                        throw stream.Unexpected();
                    node = new AstBinaryExpression(previous, previousType, right);
                    type = TokenTypes.SemiColon;
                    return true;
            }

            var preUnaryOperator = GetUnaryOperator(stream.Current);

            if (!SingleExpression(out node))
            {
                if (EndOfStatement())
                {
                    type = TokenTypes.SemiColon;
                    return true;
                }
                type = TokenTypes.None;
                return false;
            }

            if(preUnaryOperator != UnaryOperator.None)
            {
                node = new AstUnaryExpression(node.Start, node, preUnaryOperator);
            }

            var postUnaryOperator = GetUnaryOperator(stream.Current, false);

            if(postUnaryOperator != UnaryOperator.None)
            {
                node = new AstUnaryExpression(node.Start, node, postUnaryOperator, false);
            }

            if(EndOfStatement())
            {
                type = TokenTypes.SemiColon;
                return true;
            }

            var begin = Location;
            type = begin.Token.Type;
            switch (type)
            {

                // associate right...
                case TokenTypes.Assign:
                case TokenTypes.AssignAdd:
                case TokenTypes.AssignBitwideAnd:
                case TokenTypes.AssignBitwideOr:
                case TokenTypes.AssignDivide:
                case TokenTypes.AssignLeftShift:
                case TokenTypes.AssignMod:
                case TokenTypes.AssignMultiply:
                case TokenTypes.AssignPower:
                case TokenTypes.AssignRightShift:
                case TokenTypes.AssignSubtract:
                case TokenTypes.AssignUnsignedRightShift:
                case TokenTypes.AssignXor:
                    throw new FastParseException(begin.Token, "Invalid left hand side assignemnt");

                case TokenTypes.Multiply:
                case TokenTypes.Divide:
                    if (!NextExpression(node, type, out right, out rightType))
                        throw stream.Unexpected();
                    node = new AstBinaryExpression(node, type, right);
                    type = rightType;
                    return true;
                case TokenTypes.Plus:
                case TokenTypes.Minus:
                    if (!NextExpression(node, type, out right, out rightType))
                        throw stream.Unexpected();
                    if (Precedes(rightType, type)) {
                        node = new AstBinaryExpression(node, type, right);
                        type = rightType;
                        return true;
                    }
                    node = new AstBinaryExpression(previous, previousType, node);
                    type = rightType;
                    return true;


            }
            type = TokenTypes.None;
            node = null;
            return false;
        }

        bool Precedes(TokenTypes left, TokenTypes right)
        {
            switch(left)
            {
                case TokenTypes.Multiply:
                case TokenTypes.Divide:
                    return true;
            }
            return false;
        }

        bool Expression(out AstExpression node)
        {
            var begin = Location;
            node = default;
            var token = begin.Token;

            var prefixUnaryToken = GetUnaryOperator(token);

            if (!SingleExpression(out node))
                return begin.Reset();

            if (prefixUnaryToken != UnaryOperator.None)
            {
                node = new AstUnaryExpression(token, node, prefixUnaryToken);
            }
            begin = Location;
            token = begin.Token;
            var postFixUnaryOperator = GetUnaryOperator(token);
            if (postFixUnaryOperator != UnaryOperator.None)
            {
                node = new AstUnaryExpression(token, node, postFixUnaryOperator, false);
            }

            var current = stream.Current;
            if(NextExpression(node, current.Type, out var next, out var nextToken))
            {
                if(next == null)
                {
                    return true;
                }
                node = new AstBinaryExpression(node, nextToken, next);
                return true;
            }

            return begin.Reset();
        }

        private UnaryOperator GetUnaryOperator(FastToken token, bool prefix = true)
        {
            UnaryOperator prefixUnaryToken = UnaryOperator.None;
            switch (token.Type)
            {
                case TokenTypes.Plus:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Plus;
                    break;
                case TokenTypes.Minus:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Minus;
                    break;
                case TokenTypes.Increment:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Increment;
                    break;
                case TokenTypes.Decrement:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Decrement;
                    break;
                case TokenTypes.Negate:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.Negate;
                    break;
                case TokenTypes.BitwiseNot:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.BitwiseNot;
                    break;
            }
            if (!prefix)
                return UnaryOperator.None;
            switch (token.Keyword)
            {
                case FastKeywords.@typeof:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.@typeof;
                    break;
                case FastKeywords.delete:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.delete;
                    break;
                case FastKeywords.@void:
                    stream.Consume();
                    prefixUnaryToken = UnaryOperator.@void;
                    break;
            }

            return prefixUnaryToken;
        }
    }

}
