using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core;
using YantraJS.Expressions;
using YantraJS.Generator;
using YantraJS.Runtime;

namespace YantraJS.Linq
{
    [TestClass]
    public class TryCatchTest
    {
        [TestMethod]
        public void Finally() {

            var i = YExpression.Parameter(typeof(int));

            var method = typeof(TryCatchTest).GetMethod(nameof(TryCatchTest.DoNothing));

            var @try = YExpression.TryCatchFinally(
                
                YExpression.Binary(i, YOperator.Multipley, i), 
                null,
                YExpression.Call(null, method)                
                );

            ILCodeGenerator.GenerateLogs = true;

            var r = YExpression.Lambda<Func<int,int>>("finallyTest",
                @try, new YParameterExpression[] { i });

            var fx = r.CompileInAssembly();

            Assert.AreEqual(4, fx(2));

        }

        public static void DoNothing()
        {

        }


        [TestMethod]
        public void Loop()
        {


            var i = YExpression.Parameter(typeof(int));
            var r = YExpression.Parameter(typeof(int));

            var @break = YExpression.Label();
            var one = YExpression.Constant(1);
            var three = YExpression.Constant(3);
            var @catch = YExpression.Catch(YExpression.GoTo(@break));

            var @throw = YExpression.New(typeof(Exception).GetConstructor(new Type[] { typeof(string) }),
                YExpression.Constant("a"));

            var tryCatch = YExpression.TryCatchFinally(
                    YExpression.Block(
                         YExpression.Conditional(
                             YExpression.Binary(i, YOperator.Greater, YExpression.Constant(5)),
                             YExpression.GoTo(@break), null),
                         YExpression.Assign(r, i),
                         YExpression.Assign(i, YExpression.Binary(i, YOperator.Add , one ) ),
                         YExpression.Conditional(
                             YExpression.Equal(i, three),
                             YExpression.Throw(@throw),
                             null
                             )
                    ),
                    @catch
                    );

            var loop = YExpression.Block( 
                (new YParameterExpression[] { i, r }).AsSequence(), 
                    YExpression.Loop(tryCatch,@break),
                    r);

            var lambda = YExpression.Lambda<Func<int>>("tryCatch", loop, new YParameterExpression[] { });

            var fx = lambda.Compile();

            Assert.AreEqual(2, fx());

            
        }

    }
}
