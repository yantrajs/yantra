using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    public class AstVariableDeclaration: AstStatement
    {
        public readonly ArraySpan<VariableDeclarator> Declarators;

        public readonly bool IsLet;
        public readonly bool IsConst;

        public AstVariableDeclaration(
            FastToken begin, 
            FastToken previousToken, 
            in ArraySpan<VariableDeclarator> declarators, 
            bool isLet, 
            bool isConst)
            :base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = declarators;
            this.IsLet = isLet;
            this.IsConst = isConst;
        }

        public override string ToString()
        {
            if(IsLet)
            {
                return $"let {Declarators.Join()}";
            }
            if (IsConst)
            {
                return $"const {Declarators.Join()}";
            }
            return $"var {Declarators.Join()}";
        }
    }
}