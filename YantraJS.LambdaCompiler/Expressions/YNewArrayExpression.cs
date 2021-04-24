#nullable enable
using System;
using System.Collections.Generic;

namespace YantraJS.Expressions
{
    public class YNewArrayExpression: YExpression
    {
        public readonly IList<YExpression>? Elements;

        public readonly int Size;

        public YNewArrayExpression(Type type, int size, IList<YExpression>? elements = null)
            : base( YExpressionType.NewArray, type.MakeArrayType())
        {
            this.Elements = elements;
            if(elements == null)
            {
                if (size == -1)
                    throw new ArgumentException($"{nameof(size)} cannot be less than zero");
            } else
            {
                if(size < elements.Count)
                    throw new ArgumentException($"{nameof(size)} cannot be less than number of elements");
            }
            this.Size = size;
        }
    }
}