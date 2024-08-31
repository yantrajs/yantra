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

        public object Enable()
        {
            foreach (var entry in inspectorContext.Contexts)
            {
                var cid = entry.Key;
                inspectorContext.Send(new V8Runtime.ExecutionContextCreated
                {
                    Context = new V8Runtime.ExecutionContextDescription
                    {
                        Id = entry.Value.ID,
                        Name = cid.ToString(),
                        UniqueId = cid.ToString()
                    }
                });
            }
            return new { };
        }

        public V8ReturnValue GetProperties(GetPropertiesParams args)
        {
            try
            {
                var value = V8RemoteObject.From(args.ObjectId);
                var v = value as JSObject;
                var c = v;
                var list = new List<V8PropertyDescriptor>();

                // get elements

                if (args.ownProperties)
                {
                    ref var e = ref c.GetElements(false);
                    for (uint i = 0; i < e.Length; i++)
                    {
                        ref var p = ref e.Get(i);
                        if (p.IsEmpty)
                            continue;
                        list.Add(new V8PropertyDescriptor(i.ToString(), v, p, true));
                    }

                    var en = new PropertySequence.PropertyEnumerator(c.GetOwnProperties(), false);
                    while (en.MoveNext(out var key, out var p))
                    {
                        list.Add(new V8PropertyDescriptor(KeyStrings.GetNameString(key.Key).Value, v, p, true));
                    }
                    list.Add(new V8PropertyDescriptor(c.prototypeChain));
                } else
                {
                    // list all accessors...
                    var accessors = c.prototypeChain.propertySet;
                    foreach(var (i, (pt,px)) in accessors.properties.AllValues())
                    {
                        if (pt.IsEmpty || !pt.IsProperty)
                            continue;
                        list.Add(new V8PropertyDescriptor(KeyStrings.GetNameString(pt.key).Value, v, pt));
                    }

                    ref var p = ref JSContext.Current.ObjectPrototype.GetOwnProperties(false).GetValue(KeyStrings.__proto__.Key);
                    list.Add(new V8PropertyDescriptor("__proto__", c, p));
                }


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

                // return new ArgumentOutOfRangeException($"Context not found");
                c = inspectorContext.Contexts.Values.First();
            }

            try {

                var fx = CoreScript.Compile(a.FunctionDeclaration, codeCache: c.CodeCache);
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
                    return fx(Arguments.Empty).InvokeFunction(new Arguments(@this, args));

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
                inspectorContext.InjectedScripts.Add(injectedId, fx);
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

            if(a.ThrowOnSideEffect && a.Expression == "(async function(){ await 1; })()")
            {
                // return an error...
                return new JSEvalError(new Arguments(JSUndefined.Value, new JSString("Has Side Effects")));
            }

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
            if (!inspectorContext.InjectedScripts.TryGetValue(a.ScriptId, out var script))
                return new ArgumentOutOfRangeException($"Script not found");

            JSContext c;

            if (a.ExecutionContextId > 0)
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
