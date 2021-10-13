#nullable enable
using System.CodeDom.Compiler;

namespace YantraJS.Expressions
{
    public class YCatchBody
    {
        public readonly YParameterExpression? Parameter;
        public readonly YExpression Body;

        public YCatchBody(YParameterExpression? parameter, YExpression body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }
    }

    public class YTryCatchFinallyExpression: YExpression
    {
        public readonly YExpression Try;
        public new readonly YCatchBody? Catch;
        public readonly YExpression? Finally;

        public YTryCatchFinallyExpression(
            YExpression @try,
            YCatchBody? @catch,
            YExpression? @finally)
            : base(YExpressionType.TryCatchFinally, @try.Type)
        {
            this.Try = @try;
            this.Catch = @catch;
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
                if (Catch.Parameter != null) {
                    writer.WriteLine($"}} catch({Catch.Parameter.Name}) {{");
                }
                else
                {
                    writer.WriteLine("} catch {");
                }
                writer.Indent++;
                Catch.Body.Print(writer);
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