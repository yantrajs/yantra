using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YantraJS.Tests.Core;

namespace YantraJS.Tests.Generator
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

            RunTest("es5\\Array\\Typed\\reduceRight.js");
            // RunTest("Objects\\Object\\entries.js");

            // RunTest("es6/Syntax/for-of/array.js");
            // RunTest("es6/Syntax/destructuring/parameter.js");

            // RunTest("es5/Function/fib.js");

            // RunTest("es5/Syntax/for.js");

            // RunTest("es5/Syntax/TryCatch/try.js");

            // RunTest("es5/Syntax/Variables/let.js");
            // RunTest("es5/Function/inheritance.js");

            // RunTest("es5/Objects/Object/mixed.js");

            // RunTest("es5/Function/parameters.js");

            // RunTest("es5/Array/from.js");

            // RunTest("es5/Array/slice.js");

            // RunTest("es6/Syntax/class/properties.js");


        }

    }
}
