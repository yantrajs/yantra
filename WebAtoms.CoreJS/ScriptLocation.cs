using System;
using System.Linq;

namespace WebAtoms.CoreJS
{
    public struct ScriptLocation
    {
        public string Location { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

    }
}
