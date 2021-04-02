#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstStatement : AstNode
    {
        /// <summary>
        /// 
        /// </summary>
        public string[]? HoistingScope;

        public AstStatement(FastToken start, FastNodeType type, FastToken end) : base(start, type, end, isStatement: true)
        {
        }
    }

}
