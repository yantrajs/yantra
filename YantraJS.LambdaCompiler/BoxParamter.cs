using System;
using System.Linq.Expressions;

namespace YantraJS
{
    public class BoxParamter
    {
        public Type Type;
        public int Index;
        public ParameterExpression Parameter;
        internal bool Create;
        internal MemberExpression Expression;
        internal Expression Parent;
    }
}
