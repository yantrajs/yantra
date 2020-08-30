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
            var text = System.IO.File.ReadAllText("../../../Generator/Files/" + file);
            CoreScript.Evaluate(text);
        }


        [TestMethod]
        public void Try() {

            RunTest("Statements\\Switch.js");

        }

    }
}
