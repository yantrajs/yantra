namespace YantraJS.Core.FastParser
{
    internal class AstForStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Test;
        public readonly AstExpression Update;
        public readonly AstStatement Body;

        public AstForStatement(FastToken token, FastToken previousToken, AstNode beginNode, AstExpression test, AstExpression preTest, AstStatement statement)
            :base(token, FastNodeType.ForStatement, previousToken)
        {
            this.Init = beginNode;
            this.Test = test;
            this.Update = preTest;
            this.Body = statement;
        }
    }

    internal class AstForInStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Target;
        public readonly AstStatement Body;

        public AstForInStatement(FastToken token, FastToken previousToken, AstNode beginNode, AstExpression target, AstStatement statement)
            : base(token, FastNodeType.ForInStatement, previousToken)
        {
            this.Init = beginNode;
            this.Target = target;
            this.Body = statement;
        }
    }

    internal class AstForOfStatement : AstStatement
    {
        public readonly AstNode Init;
        public readonly AstExpression Target;
        public readonly AstStatement Body;

        public AstForOfStatement(FastToken token, FastToken previousToken, AstNode beginNode, AstExpression target, AstStatement statement)
            : base(token, FastNodeType.ForOfStatement, previousToken)
        {
            this.Init = beginNode;
            this.Target = target;
            this.Body = statement;
        }
    }
}