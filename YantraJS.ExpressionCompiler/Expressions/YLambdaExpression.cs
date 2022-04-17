#nullable enable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace YantraJS.Expressions
{

    public class YLambdaExpression: YExpression
    {
        public readonly FunctionName Name;
        public readonly YExpression Body;
        public new readonly YParameterExpression[] Parameters;
        public readonly Type ReturnType;
        private YParameterExpression? _This;
        public YParameterExpression This => _This;

        internal YExpression<T> As<T>()
        {
            return new YExpression<T>(Name, Body, This, Parameters, ReturnType);
        }


        public readonly Type[] ParameterTypes;

        public Type[] ParameterTypesWithThis {
            get {
                var l = new List<Type> { This!.Type };
                l.AddRange(ParameterTypes);
                return l.ToArray();
            }
        }

        internal readonly YExpression? Repository;
            

        public YLambdaExpression(
            Type delegateType,
            in FunctionName name, 
            YExpression body, 
            YParameterExpression? @this,
            YParameterExpression[]? parameters,
            Type? returnType = null,
            YExpression? repository = null)
            : base(YExpressionType.Lambda, delegateType)
        {
            this.Name = name;
            this.Body = body;
            this._This = @this;
            this.ReturnType = returnType ?? body.Type;
            if (parameters != null)
                this.Parameters = parameters;
            else
                this.Parameters = new YParameterExpression[] { };
            ParameterTypes = this.Parameters.Select(x => x.Type).ToArray();
            this.Repository = repository;
        }
        public override void Print(IndentedTextWriter writer)
        {
            writer.Write('(');
            writer.Write(string.Join(", ", Parameters.Select(p => $"{p.Type.GetFriendlyName()} {p.Name}") ));
            writer.Write(") => ");

            Body.Print(writer);
        }

        internal void SetupAsClosure()
        {
            if (_This == null)
            {
                _This = YParameterExpression.Parameter(typeof(Closures), "this");
            }
        }

        internal YLambdaExpression WithThis(Type type)
        {
            if (This != null)
                throw new ArgumentOutOfRangeException();
            var @this = YExpression.Parameter(type, "this");

            return new YLambdaExpression(Type, Name, Body, @this, Parameters, ReturnType, Repository);
        }
    }
}