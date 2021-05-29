using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace YantraJS
{
    public abstract class StackGuard<T,TIn> {

        private const int MaxStackSize = 200;

        private int count = 0;

        public T Visit(TIn input)
        {
            if (count == MaxStackSize)
            {
                T output = default;
                count = 0;
                // thread....
                var t = new Thread((p) => {
                    output = VisitIn(input);
                });
                t.Start();
                t.Join();
                count = MaxStackSize;
                return output;
            }

            count++;
            var r = VisitIn(input);
            count--;
            return r;
        }

        public abstract T VisitIn(TIn input);

    }
}
