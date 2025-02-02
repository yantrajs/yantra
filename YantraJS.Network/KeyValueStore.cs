#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Yantra.Core;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Network
{
    [JSClassGenerator]
    public partial class KeyValueStore : JSObject
    {
        private Dictionary<string, string>? headers;

        internal KeyValueStore(JSValue? first, JSObject? prototype = null) : this(prototype)
        {
            if (first == null)
                return;
            headers = new Dictionary<string, string>();
            if (first.IsArray)
            {
                // .. expecting aray...
                for (uint i = 0; i < first.Length; i++)
                {
                    var pair = first[i];
                    if (pair.IsArray && pair.Length > 1)
                    {
                        headers[pair[0].ToString().ToLower()] = pair[1].ToString();
                        continue;
                    }
                }
                return;
            }

            if (first.ConvertTo<KeyValueStore>(out var src))
            {
                if (src.headers != null)
                {
                    foreach (var pair in src.headers)
                    {
                        headers[pair.Key] = pair.Value;
                    }
                }
                return;
            }

            if (first is JSObject @object)
            {
                ref var ps = ref @object.GetOwnProperties(false);
                var ve = new PropertySequence.ValueEnumerator(@object, true);
                while (ve.MoveNext(out var value, out var p))
                {
                    headers[p.Value.Value!] = value.ToString();
                }
                return;
            }

        }


        public KeyValueStore(in Arguments a) : this(a[0], JSContext.NewTargetPrototype) { }

        public IEnumerable<KeyValuePair<string, string>> GetEnumerable()
        {
            if (headers == null)
                yield break;
            foreach (var pair in headers)
                yield return pair;
        }


        [JSExport]
        public void Append(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            headers ??= new Dictionary<string, string>();
            name = name.ToLower();
            if (headers.TryGetValue(name, out var v))
            {
                value = v + "," + value;
            }
            headers[name] = value;
        }

        [JSExport]
        public void Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            name = name.ToLower();
            if (headers == null)
            {
                return;
            }
            headers.Remove(name);
        }

        [JSExport]
        public new IEnumerable<JSValue> Entries()
        {
            if (headers == null)
                yield break;
            foreach (var pair in headers)
            {
                yield return new JSArray(
                    new JSString(pair.Key),
                    new JSString(pair.Value)
                );
            }
        }

        [JSExport]
        public JSValue ForEach(in Arguments a)
        {
            var fx = a[0] ?? throw new ArgumentException("callback is required");
            if (headers == null)
            {
                return JSUndefined.Value;
            }
            var @this = a[1] ?? JSUndefined.Value;
            foreach (var pair in headers)
            {
                fx.Call(@this,
                    new JSString(pair.Value),
                    new JSString(pair.Key),
                    this);
            }
            return JSUndefined.Value;
        }

        [JSExport]
        public JSValue Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (headers?.TryGetValue(name.ToLower(), out var value) ?? false)
            {
                return new JSString(value);
            }
            return JSUndefined.Value;
        }

        [JSExport]
        public JSValue Has(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (headers!.ContainsKey(name.ToLower()))
            {
                return JSBoolean.True;
            }
            return JSBoolean.False;
        }

        [JSExport]
        public IEnumerable<JSValue> Keys()
        {
            if (headers == null)
                yield break;
            foreach (var pair in headers)
            {
                yield return new JSString(pair.Key);
            }
        }

        [JSExport]
        public void Set(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            headers ??= new Dictionary<string, string>();
            headers[name] = value;
        }

        [JSExport]
        public IEnumerable<JSValue> Values()
        {
            if (headers == null)
                yield break;
            foreach (var pair in headers)
            {
                yield return new JSString(pair.Value);
            }
        }

    }
}
