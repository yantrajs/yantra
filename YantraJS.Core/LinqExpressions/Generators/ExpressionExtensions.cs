using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace YantraJS.Core.LinqExpressions.Generators
{
    public static class ExpressionExtensions
    {
        private static ExpressionValue Default = new ExpressionValue();
        private static ConditionalWeakTable<Expression, ExpressionValue> storage = new ConditionalWeakTable<Expression, ExpressionValue>();

        public static bool ForceBreak(this Expression exp)
        {
            return GetExtendedValue(exp).ForceBreak;
        }

        public static ExpressionValue GetExtendedValue(this Expression exp, ExpressionValue def = null)
        {
            if (storage.TryGetValue(exp, out var v))
                return v ?? Default;
            return def ?? Default;
        }

        public static void SetExtendedValue(this Expression exp, ExpressionValue value)
        {
            storage.Remove(exp);
            if (value != null)
            {
                storage.Add(exp, value);
            }
        }

        public static void UpdateExtendedValue(this Expression exp, Action<ExpressionValue> action)
        {
            if (storage.TryGetValue(exp, out var e))
            {
                action(e);
                return;
            }
            e = new ExpressionValue();
            action(e);
            storage.Add(exp, e);
        }


        public static void ClearExtendedValue(this Expression exp)
        {
            storage.Remove(exp);
        }

    }
}
