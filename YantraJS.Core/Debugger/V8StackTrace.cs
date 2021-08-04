using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Debugger
{
    public class V8StackTrace
    {
        public V8StackTrace(JSContext context)
        {
            List<V8CallFrame> cflist = new List<V8CallFrame>();
            var top = context.Top;
            while(top != null)
            {
                cflist.Add(new V8CallFrame { 
                    FunctionName = top.Function.Value,
                    ScriptId = top.FileName,
                    Url = top.FileName,
                    LineNumber = top.Line,
                    ColumnNumber = top.Column
                });
                top = top.Parent;
            }
            CallFrames = cflist;
        }

        public List<V8CallFrame> CallFrames { get; set; }

    }

    public class V8CallFrame
    {
        public int ColumnNumber { get; set; }
        public int LineNumber { get; set; }
        public string Url { get; set; }
        public string ScriptId { get; set; }
        public string FunctionName { get; set; }
    }
}
