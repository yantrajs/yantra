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

        public void AddExpressionRange(IEnumerable<YExpression> exps)
        {
            foreach (var e in exps)
                AddExpression(e);
        }


        public void AddExpression(YExpression exp)
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
                    return;
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
                                this.AddExpression(@return.Update(@return.Target, e));
                                return;
                            }
                            this.AddExpression(e);
                        }
                        return;
                    }
                    break;
            }
            expressions.Add(exp);
        }

        public YBlockExpression Build()
        {
            return new YBlockExpression(variables, expressions.ToArray());
        }

    }
}
