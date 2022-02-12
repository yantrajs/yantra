using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS
{
    public static class ClosureMarker
    {

        public static ConditionalWeakTable<YParameterExpression, string> isClosureCache
            = new ConditionalWeakTable<YParameterExpression, string>();

        public static ConditionalWeakTable<YLambdaExpression, Sequence<YParameterExpression>> isRelay
            = new ConditionalWeakTable<YLambdaExpression, Sequence<YParameterExpression>>();


        public static void MarkAsClosure(this YParameterExpression exp)
        {
            isClosureCache.Add(exp, "Yes");
        }

        public static bool IsClosure(this YParameterExpression exp)
        {
            return isClosureCache.TryGetValue(exp, out var _);
        }

        public static void MarkAsRelay(this YLambdaExpression exp)
        {
            isRelay.Add(exp, new Sequence<YParameterExpression>());
        }

        public static void AddToRelayParameter(this YLambdaExpression exp, YParameterExpression p)
        {
            if (!isRelay.TryGetValue(exp, out var list)) {
                throw new InvalidOperationException("Lambda isn't marked as a relay yet... ");
            }
            list.Add(p);
        }

        public static Sequence<YParameterExpression> GetRelayCaptures(this YLambdaExpression exp)
        {
            if (isRelay.TryGetValue(exp, out var list))
                return list;
            return null;
        }

        public static bool IsRelay(this YLambdaExpression exp)
        {
            return isRelay.TryGetValue(exp, out var _);
        }

    }
}
