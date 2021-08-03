using System;

namespace YantraJS.Core.Debugger
{
    public class V8ExceptionDetails
    {
        public int ExceptionId { get; set; }

        public string Text { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public string ScriptId { get; set; }

        public string Url { get; set; }

        public V8StackTrace StackTrace { get; set; }

        public string ExecutionContextId { get; set; }

        public V8RemoteObject Exception { get; set; }

        public V8ExceptionDetails(Exception ex, JSContext context = null)
        {
            if(context != null)
            {
                ExecutionContextId = $"C-{context.ID}";
                StackTrace = new V8StackTrace(context);
                LineNumber = context.Top?.Line ?? 0;
                ColumnNumber = context.Top?.Column ?? 0;
            }
            Text = ex.ToString();

            if(ex is JSException je)
            {
                Exception = new V8RemoteObject(je.Error);
            }
        }
    }


}
