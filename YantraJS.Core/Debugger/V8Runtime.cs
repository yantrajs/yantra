using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Debugger
{

    public class V8Runtime : V8ProtocolObject
    {
        public V8Runtime(V8InspectorProtocol inspectorContext) : base(inspectorContext)
        {
        }

        public object Enable(JSObject none)
        {
            foreach (var entry in inspectorContext.Contexts)
            {
                var cid = entry.Key;
                inspectorContext.Send(new V8Runtime.ExecutionContextCreated
                {
                    Context = new V8Runtime.ExecutionContextDescription
                    {
                        Id = cid,
                        Name = cid,
                        UniqueId = cid
                    }
                });
            }
            return new { };
        }

        public class ConsoleApiCalled: V8ProtocolEvent
        {
            internal override string EventName => "Runtime.consoleApiCalled";
        }

        public class ExecutionContextCreated: V8ProtocolEvent
        {
            internal override string EventName => "Runtime.executionContextCreated";

            public ExecutionContextDescription Context { get; set; }
        }

        public class ExecutionContextDescription
        {
            public string Id { get; set; }

            public string Origin { get; set; } = "YantraJS";

            public string Name { get; set; }

            public string UniqueId { get; set; }            
        }

        public object GetIsolateId() {
            return new { 
                id = inspectorContext.ID
            };
        }
    }


}
