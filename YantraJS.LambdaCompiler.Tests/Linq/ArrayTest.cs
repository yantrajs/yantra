using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YantraJS.Expressions;
using YantraJS.Runtime;

namespace YantraJS.Linq
{
    [TestClass]
    public class ArrayTest
    {

        [TestMethod]
        public void Load()
        {
            var args = YExpression.Parameters(typeof(int[]), typeof(int));

            var exp = YExpression.Lambda<Func<int[],int,int>>("load", YExpression.ArrayIndex(
                args[0],
                args[1]
                ), args);

            var fx = exp.CompileWithNestedLambdas();

            var n = new int[]{1,2,3 };

            Assert.AreEqual(1, fx(n, 0));
        }


        [TestMethod]
        public void Save()
        {


            var args = YExpression.Parameters(typeof(int[]), typeof(int), typeof(int));

            var ret = YExpression.Label("ret", typeof(int));

            var exp = YExpression.Lambda<Func<int[], int, int, int>>("save", 
                YExpression.Block( 
                    YExpression.Return(ret,
                        YExpression.Assign( 
                            YExpression.ArrayIndex(
                                args[0],
                                args[1]
                            ), 
                            args[2]
                        )
                    ),
                    YExpression.Label(ret, YExpression.Constant(0))
                ), args);

            var fx = exp.Compile();

            var n = new int[] { 1, 2, 3 };


            var m = fx(n, 1, 4);
            Assert.AreEqual(m, 4);
            Assert.AreEqual(n[1], 4);
        }

        [TestMethod]
        public void Struct() {


            var b = YExpression.Parameters(typeof(string));

            var indices = typeof(ScriptInfo).GetField(nameof(ScriptInfo.Indices));

            var getOrCreate = typeof(KeyString).GetMethod(nameof(KeyString.GetOrCreate));

            var create = YExpression.MemberInit(
                    YExpression.New(typeof(ScriptInfo)),
                    YExpression.Bind(indices,  
                        YExpression.NewArray(typeof(KeyString),
                            YExpression.Call(null, getOrCreate, b[0] )
                        )
                    ));

            var lambda = YExpression.Lambda<Func<string, ScriptInfo>>("c", create, b);

            var fx = lambda.CompileInAssembly();


            var r = fx("a");

            Assert.AreEqual(r.Indices[0].Value, "a");

        }


        public class ScriptInfo {

            public KeyString[] Indices;

        }

        public readonly struct KeyString {
            
            public readonly string Value;

            public KeyString(string value)
            {
                this.Value = value;
            }

            public static KeyString GetOrCreate(string a)
            {
                return new KeyString(a);
            }
        }

    }
}
