using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.String
{
    public class JSTemplateString
    {
        StringBuilder sb;
        public JSTemplateString(int size)
        {
            sb = new StringBuilder(size);
        }

        public void Add(string t)
        {
            sb.Append(t);
        }

        public unsafe void Add(JSValue value)
        {
            sb.Append(value.ToString());
        }

        public JSTemplateString AddQuasi(string text)
        {
            sb.Append(text);
            return this;
        }

        public unsafe JSTemplateString AddExpression(JSValue value)
        {
            //if (value is JSString @string)
            //{
            //    var span = @string.Value;
            //    fixed (char* start = span.Source)
            //    {
            //        char* ch1 = start + (span.Offset);
            //        sb.Append(ch1, span.Length);
            //    }
            //    return this;
            //}
            sb.Append(value.ToString());
            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        public JSValue ToJSString()
        {
            return new JSString(sb.ToString());
        }

    }
}
