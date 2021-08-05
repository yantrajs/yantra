using System;
using System.Collections.Generic;
using System.Linq;
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

        public YBlockBuilder AddVariable(YParameterExpression pe, YExpression init)
        {
            variables.Add(pe);
            // break init if it is block..
            if(init.NodeType == YExpressionType.Block)
            {
                var block = init as YBlockExpression;
                this.variables.AddRange(block.FlattenVariables);
                foreach(var (e, last) in block.FlattenExpressions)
                {
                    if (last)
                    {
                        return AddExpression(YExpression.Assign(pe, e));
                    }
                    AddExpression(e);
                }
            }
            return AddExpression(YExpression.Assign(pe, init));
        }

        public YExpression ConvertToVariable(YExpression exp)
        {
            if (exp.NodeType == YExpressionType.Parameter)
                return exp;
            var e = YExpression.Parameter(exp.Type);
            AddVariable(e, exp);
            return e;
        }

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

        public YExpression Build()
        {
            if (variables.Count == 0 && expressions.Count == 1)
                return expressions.First();
            return new YBlockExpression(variables, expressions.ToArray());
        }

    }
}
