namespace YantraJS.Core
{
    public class JSClassFunction: JSFunction
    {
        private readonly JSFunctionDelegate CreateFunction;

        public JSClassFunction(
            JSFunctionDelegate @delegate,
            in StringSpan name,
            in StringSpan source,
            int length = 0) : base(@delegate, name, source, length)
        {
            this.CreateFunction = @delegate;
            this.InvokeFunction = InvokeClass;
        }

        public JSValue InvokeClass(in Arguments a)
        {
            throw JSContext.Current.NewTypeError($"{this.name} cannot be invoked directly");
        }

        public override JSValue CreateInstance(in Arguments a)
        {
            return this.CreateFunction(a);
        }
    }
}
