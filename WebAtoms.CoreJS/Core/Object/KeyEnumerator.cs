using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core.Enumerators
{
    public struct KeyEnumerator : IElementEnumerator
    {
        private JSObject target;
        private bool showEnumerableOnly;
        private bool inherited;
        private KeyEnumerator? parent;
        IElementEnumerator elements;
        PropertySequence.ValueEnumerator? properties;

        public KeyEnumerator(JSObject jSObject, bool showEnumerableOnly, bool inherited)
        {
            this.target = jSObject;
            this.elements = jSObject.GetElementEnumerator();
            this.properties = jSObject.ownProperties != null
                ? new PropertySequence.ValueEnumerator(jSObject, showEnumerableOnly)
                : (PropertySequence.ValueEnumerator?)null;
            this.showEnumerableOnly = showEnumerableOnly;
            this.inherited = inherited;
            parent = null;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (this.elements != null)
            {
                if (this.elements.MoveNext(out var hasValueout, out var _, out var ui)) {
                    value = new JSNumber(ui);
                    hasValue = true;
                    index = ui;
                    return true;
                }
                this.elements = null;
            }
            if(this.properties != null)
            {
                if (this.properties.Value.MoveNext(out var key)) {
                    value = key.ToJSValue();
                    hasValue = true;
                    index = 0;
                    return true;
                }
                this.properties = null;
                if (this.inherited)
                {
                    if(target.prototypeChain != null && target.prototypeChain != target)
                    {
                        parent = new KeyEnumerator(target.prototypeChain, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if(parent.Value.MoveNext(out hasValue, out value, out index))
                {
                    return true;
                }
                parent = null;
            }
            hasValue = false;
            value = null;
            index = 0;
            return false;
        }

        public bool MoveNext(out JSValue value)
        {
            throw new NotImplementedException();
        }
    }
}
