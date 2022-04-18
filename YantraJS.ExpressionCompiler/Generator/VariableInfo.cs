using System;
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

        private Dictionary<YParameterExpression, Variable> variables 
            = new Dictionary<YParameterExpression, Variable>(ReferenceEqualityComparer.Instance);
        private readonly ILGenerator il;

        public Variable this[YParameterExpression exp]
        {
            get => variables[exp];
        }

        public Variable Create(
            YParameterExpression exp, 
            bool isArgument = false, 
            short index = -1)
        {
            var vb = new Variable(il.DeclareLocal(exp.Type), isArgument, index, exp.Type.IsByRef, exp.Name);
            variables[exp] = vb;
            return vb;
        }

        public Variable Create(
            YParameterExpression exp,
            TempVariables.TempVariableItem tvs)
        {
            var vb = new Variable(tvs.Get(exp.Type), false, -1, exp.Type.IsByRef, exp.Name);
            variables[exp] = vb;
            return vb;
        }


    }
}
