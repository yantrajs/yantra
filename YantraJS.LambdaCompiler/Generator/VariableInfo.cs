using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public class VariableInfo {

        public VariableInfo(ILGenerator il)
        {
            this.il = il;
        }

        private Dictionary<YParameterExpression, LocalBuilder> variables 
            = new Dictionary<YParameterExpression, LocalBuilder>();
        private readonly ILGenerator il;

        public LocalBuilder this[YParameterExpression exp]
        {
            get => Create(exp);
        }

        public LocalBuilder Create(YParameterExpression exp)
        {
            if (variables.TryGetValue(exp, out var lb))
                return lb;
            lb = il.DeclareLocal(exp.Type);
            variables[exp] = lb;
            return lb;
        }

    }
}
