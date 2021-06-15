using Microsoft.VisualStudio.TestTools.UnitTesting;
using YantraJS.Core;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Tests
{
    [TestClass]
    public class BinaryMapTests
    {
        [TestMethod]
        public void CharTest()
        {
            var tm = new StringMap<string>();

            var i1 = "k1";
            var i2 = "k2";

            tm[i1] = "a";
            tm[i2] = "b";

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
            var im = new UInt32Map<string>();

            var i1 = (uint)4;
            var i2 = (uint)687;

#pragma warning disable CS0618 // Type or member is obsolete
            im[i1] = "a";
            im[i2] = "b";
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.AreEqual("a", im[i1]);
            Assert.AreEqual("b", im[i2]);
            Assert.IsNull(im[680]);
            Assert.IsNull(im[3]);



            im.RemoveAt(i2);

            Assert.IsNull(im[i2]);
        }
    }
}
