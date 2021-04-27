using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public class LabelInfo
    {
        private readonly ILWriter il;
        private Dictionary<YLabelTarget, ILWriterLabel> labels = new Dictionary<YLabelTarget, ILWriterLabel>();

        public LabelInfo(ILWriter il)
        {
            this.il = il;
        }

        public ILWriterLabel this[YLabelTarget target] => Create(target);



        private ILWriterLabel Create(YLabelTarget target)
        {
            if (labels.TryGetValue(target, out var l))
                return l;
            l = il.DefineLabel();
            labels[target] = l;
            return l;
        }
    }

    public class Variable
    {
        public readonly LocalBuilder LocalBuilder;
        public readonly bool IsArgument;
        public readonly short Index;
        public readonly bool IsReference;
        public Variable(LocalBuilder builder , bool isArg, short index, bool isReference)
        {
            this.LocalBuilder = builder;
            this.IsArgument = isArg;
            this.Index = index;
            this.IsReference = isReference;
        }
    }

    public class VariableInfo {

        public VariableInfo(ILGenerator il)
        {
            this.il = il;
        }

        private Dictionary<YParameterExpression, Variable> variables 
            = new Dictionary<YParameterExpression, Variable>();
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
            var vb = new Variable(il.DeclareLocal(exp.Type), isArgument, index, exp.Type.IsByRef);
            variables[exp] = vb;
            return vb;
        }

    }
}
