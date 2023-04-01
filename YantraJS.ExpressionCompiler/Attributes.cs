#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Yantra.Core
{
    public class JSClassGeneratorAttribute : Attribute
    {
        public readonly string? Name;

        public readonly string KeysClass;

        public JSClassGeneratorAttribute(string? name = null, string keysClass = "KeyStrings")
        {
            this.Name = name;
            this.KeysClass = keysClass;
        }
    }

    public class JSNameGeneratorAttribute : Attribute
    {
    }
}
