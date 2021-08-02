using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Expressions
{
    public class YBlockBuilder
    {

        private List<YExpression> expressions = new List<YExpression>();
        private List<YParameterExpression> variables = new List<YParameterExpression>();

        public YBlockBuilder()
        {

        }

        public void AddVariable(YParameterExpression pe) => variables.Add(pe);

        public YBlockBuilder AddExpressionRange(IEnumerable<YExpression> exps)
        {
            foreach (var e in exps)
                AddExpression(e);
            return this;
        }


        public YBlockBuilder AddExpression(YExpression exp)
        {
            switch(exp.NodeType)
            {
                case YExpressionType.Block:
                    var block = (exp as YBlockExpression)!;
                    variables.AddRange(block.Variables);

                    foreach (var e in block.Expressions)
                    {
                        this.AddExpression(e);
                    }
                    return this;
                case YExpressionType.Return:
                    var @return = (exp as YReturnExpression)!;
                    if(@return.Default?.NodeType == YExpressionType.Block)
                    {
                        block = (@return.Default as YBlockExpression)!;
                        var en = block.Enumerate();
                        while(en.MoveNext(out var e, out var isLast))
                        {
                            if (isLast)
                            {
                                return this.AddExpression(@return.Update(@return.Target, e));
                            }
                            this.AddExpression(e);
                        }
                        return this;
                    }
                    break;
            }
            expressions.Add(exp);
            return this;
        }

        public YBlockExpression Build()
        {
            return new YBlockExpression(variables, expressions.ToArray());
        }

    }
}
