using Esprima.Ast;
using System.Collections.Generic;

namespace WebAtoms.CoreJS
{
    public class ParsedScript
    {
        readonly List<Esprima.Ast.Range> lines;
        readonly string code;
        public ParsedScript(string code)
        {
            this.code = code;
            lines = new List<Esprima.Ast.Range>();
            // split in line numbers...
            int start = 0;
            foreach(var line in code.Split('\n'))
            {
                lines.Add(new Range(start, start + line.Length));
                start += line.Length + 1;
            }
        }

        public string Text(Range r)
        {
            int start = r.Start;
            int end = r.End;
            return this.code.Substring(start, end - start);
        }

    }
}
