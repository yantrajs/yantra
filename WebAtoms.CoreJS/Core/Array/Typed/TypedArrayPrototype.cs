using System;
using System.Collections.Generic;
using System.Text;
using WebAtoms.CoreJS.Core.Generator;

namespace WebAtoms.CoreJS.Core.Typed
{
    public static class TypedArrayPrototype
    {
        [Prototype("toString")]
        public static JSValue ToString(in Arguments a) {

            return new JSString(a.This.ToString());
        }

        [Prototype("copyWithin", Length = 2)]
        public static JSValue CopyWithin(in Arguments a) {
            var(target, start, end) = a.Get3();
            throw new NotImplementedException();
        }


        [Prototype("entries", Length = 0)]
        public static JSValue Entries(in Arguments a)
        {
            var array = a.This.AsTypedArray();
            return new JSGenerator(array.GetElementEnumerator());
        }

    }
}
