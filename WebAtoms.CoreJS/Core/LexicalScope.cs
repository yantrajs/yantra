﻿using Esprima;
using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{

    public struct DisposableAction: IDisposable
    {
        readonly Action action;
        public DisposableAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }

    public class LexicalScope : LinkedStackItem<LexicalScope>
    {
        private readonly BinaryUInt32Map<JSVariable> scope;

        internal bool IsRoot = false;

        internal LexicalScope(string fileName, string function, int line, int column)
        {
            this.scope = new BinaryUInt32Map<JSVariable>();
            FileName = fileName;
            Function = function;
            Position = new Position(line, column);
        }

        public string FileName;
        public string Function;
        public Position Position;

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