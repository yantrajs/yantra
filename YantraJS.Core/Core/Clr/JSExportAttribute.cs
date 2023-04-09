#nullable enable
using System;

namespace YantraJS.Core.Clr
{
    public class JSExportAttribute: Attribute {

        public readonly string? Name;

        public bool AsCamel = true;

        public int Length { get; set; }

        public bool Pure { get; set; }

        public bool IsConstructor { get; set; }

        public JSExportAttribute(
            string? name = null)
        {
            this.Name = name;
        }

    }
}
