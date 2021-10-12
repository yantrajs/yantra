using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    public enum FastVariableKind
    {
        None,
        Let,
        Const,
        Var
    }

    public class AstVariableDeclaration: AstStatement
    {
        public readonly IFastEnumerable<VariableDeclarator> Declarators;

        public readonly FastVariableKind Kind;

        public AstVariableDeclaration(
            FastToken begin,
            FastToken previousToken,
            in VariableDeclarator declarator,
            FastVariableKind kind = FastVariableKind.Var)
            : base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = new Sequence<VariableDeclarator>(1) { declarator };
            this.Kind = kind;
        }


        public AstVariableDeclaration(
            FastToken begin, 
            FastToken previousToken, 
            IFastEnumerable<VariableDeclarator> declarators, 
            FastVariableKind kind = FastVariableKind.Var)
            :base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = declarators;
            this.Kind = kind;
        }

        public override string ToString()
        {
            switch(Kind)
            {
                case FastVariableKind.Let:
                    return $"let {Declarators.Join()}";
                case FastVariableKind.Const:
                    return $"const {Declarators.Join()}";
            }
            return $"var {Declarators.Join()}";
        }
    }
}