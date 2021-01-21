/*! *****************************************************************************
Copyright (C) Microsoft. All rights reserved.
Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License at http://www.apache.org/licenses/LICENSE-2.0

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
MERCHANTABLITY OR NON-INFRINGEMENT.

See the Apache Version 2.0 License for specific language governing permissions
and limitations under the License.
***************************************************************************** */

var Reflect;
(function (Reflect) {
    (function (factory) {
        const root = typeof global === "object" ? global :
            typeof self === "object" ? self :
                typeof this === "object" ? this :
                    Function("return this;")();
        let exporter = makeExporter(Reflect);
        if (typeof root.Reflect === "undefined") {
            root.Reflect = Reflect;
        }
        else {
            exporter = makeExporter(root.Reflect, exporter);
        }
        factory(exporter);
        function makeExporter(target, previous) {
            return (key, value) => {
                if (typeof target[key] !== "function") {
                    Object.defineProperty(target, key, { configurable: true, writable: true, value });
                }
                if (previous)
                    previous(key, value);
            };
        }
    })(function (exporter) {
        const hasOwn = Object.prototype.hasOwnProperty;
        const supportsSymbol = typeof Symbol === "function";
        const toPrimitiveSymbol = supportsSymbol && typeof Symbol.toPrimitive !== "undefined" ? Symbol.toPrimitive : "@@toPrimitive";
        const iteratorSymbol = supportsSymbol && typeof Symbol.iterator !== "undefined" ? Symbol.iterator : "@@iterator";
        const supportsCreate = typeof Object.create === "function";
        const supportsProto = { __proto__: [] } instanceof Array;
        const downLevel = !supportsCreate && !supportsProto;
        const HashMap = {
            create: supportsCreate
                ? () => MakeDictionary(Object.create(null))
                : supportsProto
                    ? () => MakeDictionary({ __proto__: null })
                    : () => MakeDictionary({}),
            has: downLevel
                ? (map, key) => hasOwn.call(map, key)
                : (map, key) => key in map,
            get: downLevel
                ? (map, key) => hasOwn.call(map, key) ? map[key] : undefined
                : (map, key) => map[key],
        };
        const functionPrototype = Object.getPrototypeOf(Function);
        const usePolyfill = typeof process === "object" && process.env && process.env["REFLECT_METADATA_USE_MAP_POLYFILL"] === "true";
        const _Map = !usePolyfill && typeof Map === "function" && typeof Map.prototype.entries === "function" ? Map : CreateMapPolyfill();
        const _Set = !usePolyfill && typeof Set === "function" && typeof Set.prototype.entries === "function" ? Set : CreateSetPolyfill();
        const _WeakMap = !usePolyfill && typeof WeakMap === "function" ? WeakMap : CreateWeakMapPolyfill();
        const Metadata = new _WeakMap();
        function decorate(decorators, target, propertyKey, attributes) {
            if (!IsUndefined(propertyKey)) {
                if (!IsArray(decorators))
                    throw new TypeError();
                if (!IsObject(target))
                    throw new TypeError();
                if (!IsObject(attributes) && !IsUndefined(attributes) && !IsNull(attributes))
                    throw new TypeError();
                if (IsNull(attributes))
                    attributes = undefined;
                propertyKey = ToPropertyKey(propertyKey);
                return DecorateProperty(decorators, target, propertyKey, attributes);
            }
            else {
                if (!IsArray(decorators))
                    throw new TypeError();
                if (!IsConstructor(target))
                    throw new TypeError();
                return DecorateConstructor(decorators, target);
            }
        }
        exporter("decorate", decorate);
        function metadata(metadataKey, metadataValue) {
            function decorator(target, propertyKey) {
                if (!IsObject(target))
                    throw new TypeError();
                if (!IsUndefined(propertyKey) && !IsPropertyKey(propertyKey))
                    throw new TypeError();
                OrdinaryDefineOwnMetadata(metadataKey, metadataValue, target, propertyKey);
            }
            return decorator;
        }
        exporter("metadata", metadata);
        function defineMetadata(metadataKey, metadataValue, target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryDefineOwnMetadata(metadataKey, metadataValue, target, propertyKey);
        }
        exporter("defineMetadata", defineMetadata);
        function hasMetadata(metadataKey, target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryHasMetadata(metadataKey, target, propertyKey);
        }
        exporter("hasMetadata", hasMetadata);
        function hasOwnMetadata(metadataKey, target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryHasOwnMetadata(metadataKey, target, propertyKey);
        }
        exporter("hasOwnMetadata", hasOwnMetadata);
        function getMetadata(metadataKey, target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryGetMetadata(metadataKey, target, propertyKey);
        }
        exporter("getMetadata", getMetadata);
        function getOwnMetadata(metadataKey, target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryGetOwnMetadata(metadataKey, target, propertyKey);
        }
        exporter("getOwnMetadata", getOwnMetadata);
        function getMetadataKeys(target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryMetadataKeys(target, propertyKey);
        }
        exporter("getMetadataKeys", getMetadataKeys);
        function getOwnMetadataKeys(target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            return OrdinaryOwnMetadataKeys(target, propertyKey);
        }
        exporter("getOwnMetadataKeys", getOwnMetadataKeys);
        function deleteMetadata(metadataKey, target, propertyKey) {
            if (!IsObject(target))
                throw new TypeError();
            if (!IsUndefined(propertyKey))
                propertyKey = ToPropertyKey(propertyKey);
            const metadataMap = GetOrCreateMetadataMap(target, propertyKey, false);
            if (IsUndefined(metadataMap))
                return false;
            if (!metadataMap.delete(metadataKey))
                return false;
            if (metadataMap.size > 0)
                return true;
            const targetMetadata = Metadata.get(target);
            targetMetadata.delete(propertyKey);
            if (targetMetadata.size > 0)
                return true;
            Metadata.delete(target);
            return true;
        }
        exporter("deleteMetadata", deleteMetadata);
        function DecorateConstructor(decorators, target) {
            for (let i = decorators.length - 1; i >= 0; --i) {
                const decorator = decorators[i];
                const decorated = decorator(target);
                if (!IsUndefined(decorated) && !IsNull(decorated)) {
                    if (!IsConstructor(decorated))
                        throw new TypeError();
                    target = decorated;
                }
            }
            return target;
        }
        function DecorateProperty(decorators, target, propertyKey, descriptor) {
            for (let i = decorators.length - 1; i >= 0; --i) {
                const decorator = decorators[i];
                const decorated = decorator(target, propertyKey, descriptor);
                if (!IsUndefined(decorated) && !IsNull(decorated)) {
                    if (!IsObject(decorated))
                        throw new TypeError();
                    descriptor = decorated;
                }
            }
            return descriptor;
        }
        function GetOrCreateMetadataMap(O, P, Create) {
            let targetMetadata = Metadata.get(O);
            if (IsUndefined(targetMetadata)) {
                if (!Create)
                    return undefined;
                targetMetadata = new _Map();
                Metadata.set(O, targetMetadata);
            }
            let metadataMap = targetMetadata.get(P);
            if (IsUndefined(metadataMap)) {
                if (!Create)
                    return undefined;
                metadataMap = new _Map();
                targetMetadata.set(P, metadataMap);
            }
            return metadataMap;
        }
        function OrdinaryHasMetadata(MetadataKey, O, P) {
            const hasOwn = OrdinaryHasOwnMetadata(MetadataKey, O, P);
            if (hasOwn)
                return true;
            const parent = OrdinaryGetPrototypeOf(O);
            if (!IsNull(parent))
                return OrdinaryHasMetadata(MetadataKey, parent, P);
            return false;
        }
        function OrdinaryHasOwnMetadata(MetadataKey, O, P) {
            const metadataMap = GetOrCreateMetadataMap(O, P, false);
            if (IsUndefined(metadataMap))
                return false;
            return ToBoolean(metadataMap.has(MetadataKey));
        }
        function OrdinaryGetMetadata(MetadataKey, O, P) {
            const hasOwn = OrdinaryHasOwnMetadata(MetadataKey, O, P);
            if (hasOwn)
                return OrdinaryGetOwnMetadata(MetadataKey, O, P);
            const parent = OrdinaryGetPrototypeOf(O);
            if (!IsNull(parent))
                return OrdinaryGetMetadata(MetadataKey, parent, P);
            return undefined;
        }
        function OrdinaryGetOwnMetadata(MetadataKey, O, P) {
            const metadataMap = GetOrCreateMetadataMap(O, P, false);
            if (IsUndefined(metadataMap))
                return undefined;
            return metadataMap.get(MetadataKey);
        }
        function OrdinaryDefineOwnMetadata(MetadataKey, MetadataValue, O, P) {
            const metadataMap = GetOrCreateMetadataMap(O, P, true);
            metadataMap.set(MetadataKey, MetadataValue);
        }
        function OrdinaryMetadataKeys(O, P) {
            const ownKeys = OrdinaryOwnMetadataKeys(O, P);
            const parent = OrdinaryGetPrototypeOf(O);
            if (parent === null)
                return ownKeys;
            const parentKeys = OrdinaryMetadataKeys(parent, P);
            if (parentKeys.length <= 0)
                return ownKeys;
            if (ownKeys.length <= 0)
                return parentKeys;
            const set = new _Set();
            const keys = [];
            for (const key of ownKeys) {
                const hasKey = set.has(key);
                if (!hasKey) {
                    set.add(key);
                    keys.push(key);
                }
            }
            for (const key of parentKeys) {
                const hasKey = set.has(key);
                if (!hasKey) {
                    set.add(key);
                    keys.push(key);
                }
            }
            return keys;
        }
        function OrdinaryOwnMetadataKeys(O, P) {
            const keys = [];
            const metadataMap = GetOrCreateMetadataMap(O, P, false);
            if (IsUndefined(metadataMap))
                return keys;
            const keysObj = metadataMap.keys();
            const iterator = GetIterator(keysObj);
            let k = 0;
            while (true) {
                const next = IteratorStep(iterator);
                if (!next) {
                    keys.length = k;
                    return keys;
                }
                const nextValue = IteratorValue(next);
                try {
                    keys[k] = nextValue;
                }
                catch (e) {
                    try {
                        IteratorClose(iterator);
                    }
                    finally {
                        throw e;
                    }
                }
                k++;
            }
        }
        function Type(x) {
            if (x === null)
                return 1;
            switch (typeof x) {
                case "undefined": return 0;
                case "boolean": return 2;
                case "string": return 3;
                case "symbol": return 4;
                case "number": return 5;
                case "object": return x === null ? 1 : 6;
                default: return 6;
            }
        }
        function IsUndefined(x) {
            return x === undefined;
        }
        function IsNull(x) {
            return x === null;
        }
        function IsSymbol(x) {
            return typeof x === "symbol";
        }
        function IsObject(x) {
            return typeof x === "object" ? x !== null : typeof x === "function";
        }
        function ToPrimitive(input, PreferredType) {
            switch (Type(input)) {
                case 0: return input;
                case 1: return input;
                case 2: return input;
                case 3: return input;
                case 4: return input;
                case 5: return input;
            }
            const hint = PreferredType === 3 ? "string" : PreferredType === 5 ? "number" : "default";
            const exoticToPrim = GetMethod(input, toPrimitiveSymbol);
            if (exoticToPrim !== undefined) {
                const result = exoticToPrim.call(input, hint);
                if (IsObject(result))
                    throw new TypeError();
                return result;
            }
            return OrdinaryToPrimitive(input, hint === "default" ? "number" : hint);
        }
        function OrdinaryToPrimitive(O, hint) {
            if (hint === "string") {
                const toString = O.toString;
                if (IsCallable(toString)) {
                    const result = toString.call(O);
                    if (!IsObject(result))
                        return result;
                }
                const valueOf = O.valueOf;
                if (IsCallable(valueOf)) {
                    const result = valueOf.call(O);
                    if (!IsObject(result))
                        return result;
                }
            }
            else {
                const valueOf = O.valueOf;
                if (IsCallable(valueOf)) {
                    const result = valueOf.call(O);
                    if (!IsObject(result))
                        return result;
                }
                const toString = O.toString;
                if (IsCallable(toString)) {
                    const result = toString.call(O);
                    if (!IsObject(result))
                        return result;
                }
            }
            throw new TypeError();
        }
        function ToBoolean(argument) {
            return !!argument;
        }
        function ToString(argument) {
            return "" + argument;
        }
        function ToPropertyKey(argument) {
            const key = ToPrimitive(argument, 3);
            if (IsSymbol(key))
                return key;
            return ToString(key);
        }
        function IsArray(argument) {
            return Array.isArray
                ? Array.isArray(argument)
                : argument instanceof Object
                    ? argument instanceof Array
                    : Object.prototype.toString.call(argument) === "[object Array]";
        }
        function IsCallable(argument) {
            return typeof argument === "function";
        }
        function IsConstructor(argument) {
            return typeof argument === "function";
        }
        function IsPropertyKey(argument) {
            switch (Type(argument)) {
                case 3: return true;
                case 4: return true;
                default: return false;
            }
        }
        function GetMethod(V, P) {
            const func = V[P];
            if (func === undefined || func === null)
                return undefined;
            if (!IsCallable(func))
                throw new TypeError();
            return func;
        }
        function GetIterator(obj) {
            const method = GetMethod(obj, iteratorSymbol);
            if (!IsCallable(method))
                throw new TypeError();
            const iterator = method.call(obj);
            if (!IsObject(iterator))
                throw new TypeError();
            return iterator;
        }
        function IteratorValue(iterResult) {
            return iterResult.value;
        }
        function IteratorStep(iterator) {
            const result = iterator.next();
            return result.done ? false : result;
        }
        function IteratorClose(iterator) {
            const f = iterator["return"];
            if (f)
                f.call(iterator);
        }
        function OrdinaryGetPrototypeOf(O) {
            const proto = Object.getPrototypeOf(O);
            if (typeof O !== "function" || O === functionPrototype)
                return proto;
            if (proto !== functionPrototype)
                return proto;
            const prototype = O.prototype;
            const prototypeProto = prototype && Object.getPrototypeOf(prototype);
            if (prototypeProto == null || prototypeProto === Object.prototype)
                return proto;
            const constructor = prototypeProto.constructor;
            if (typeof constructor !== "function")
                return proto;
            if (constructor === O)
                return proto;
            return constructor;
        }
        function CreateMapPolyfill() {
            const cacheSentinel = {};
            const arraySentinel = [];
            class MapIterator {
                constructor(keys, values, selector) {
                    this._index = 0;
                    this._keys = keys;
                    this._values = values;
                    this._selector = selector;
                }
                "@@iterator"() { return this; }
                [iteratorSymbol]() { return this; }
                next() {
                    const index = this._index;
                    if (index >= 0 && index < this._keys.length) {
                        const result = this._selector(this._keys[index], this._values[index]);
                        if (index + 1 >= this._keys.length) {
                            this._index = -1;
                            this._keys = arraySentinel;
                            this._values = arraySentinel;
                        }
                        else {
                            this._index++;
                        }
                        return { value: result, done: false };
                    }
                    return { value: undefined, done: true };
                }
                throw(error) {
                    if (this._index >= 0) {
                        this._index = -1;
                        this._keys = arraySentinel;
                        this._values = arraySentinel;
                    }
                    throw error;
                }
                return(value) {
                    if (this._index >= 0) {
                        this._index = -1;
                        this._keys = arraySentinel;
                        this._values = arraySentinel;
                    }
                    return { value: value, done: true };
                }
            }
            return class Map {
                constructor() {
                    this._keys = [];
                    this._values = [];
                    this._cacheKey = cacheSentinel;
                    this._cacheIndex = -2;
                }
                get size() { return this._keys.length; }
                has(key) { return this._find(key, false) >= 0; }
                get(key) {
                    const index = this._find(key, false);
                    return index >= 0 ? this._values[index] : undefined;
                }
                set(key, value) {
                    const index = this._find(key, true);
                    this._values[index] = value;
                    return this;
                }
                delete(key) {
                    const index = this._find(key, false);
                    if (index >= 0) {
                        const size = this._keys.length;
                        for (let i = index + 1; i < size; i++) {
                            this._keys[i - 1] = this._keys[i];
                            this._values[i - 1] = this._values[i];
                        }
                        this._keys.length--;
                        this._values.length--;
                        if (key === this._cacheKey) {
                            this._cacheKey = cacheSentinel;
                            this._cacheIndex = -2;
                        }
                        return true;
                    }
                    return false;
                }
                clear() {
                    this._keys.length = 0;
                    this._values.length = 0;
                    this._cacheKey = cacheSentinel;
                    this._cacheIndex = -2;
                }
                keys() { return new MapIterator(this._keys, this._values, getKey); }
                values() { return new MapIterator(this._keys, this._values, getValue); }
                entries() { return new MapIterator(this._keys, this._values, getEntry); }
                "@@iterator"() { return this.entries(); }
                [iteratorSymbol]() { return this.entries(); }
                _find(key, insert) {
                    if (this._cacheKey !== key) {
                        this._cacheIndex = this._keys.indexOf(this._cacheKey = key);
                    }
                    if (this._cacheIndex < 0 && insert) {
                        this._cacheIndex = this._keys.length;
                        this._keys.push(key);
                        this._values.push(undefined);
                    }
                    return this._cacheIndex;
                }
            };
            function getKey(key, _) {
                return key;
            }
            function getValue(_, value) {
                return value;
            }
            function getEntry(key, value) {
                return [key, value];
            }
        }
        function CreateSetPolyfill() {
            return class Set {
                constructor() {
                    this._map = new _Map();
                }
                get size() { return this._map.size; }
                has(value) { return this._map.has(value); }
                add(value) { return this._map.set(value, value), this; }
                delete(value) { return this._map.delete(value); }
                clear() { this._map.clear(); }
                keys() { return this._map.keys(); }
                values() { return this._map.values(); }
                entries() { return this._map.entries(); }
                "@@iterator"() { return this.keys(); }
                [iteratorSymbol]() { return this.keys(); }
            };
        }
        function CreateWeakMapPolyfill() {
            const UUID_SIZE = 16;
            const keys = HashMap.create();
            const rootKey = CreateUniqueKey();
            return class WeakMap {
                constructor() {
                    this._key = CreateUniqueKey();
                }
                has(target) {
                    const table = GetOrCreateWeakMapTable(target, false);
                    return table !== undefined ? HashMap.has(table, this._key) : false;
                }
                get(target) {
                    const table = GetOrCreateWeakMapTable(target, false);
                    return table !== undefined ? HashMap.get(table, this._key) : undefined;
                }
                set(target, value) {
                    const table = GetOrCreateWeakMapTable(target, true);
                    table[this._key] = value;
                    return this;
                }
                delete(target) {
                    const table = GetOrCreateWeakMapTable(target, false);
                    return table !== undefined ? delete table[this._key] : false;
                }
                clear() {
                    this._key = CreateUniqueKey();
                }
            };
            function CreateUniqueKey() {
                let key;
                do
                    key = "@@WeakMap@@" + CreateUUID();
                while (HashMap.has(keys, key));
                keys[key] = true;
                return key;
            }
            function GetOrCreateWeakMapTable(target, create) {
                if (!hasOwn.call(target, rootKey)) {
                    if (!create)
                        return undefined;
                    Object.defineProperty(target, rootKey, { value: HashMap.create() });
                }
                return target[rootKey];
            }
            function FillRandomBytes(buffer, size) {
                for (let i = 0; i < size; ++i)
                    buffer[i] = Math.random() * 0xff | 0;
                return buffer;
            }
            function GenRandomBytes(size) {
                if (typeof Uint8Array === "function") {
                    if (typeof crypto !== "undefined")
                        return crypto.getRandomValues(new Uint8Array(size));
                    if (typeof msCrypto !== "undefined")
                        return msCrypto.getRandomValues(new Uint8Array(size));
                    return FillRandomBytes(new Uint8Array(size), size);
                }
                return FillRandomBytes(new Array(size), size);
            }
            function CreateUUID() {
                const data = GenRandomBytes(UUID_SIZE);
                data[6] = data[6] & 0x4f | 0x40;
                data[8] = data[8] & 0xbf | 0x80;
                let result = "";
                for (let offset = 0; offset < UUID_SIZE; ++offset) {
                    const byte = data[offset];
                    if (offset === 4 || offset === 6 || offset === 8)
                        result += "-";
                    if (byte < 16)
                        result += "0";
                    result += byte.toString(16).toLowerCase();
                }
                return result;
            }
        }
        function MakeDictionary(obj) {
            obj.__ = undefined;
            delete obj.__;
            return obj;
        }
    });
})(Reflect || (Reflect = {}));

var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var r1 = {};

function ip(t, n, i) {
    console.log(`${t} ${n} ${i}`);
}

function b(t, n, i) {
    r1.a = Reflect.getMetadata("design:paramtypes", t, n);
}

var mm = (function () {

    let AtomComponent = class AtomComponent {
    };
    var p = __param(0, b);
    var m = __metadata("design:paramtypes", [String]);

    __decorate([
        ip,
        __metadata("design:type", Object)
    ], AtomComponent.prototype, "data", void 0);

    AtomComponent = __decorate([
        p,
        m
    ], AtomComponent);
    return AtomComponent;
})()

var r = Reflect.getMetadata("design:paramtypes", mm);
assert.strictEqual(r[0], String);
assert.strictEqual(r1.a[0], String);
// console.log(r);
