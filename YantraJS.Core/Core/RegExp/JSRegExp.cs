using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{


    [JSClassGenerator("RegExp")]
    public partial class JSRegExp: JSObject
    {

        [JSExport("source")]
        public readonly string pattern;

        [JSExport]
        public readonly string flags;

        [JSExport("global")]
        public readonly bool globalSearch;

        [JSExport]
        public readonly bool multiline;
        [JSExport]
        public readonly bool ignoreCase;
        
        internal Regex value;

        [JSExport]
        public int lastIndex = 0;


        public JSRegExp(in Arguments a): base(JSContext.NewTargetPrototype)
        {
            var pattern = "";
            var flags = "";
            if (a.Length > 0)
            {
                pattern = a.GetAt(0).ToString();
            }
            if (a.Length > 1)
            {
                flags = a.GetAt(1).ToString();
            }
            this.flags = flags;
            this.pattern = pattern;

            (this.value, globalSearch, ignoreCase, multiline) = CreateRegex(pattern, flags);

        }


        public JSRegExp(string pattern, string flags): this()
        {
            this.flags = flags;
            this.pattern = pattern;

            (this.value, globalSearch, ignoreCase, multiline) = CreateRegex(pattern, flags);

            //this.DefineProperty(KeyStrings.lastIndex, 
            //    JSProperty.Property(KeyStrings.lastIndex, 
            //    (in Arguments a) => new JSNumber(lastIndex), 
            //    (in Arguments a) =>
            //    {
            //        this.lastIndex = a.Get1().IntValue;
            //        return a.Get1();
            //    }, 
            //    JSPropertyAttributes.ConfigurableProperty ));
        }

        /// <summary>
        /// Finds all regular expression matches within the given string.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <returns> An array containing the matched strings. </returns>
        public JSValue Match(JSValue input)
        {
            // If the global flag is not set, returns a single match.
            if (this.globalSearch == false)
            {
                var arg = new Arguments(this, input);

                return Exec(arg);
            }

            // Otherwise, find all matches.
            var matches = this.value.Matches(input.ToString());
            if (matches.Count == 0)
                return JSNull.Value;

            // Set the deprecated RegExp properties (using the last match).
            //this.Engine.RegExp.SetDeprecatedProperties(input, matches[matches.Count - 1]);

            // Construct the array to return.
            JSArray matchValues = new JSArray((uint)matches.Count);
            for (int i = 0; i < matches.Count; i++)
                matchValues[(uint)i] = new JSString(matches[i].Value);
            return matchValues;
            //return this.Engine.Array.New(matchValues);
        }

        /// <summary>
        /// Splits the given string into an array of strings by separating the string into substrings.
        /// </summary>
        /// <param name="input"> The string to split. </param>
        /// <param name="limit"> The maximum number of array items to return.  Defaults to unlimited. </param>
        /// <returns> An array containing the split strings. </returns>
        public JSValue Split(string input, uint limit = uint.MaxValue)
        {
            // Return an empty array if limit = 0.
            if (limit == 0)
                return new JSArray();
                

            // Find the first match.
            Match match = this.value.Match(input, 0);

            
            var results = new JSArray();
            int startIndex = 0;
            Match lastMatch = null;
            while (match.Success == true)
            {
                // Do not match the an empty substring at the start or end of the string or at the
                // end of the previous match.
                if (match.Length == 0 && (match.Index == 0 || match.Index == input.Length || match.Index == startIndex))
                {
                    // Find the next match.
                    match = match.NextMatch();
                    continue;
                }

                // Add the match results to the array.
                var element = input.Substring(startIndex, match.Index - startIndex);
                results.Add(new JSString(element));
                //if (results.Count >= limit)
                if (results.Length >= limit)
                  return  results;
                startIndex = match.Index + match.Length;
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    var group = match.Groups[i];
                    if (group.Captures.Count == 0)
                        results.Add(JSUndefined.Value);       // Non-capturing groups return "undefined".
                    else
                        results.Add(new JSString(match.Groups[i].Value));
                    if (results.Length >= limit)
                        return results;
                }

                // Record the last match.
                lastMatch = match;

                // Find the next match.
                match = match.NextMatch();
            }
            var ele = input.Substring(startIndex, input.Length - startIndex);
            results.Add(new JSString(ele));
            return results;
            // Set the deprecated RegExp properties.
           // if (lastMatch != null)
           //     this.Engine.RegExp.SetDeprecatedProperties(input, lastMatch);

           // return this.Engine.Array.New(results.ToArray());
        }


        /// <summary>
        /// Returns a copy of the given string with text replaced using a regular expression.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <param name="replaceFunction"> A function that is called to produce the text to replace
        /// for every successful match. </param>
        /// <returns> A copy of the given string with text replaced using a regular expression. </returns>
        public string Replace(string input, JSValue replaceFunction)
        {
            if (!replaceFunction.IsFunction)
                return Replace(input, replaceFunction.ToString());
            return this.value.Replace(input, match =>
            {
                // Set the deprecated RegExp properties.
                //this.Engine.RegExp.SetDeprecatedProperties(input, match);

                JSValue[] parameters = new JSValue[match.Groups.Count + 2];
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    if (match.Groups[i].Success == false)
                        parameters[i] = JSUndefined.Value;
                    else
                        parameters[i] = new JSString(match.Groups[i].Value);
                }
                parameters[match.Groups.Count] = new JSNumber(match.Index);
                parameters[match.Groups.Count + 1] = new JSString(input);
                var a = new Arguments(JSNull.Value, parameters);
                return replaceFunction.InvokeFunction(a).ToString();
            }, this.globalSearch == true ? int.MaxValue : 1);
        }

        /// <summary>
        /// Returns a copy of the given string with text replaced using a regular expression.
        /// </summary>
        /// <param name="input"> The string on which to perform the search. </param>
        /// <param name="replaceText"> A string containing the text to replace for every successful match. </param>
        /// <returns> A copy of the given string with text replaced using a regular expression. </returns>
        public string Replace(string input, string replaceText)
        {
            // Check if the replacement string contains any patterns.
            bool replaceTextContainsPattern = replaceText.IndexOf('$') >= 0;

            // Replace the input string with replaceText, recording the last match found.
            Match lastMatch = null;
            string result = this.value.Replace(input, match =>
            {
                lastMatch = match;

                // If there is no pattern, replace the pattern as is.
                if (replaceTextContainsPattern == false)
                    return replaceText;

                // Patterns
                // $$	Inserts a "$".
                // $&	Inserts the matched substring.
                // $`	Inserts the portion of the string that precedes the matched substring.
                // $'	Inserts the portion of the string that follows the matched substring.
                // $n or $nn	Where n or nn are decimal digits, inserts the nth parenthesized submatch string, provided the first argument was a RegExp object.
                var replacementBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < replaceText.Length; i++)
                {
                    char c = replaceText[i];
                    if (c == '$' && i < replaceText.Length - 1)
                    {
                        c = replaceText[++i];
                        if (c == '$')
                            replacementBuilder.Append('$');
                        else if (c == '&')
                            replacementBuilder.Append(match.Value);
                        else if (c == '`')
                            replacementBuilder.Append(input.Substring(0, match.Index));
                        else if (c == '\'')
                            replacementBuilder.Append(input.Substring(match.Index + match.Length));
                        else if (c >= '0' && c <= '9')
                        {
                            int matchNumber1 = c - '0';

                            // The match number can be one or two digits long.
                            int matchNumber2 = 0;
                            if (i < replaceText.Length - 1 && replaceText[i + 1] >= '0' && replaceText[i + 1] <= '9')
                                matchNumber2 = matchNumber1 * 10 + (replaceText[i + 1] - '0');

                            // Try the two digit capture first.
                            if (matchNumber2 > 0 && matchNumber2 < match.Groups.Count)
                            {
                                // Two digit capture replacement.
                                replacementBuilder.Append(match.Groups[matchNumber2].Value);
                                i++;
                            }
                            else if (matchNumber1 > 0 && matchNumber1 < match.Groups.Count)
                            {
                                // Single digit capture replacement.
                                replacementBuilder.Append(match.Groups[matchNumber1].Value);
                            }
                            else
                            {
                                // Capture does not exist.
                                replacementBuilder.Append('$');
                                i--;
                            }
                        }
                        else
                        {
                            // Unknown replacement pattern.
                            replacementBuilder.Append('$');
                            replacementBuilder.Append(c);
                        }
                    }
                    else
                        replacementBuilder.Append(c);
                }

                return replacementBuilder.ToString();
            }, this.globalSearch == true ? -1 : 1);

            // Set the deprecated RegExp properties if at least one match was found.
            // if (lastMatch != null)
            //     this.Engine.RegExp.SetDeprecatedProperties(input, lastMatch);

            return result;
        }

        /// <summary>
        /// Parses the flags parameter into an enum.
        /// </summary>
        /// <param name="flags"> Available flags, which may be combined, are:
        /// g (global search for all occurrences of pattern)
        /// i (ignore case)
        /// m (multiline search)</param>
        /// <returns> RegexOptions flags that correspond to the given flags. </returns>
        private static
            (RegexOptions, bool, bool, bool)
            ParseFlags(string flags)
        {
            bool globalSearch = false;
            bool ignoreCase = false;
            bool multiline = false;

            var options = RegexOptions.ECMAScript;

            if (flags != null)
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    char flag = flags[i];
                    if (flag == 'g')
                    {
                        if (globalSearch == true)
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
                    else if (flag == 'u')
                    {
                        // don't know what to do....
                    }
                    else
                    {
                        throw JSContext.Current.NewSyntaxError($"Unknown flag {flag}");
                    }
                }
            }
            return (options, globalSearch, ignoreCase, multiline);
        }

        /// <summary>
        /// Creates a .NET Regex object using the given pattern and options.
        /// </summary>
        /// <param name="pattern"> The pattern string. </param>
        /// <param name="options"> The regular expression options. </param>
        /// <returns> A constructed .NET Regex object. </returns>
        public static (Regex, bool, bool, bool) CreateRegex(string pattern, string flags)
        {
            try
            {
                var (options, globalSearch, ignoreCase, multiline) = ParseFlags(flags);

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

                return (new Regex(pattern, options), globalSearch, ignoreCase, multiline);
            } catch
            {
                return (null, false, false, false);
            }
        }

        public override string ToString()
        {
            return $"/{this.pattern}/{this.flags}";
        }
    }
}
