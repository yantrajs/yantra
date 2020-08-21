using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace WebAtoms.CoreJS.Tests.CSharp
{
    [TestClass]
    public class CodeGenerator
    {

        [TestMethod]
        public void Generate()
        {

            var text = Literal("a\"b").ToFullString();

            Assert.AreEqual("\"a\\\"b\"", text);
        }

        //[TestMethod]
        //public void ClosureSample()
        //{
        //    var ctx = JSContext.Current;
        //    var f1 = new Closures.FX1((_, _) =>
        //    {

        //        var f2 = new Closures.FX2((_, _) => {
                    
        //            return JSUndefined.Value;
        //        }, "", "", f1);

        //        return f2;
        //    }, "", "");


        //}

        //internal static class Closures
        //{
        //    public class FX1: JSFunction
        //    {
        //        internal FX1(JSFunctionDelegate f, string name, string source) : base(f, name, source)
        //        {

        //        }
        //    }

        //    public class FX2 : JSFunction {

        //        public FX1 parent;

        //        internal FX2(
        //            JSFunctionDelegate f, 
        //            string name, 
        //            string source, FX1 parent) : base(f, name, source)
        //        {
        //            this.parent = parent;
        //        }
        //    }
        //}

    }
}
