using System;
using System.Collections;
using System.Collections.Generic;

namespace YantraJS.Core.Generator
{
    public struct JSGeneratorEnumerator : IEnumerator<(uint Key, JSProperty Value)>
    {
        readonly JSGenerator g;
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
            index++;
            return !g.done;
        }

        public void Reset()
        {

        }
    }
}
