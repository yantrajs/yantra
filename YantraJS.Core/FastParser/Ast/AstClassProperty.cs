namespace YantraJS.Core.FastParser
{
    public readonly struct AstClassProperty
    {
        public readonly bool IsStatic;
        public readonly bool IsPrivate;
        public readonly bool Async;
        public readonly bool Generator;
        public readonly AstPropertyKind PropertyKind;
        public readonly AstExpression Key;
        public readonly AstExpression Init;
        public readonly VariableDeclarator[] Parameters;
        public readonly AstStatement Body;

        public AstClassProperty(AstPropertyKind propertyKind, 
            bool isPrivate,
            bool isStatic,
            AstExpression propertyName,
            AstExpression init)
        {
            this.IsStatic = isStatic;
            this.IsPrivate = isPrivate;
            this.PropertyKind = propertyKind;
            this.Key = propertyName;
            this.Init = init;
            this.Parameters = null;
            this.Body = null;
            this.Generator = false;
            this.Async = false;
        }

        public AstClassProperty(AstPropertyKind propertyKind,
            bool isPrivate,
            bool isStatic,
            bool @async,
            bool generator,
            AstExpression propertyName,
            VariableDeclarator[] parameters,
            AstStatement body)
        {
            this.IsStatic = isStatic;
            this.IsPrivate = isPrivate;
            this.Async = async;
            this.Generator = generator;
            this.PropertyKind = propertyKind;
            this.Key = propertyName;
            this.Init = null;
            this.Parameters = parameters;
            this.Body = body;
        }

    }

}
