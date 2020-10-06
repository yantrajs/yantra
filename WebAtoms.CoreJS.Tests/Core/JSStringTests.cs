using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core
{
    [TestClass]
    public class JSStringTests: BaseTest
    {
        [TestMethod]
        public void Index()
        {
            CoreScript.Evaluate(@"var a = 'akash';
assert(a[1] === 'k', a[1]); ");
        }

        [TestMethod]
        public void Substring()
        {
            var js = new JSString("akash");

            Assert.AreEqual(5, js.Length);

            var jsLength = js[KeyStrings.length];

            Assert.AreEqual(5, jsLength.IntValue);

            var zero = new JSNumber(0);
            var length = new JSNumber(2);
            var prefix = js.InvokeMethod("substr", new Arguments(js, zero, length));

            Assert.AreEqual(2, prefix.Length);

            Assert.AreEqual("ak", prefix.ToString());

        }

        [TestMethod]
        public void CodeCharAt()
        { }

        [TestMethod]
        public void CodePointAt()
        { }

        [TestMethod]
        public void Concat()
        { }

        [TestMethod]
        public void EndsWith()
        { }

        [TestMethod]
        public void Includes()
        { }

        [TestMethod]
        public void IndexOf()
        { }

        [TestMethod]
        public void LastIndexOf()
        { }

        [TestMethod]
        public void Match()
        { }

        [TestMethod]
        public void MatchAll()
        { }

        [TestMethod]
        public void Normalize()
        { }

        [TestMethod]
        public void PadEnd()
        { }

        [TestMethod]
        public void PadStart()
        { }

        [TestMethod]
        public void Repeat()
        { }

        [TestMethod]
        public void Replace()
        { }

        [TestMethod]
        public void ReplaceAll()
        { }

        [TestMethod]
        public void Search()
        { }

        [TestMethod]
        public void Slice()
        { }

        [TestMethod]
        public void Split()
        { }

        [TestMethod]
        public void ToLocaleLowerCase()
        { }

        [TestMethod]
        public void ToLocaleUpperCase()
        { }

        [TestMethod]
        public void ToLowerCase()
        { }

        [TestMethod]
        public void ToUpperCase()
        { }

        [TestMethod]
        public void Trim()
        { }

        [TestMethod]
        public void TrimEnd()
        { }

        [TestMethod]
        public void TrimStart()
        { }

        [TestMethod]
        public void ValueOf()
        { }

    }
}
