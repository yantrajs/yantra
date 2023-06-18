using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Debugger;

namespace YantraJS.Core.Debugger
{
    public delegate Task<string> MessageProcessor(long id, JsonNode p);

    public abstract class V8InspectorProtocol : JSDebugger, IDisposable
    {
        private static System.Text.Json.JsonSerializerOptions options =
            new JsonSerializerOptions {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
       //     new JsonSerializerSettings {
        
       //     ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
       //     {
       //         NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
       //     },
       //     NullValueHandling = NullValueHandling.Ignore

       //};

        public abstract void Dispose();
        public abstract Task ConnectAsync();

        private Dictionary<string, MessageProcessor> protocols
            = new Dictionary<string, MessageProcessor>();

        public string ID { get; }

        private static long id = 0;

        public Dictionary<long, JSContext> Contexts = new Dictionary<long, JSContext>();

        public Dictionary<string, JSFunctionDelegate> InjectedScripts = new Dictionary<string, JSFunctionDelegate>();

        private static long nextScriptId = 1;

        public Dictionary<string, string> Scripts = new Dictionary<string, string>();

        public V8InspectorProtocol()
        {

            ID = $"D-{Interlocked.Increment(ref id)}";

            var runtime = new V8Runtime(this);
            var debugger = new V8Debugger(this);

            Add("Runtime", runtime);
            Add("Debugger", debugger);
        }

        internal void AddContext(JSContext a)
        {
            var cid = $"C-{a.ID}";
            Contexts[a.ID] = a;
            a.ConsoleEvent += OnConsoleEvent;
        }

        private void OnConsoleEvent(JSContext context, string type, in Arguments a)
        {
            var id = $"C-{context.ID}";
            this.Send(new V8Runtime.ConsoleApiCalled(id, context, type, in a));
        }

        void Add(string prefix, V8ProtocolObject p)
        {
            foreach (var m in p.GetType().GetMethods())
            {
                var name = $"{prefix}.{m.Name.ToCamelCase()}";
                var pl = m.GetParameters();
                if (typeof(Task).IsAssignableFrom(m.ReturnType))
                {
                    if(pl.Length == 0)
                    {
                        //protocols[name] = createEmptyAsyncMethod.MakeGenericMethod(m.ReturnType.GetGenericArguments()[0])
                        //    .Invoke(null, new object[] { p, m }) as MessageProcessor;
                        protocols[name] = Generic.InvokeAs(m.ReturnType.GetGenericArguments()[0], CreateEmptyAsync<object>, m);
                        continue;
                    }
                    //protocols[name] = createAsyncMethod.MakeGenericMethod(pl[0].ParameterType, m.ReturnType.GetGenericArguments()[0])
                    //    .Invoke(null, new object[] { p, m }) as MessageProcessor;
                    protocols[name] = Generic.InvokeAs(
                        pl[0].ParameterType, m.ReturnType.GetGenericArguments()[0], CreateAsync<object,object>, m);
                    continue;
                }
                if(pl.Length == 0)
                {
                    protocols[name] = Generic.InvokeAs(
                        m.ReturnType, CreateEmpty<object>, p, m);
                    // protocols[name] = createEmptyMethod.MakeGenericMethod(m.ReturnType).Invoke(null, new object[] { p, m }) as MessageProcessor;
                    continue;
                }
                // protocols[name] = createMethod.MakeGenericMethod(pl[0].ParameterType, m.ReturnType).Invoke(null, new object[] { p, m }) as MessageProcessor;
                protocols[name] = Generic.InvokeAs(pl[0].ParameterType, m.ReturnType, Create<object,object>, p, m );
            }
        }

        public static MessageProcessor CreateEmpty<RT>(V8ProtocolObject p, MethodInfo m)
        {

            var fx = (Func<RT>)m.CreateDelegate(typeof(Func<RT>), p);
            Task<string> RunAsync(long id, JsonNode e)
            {
                var result = fx();
                return Task.Run(() => JsonSerializer.Serialize(new { id , result }, options));
            }
            return RunAsync;
        }


        public static MessageProcessor CreateEmptyAsync<RT>(MethodInfo m)
        {

            var fx = (Func<Task<RT>>)m.CreateDelegate(typeof(Func<Task<RT>>));
            async Task<string> RunAsync(long id, JsonNode e)
            {
                var result = await fx();
                return await Task.Run(() => JsonSerializer.Serialize(new { id, result }, options));
            }
            return RunAsync;
        }


        public static MessageProcessor Create<T, RT>(V8ProtocolObject p, MethodInfo m)
        {

            var fx = (Func<T, RT>)m.CreateDelegate(typeof(Func<T, RT>), p);
            Task<string> RunAsync(long id, JsonNode e)
            {
                var a = e == null ? default : JsonSerializer.Deserialize<T>(e, options);
                var result = fx(a);
                return Task.Run(() => JsonSerializer.Serialize(new { id, result }, options));
            }
            return RunAsync;
        }


        public static MessageProcessor CreateAsync<T, RT>(MethodInfo m) {

            var fx = (Func<T,Task<RT>>)m.CreateDelegate(typeof(Func<T, Task<RT>>));
            async Task<string> RunAsync(long id, JsonNode e)
            {
                var a = JsonSerializer.Deserialize<T>(e, options);
                var result = await fx(a);
                return await Task.Run(() => JsonSerializer.Serialize(new { id, result }, options));
            }
            return RunAsync;
        }

        public abstract void SendMessage(string message);

        //public static V8InspectorProtocol CreateWebSocketServer(int port)
        //{
        //    return new V8InspectorProtocolServer(port);
        //}

        public static V8InspectorProtocol CreateInverseProxy(Uri uri)
        {
            var p = new V8InspectorProtocolProxy(uri);
            return p;
        }



        public void Send(V8ProtocolEvent e)
        {
            Task.Run(() => {
                SendMessage(JsonSerializer.Serialize(new { method = e.EventName, @params = e }, options));
            });
        }

        public override void ReportException(JSValue error)
        {
            
        }

        private SHA256 hash = SHA256.Create();

        public override void ScriptParsed(long contextId, string code, string codeFilePath)
        {
            Task.Run(() =>
            {
                var id = codeFilePath ?? $"S-{Interlocked.Increment(ref nextScriptId)}";
                Scripts[id] = code;
                Send(new V8Debugger.ScriptParsed
                {
                    ScriptId = id,
                    Url = codeFilePath,
                    ExecutionContextId = contextId,
                    Hash = hash.ComputeHash(code),
                    HasSourceURL = !string.IsNullOrWhiteSpace(codeFilePath),
                    Length = code.Length,
                    ScriptLanguage = "JavaScript"
                });
            });
        }

        public async Task OnMessageReceived(IncomingMessage e) {

            if(!protocols.TryGetValue(e.Method, out var vm))
            {
                var sr = new JsonObject
                {
                    { "id", e.ID },
                    { "error", "Not found" }
                };
                System.Diagnostics.Debug.WriteLine($"Method {e.Method} not found");
                SendMessage(sr.ToString());
                return;
            }

            try
            {
                var r = await vm(e.ID, e.Params);
                SendMessage(r);

            } catch (Exception ex)
            {
                ReportException(new JSString(ex.ToString()));
            }

            return;
        }

    }
}
