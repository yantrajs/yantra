using System.Collections.Generic;
using System.Linq.Expressions;
using YantraJS.Expressions;

namespace YantraJS
{
    public class PendingReplacements
    {
        public Dictionary<YParameterExpression, BoxParamter> Variables { get; }
            = new Dictionary<YParameterExpression, BoxParamter>();

    }
}
