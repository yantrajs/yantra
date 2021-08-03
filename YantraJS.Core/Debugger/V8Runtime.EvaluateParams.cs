namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class EvaluateParams
        {
            public string Expression { get; set; }

            public string ObjectGroup { get; set; }

            public bool IncludeCommandLineAPI { get; set; }

            public bool Silent { get; set; }

            public string ContextId { get; set; }

            public bool ReturnByValue { get; set; }

            public bool UserGesture { get; set; }

            public bool AwaitPromise { get; set; }
        }

        
    }


}
