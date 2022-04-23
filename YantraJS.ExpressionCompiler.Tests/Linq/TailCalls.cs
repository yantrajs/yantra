using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using YantraJS.Expressions;
using YantraJS.Generator;
using YantraJS.Runtime;

namespace YantraJS.Linq
{
    [TestClass]
    public class TailCalls {

        [TestMethod]
        public void Simple() {

            var a = YExpression.Parameters(typeof(int));

            MethodInfo processMethod = typeof(TailCalls).GetMethod(nameof(TailCalls.Process));

            var body = YExpression.Lambda<Func<int, int>>("tail",
                YExpression.Call(null, processMethod, a[0])
                , a);

            ILCodeGenerator.GenerateLogs = true;

            var fx = body.CompileInAssembly();
            // var fx = body.Compile();

            var r = fx(1);
            Assert.AreEqual(2, r);

        }


    public static int Process(int a)
        {
            return a + 1;
        }
    }
}
