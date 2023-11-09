using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Schema;
using Yantra.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core
{
    public delegate JSValue JsonParserReceiver((string key, JSValue value) property);

    [JSClassGenerator("JSON"), JSInternalObject]
    public partial class JSJSON: JSObject
    {
        [JSExport]
        public static JSValue Parse(in Arguments a)
        {
            var (text, receiver) = a.Get2();
            JsonParserReceiver r = null;
            var t = a.This;
            if (receiver is JSFunction function)
            {
                r = (p) => function.f( new Arguments(t, new JSString(p.key), p.value));
            }
            return JSJsonParser.Parse(text.ToString(), r) ?? JSNull.Value;

        }

        //private static JsonSerializerOptions options;

        //private static JSValue Parse(string text)
        //{
        //    var e = text.
        //}


        [JSExport]
        public static JSValue Stringify(in Arguments a)
        {
            var (f, r, pi) = a.Get3();
            if (f.IsUndefined)
                return f;
            TextWriter sb = new StringWriter();
            Func<(JSValue target, JSValue key, JSValue value),JSValue> replacer = null;
            string indent = null;

            // build replacer...
            if (a.Length > 1)
            {
                if (a.Length > 2)
                {
                    if (pi is JSNumber jn)
                    {
                        indent = new string(' ', pi.IntValue);
                    } else if (pi is JSString js)
                    {
                        indent = js.ToString();
                    }
                }

                if (r is JSFunction rf)
                {
                    replacer = (item) =>
                     rf.f(new Arguments(item.target, item.key, item.value));
                } else if (r is JSArray ra)
                {

                    StringMap<int> map = new StringMap<int>();
                    
                    replacer = (item) =>
                    {
                        var en = ra.GetElementEnumerator();
                        while(en.MoveNext(out var hasValue, out var ri, out var index))
                        {
                            map.Put(ri.ToString()) = 1;
                        }
                        if (map.TryGetValue(item.key.ToString(), out var a1))
                            return item.value;
                        return JSUndefined.Value;
                    };
                }
            }
            if (indent != null)
            {
                var writer = new IndentedTextWriter(sb, indent);
                Stringify(writer, f, replacer, writer);
            } else
            {
                Stringify(sb, f, replacer, null);
            }
            
            return new JSString(sb.ToString());
        }

        public static string Stringify(JSValue value)
        {
            var sb = new StringWriter();
            Stringify(sb, value, null, null);
            return sb.ToString();
        }

        private static void Stringify(
            TextWriter sb, 
            JSValue target, 
            Func<(JSValue, JSValue, JSValue), JSValue> replacer,
            IndentedTextWriter indent)
        {
            if (target == null || target.IsNullOrUndefined)
            {
                sb.Write("null");
                return;
            }
            if (target == JSBoolean.True)
            {
                sb.Write("true");
                return;
            }
            if (target == JSBoolean.False)
            {
                sb.Write("false");
                return;
            }
            switch (target) {
                case JSNumber n:
                    sb.Write(n.value.ToString());
                    return;
                case JSString str:
                    QuoteString(str.value, sb);
                    return;
                case JSFunction _:
                    return;
                case JSArray a:
                    sb.Write('[');
                    if (indent != null)
                    {
                        indent.Indent++;
                    }
                    bool f = true;
                    var ae = a.GetElementEnumerator();
                    while(ae.MoveNext(out var hasValue, out var item, out var index))
                    {
                        if (!f)
                        {
                            sb.Write(',');
                        }
                        f = false;
                        if (indent != null)
                        {
                            sb.WriteLine();
                        }
                        Stringify(sb, ToJson(item), replacer, indent);
                    }
                    if (indent != null)
                    {
                        sb.WriteLine();
                        indent.Indent--;
                    }
                    sb.Write(']');
                    return;
            }

            sb.Write('{');
            if (indent != null)
            {
                indent.Indent++;
            }
            bool first = true;
            // the only left type is JSObject...
            var obj = target as JSObject;
            var pen = obj.GetOwnProperties().GetEnumerator();
            while(pen.MoveNext(out var key, out var value))
            {
                if (value.IsEmpty || !value.IsEnumerable)
                    continue;
                JSValue jsValue;
                if (!value.IsValue)
                {
                    if (value.get == null)
                        continue;
                    jsValue = (value.get as JSFunction).f(new Arguments(target));
                } else
                {
                    jsValue = value.value;
                }

                if (jsValue.IsUndefined || jsValue is JSFunction)
                    continue;

                jsValue = ToJson(jsValue);

                // check replacer...
                if (replacer != null)
                {
                    jsValue = replacer(
                        (target,
                        KeyStrings.GetJSString(value.key), jsValue));
                    if (jsValue.IsUndefined)
                        continue;
                }

                // write indention here...
                if (!first)
                {
                    sb.Write(',');
                }
                first = false;
                if (indent != null)
                {
                    sb.WriteLine();
                }

                QuoteString(key.Value, sb);
                sb.Write(':');
                if (indent != null)
                {
                    sb.Write(' ');
                }
                Stringify(sb, jsValue, replacer, indent);

            }
            if (indent != null)
            {
                sb.WriteLine();
                indent.Indent--;
            }

            sb.Write('}');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static JSValue ToJson(JSValue value)
        {
            if (!(value is JSObject jobj))
                return value;
            var p = jobj.GetMethod(KeyStrings.toJSON);
            if (p == null)
                return value;
            return p(new Arguments(value));
        }


        /// <summary>
        /// Adds double quote characters to the start and end of the given string and converts any
        /// invalid characters into escape sequences.
        /// </summary>
        /// <param name="input"> The string to quote. </param>
        /// <param name="result"> The StringBuilder to write the quoted string to. </param>
        private static void QuoteString(in StringSpan input, TextWriter result)
        {
            result.Write('\"');

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
                result.Write(input);
            }
            else
            {
                // The string contains escape characters - fall back to the slower code path.
                var en = input.GetEnumerator();
                while(en.MoveNext(out  var c))
                {
                    switch (c)
                    {
                        case '\"':
                        case '\\':
                            result.Write('\\');
                            result.Write(c);
                            break;
                        case '\b':
                            result.Write("\\b");
                            break;
                        case '\f':
                            result.Write("\\f");
                            break;
                        case '\n':
                            result.Write("\\n");
                            break;
                        case '\r':
                            result.Write("\\r");
                            break;
                        case '\t':
                            result.Write("\\t");
                            break;
                        default:
                            if (c < 0x20)
                            {
                                result.Write('\\');
                                result.Write('u');
                                result.Write(((int)c).ToString("x4"));
                            }
                            else
                                result.Write(c);
                            break;
                    }
                }
            }
            result.Write('\"');
        }
    }
}
