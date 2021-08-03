using System.Collections.Generic;

namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class CallFunctionOnParams
        {
            public string FunctionDeclaration { get; set; }

            public List<CallArgument> Arguments { get; set; }

            public string ObjectId { get; set; }

            public bool Silent { get; set; }

            public bool ReturnByValue { get; set; }

            public bool UserGesture { get; set; }

            public bool AwaitPromise { get; set; }

            public long ExecutionContextId { get; set; }

            public string ObjectGroup { get; set; }
        }

        
    }


}
