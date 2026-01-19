
using System.Globalization;

namespace YantraJS.Core.BigInt;

internal partial class JSBigInt
{
    class ToLocaleStringMethod : JSFunction
    {
        public ToLocaleStringMethod(JSObject prototype): base("toLocaleString")
        {
            
        }

        public sealed override JSValue InvokeFunction(in Arguments a)
        {
            if (!(a.This is JSBigInt @this))
                throw JSContext.Current.NewTypeError("Failed to convert this to JSBigInt");
            return new JSString(@this.value.ToString(CultureInfo.CurrentCulture));
        }
    }
}
