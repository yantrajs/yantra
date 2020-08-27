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

            context["assert"] = new JSFunction((t, a) => {
                var test = a[0];
                var message = a[1];
                message = message is JSUndefined ? new JSString("Assert failed, no message") : message;
                if (JSBoolean.IsTrue(test))
                    throw new JSException(message);
                return JSUndefined.Value;
            });
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
