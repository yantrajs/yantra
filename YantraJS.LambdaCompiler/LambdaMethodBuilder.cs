﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using YantraJS.Expressions;

namespace YantraJS
{
    public class LambdaMethodBuilder: IMethodBuilder
    {
        private readonly TypeBuilder typeBuilder;

        public LambdaMethodBuilder(MethodBuilder builder)
        {
            this.typeBuilder = (TypeBuilder)builder.DeclaringType;
        }


        public YExpression Create(string name, YLambdaExpression lambdaExpression)
        {

            var ptypes = lambdaExpression.Parameters.Select(x => x.Type).ToArray();

            name = ExpressionCompiler.GetUniqueName(name);
            var m = typeBuilder.DefineMethod(
                name, 
                System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public,
                lambdaExpression.ReturnType,
                ptypes);

            ExpressionCompiler.InternalCompileToMethod(lambdaExpression, m);

            var plist = new List<Type>(ptypes);
            plist.Add(m.ReturnType);

            // we have to create a delegate as a static field...

            var dt = Expression.GetDelegateType(plist.ToArray());

            var factory = ExpressionCompiler.GetUniqueName(name + "_Factory");

            var fld = typeBuilder.DefineMethod(factory, 
                System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static,
                dt, new Type[] { });


            var il = fld.GetILGenerator();
            il.Emit(OpCodes.Ldftn, m);
            il.Emit(OpCodes.Newobj, dt);
            il.Emit(OpCodes.Ret);

            return YExpression.Call(null, fld);

        }
    }
}