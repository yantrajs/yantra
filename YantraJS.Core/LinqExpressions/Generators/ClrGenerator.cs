using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public class StackLabel {
        private readonly int n;

        public StackLabel(int n)
        {
            this.n = n;
        }
        public object Nop() {
            return null;
        }

        public override string ToString()
        {
            return n.ToString();
        }
    }

    public class ClrGenerator
    {

        private Stack<Func<object>> Stack = new Stack<Func<object>>();

        private List<Func<object>> labels = new List<Func<object>>();

        public ClrGenerator()
        {
        }

        private bool stop = false;
        private JSValue result = null;

        public int NewLabel()
        {
            var i = labels.Count;
            var sl = new StackLabel(i);
            labels.Add(sl.Nop);
            return i;
        }

        public Func<object> Yield(Func<object> yield)
        {
            return () => {
                result = yield() as JSValue;
                stop = true;
                return result;
            };
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

        public Func<object> Block(params Func<object>[] list)
        {
            return () =>
            {
                for (int i = list.Length - 1; i >= 0; i--)
                {
                    var step = list[i];
                    Stack.Push(step);
                }
                return null;
            };
        }

        public void Body(Func<object> body)
        {
            Stack.Push(body);
        }

        public Func<object> Assign<T>(Action<object> left, Func<object> right) {
            return () =>
            {
                T result = default;
                Stack.Push(() =>
                {
                    left(result);
                    return result;
                });
                Stack.Push(() =>
                {
                    result = (T)right();
                    return result;
                });
                return result;
            };
        }

        public Func<object> If(Func<bool> test, Func<object> @true, Func<object> @false = null)
        {
            return () => { 
                if (test())
                {
                    Stack.Push(@true);
                    return null;
                }
                if (@false != null)
                    Stack.Push(@false);
                return null;
            };
        }

        public Func<object> Loop(Func<object> body, int breakLabel, int continueLabel)
        {
            var @break = labels[breakLabel];
            var @continue = labels[continueLabel];
            object LoopBody() {
                Stack.Push(LoopBody);
                Stack.Push(@continue);
                return body();
            };
            return () =>
            {
                Stack.Push(@break);
                return LoopBody();
            };
        }

        public Func<object> Goto(int label)
        {
            return () => {
                var l = labels[label];
                while (Stack.Count > 0)
                {
                    if (Stack.Peek() == l)
                        break;
                    Stack.Pop();
                }
                return null;
            };
        }

        public Func<object> Constant(object value)
        {
            return () => {
                return value;
            };
        }

        public Func<object> Coalesc(Func<object> left, Func<object> right)
        {
            return () => {
                object r = null;
                Stack.Push(() => {
                    return r = r ?? right();
                });
                Stack.Push(() => {
                    return r = left();
                });
                return r;
            };
        }

        public Func<object> Call(Func<object>[] parameters, Func<object[],object> call)
        {
            return () => {
                int length = parameters.Length;
                object[] pa = length > 0 ? new object[length] : Array.Empty<object>();
                // we need to push parameter in reverse order
                // so actual evalution will will be in correct order
                for (int i = length -1; i >= 0 ; i--)
                {
                    var pi = i;
                    var fx = parameters[pi];
                    Stack.Push(() => {
                        var r = pa[pi] = fx();
                        return r;
                    });
                }
                Stack.Push(() => {
                    return call(pa);
                });
                return null;
            };
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

    //public class YieldRewriter: ExpressionVisitor
    //{
    //    List<ParameterExpression> lifedVariables = new List<ParameterExpression>();

    //    public static Expression Rewrite(Expression body)
    //    {
    //        return (new YieldRewriter()).Visit(body);
    //    }

    //    public YieldRewriter()
    //    {

    //    }

    //    protected override Expression VisitBlock(BlockExpression node)
    //    {
    //        lifedVariables.AddRange(node.Variables);


    //        Step step = new Step();
    //        List<Step> lambdaList = new List<Step>() { step };

    //        if (lifedVariables.Count> 0)
    //        {
    //            foreach(var lv in lifedVariables)
    //            {
    //                step.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
    //            }
    //        }
    //        List<Expression> afterYield = null;
    //        foreach (var child in node.Expressions)
    //        {
    //            if (YieldFinder.ContainsYield(child))
    //            {
    //                // we need to break here...
    //                afterYield = afterYield ?? new List<Expression>();

    //                step = new Step();
    //                lambdaList.Add(step);

    //                step.Add(Visit(child));

    //                step = new Step();
    //                lambdaList.Add(step);
    //                continue;
    //            }
    //            step.Add(child);
    //        }
    //        if (lifedVariables.Count > 0)
    //        {
    //            foreach (var lv in lifedVariables)
    //            {
    //                step.Add(Expression.Assign(lv, Expression.Constant(null, lv.Type)));
    //            }
    //        }

            
    //    }
    //}

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
