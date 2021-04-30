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
        public new readonly YParameterExpression[] Parameters;
        public readonly Type ReturnType;
        public readonly Type[] ParameterTypes;
        internal readonly YExpression? Repository;

        public YLambdaExpression(string name, 
            YExpression body, 
            YParameterExpression[]? parameters,
            Type? returnType = null,
            YExpression? repository = null)
            : base(YExpressionType.Lambda, Create(parameters, body.Type))
        {
            this.Name = name;
            this.Body = body;
            this.ReturnType = returnType ?? body.Type;
            if (parameters != null)
                this.Parameters = parameters;
            else
                this.Parameters = new YParameterExpression[] { };
            ParameterTypes = this.Parameters.Select(x => x.Type).ToArray();
            this.Repository = repository;
        }

        public Type ThisDelegateType
        {
            get
            {
                List<Type> types = new List<Type>(ParameterTypes.Skip(1));
                types.Add(ReturnType);
                return System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());
            }
        }

        private static Type Create(YParameterExpression[]? parameters, Type type)
        {
            List<Type> types = new List<Type>((parameters?.Length ?? 0) + 1);
            if(parameters != null)
            {
                foreach (var p in parameters)
                    types.Add(p.Type);
            }

            types.Add(type);
            return System.Linq.Expressions.Expression.GetDelegateType(types.ToArray());
        }

        public override void Print(IndentedTextWriter writer)
        {
            writer.Write('(');
            writer.Write(string.Join(", ", Parameters.Select(p => $"{p.Type.GetFriendlyName()} {p.Name}") ));
            writer.Write(") => ");

            Body.Print(writer);
        }

        internal YLambdaExpression PrefixParameter(Type type)
        {

            var pp = YExpression.Parameter(type);

            var pl = new List<YParameterExpression>() { pp };
            foreach(var p in Parameters)
            {
                pl.Add(p);
            }

            return new YLambdaExpression(Name, Body, pl.ToArray(), ReturnType, Repository);
        }
    }
}