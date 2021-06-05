using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Tests.Core
{
    [TestClass]
    public class StringSpanTests
    {

        [TestMethod]
        public void Test1()
        {
            StringSpan s1 = StringSpan.Empty;

            Assert.AreEqual(0, s1.GetHashCode());

            StringSpan s2 = null;
            Assert.AreEqual(0, s2.GetHashCode());
        }

    }
}
