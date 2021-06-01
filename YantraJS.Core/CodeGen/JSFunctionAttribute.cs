using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.CodeGen
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JSFunctionAttribute: Attribute
    {
        public readonly string Location;
        public readonly int Line;
        public readonly int Column;

        public JSFunctionAttribute(string location, int line, int column)
        {
            this.Location = location;
            this.Line = line;
            this.Column = column;
        }

    }
}
