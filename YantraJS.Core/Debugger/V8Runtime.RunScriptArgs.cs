namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class RunScriptArgs
        {
            public string ScriptId { get; set; }

            public long ExecutionContextId { get; set; }

            public string ObjectGroup { get; set; }

            public bool Silent { get; set; }

            public bool IncludeCommandLineAPI { get; set; }

            public bool ReturnByValue { get; set; }

            public bool GeneratePreview { get; set; }

            public bool AwaitPromise { get; set; }
        }

        
    }


}
