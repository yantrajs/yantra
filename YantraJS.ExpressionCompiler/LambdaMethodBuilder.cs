using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Generator;

namespace YantraJS
{
    public class LambdaMethodBuilder: IMethodBuilder
    {
        private readonly TypeBuilder typeBuilder;

        public LambdaMethodBuilder(MethodBuilder builder)
        {
            this.typeBuilder = (TypeBuilder)builder.DeclaringType;
        }

        public YExpression Relay(YExpression @this, IFastEnumerable<YExpression> closures, YLambdaExpression innerLambda)
        {

            var derived = (typeBuilder.Module as ModuleBuilder).DefineType(
                ExpressionCompiler.GetUniqueName(innerLambda.Name + ":" + innerLambda.Name.Line),
                TypeAttributes.Public,
                typeof(Closures));

            var (m, il, exp) = innerLambda.CompileToInstnaceMethod(derived, false);


            var cnstr = derived.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] {
                typeof(Box[])
            });

            var boxes = YExpression.Parameter(typeof(Box[]));

            // exp = null;
            // il = null;

            var cnstrLambda = YExpression.Lambda(innerLambda.Type, "cnstr",
                YExpression.CallNew(Closures.constructor, YExpression.Null, boxes, YExpression.Null, YExpression.Null),
                new YParameterExpression[] { YExpression.Parameter(derived), boxes });

            var cnstrIL = new ILCodeGenerator( cnstr.GetILGenerator(), null);
            cnstrIL.EmitConstructor(cnstrLambda);

            var dt = innerLambda.Type;

            var cdt = dt.GetConstructors().First(x => x.GetParameters().Length == 2);

            var cd = typeof(MethodInfo).GetMethod(nameof(MethodInfo.CreateDelegate), new Type[] { typeof(Type), typeof(object) });

            var derivedType = derived.CreateTypeInfo();
            var ct = derivedType.GetConstructors()[0];

            var im = derivedType.GetMethods().First(x => x.Name == m.Name);

            //var create = YExpression.Lambda("Create",

            //    YExpression.New(cdt,
            //        YExpression.New(cnstr, YExpression.Null),
            //        YExpression.Constant(im))
            //    , new YParameterExpression[] { });

            return YExpression.New(cdt, YExpression.New(ct,
                closures == null ? YExpression.Null : (YExpression)YExpression.NewArray(typeof(Box), closures)
                ),
                YExpression.Constant(im));

        }
    }
}
 