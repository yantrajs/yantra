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

            //if (EndOfStatement())
            //{
            //    node = null;
            //    type = TokenTypes.SemiColon;
            //    return true;
            //}


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

                case TokenTypes.BracketStart:
                    stream.CheckAndConsume(previousType);
                    if (!ExpressionArray(out var plist))
                        throw stream.Unexpected();
                    previous = new AstCallExpression(previous, plist);
                    previousType = stream.Current.Type;
                    return NextExpression(ref previous, ref previousType, out node, out type);

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

                //case TokenTypes.SquareBracketStart:
                //    stream.CheckAndConsume(previousType);
                //    if (!ExpressionSequence(out var list, TokenTypes.SquareBracketEnd))
                //        throw stream.Unexpected();
                //    previous = new AstMemberExpression(previous, list, true);
                //    previousType = stream.Current.Type;
                //    return NextExpression(ref previous, ref previousType, out node, out type);

            }

            stream.CheckAndConsume(previousType);


            var preUnaryOperator = GetUnaryOperator(stream.Current);

            if (!SingleComputedExpression(out node))
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

            //switch(previousType)
            //{
            //    case TokenTypes.Dot:
            //        previous = new AstMemberExpression(previous, node);
            //        return NextExpression(ref previous, ref previousType, out node, out type);
            //}


            var begin = Location;
            type = begin.Token.Type;

            //switch (type)
            //{
            //    case TokenTypes.SquareBracketStart:
            //        stream.Consume();
            //        if (!ExpressionSequence(out var arguments, TokenTypes.SquareBracketEnd))
            //            throw stream.Unexpected();
            //        node = node.Computed(arguments);
            //        begin = Location;
            //        type = begin.Token.Type;
            //        break;
            //}

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
                    previous = previous.Combine(previousType, node);
                    node = null;
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
                    if (!NextExpression(ref node, ref type, out right, out rightType))
                        throw stream.Unexpected();
                    if (Precedes(rightType, type)) {
                        node = node.Combine(type, right);
                        type = rightType;
                        return true;
                    }
                    previous = previous.Combine(previousType, node);
                    previousType = type;
                    node = right;
                    type = rightType;
                    return true;
                //case TokenTypes.BracketStart:
                //    stream.Consume();
                //    // method call..
                //    if (!ExpressionArray(out var arguments))
                //        throw stream.Unexpected();
                //    if(previousType == TokenTypes.Dot)
                //    {
                //        previous = new AstCallExpression( new AstMemberExpression(previous, node), arguments);
                //        previousType = stream.Current.Type;
                //        return NextExpression(ref previous, ref previousType, out node, out type);
                //    }
                //    node = new AstCallExpression(node, arguments);
                //    type = stream.Current.Type;
                //    if (!NextExpression(ref node, ref type, out right, out rightType))
                //        return true;
                //    previous = previous.Combine(previousType, node);
                //    previousType = type;
                //    node = right;
                //    type = rightType;
                //    return true;
                //case TokenTypes.Dot:
                //    stream.Consume();
                //    if(previousType == TokenTypes.Dot)
                //    {
                //        previous = new AstMemberExpression(previous, node);
                //        previousType = type;
                //        return NextExpression(ref previous, ref previousType, out node, out type);
                //    }
                //    if (!NextExpression(ref node, ref type, out right, out rightType))
                //        throw stream.Unexpected();
                //    node = node.Combine(rightType, right);
                //    type = rightType;
                //    return true;
                //case TokenTypes.SquareBracketStart:
                //    stream.Consume();
                //    if (!ExpressionSequence(out var index, TokenTypes.SquareBracketEnd))
                //        throw stream.Unexpected();
                //    node = new AstMemberExpression(node, index, true);
                //    type = stream.Current.Type;
                //    if (!NextExpression(ref node, ref type, out right, out rightType))
                //        return true;
                    //// combine... based on what...
                    //if(Precedes(previousType, type))
                    //{
                    //    previous = previous.Combine(previousType, node);
                    //    previousType = type;
                    //    node = right;
                    //    type = rightType;
                    //    return true;
                    //}
                    //node = node.Combine(type, right);
                    //type = rightType;
                    // return true;
                default:
                    throw stream.Unexpected();


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

        /// <summary>
        /// a[]
        /// a.a
        /// a()
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool SingleComputedExpression(out AstExpression node)
        {
            AstExpression prev = null;
            bool computed = false;
            do
            {
                var begin = Location;
                var token = begin.Token;
                if (!SingleExpression(out node))
                    return false;
                node = prev.Member(node, computed);

                begin = Location;
                token = begin.Token;
                switch (token.Type)
                {
                    case TokenTypes.SquareBracketStart:
                        stream.Consume();
                        if (!ExpressionSequence(out var index, TokenTypes.SquareBracketEnd))
                            throw stream.Unexpected();
                        node = node.Member(index, true);
                        break;
                    case TokenTypes.BracketStart:
                        stream.Consume();
                        if (!ExpressionArray(out var arguments))
                            throw stream.Unexpected();
                        node = new AstCallExpression(node, arguments);
                        break;
                }

                begin = Location;
                token = begin.Token;
                switch (token.Type)
                {
                    case TokenTypes.Dot:
                        prev = node;
                        stream.Consume();
                        continue;
                    default:
                        return true;
                }
            } while (true);

        }

        bool Expression(out AstExpression node)
        {
            var begin = Location;
            var token = begin.Token;

            var prefixUnaryToken = GetUnaryOperator(token);

            var isAsync = false;
            var isGenerator = false;
            if (stream.CheckAndConsume(FastKeywords.async))
                isAsync = true;
            if (stream.CheckAndConsume(TokenTypes.Multiply))
                isGenerator = true;

            if (!SingleComputedExpression(out node))
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

            if (prefixUnaryToken != UnaryOperator.None)
            {
                node = new AstUnaryExpression(token, node, prefixUnaryToken);
            }
            begin = Location;
            token = begin.Token;
            var postFixUnaryOperator = GetUnaryOperator(token, false);
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
                node = node.Combine(nextToken, next);
                return true;
            }

            return true;
        }

        private UnaryOperator GetUnaryOperator(FastToken token, bool prefix = true)
        {
            UnaryOperator prefixUnaryToken = UnaryOperator.None;
            switch (token.Type)
            {
                case TokenTypes.Plus:
                    if (prefix)
                    {
                        stream.Consume();
                        prefixUnaryToken = UnaryOperator.Plus;
                    }
                    break;
                case TokenTypes.Minus:
                    if (prefix)
                    {
                        stream.Consume();
                        prefixUnaryToken = UnaryOperator.Minus;
                    }
                    break;
                case TokenTypes.Increment:
                    stream.Consume();
                    return UnaryOperator.Increment;
                case TokenTypes.Decrement:
                    stream.Consume();
                    return UnaryOperator.Decrement;
                case TokenTypes.Negate:
                    stream.Consume();
                    return UnaryOperator.Negate;
                case TokenTypes.BitwiseNot:
                    stream.Consume();
                    return UnaryOperator.BitwiseNot;
            }
            if (!prefix)
                return UnaryOperator.None;
            switch (token.Keyword)
            {
                case FastKeywords.@typeof:
                    stream.Consume();
                    return UnaryOperator.@typeof;
                case FastKeywords.delete:
                    stream.Consume();
                    return UnaryOperator.delete;
                case FastKeywords.@void:
                    stream.Consume();
                    return UnaryOperator.@void;
            }

            return prefixUnaryToken;
        }
    }

}
