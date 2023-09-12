using System.Collections.Generic;

namespace YantraJS.Core.FastParser
{

    public enum FastVariableKind
    {
        None,
        Let,
        Const,
        Var,
    }

    public class AstVariableDeclaration: AstStatement
    {
        public readonly IFastEnumerable<VariableDeclarator> Declarators;

        public readonly FastVariableKind Kind;

        /// <summary>
        /// This declaration must be disposed at end of the containing scope.
        /// </summary>
        public readonly bool Using;

        /// <summary>
        /// This declaration must be disposed asynchronously at end of the containing scope.
        /// </summary>
        public readonly bool AwaitUsing;

        public AstVariableDeclaration(
            FastToken begin,
            FastToken previousToken,
            in VariableDeclarator declarator,
            FastVariableKind kind = FastVariableKind.Var, 
            bool @using = false,
            bool @await = false)
            : base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = new Sequence<VariableDeclarator>(1) { declarator };
            this.Kind = kind;
            this.Using = @using;
            this.AwaitUsing = @await;
        }


        public AstVariableDeclaration(
            FastToken begin, 
            FastToken previousToken, 
            IFastEnumerable<VariableDeclarator> declarators, 
            FastVariableKind kind = FastVariableKind.Var,
            bool @using = false,
            bool @await = false)
            :base(begin, FastNodeType.VariableDeclaration, previousToken)
        {
            this.Declarators = declarators;
            this.Kind = kind;
            this.Using = @using;
            this.AwaitUsing = @await;
        }

        public override string ToString()
        {
            if (this.Using)
            {
                if (this.AwaitUsing)
                {
                    return $"await using {Declarators.Join()}";
                }
                return $"using {Declarators.Join()}";
            }
            switch (Kind)
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