using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{
    public readonly struct VariableDeclarator
    {
        public readonly AstExpression Identifier;
        public readonly AstExpression Init;

        public VariableDeclarator(AstExpression identifier, AstExpression init)
        {
            Identifier = identifier;
            Init = init;
        }
    }

    internal class AstVariableDeclaration: AstStatement
    {
        public readonly List<VariableDeclarator> Declarators;

        public readonly bool IsLet;
        public readonly bool IsConst;

        public AstVariableDeclaration(FastToken begin, FastToken previousToken, List<VariableDeclarator> declarators, bool isLet, bool isConst)
            :base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = declarators;
            this.IsLet = isLet;
            this.IsConst = isConst;
        }
    }
}