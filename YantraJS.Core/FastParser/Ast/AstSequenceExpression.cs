#nullable enable
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class AstSequenceExpression : AstExpression
    {
        public readonly ArraySpan<AstExpression> Expressions;

        public AstSequenceExpression(
            FastToken start, 
            FastToken end, 
            in ArraySpan<AstExpression> expressions) : base(start, FastNodeType.SequenceExpression, end)
        {
            this.Expressions = expressions;
        }

        public AstSequenceExpression(
            in ArraySpan<AstExpression> expressions) : base(
                expressions.FirstOrDefault().Start, 
                FastNodeType.SequenceExpression, 
                expressions.LastOrDefault().End)
        {
            this.Expressions = expressions;
        }

        public override string ToString()
        {
            return Expressions.Join();
        }
    }

}
