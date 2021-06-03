#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstExportStatement : AstStatement
    {
        public readonly AstNode? Declaration;
        public readonly bool IsDefault;
        public readonly bool ExportAll;
        public readonly AstNode? Source;

        public AstExportStatement(FastToken token, AstNode argument, bool IsDefault = false)
            : base(token, FastNodeType.ExportStatement, argument.End)
        {
            this.Declaration = argument;
            this.IsDefault = IsDefault;
            this.Source = null;
        }

        public AstExportStatement(FastToken token, AstNode? argument, AstNode source)
            : base(token, FastNodeType.ExportStatement, source.End)
        {
            this.Declaration = argument;
            this.IsDefault = false;
            this.ExportAll = argument == null;
            this.Source = source;
        }

    }
}