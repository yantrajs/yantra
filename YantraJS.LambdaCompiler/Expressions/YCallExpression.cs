#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YantraJS.Expressions
{
    public class YCallExpression: YExpression
    {
        public readonly YExpression? Target;
        public readonly MethodInfo Method;
        public readonly YExpression[] Arguments;

        public YCallExpression(YExpression? target, MethodInfo method, IList<YExpression> args)
            : base(YExpressionType.Call, method.ReturnType)
        {
            this.Target = target;
            this.Method = method;
            this.Arguments = args.ToArray();
        }
    }
}