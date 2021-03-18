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
            }

            if(token.IsKeyword)
            {
                switch (token.Keyword)
                {
                    case FastKeywords.var:
                        return VariableDeclaration(out node);
                    case FastKeywords.let:
                        return VariableDeclaration(out node, isLet: true);
                    case FastKeywords.@const:
                        return VariableDeclaration(out node, isConst: true);
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
                    case FastKeywords.yield:
                        return Yield(out node);
                    case FastKeywords.with:
                        throw stream.Unexpected();
                    //case FastKeywords.@switch:
                    //    return Switch(out node);
                    case FastKeywords.@throw:
                    case FastKeywords.@try:
                    case FastKeywords.debugger:
                    case FastKeywords.@class:
                    case FastKeywords.function:
                        throw stream.Unexpected();

                }
            }

            if(Expression(out var expression))
            {
                node = new AstExpressionStatement(begin.Token, PreviousToken, expression);
                return true;
            }

            return begin.Reset();

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

            bool Yield(out AstStatement statement)
            {
                var begin = Location;
                stream.Consume();
                bool star = false;
                if(stream.CheckAndConsume(TokenTypes.Multiply))
                {
                    star = true;
                }
                if (Expression(out var target))
                {
                    statement = new AstYieldStatement(begin.Token, PreviousToken, target, star);
                    EndOfStatement();
                    return true;
                }
                throw stream.Unexpected();
            }
        }


    }

}
