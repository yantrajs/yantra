using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        bool NextExpression(
            ref AstExpression previous, ref TokenTypes previousType,
            out AstExpression node, out TokenTypes type)
        {

            if (EndOfStatement())
            {
                node = null;
                type = TokenTypes.SemiColon;
                return true;
            }


            AstExpression right = null;
            TokenTypes rightType = TokenTypes.None;

            switch(previousType)
            {

                case TokenTypes.LineTerminator:
                case TokenTypes.SemiColon:
                    node = null;
                    type = TokenTypes.SemiColon;
                    stream.Reset(stream.Position - 1);
                    return true;
                case TokenTypes.Colon:
                    node = null;
                    type = TokenTypes.SemiColon;
                    stream.Reset(stream.Position - 1);
                    return true;

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

                case TokenTypes.BracketStart:
                    if (!ExpressionArray(out var plist))
                        throw stream.Unexpected();
                    previous = new AstCallExpression(previous, plist);
                    previousType = stream.Current.Type;
                    stream.Consume();
                    return NextExpression(ref previous, ref previousType, out node, out type);

                case TokenTypes.QuestionMark:
                    if (!Expression(out var @true))
                        throw stream.Unexpected();
                    stream.Expect(TokenTypes.Colon);
                    if (!Expression(out var @false))
                        throw stream.Unexpected();
                    previous = new AstConditionalExpression(previous, @true, @false);
                    previousType = stream.Current.Type;
                    stream.Consume();
                    return NextExpression(ref previous, ref previousType, out node, out type);
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

            switch(previousType)
            {
                case TokenTypes.Dot:
                    previous = new AstMemberExpression(previous, node);
                    return NextExpression(ref previous, ref previousType, out node, out type);
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
                case TokenTypes.Plus:
                case TokenTypes.Minus:
                case TokenTypes.BitwiseAnd:
                case TokenTypes.BitwiseNot:
                case TokenTypes.BooleanAnd:
                case TokenTypes.BooleanOr:
                case TokenTypes.Xor:
                case TokenTypes.LeftShift:
                case TokenTypes.RightShift:
                case TokenTypes.Less:
                case TokenTypes.LessOrEqual:
                case TokenTypes.Greater:
                case TokenTypes.GreaterOrEqual:
                    if (!NextExpression(ref node, ref type, out right, out rightType))
                        throw stream.Unexpected();
                    if (Precedes(rightType, type)) {
                        node = new AstBinaryExpression(node, type, right);
                        type = rightType;
                        return true;
                    }
                    previous = new AstBinaryExpression(previous, previousType, node);
                    previousType = type;
                    node = right;
                    type = rightType;
                    return true;


            }
            type = TokenTypes.None;
            node = null;
            return false;
        }

        bool Precedes(TokenTypes left, TokenTypes right)
        {
            if (left != TokenTypes.SemiColon && left != TokenTypes.EOF) {
                return left < right;
            }
            return false;
        }

        bool Expression(out AstExpression node)
        {
            var begin = Location;
            node = default;
            var token = begin.Token;

            var prefixUnaryToken = GetUnaryOperator(token);

            var isAsync = false;
            var isGenerator = false;
            if (stream.CheckAndConsume(FastKeywords.async))
                isAsync = true;
            if (stream.CheckAndConsume(TokenTypes.Multiply))
                isGenerator = true;

            if (!SingleExpression(out node))
            {
                // lets check if we have expression sequence
                if(!ExpressionArray(out var nodes))
                    return begin.Reset();
                if(stream.CheckAndConsume(TokenTypes.Lambda))
                {
                    // array function...
                    if(stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                    {
                        if (!Block(out var block))
                            throw stream.Unexpected();
                        node = new AstFunctionExpression(token, PreviousToken, isAsync, isGenerator, null, VariableDeclarator.From(nodes), block);
                        return true;
                    }
                    if (!Expression(out var r))
                        throw stream.Unexpected();
                    node = new AstFunctionExpression(token, PreviousToken, isAsync, isGenerator, null, VariableDeclarator.From(nodes), new AstExpressionStatement( r));
                    return true;
                }
            }

            if(stream.CheckAndConsume(TokenTypes.Lambda))
            {
                if (stream.CheckAndConsume(TokenTypes.CurlyBracketStart))
                {
                    if (!Block(out var block))
                        throw stream.Unexpected();
                    node = new AstFunctionExpression(token, PreviousToken, isAsync, isGenerator, null, VariableDeclarator.From(node), block);
                    return true;
                }
                if (!Expression(out var r))
                    throw stream.Unexpected();
                node = new AstFunctionExpression(token, PreviousToken, isAsync, isGenerator, null, VariableDeclarator.From(node), new AstExpressionStatement(r));
                return true;

            }

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
            var currentType = current.Type;
            if(NextExpression(ref node, ref currentType, out var next, out var nextToken))
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
