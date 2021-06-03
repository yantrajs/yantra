using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.FastParser
{


    partial class FastParser
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SkipNewLines()
        {
            var type = stream.Current.Type;
            while (type == TokenTypes.LineTerminator)
            {
                type = stream.Consume().Type;
            }
        }

        bool Block(out AstBlock node)
        {
            var begin = stream.Current;
            var list = Pool.AllocateList<AstStatement>();
            var scope = variableScope.Push(begin, FastNodeType.Block);

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
                    if (stream.CheckAndConsumeWithLineTerminator(TokenTypes.SemiColon))
                        continue;
                    if (stream.CheckAndConsumeAny(TokenTypes.CurlyBracketEnd, TokenTypes.EOF))
                        break;
                    throw stream.Unexpected();
                } while (true);
                node = new AstBlock(begin, PreviousToken, list);
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
