using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS.Runtime
{
    public class YantraMethodBuilder : IMethodBuilder
    {
        private List<object> list = new List<object>();

        public YExpression Relay(YExpression @this, IFastEnumerable<YExpression> closures, YLambdaExpression innerLambda)
        {
            throw new NotImplementedException();
        }
    }

    public class YantraMethod
    {
        public readonly YantraMethodBuilder List;
        public readonly Box[] Boxes;
        public readonly string IL;
        public readonly string Exp;

        public YantraMethod(
            YantraMethodBuilder list = null,
            Box[] boxes = null,
            string il = null,
            string exp = null)
        {
            this.List = list ?? new YantraMethodBuilder();
            this.Boxes = boxes;
            this.IL = il;
            this.Exp = exp;
        }

    }
}
