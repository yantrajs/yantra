namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class ExecutionContextCreated: V8ProtocolEvent
        {
            internal override string EventName => "Runtime.executionContextCreated";

            public ExecutionContextDescription Context { get; set; }
        }
    }


}
