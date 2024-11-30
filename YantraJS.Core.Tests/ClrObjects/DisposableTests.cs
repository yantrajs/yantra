using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        class AsyncDisposableFile : IAsyncDisposable
        {
            public bool Open = true;

            public string Value = "";

            [JSExport("add")]
            public void Add(string text)
            {
                this.Value += text + "\r\n";
            }

            public async ValueTask DisposeAsync()
            {
                await Task.Delay(10);
                this.Open = false;
            }
        }

        [TestMethod]
        public void SyncDispose()
        {
            var c = new JSTestContext();
            
            c["DisposableFile"] = ClrType.From(typeof(DisposableFile));


            var a = c.Eval(@"
                function use(d) {
                    using f = d;
                    f.add('a');
                }
                var a = new DisposableFile();
                use(a);
                return a;
            ");

            a.ConvertTo<DisposableFile>(out var d);
            Assert.IsFalse(d.Open);
        }

        [TestMethod]
        public void FieldAccess()
        {
            var c = new JSTestContext();
            JSValue guidTypeValue = ClrType.From(typeof(Guid));
            c["guid"] = guidTypeValue;
            string result = c.Eval("guid.empty.toString()").ToString();
            Assert.AreEqual("00000000-0000-0000-0000-000000000000", result);
        }

        // [TestMethod]
        public void AsyncDispose()
        {
            AsyncPump.Run(async () =>
            {
                var c = new JSTestContext();

                c["AsyncDisposableFile"] = ClrType.From(typeof(AsyncDisposableFile));


                var a = await c.ExecuteAsync(@"
                (async function() {
                async function use(d) {
                    await using f = d;
                    f.add('a');
                }
                var a = new AsyncDisposableFile();
                await use(a);
                return a;
                })();
            ");

                a.ConvertTo<AsyncDisposableFile>(out var d);
                Assert.IsFalse(d.Open);
            });
        }
    }
}
