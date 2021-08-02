using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace YantraJS.Core.Debugger
{
    public class V8RemoteObject
    {
        public string Type { get; set; }
        public string ObjectId { get; set; }
        public object Value { get; set; }
        public string ClassName { get; set; }

        public static List<V8RemoteObject> From(in Arguments a)
        {
            var list = new List<V8RemoteObject>();
            for (int i = 0; i < a.Length; i++)
            {
                list.Add(new V8RemoteObject(a.GetAt(i)));
            }
            return list;
        }

        public static JSValue From(string id)
        {
            try
            {
                IntPtr v = new IntPtr(long.Parse(id));
                var h = GCHandle.FromIntPtr(v);
                if (h.IsAllocated)
                {
                    return h.Target as JSValue;
                }
            } catch (Exception)
            {
            }
            return JSUndefined.Value;
        }

        public V8RemoteObject(string v)
        {
            Type = "string";
            Value = v;
        }

        public V8RemoteObject(JSValue v)
        {
            if(v.IsUndefined)
            {
                Type = "undefined";
                return;
            }
            if (v.IsNull)
            {
                Type = "object";
                Value = "null";
                return;
            }
            if (v.IsString)
            {
                Type = "string";
                Value = v.ToString();
                return;
            }
            if (v.IsNumber)
            {
                Type = "number";
                Value = v.ToString();
                return;
            }
            if (v.IsBoolean)
            {
                Type = "boolean";
                Value = v.BooleanValue;
                return;
            }

            Type = "object";
            ObjectId = GCHandle.ToIntPtr(GCHandle.Alloc(v, GCHandleType.Weak)).ToInt64().ToString();

            var p = v.prototypeChain?.@object;
            if(p != null)
            {
                var c = p[KeyStrings.constructor];
                if (!c.IsNullOrUndefined)
                {
                    ClassName = c[KeyStrings.name].ToString();
                }
            }
        }
    }
}
