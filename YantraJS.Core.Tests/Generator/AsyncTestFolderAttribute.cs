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

        protected override void Evaluate(JSContext context, string content, string fullName)
        {
            try
            {
                

                AsyncPump.Run(async () =>
                {
                    // this needs to run inside AsyncPump 
                    // as Promise expects SynchronizationContext to be present
                    var r = CoreScript.Evaluate(content, fullName, DictionaryCodeCache.Current);
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
                    if (context.WaitTask != null)
                    {
                        try
                        {
                            await context.WaitTask;
                        }
                        catch (TaskCanceledException) { }
                    }
                });
            }
            catch (Exception ex)
            {
                throw JSException.From(ex);
            }
        }
    }
}
