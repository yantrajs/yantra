using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        bool Block(out AstBlock node)
        {
            var begin = Location;
            var list = Pool.AllocateList<AstStatement>();
            var scope = variableScope.Push(begin.Token, FastNodeType.Block);
            try
            {
                do
                {
                    if (stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
                        break;
                    if (Statement(out var stmt))
                    {
                        // ignore empty expression statement...
                        if (stmt.Type == FastNodeType.ExpressionStatement
                            && stmt is AstExpressionStatement exp
                            && exp.Expression.Type == FastNodeType.EmptyExpression)
                            continue;
                        list.Add(stmt);
                        continue;
                    }
                    if (stream.CheckAndConsumeAny(TokenTypes.LineTerminator,TokenTypes.SemiColon))
                        continue;
                    if (stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
                        break;
                    throw stream.Unexpected();
                } while (true);
                node = new AstBlock(begin.Token, PreviousToken, list);
                node.HoistingScope = scope.GetVariables();
            } finally
            {
                list.Clear();
                scope.Dispose();
            }
            return true;
        }


    }

}
