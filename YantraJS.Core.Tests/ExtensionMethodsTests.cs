using Microsoft.VisualStudio.TestTools.UnitTesting;
using YantraJS.Core;

namespace YantraJS.Tests
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void ExtensionMethodsMappingDontCrash()
        {
            JSContext context = new JSContext();
            context["typeSome"] = typeof(SomeBaseClass).Marshal();
            context["objectSome"] = new SomeBaseClass().Marshal();
        }
    }

    public class SomeBaseClass
    {
        
    }
    
    public static class SomeClassExtensions 
    {
        public static JSValue jsFuncEx(this SomeBaseClass @class, in Arguments a)
        {
            return "jsFuncDelegate".Marshal();
        }

        public static string cSharpFunc(this SomeBaseClass @cClass, string love)
        {
            return "charp" + love;
        }
        
        
    }
}