using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Tests.Maps
{
    [TestClass]
    public class BinaryMapEnumerateTests
    {

        [TestMethod]
        public void CharMap()
        {
            var a = new StringMap<int>();
            // a["<"] = 0;
            a["a"] = 1;
            a["add"] = 5;
            a["aa"] = 3;
            a["b"] = 2;
            a["bb"] = 4;

            // var all = a.AllValues.ToList();
            // Assert.AreEqual(4, all.Count);
            Assert.AreEqual(1, a["a"]);
            Assert.AreEqual(2, a["b"]);
            Assert.AreEqual(3, a["aa"]);
            Assert.AreEqual(4, a["bb"]);
            Assert.AreEqual(0, a["ba"]);
            Assert.AreEqual(5, a["add"]);
            //Assert.AreEqual("a", all[0].Key);
            //Assert.AreEqual(1, all[0].Value);
            //Assert.AreEqual("aa", all[1].Key);
            //Assert.AreEqual(3, all[1].Value);
            //Assert.AreEqual("b", all[2].Key);
            //Assert.AreEqual(2, all[2].Value);
            //Assert.AreEqual("bb", all[3].Key);
            //Assert.AreEqual(4, all[3].Value);
        }

        [TestMethod]
        public void BigMap()
        {
            var a = new StringMap<int>();
            a["toString"] = 1;
            a["constructor"] = 2;
            a["push"] = 3;
            Assert.AreEqual(2, a["constructor"]);
        }

        [TestMethod]
        public void IntMap()
        {
            var a = new UInt32Map<int>();

            a[1] = 1;
            a[3] = 2;
            a[2] = 3;
            a[4] = 4;

            var all = a.AllValues().ToList();
            Assert.AreEqual(4, all.Count);

            //Assert.AreEqual((uint)1, all[0].Key);
            //Assert.AreEqual(1, all[0].Value);
            //Assert.AreEqual((uint)2, all[1].Key);
            //Assert.AreEqual(3, all[1].Value);
            //Assert.AreEqual((uint)3, all[2].Key);
            //Assert.AreEqual(2, all[2].Value);
            //Assert.AreEqual((uint)4, all[3].Key);
            //Assert.AreEqual(4, all[3].Value);
        }

        //[TestMethod]
        //public void CompactIntMap()
        //{
        //    var a = new CompactUInt32Trie<int>();

        //    int max = 100;
        //    for (int i = max; i >= 0; i--)
        //    {
        //        a[(uint)i] = i;
        //    }

        //    for (int i = 0; i < max; i++)
        //    {
        //        Assert.AreEqual(a[(uint)i], i);
        //    }
        //}


        [TestMethod]
        public void UIntMapTest()
        {
            var map = new UInt32Map<uint>();

            int max = 100;
            for (int i = max; i >= 0; i--)
            {
                map.Save((uint)i ,(uint)i);
            }

            for (uint i = 0; i < max; i++)
            {
                if (!map.TryGetValue(i, out var value))
                    Assert.Fail();
                Assert.AreEqual(i, value);
            }


            map = new UInt32Map<uint>();

            for (int i = 0; i < max; i++)
            {
                map.Save((uint)i, (uint)i);
            }

            for (uint i = 0; i < max; i++)
            {
                if (!map.TryGetValue(i, out var value))
                    Assert.Fail();
                Assert.AreEqual(i, value);
            }


        }
    }
}
