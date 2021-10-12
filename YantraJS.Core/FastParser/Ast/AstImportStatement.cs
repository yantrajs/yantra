#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstImportStatement : AstStatement
    {
        public readonly AstIdentifier? Default;
        public readonly AstIdentifier? All;
        public readonly IFastEnumerable<(StringSpan name, StringSpan asName)>? Members;
        public readonly AstLiteral Source;

        public AstImportStatement(
            FastToken token,
            AstIdentifier? defaultIdentifier,
            AstIdentifier? all,
            IFastEnumerable<(StringSpan, StringSpan)>? members,
            AstLiteral source)
            : base(token, FastNodeType.ImportStatement, source.End)
        {
            this.Default = defaultIdentifier;
            this.All = all;
            this.Members = members;
            this.Source = source;
        }

    }
}