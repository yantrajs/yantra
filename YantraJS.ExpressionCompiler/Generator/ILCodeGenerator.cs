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
        private YParameterExpression? This;
        private readonly VariableInfo variables;
        private readonly LabelInfo labels;
        private readonly TempVariables tempVariables;
        private readonly IMethodBuilder methodBuilder;
        private readonly TextWriter? expressionWriter;

        private readonly Dictionary<YParameterExpression,(Type type, int localIndex)> uninitialized
            = new Dictionary<YParameterExpression, (Type, int)>(ReferenceEqualityComparer.Instance);

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

        private void InitializeClosure(YParameterExpression pe)
        {
            if (uninitialized.TryGetValue(pe, out var x))
            {
                uninitialized.Remove(pe);
                il.EmitNew(x.type.GetConstructor());
                il.EmitSaveLocal(x.localIndex);
            }
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
            this.@This = exp.This;
            var closures = closureRepository.Closures;
            if (closures.Any())
            {
                bool isThisLoaded = false;
                // add temporary replacements
                // load this...

                // Outer Closures
                foreach (var kvp in closures.Where(x => x.Value.index != -1))
                {
                    var (local, _, index, isArg) = kvp.Value;

                    variables.Create(local, false, i);


                    if (index != -1)
                    {
                        if (!isThisLoaded)
                        {
                            isThisLoaded = true;
                            il.EmitLoadArg(0);
                            il.Emit(OpCodes.Ldfld, Closures.boxesField);
                        }
                        il.Emit(OpCodes.Dup);
                        il.EmitConstant(index);
                        il.Emit(OpCodes.Ldelem, local.Type);
                        // save it in field...
                        il.EmitSaveLocal(i);
                    }

                    i++;
                }
                if (isThisLoaded)
                {
                    il.Emit(OpCodes.Pop);
                }

                // Self Closures (Needs initialization by parameters)
                foreach (var kvp in closures.Where(x => x.Value.index == -1))
                {
                    var (local, original, index, argIndex) = kvp.Value;

                    variables.Create(local, false, i);

                    if (argIndex != -1)
                    {
                        il.EmitLoadArg(argIndex + 1);
                        if (local.Type != original.Type)
                        {
                            var cnstr = local.Type.GetConstructor(original.Type);
                            il.EmitNew(cnstr);
                        }
                        il.EmitSaveLocal(i);
                    }
                    else
                    {
                        // this is a problem in loop
                        // so box should be created when first assigned
                        // or read...
                        uninitialized[kvp.Key] = (local.Type, i);
                    }

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
