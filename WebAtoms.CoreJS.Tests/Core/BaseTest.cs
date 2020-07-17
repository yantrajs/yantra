using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core
{
    public class BaseTest: IDisposable
    {
        protected JSContext context;

        public BaseTest()
        {
            context = new JSContext();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
