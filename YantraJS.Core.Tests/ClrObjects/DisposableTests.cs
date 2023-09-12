using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core.Clr;
using YantraJS.Utils;

namespace YantraJS.Core.Tests.ClrObjects
{
    [TestClass]
    public class DisposableTests
    {
        class DisposableFile : IDisposable
        {
            public bool Open = true;

            public string Value = "";

            [JSExport("add")]
            public void Add(string text)
            {
                this.Value += text + "\r\n";
            }

            public void Dispose()
            {
                this.Open = false;
            }
        }

        [TestMethod]
        public void TestNamingConvention()
        {
            var c = new JSTestContext();
            
            c["DisposableFile"] = ClrType.From(typeof(CustomObject2));


            var a = c.Eval(@"
                function use(d) {
                    using f = d;
                    f.add('a');
                }
                var a = new DisposableFile('b');
                use(a);
                return a;
            ");

            a.ConvertTo<DisposableFile>(out var d);
            Assert.IsFalse(d.Open);
        }
    }
}
