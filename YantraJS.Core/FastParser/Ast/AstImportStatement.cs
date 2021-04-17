#nullable enable
namespace YantraJS.Core.FastParser
{
    internal class AstImportStatement : AstStatement
    {
        public readonly AstIdentifier? Default;
        public readonly AstNode? Declaration;
        public readonly AstLiteral Source;

        public AstImportStatement(
            FastToken token, 
            AstIdentifier? defaultIdentifier, 
            AstNode? all,
            AstLiteral source)
            : base(token, FastNodeType.ImportStatement, source.End)
        {
            this.Default = defaultIdentifier;
            this.Declaration = all;
            this.Source = source;
        }
    }
}