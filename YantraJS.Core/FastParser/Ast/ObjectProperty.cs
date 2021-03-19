namespace YantraJS.Core.FastParser
{
    public readonly struct ObjectProperty
    {
        public readonly AstExpression Left;
        public readonly AstExpression Right;
        public readonly bool Spread;

        public ObjectProperty(AstExpression left, AstExpression right, bool spread = false)
        {
            this.Left = left;
            this.Right = right;
            this.Spread = spread;
        }
    }

}
