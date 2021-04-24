using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Generator
{

    public partial class ILCodeGenerator
    {
        private readonly ILGenerator il;

        private readonly VariableInfo variables;
        private readonly LabelInfo labels;

        public ILCodeGenerator(ILGenerator il)
        {
            this.il = il;
            this.variables = new VariableInfo(il);
            this.labels = new LabelInfo(il);
        }

        internal void Emit(YLambdaExpression exp)
        {
            short i = 0;
            foreach(var p in exp.Parameters)
            {
                variables.Create(p, true, i++);
            }

            Visit(exp.Body);

        }
    }
}
