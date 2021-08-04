using System;
using System.Collections.Generic;

namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class ConsoleApiCalled: V8ProtocolEvent
        {
            public string Type { get; set; }

            public List<V8RemoteObject> Args { get; set; }
            public long Timestamp { get; set; }
            public string ExecutionContextId { get; set; }

            public ConsoleApiCalled(string id, JSContext context, string type, in Arguments a)
            {
                ExecutionContextId = id;

                this.StackTrace = new V8StackTrace(context);
                Args = V8RemoteObject.From(in a);

                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            public V8StackTrace StackTrace { get; set; }

            internal override string EventName => "Runtime.consoleAPICalled";
        }
    }


}
