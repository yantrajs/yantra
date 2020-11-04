using System;
using System.Linq;

namespace YantraJS
{
    public struct ScriptLocation
    {
        public string Location { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

    }
}
