using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    internal class AstVariableDeclaration: AstStatement
    {
        public readonly VariableDeclarator[] Declarators;

        public readonly bool IsLet;
        public readonly bool IsConst;

        public AstVariableDeclaration(FastToken begin, FastToken previousToken, VariableDeclarator[] declarators, bool isLet, bool isConst)
            :base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = declarators;
            this.IsLet = isLet;
            this.IsConst = isConst;
        }
    }
}