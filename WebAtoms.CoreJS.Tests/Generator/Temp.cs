using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebAtoms.CoreJS.Tests.Core;

namespace WebAtoms.CoreJS.Tests.Generator
{
    [TestClass]
    public class Temp: BaseTest
    {

        private void RunTest(string file)
        {
            var fileObj = new System.IO.FileInfo("../../../Generator/Files/" + file);
            var text = System.IO.File.ReadAllText(fileObj.FullName);
            CoreScript.Evaluate(text, fileObj.FullName);
        }


        [TestMethod]
        public void Try() {

            // pending new object initialization

            RunTest("es5\\Array\\Typed\\set.js");
            // RunTest("Objects\\Object\\entries.js");

            // RunTest("es6/Syntax/for-of/array.js");
            // RunTest("es6/Syntax/destructuring/parameter.js");

            // RunTest("es5/Function/fib.js");

            // RunTest("es5/Syntax/for.js");

            // RunTest("es5/Syntax/Variables/let.js");
            // RunTest("es5/Function/hoisting.js");


        }

    }
}
