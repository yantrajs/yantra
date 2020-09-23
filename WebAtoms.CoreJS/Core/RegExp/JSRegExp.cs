using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAtoms.CoreJS.Core
{

    [JSRuntime(typeof(JSRegExpStatic), typeof(JSRegExpPrototype))]
    public class JSRegExp: JSObject
    {

        internal readonly string pattern;
        internal readonly string flags;

        internal readonly bool globalSearch;
        internal readonly bool multiline;
        internal readonly bool ignoreCase;
        internal Regex value;

        internal int lastIndex = 0;

        public JSRegExp(string pattern, string flags): base(JSContext.Current.RegExpPrototype)
        {
            this.flags = flags;
            this.pattern = pattern;

            this.value = CreateRegex(pattern, ParseFlags(flags, ref this.globalSearch, ref this.ignoreCase, ref this.multiline));

            this.DefineProperty(KeyStrings.lastIndex, 
                JSProperty.Property(KeyStrings.lastIndex, 
                (_,__) => new JSNumber(lastIndex), 
                null, 
                JSPropertyAttributes.ConfigurableReadonlyProperty ));
        }

        /// <summary>
        /// Parses the flags parameter into an enum.
        /// </summary>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        /// <returns> RegexOptions flags that correspond to the given flags. </returns>
        private RegexOptions ParseFlags(string flags, 
            ref bool globalSearch,
            ref bool ignoreCase,
            ref bool multiline)
        {
            var options = RegexOptions.ECMAScript;
            globalSearch = false;

            if (flags != null)
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    char flag = flags[i];
                    if (flag == 'g')
                    {
                        if (this.globalSearch == true)
                            throw JSContext.Current.NewSyntaxError("The 'g' flag cannot be specified twice");
                        globalSearch = true;
                    }
                    else if (flag == 'i')
                    {
                        if ((options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
                            throw JSContext.Current.NewSyntaxError("The 'i' flag cannot be specified twice");
                        options |= RegexOptions.IgnoreCase;
                        ignoreCase = true;
                    }
                    else if (flag == 'm')
                    {
                        if ((options & RegexOptions.Multiline) == RegexOptions.Multiline)
                            throw JSContext.Current.NewSyntaxError("The 'm' flag cannot be specified twice");
                        options |= RegexOptions.Multiline;
                        multiline = true;
                    }
                    else
                    {
                        throw JSContext.Current.NewSyntaxError($"Unknown flag {flag}");
                    }
                }
            }
            return options;
        }

        /// <summary>
        /// Creates a .NET Regex object using the given pattern and options.
        /// </summary>
        /// <param name="pattern"> The pattern string. </param>
        /// <param name="options"> The regular expression options. </param>
        /// <returns> A constructed .NET Regex object. </returns>
        private Regex CreateRegex(string pattern, RegexOptions options)
        {
            if ((options & RegexOptions.Multiline) == RegexOptions.Multiline)
            {
                // In the .NET Regex implementation with multiline mode:
                // '.' matches any character except \n
                // '^' matches the start of the string or \n (positive lookbehind)
                // '$' matches the end of the string or \n (positive lookahead)
                // In Javascript, we want all three characters to also match \r in the same way they match \n.

                StringBuilder builder = null;
                int start = 0, end = -1;
                while (end < pattern.Length)
                {
                    end = pattern.IndexOfAny(new char[] { '.', '^', '$', '\\' }, end + 1);
                    if (end == -1)
                        break;
                    if (builder == null)
                        builder = new StringBuilder();
                    builder.Append(pattern.Substring(start, end - start));
                    start = end + 1;
                    switch (pattern[end])
                    {
                        case '.':
                            builder.Append(@"[^\r\n]");
                            break;
                        case '^':
                            // [^abc] is a thing. The ^ does NOT match the start of the line in this case.
                            if (end > 0 && pattern[end - 1] == '[')
                                builder.Append('^');
                            else
                                builder.Append(@"(?<=^|\r)");
                            break;
                        case '$':
                            builder.Append(@"(?=$|\r)");
                            break;
                        case '\\':
                            // $ is an anchor. \$ matches the literal dollar sign. \\$ is a backslash then an anchor.
                            if (end < pattern.Length - 1)
                            {
                                builder.Append(pattern[end]);
                                builder.Append(pattern[end + 1]);
                                start++;
                                end++;
                            }
                            break;
                    }
                }
                if (builder != null)
                {
                    builder.Append(pattern.Substring(start));
                    pattern = builder.ToString();
                }
            }

            return new Regex(pattern, options);
        }

        public override string ToString()
        {
            return $"/{this.pattern}/{this.flags}";
        }
    }
}
