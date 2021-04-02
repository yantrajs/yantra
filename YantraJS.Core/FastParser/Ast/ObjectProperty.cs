namespace YantraJS.Core.FastParser
{
    public readonly struct ObjectProperty
    {
        public readonly AstExpression Key;
        public readonly AstExpression Value;
        public readonly bool Spread;

        public ObjectProperty(AstExpression left, AstExpression right, bool spread = false)
        {
            this.Key = left;
            this.Value = right;
            this.Spread = spread;
        }
    }

}
