using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Expressions;

namespace YantraJS.Linq
{
    [TestClass]
    public class NestedStructTest
    {

        [TestMethod]
        public void Struct()
        {


            var b = YExpression.Parameters(typeof(string));

            var indices = typeof(ScriptInfo).GetField(nameof(ScriptInfo.Indices));

            var getOrCreate = typeof(KeyString).GetMethod(nameof(KeyString.GetOrCreate));

            var ns = YExpression.New(typeof(StringSpan), b[0]);

            var create = YExpression.MemberInit(
                    YExpression.New(typeof(ScriptInfo)),
                    YExpression.Bind(indices,
                        YExpression.NewArray(typeof(KeyString),
                            YExpression.Call(null, getOrCreate, ns)
                        )
                    ));

            var lambda = YExpression.Lambda<Func<string, ScriptInfo>>("c", create, b);

            var fx = lambda.CompileInAssembly();


            var r = fx("a");

            Assert.AreEqual(r.Indices[0].Value.Span, "a");

        }


        public class ScriptInfo
        {

            public KeyString[] Indices;

        }

        public readonly struct StringSpan {
            public readonly string Span;

            public StringSpan(string span)
            {
                this.Span = span;
            }
        }

        public readonly struct KeyString
        {

            public readonly StringSpan Value;

            public KeyString(in StringSpan value)
            {
                this.Value = value;
            }

            public static KeyString GetOrCreate(in StringSpan a)
            {
                return new KeyString(a);
            }
        }

    }

}

