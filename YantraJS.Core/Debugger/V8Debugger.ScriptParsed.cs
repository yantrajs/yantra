namespace YantraJS.Core.Debugger
{
    public partial class V8Debugger
    {
        public class ScriptParsed: V8ProtocolEvent
        {
            public string ScriptId { get; set; }
            public string Url { get; set; }
            public long ExecutionContextId { get; set; }
            public string Hash { get; set; }
            public bool HasSourceURL { get; set; }
            public int Length { get; set; }
            public string ScriptLanguage { get; set; }

            internal override string EventName => "Debugger.scriptParsed";
        }
    }
}
