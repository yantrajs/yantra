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

            // RunTest("Objects\\Array\\slice.js");
            // RunTest("Objects\\Object\\entries.js");

            RunTest("es6/Syntax/for-of/array.js");

        }

    }
}
