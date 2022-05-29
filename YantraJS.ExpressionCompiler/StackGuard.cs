using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS
{
    public class YDispatcher
    {
        public static object Queue(object input, Func<object,object> func)
        {
            object r = null;
            // var thread = new Thread((i) => r = func(i), 10 * 1024 * 1024);
            // thread.Start(input);
            // thread.Join();
            TaskCompletionSource<object> result = new TaskCompletionSource<object>();
            ThreadPool.QueueUserWorkItem((input) => {
                result.SetResult(func(input));
            }, input);
            return result.Task.GetAwaiter().GetResult();
            // return r;
        }
    }

    //public class YDispatcher: IDisposable {

    //    Thread thread;
    //    BlockingCollection<(TaskCompletionSource<object> taskSource, object input, Func<object, object> func)> tasks
    //        = new BlockingCollection<(TaskCompletionSource<object> taskSource, object input, Func<object, object> func)>();

    //    private YDispatcher()
    //    {
    //        thread = new Thread(Run, 64*1024*1024);
    //        thread.Start();
    //    }

    //    public static object Queue(object input, Func<object, object> func)
    //    {
    //        TaskCompletionSource<object> task = new TaskCompletionSource<object>();
            
    //        if(!queue.TryDequeue(out var d))
    //        {
    //            d = new YDispatcher();
    //        }

    //        d.tasks.Add((task, input, func));
    //        try
    //        {
    //            task.Task.Wait();
    //            return task.Task.Result;
    //        } finally
    //        {
    //            if (queue.Count < 4)
    //            {
    //                queue.Enqueue(d);
    //            }
    //            else
    //            {
    //                d.Dispose();
    //            }
    //        }
    //    }

    //    private static ConcurrentQueue<YDispatcher> queue = new ConcurrentQueue<YDispatcher>();

    //    public void Dispose()
    //    {
    //        tasks.CompleteAdding();
    //    }

    //    private static CancellationTokenSource cancellationToken = new CancellationTokenSource();

    //    public static void Shutdown()
    //    {
    //        cancellationToken.Cancel();
    //    }

    //    private void Run(object b)
    //    {
    //        foreach(var (task, input, func) in tasks.GetConsumingEnumerable(cancellationToken.Token))
    //        {
    //            try
    //            {
    //                task.SetResult(func(input));
    //            } catch (Exception ex)
    //            {
    //                task.TrySetException(ex);
    //            }
    //        }
    //    }
    //}


    public abstract class StackGuard<T,TIn> {

        private const int MaxStackSize = 1024;


        // private int count = 200;

        private int start = 0;

        public unsafe T Visit(TIn input)
        {

            int self;
            int address = (int)(&self);
            if (start == 0)
            {
                start = address;
            }
            else
            {
                int diff = address - start;
                if (diff > MaxStackSize)
                {
                    var prev = this.start;
                    this.start = 0;
                    var output = (T)YDispatcher.Queue(input, (i) => this.VisitIn((TIn)i));
                    this.start = prev;
                    return output;
                }
            }

            return VisitIn(input);

            //// checking this is very slow..instead starting new thread and stopping it is faster...
            //if (count == 0)
            //{
            //    try
            //    {
            //        RuntimeHelpers.EnsureSufficientExecutionStack();
            //        count = 100;
            //    }
            //    catch (InsufficientExecutionStackException)
            //    {

            //    }
            //}

            //if (count == 0)
            //{
            //    T output = default;
            //    count = MaxStackSize;
            //    output = (T)YDispatcher.Queue(input, (i) => this.VisitIn((TIn)i));
            //    count = 0;
            //    return output;
            //}

            //count--;
            //var r = VisitIn(input);
            //count++;
            //return r;
        }

        public abstract T VisitIn(TIn input);

    }
}
