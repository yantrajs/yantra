using YantraJS.Core.Clr;

namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime
    {
        public class CallArgument
        {
            public object Value { get; set; }

            public string UnserializableValue { get; set; }

            public string ObjectId { get; set; }

            public JSValue ToJSValue()
            {
                if(ObjectId != null)
                {
                    return V8RemoteObject.From(ObjectId);
                }
                return ClrProxy.Marshal(Value);
            }
        }
    }


}
