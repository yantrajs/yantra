using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class AstIdentifier : AstExpression
    {
        public readonly StringSpan Identifier;

        public AstIdentifier(FastToken identifier) : base(identifier,  FastNodeType.Identifier, identifier)
        {
            this.Identifier = identifier.Span;
        }

        public AstIdentifier(FastToken token, string id) : base(token, FastNodeType.Identifier, token)
        {
            this.Identifier = id;
        }


        public override string ToString()
        {
            return Identifier.Value ;
        }

    }
}
