using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    partial class FastParser
    {



        bool Statement(out AstStatement node)
        {
            var begin = Location;
            node = default;

            var token = stream.Current;
            switch (token.Type)
            {
                case TokenTypes.CurlyBracketStart:
                    stream.Consume();
                    if(Block(out var block))
                    {
                        node = block;
                        return true;
                    }
                    break;
                case TokenTypes.SemiColon:
                    stream.Consume();
                    node = new AstExpressionStatement(new AstEmptyExpression(begin.Token));
                    return true;
            }

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

            if(Expression(out var expression))
            {
                node = new AstExpressionStatement(begin.Token, PreviousToken, expression);
                return true;
            }

            return begin.Reset();

            bool LabeledLoop(out AstStatement statement)
            {
                var begin = Location;

                if(stream.CheckAndConsume(TokenTypes.Identifier, out var id))
                {
                    if (stream.CheckAndConsume(TokenTypes.Colon))
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
                }
                statement = null;
                return begin.Reset();
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
                stream.Expect(TokenTypes.CurlyBracketStart);
                if (!Statement(out statement))
                    throw stream.Unexpected();
                stream.Expect(TokenTypes.CurlyBracketEnd);
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

                EndOfStatement();
                statement = new AstContinueStatement(begin.Token, PreviousToken, id);
                return true;
            }

            bool Break(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();

                Identitifer(out var id);

                EndOfStatement();
                statement = new AstBreakStatement(begin.Token, PreviousToken, id);
                return true;
            }

            bool Return(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();
                if (EndOfStatement())
                {
                    statement = new AstReturnStatement(begin.Token, PreviousToken);
                    return true;
                }

                if(Expression(out var target))
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
