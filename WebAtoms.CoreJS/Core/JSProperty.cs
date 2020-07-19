using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public struct JSProperty
    {

        public JSName key;
        public JSValue get;
        public JSValue set;

        public JSValue value;

        public bool configurable;

        public bool enumerable;

        public bool IsEmpty => get == null && set == null && value == null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Function(JSFunctionDelegate d)
        {
            return new JSProperty {
                value = new JSFunction(d)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(JSValue d)
        {
            return new JSProperty
            {
                value = d
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSProperty Property(JSFunctionDelegate get, JSFunctionDelegate set = null)
        {
            return new JSProperty
            {
                get = new JSFunction(get),
                set = set != null ? new JSFunction(set): null
            };
        }
    }
}
