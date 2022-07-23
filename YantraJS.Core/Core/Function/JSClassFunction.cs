namespace YantraJS.Core
{
    public class JSClassFunction: JSFunction
    {

        public JSClassFunction(
            JSFunctionDelegate @delegate,
            in StringSpan name,
            in StringSpan source,
            int length = 0) : base(@delegate, name, source, length)
        {

        }

        public override JSValue InvokeFunction(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this.name} cannot be invoked directly");
        }
    }
}
