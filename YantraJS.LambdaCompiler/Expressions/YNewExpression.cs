using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YNewExpression: YExpression
    {
        public readonly ConstructorInfo constructor;
        public readonly YExpression[] args;

        public YNewExpression(ConstructorInfo constructor, IList<YExpression> args)
            : base(YExpressionType.New, constructor.DeclaringType)
        {
            this.constructor = constructor;
            this.args = args.ToArray();
        }
    }
}