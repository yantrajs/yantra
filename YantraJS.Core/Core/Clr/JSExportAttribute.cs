using System;

namespace YantraJS.Core.Clr
{
    public class JSExportAttribute: Attribute {

        public readonly string? Name;

        public bool AsCamel = true;

        public JSExportAttribute(
            string? name = null)
        {
            this.Name = name;
        }

    }
}
