#nullable enable
namespace YantraJS.Core.FastParser
{
    public class AstIfStatement : AstStatement
    {
        public readonly AstExpression Test;
        public readonly AstStatement True;
        public readonly AstStatement? False;

        public AstIfStatement(FastToken start, FastToken end, AstExpression test, AstStatement @true, AstStatement? @false = null)
            : base(start, FastNodeType.IfStatement, end)
        {
            this.Test = test;
            this.True = @true;
            this.False = @false;
        }

        public override string ToString()
        {
            if(False!=null) {
                return $"if({Test}) {True} else {False}";
            }
            return $"if({Test}) {True}";
        }
    }

}
