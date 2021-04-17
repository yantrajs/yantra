#nullable enable
namespace YantraJS.Core.FastParser
{
    internal class AstExportStatement : AstStatement
    {
        public readonly AstExpression? Name;
        public readonly AstExpression Expression;

        public AstExportStatement(FastToken token, AstExpression? name, AstExpression argument)
            : base(token, FastNodeType.ExportStatement, argument.End)
        {
            this.Name = name;
            this.Expression = argument;
        }
    }
}