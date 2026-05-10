using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Tests.Core;

namespace YantraJS.Core.Tests.Core;

[TestClass]
public class JSErrorTests : BaseTest
{

    [TestMethod]
    public void Name()
    {
        using (var context = new JSContext())
        {
            try
            {
                context.Execute("throw new Error('Some error...');");
            }
            catch (JSException e)
            {
                var msg = e.Error[KeyString.stack].AsStringOrDefault();
                Assert.StartsWith(msg, "Error:");
            }
        }
    }
}
