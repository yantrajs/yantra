using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Core.Generator
{
    public struct JSIterator : IElementEnumerator
    {
        private JSValue iterator;
        private uint index;

        public JSIterator(JSValue iterator)
        {
            this.iterator = iterator;
            this.index = 0;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            value = iterator.InvokeMethod(KeyStrings.next);
            var done = value[KeyStrings.done];
            value = value[KeyStrings.value];
            if (done.BooleanValue)
            {
                index = 0;
                hasValue = false;
                return false;
            }
            index = this.index++;
            hasValue = true;
            return true;
        }

        public bool MoveNext(out JSValue value)
        {
            value = iterator.InvokeMethod(KeyStrings.next);
            var done = value[KeyStrings.done];
            value = value[KeyStrings.value];
            if (done.BooleanValue)
            {
                return false;
            }
            return true;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            value = iterator.InvokeMethod(KeyStrings.next);
            var done = value[KeyStrings.done];
            if (done.BooleanValue)
            {
                value = @default;
                return false;
            }
            value = value[KeyStrings.value];
            return true;
        }

        public JSValue NextOrDefault(JSValue @default)
        {
            var value = iterator.InvokeMethod(KeyStrings.next);
            var done = value[KeyStrings.done];
            if (done.BooleanValue)
            {
                return @default;
            }
            return value[KeyStrings.value];
        }

    }
}
