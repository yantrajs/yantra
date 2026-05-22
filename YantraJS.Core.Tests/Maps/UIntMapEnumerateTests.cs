using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Tests.Maps
{
    [TestClass]
    public class UIntMapEnumerateTests
    {

        [TestMethod]
        public void IntMap()
        {
            var a = new Uint32Map<int>();

            a.Put(1) = 1;
            a.Put(3) = 2;
            a.Put(2) = 3;
            a.Put(4) = 4;

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
            var map = new Uint32Map<uint>();

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


            map = new Uint32Map<uint>();

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
