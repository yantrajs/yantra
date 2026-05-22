using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Enumerators
{
    public interface IKeyEnumerator
    {
        bool MoveNext(out JSValue value);
    }

    public class ForInUIntEnumerator: IElementEnumerator
    {
        private readonly JSObject target;
        private uint start;
        private readonly uint length;

        public ForInUIntEnumerator(JSObject target) {
            this.target = target;
            this.start = 0;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            throw new NotImplementedException();
        }

        public bool MoveNext(out JSValue value)
        {
            if(!this.target.elements.TryGetValue(this.start, out var p))
            {
                value = default;
                return false;
            }
            if (p.IsEnumerable)
            {
                value = target.GetValue(p);
                return true;
            }
            value = default;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            throw new NotImplementedException();
        }

        public JSValue NextOrDefault(JSValue @default)
        {
            throw new NotImplementedException();
        }
    }

    public class PropertyEnumerator
    {
        readonly JSObject target;
        readonly bool showEnumerableOnly;
        readonly bool inherited;
        private PropertyEnumerator parent;
        PropertySequence.ValueEnumerator properties;

        public PropertyEnumerator(JSObject jSObject, bool showEnumerableOnly, bool inherited)
        {
            this.target = jSObject;
            ref var op = ref jSObject.GetOwnProperties(false);
            this.properties = !op.IsEmpty
                ? new PropertySequence.ValueEnumerator(jSObject, showEnumerableOnly)
                : new PropertySequence.ValueEnumerator();
            this.showEnumerableOnly = showEnumerableOnly;
            this.inherited = inherited;
            parent = null;
        }

        public bool MoveNextProperty(out KeyString key, out JSProperty value)
        {
            if (this.properties.target != null)
            {
                if (this.properties.MoveNextProperty(out value, out key))
                {
                    return true;
                }
                this.properties.target = null;
                if (this.inherited)
                {
                    var @base = target.prototypeChain?.@object;
                    if (@base != null
                        && @base != target)
                    {
                        parent = new PropertyEnumerator(@base, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if (parent.MoveNextProperty(out key, out value))
                {
                    return true;
                }
                parent = null;
            }
            key = 0;
            value = default;
            return false;
        }

        public bool MoveNext(out KeyString key, out JSValue value)
        {
            if (this.properties.target != null)
            {
                if (this.properties.MoveNext(out value, out key))
                {
                    return true;
                }
                this.properties.target = null;
                if (this.inherited)
                {
                    var @base = target.prototypeChain?.@object;
                    if (@base != null 
                        && @base != target)
                    {
                        parent = new PropertyEnumerator(@base, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if (parent.MoveNext(out key, out value))
                {
                    return true;
                }
                parent = null;
            }
            key = 0;
            value = null;
            return false;
        }
    }

    public class KeyEnumerator : IElementEnumerator
    {
        readonly JSObject target;
        readonly bool inherited;
        readonly bool showEnumerableOnly;
        private KeyEnumerator parent;
        IElementEnumerator elements;
        PropertySequence.ValueEnumerator properties;
        private int mask;

        public KeyEnumerator(JSObject jSObject, bool showEnumerableOnly, bool inherited)
        {
            this.target = jSObject;
            this.elements = jSObject.GetElementEnumerator();
            this.properties = new PropertySequence.ValueEnumerator(jSObject, showEnumerableOnly);
            this.mask = showEnumerableOnly ? (-1 & ~((int)JSPropertyAttributes.Enumerable)) : -1;
            this.inherited = inherited;
            parent = null;
        }

        public bool MoveNext(out bool hasValue, out JSValue value, out uint index)
        {
            if (this.elements != null)
            {
                if (this.elements.MoveNext(out var hasValueout, out var _, out var ui))
                {
                    value = new JSString(ui.ToString());
                    hasValue = hasValueout;
                    index = ui;
                    return true;
                }
                this.elements = null;
            }
            if (this.properties.target != null)
            {
                if (this.properties.MoveNext(out var key))
                {
                    value = key.ToJSValue();
                    hasValue = true;
                    index = 0;
                    return true;
                }
                this.properties.target = null;
                if (this.inherited)
                {
                    var @base = target.prototypeChain?.@object;
                    if (@base != null && @base != target)
                    {
                        parent = new KeyEnumerator(@base, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if (parent.MoveNext(out hasValue, out value, out index))
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
            if (this.elements != null)
            {
                if (this.elements.MoveNext(out var hasValueout, out var _, out var ui))
                {
                    value = new JSString(ui.ToString());
                    return true;
                }
                this.elements = null;
            }
            if (this.properties.target != null)
            {
                if (this.properties.MoveNext(out var key))
                {
                    value = key.ToJSValue();
                    return true;
                }
                this.properties.target = null;
                if (this.inherited)
                {
                    var @base = target.prototypeChain?.@object;
                    if (@base != null && @base != target)
                    {
                        parent = new KeyEnumerator(@base, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if (parent.MoveNext(out value))
                {
                    return true;
                }
                parent = null;
            }
            value = JSUndefined.Value;
            return false;
        }

        public bool MoveNextOrDefault(out JSValue value, JSValue @default)
        {
            if (this.elements != null)
            {
                if (this.elements.MoveNext(out var hasValueout, out var _, out var ui))
                {
                    value = new JSString(ui.ToString());
                    return true;
                }
                this.elements = null;
            }
            if (this.properties.target != null)
            {
                if (this.properties.MoveNext(out var key))
                {
                    value = key.ToJSValue();
                    return true;
                }
                this.properties.target = null;
                if (this.inherited)
                {
                    var @base = target.prototypeChain?.@object;
                    if (@base != null && @base != target)
                    {
                        parent = new KeyEnumerator(@base, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if (parent.MoveNext(out value))
                {
                    return true;
                }
                parent = null;
            }
            value = @default;
            return false;
        }

        public JSValue NextOrDefault(JSValue @default)
        {
            if (this.elements != null)
            {
                if (this.elements.MoveNext(out var hasValueout, out var _, out var ui))
                {
                    return new JSString(ui.ToString());
                }
                this.elements = null;
            }
            if (this.properties.target != null)
            {
                if (this.properties.MoveNext(out var key))
                {
                    return key.ToJSValue();
                }
                this.properties.target = null;
                if (this.inherited)
                {
                    var @base = target.prototypeChain?.@object;
                    if (@base != null && @base != target)
                    {
                        parent = new KeyEnumerator(@base, showEnumerableOnly, inherited);
                    }
                }
            }
            if (parent != null)
            {
                if (parent.MoveNext(out var value))
                {
                    return value;
                }
                parent = null;
            }
            return @default;
        }
    }
}
