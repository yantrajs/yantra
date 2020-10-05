using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.String
{
    public class JSTemplateString
    {
        StringBuilder sb;
        public JSTemplateString()
        {
            sb = new StringBuilder();
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

    }
}
