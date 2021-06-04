using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Emit;
namespace YantraJS.Tests.Generator
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AsyncTestFolderAttribute : TestFolderAttribute
    {
        public AsyncTestFolderAttribute(string root) : base(root)
        {

        }

        protected override async Task EvaluateAsync(JSContext context, string content, string fullName)
        {
            JSValue r;
            try
            {
                // this needs to run inside AsyncPump 
                // as Promise expects SynchronizationContext to be present
                r = CoreScript.Evaluate(content, fullName, DictionaryCodeCache.Current);
                var w = context.WaitTask;
                if (w != null)
                {
                    try
                    {
                        await w;
                    }
                    catch (TaskCanceledException) { }
                }
                if (r is JSPromise jp)
                {
                    try
                    {
                        await jp.Task;
                    }
                    catch (Exception ex)
                    {
                        throw JSException.From(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw JSException.From(ex);
            }
        }
    }
}
