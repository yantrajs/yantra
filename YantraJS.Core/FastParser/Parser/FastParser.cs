using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.FastParser
{
    public partial class FastParser
    {
        private readonly FastTokenStream stream;

        public FastParser(FastTokenStream stream)
        {
            this.stream = stream;

        }

        private Func<T> Consume<T>(Func<T> func)
        {
            return () => {
                stream.Consume();
                return func();
            };
        }

    }
}
