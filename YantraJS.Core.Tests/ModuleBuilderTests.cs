using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YantraJS.Core;

namespace YantraJS.Tests
{
    [TestClass]
    public class ModuleBuilderTests
    {
        public JSValue TestMethod(in Arguments a)
        {
            return new JSString("bar");
        }
        private readonly JSModuleContext _context;

        public ModuleBuilderTests()
        {
            _context = new JSModuleContext();
        }

        [TestMethod]
        public async Task ModuleBuilderShouldWord()
        {
            _context.RegisterModule("test", builder =>
            {
                builder.ExportType<TestClass>()
                    .ExportFunction("testFunc", TestMethod);
            } );
            var barfromclass = await _context
                .RunScriptAsync("import {testFunc} from \"test\"\n export default testFunc();",
                    String.Empty);
            if (barfromclass is JSString str)
            {
                Assert.AreEqual("bar", barfromclass.StringValue);
            }
            
        }
        
    }
    public class TestClass
    {
        public string Foo() => "bar";
    }
}