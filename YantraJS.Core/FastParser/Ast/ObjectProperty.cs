namespace YantraJS.Core.FastParser
{
    public readonly struct ObjectProperty
    {
        public readonly AstExpression Key;
        public readonly AstExpression Value;
        public readonly AstExpression Init;
        public readonly bool Computed;
        public readonly bool Spread;

        public ObjectProperty(
            AstExpression left, 
            AstExpression right,
            AstExpression init,
            bool spread = false,
            bool computed = false)
        {
            this.Key = left;
            this.Value = right;
            this.Init = init;
            this.Spread = spread;                  
            this.Computed = computed;
        }

    }

}
