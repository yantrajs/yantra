using System;
using System.Runtime.CompilerServices;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public static class JSRegExpPrototype
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static JSRegExp ToRegExp(this JSValue target, [CallerMemberName] string name = null)
        {
            if (!(target is JSRegExp n))
                throw JSContext.Current.NewTypeError($"RegExp.prototype.{name} requires that 'this' be a RegExp");
            return n;
        }


        [Prototype("test")]
        public static JSValue Test(JSValue t, JSValue[] a)
        {
            var r = t.ToRegExp();
            var text = a.GetAt(0).ToString();
            var match = r.value.Match(text, r.CalculateStartPosition(text));
            if (match.Success)
            {
                if (r.globalSearch)
                {
                    r.lastIndex = match.Index + match.Length;
                }
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }


        /// <summary>
        /// Calculates the position to start searching.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> The character position to start searching. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateStartPosition(this JSRegExp r, string input)
        {
            if (r.globalSearch == false)
                return 0;
            return Math.Min(Math.Max(r.lastIndex, 0), input.Length);
        }
    }
}
