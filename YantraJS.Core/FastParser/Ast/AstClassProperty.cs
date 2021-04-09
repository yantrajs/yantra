using System;
namespace YantraJS.Core.FastParser
{
    public readonly struct AstClassProperty
    {
        public readonly bool IsStatic;
        public readonly bool IsPrivate;
        public readonly bool Async;
        public readonly bool Generator;
        public readonly AstPropertyKind Kind;
        public readonly AstExpression Key;
        public readonly AstExpression Init;
        public readonly ArraySpan<VariableDeclarator> Parameters;
        public readonly AstStatement Body;
        public readonly bool Computed;

        public AstExpression Value
        {
            get
            {
                switch(Body.Type)
                {
                    case FastNodeType.ExpressionStatement:
                        return (Body as AstExpressionStatement).Expression;
                }
                throw new NotImplementedException();
            }
        }

        public AstClassProperty(AstPropertyKind propertyKind, 
            bool isPrivate,
            bool isStatic,
            AstExpression propertyName,
            AstExpression init)
        {
            this.IsStatic = isStatic;
            this.IsPrivate = isPrivate;
            this.Kind = propertyKind;
            this.Key = propertyName;
            this.Init = init;
            this.Parameters = null;
            this.Body = null;
            this.Generator = false;
            this.Async = false;
            Computed = Key.Type != FastNodeType.Literal;
        }

        public AstClassProperty(AstPropertyKind propertyKind,
            bool isPrivate,
            bool isStatic,
            bool @async,
            bool generator,
            AstExpression propertyName,
            in ArraySpan<VariableDeclarator> parameters,
            AstStatement body)
        {
            this.IsStatic = isStatic;
            this.IsPrivate = isPrivate;
            this.Async = async;
            this.Generator = generator;
            this.Kind = propertyKind;
            this.Key = propertyName;
            this.Init = null;
            this.Parameters = parameters;
            this.Body = body;
            Computed = Key.Type != FastNodeType.Literal;
        }

    }

}
