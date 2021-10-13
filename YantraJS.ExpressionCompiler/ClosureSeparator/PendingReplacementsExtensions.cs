using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using YantraJS.Expressions;

namespace YantraJS
{
    public static class PendingReplacementsExtensions
    {

        private static ConditionalWeakTable<YExpression, PendingReplacements> storage 
            = new ConditionalWeakTable<YExpression, PendingReplacements>();

        public static PendingReplacements GetPendingReplacements(this YExpression exp)
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
