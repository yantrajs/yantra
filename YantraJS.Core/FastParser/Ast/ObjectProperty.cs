namespace YantraJS.Core.FastParser
{
    public readonly struct ObjectProperty
    {
        public readonly AstExpression Left;
        public readonly AstExpression Right;

        public ObjectProperty(AstExpression left, AstExpression right)
        {
            this.Left = left;
            this.Right = right;
        }
    }

}
