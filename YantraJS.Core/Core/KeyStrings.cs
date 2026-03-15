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
    public class KeyStrings
    {
        public readonly KeyString __proto__;
        public readonly KeyString length;

        public readonly KeyString Number;

        public readonly KeyString BigInt;

        public readonly KeyString Object;
        public readonly KeyString toString;

        public readonly KeyString String;
        public readonly KeyString substring;

        public readonly KeyString Array;
        public readonly KeyString join;

        public readonly KeyString Function;
        public readonly KeyString apply;
        public readonly KeyString call;
        public readonly KeyString callee;
        public readonly KeyString bind;

        public readonly KeyString Boolean;
        public readonly KeyString Math;
        public readonly KeyString Reflect;
        public readonly KeyString Date;
        public readonly KeyString Symbol;

        public readonly KeyString Promise;
        public readonly KeyString then;
        public readonly KeyString @catch;

        public readonly KeyString JSON;
        public readonly KeyString parse;
        public readonly KeyString stringify;
        public readonly KeyString toJSON;


        public readonly KeyString RegExp;
        public readonly KeyString test;
        public readonly KeyString index;
        public readonly KeyString input;
        public readonly KeyString lastIndex;

        public readonly KeyString Error;
        public readonly KeyString message;
        public readonly KeyString stack;
        public readonly KeyString RangeError;
        public readonly KeyString SyntaxError;
        public readonly KeyString TypeError;
        public readonly KeyString URIError;
        public readonly KeyString EvalError;
        public readonly KeyString ReferenceError;

        public readonly KeyString ArrayBuffer;

        public readonly KeyString Int8Array;
        public readonly KeyString Uint8Array;
        public readonly KeyString Uint8ClampedArray;
        public readonly KeyString Int16Array;
        public readonly KeyString Uint16Array;
        public readonly KeyString Int32Array;
        public readonly KeyString Uint32Array;
        public readonly KeyString Float32Array;
        public readonly KeyString Float64Array;

        public readonly KeyString DataView;

        public readonly KeyString Map;
        public readonly KeyString Set;
        public readonly KeyString WeakRef;
        public readonly KeyString WeakMap; 
        public readonly KeyString WeakSet;
        public readonly KeyString valueOf;
        public readonly KeyString name;
        public readonly KeyString prototype;
        public readonly KeyString constructor;
        public readonly KeyString defineProperty;
        public readonly KeyString deleteProperty;

        public readonly KeyString FinalizationRegistry;

        public readonly KeyString configurable;
        public readonly KeyString enumerable;
        public readonly KeyString @readonly;
        public readonly KeyString writable;

        public readonly KeyString @assert;
        
        public readonly KeyString native;
        public readonly KeyString value;
        public readonly KeyString done;
        public readonly KeyString get;
        public readonly KeyString set;
        public readonly KeyString undefined;
        public readonly KeyString NaN;
        public readonly KeyString @null;
        public readonly KeyString getPrototypeOf;
        public readonly KeyString ownKeys;
        public readonly KeyString setPrototypeOf;

        public readonly KeyString @global;
        public readonly KeyString globalThis;

        public readonly KeyString Module;
        public readonly KeyString module;
        public readonly KeyString resolve;
        public readonly KeyString require;
        public readonly KeyString @default;
        public readonly KeyString import;

        public readonly KeyString exports;

        public readonly KeyString Generator;
        public readonly KeyString next;
        public readonly KeyString @throw;
        public readonly KeyString @return;

        // intl...
        public readonly KeyString weekday;
        public readonly KeyString year;
        public readonly KeyString month;
        public readonly KeyString day;
        public readonly KeyString hour;
        public readonly KeyString minute;
        public readonly KeyString second;


        // global methods...
        public readonly KeyString eval;
        public readonly KeyString encodeURI;
        public readonly KeyString encodeURIComponent;
        public readonly KeyString decodeURI;
        public readonly KeyString decodeURIComponent;

        public readonly KeyString isFinite;
        public readonly KeyString isNaN;
        public readonly KeyString parseFloat;
        public readonly KeyString parseInt;

        public readonly KeyString arguments;

        public readonly KeyString Infinity;


        public readonly KeyString console;
        public readonly KeyString @debug;
        public readonly KeyString log;

        public readonly KeyString clr;

        public readonly KeyString @true;

        public readonly KeyString @false;

        public readonly KeyString bubbles;
        public readonly KeyString detail;
        public readonly KeyString cancelable;
        public readonly KeyString composed;

        public readonly KeyString capture;
        public readonly KeyString deferred;
        public readonly KeyString once;


        public readonly KeyString raw;

        public static readonly KeyStrings Instance = new KeyStrings();

        KeyStrings()
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
                .GetFields(System.Reflection.BindingFlags.Public))
            {
                if (f.FieldType != t)
                    continue;
                f.SetValue(null, Create(f.Name));
            }
        }

        private ConcurrentStringMap<KeyString> map;
        private ConcurrentUInt32Map<StringSpan> names;

        private static int NextID = 1;

        // internal static (int size, int total, int next) Total => (map.Size, map.Total, NextID);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyString GetOrCreate(in StringSpan key)
        {
            return map.GetOrCreate(key, (keyName) =>
            {
                var i = (uint)Interlocked.Increment(ref NextID);
                names[i] = keyName;
                return new KeyString(i);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryGet(in StringSpan key, out KeyString ks)
        {
            return map.TryGetValue(key, out ks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal KeyString GetName(uint id)
        {
            return new KeyString(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal StringSpan GetNameString(uint id)
        {
            return names[id];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal JSString GetJSString(uint id)
        {
            var name = GetName(id);
            return new JSString(name.Value, name);
        }

    }
}
