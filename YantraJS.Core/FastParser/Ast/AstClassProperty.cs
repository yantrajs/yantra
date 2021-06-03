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

        public override string ToString()
        {
            if (Kind == AstPropertyKind.Constructor)
                return $"constructor: {Init}";
            if(IsStatic)
            {
                if (Kind == AstPropertyKind.Get)
                    return $"static get {Key} {Init}";
                if (Kind == AstPropertyKind.Set)
                    return $"static set {Key} {Init}";
                if (this.Computed)
                {
                    if (Kind == AstPropertyKind.Data)
                        return $"static [{Key}]: {Init}";
                }
                if (Kind == AstPropertyKind.Data)
                    return $"static {Key}: {Init}";
            }
            if (Kind == AstPropertyKind.Get)
                return $"get {Key} {Init}";
            if (Kind == AstPropertyKind.Set)
                return $"set {Key} {Init}";
            if (Kind == AstPropertyKind.Data)
                return $"{Key}: {Init}";
            if (this.Computed)
            {
                if (Kind == AstPropertyKind.Data)
                    return $"[{Key}]: {Init}";
            }
            return "AstClassProperty";
        }
    }

}
