using System;
using System.Collections;
using System.Collections.Generic;

namespace WebAtoms.CoreJS.Core.Generator
{
    public struct JSGeneratorEnumerator : IEnumerator<(uint Key, JSProperty Value)>
    {
        JSGenerator g;
        uint index;
        public JSGeneratorEnumerator(JSGenerator g)
        {
            this.g = g;
            index = 0;
        }

        public (uint Key, JSProperty Value) Current => (index - 1, JSProperty.Property(g.value));

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            this.g.Next();
            return !g.done;
        }

        public void Reset()
        {

        }
    }
}
