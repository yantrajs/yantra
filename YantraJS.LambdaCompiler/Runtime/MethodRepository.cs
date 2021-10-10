using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using YantraJS.Core;

namespace YantraJS
{
    public class MethodRepository : IMethodRepository
    {

        public string IL;
        public string Exp;

        private Sequence<(DynamicMethod method, string il, string exp, Type type)> delegates
            = new Sequence<(DynamicMethod method, string il, string exp, Type type)>();


        public int RegisterNew(DynamicMethod d, string il, string exp, Type type)
        {
            int i = delegates.Count;
            delegates.Add((d, il, exp, type));
            return i;
        }

        public object Create(Box[] boxes, int id)
        {
            var (m, il, exp,t) = delegates[id];
            var c = new Closures(boxes, il, exp);
            return m.CreateDelegate(t, c);
        }
    }
}
