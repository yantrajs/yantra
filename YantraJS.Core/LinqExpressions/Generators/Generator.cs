using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public class Generator
    {

        private Stack<Action> Stack = new Stack<Action>();

        public Arguments Arguments { get; }

        public Generator(in Arguments a)
        {
            this.Arguments = a;
        }

        private bool stop = false;
        private JSValue result = null;

        public void Yield(Func<JSValue> yield)
        {
            Stack.Push(() => {
                result = yield();
                stop = true;
            });
        }

        public bool Next(JSValue next, out JSValue value)
        {
            result = result ?? next;
            while(Stack.Count > 0)
            {
                var step = Stack.Pop();
                stop = false;
                step();
                if (stop) {
                    value = result;
                    return true;
                }
            }
            value = null;
            return false;
        }

        public void Block(Action[] list)
        {
            for (int i = list.Length-1; i > 0; i--)
            {
                Stack.Push(list[i]);
            }
        }

        public void Assign(Action<JSValue> left, Func<JSValue> right) {
            JSValue result = null;
            Stack.Push(() => {
                left(result);
            });
            Stack.Push(() => {
                result = right();
            });
        }

        public void If(Func<bool> test, Action @true, Action @false = null)
        {
            Stack.Push(() => { 
                if (test())
                {
                    Stack.Push(@true);
                }
                if (@false != null)
                    Stack.Push(@false);
            });
        }

        public void Break(Action body)
        {
            while(Stack.Count > 0)
            {
                if(Stack.Peek() == body)
                {
                    Stack.Pop();
                    break;
                }
                Stack.Pop();
            }
        }

        public void Continue(Action body)
        {
            while (Stack.Count > 0)
            {
                if (Stack.Peek() == body)
                {
                    break;
                }
                Stack.Pop();
            }
        }

        public void Loop(Action body)
        {
            Stack.Push(() => {
                Loop(body);
                body();
            });
        }

    }

    public class YieldExpression: Expression
    {
        public YieldExpression New(Expression argument)
        {
            return new YieldExpression(argument);
        }

        private YieldExpression(Expression argument)
        {
            Argument = argument;
        }

        public Expression Argument { get; }

        public override Type Type => Argument.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;
    }

    public class Step {

        public List<Expression> Steps = new List<Expression>();

        public void Add(Expression exp) => Steps.Add(exp);

    }

    public class YieldRewriter: ExpressionVisitor
    {
        List<ParameterExpression> lifedVariables = new List<ParameterExpression>();

        public static Expression Rewrite(Expression body)
        {
            return (new YieldRewriter()).Visit(body);
        }

        public YieldRewriter()
        {

        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            lifedVariables.AddRange(node.Variables);


            Step step = new Step();
            List<Step> lambdaList = new List<Step>() { step };

            if (lifedVariables.Count> 0)
            {
                foreach(var lv in lifedVariables)
                {
                    step.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
                }
            }
            List<Expression> afterYield = null;
            foreach (var child in node.Expressions)
            {
                if (YieldFinder.ContainsYield(child))
                {
                    // we need to break here...
                    afterYield = afterYield ?? new List<Expression>();

                    step = new Step();
                    lambdaList.Add(step);

                    step.Add(Visit(child));

                    step = new Step();
                    lambdaList.Add(step);
                    continue;
                }
                step.Add(child);
            }
            if (lifedVariables.Count > 0)
            {
                foreach (var lv in lifedVariables)
                {
                    step.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
                }
            }

            
        }
    }

    public class YieldFinder: ExpressionVisitor {

        private bool found = false;

        public static bool ContainsYield(Expression node)
        {
            var finder = new YieldFinder();
            finder.Visit(node);
            return finder.found;
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node is YieldExpression)
            {
                found = found || true;
            }
            return node;
        }

        public override Expression Visit(Expression node)
        {
            if (node is LambdaExpression)
                return node;
            return base.Visit(node);
        }
    }
}
