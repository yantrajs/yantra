﻿namespace YantraJS.Core.FastParser
{
    internal class AstBreakStatement : AstStatement
    {
        public readonly AstIdentifier Label;

        public AstBreakStatement(FastToken token, FastToken previousToken, AstIdentifier label = null)
            : base(token, FastNodeType.ContinueStatement, previousToken)
        {
            this.Label = label;
        }
    }
}