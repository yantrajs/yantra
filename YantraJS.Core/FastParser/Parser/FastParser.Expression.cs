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

            AstExpression right;
            TokenTypes rightType;

            switch(previousType)
            {

                /**
                 * Following are single expression terminators
                 */
                
                case TokenTypes.Comma:
                case TokenTypes.LineTerminator:
                case TokenTypes.SemiColon:
                case TokenTypes.SquareBracketEnd:
                case TokenTypes.BracketEnd:
                case TokenTypes.CurlyBracketEnd:
                case TokenTypes.Colon:
                case TokenTypes.EOF:
                    node = null;
                    type = TokenTypes.SemiColon;
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
                    stream.Consume();
                    if (!Expression(out right))
                        throw stream.Unexpected();
                    previous = previous.Combine(previousType, right);
                    node = null;
                    type = TokenTypes.SemiColon;
                    return true;

                case TokenTypes.QuestionMark:
                    stream.CheckAndConsume(previousType);
                    if (!Expression(out var @true))
                        throw stream.Unexpected();
                    stream.Expect(TokenTypes.Colon);
                    if (!Expression(out var @false))
                        throw stream.Unexpected();
                    previous = new AstConditionalExpression(previous, @true, @false);
                    previousType = stream.Current.Type;
                    return NextExpression(ref previous, ref previousType, out node, out type);
            }

            stream.CheckAndConsume(previousType);

            if (!SingleMemberExpression(out node))
            {
                if (EndOfStatement())
                {
                    type = TokenTypes.SemiColon;
                    return true;
                }
                type = TokenTypes.None;
                return false;
            }

            var begin = Location;
            type = begin.Token.Type;

            switch (type)
            {

                case TokenTypes.Comma:
                case TokenTypes.LineTerminator:
                case TokenTypes.SemiColon:
                case TokenTypes.SquareBracketEnd:
                case TokenTypes.BracketEnd:
                case TokenTypes.CurlyBracketEnd:
                case TokenTypes.Colon:
                case TokenTypes.EOF:
                    // previous = previous.Combine(previousType, node);
                    // node = null;
                    type = TokenTypes.SemiColon;
                    return true;


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
                    stream.Consume();
                    if (Precedes(type, previousType)) {
                        if (!NextExpression(ref node, ref type, out right, out rightType))
                            return true;
                        if (type == TokenTypes.SemiColon)
                            return true;
                        node = node.Combine(type, right);
                        type = rightType;
                        return true;
                    }
                    previous = previous.Combine(previousType, node);
                    previousType = type;
                    return NextExpression(ref previous, ref previousType, out node, out type);
                default:
                    throw stream.Unexpected();
            }
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
            var token = begin.Token;

            if (!SinglePrefixPostfixExpression(out node, out var isAsync, out var isGenerator))
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
                        node = new AstFunctionExpression(token, PreviousToken, isAsync, isGenerator, null, VariableDeclarator.From(in nodes), block);
                        return true;
                    }
                    if (!Expression(out var r))
                        throw stream.Unexpected();
                    node = new AstFunctionExpression(token, PreviousToken, isAsync, isGenerator, null, VariableDeclarator.From(in nodes), new AstExpressionStatement( r));
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
