using Exp = System.Linq.Expressions.Expression;

namespace YantraJS
{
    public class ScopedVariableDeclarator {
        public Esprima.Ast.VariableDeclarator Declarator { get; set; }

        public Exp Init { get; set; }
    }
}
