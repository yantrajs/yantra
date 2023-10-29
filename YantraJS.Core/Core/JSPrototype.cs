using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Core.Storage;
using YantraJS.Extensions;

namespace YantraJS.Core.Core
{
    

    public class JSPrototype
    {

        internal class JSPropertySet
        {
            internal SAUint32Map<(JSProperty property, JSPrototype owner)> properties;
            internal SAUint32Map<(JSProperty property, JSPrototype owner)> elements;
            internal SAUint32Map<(JSProperty property, JSPrototype owner)> symbols;

            internal Sequence<KeyString> stringKeys = new Sequence<KeyString>();
            internal Sequence<uint> uintKeys = new Sequence<uint>();
        }
        internal JSPropertySet propertySet;
        public readonly JSObject @object;
        private bool dirty = true;


        internal JSPrototype(JSObject @object)
        {
            this.@object = @object;

            this.Build();
        }

        private void Build()
        {
            
            if (!this.dirty)
                return;
            var ps = new JSPropertySet();
            lock (this)
            {
                if (!this.dirty)
                    return;
                ps.properties = new SAUint32Map<(JSProperty, JSPrototype)>();
                ps.elements = new SAUint32Map<(JSProperty, JSPrototype)>();
                ps.symbols = new SAUint32Map<(JSProperty, JSPrototype)>();

                Build(ps, this);
                dirty = false;
                this.propertySet = ps;
            }
        }

        private void Build(JSPropertySet ps, JSPrototype target)
        {
            // first build the base class for correct inheritance...

            var @object = target.@object;

            var @base = @object.prototypeChain;
            if (@base != null && @base != this)
            {
                this.Build(ps, @base);
            }

            // if it is registered, remove it first
            @object.PropertyChanged -= @object_PropertyChanged;

            @object.PropertyChanged += @object_PropertyChanged;
            ref var objectProperties = ref @object.GetOwnProperties(false);
            var ve = objectProperties.GetEnumerator(false);
            while(ve.MoveNext(out var key, out var value)){
                ps.properties.Put(key.Key) = (value.ToNotReadOnly(),target);
            }

            ref var objectElements = ref @object.GetElements(false);
            if (!objectElements.IsNull)
            {
                foreach(var e in objectElements.AllValues())
                {
                    if (!e.Value.IsEmpty)
                    {
                        ps.elements.Put(e.Key) = (e.Value.ToNotReadOnly(), target);
                    }
                }
            }

            ref var objectSymbols = ref @object.GetSymbols();
            if(!objectSymbols.IsNull)
            {
                foreach(var e in objectSymbols.AllValues())
                {
                    if (!e.Value.IsEmpty)
                    {
                        ps.symbols.Put(e.Key) = (e.Value.ToNotReadOnly(), target);
                    }
                }
            }
        }

        internal void Dirty()
        {
            this.dirty = true;
        }

        private void @object_PropertyChanged(JSObject sender, (uint keyString, uint index, JSSymbol symbol) index)
        {
            dirty = true;
        }

        internal JSProperty GetInternalProperty(in KeyString name)
        {
            this.Build();
            var (p, owner) = propertySet.properties[name.Key];
            return p;
        }

        internal JSProperty GetInternalProperty(uint name)
        {
            this.Build();
            return propertySet.elements[name].property;
        }

        internal JSProperty GetInternalProperty(JSSymbol symbol)
        {
            this.Build();
            return propertySet.symbols[symbol.Key].property;
        }

        internal JSFunctionDelegate GetMethod(in KeyString key)
        {
            this.Build();
            var (p, _) = propertySet.properties[key.Key];
            if(p.IsValue)
            {
                if (p.get != null)
                    return p.get.f;
            }
            if (p.IsProperty)
            {
                return p.get.f;
            }
            return null;
        }

        internal bool TryRemove(uint i, out JSProperty p)
        {
            if(propertySet.elements.TryGetValue(i, out var ee))
            {
                var @object = ee.owner.@object;
                ref var elements = ref @object.GetElements(false);
                return elements.TryRemove(i, out p);
            }
            p = JSProperty.Empty;
            return false;
        }

        public JSValue this[in KeyString k]
        {
            get
            {
                var p = GetInternalProperty(k);
                return @object.GetValue(p);
            }
        }
    }

}
