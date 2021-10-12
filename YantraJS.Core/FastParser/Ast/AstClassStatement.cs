#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstClassExpression : AstExpression
    {
        public readonly AstIdentifier? Identifier;
        public readonly AstExpression? Base;
        public readonly IFastEnumerable<AstClassProperty> Members;

        public AstClassExpression(
            FastToken token, 
            FastToken previousToken, 
            AstIdentifier? identifier, 
            AstExpression? @base,
            IFastEnumerable<AstClassProperty> astClassProperties)
            : base(token,  FastNodeType.ClassStatement, previousToken)
        {
            this.Identifier = identifier;
            this.Base = @base;
            this.Members = astClassProperties;
        }

        public override string ToString()
        {
            if(Base != null)
            {
                return $"class {Identifier} extends {Base} {{ {Members.Join("\n\t")} }}";
            }
            if(Identifier == null)
                return $"class {{ {Members.Join("\n\t")} }}";
            return $"class {Identifier} {{ {Members.Join("\n\t")} }}";
        }
    }
}