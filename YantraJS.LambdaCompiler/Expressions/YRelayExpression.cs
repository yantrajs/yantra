﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace YantraJS.Expressions
{
    public class YRelayExpression: YExpression
    {
        public readonly YExpression[] Closures;
        public readonly YLambdaExpression InnerLambda;

        public YRelayExpression(YExpression[] closures, YLambdaExpression inner)
            : base(YExpressionType.Relay, PrepareType(inner))
        {
            this.Closures = closures;
            this.InnerLambda = inner;
        }

        private static Type PrepareType(YLambdaExpression inner)
        {
            List<Type> types = new List<Type>();
            var pa = inner.Parameters;
            for (int i = 0; i < pa.Length; i++)
            {
                if (i == 0)
                    continue;
                types.Add(pa[i].Type);
            }
            types.Add(inner.ReturnType);
            return System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write("relay([");
            writer.PrintCSV(Closures);
            writer.Write("], ");
            InnerLambda.Print(writer);
            writer.Write(")");
        }
    }
}