#nullable enable
using System.Linq;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public class AstSequenceExpression : AstExpression
    {
        public readonly IFastEnumerable<AstExpression> Expressions;

        public AstSequenceExpression(
            FastToken start, 
            FastToken end, 
            IFastEnumerable<AstExpression> expressions) : base(start, FastNodeType.SequenceExpression, end)
        {
            this.Expressions = expressions;
        }

        public AstSequenceExpression(
            IFastEnumerable<AstExpression> expressions) : base(
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
