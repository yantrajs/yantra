using Microsoft.VisualStudio.TestTools.UnitTesting;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Tests
{
    [TestClass]
    public class SATrieMapTests
    {
        [TestMethod]
        public void CharTest()
        {
            var tm = new StringMap<string>();

            var i1 = "k1";
            var i2 = "k2";

#pragma warning disable CS0618 // Type or member is obsolete
            tm[i1] = "a";
            tm[i2] = "b";
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.AreEqual("a", tm[i1]);
            Assert.AreEqual("b", tm[i2]);
            Assert.IsNull(tm["k3"]);
            Assert.IsNull(tm["k"]);



            tm.RemoveAt(i2);

            Assert.IsNull(tm["k2"]);
        }

        [TestMethod]
        public void IntTest()
        {
            var im = new SAUint32Map<string>();

            var i1 = (uint)4;
            var i2 = (uint)687;

            im.Put(i1) = "a";
            im.Put(i2) = "b";

            Assert.AreEqual("a", im[i1]);
            Assert.AreEqual("b", im[i2]);
            Assert.IsNull(im[680]);
            Assert.IsNull(im[3]);



            im.RemoveAt(i2);

            Assert.IsNull(im[i2]);
        }

        [TestMethod]
        public void UIntMapTest()
        {
            var map = new SAUint32Map<uint>();

            int max = 100;
            for (int i = max; i >= 0; i--)
            {
                map.Save((uint)i, (uint)i);
            }

            for (uint i = 0; i < max; i++)
            {
                if (!map.TryGetValue(i, out var value))
                    Assert.Fail($"Could not get value for {i}");
                Assert.AreEqual(i, value);
            }


            map = new SAUint32Map<uint>();

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
