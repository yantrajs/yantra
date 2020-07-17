using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public struct JSProperty
    {

        public static JSProperty Empty = new JSProperty() { IsEmpty = true };
        public JSString key;
        public JSValue get;
        public JSValue set;

        public JSValue value;

        public bool configurable;

        public bool enumerable;

        public bool IsEmpty { get; private set; }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSProperty Function(JSFunctionDelegate d)
        {
            return new JSProperty {
                value = new JSFunction(d)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSProperty Property(JSFunctionDelegate get, JSFunctionDelegate set)
        {
            return new JSProperty
            {
                get = new JSFunction(get),
                set = new JSFunction(set)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSProperty Property(JSValue value)
        {
            return new JSProperty
            {
                value = value
            };
        }

    }
}
