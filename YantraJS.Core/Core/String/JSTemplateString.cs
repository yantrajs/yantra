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

        public JSTemplateString AddQuasi(string text)
        {
            sb.Append(text);
            return this;
        }

        public JSTemplateString AddExpression(JSValue value)
        {
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
