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

    /// <summary>
    /// Only create new scope if there is an internal function definition
    /// </summary>
    public class LexicalScope
    {

        private BinaryUInt32Map<JSVariable> scope = new BinaryUInt32Map<JSVariable>();

        public LexicalScope()
        {

        }

        public ScopeItem NewScope()
        {
            return new ScopeItem(this.scope);
        }

        public JSVariable this[KeyString key]
        {
            get
            {
                return scope[key.Key];
            }
            set
            {
                scope[key.Key] = value;
            }
        }

    }

    public class ScopeItem : IDisposable
    {
        readonly List<(UInt32, JSVariable)> previous = new List<(uint, JSVariable)>();
        readonly BinaryUInt32Map<JSVariable> scope;
        internal ScopeItem(BinaryUInt32Map<JSVariable> scope)
        {
            this.scope = scope;
        }

        public JSVariable Create(KeyString name, JSValue v)
        {
            var old = scope[name.Key];
            previous.Add((name.Key, old));
            var v1 = new JSVariable { Value = v };
            scope[name.Key] = v1;
            return v1;
        }

        public void Dispose()
        {
            foreach(var (key, value) in previous)
            {
                if (value == null)
                {
                    scope.RemoveAt(key);
                } else {
                    scope[key] = value;
                }
            }
        }
    }
}
