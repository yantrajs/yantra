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
    public class CustomObject: JavaScriptObject
    {

        /// <summary>
        /// JavaScriptObject must have a constructor with in Arguments a
        /// </summary>
        /// <param name="a"></param>
        public CustomObject(in Arguments a): base(a) {
            this.Name = a[0]?.ToString() ?? "Not Specified";
            this.Version = 4;
        }

        [JSName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Hidden from JavaScript world...
        /// </summary>
        public int Version { get; set; }

        [JSExport]
        public JSValue Print(in Arguments a)
        {
            return a[0] ?? JSNull.Value;
        }

        [JSExport]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [JSExport]
        public void Log(string a)
        {
            System.Diagnostics.Debug.WriteLine(a);
        }

        /// <summary>
        /// Hidden from JS
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public object Print(object a)
        {
            return a;
        }

        /// <summary>
        /// Hidden from JS
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public object Print2(object a)
        {
            return a;
        }

    }

    [TestClass]
    public class CustomObjectTest
    {
        [TestMethod]
        public void Test()
        {
            var c = new JSTestContext();
            c["CustomObject"] = ClrType.From(typeof(CustomObject));


            c.Eval(@"
                var a = new CustomObject('b');
                assert.strictEqual(a.name, 'b');
                assert.strictEqual(a.version, undefined);

                assert.strictEqual(a.print(2), 2);
                assert.strictEqual(a.add(1,2), 3);

                a.log('b');

                assert.throws(() => {
                    a.print2(2);
                });
            ");
        }
    }
}
