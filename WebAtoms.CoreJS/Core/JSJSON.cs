using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Schema;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Core
{
    public static class JSJSON
    {
        public static KeyString JSON = "JSON";

        internal static JSFunction Create()
        {
            var r = new JSFunction(JSFunction.empty, "JSON");
            r.DefineProperty("stringify", JSProperty.Function(_Stringify));
            return r;
        }


        public static JSValue _Stringify(JSValue t, JSArray a)
        {
            if (a._length == 0)
                return JSUndefined.Value;
            var f = a[0];
            if (f is JSUndefined)
                return f;
            var sb = new StringBuilder();
            Func<(JSValue target, JSValue key, JSValue value),JSValue> replacer = null;
            string indent = "";

            // build replacer...
            if (a._length > 1)
            {
                var r = a[1];
                if (r is JSFunction rf)
                {
                    replacer = (item) =>
                     rf.f(item.target, JSArguments.From(item.key, item.value));
                } else if (r is JSArray ra)
                {

                    BinaryCharMap<int> map = null;
                    
                    replacer = (item) =>
                    {
                        if (map == null)
                        {
                            map = new BinaryCharMap<int>();
                            foreach (var ritem in ra.All)
                            {
                                map[ritem.ToString()] = 1;
                            }
                        }
                        if (map.TryGetValue(item.key.ToString(), out var a1))
                            return item.value;
                        return JSUndefined.Value;
                    };
                }
            }

            Stringify(sb, f, replacer, indent);
            return new JSString(sb.ToString());
        }

        public static string Stringify(JSValue value)
        {
            var sb = new StringBuilder();
            Stringify(sb, value, null, "");
            return sb.ToString();
        }

        private static void Stringify(
            StringBuilder sb, 
            JSValue target, 
            Func<(JSValue, JSValue, JSValue), JSValue> replacer, 
            string indent)
        {
            switch(target) {
                case null:
                case JSNull _:
                    sb.Append("null");
                    return;
                case JSUndefined _:
                    sb.Append("null");
                    return;
                case JSBoolean b:
                    sb.Append(b._value ? "true" : "false");
                    return;
                case JSNumber n:
                    sb.Append(n.value.ToString());
                    return;
                case JSString str:
                    QuoteString(str.value, sb);
                    return;
                case JSFunction _:
                    // do nothing if value is function...
                    sb.Append("null");
                    return;
                case JSArray a:
                    sb.Append('[');
                    bool f = true;
                    foreach(var item in a.All)
                    {
                        if (!f)
                        {
                            sb.Append(',');
                        }
                        f = false;
                        Stringify(sb, ToJson(item), replacer, indent);
                    }
                    sb.Append(']');
                    return;
            }

            sb.Append('{');
            bool first = true;
            // the only left type is JSObject...
            var obj = target as JSObject;
            foreach(var p in obj.ownProperties.AllValues())
            {
                var key = p.Key;
                var value = p.Value;
                if (value.key.IsSymbol || value.IsEmpty || !value.IsEnumerable)
                    continue;
                JSValue jsValue;
                if (!value.IsValue)
                {
                    if (value.get == null)
                        continue;
                    jsValue = (value.get as JSFunction).f(target, JSArguments.Empty);
                } else
                {
                    jsValue = value.value;
                }

                if (jsValue is JSUndefined || jsValue is JSFunction)
                    continue;

                jsValue = ToJson(jsValue);

                // check replacer...
                if (replacer != null)
                {
                    jsValue = replacer(
                        (target,
                        value.key.ToJSValue(), jsValue));
                    if (jsValue is JSUndefined)
                        continue;
                }

                // write indention here...
                if (!first)
                {
                    sb.Append(',');
                }
                first = false;

                QuoteString(value.key.ToString(), sb);
                sb.Append(':');
                Stringify(sb, jsValue, replacer, indent);

            }

            sb.Append('}');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static JSValue ToJson(JSValue value)
        {
            if (!(value is JSObject jobj))
                return value;
            var p = jobj.GetInternalProperty(JSObject.KeyToJSON);
            if (p.IsEmpty)
                return value;
            return (jobj.GetValue(p) as JSFunction).f(value, JSArguments.Empty);
        }


        /// <summary>
        /// Adds double quote characters to the start and end of the given string and converts any
        /// invalid characters into escape sequences.
        /// </summary>
        /// <param name="input"> The string to quote. </param>
        /// <param name="result"> The StringBuilder to write the quoted string to. </param>
        private static void QuoteString(string input, System.Text.StringBuilder result)
        {
            result.Append('\"');

            // Check if there are characters that need to be escaped.
            // These characters include '"', '\' and any character with an ASCII value less than 32.
            bool containsUnsafeCharacters = false;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '\\' || c == '\"' || c < 0x20)
                {
                    containsUnsafeCharacters = true;
                    break;
                }
            }

            if (containsUnsafeCharacters == false)
            {
                // The string does not contain escape characters.
                result.Append(input);
            }
            else
            {
                // The string contains escape characters - fall back to the slower code path.
                foreach (char c in input)
                {
                    switch (c)
                    {
                        case '\"':
                        case '\\':
                            result.Append('\\');
                            result.Append(c);
                            break;
                        case '\b':
                            result.Append("\\b");
                            break;
                        case '\f':
                            result.Append("\\f");
                            break;
                        case '\n':
                            result.Append("\\n");
                            break;
                        case '\r':
                            result.Append("\\r");
                            break;
                        case '\t':
                            result.Append("\\t");
                            break;
                        default:
                            if (c < 0x20)
                            {
                                result.Append('\\');
                                result.Append('u');
                                result.Append(((int)c).ToString("x4"));
                            }
                            else
                                result.Append(c);
                            break;
                    }
                }
            }
            result.Append('\"');
        }
    }
}
