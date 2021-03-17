using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class AstIdentifier : AstExpression
    {
        public readonly FastToken Identifier;

        public AstIdentifier(FastToken identifier) : base(identifier,  FastNodeType.Identifier, identifier)
        {
            this.Identifier = identifier;
        }
    }
}
