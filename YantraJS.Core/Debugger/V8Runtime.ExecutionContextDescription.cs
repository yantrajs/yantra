namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class ExecutionContextDescription
        {
            public string Id { get; set; }

            public string Origin { get; set; } = "YantraJS";

            public string Name { get; set; }

            public string UniqueId { get; set; }            
        }
    }


}
