using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core.Debugger
{

    public class V8Runtime : V8ProtocolObject
    {
        public V8Runtime(V8InspectorProtocol inspectorContext) : base(inspectorContext)
        {
        }

        public object Enable(JSObject none)
        {
            foreach (var entry in inspectorContext.Contexts)
            {
                var cid = entry.Key;
                inspectorContext.Send(new V8Runtime.ExecutionContextCreated
                {
                    Context = new V8Runtime.ExecutionContextDescription
                    {
                        Id = cid,
                        Name = cid,
                        UniqueId = cid
                    }
                });
            }
            return new { };
        }

        public class GetPropertiesParams { 
            public string ObjectId { get; set; }

            public bool ownProperties { get; set; }
        }

        public object GetProperties(GetPropertiesParams args)
        {
            var v = V8RemoteObject.From(args.ObjectId) as JSObject;
            var c = v;
            var list = new List<V8PropertyDescriptor>();
            do
            {
                ref var e = ref c.GetElements(false);
                for (uint i = 0; i < e.Length; i++)
                {
                    ref var p = ref e.Get(i);
                    if (p.IsEmpty)
                        continue;
                    list.Add(new V8PropertyDescriptor(i.ToString(), v, p, v == c));
                }
                c = v.prototypeChain?.@object;
            } while (!args.ownProperties && c != null);
            c = v;
            do
            {
                ref var ps = ref c.GetOwnProperties(false);
                foreach (var p in ps.properties)
                {
                    if (p.IsEmpty)
                        continue;
                    list.Add(new V8PropertyDescriptor(KeyStrings.GetNameString(p.key).Value,v, in p, v == c));
                }
                c = v.prototypeChain?.@object;
            } while (!args.ownProperties && c != null);
            //c = v;
            //do
            //{
            //    ref var ps = ref c.GetSymbols();
            //    foreach (var p in ps.AllValues)
            //    {
            //        if (p.IsEmpty)
            //            continue;
            //        list.Add(new V8PropertyDescriptor(KeyStrings.GetNameString(p.key).Value, v, in p, v == c));
            //    }
            //    c = v.prototypeChain?.@object;
            //} while (!args.ownProperties && c != null);

            return new { 
                result = list
            };
        }

        public class ConsoleApiCalled: V8ProtocolEvent
        {
            public string Type { get; set; }

            public List<V8RemoteObject> Args { get; set; }
            public long Timestamp { get; set; }
            public string ExecutionContextId { get; set; }

            public ConsoleApiCalled(string id, JSContext context, string type, in Arguments a)
            {
                ExecutionContextId = id;

                if (type== "error")
                {
                    // gather stack trace...
                    this.StackTrace = new V8StackTrace(context);
                }

                Args = V8RemoteObject.From(in a);

                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            public V8StackTrace StackTrace { get; set; }

            internal override string EventName => "Runtime.consoleApiCalled";
        }

        public class ExecutionContextCreated: V8ProtocolEvent
        {
            internal override string EventName => "Runtime.executionContextCreated";

            public ExecutionContextDescription Context { get; set; }
        }

        public class ExecutionContextDescription
        {
            public string Id { get; set; }

            public string Origin { get; set; } = "YantraJS";

            public string Name { get; set; }

            public string UniqueId { get; set; }            
        }

        public object GetIsolateId() {
            return new { 
                id = inspectorContext.ID
            };
        }
    }


}
