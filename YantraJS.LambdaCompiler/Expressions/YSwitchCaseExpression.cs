namespace YantraJS.Expressions
{
    public class YSwitchCaseExpression
    {
        public readonly YExpression Body;
        public readonly YExpression[] TestValues;

        public YSwitchCaseExpression(YExpression body, YExpression[] testValues)
        {
            this.Body = body;
            this.TestValues = testValues;
        }
    }
}