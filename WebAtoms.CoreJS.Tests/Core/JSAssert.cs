using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.Tests.Core
{
    public class JSAssert: JSObject
    {

        [Prototype("sameValue")]
        public static JSValue SameValue(JSValue t, JSArguments a)
        {
            var left = a[0];
            var right = a[1];
            var msgObj = a[2];
            if (!left.Equals(right).BooleanValue)
            {
                var msg = !(msgObj is JSUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

        [Prototype("notSameValue")]
        public static JSValue NotSameValue(JSValue t, JSArguments a)
        {
            var left = a[0];
            var right = a[1];
            var msgObj = a[2];
            if (left.Equals(right).BooleanValue)
            {
                var msg = !(msgObj is JSUndefined) ? msgObj.ToString() : $"Values {left},{right} are not same";
                throw new JSException(msg);
            }
            return JSUndefined.Value;
        }

    }
}
