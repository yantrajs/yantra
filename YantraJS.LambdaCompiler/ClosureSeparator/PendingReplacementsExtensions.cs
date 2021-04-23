using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace YantraJS
{
    public static class PendingReplacementsExtensions
    {

        private static ConditionalWeakTable<Expression, PendingReplacements> storage 
            = new ConditionalWeakTable<Expression, PendingReplacements>();

        public static PendingReplacements GetPendingReplacements(this Expression exp)
        {
            if(!storage.TryGetValue(exp, out var p))
            {
                p = new PendingReplacements { };
                storage.Add(exp, p);
            }
            return p;
        }

    }
}
