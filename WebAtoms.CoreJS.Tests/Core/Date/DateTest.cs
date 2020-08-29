using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core.Date
{
    [TestClass]
    public class DateTest: BaseTest
    {

        [TestMethod]
        public void Now()
        {

            var a = CoreScript.Evaluate("Date.now()");
            Assert.IsTrue(a is JSDate);
        }

    }
}
