using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace YantraJS.Generator
{
    public class VariableInfo {

        public VariableInfo(ILGenerator il)
        {
            this.il = il;
        }

        private Dictionary<ParameterExpression, LocalBuilder> variables 
            = new Dictionary<ParameterExpression, LocalBuilder>();
        private readonly ILGenerator il;

        public LocalBuilder this[ParameterExpression exp]
        {
            get => Create(exp);
        }

        public LocalBuilder Create(ParameterExpression exp)
        {
            if (variables.TryGetValue(exp, out var lb))
                return lb;
            lb = il.DeclareLocal(exp.Type);
            variables[exp] = lb;
            return lb;
        }

    }
}
