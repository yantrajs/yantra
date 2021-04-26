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
        private readonly TempVariables tempVariables;
        private readonly AddressScope addressScope;
        private YLambdaExpression root;

        /// <summary>
        /// IL code must load the address
        /// </summary>
        public bool RequiresAddress => addressScope.RequiresAddress;

        public ILCodeGenerator(ILGenerator il)
        {
            this.il = il;
            this.variables = new VariableInfo(il);
            this.labels = new LabelInfo(il);
            this.tempVariables = new TempVariables(il);
            this.addressScope = new AddressScope();
        }

        internal void Emit(YLambdaExpression exp)
        {
            this.root = exp;
            short i = 0;
            foreach(var p in exp.Parameters)
            {
                variables.Create(p, true, i++);
            }

            using (tempVariables.Push())
            {
                using (addressScope.Push(false))
                {
                    Visit(exp.Body);
                }
            }

        }
    }
}
