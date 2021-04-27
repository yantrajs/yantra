#nullable enable
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YTryCatchFinallyExpression: YExpression
    {
        public readonly YExpression Try;
        public readonly YExpression? Catch;
        public readonly YParameterExpression? CatchParameter;
        public readonly YExpression? Finally;

        public YTryCatchFinallyExpression(
            YExpression @try, 
            YExpression? @catch,
            YParameterExpression? catchParameter,
            YExpression? @finally)
            : base(YExpressionType.TryCatchFinally, @try.Type)
        {
            this.Try = @try;
            this.Catch = @catch;
            this.CatchParameter = catchParameter;
            this.Finally = @finally;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.WriteLine("try {");
            writer.Indent++;
            Try.Print(writer);
            writer.Indent--;
            if (Catch != null)
            {
                if (CatchParameter != null) {
                    writer.WriteLine($"}} catch({CatchParameter.Name}) {{");
                }
                else
                {
                    writer.WriteLine("} catch {");
                }
                writer.Indent++;
                Catch.Print(writer);
                writer.Indent--;
            }
            if(Finally != null)
            {
                writer.WriteLine("} finally {");
                writer.Indent++;
                Finally.Print(writer);
                writer.Indent--;
            }
            writer.WriteLine("}");
        }
    }
}