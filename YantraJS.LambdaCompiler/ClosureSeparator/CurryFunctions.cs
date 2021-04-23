using System;
using System.Reflection;

namespace YantraJS
{
    public class CurryFunctions
    {
        public static MethodInfo[] methods = new MethodInfo[] {
            typeof(CurryFunctions).GetMethod(nameof(Curry)),
            typeof(CurryFunctions).GetMethod(nameof(Curry1)),
            typeof(CurryFunctions).GetMethod(nameof(Curry2)),
            typeof(CurryFunctions).GetMethod(nameof(Curry3)),
            typeof(CurryFunctions).GetMethod(nameof(Curry4)),
            typeof(CurryFunctions).GetMethod(nameof(Curry5)),
            typeof(CurryFunctions).GetMethod(nameof(Curry6)),
            typeof(CurryFunctions).GetMethod(nameof(Curry7)),
            typeof(CurryFunctions).GetMethod(nameof(Curry8)),
            typeof(CurryFunctions).GetMethod(nameof(Curry9)),
            typeof(CurryFunctions).GetMethod(nameof(CurryA))
        };

        public static Func<T> Curry<T>(Box[] boxes, Func<Box[], T> r)
            => () => r(boxes);

        public static Func<T1, T> Curry1<T1, T>(Box[] boxes, Func<Box[], T1, T> r)
            => (t1) => r(boxes, t1);

        public static Func<T1, T2, T> Curry2<T1, T2, T>(Box[] boxes, Func<Box[], T1, T2, T> r)
            => (t1, t2) => r(boxes, t1, t2);


        public static Func<T1, T2, T3, T> Curry3<T1, T2, T3, T>(Box[] boxes, Func<Box[], T1, T2, T3, T> r)
            => (t1, t2, t3) => r(boxes, t1, t2, t3);

        public static Func<T1, T2, T3, T4, T> Curry4<T1, T2, T3, T4, T>(Box[] boxes, Func<Box[], T1, T2, T3, T4, T> r)
            => (t1, t2, t3, t4) => r(boxes, t1, t2, t3, t4);

        public static Func<T1, T2, T3, T4, T5, T> Curry5<T1, T2, T3, T4, T5, T>(Box[] boxes, Func<Box[], T1, T2, T3, T4, T5, T> r)
            => (t1, t2, t3, t4,t5) => r(boxes, t1, t2, t3, t4, t5);

        public static Func<T1, T2, T3, T4, T5, T6, T>
            Curry6<T1, T2, T3, T4, T5, T6, T>(Box[] boxes, 
            Func<Box[], T1, T2, T3, T4, T5, T6, T> r)
            => (t1, t2, t3, t4, t5, t6) 
            => r(boxes, t1, t2, t3, t4, t5, t6);

        public static Func<T1, T2, T3, T4, T5, T6, T7, T>
            Curry7<T1, T2, T3, T4, T5, T6, T7, T>(Box[] boxes,
            Func<Box[], T1, T2, T3, T4, T5, T6, T7, T> r)
            => (t1, t2, t3, t4, t5, t6, t7)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7);

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T>
            Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T>(Box[] boxes,
            Func<Box[], T1, T2, T3, T4, T5, T6, T7, T8, T> r)
            => (t1, t2, t3, t4, t5, t6, t7, t8)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7, t8);

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>
            Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>(Box[] boxes,
            Func<Box[], T1, T2, T3, T4, T5, T6, T7, T8, T9, T> r)
            => (t1, t2, t3, t4, t5, t6, t7, t8, t9)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7, t8, t9);

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Ta, T>
            CurryA<T1, T2, T3, T4, T5, T6, T7, T8, T9, Ta, T>(Box[] boxes,
            Func<Box[], T1, T2, T3, T4, T5, T6, T7, T8, T9, Ta, T> r)
            => (t1, t2, t3, t4, t5, t6, t7, t8, t9, ta)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7, t8, t9, ta);

    }
}
