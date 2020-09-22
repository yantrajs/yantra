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
            var text = System.IO.File.ReadAllText("../../../Generator/Files/es5/" + file);
            CoreScript.Evaluate(text);
        }


        [TestMethod]
        public void Try() {

            // pending new object initialization

            // RunTest("Function\\fib.js");

            RunTest("Function\\class.js");

        }

    }
}
