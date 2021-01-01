using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core.LinqExpressions.Generators
{

    /**
     * Switch based generator is too complicated to generate
     * 
     * It is posponed for future.
     * 
     * Instead we have opted for creating Function based VM
     * 
     * Switch based generator is only perfect
     * 1. As they run on same thread
     * 2. Do not use too much of allocation
     * 
     * 
     * Each method actually contains steps to manipulate execution stack. 
     * Block/TryCatch etc cannot directly add anything onto stack as code
     * may not even reach there, so in case of nested logic, every logic
     * just contains a logic to put items on stack and to execute it further.
     * 
     * Instructions cannot be executed within simple block as instructions need to
     * go on Stack in reverse order.
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


        private InstructionStack Stack = new InstructionStack(8);

        private Stack<(uint label, Func<object> @catch)> CatchStack = new Stack<(uint, Func<object>)>();

        public readonly CallStackItem StackItem;

        public ClrGenerator()
        {
        }

        public ClrGenerator(ScriptInfo script, JSVariable[] closures, CallStackItem c)
        {
            this.script = script;
            this.closures = closures;
            this.StackItem = c;
        }

        private bool stop = false;
        private JSValue result = null;

        private static uint _nextLabel = 1;
        private ScriptInfo script;
        private JSVariable[] closures;

        public uint NewLabel()
        {
            return _nextLabel++;
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
            while (Stack.Count > 0)
            {
                var (label, step) = Stack.Pop();
                if (step == null)
                    continue;
                stop = false;
                try
                {
                    var a = step();
                    if (a is Func<object> fx)
                    {
                        Stack.Push(fx);
                    }
                }
                catch (Exception ex)
                {
                    // catch... 
                    // and go upto last try catch..
                    if (CatchStack.Count > 0)
                    {
                        var (id, @catch) = CatchStack.Pop();
                        while (Stack.Count > 0)
                        {
                            var (l, _) = Stack.Pop();
                            if (l == id)
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
                if (stop)
                {
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

        public void Build(Func<object> body)
        {
            Stack.Push(body);
        }

        public Func<object> Assign<T>(Action<T> left, Func<object> right)
        {
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
                return null;
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
            var finallyLabel = NewLabel();
            return () => {
                CatchStack.Push((finallyLabel, null));
                Stack.Push(@finally);
                Stack.Push(finallyLabel);
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
            var finallyLabel = NewLabel();
            // catch block is only pushed onto stack
            // if there was any exception caught
            return () => {
                CatchStack.Push((finallyLabel, @catch));
                Stack.Push(@finally);
                Stack.Push(finallyLabel);
                Stack.Push(() => CatchStack.Pop());
                Stack.Push(tryBody);
                return null;
            };
        }

        public Func<object> TryCatch(
            Func<object> tryBody,
            Func<object> @catch)
        {
            var finallyLabel = NewLabel();
            // catch block is only pushed onto stack
            // if there was any exception caught
            return () => {
                CatchStack.Push((finallyLabel, @catch));
                Stack.Push(finallyLabel);
                Stack.Push(() => CatchStack.Pop());
                Stack.Push(tryBody);
                return null;
            };
        }


        public Func<object> Loop(Func<object> body, uint breakLabel, uint continueLabel)
        {
            var @break = breakLabel;
            var @continue = continueLabel;
            object LoopBody()
            {
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

        public Func<object> Goto(uint label)
        {
            return () => {
                var l = label;
                while (Stack.Count > 0)
                {
                    if (Stack.Peek().label == l)
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

        public Func<object> Unary<T>(Func<object> target, Func<T, object> process)
        {
            return () => {
                T t = default;
                Stack.Push(() => process(t));
                Stack.Push(() => t = (T)target());
                return null;
            };
        }

        public Func<object> Binary<TLeft,TRight>(Func<TLeft> left, Func<TRight> right, Func<TLeft, TRight, TRight> process)
        {
            return () => {

                TLeft l = default;
                TRight r = default;

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

        public Func<object> Call(Func<object>[] parameters, Func<object[], object> call)
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
                for (int i = length - 1; i >= 0; i--)
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
            uint breakLabel,
            CatchBody[] @cases)
        {
            return () => {
                object t = null;
                var @break = breakLabel;
                Stack.Push(@break);
                Stack.Push(() => {
                    bool push = false;
                    foreach (var c in cases)
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

        public struct CatchBody
        {
            public object Test;
            public Func<object> Body;
        }

    }
}
