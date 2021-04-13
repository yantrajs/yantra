using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        int lastNextExpressionPosition = 0;

        /// <summary>
        /// NextExpression evaluates and reads next set of tokens,
        /// It decides precedence of right side expression and combines
        /// expressions and returns true/false if expression has parsed successfully.
        /// 
        /// It will return true if expression ends successfully.
        /// 
        /// It will return true if no computable expression is found. It will only
        /// return false if parser does not find a valid expression, but it could be
        /// parsed by some other rule.
        /// 
        /// It will rewrite next token as semicolon.
        /// 
        /// This uses recursive calls as using inbuilt .net stack is much easier rather
        /// than using our own stack.
        /// 
        /// There might be some issue with stack overflow.. we will revisit it when needed.
        /// In that case we might introduce local stack.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="previousType"></param>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool NextExpression(
            ref AstExpression previous, ref TokenTypes previousType,
            out AstExpression node, out TokenTypes type)
        {

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
                case TokenTypes.CurlyBracketStart:
                case TokenTypes.Colon:
                case TokenTypes.EOF:
                case TokenTypes.TemplateBegin:
                case TokenTypes.TemplatePart:
                case TokenTypes.TemplateEnd:
                    node = null;
                    type = TokenTypes.SemiColon;
                    return true;

            }

            PreventStackoverFlow(ref lastNextExpressionPosition);

            AstExpression right;
            TokenTypes rightType;

            //if (previous.End.LineTerminator)
            //{
            //    node = null;
            //    type = TokenTypes.SemiColon;
            //    return true;
            //}

            switch (previousType)
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

            if (!SinglePrefixPostfixExpression(out node, out var x, out var b))
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
            if (node.End.LineTerminator) {
                type = TokenTypes.SemiColon;
                return true;
            }
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

                case TokenTypes.QuestionMark:
                    stream.Consume();
                    previous = previous.Combine(previousType, node);
                    if (!Expression(out var @true))
                        throw stream.Unexpected();
                    stream.Expect(TokenTypes.Colon);
                    if (!Expression(out var @false))
                        throw stream.Unexpected();
                    previous = new AstConditionalExpression(previous, @true, @false);
                    previousType = stream.Current.Type;
                    return NextExpression(ref previous, ref previousType, out node, out type);

                case TokenTypes.Multiply:
                case TokenTypes.Divide:
                case TokenTypes.Plus:
                case TokenTypes.Minus:
                case TokenTypes.BitwiseAnd:
                case TokenTypes.BitwiseOr:
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
                case TokenTypes.In:
                case TokenTypes.InstanceOf:
                case TokenTypes.StrictlyEqual:
                case TokenTypes.StrictlyNotEqual:
                case TokenTypes.Equal:
                case TokenTypes.NotEqual:
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
                    return false;
            }
        }

        bool Precedes(TokenTypes left, TokenTypes right)
        {
            if (left != TokenTypes.SemiColon && left != TokenTypes.EOF) {
                return left < right;
            }
            return false;
        }

        
    }

}
