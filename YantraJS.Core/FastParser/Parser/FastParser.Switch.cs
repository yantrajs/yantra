using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{

    public readonly struct AstCase
    {
        public readonly AstExpression Test;
        public readonly IFastEnumerable<AstStatement> Statements;

        public AstCase(AstExpression test, IFastEnumerable<AstStatement> last)
        {
            this.Test = test;
            this.Statements = last;
        }
    }

    partial class FastParser
    {



        bool Switch(out AstStatement node)
        {
            var begin = stream.Current;
            stream.Consume();
            node = null;

            stream.Expect(TokenTypes.BracketStart);
            if (!Expression(out var target))
                throw stream.Unexpected();
            stream.Expect(TokenTypes.BracketEnd);

            stream.Expect(TokenTypes.CurlyBracketStart);
            var nodes = new Sequence<AstCase>();
            var statements = new Sequence<AstStatement>();
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
                            nodes.Add(new AstCase(test, statements));
                            statements = new Sequence<AstStatement>();
                        }
                        if (!Expression(out test))
                            throw stream.Unexpected();
                        stream.Expect(TokenTypes.Colon);
                    } else if(stream.CheckAndConsume(FastKeywords.@default))
                    {
                        stream.Expect(TokenTypes.Colon);
                        if (test != null) {
                            nodes.Add(new AstCase(test, statements));
                            statements = new Sequence<AstStatement>();
                        }
                        test = null;
                        hasDefault = true;
                    } else if (Statement(out var stmt))
                        statements.Add(stmt);
                }

                if(test != null || hasDefault)
                {
                    nodes.Add(new AstCase(test, statements));
                    // statements = new Sequence<AstStatement>();
                }

                node = new AstSwitchStatement(begin, PreviousToken, target, nodes);
                return true;

            } finally
            {
                // nodes.Clear();
                // statements.Clear();
            }
        }


    }

}
