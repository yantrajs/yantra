using System;
namespace YantraJS.Core.FastParser
{
    public class AstClassProperty: AstNode
    {
        public readonly bool IsStatic;
        public readonly bool IsPrivate;
        public readonly AstPropertyKind Kind;
        public readonly AstExpression Key;
        public readonly AstExpression Init;
        public readonly bool Computed;

        public AstClassProperty(
            FastToken begin,
            FastToken last,
            AstPropertyKind propertyKind, 
            bool isPrivate,
            bool isStatic,
            AstExpression propertyName,
            bool computed,
            AstExpression init)
            : base(begin,  FastNodeType.ClassProperty, last)
        {
            this.IsStatic = isStatic;
            this.IsPrivate = isPrivate;
            this.Kind = propertyKind;
            this.Key = propertyName;
            this.Init = init;
            Computed = computed;
        }

        public AstClassProperty Reduce(AstExpression key, AstExpression init)
        {
            return new AstClassProperty(Start,End, Kind, IsPrivate, IsStatic, key, Computed, init);
        }
    }

}
