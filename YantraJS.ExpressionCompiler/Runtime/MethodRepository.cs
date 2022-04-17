using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using YantraJS.Core;

namespace YantraJS
{
    public class MethodRepository : IMethodRepository
    {

        public static ConstructorInfo constructor = typeof(MethodRepository).GetConstructor();

        public string IL;
        public string Exp;

        public class RuntimeMethod
        {
            public DynamicMethod Method;
            public string IL;
            public string Exp;
            public Type Type;
        }

        //private Sequence<(DynamicMethod method, string il, string exp, Type type)> delegates
        //    = new Sequence<(DynamicMethod method, string il, string exp, Type type)>();


        public ulong RegisterNew(DynamicMethod d, string il, string exp, Type type)
        {
            //int i = delegates.Count;
            //delegates.Add((d, il, exp, type));
            //return i;
            var x = GCHandle.Alloc(new RuntimeMethod { 
                Method = d,
                IL = il,
                Exp = exp,
                Type = type
            });
            return (ulong)(IntPtr)x;
        }

        public object Create(Box[] boxes, ulong id)
        {
            //var (m, il, exp,t) = delegates[id];
            //return m.CreateDelegate(t, c);
            var rm = GCHandle.FromIntPtr((IntPtr)id).Target as RuntimeMethod;
            var c = new Closures(this, boxes, rm.IL, rm.Exp);
            return rm.Method.CreateDelegate(rm.Type, c);
        }
    }
}
