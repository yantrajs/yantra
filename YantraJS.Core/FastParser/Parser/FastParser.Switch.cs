using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public readonly struct AstCase
    {
        public readonly AstExpression Test;
        public readonly ArraySpan<AstStatement> Statements;

        public AstCase(AstExpression test, in ArraySpan<AstStatement> last)
        {
            this.Test = test;
            this.Statements = last;
        }
    }

    partial class FastParser
    {



        bool Switch(out AstStatement node)
        {
            var begin = Location;
            stream.Consume();
            node = null;

            stream.Expect(TokenTypes.BracketStart);
            if (!Expression(out var target))
                throw stream.Unexpected();
            stream.Expect(TokenTypes.BracketEnd);

            stream.Expect(TokenTypes.CurlyBracketStart);
            var nodes = Pool.AllocateList<AstCase>();
            var statements = Pool.AllocateList<AstStatement>();
            AstExpression test = null;
            bool hasDefault = false;
            try
            {
                while (!stream.CheckAndConsume(TokenTypes.CurlyBracketEnd))
                {
                    if(stream.CheckAndConsume(FastKeywords.@case))
                    {
                        if (test != null)
                        {
                            nodes.Add(new AstCase(test, statements.ToSpan()));
                        }
                        if (!Expression(out test))
                            throw stream.Unexpected();
                        stream.Expect(TokenTypes.Colon);
                    } else if(stream.CheckAndConsume(FastKeywords.@default))
                    {
                        stream.Expect(TokenTypes.Colon);
                        if (test != null) {
                            nodes.Add(new AstCase(test, statements.ToSpan()));
                        }
                        test = null;
                        hasDefault = true;
                    } else if (Statement(out var stmt))
                        statements.Add(stmt);
                }

                if(test != null || hasDefault)
                {
                    nodes.Add(new AstCase(test, statements.ToSpan()));
                }

                node = new AstSwitchStatement(begin.Token, PreviousToken, target, nodes);
                return true;

            } finally
            {
                nodes.Clear();
                statements.Clear();
            }
        }


    }

}
