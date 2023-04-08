using System;
using System.Runtime.CompilerServices;
using YantraJS.Core.Clr;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public partial class JSRegExp
    {

        [JSExport]
        public int LastIndex
        {
            get
            {
                return this.lastIndex;
            }
            set
            {
                this.lastIndex = value;
            }
        }

        [JSExport("test")]
        public JSValue Test(in Arguments a)
        {
            var text = a.Get1().ToString();
            var match = value.Match(text, CalculateStartPosition(text));
            if (match.Success)
            {
                if (globalSearch)
                {
                    lastIndex = match.Index + match.Length;
                }
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [JSExport("exec")]
        public JSValue Exec(in Arguments a)
        {
            var input = a.Get1().ToString();
            // Perform the regular expression matching.
            var match = value.Match(input, CalculateStartPosition(input));

            // Return null if no match was found.
            if (match.Success == false)
            {
                // Reset the lastIndex property.
                if (globalSearch == true)
                    lastIndex = 0;
                return JSNull.Value;
            }

            if (globalSearch)
            {
                lastIndex = match.Index + match.Length;
            }
            var groups = match.Groups;
            var c = (int)groups.Count;
            JSArray result = new JSArray((uint)c);
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
            result[KeyStrings.index] = new JSNumber(match.Index);
            result[KeyStrings.input] = a.Get1();
            return result;
        }

        /// <summary>
        /// Calculates the position to start searching.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> The character position to start searching. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateStartPosition(string input)
        {
            if (globalSearch == false)
                return 0;
            var maxIndex = lastIndex > 0 ? lastIndex : 0;
            var minIndex = maxIndex < input.Length ? maxIndex : input.Length;
            // return Math.Min(Math.Max(r.lastIndex, 0), input.Length);
            return minIndex;
        }
    }
}
