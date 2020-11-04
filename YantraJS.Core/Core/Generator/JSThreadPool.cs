using Esprima.Ast;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace YantraJS.Core.Generator
{
    public static class JSThreadPool
    {

        internal static ConcurrentQueue<JSThread> Pool = new ConcurrentQueue<JSThread>();

        public static void Queue(WaitCallback action, object p)
        {
            // ThreadPool.QueueUserWorkItem(action, p);
            if (Pool.TryDequeue(out var thread))
            {
                thread.Action = action;
                thread.Parameter = p;
                thread.Resume();
                return;
            }
            thread = new JSThread(action) {
                Parameter = p
            };
            thread.Resume();
        }

        public class JSThread
        {
            public WaitCallback Action;
            public object Parameter;
            public Thread thread;

            public AutoResetEvent waiter = new AutoResetEvent(false);

            public JSThread(WaitCallback action)
            {
                Action = action;
                thread = new Thread(Start);
                thread.Start(this);
            }

            public void Resume()
            {
                waiter.Set();
            }

            private static void Start(object p)
            {
                JSThread t = p as JSThread;
                while (true)
                {
                    t.waiter.WaitOne();
                    try
                    {
                        var p1 = t.Parameter;
                        t.Parameter = null;
                        t.Action?.Invoke(p1);
                        t.Action = null;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    if (Pool.Count < 20)
                    {
                        // put it back in Pool...
                        Pool.Enqueue(t);
                        continue;
                    }
                    break;
                }
            }


        }
    }

}
