using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Debug;
using YantraJS.Extensions;

namespace YantraJS.Utils
{
    //public class JSConsole: JSObject
    //{
    //    public JSConsole(): base(JSContext.Current.ObjectPrototype)
    //    {
            
    //    }
    //}


    public class JSTestContext: JSContext
    {
        public JSTestContext(SynchronizationContext ctx = null): base(ctx)
        {
            // this.CreateSharedObject(KeyString.assert, typeof(JSAssert), true);
            this[KeyString.assert] = JSAssert.CreateClass(this, false);
            this[KeyString.global] = this;
            this[KeyString.clr] = ClrType.From(typeof(ClrModule));
        }

    }
}
