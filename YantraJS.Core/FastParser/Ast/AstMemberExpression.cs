#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstMemberExpression : AstExpression
    {
        public readonly AstExpression Object;
        public readonly AstExpression Property;
        public readonly bool Computed;
        public readonly bool Coalesce;

        public AstMemberExpression(
            AstExpression target, 
            AstExpression node, 
            bool computed = false,
            bool coalesce = false):
            base(target.End, FastNodeType.MemberExpression, node.End)
        {
            this.Object = target;
            this.Property = node;
            this.Computed = computed;
            this.Coalesce = coalesce;
        }

        public override string ToString()
        {
            if(Computed)
                return $"{Object}[{Property}]";
            if(Coalesce)
                return $"{Object}?.{Property}";
            return $"{Object}.{Property}";
        }
    }
}