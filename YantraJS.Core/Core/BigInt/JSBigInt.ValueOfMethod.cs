
namespace YantraJS.Core.BigInt;

internal partial class JSBigInt
{
    class ValueOfMethod : JSFunction
    {
        public ValueOfMethod(JSObject prototype): base("valueOf")
        {
            
        }

        public sealed override JSValue InvokeFunction(in Arguments a)
        {
            if (!(a.This is JSBigInt @this))
                throw JSContext.Current.NewTypeError("Failed to convert this to JSBigInt");
            return @this;
        }
    }
}
