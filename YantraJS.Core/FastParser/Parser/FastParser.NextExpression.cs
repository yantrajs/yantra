using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        public AstExpression Combine(AstExpression left,
            TokenTypes type,
            AstExpression right, TokenTypes next = TokenTypes.SemiColon)
        {
            if (right == null)
                return left;
            switch (type)
            {
                case TokenTypes.SemiColon:
                case TokenTypes.EOF:
                case TokenTypes.BracketEnd:
                case TokenTypes.SquareBracketEnd:
                case TokenTypes.CurlyBracketEnd:
                case TokenTypes.LineTerminator:
                    return left;
                case TokenTypes.QuestionMark:
                    if (next != TokenTypes.Colon)
                        throw stream.Unexpected();
                    if (!Expression(out var @false))
                        throw stream.Unexpected();
                    return new AstConditionalExpression(left, right, @false);
            }
            if (type == TokenTypes.Dot)
                return new AstMemberExpression(left, right);
            if (type == TokenTypes.QuestionDot)
                return new AstMemberExpression(left, right, false, true);
            return new AstBinaryExpression(left, type, right);
        }


        FastToken lastNextExpressionPosition;

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
            out AstExpression node, out TokenTypes type, int depth = 0)
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
                case TokenTypes.In:
                    if (!considerInOfAsOperators)
                    {
                        node = null;
                        type = TokenTypes.SemiColon;
                        return true;
                    }
                    break;
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

                    // left must be converted to asssignable...
                    if (previous.Type == FastNodeType.ArrayExpression || previous.Type == FastNodeType.ObjectLiteral)
                    {
                        previous = previous.ToPattern();
                    }

                    previous = Combine(previous, previousType, right);
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
                    if(stream.Previous.Type == TokenTypes.SemiColon)
                    {
                        node = null;
                        type = TokenTypes.SemiColon;
                        return true;
                    }
                    return NextExpression(ref previous, ref previousType, out node, out type, depth+1);
            }

            stream.CheckAndConsume(previousType);

            stream.SkipNewLines();

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

            var m = stream.SkipNewLines();

            var begin = stream.Current;
            type = begin.Type;
            if(node.End.Type == TokenTypes.SemiColon)
            {
                type = TokenTypes.SemiColon;
                return true;
            }

            if(m.LinesSkipped && !type.IsOperator())
            {
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
                case TokenTypes.TemplateEnd:
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
                    throw new FastParseException(begin, 
                        $"Invalid left hand side assignemnt at {begin.Start}");

                case TokenTypes.QuestionMark:

                    // we should not take a decision here
                    // pass it on to previous expression...

                    stream.Consume();
                    if (depth == 0)
                    {
                        previous = Combine(previous, previousType, node);
                        if (!Expression(out var @true))
                            throw stream.Unexpected();
                        stream.Expect(TokenTypes.Colon);
                        if (!Expression(out var @false))
                            throw stream.Unexpected();
                        previous = new AstConditionalExpression(previous, @true, @false);
                        // end of expression ??
                        // previousType = stream.Current.Type;
                        // return NextExpression(ref previous, ref previousType, out node, out type);
                        node = null;
                        type = TokenTypes.SemiColon;
                    }
                    return true;

                case TokenTypes.Multiply:
                case TokenTypes.Divide:
                case TokenTypes.Mod:
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
                case TokenTypes.UnsignedRightShift:
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
                case TokenTypes.Power:

                    // check type first as it may be in
                    // recent memory accessed...
                    if(type == TokenTypes.In)
                    {
                        if (!considerInOfAsOperators)
                        {
                            type = TokenTypes.SemiColon;
                            return true;
                        }
                    }

                    stream.Consume();
                    do
                    {
                        if (Precedes(type, previousType))
                        {
                            if (!NextExpression(ref node, ref type, out right, out rightType, depth + 1))
                                break;
                            if (type == TokenTypes.SemiColon)
                                return true;
                            node = Combine(node, type, right);
                            type = rightType;
                            if (type == TokenTypes.SemiColon)
                                break;
                            continue;
                        }
                        previous = Combine(previous, previousType, node);
                        previousType = type;
                        if (!NextExpression(ref previous, ref previousType, out node, out type, depth + 1))
                            break;
                        if (type == TokenTypes.SemiColon)
                            return true;
                    } while (true);
                    return true;
                default:
                    return false;
            }
        }

        bool Precedes(TokenTypes left, TokenTypes right)
        {
            int CalculatePrecedence(TokenTypes token)
            {
                switch (token) {
                    case TokenTypes.Mod:
                    case TokenTypes.Divide:
                    case TokenTypes.Multiply:
                        return 1;
                    case TokenTypes.Plus:
                    case TokenTypes.Minus:
                        return 2;
                    case TokenTypes.LeftShift:
                    case TokenTypes.RightShift:
                    case TokenTypes.UnsignedRightShift:
                        return 3;
                    case TokenTypes.Less:
                    case TokenTypes.LessOrEqual:
                    case TokenTypes.Greater:
                    case TokenTypes.GreaterOrEqual:
                    case TokenTypes.In:
                    case TokenTypes.InstanceOf:
                        return 4;
                    case TokenTypes.Equal:
                    case TokenTypes.NotEqual:
                    case TokenTypes.StrictlyEqual:
                    case TokenTypes.StrictlyNotEqual:
                        return 5;
                    case TokenTypes.Coalesce:
                        return 6;
                    case TokenTypes.BitwiseAnd:
                        return 7;
                    case TokenTypes.Xor:
                        return 8;
                    case TokenTypes.BitwiseOr:
                        return 9;
                    case TokenTypes.BooleanAnd:
                        return 10;
                    case TokenTypes.BooleanOr:
                        return 11;
                }

                return int.MaxValue;
            }

            if (left != TokenTypes.SemiColon && left != TokenTypes.EOF) {
                return CalculatePrecedence(left) < CalculatePrecedence(right);
            }
            return false;
        }

        
    }

}
