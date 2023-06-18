using System.Runtime.CompilerServices;
using YantraJS.Core.Core.Primitive;

namespace YantraJS.Core.InternalExtensions
{
    internal static class JSStringExtensions
    {

        internal static JSString AsJSString(this JSValue v,
            [CallerMemberName] string helper = null)
        {
            if (v.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError($"String.prototype.{helper} called on null or undefined");
            if (v is JSString str)
                return str;
            if (v is JSPrimitiveObject primitiveObject)
                return primitiveObject.value.AsJSString();
            throw JSContext.Current.NewTypeError($"String.prototype.{helper} called with non string");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AsString(this JSValue v,
            [CallerMemberName] string helper = null)
        {
            if (v.IsNullOrUndefined)
                throw JSContext.Current.NewTypeError($"String.prototype.{helper} called on null or undefined");
            return v.ToString();
        }
    }


}
