using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using YantraJS.Core.Core.Storage;

namespace YantraJS.Core
{
    /// <summary>
    /// KeyStrings is a collection of frequently used string, KeyString is created with
    /// a unique numeric ID, which can be used as a key to store key value pair in Object.
    /// 
    /// UInt32 has only 4 bytes, and using it as key will reduce storage in K map. 
    /// 
    /// Compiler will create KeyStrings of all referenced keys in the script if they are not part of
    /// this class.
    /// 
    /// KeyString created with same string will always save.
    /// </summary>
    public static class KeyStrings
    {
        public readonly static KeyString __proto__;
        public readonly static KeyString length;

        public readonly static KeyString Number;

        public readonly static KeyString BigInt;

        public readonly static KeyString Object;
        public readonly static KeyString toString;

        public readonly static KeyString String;
        public readonly static KeyString substring;

        public readonly static KeyString Array;
        public readonly static KeyString join;

        public readonly static KeyString Function;
        public readonly static KeyString apply;
        public readonly static KeyString call;
        public readonly static KeyString callee;
        public readonly static KeyString bind;

        public readonly static KeyString Boolean;
        public readonly static KeyString Math;
        public readonly static KeyString Reflect;
        public readonly static KeyString Date;
        public readonly static KeyString Symbol;

        public readonly static KeyString Promise;
        public readonly static KeyString then;
        public readonly static KeyString @catch;

        public readonly static KeyString JSON;
        public readonly static KeyString parse;
        public readonly static KeyString stringify;
        public readonly static KeyString toJSON;


        public readonly static KeyString RegExp;
        public readonly static KeyString test;
        public readonly static KeyString index;
        public readonly static KeyString input;
        public readonly static KeyString lastIndex;

        public readonly static KeyString Error;
        public readonly static KeyString message;
        public readonly static KeyString stack;
        public readonly static KeyString RangeError;
        public readonly static KeyString SyntaxError;
        public readonly static KeyString TypeError;
        public readonly static KeyString URIError;
        public readonly static KeyString EvalError;
        public readonly static KeyString ReferenceError;

        public readonly static KeyString ArrayBuffer;

        public readonly static KeyString Int8Array;
        public readonly static KeyString Uint8Array;
        public readonly static KeyString Uint8ClampedArray;
        public readonly static KeyString Int16Array;
        public readonly static KeyString Uint16Array;
        public readonly static KeyString Int32Array;
        public readonly static KeyString Uint32Array;
        public readonly static KeyString Float32Array;
        public readonly static KeyString Float64Array;

        public readonly static KeyString DataView;

        public readonly static KeyString Map;
        public readonly static KeyString Set;
        public readonly static KeyString WeakRef;
        public readonly static KeyString WeakMap; 
        public readonly static KeyString WeakSet;
        public readonly static KeyString valueOf;
        public readonly static KeyString name;
        public readonly static KeyString prototype;
        public readonly static KeyString constructor;
        public readonly static KeyString defineProperty;
        public readonly static KeyString deleteProperty;

        public readonly static KeyString FinalizationRegistry;

        public readonly static KeyString configurable;
        public readonly static KeyString enumerable;
        public readonly static KeyString @readonly;
        public readonly static KeyString writable;

        public readonly static KeyString @assert;
        
        public readonly static KeyString native;
        public readonly static KeyString value;
        public readonly static KeyString done;
        public readonly static KeyString get;
        public readonly static KeyString set;
        public readonly static KeyString undefined;
        public readonly static KeyString NaN;
        public readonly static KeyString @null;
        public readonly static KeyString getPrototypeOf;
        public readonly static KeyString ownKeys;
        public readonly static KeyString setPrototypeOf;

        public readonly static KeyString @global;
        public readonly static KeyString globalThis;

        public readonly static KeyString Module;
        public readonly static KeyString module;
        public readonly static KeyString resolve;
        public readonly static KeyString require;
        public readonly static KeyString @default;
        public readonly static KeyString import;

        public readonly static KeyString exports;

        public readonly static KeyString Generator;
        public readonly static KeyString next;
        public readonly static KeyString @throw;
        public readonly static KeyString @return;

        // intl...
        public readonly static KeyString weekday;
        public readonly static KeyString year;
        public readonly static KeyString month;
        public readonly static KeyString day;
        public readonly static KeyString hour;
        public readonly static KeyString minute;
        public readonly static KeyString second;


        // global methods...
        public readonly static KeyString eval;
        public readonly static KeyString encodeURI;
        public readonly static KeyString encodeURIComponent;
        public readonly static KeyString decodeURI;
        public readonly static KeyString decodeURIComponent;

        public readonly static KeyString isFinite;
        public readonly static KeyString isNaN;
        public readonly static KeyString parseFloat;
        public readonly static KeyString parseInt;

        public readonly static KeyString arguments;

        public readonly static KeyString Infinity;


        public readonly static KeyString console;
        public readonly static KeyString @debug;
        public readonly static KeyString log;

        public readonly static KeyString clr;

        public readonly static KeyString @true;

        public readonly static KeyString @false;

        public readonly static KeyString bubbles;
        public readonly static KeyString detail;
        public readonly static KeyString cancelable;
        public readonly static KeyString composed;

        public readonly static KeyString capture;
        public readonly static KeyString deferred;
        public readonly static KeyString once;


        public readonly static KeyString raw;

        static KeyStrings()
        {
            lock (typeof(KeyStrings))
            {
                map = ConcurrentStringMap<KeyString>.Create();
                names = ConcurrentUInt32Map<StringSpan>.Create();
                KeyString Create(string key)
                {
                    var i = (uint)(NextID++);
                    var js = new KeyString(i);
                    map[key] =  js;
                    names[i] = key;
                    return js;
                }
                var t = typeof(KeyString);
                foreach (var f in typeof(KeyStrings)
                    .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                {
                    if (f.FieldType != t)
                        continue;
                    f.SetValue(null, Create(f.Name));
                }
            }
        }

        private static ConcurrentStringMap<KeyString> map;
        private static ConcurrentUInt32Map<StringSpan> names;

        private static int NextID = 1;

        // internal static (int size, int total, int next) Total => (map.Size, map.Total, NextID);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyString GetOrCreate(in StringSpan key)
        {
            return map.GetOrCreate(key, (keyName) =>
            {
                var i = (uint)Interlocked.Increment(ref NextID);
                names[i] = keyName;
                return new KeyString(i);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryGet(in StringSpan key, out KeyString ks)
        {
            return map.TryGetValue(key, out ks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static KeyString GetName(uint id)
        {
            return new KeyString(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static StringSpan GetNameString(uint id)
        {
            return names[id];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JSString GetJSString(uint id)
        {
            var name = GetName(id);
            return new JSString(name.Value, name);
        }

    }
}
