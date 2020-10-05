using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Maps
{
    [TestClass]
    public class BinaryMapEnumerateTests
    {

        [TestMethod]
        public void CharMap()
        {
            var a = new StringTrie<int>();

            a["a"] = 1;
            a["b"] = 2;
            a["aa"] = 3;
            a["bb"] = 4;

            var all = a.AllValues.ToList();
            Assert.AreEqual(4, all.Count);

            Assert.AreEqual("a", all[0].Key);
            Assert.AreEqual(1, all[0].Value);
            Assert.AreEqual("aa", all[1].Key);
            Assert.AreEqual(3, all[1].Value);
            Assert.AreEqual("b", all[2].Key);
            Assert.AreEqual(2, all[2].Value);
            Assert.AreEqual("bb", all[3].Key);
            Assert.AreEqual(4, all[3].Value);
        }

        [TestMethod]
        public void BigMap()
        {
            var a = new StringTrie<int>();
            a["toString"] = 1;
            a["constructor"] = 2;
            a["push"] = 3;
            Assert.AreEqual(2, a["constructor"]);
        }

        [TestMethod]
        public void IntMap()
        {
            var a = new UInt32Trie<int>();

            a[1] = 1;
            a[3] = 2;
            a[2] = 3;
            a[4] = 4;

            var all = a.AllValues.ToList();
            Assert.AreEqual(4, all.Count);

            Assert.AreEqual((uint)1, all[0].Key);
            Assert.AreEqual(1, all[0].Value);
            Assert.AreEqual((uint)2, all[1].Key);
            Assert.AreEqual(3, all[1].Value);
            Assert.AreEqual((uint)3, all[2].Key);
            Assert.AreEqual(2, all[2].Value);
            Assert.AreEqual((uint)4, all[3].Key);
            Assert.AreEqual(4, all[3].Value);
        }

        [TestMethod]
        public void CompactIntMap()
        {
            var a = new CompactUInt32Trie<int>();

            int max = 100;
            for (int i = max; i >= 0; i--)
            {
                a[(uint)i] = i;
            }

            for (int i = 0; i < max; i++)
            {
                Assert.AreEqual(a[(uint)i], i);
            }
        }

    }
}
