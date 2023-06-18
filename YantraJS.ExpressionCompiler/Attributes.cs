#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Yantra.Core
{
    public class JSInternalObjectAttribute: Attribute
    {

    }
    public class JSBaseClassAttribute: Attribute
    {
        public readonly string Name;

        public JSBaseClassAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class JSPrototypeMethodAttribute: Attribute {
    }

    public class JSGlobalFunctionAttribute: Attribute
    {

    }

    public class JSFunctionGeneratorAttribute : Attribute
    {
        public readonly string? Name;

        public readonly string KeysClass;

        public bool Register { get; set; } = true;

        public bool Globals { get; set; }

        public JSFunctionGeneratorAttribute(string? name = null, string keysClass = "KeyStrings")
        {
            this.Name = name;
            this.KeysClass = keysClass;
        }
    }

    public class JSClassGeneratorAttribute : Attribute
    {
        public readonly string? Name;

        public readonly string KeysClass;

        public bool Register { get; set; } = true;

        public JSClassGeneratorAttribute(string? name = null, string keysClass = "KeyStrings")
        {
            this.Name = name;
            this.KeysClass = keysClass;
        }
    }

    public class JSRegistrationGeneratorAttribute : Attribute
    {
    }
}
