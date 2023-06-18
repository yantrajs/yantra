using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YantraJS.Core.Weak;
using YantraJS.Tests.Core;

namespace YantraJS.Core.Tests.Core.GCTests
{
    [TestClass]
    public class FinalizationTests : BaseTest
    {

        [TestMethod]
        public void FinalizeTest()
        {
            var success = false;

            var a = new Arguments(JSUndefined.Value, new JSFunction((in Arguments a) =>
            {
                success = true;
                return JSUndefined.Value;
            }));
            var fr = new JSFinalizationRegistry(a);

            Register(fr);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(success);

        }

        private static void Register(JSFinalizationRegistry fr)
        {
            fr.Register(new Arguments(fr, new JSObject(), new JSString("a")));
        }
    }
}
