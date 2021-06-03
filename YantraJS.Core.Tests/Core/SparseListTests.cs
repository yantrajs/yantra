using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YantraJS.Core;

namespace YantraJS.Parser.Tests
{

    public static class ToStringExtensions
    {
        public static string ToCSV<T>(this IList<T> list)
        {
            var sb = new StringBuilder();
            foreach(var item in list)
            {
                sb.Append(item);
                sb.Append(',');
            }
            if (sb.Length == 0)
                return string.Empty;
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }

    [TestClass]
    public class SparseListTests
    {
        [TestMethod]
        public void Test()
        {

            var list = new SparseList<int>();

            list.Add(1);
            Assert.AreEqual(1, list.Count);

            list.Insert(9, 1);
            Assert.AreEqual(10, list.Count);

            Assert.AreEqual(1, list[9]);

            var a = list.ToArray();
            Assert.AreEqual(10, a.Length);

            // Assert.AreEqual("1,0,0,0,0,0,0,0,0,1", list.ToCSV());
            Assert.AreEqual("1,0,0,0,0,0,0,0,0,1", a.ToCSV());
        }

        [TestMethod]
        public void AddRange()
        {
            var list = new SparseList<int>();
            list.AddRange(new int[] { 1, 2, 3 });
            Assert.AreEqual("1,2,3", list.ToCSV());
        }

        [TestMethod]
        public void Remove()
        {
            var list = new SparseList<int>() { 
                1,2,3
            };

            list.Remove(2);
            Assert.AreEqual("1,3", list.ToCSV());
        }

        [TestMethod]
        public void Insert()
        {
            var list = new SparseList<int>() {
                2,3
            };
            list.Insert(0, 1);
            Assert.AreEqual("1,2,3", list.ToCSV());
        }

        [TestMethod]
        public void InsertAtEnd()
        {
            var list = new SparseList<int>() {
                1,2
            };
            list.Insert(2, 3);
            Assert.AreEqual("1,2,3", list.ToCSV());
        }
    }
}
