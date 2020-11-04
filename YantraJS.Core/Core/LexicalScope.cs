using Esprima;
using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{
    public class LexicalScope : LinkedStackItem<LexicalScope>
    {
        private readonly UInt32Trie<JSVariable> scope;

        internal bool IsRoot = false;

        internal LexicalScope(string fileName, string function, int line, int column)
        {
            this.scope = new UInt32Trie<JSVariable>();
            FileName = fileName;
            Function = function;
            position = new Position(line, column);
            // Console.WriteLine($"{function}, {line}, {column}");
        }

        public string FileName;
        public string Function;
        private Position position;

        public Position Position { 
            get => position;
            set
            {
                position = value;
                // Console.WriteLine($"{value.Line}, {value.Column}");
            }
        }

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
