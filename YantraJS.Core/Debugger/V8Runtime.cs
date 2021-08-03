using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace YantraJS.Core.Debugger
{

    public partial class V8Runtime : V8ProtocolObject
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

        public V8ReturnValue GetProperties(GetPropertiesParams args)
        {
            try
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
                        list.Add(new V8PropertyDescriptor(KeyStrings.GetNameString(p.key).Value, v, in p, v == c));
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

                return new V8ReturnValue
                {
                    Result = list
                };
            } catch (Exception ex)
            {
                return ex;
            }
        }

        public V8ReturnValue GetIsolateId() {
            return new V8ReturnValue { 
                Id = inspectorContext.ID
            };
        }

        public V8ReturnValue CallFunctionOn(CallFunctionOnParams a)
        {

            if (!inspectorContext.Contexts.TryGetValue(a.ExecutionContextId, out var c))
            {
                return new ArgumentOutOfRangeException($"Context not found");
            }

            try {

                var fx = CoreScript.Compile(a.FunctionDeclaration);
                var previous = JSContext.Current;
                try {
                    JSContext.Current = c;
                    JSValue @this = a.ObjectId != null ? V8RemoteObject.From(a.ObjectId) : c;
                    var length = a.Arguments.Count;
                    var args = new JSValue[length];
                    for (int i = 0; i < length; i++)
                    {
                        args[i] = a.Arguments[i].ToJSValue();
                    }
                    return fx(new Arguments(@this, args));

                } finally {
                    JSContext.Current = previous;
                }


            } catch(Exception ex)
            {
                return ex;
            }

        }

        private static long nextInjectedId = 1;

        public V8ReturnValue CompileScript(CompileScriptParams a)
        {
            try {
                var injectedId = $"I-{Interlocked.Increment(ref nextInjectedId)}";
                var fx = CoreScript.Compile(a.Expression);
                inspectorContext.Scripts.Add(injectedId, fx);
                return new V8ReturnValue { 
                    ScriptId = injectedId
                };
            } catch(Exception ex)
            {
                return ex;
            }
        }

        public V8ReturnValue Evaluate(EvaluateParams a)
        {
            if(!inspectorContext.Contexts.TryGetValue(a.ContextId, out var c))
            {
                return new ArgumentOutOfRangeException($"{a.ContextId} context not found");
            }

            try
            {

                var fx = CoreScript.Compile(a.Expression);
                var previous = JSContext.Current;
                try
                {
                    JSContext.Current = c;
                    JSValue @this = c;
                    return fx(new Arguments(@this));
                }
                finally
                {
                    JSContext.Current = previous;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public V8ReturnValue RunScript(RunScriptArgs a)
        {
            if (!inspectorContext.Scripts.TryGetValue(a.ScriptId, out var script))
                return new ArgumentOutOfRangeException($"Script not found");

            JSContext c;

            if (a.ExecutionContextId != null)
            {
                if (!inspectorContext.Contexts.TryGetValue(a.ExecutionContextId, out c))
                    return new ArgumentOutOfRangeException($"Context not found");
            } else
            {
                c = inspectorContext.Contexts.Values.First();
            }
            
            try
            {
                var prev = JSContext.Current;
                try
                {
                    JSContext.Current = c;
                    return script(Arguments.Empty);
                } finally {
                    JSContext.Current = prev;
                }
            } catch (Exception ex)
            {
                return ex;
            }

        }

        
    }


}
