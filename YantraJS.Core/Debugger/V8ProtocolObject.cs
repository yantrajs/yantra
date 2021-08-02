using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YantraJS.Core.Debugger
{

    public class V8ProtocolObject
    {
        protected readonly V8InspectorProtocol inspectorContext;

        public V8ProtocolObject(V8InspectorProtocol inspectorContext)
        {
            this.inspectorContext = inspectorContext;
        }
    }
}
