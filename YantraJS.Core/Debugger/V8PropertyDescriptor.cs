using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Extensions;

namespace YantraJS.Core.Debugger
{
    public class V8PropertyDescriptor
    {
        public V8PropertyDescriptor(string name, JSValue v, in JSProperty p, bool isOwn = false)
        {
            this.Name = name;
            this.Writable = !p.IsReadOnly;
            this.Configurable = p.IsConfigurable;
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
        }

        public string Name { get; set; }
        public bool Writable { get; set; }
        public bool Configurable { get; set; }
        public bool IsOwn { get; set; }
        public V8RemoteObject Value { get; set; }
        public bool WasThrown { get; set; }
    }
}
