using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core;

namespace YantraJS.Utils
{
    [TestClass]
    public class StringSpanTests
    {
        [TestMethod]
        public void Trim()
        {
            StringSpan s = " a ";
            var t = s.Trim();
            Assert.AreEqual("a", t.Value);

            s = "a ";
            t = s.Trim();
            Assert.AreEqual("a", t.Value);

            s = " a";
            t = s.Trim();
            Assert.AreEqual("a", t.Value);

            s = "a";
            t = s.Trim();
            Assert.AreEqual("a", t.Value);
        }
    }

    [TestClass]
    public class SequenceTests
    {

        [TestMethod]
        public void Simple()
        {
            var sequence = new Sequence<int>() { 
                1,
                2,
                3,
                4
            };

            Assert.AreEqual("1,2,3,4", sequence.Description);
            
        }

        [TestMethod]
        public void Insert()
        {
            var sequence = new Sequence<int>(2) {
                2,
                3,
                4
            };

            sequence.Insert(0, 1);

            Assert.AreEqual("1,2,3,4", sequence.Description);

        }

        [TestMethod]
        public void ThreeLevels()
        {
            var sequence = new Sequence<int>();
            var sb = new StringBuilder();
            for (int i = 0; i < 20; i++)
            {
                sequence.Add(i);
                if (i != 0)
                    sb.Append(',');
                sb.Append(i);
            }
            Assert.AreEqual(sb.ToString(), sequence.Description);

        }
    }
}
