using System;
using System.Collections.Generic;

namespace YantraJS
{
    public class MethodRepository : IMethodRepository
    {
        private List<Delegate> delegates = new List<Delegate>();
        public int RegisterNew(Delegate d)
        {
            int i = delegates.Count;
            delegates.Add(d);
            return i;
        }

        public object Run(int id)
        {
            return delegates[id];
        }
    }
}
