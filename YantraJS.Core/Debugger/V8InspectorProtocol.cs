using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YantraJS.Debugger;

namespace YantraJS.Core.Debugger
{
    public abstract class V8InspectorProtocol : JSDebugger, IDisposable
    {
        private Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer()
        {
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
            {
                NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
            }
            
        };

        public abstract void Dispose();
        public abstract Task ConnectAsync();

        private Dictionary<string, Func<JObject, Task<JToken>>> protocols
            = new Dictionary<string, Func<JObject, Task<JToken>>>();

        public string ID { get; }

        private static long id = 0;

        public Dictionary<string, JSContext> Contexts = new Dictionary<string, JSContext>();

        public Dictionary<string, JSFunctionDelegate> Scripts = new Dictionary<string, JSFunctionDelegate>();

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
            Contexts[cid] = a;
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
                        protocols[name] = createEmptyAsyncMethod.MakeGenericMethod(m.ReturnType.GetGenericArguments()[0])
                            .Invoke(null, new object[] { p, m }) as Func<JObject, Task<JToken>>;
                        continue;
                    }
                    protocols[name] = createAsyncMethod.MakeGenericMethod(pl[0].ParameterType, m.ReturnType.GetGenericArguments()[0])
                        .Invoke(null, new object[] { p, m }) as Func<JObject, Task<JToken>>;
                    continue;
                }
                if(pl.Length == 0)
                {
                    protocols[name] = createEmptyMethod.MakeGenericMethod(m.ReturnType).Invoke(null, new object[] { p, m }) as Func<JObject, Task<JToken>>;
                    continue;
                }
                protocols[name] = createMethod.MakeGenericMethod(pl[0].ParameterType, m.ReturnType).Invoke(null, new object[] { p, m }) as Func<JObject, Task<JToken>>;
            }
        }

        private static MethodInfo createMethod = typeof(V8InspectorProtocol).GetMethod(nameof(Create));
        private static MethodInfo createAsyncMethod = typeof(V8InspectorProtocol).GetMethod(nameof(CreateAsync));

        private static MethodInfo createEmptyMethod = typeof(V8InspectorProtocol).GetMethod(nameof(CreateEmpty));
        private static MethodInfo createEmptyAsyncMethod = typeof(V8InspectorProtocol).GetMethod(nameof(CreateEmptyAsync));

        public static Func<JObject, Task<JToken>> CreateEmpty<RT>(V8ProtocolObject p, MethodInfo m)
        {

            var fx = (Func<RT>)m.CreateDelegate(typeof(Func<RT>), p);
            Task<JToken> RunAsync(JObject e)
            {
                var r = fx();
                return Task.FromResult(JToken.FromObject(r));
            }
            return RunAsync;
        }


        public static Func<JObject, Task<JToken>> CreateEmptyAsync<RT>(MethodInfo m)
        {

            var fx = (Func<Task<RT>>)m.CreateDelegate(typeof(Func<Task<RT>>));
            async Task<JToken> RunAsync(JObject e)
            {
                var r = await fx();
                return JToken.FromObject(r);
            }
            return RunAsync;
        }


        public static Func<JObject, Task<JToken>> Create<T, RT>(V8ProtocolObject p, MethodInfo m)
        {

            var fx = (Func<T, RT>)m.CreateDelegate(typeof(Func<T, RT>), p);
            Task<JToken> RunAsync(JObject e)
            {
                var a = e == null ? default : e.ToObject<T>();
                var r = fx(a);
                return Task.FromResult(JToken.FromObject(r));
            }
            return RunAsync;
        }


        public static Func<JObject, Task<JToken>> CreateAsync<T, RT>(MethodInfo m) {

            var fx = (Func<T,Task<RT>>)m.CreateDelegate(typeof(Func<T, Task<RT>>));
            async Task<JToken> RunAsync(JObject e)
            {
                var a = e.ToObject<T>();
                var r = await fx(a);
                return JToken.FromObject(r);
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
            var sr = new JObject();
            sr.Add("method", e.EventName);
            sr.Add("params", JObject.FromObject(e, serializer));
            SendMessage(sr.ToString());
        }

        public override void ReportException(JSValue error)
        {
            
        }

        public override void ScriptParsed(string code, string codeFilePath)
        {
            
        }

        public async Task OnMessageReceived(IncomingMessage e) {

            if(!protocols.TryGetValue(e.Method, out var vm))
            {
                var sr = new JObject
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
                var r = await vm(e.Params);
                var sr = new JObject
                {
                    { "id", e.ID },
                    { "result", r }
                };
                SendMessage(sr.ToString());

            } catch (Exception ex)
            {
                ReportException(new JSString(ex.ToString()));
            }

            return;
        }

    }
}
