#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace YantraJS.Expressions
{
    public class YLambdaExpression: YExpression
    {
        public readonly string Name;
        public readonly YExpression Body;
        public readonly YParameterExpression[] Parameters;
        public readonly Type ReturnType;
        public readonly Type[] ParameterTypes;
        internal readonly YExpression? Repository;

        public Type DelegateType
        {
            get
            {
                List<Type> types = new List<Type>();
                types.AddRange(ParameterTypes);
                types.Add(ReturnType ?? Type);
                return System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());
            }
        }

        public YLambdaExpression(string name, 
            YExpression body, 
            IList<YParameterExpression>? parameters,
            Type? returnType = null,
            YExpression? repository = null)
            : base(YExpressionType.Lambda, body.Type)
        {
            this.Name = name;
            this.Body = body;
            this.ReturnType = returnType ?? body.Type;
            if (parameters != null)
                this.Parameters = parameters.ToArray();
            else
                this.Parameters = new YParameterExpression[] { };
            ParameterTypes = this.Parameters.Select(x => x.Type).ToArray();
            this.Repository = repository;
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write('(');
            writer.Write(string.Join(", ", Parameters.Select(p => $"{p.Type.FullName} {p.Name}") ));
            writer.Write(") => ");

            Body.Print(writer);
        }
    }
}