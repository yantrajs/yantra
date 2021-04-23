using System.Collections.Generic;
using System.Linq.Expressions;

namespace YantraJS
{
    public class PendingReplacements
    {
        public Dictionary<ParameterExpression, BoxParamter> Variables { get; }
            = new Dictionary<ParameterExpression, BoxParamter>();

    }
}
