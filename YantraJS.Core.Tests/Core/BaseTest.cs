using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core;
using YantraJS.Utils;

namespace YantraJS.Tests.Core
{
    public class BaseTest: IDisposable
    {
        protected JSContext context;

        public BaseTest()
        {
            context = new JSTestContext();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        
        protected dynamic DynamicContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (dynamic)context;
        }
    }
}
