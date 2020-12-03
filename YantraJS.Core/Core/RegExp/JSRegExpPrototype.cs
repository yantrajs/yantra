using System;
using System.Runtime.CompilerServices;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public static class JSRegExpPrototype
    {

        [Constructor]
        public static JSValue Constructor(in Arguments a)
        {
            var pattern = "";
            var flags = "";
            if(a.Length > 0)
            {
                pattern = a.GetAt(0).ToString();
            }
            if (a.Length > 1)
            {
                flags = a.GetAt(1).ToString();
            }
            return new JSRegExp(pattern, flags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static JSRegExp ToRegExp(this JSValue target, [CallerMemberName] string name = null)
        {
            if (!(target is JSRegExp n))
                throw JSContext.Current.NewTypeError($"RegExp.prototype.{name} requires that 'this' be a RegExp");
            return n;
        }

        [GetProperty("flags", JSPropertyAttributes.ConfigurableReadonlyProperty)]
        public static JSValue GetFlags(in Arguments a)
        {
            return new JSString(a.This.ToRegExp().flags);
        }

        [GetProperty("global", JSPropertyAttributes.ConfigurableReadonlyProperty)]
        public static JSValue GetGlobal(in Arguments a)
        {
            return a.This.ToRegExp().globalSearch ? JSBoolean.True : JSBoolean.False;
        }

        [GetProperty("lastIndex", JSPropertyAttributes.ConfigurableProperty)]
        public static JSValue GetLastIndex(in Arguments a)
        {
            return new JSNumber(a.This.ToRegExp().lastIndex);
        }

        [SetProperty("lastIndex", JSPropertyAttributes.ConfigurableProperty)]
        public static JSValue SetLastIndex(in Arguments a)
        {
            var @this = a.This.ToRegExp();
            var index = a.Get1().IntValue;
            @this.lastIndex = index;
            return a.Get1();
        }

        [GetProperty("ignoreCase", JSPropertyAttributes.ConfigurableReadonlyProperty)]
        public static JSValue GetIgnoreCase(in Arguments a)
        {
            return a.This.ToRegExp().ignoreCase ? JSBoolean.True : JSBoolean.False;
        }

        [GetProperty("multiline", JSPropertyAttributes.ConfigurableReadonlyProperty)]
        public static JSValue GetMultiline(in Arguments a)
        {
            return a.This.ToRegExp().multiline ? JSBoolean.True : JSBoolean.False;
        }

        [GetProperty("source", JSPropertyAttributes.ConfigurableReadonlyProperty)]
        public static JSValue GetSource(in Arguments a)
        {
            return new JSString(a.This.ToRegExp().pattern);
        }


        [Prototype("test")]
        public static JSValue Test(in Arguments a)
        {
            var r = a.This.ToRegExp();
            var text = a.Get1().ToString();
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

        [Prototype("exec")]
        public static JSValue Exec(in Arguments a)
        {
            var r = a.This.ToRegExp();
            var input = a.Get1().ToString();
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
            var groups = match.Groups;
            var c = (int)groups.Count;
            JSArray result = new JSArray(c);
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
            result["index"] = new JSNumber(match.Index);
            result["input"] = a.Get1();
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
            var maxIndex = r.lastIndex > 0 ? r.lastIndex : 0;
            var minIndex = maxIndex < input.Length ? maxIndex : input.Length;
            // return Math.Min(Math.Max(r.lastIndex, 0), input.Length);
            return minIndex;
        }
    }
}
