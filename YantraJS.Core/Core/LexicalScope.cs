using Esprima;
using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core
{
    public class LexicalScope : LinkedStackItem<LexicalScope>
    {
        private UInt32Map<JSVariable> scope;

        internal bool IsRoot = false;

        internal LexicalScope(string fileName, in StringSpan function, int line, int column)
        {
            // this.scope = new UInt32Trie<JSVariable>();
            FileName = fileName;
            Function = function;
            this.Line = line;
            this.Column = column;
            // Console.WriteLine($"{function}, {line}, {column}");
        }

        public string FileName;
        public StringSpan Function;
        public int Line;
        public int Column;

        //public Position Position { 
        //    get => position;
        //    set
        //    {
        //        position = value;
        //        // Console.WriteLine($"{Function} {value.Line}, {value.Column}");
        //    }
        //}

        public JSVariable Create(KeyString name, JSValue v)
        {
            var v1 = new JSVariable(v, name.ToString());
            scope[name.Key] = v1;
            return v1;
        }

        public JSVariable this[KeyString name]
        {
            get => scope[name.Key];
            set => scope[name.Key] = value;
        }

    }
}
