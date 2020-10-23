using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Typed
{
    public static class TypedArrayPrototype
    {
        [Prototype("toString")]
        public static JSValue ToString(in Arguments a) {

            return new JSString(a.This.ToString());
        }
    }
}
