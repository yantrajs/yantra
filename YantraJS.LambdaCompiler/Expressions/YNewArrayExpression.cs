#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YNewArrayExpression: YExpression
    {
        public readonly YExpression[]? Elements;
        public readonly Type ElementType;

        public YNewArrayExpression(Type type, IList<YExpression>? elements = null)
            : base( YExpressionType.NewArray, type.MakeArrayType())
        {
            this.ElementType = type;
            if(elements == null)
            {
                Elements = null;
            } else
            {
                this.Elements = elements.ToArray();
            }
        }
    }
}