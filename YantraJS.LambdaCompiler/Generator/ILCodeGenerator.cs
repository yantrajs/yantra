#nullable enable
using System;
using System.Collections.Generic;
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
        private readonly AddressScope addressScope;

        /// <summary>
        /// IL code must load the address
        /// </summary>
        public bool RequiresAddress => addressScope.RequiresAddress;

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
            this.addressScope = new AddressScope();
        }

        internal void Emit(YLambdaExpression exp)
        {
            short i = 0;
            foreach(var p in exp.Parameters)
            {
                variables.Create(p, true, i++);
            }

            using (tempVariables.Push())
            {
                using (addressScope.Push(false))
                {
                    // if it is try/catch... reset try/finally 

                    Visit(ReWriteTryCatch(exp.Body));

                    il.Emit(OpCodes.Ret);
                }
            }

            il.Verify();

        }

        private YExpression ReWriteTryCatch(YExpression body)
        {
            if (body.NodeType != YExpressionType.TryCatchFinally)
                return body;

            YTryCatchFinallyExpression exp = (body as YTryCatchFinallyExpression)!;

            if (exp.Try.NodeType != YExpressionType.Block)
                return body;

            YBlockExpression block = (exp.Try as YBlockExpression)!;


            var exps = block.Expressions;
            var last = exps[exps.Length - 1];

            if (last.NodeType != YExpressionType.Label)
                return body;

            var label = (last as YLabelExpression)!;
            if (label.Default == null)
                return body;

            var outerTemp = YExpression.Parameter(label.Default.Type);

            var newLabel = YExpression.Label("BodyRet", outerTemp.Type);

            exps[exps.Length - 1] = YExpression.GoTo(newLabel, label.Default);

            return YExpression.Block(new YParameterExpression[] { outerTemp }, exps);
            
        }
    }
}
