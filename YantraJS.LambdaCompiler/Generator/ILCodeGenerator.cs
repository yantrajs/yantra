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

        public ILCodeGenerator(ILGenerator il)
        {
            this.il = il;
            this.variables = new VariableInfo(il);
        }

        internal void Emit(YLambdaExpression exp)
        {

            foreach(var p in exp.Parameters)
            {
                variables.Create(p);
            }

            Visit(exp.Body);

        }
    }
}
