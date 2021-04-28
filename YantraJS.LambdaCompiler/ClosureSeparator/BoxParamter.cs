using System;
using YantraJS.Expressions;

namespace YantraJS
{
    public class BoxParamter
    {
        public Type Type;
        public int Index;
        public YParameterExpression Parameter;
        internal bool Create;
        internal YExpression Expression;
        internal YExpression Parent;
        internal YExpression ParentParameter;
    }
}
