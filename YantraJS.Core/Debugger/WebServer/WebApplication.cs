#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger.WebServer
{
    internal delegate Task Process(HttpListenerContext context, Process? next);

    internal class Step
    {
        public Step? Next { get; set; }
        public Process Process { get; set; }

        public Step(Process process)
        {
            this.Process = process;
        }
    }

    /// <summary>
    /// https://chromedevtools.github.io/devtools-protocol/
    /// </summary>
    internal class WebApplication
    {
        private Step start;

        public WebApplication()
        {
            start = new Step(Error);
        }

        public Task Error(HttpListenerContext context, Process? next)
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }

        public WebApplication Use(Process process)
        {
            var next = start.Process;
            start.Process = (c, n) =>  process(c, next);
            start.Next = new Step(next);
            return this;
        }

        public Task Run(HttpListenerContext context)
        {
            return start.Process(context, null);
        }

    }
}
