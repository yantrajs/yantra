using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YantraJS
{
    

    internal class YDispatcher: IDisposable {

        Thread thread;
        BlockingCollection<(TaskCompletionSource<object> taskSource, object input, Func<object, object> func)> tasks
            = new BlockingCollection<(TaskCompletionSource<object> taskSource, object input, Func<object, object> func)>();

        public YDispatcher()
        {
            thread = new Thread(Run, 64*1024*1024);
            thread.Start();
        }

        public static object Queue(object input, Func<object, object> func)
        {
            TaskCompletionSource<object> task = new TaskCompletionSource<object>();
            
            if(!queue.TryDequeue(out var d))
            {
                d = new YDispatcher();
            }

            d.tasks.Add((task, input, func));
            task.Task.Wait();

            if(queue.Count < 10)
            {
                queue.Enqueue(d);
            } else
            {
                d.Dispose();
            }

            return task.Task.Result;
        }

        private static ConcurrentQueue<YDispatcher> queue = new ConcurrentQueue<YDispatcher>();

        public void Dispose()
        {
            tasks.CompleteAdding();
        }

        private void Run(object b)
        {
            foreach(var (task, input, func) in tasks.GetConsumingEnumerable())
            {
                task.SetResult(func(input));
            }
        }
    }


    public abstract class StackGuard<T,TIn> {

        private const int MaxStackSize = 1024;


        private static int count = MaxStackSize;

        public T Visit(TIn input)
        {

            if (count == MaxStackSize)
            {
                T output = default;
                count = 0;
                output = (T)YDispatcher.Queue(input, (i) => this.VisitIn((TIn)i));
                count = MaxStackSize;
                return output;
            }

            count++;
            var r = VisitIn(input);
            count--;
            return r;
        }

        public abstract T VisitIn(TIn input);

    }
}
