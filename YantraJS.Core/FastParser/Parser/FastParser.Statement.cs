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
                }
            }

            if(Expression(out var expression))
            {
                node = new AstExpressionStatement(begin.Token, PreviousToken, expression);
                return true;
            }

            return begin.Reset();
        }


    }

}
