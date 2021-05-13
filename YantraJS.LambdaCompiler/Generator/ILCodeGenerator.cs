#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.Generator
{

    public partial class ILCodeGenerator
    {
        private readonly ILWriter il;

        private readonly VariableInfo variables;
        private readonly LabelInfo labels;
        private readonly TempVariables tempVariables;

        /// <summary>
        /// IL code must load the address
        /// </summary>
        // public bool RequiresAddress => addressScope.RequiresAddress;

        public override string ToString()
        {
            return il.ToString();
        }

        public ILCodeGenerator(ILGenerator il)
        {
            this.il = new ILWriter(il);
            this.variables = new VariableInfo(il);
            this.labels = new LabelInfo(this.il);
            this.tempVariables = new TempVariables(il);
        }

        internal (string il, string exp) Emit(YLambdaExpression exp)
        {
            var sw = new StringWriter();
            System.CodeDom.Compiler.IndentedTextWriter writer = new System.CodeDom.Compiler.IndentedTextWriter(sw, "\t");
            

            var f = new FlattenVisitor();
            var body = exp.Body;

            writer.WriteLine("Original");
            body.Print(writer);

            body = f.Visit(body);

            writer.WriteLine("Flatten");
            body.Print(writer);

            short i = 0;
            if(exp.This != null)
            {
                variables.Create(exp.This, true, i++);
            }
            foreach(var p in exp.Parameters)
            {
                variables.Create(p, true, i++);
            }

            using (tempVariables.Push())
            {
                body = ReWriteTryCatch(body);
                writer.WriteLine("Final");
                body.Print(writer);

                if(body.NodeType == YExpressionType.Call)
                {
                    if(exp.ReturnType.IsAssignableFrom(body.Type) && body.Type != typeof(void))
                    {
                        if (VisitTailCall((body as YCallExpression)!))
                            return (il.ToString(), body.ToString());
                    }
                }

                Visit(body);

                il.Emit(OpCodes.Ret);
            }
            il.Verify();

            return (il.ToString(), sw.ToString());

        }

        private YExpression ReWriteTryCatch(YExpression body)
        {
            switch (body.NodeType)
            {
                case YExpressionType.Block:
                case YExpressionType.Assign:
                    var l = YExpression.Label("ReturnLabel", body.Type);
                    return YExpression.Block(YExpression.Return(l, body), YExpression.Label(l, YExpression.Null));
            }
            if (body.NodeType != YExpressionType.TryCatchFinally)
            {
                return body;
            }

            YTryCatchFinallyExpression exp = (body as YTryCatchFinallyExpression)!;

            var returnLabel = YExpression.Label("ReturnLabel", exp.Try.Type);

            // replace catchbody...
            var @catch = exp.Catch;
            if(@catch != null)
            {
                @catch = YExpression.Catch(@catch.Parameter!, YExpression.Return(returnLabel, @catch.Body));
            }

            exp = new YTryCatchFinallyExpression(
                YExpression.Return(returnLabel, exp.Try), @catch, exp.Finally);

            return YExpression.Block(exp, YExpression.Label(returnLabel));
            
        }
    }
}
