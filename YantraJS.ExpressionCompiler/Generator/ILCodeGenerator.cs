#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static bool GenerateLogs = false;
        private ClosureRepository closureRepository;
        private readonly VariableInfo variables;
        private readonly LabelInfo labels;
        private readonly TempVariables tempVariables;
        private readonly IMethodBuilder methodBuilder;
        private readonly TextWriter? expressionWriter;

        public Sequence<ILDebugInfo> SequencePoints { get; }
            = new Sequence<ILDebugInfo>();

        /// <summary>
        /// IL code must load the address
        /// </summary>
        // public bool RequiresAddress => addressScope.RequiresAddress;

        public override string ToString()
        {
            return il.ToString();
        }

        public ILCodeGenerator(
            ILGenerator il,
            IMethodBuilder methodBuilder,
            TextWriter? writer = null, TextWriter? expressionWriter = null)
        {
            if(!GenerateLogs)
            {
                writer = null;
                expressionWriter = null;
            }
            this.il = new ILWriter(il, writer);
            this.variables = new VariableInfo(il);
            this.labels = new LabelInfo(this.il);
            this.tempVariables = new TempVariables(this.il);
            this.methodBuilder = methodBuilder;
            this.expressionWriter = expressionWriter;
        }

        internal void Emit(YLambdaExpression exp)
        {
            

            // var f = new FlattenVisitor();
            var body = exp.Body;

            //writer.WriteLine("Original");
            //body.Print(writer);

            //body = f.Visit(body);

            //writer.WriteLine("Flatten");
            

            short i = 0;
            if(exp.This != null)
            {
                variables.Create(exp.This, true, i++);
            }
            foreach(var p in exp.Parameters)
            {
                variables.Create(p, true, i++);
            }

            this.closureRepository = exp.GetClosureRepository();
            var closures = closureRepository.Closures;
            if (closures.Any())
            {

                // add temporary replacements
                // load this...
                il.EmitLoadArg(0);

                foreach (var kvp in closures.Where(x => x.Value.index != -1))
                {
                    var (local, _, index) = kvp.Value;

                    variables.Create(local, false, i);


                    if (index != -1)
                    {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Ldfld, Closures.boxesField);
                        il.Emit(OpCodes.Ldelem, index);
                        // save it in field...
                        il.EmitSaveLocal(i);
                    }

                    i++;
                }
                il.Emit(OpCodes.Pop);

                foreach (var kvp in closures.Where(x => x.Value.index == -1))
                {
                    var (local, _, index) = kvp.Value;

                    variables.Create(local, false, i);

                    il.EmitNew(BoxHelper.For(local.Type).Constructor);
                    il.EmitSaveLocal(i);

                    i++;
                }

            }

            using (tempVariables.Push())
            {
                body = ReWriteTryCatch(body);
                if(expressionWriter != null)
                {
                    var writer = new System.CodeDom.Compiler.IndentedTextWriter(expressionWriter, "\t");
                    body.Print(writer);
                }

                if(body.NodeType == YExpressionType.Call)
                {
                    if(exp.ReturnType.IsAssignableFrom(body.Type) && body.Type != typeof(void))
                    {
                        if (VisitTailCall((body as YCallExpression)!))
                            return;
                    }
                }

                Visit(body);

                il.Emit(OpCodes.Ret);
            }
            il.Verify();

            return;

        }

        private YExpression ReWriteTryCatch(YExpression body)
        {
            //switch (body.NodeType)
            //{
            //    case YExpressionType.Block:
            //    case YExpressionType.Assign:
            //        var l = YExpression.Label("ReturnLabel", body.Type);
            //        return YExpression.Block(YExpression.Return(l, body), YExpression.Label(l, YExpression.Null));
            //}
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
