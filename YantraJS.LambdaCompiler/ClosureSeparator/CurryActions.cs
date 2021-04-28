#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using YantraJS.Expressions;

namespace YantraJS
{
    public static class CurryHelper
    {

        public static YExpression Create(
            string? name,
            IList<YExpression> closures,
            YParameterExpression closure,
            YParameterExpression[] parameters,
            YExpression body,
            YExpression? repository
            )
        {
            var methods = body.Type == typeof(void)
                ? CurryActions.methods
                : CurryFunctions.methods;

            var n = parameters.Length;

            if (n > 10)
                throw new NotSupportedException();

            var method = methods[n];

            var newParameterList = new List<YParameterExpression> { closure };
            var parameterTypes = new List<Type>();
            foreach(var p in parameters)
            {
                parameterTypes.Add(p.Type);
                newParameterList.Add(p);
            }

            if(body.Type != typeof(void))
            {
                parameterTypes.Add(body.Type);
            }

            if(parameterTypes.Count > 0)
                method = method.MakeGenericMethod(parameterTypes.ToArray());

            var lambda = YExpression.InlineLambda(name ?? "Unnamed", body, newParameterList, repository);

            var newArray = YExpression.NewArray(typeof(Box), closures);

            return YExpression.Call(null, method, newArray, lambda);

            // YExpression? call = null;
            //var @return = YExpression.Label("R", method.ReturnType);
            //setup.Add(YExpression.Return(@return, YExpression.Call(null, method, closure, lambda)));
            //setup.Add(YExpression.Label(@return));
            //return YExpression.Block(new YParameterExpression[] { closure }, setup);
        }

    }

    public class CurryActions
    {

        public static MethodInfo[] methods = new MethodInfo[] {
            typeof(CurryActions).GetMethod(nameof(Curry)),
            typeof(CurryActions).GetMethod(nameof(Curry1)),
            typeof(CurryActions).GetMethod(nameof(Curry2)),
            typeof(CurryActions).GetMethod(nameof(Curry3)),
            typeof(CurryActions).GetMethod(nameof(Curry4)),
            typeof(CurryActions).GetMethod(nameof(Curry5)),
            typeof(CurryActions).GetMethod(nameof(Curry6)),
            typeof(CurryActions).GetMethod(nameof(Curry7)),
            typeof(CurryActions).GetMethod(nameof(Curry8)),
            typeof(CurryActions).GetMethod(nameof(Curry9)),
            typeof(CurryActions).GetMethod(nameof(CurryA))
        };

        public static Action Curry(Box[] boxes, Action<Box[]> r)
            => () => r(boxes);

        public static Action<T1> Curry1<T1, T>(Box[] boxes, Action<Box[], T1> r)
            => (t1) => r(boxes, t1);

        public static Action<T1, T2> Curry2<T1, T2>(Box[] boxes, Action<Box[], T1, T2> r)
            => (t1, t2) => r(boxes, t1, t2);


        public static Action<T1, T2, T3> Curry3<T1, T2, T3>(Box[] boxes, Action<Box[], T1, T2, T3> r)
            => (t1, t2, t3) => r(boxes, t1, t2, t3);

        public static Action<T1, T2, T3, T4> Curry4<T1, T2, T3, T4>(Box[] boxes, Action<Box[], T1, T2, T3, T4> r)
            => (t1, t2, t3, t4) => r(boxes, t1, t2, t3, t4);

        public static Action<T1, T2, T3, T4, T5> Curry5<T1, T2, T3, T4, T5>(Box[] boxes, Action<Box[], T1, T2, T3, T4, T5> r)
            => (t1, t2, t3, t4, t5) => r(boxes, t1, t2, t3, t4, t5);

        public static Action<T1, T2, T3, T4, T5, T6>
            Curry6<T1, T2, T3, T4, T5, T6>(Box[] boxes,
            Action<Box[], T1, T2, T3, T4, T5, T6> r)
            => (t1, t2, t3, t4, t5, t6)
            => r(boxes, t1, t2, t3, t4, t5, t6);

        public static Action<T1, T2, T3, T4, T5, T6, T7>
            Curry7<T1, T2, T3, T4, T5, T6, T7>(Box[] boxes,
            Action<Box[], T1, T2, T3, T4, T5, T6, T7> r)
            => (t1, t2, t3, t4, t5, t6, t7)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7);

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8>
            Curry8<T1, T2, T3, T4, T5, T6, T7, T8>(Box[] boxes,
            Action<Box[], T1, T2, T3, T4, T5, T6, T7, T8> r)
            => (t1, t2, t3, t4, t5, t6, t7, t8)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7, t8);

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Box[] boxes,
            Action<Box[], T1, T2, T3, T4, T5, T6, T7, T8, T9> r)
            => (t1, t2, t3, t4, t5, t6, t7, t8, t9)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7, t8, t9);

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, Ta>
            CurryA<T1, T2, T3, T4, T5, T6, T7, T8, T9, Ta>(Box[] boxes,
            Action<Box[], T1, T2, T3, T4, T5, T6, T7, T8, T9, Ta> r)
            => (t1, t2, t3, t4, t5, t6, t7, t8, t9, ta)
            => r(boxes, t1, t2, t3, t4, t5, t6, t7, t8, t9, ta);

    }
}
