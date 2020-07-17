using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public class JSObject : JSValue
    {

        public JSObject()
        {
            ownProperties = new BinaryUInt32Map<JSValue>();
        }

        public override JSValue this[JSValue key] {
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }
        public static void SetupPrototype(JSObject obj)
        {
            throw new NotImplementedException();
        }

    }
}
