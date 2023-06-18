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
        Dictionary<int, object> dictionary = new Dictionary<int, object>();

        /// <summary>
        /// Indexer must be exported
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [JSExport]
        public object this[int index]
        {
            get => dictionary.TryGetValue(index, out var v) ? v : null;
            set => dictionary[index] = value;
        }


        /// <summary>
        /// JavaScriptObject must have a constructor with in Arguments a
        /// </summary>
        /// <param name="a"></param>
        public CustomObject(in Arguments a): base(a) {
            this.Name = a[0]?.ToString() ?? "Not Specified";
            this.Version = 4;
        }

        [JSExportSameName]
        public static int NONE = 0;

        [JSExport("name")]
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

                assert.strictEqual(CustomObject.NONE,0);

                assert.throws(() => {
                    a.print2(2);
                });

                a[1] = 'a';
                assert.strictEqual(a[1],'a');
            ");
        }

        [TestMethod]
        public void TestNamingConvention()
        {
            var c = new JSTestContext();
            c.ClrMemberNamingConvention = ClrMemberNamingConvention.Declared;
            c["CustomObject"] = ClrType.From(typeof(CustomObject2));


            c.Eval(@"
                var a = new CustomObject('b');
                assert.strictEqual(a.Name, 'b');
                assert.strictEqual(a.Version, undefined);
            ");
        }
    }

    public class CustomObject2
    {
        public string Name { get; }

        public CustomObject2(string a)
        {
            this.Name = a;
        }
    }

}
