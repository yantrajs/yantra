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

    /**
     * Each method actually contains steps to manipulate execution stack. 
     * Block/TryCatch etc cannot directly add anything onto stack as code
     * may not even reach there, so in case of nested logic, every logic
     * just contains a logic to put items on stack and to execute it further.
     * 
     * 
     * Generators are very bad for extensive logic, it is recommended to extract 
     * complicated logic into different functions or differnet statements.
     * 
     * Following statements will process slow...
     * 
     */
    public class ClrGenerator
    {

        private Stack<Func<object>> Stack = new Stack<Func<object>>();

        private List<Func<object>> labels = new List<Func<object>>();

        private Stack<(int label, Func<object> @catch)> CatchStack = new Stack<(int, Func<object>)>();

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
                Stack.Push(() =>
                {
                    result = yield() as JSValue;
                    stop = true;
                    return result;
                });
                return null;
            };
        }

        public bool Next(JSValue next, out JSValue value)
        {
            result = result ?? next;
            while(Stack.Count > 0)
            {
                var step = Stack.Pop();
                stop = false;
                try
                {
                    step();
                }
                catch (Exception ex){ 
                    // catch... 
                    // and go upto last try catch..
                    if(CatchStack.Count > 0)
                    {
                        var (id,@catch) = CatchStack.Pop();
                        var catchLabel = labels[id];
                        while (Stack.Count > 0)
                        {
                            var l = Stack.Pop();
                            if(l == catchLabel)
                            {
                                break;
                            }
                        }
                        // push the exception on stack...
                        // if it requires catch...
                        if (@catch != null)
                        {
                            Stack.Push(() => ex);
                            Stack.Push(@catch);
                        }
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
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

        public Func<object> Assign<T>(Action<T> left, Func<object> right) {
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
                bool testResult = false;
                Stack.Push(() => {
                    if (testResult)
                    {
                        Stack.Push(@true);
                    }
                    else if (@false != null)
                        Stack.Push(@false);
                    return testResult;
                });
                Stack.Push(() => {
                    testResult = test();
                    return null;
                });
                return null;
            };
        }

        public Func<object> TryFinally(Func<object> tryBody, Func<object> @finally)
        {
            int finallyLabel = NewLabel();
            return () => {
                CatchStack.Push((finallyLabel, null));
                Stack.Push(@finally);
                Stack.Push(labels[finallyLabel]);
                Stack.Push(() => CatchStack.Pop());
                Stack.Push(tryBody);
                return null;
            };
        }

        public Func<object> TryCatchFinally(
            Func<object> tryBody, 
            Func<object> @catch,
            Func<object> @finally)
        {
            int finallyLabel = NewLabel();
            // catch block is only pushed onto stack
            // if there was any exception caught
            return () => {
                CatchStack.Push((finallyLabel, @catch));
                Stack.Push(@finally);
                Stack.Push(labels[finallyLabel]);
                Stack.Push(() => CatchStack.Pop());
                Stack.Push(tryBody);
                return null;
            };
        }

        public Func<object> TryCatch(
            Func<object> tryBody,
            Func<object> @catch)
        {
            int finallyLabel = NewLabel();
            // catch block is only pushed onto stack
            // if there was any exception caught
            return () => {
                CatchStack.Push((finallyLabel, @catch));
                Stack.Push(labels[finallyLabel]);
                Stack.Push(() => CatchStack.Pop());
                Stack.Push(tryBody);
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
                Stack.Push(() => value);
                return null;
            };
        }

        public Func<object> Unary(Func<object> target, Func<object,object> process)
        {
            return () => {
                object t = null;
                Stack.Push(() => process(t));
                Stack.Push(() => t = target());
                return null;
            };
        }

        public Func<object> Binary(Func<object> left, Func<object> right, Func<object,object,object> process)
        {
            return () => {

                object l = null;
                object r = null;

                Stack.Push(() => {
                    return process(l, r);
                });

                Stack.Push(() => {
                    return r = right();
                });
                Stack.Push(() => {
                    return l = left();
                });

                return null;
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

                // we need to push call
                Stack.Push(() => {
                    return call(pa);
                });

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
                return null;
            };
        }

        

        public Func<object> Switch(
            Func<object> target,
            int breakLabel,
            CatchBody[] @cases )
        {
            return () => {
                object t = null;
                var @break = labels[breakLabel];
                Stack.Push(@break);
                Stack.Push(() => {
                    bool push = false;
                    foreach(var c in cases)
                    {
                        if (push = push || c.Test == t)
                        {
                            Stack.Push(c.Body);
                        }
                    }
                    return null;
                });
                Stack.Push(() => t = target());
                return t;
            };
        }

        public struct CatchBody {
            public object Test;
            public Func<object> Body;
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
