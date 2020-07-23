using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        
        protected dynamic DynamicContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (dynamic)context;
        }
    }
}
