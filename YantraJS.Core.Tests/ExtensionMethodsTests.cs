using Microsoft.VisualStudio.TestTools.UnitTesting;
using YantraJS.Core;
using YantraJS.Core.Clr;

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
        [TestMethod]
        public void ExtentsionMethodCallFromObject()
        {
            JSContext context = new JSContext();
            context["objectSome"] = new SomeBaseClass().Marshal();
            var value = context["objectSome"]["jsFuncEx"].InvokeFunction(Arguments.Empty);
            Assert.AreEqual("jsFuncDelegate", value.ToString());
            value = context.FastEval("objectSome.jsFuncEx()");
            Assert.AreEqual("jsFuncDelegate", value.ToString());
        }
        [TestMethod]
        public void ExtentsionMethodCallFromType()
        {
            JSContext context = new JSContext();
            context["typeSome"] = typeof(SomeBaseClass).Marshal();
            var constructor = context["typeSome"] as ClrType;
            var @object = constructor.Create(Arguments.Empty);
            var value = @object["jsFuncEx"].InvokeFunction(Arguments.Empty);
            Assert.AreEqual("jsFuncDelegate", value.ToString());
            value = context.FastEval("new typeSome().jsFuncEx()");
            Assert.AreEqual("jsFuncDelegate", value.ToString());
        }
        [TestMethod]
        public void NotJsFuncDelegateExtensionMethodMustCall()
        {
            JSContext context = new JSContext();
            context["typeSome"] = typeof(SomeBaseClass).Marshal();
            context["objectSome"] = new SomeBaseClass().Marshal();
            var val1 = context.FastEval("objectSome.cSharpFunc('love')");
            Assert.AreEqual("csharplove", val1.ToString());
            var val2 = context.FastEval("new typeSome().cSharpFunc('love')");
            Assert.AreEqual("csharplove", val2.ToString());
        }
        [TestMethod]
        public void CsharpFuncProperlyCallWithManyArgs()
        {
            JSContext context = new JSContext();
            context["typeSome"] = typeof(SomeBaseClass).Marshal();
            context["objectSome"] = new SomeBaseClass().Marshal();
            
            //FIXME integers mapped as doubles
            var val1 = context.FastEval("objectSome.sum(1, 2, 3)");
            Assert.AreEqual("6", val1.ToString());
            var val2 = context.FastEval("new typeSome().sum(1, 2, 3)");
            Assert.AreEqual("6", val2.ToString());
        }
        [TestMethod]
        public void CsharpFuncWithNotPrimetive()
        {
            JSContext context = new JSContext();
            context["typeSome"] = typeof(SomeBaseClass).Marshal();
            context["objectSome"] = new SomeBaseClass().Marshal();
            var val1 = context.FastEval("objectSome.twoSharp(objectSome)");
            Assert.AreEqual("csharp+jsFuncDelegate", val1.ToString());
            var val2 = context.FastEval("new typeSome().twoSharp(objectSome)");
            Assert.AreEqual("csharp+jsFuncDelegate", val2.ToString());
        }
        
        [TestMethod]
        public void TryConvertDontBroken()
        {
            JSContext context = new JSContext();
            context["objectSome"] = new SomeBaseClass().Marshal();
            if (!context["objectSome"].ConvertTo<SomeBaseClass>(out var @class))
            {
                Assert.Fail();
            }
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
            return "csharp" + love;
        }

        public static double Sum(this SomeBaseClass @class, double one, double two, double three)
        {
            return one + two + three;
        }

        public static string twoSharp(this SomeBaseClass @class, SomeBaseClass other)
        {
            return @class.cSharpFunc("+") + other.jsFuncEx(Arguments.Empty);
        }
        
        
    }
}