namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class CompileScriptParams
        {
            public string Expression { get; set; }

            public string SourceUrl { get; set; }

            public bool PersistScript { get; set; }

            public long ExecutionContextId { get; set; }
        }

        
    }


}
