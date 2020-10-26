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
            return new JSGenerator(array.GetEntries());
        }

        [Prototype("every", Length = 0)]
        public static JSValue Every(in Arguments a) {

            var array = a.This.AsTypedArray();
            var first = a.Get1();
            if (!(first is JSFunction fn))
                throw JSContext.Current.NewTypeError($"First argument is not function");
            var en = array.GetElementEnumerator();
            while (en.MoveNext(out var hasValue, out var item, out var index))
            {
                var itemArgs = new Arguments(a.This, item, new JSNumber(index), array);
                if (!fn.f(itemArgs).BooleanValue)
                    return JSBoolean.False;
            }
            return JSBoolean.True;
        }
    }
}
