using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Debug;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS.Utils
{
    //public class JSConsole: JSObject
    //{
    //    public JSConsole(): base(JSContext.Current.ObjectPrototype)
    //    {
            
    //    }
    //}


    public class JSTestContext: JSContext
    {
        public JSTestContext()
        {
            this.CreateSharedObject(KeyStrings.assert, typeof(JSAssert), true);
        }

    }
}
