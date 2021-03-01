using System.Collections.Generic;
using YantraJS.Core;
using Exp = System.Linq.Expressions.Expression;

namespace YantraJS
{
    public class ScopedVariableDeclaration
    {
        public bool NewScope { get; private set; }

        public bool Copy { get; set; }
        public SparseList<ScopedVariableDeclarator> Declarators { get; }
            = new SparseList<ScopedVariableDeclarator>();

        public ScopedVariableDeclaration(IEnumerable<ScopedVariableDeclarator> list)
        {
            NewScope = true;
            Declarators.AddRange(list);
        }

        public ScopedVariableDeclaration(
            Esprima.Ast.VariableDeclaration declaration,
            Exp init = null)
        {
            NewScope = 
                declaration.Kind == Esprima.Ast.VariableDeclarationKind.Const 
                || declaration.Kind == Esprima.Ast.VariableDeclarationKind.Let;
            foreach(var d in declaration.Declarations)
            {
                var sd = new ScopedVariableDeclarator
                {
                    Declarator = d
                };
                Declarators.Add(sd);
                if (init != null)
                {
                    sd.Init = init;
                }
            }
        }
    }
}
