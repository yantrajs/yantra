using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger
{
    public partial class V8Debugger : V8ProtocolObject
    {
        public V8Debugger(V8InspectorProtocol inspectorContext) : base(inspectorContext)
        {
        }

        public object Enable()
        {
            return new {
                debuggerId = inspectorContext.ID
            };
        }

        public object SetPauseOnExceptions(SetPauseOnExceptionsParams p)
        {
            return new { };
        }

        public object SetAsyncCallStackDepth(SetAsyncCallStackDepthParams p)
        {
            return new { };
        }

        public V8ReturnValue GetScriptSource(GetScriptSourceArgs a)
        {
            if(!inspectorContext.Scripts.TryGetValue(a.ScriptId, out var script))
            {
                return new V8ReturnValue { };
            }
            return new V8ReturnValue { 
                ScriptSource = script
            };
        }
    }
}
