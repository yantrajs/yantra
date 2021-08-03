using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core.Core;
using YantraJS.Extensions;

namespace YantraJS.Core.Debugger
{
    public class V8PropertyDescriptor
    {
        public V8PropertyDescriptor(JSPrototype prototypeChain)
        {
            this.Name = "__proto__";
            this.Enumerable = false;
            this.Configurable = true;
            this.IsOwn = true;
            this.Writable = true;
            this.Value = new V8RemoteObject(prototypeChain.@object);
        }

        public V8PropertyDescriptor(string name, JSValue v, in JSProperty p, bool isOwn = false)
        {
            this.Name = name;
            this.Writable = !p.IsReadOnly;
            this.Configurable = p.IsConfigurable;
            this.Enumerable = p.IsEnumerable;
            this.IsOwn = isOwn;
            if (!p.IsProperty)
            {
                try {
                    Value = new V8RemoteObject(v.GetValue(p));
                } catch (Exception ex)
                {
                    Value = new V8RemoteObject(ex.ToString());
                    WasThrown = true;
                }
                return;
            }
            if (p.get != null)
                this.Get = new V8RemoteObject(p.get);
            if (p.set != null)
                this.Set = new V8RemoteObject(p.set);
        }

        public string Name { get; set; }
        public bool Writable { get; set; }
        public bool Configurable { get; set; }
        public bool Enumerable { get; set; }
        public bool IsOwn { get; set; }
        public V8RemoteObject Value { get; set; }
        public bool WasThrown { get; set; }
        public V8RemoteObject Get { get; set; }
        public V8RemoteObject Set { get; set; }
    }
}
