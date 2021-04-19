using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {

        int lastStatementPosition = 0;
        bool Statement(out AstStatement node)
        {

            PreventStackoverFlow(ref lastStatementPosition);

            var begin = Location;

            var token = begin.Token;
            switch (token.Type)
            {
                case TokenTypes.CurlyBracketStart:
                    stream.Consume();
                    if (Block(out var block))
                    {
                        node = block;
                        return true;
                    }
                    break;
                case TokenTypes.SemiColon:
                    stream.Consume();
                    node = new AstExpressionStatement(new AstEmptyExpression(token));
                    return true;
            }

            if (SingleStatement(begin, out node))
            {
                stream.CheckAndConsumeAny(TokenTypes.SemiColon, TokenTypes.LineTerminator);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool SingleStatement(in StreamLocation begin, out AstStatement node)
        {
            var token = begin.Token;
            if(token.IsKeyword)
            {
                switch (token.Keyword)
                {
                    case FastKeywords.var:
                        return VariableDeclaration(out node);
                    case FastKeywords.let:
                        return VariableDeclaration(out node, FastVariableKind.Let);
                    case FastKeywords.@const:
                        return VariableDeclaration(out node, FastVariableKind.Const);
                    case FastKeywords.@if:
                        return IfStatement(out node);
                    case FastKeywords.@while:
                        return WhileStatement(out node);
                    case FastKeywords.@do:
                        return DoWhileStatement(out node);
                    case FastKeywords.@for:
                        return ForStatement(out node);
                    case FastKeywords.@continue:
                        return Continue(out node);
                    case FastKeywords.@break:
                        return Break(out node);
                    case FastKeywords.@return:
                        return Return(out node);
                    case FastKeywords.with:
                        throw stream.Unexpected();
                    case FastKeywords.@switch:
                        return Switch(out node);
                    case FastKeywords.@throw:
                        return Throw(out node);
                    case FastKeywords.@try:
                        return Try(out node);
                    case FastKeywords.debugger:
                        return Debugger(out node);
                    case FastKeywords.@class:
                        return Class(out node);
                    case FastKeywords.export:
                        return Export(token, out node);
                    case FastKeywords.import:
                        return Import(token, out node);
                    case FastKeywords.async:
                        stream.Consume();
                        if (stream.Current.Keyword != FastKeywords.function)
                            throw stream.Unexpected();
                        return Function(out node, true);
                    case FastKeywords.function:
                        return Function(out node);

                }
            }

            // goto....
            if (LabeledLoop(out node))
                return true;

            if(ExpressionSequence(out var expression, TokenTypes.SemiColon))
            {
                node = new AstExpressionStatement(token, PreviousToken, expression);
                return true;
            }

            return begin.Reset();

            bool LabeledLoop(out AstStatement statement)
            {
                if(stream.CheckAndConsume(TokenTypes.Identifier, TokenTypes.Colon, out var id, out var _))
                {
                    // has to be do/while/for...
                    var current = stream.Current;
                    switch (current.Keyword)
                    {
                        case FastKeywords.@do:
                            if (!DoWhileStatement(out statement))
                                throw stream.Unexpected();
                            break;
                        case FastKeywords.@for:
                            if (!ForStatement(out statement))
                                throw stream.Unexpected();
                            break;
                        case FastKeywords.@while:
                            if (!WhileStatement(out statement))
                                throw stream.Unexpected();
                            break;
                        default:
                            throw stream.Unexpected();
                    }

                    statement = new AstLabeledStatement(id, statement);
                    return true;
                }
                statement = null;
                return false;
            }

            bool Debugger(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();
                statement = new AstDebuggerStatement(begin.Token);
                EndOfStatement();
                return true;
            }

            bool Try(out  AstStatement statement)
            {
                var begin = Location;
                stream.Consume();

                if (!Statement(out var body))
                    throw stream.Unexpected();

                // we may not have catch...
                if(stream.CheckAndConsume(FastKeywords.@catch)) {

                    stream.Expect(TokenTypes.BracketStart);
                    if (!Identitifer(out var id))
                        throw stream.Unexpected();
                    stream.Expect(TokenTypes.BracketEnd);

                    if (!Statement(out var @catch))
                        throw stream.Unexpected();
                    Finally(out var @finally);
                    statement = new AstTryStatement(begin.Token, PreviousToken, body, id, @catch, @finally);
                    return true;
                }
                else if (Finally(out var @finally))
                {
                    statement = new AstTryStatement(begin.Token, PreviousToken, body, null, null, @finally);
                    return true;
                }
                else
                    throw stream.Unexpected();

            }

            bool Finally(out AstStatement statement)
            {
                statement = null;
                if (!stream.CheckAndConsume(FastKeywords.@finally))
                    return false;
                if (!Statement(out statement))
                    throw stream.Unexpected();
                return true;
            }

            bool Throw(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();

                if (!Expression(out var target))
                    stream.Unexpected();

                statement = new AstThrowStatement(begin.Token, PreviousToken, target);
                return true;
            }

            bool Continue(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();
                
                Identitifer(out var id);

                statement = new AstContinueStatement(begin.Token, PreviousToken, id);
                return true;
            }

            bool Break(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();

                Identitifer(out var id);

                statement = new AstBreakStatement(begin.Token, PreviousToken, id);
                return true;
            }

            bool Return(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();

                var current = stream.Current;
                if(current.Type ==  TokenTypes.SemiColon || current.Type == TokenTypes.LineTerminator)
                {
                    statement = new AstReturnStatement(begin.Token, current);
                    return true;
                }

                if(ExpressionSequence(out var target, TokenTypes.SemiColon))
                {
                    statement = new AstReturnStatement(begin.Token, PreviousToken, target);
                    EndOfStatement();
                    return true;
                }
                throw stream.Unexpected();
            }
        }


    }

}
