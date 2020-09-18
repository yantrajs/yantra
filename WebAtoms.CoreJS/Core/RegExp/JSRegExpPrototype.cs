using System;
using System.Runtime.CompilerServices;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public static class JSRegExpPrototype
    {

        [Constructor]
        public static JSValue Constructor(JSValue t, JSValue[] a)
        {
            var (pattern, flags) = a.Get2();
            return new JSRegExp(pattern.ToString(), flags.IsNull || flags.IsUndefined ? "" : flags.ToString());
        }

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

        [Prototype("test")]
        public static JSValue Exec(JSValue t, JSValue[] a)
        {
            var r = t.ToRegExp();
            var input = a.GetAt(0).ToString();
            // Perform the regular expression matching.
            var match = r.value.Match(input, r.CalculateStartPosition(input));

            // Return null if no match was found.
            if (match.Success == false)
            {
                // Reset the lastIndex property.
                if (r.globalSearch == true)
                    r.lastIndex = 0;
                return JSNull.Value;
            }

            if (r.globalSearch)
            {
                r.lastIndex = match.Index + match.Length;
            }
            JSArray result = new JSArray();
            var groups = match.Groups;
            var c = (int)groups.Count;
            for (int i = 0; i < c; i++)
            {
                var group = groups[i];
                if (group.Captures.Count == 0)
                {
                    result[(uint)i] = JSUndefined.Value;
                } else
                {
                    result[(uint)i] = new JSString(group.Value);
                }
            }
            return result;
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
