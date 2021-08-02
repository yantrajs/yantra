using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger
{
    public class V8Debugger : V8ProtocolObject
    {
        public V8Debugger(V8InspectorProtocol inspectorContext) : base(inspectorContext)
        {
        }

        public class EnableParams
        {

        }

        public object Enable(EnableParams p)
        {
            return new {
                debuggerId = inspectorContext.ID
            };
        }

        public class SetPauseOnExceptionsParams { 
            public string State { get; set; }
        }

        public object SetPauseOnExceptions(SetPauseOnExceptionsParams p)
        {
            return new { };
        }

        public class SetAsyncCallStackDepthParams { 
        }

        public object SetAsyncCallStackDepth(SetAsyncCallStackDepthParams p)
        {
            return new { };
        }

        public class ScriptParsed: V8ProtocolEvent
        {
            internal override string EventName => "Debugger.scriptParsed";
        }
    }
}
