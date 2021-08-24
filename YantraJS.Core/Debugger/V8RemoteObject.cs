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
        public string Description { get; set; }
        public string SubType { get; set; }

        private static JSSymbol systemID = new JSSymbol("Debugger.ObjectID");

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
                Description = "undefined";
                return;
            }
            if (v.IsNull)
            {
                Type = "object";
                Value = "null";
                Description = "null";
                return;
            }
            if (v.IsString)
            {
                Type = "string";
                var t = v.ToString();
                Value = t;
                Description = t;
                return;
            }
            if (v.IsNumber)
            {
                Type = "number";
                var t = v.DoubleValue;
                Value = t;
                Description = t.ToString();
                return;
            }
            if (v.IsBoolean)
            {
                Type = "boolean";
                var t = v.BooleanValue;
                Value = t;
                Description = t ? "true" : "false";
                return;
            }

            Type = "object";
            var id = v[systemID];
            if (id.IsUndefined)
            {
                var idStr = GCHandle.ToIntPtr(GCHandle.Alloc(v, GCHandleType.Normal)).ToInt64().ToString();
                v[systemID] = new JSString(idStr);
                ObjectId = idStr;
            }
            else
            {
                ObjectId = id.ToString();
            }

            switch (v)
            {
                case JSContext:
                    ClassName = "global";
                    Description = "global";
                    break;
                case JSError:
                    SubType = "error";
                    Description = "Error";
                    break;
                case JSArray:
                    SubType = "array";
                    Description = "Array";
                    break;
                default:
                    Description = "Object";
                    break;
            }

            var p = v.prototypeChain?.@object;
            if(p != null)
            {
                var c = p[KeyStrings.constructor];
                if (!c.IsNullOrUndefined)
                {
                    ClassName = c[KeyStrings.name].ToString();
                    Description = ClassName;
                }
            }
        }
    }
}
