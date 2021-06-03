using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class AstIdentifier : AstExpression
    {
        public readonly StringSpan Name;

        public AstIdentifier(FastToken identifier) : base(identifier,  FastNodeType.Identifier, identifier)
        {
            this.Name = identifier.Span;
        }

        public AstIdentifier(FastToken token, string id) : base(token, FastNodeType.Identifier, token)
        {
            this.Name = id;
        }


        public override string ToString()
        {
            return Name.Value ;
        }

    }
}
