using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YantraJS.ModuleExtensions;
using YantraJS.Core;

namespace YantraJS.Tests
{
    [TestClass]
    public class ModuleBuilderTests
    {
        public static JSValue TestMethod(in Arguments a)
        {
            return "bar".Marshal();
        }

        public ModuleBuilderTests()
        {


        }

        [TestMethod]
        public async Task ModuleBuilderExportClassShouldWork()
        {
            JSModuleContext context = new JSModuleContext();
            context.CreateModule("test", x => x.ExportType<TestClass>());
            var str = await context.RunScriptAsync("import {TestClass} from 'test' \nexport default new TestClass()",
                String.Empty);
            if (str.ConvertTo<TestClass>(out var @class))
            {
                Console.WriteLine(@class.Foo());
            }
            else
            {
                Assert.Fail();
            }
        }
        

        [TestMethod]
        public async Task SyntheticDefaultMethod()
        {
            JSModuleContext context = new JSModuleContext();
            context.CreateModule("test", x => x.ExportType<TestClass>());
            var val1 = await context.RunScriptAsync("import Test from 'test' \nexport default new Test.TestClass().foo()", string.Empty);
            var val2 = await context.RunScriptAsync("import * as Test from 'test' \nexport default new Test.TestClass().foo()", string.Empty);
            var val1str = val1[KeyStrings.@default] as JSString;
            var val2str = val2[@KeyStrings.@default] as JSString;
            Assert.AreEqual(val1[KeyStrings.@default] as JSString, val2[@KeyStrings.@default] as JSString);
        }


        [TestMethod]
        public async Task ExportFunctionShouldWork()
        {
            JSModuleContext context = new JSModuleContext();
            context.CreateModule("test", x => x.ExportFunction("lmao", TestMethod));
            var modulereturn = await context.RunScriptAsync("import {lmao} from 'test' \nexport default lmao()",
                String.Empty);
            if (modulereturn[KeyStrings.@default] is JSString str)
            {
                Assert.AreEqual("bar", str.ToString());
            }
            else
            {
                Assert.Fail();
            }
        }
        
        [TestMethod]
        public async Task ExportValueShouldWork()
        {
            TestClass @class = new TestClass();
            JSModuleContext context = new JSModuleContext();
            context.CreateModule("test", x => x.ExportValue("t", @class));
            var modulereturn = await context.RunScriptAsync("import {t} from 'test' \nexport default t.prop",
                String.Empty);
            if (modulereturn[KeyStrings.@default] is JSString str)
            {
                Assert.AreEqual("prop", str.ToString());
            }
            else
            {
                Assert.Fail();
            }
        }
        
        [TestMethod]
        public void ImportModuleThrowException()
        {
            JSModuleContext context = new JSModuleContext();
            Assert.ThrowsException<ArgumentException>((() => context.ImportModule("test")));
        }
        
        [TestMethod]
        public void ImportModuleFindModule()
        {
            JSModuleContext context = new JSModuleContext();
            context.CreateModule("test", x => x.ExportType<TestClass>());
            JSValue val = context.ImportModule("test");
            Assert.IsNotNull(val);
        }

        public class TestClass
        {
            public string Prop { get; set; } = "prop";
            public string Foo()
            {
                return "foo";
            }
        }
    }
}