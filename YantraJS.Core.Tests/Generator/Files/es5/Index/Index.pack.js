var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory() :
        typeof define === 'function' && define.amd ? define(factory) :
            (factory());
}(this, (function () {
    'use strict';
    function finallyConstructor(callback) {
        var constructor = this.constructor;
        return this.then(function (value) {
            return constructor.resolve(callback()).then(function () {
                return value;
            });
        }, function (reason) {
            return constructor.resolve(callback()).then(function () {
                return constructor.reject(reason);
            });
        });
    }
    var setTimeoutFunc = setTimeout;
    function isArray(x) {
        return Boolean(x && typeof x.length !== 'undefined');
    }
    function noop() { }
    function bind(fn, thisArg) {
        return function () {
            fn.apply(thisArg, arguments);
        };
    }
    function Promise(fn) {
        if (!(this instanceof Promise))
            throw new TypeError('Promises must be constructed via new');
        if (typeof fn !== 'function')
            throw new TypeError('not a function');
        this._state = 0;
        this._handled = false;
        this._value = undefined;
        this._deferreds = [];
        doResolve(fn, this);
    }
    function handle(self, deferred) {
        while (self._state === 3) {
            self = self._value;
        }
        if (self._state === 0) {
            self._deferreds.push(deferred);
            return;
        }
        self._handled = true;
        Promise._immediateFn(function () {
            var cb = self._state === 1 ? deferred.onFulfilled : deferred.onRejected;
            if (cb === null) {
                (self._state === 1 ? resolve : reject)(deferred.promise, self._value);
                return;
            }
            var ret;
            try {
                ret = cb(self._value);
            }
            catch (e) {
                reject(deferred.promise, e);
                return;
            }
            resolve(deferred.promise, ret);
        });
    }
    function resolve(self, newValue) {
        try {
            if (newValue === self)
                throw new TypeError('A promise cannot be resolved with itself.');
            if (newValue &&
                (typeof newValue === 'object' || typeof newValue === 'function')) {
                var then = newValue.then;
                if (newValue instanceof Promise) {
                    self._state = 3;
                    self._value = newValue;
                    finale(self);
                    return;
                }
                else if (typeof then === 'function') {
                    doResolve(bind(then, newValue), self);
                    return;
                }
            }
            self._state = 1;
            self._value = newValue;
            finale(self);
        }
        catch (e) {
            reject(self, e);
        }
    }
    function reject(self, newValue) {
        self._state = 2;
        self._value = newValue;
        finale(self);
    }
    function finale(self) {
        if (self._state === 2 && self._deferreds.length === 0) {
            Promise._immediateFn(function () {
                if (!self._handled) {
                    Promise._unhandledRejectionFn(self._value);
                }
            });
        }
        for (var i = 0, len = self._deferreds.length; i < len; i++) {
            handle(self, self._deferreds[i]);
        }
        self._deferreds = null;
    }
    function Handler(onFulfilled, onRejected, promise) {
        this.onFulfilled = typeof onFulfilled === 'function' ? onFulfilled : null;
        this.onRejected = typeof onRejected === 'function' ? onRejected : null;
        this.promise = promise;
    }
    function doResolve(fn, self) {
        var done = false;
        try {
            fn(function (value) {
                if (done)
                    return;
                done = true;
                resolve(self, value);
            }, function (reason) {
                if (done)
                    return;
                done = true;
                reject(self, reason);
            });
        }
        catch (ex) {
            if (done)
                return;
            done = true;
            reject(self, ex);
        }
    }
    Promise.prototype['catch'] = function (onRejected) {
        return this.then(null, onRejected);
    };
    Promise.prototype.then = function (onFulfilled, onRejected) {
        var prom = new this.constructor(noop);
        handle(this, new Handler(onFulfilled, onRejected, prom));
        return prom;
    };
    Promise.prototype['finally'] = finallyConstructor;
    Promise.all = function (arr) {
        return new Promise(function (resolve, reject) {
            if (!isArray(arr)) {
                return reject(new TypeError('Promise.all accepts an array'));
            }
            var args = Array.prototype.slice.call(arr);
            if (args.length === 0)
                return resolve([]);
            var remaining = args.length;
            function res(i, val) {
                try {
                    if (val && (typeof val === 'object' || typeof val === 'function')) {
                        var then = val.then;
                        if (typeof then === 'function') {
                            then.call(val, function (val) {
                                res(i, val);
                            }, reject);
                            return;
                        }
                    }
                    args[i] = val;
                    if (--remaining === 0) {
                        resolve(args);
                    }
                }
                catch (ex) {
                    reject(ex);
                }
            }
            for (var i = 0; i < args.length; i++) {
                res(i, args[i]);
            }
        });
    };
    Promise.resolve = function (value) {
        if (value && typeof value === 'object' && value.constructor === Promise) {
            return value;
        }
        return new Promise(function (resolve) {
            resolve(value);
        });
    };
    Promise.reject = function (value) {
        return new Promise(function (resolve, reject) {
            reject(value);
        });
    };
    Promise.race = function (arr) {
        return new Promise(function (resolve, reject) {
            if (!isArray(arr)) {
                return reject(new TypeError('Promise.race accepts an array'));
            }
            for (var i = 0, len = arr.length; i < len; i++) {
                Promise.resolve(arr[i]).then(resolve, reject);
            }
        });
    };
    Promise._immediateFn =
        (typeof setImmediate === 'function' &&
            function (fn) {
                setImmediate(fn);
            }) ||
            function (fn) {
                setTimeoutFunc(fn, 0);
            };
    Promise._unhandledRejectionFn = function _unhandledRejectionFn(err) {
        if (typeof console !== 'undefined' && console) {
            console.warn('Possible Unhandled Promise Rejection:', err);
        }
    };
    var globalNS = (function () {
        if (typeof self !== 'undefined') {
            return self;
        }
        if (typeof window !== 'undefined') {
            return window;
        }
        if (typeof global !== 'undefined') {
            return global;
        }
        throw new Error('unable to locate global object');
    })();
    if (!('Promise' in globalNS)) {
        globalNS['Promise'] = Promise;
    }
    else if (!globalNS.Promise.prototype['finally']) {
        globalNS.Promise.prototype['finally'] = finallyConstructor;
    }
})));
;
!function (o) { !function (t) { var e = "object" == typeof global ? global : "object" == typeof self ? self : "object" == typeof this ? this : Function("return this;")(), r = n(o); function n(r, n) { return function (t, e) { "function" != typeof r[t] && Object.defineProperty(r, t, { configurable: !0, writable: !0, value: e }), n && n(t, e); }; } void 0 === e.Reflect ? e.Reflect = o : r = n(e.Reflect, r), function (t) { var f = Object.prototype.hasOwnProperty, e = "function" == typeof Symbol, i = e && void 0 !== Symbol.toPrimitive ? Symbol.toPrimitive : "@@toPrimitive", s = e && void 0 !== Symbol.iterator ? Symbol.iterator : "@@iterator", r = "function" == typeof Object.create, n = { __proto__: [] } instanceof Array, o = !r && !n, c = { create: r ? function () { return S(Object.create(null)); } : n ? function () { return S({ __proto__: null }); } : function () { return S({}); }, has: o ? function (t, e) { return f.call(t, e); } : function (t, e) { return e in t; }, get: o ? function (t, e) { return f.call(t, e) ? t[e] : void 0; } : function (t, e) { return t[e]; } }, u = Object.getPrototypeOf(Function), a = "object" == typeof process && process.env && "true" === process.env.REFLECT_METADATA_USE_MAP_POLYFILL, h = a || "function" != typeof Map || "function" != typeof Map.prototype.entries ? function () { var o = {}, r = [], e = function () { function t(t, e, r) { this._index = 0, this._keys = t, this._values = e, this._selector = r; } return t.prototype["@@iterator"] = function () { return this; }, t.prototype[s] = function () { return this; }, t.prototype.next = function () { var t = this._index; if (0 <= t && t < this._keys.length) {
    var e = this._selector(this._keys[t], this._values[t]);
    return t + 1 >= this._keys.length ? (this._index = -1, this._keys = r, this._values = r) : this._index++, { value: e, done: !1 };
} return { value: void 0, done: !0 }; }, t.prototype.throw = function (t) { throw 0 <= this._index && (this._index = -1, this._keys = r, this._values = r), t; }, t.prototype.return = function (t) { return 0 <= this._index && (this._index = -1, this._keys = r, this._values = r), { value: t, done: !0 }; }, t; }(); return function () { function t() { this._keys = [], this._values = [], this._cacheKey = o, this._cacheIndex = -2; } return Object.defineProperty(t.prototype, "size", { get: function () { return this._keys.length; }, enumerable: !0, configurable: !0 }), t.prototype.has = function (t) { return 0 <= this._find(t, !1); }, t.prototype.get = function (t) { var e = this._find(t, !1); return 0 <= e ? this._values[e] : void 0; }, t.prototype.set = function (t, e) { var r = this._find(t, !0); return this._values[r] = e, this; }, t.prototype.delete = function (t) { var e = this._find(t, !1); if (0 <= e) {
    for (var r = this._keys.length, n = e + 1; n < r; n++)
        this._keys[n - 1] = this._keys[n], this._values[n - 1] = this._values[n];
    return this._keys.length--, this._values.length--, t === this._cacheKey && (this._cacheKey = o, this._cacheIndex = -2), !0;
} return !1; }, t.prototype.clear = function () { this._keys.length = 0, this._values.length = 0, this._cacheKey = o, this._cacheIndex = -2; }, t.prototype.keys = function () { return new e(this._keys, this._values, n); }, t.prototype.values = function () { return new e(this._keys, this._values, i); }, t.prototype.entries = function () { return new e(this._keys, this._values, u); }, t.prototype["@@iterator"] = function () { return this.entries(); }, t.prototype[s] = function () { return this.entries(); }, t.prototype._find = function (t, e) { return this._cacheKey !== t && (this._cacheIndex = this._keys.indexOf(this._cacheKey = t)), this._cacheIndex < 0 && e && (this._cacheIndex = this._keys.length, this._keys.push(t), this._values.push(void 0)), this._cacheIndex; }, t; }(); function n(t, e) { return t; } function i(t, e) { return e; } function u(t, e) { return [t, e]; } }() : Map, l = a || "function" != typeof Set || "function" != typeof Set.prototype.entries ? function () { function t() { this._map = new h; } return Object.defineProperty(t.prototype, "size", { get: function () { return this._map.size; }, enumerable: !0, configurable: !0 }), t.prototype.has = function (t) { return this._map.has(t); }, t.prototype.add = function (t) { return this._map.set(t, t), this; }, t.prototype.delete = function (t) { return this._map.delete(t); }, t.prototype.clear = function () { this._map.clear(); }, t.prototype.keys = function () { return this._map.keys(); }, t.prototype.values = function () { return this._map.values(); }, t.prototype.entries = function () { return this._map.entries(); }, t.prototype["@@iterator"] = function () { return this.keys(); }, t.prototype[s] = function () { return this.keys(); }, t; }() : Set, p = new (a || "function" != typeof WeakMap ? function () { var o = 16, e = c.create(), r = n(); return function () { function t() { this._key = n(); } return t.prototype.has = function (t) { var e = i(t, !1); return void 0 !== e && c.has(e, this._key); }, t.prototype.get = function (t) { var e = i(t, !1); return void 0 !== e ? c.get(e, this._key) : void 0; }, t.prototype.set = function (t, e) { var r = i(t, !0); return r[this._key] = e, this; }, t.prototype.delete = function (t) { var e = i(t, !1); return void 0 !== e && delete e[this._key]; }, t.prototype.clear = function () { this._key = n(); }, t; }(); function n() { for (var t; t = "@@WeakMap@@" + a(), c.has(e, t);)
    ; return e[t] = !0, t; } function i(t, e) { if (!f.call(t, r)) {
    if (!e)
        return;
    Object.defineProperty(t, r, { value: c.create() });
} return t[r]; } function u(t, e) { for (var r = 0; r < e; ++r)
    t[r] = 255 * Math.random() | 0; return t; } function a() { var t = function (t) { if ("function" == typeof Uint8Array)
    return "undefined" != typeof crypto ? crypto.getRandomValues(new Uint8Array(t)) : "undefined" != typeof msCrypto ? msCrypto.getRandomValues(new Uint8Array(t)) : u(new Uint8Array(t), t); return u(new Array(t), t); }(o); t[6] = 79 & t[6] | 64, t[8] = 191 & t[8] | 128; for (var e = "", r = 0; r < o; ++r) {
    var n = t[r];
    4 !== r && 6 !== r && 8 !== r || (e += "-"), n < 16 && (e += "0"), e += n.toString(16).toLowerCase();
} return e; } }() : WeakMap); function y(t, e, r) { var n = p.get(t); if (b(n)) {
    if (!r)
        return;
    n = new h, p.set(t, n);
} var o = n.get(e); if (b(o)) {
    if (!r)
        return;
    o = new h, n.set(e, o);
} return o; } function v(t, e, r) { var n = y(e, r, !1); return !b(n) && !!n.has(t); } function _(t, e, r) { var n = y(e, r, !1); if (!b(n))
    return n.get(t); } function d(t, e, r, n) { var o = y(r, n, !0); o.set(t, e); } function w(t, e) { var r = [], n = y(t, e, !1); if (b(n))
    return r; for (var o, i = n.keys(), u = function (t) { var e = M(t, s); if (!j(e))
    throw new TypeError; var r = e.call(t); if (!m(r))
    throw new TypeError; return r; }(i), a = 0;;) {
    var f = (void 0, !(o = u.next()).done && o);
    if (!f)
        return r.length = a, r;
    var c = f.value;
    try {
        r[a] = c;
    }
    catch (t) {
        try {
            A(u);
        }
        finally {
            throw t;
        }
    }
    a++;
} } function g(t) { if (null === t)
    return 1; switch (typeof t) {
    case "undefined": return 0;
    case "boolean": return 2;
    case "string": return 3;
    case "symbol": return 4;
    case "number": return 5;
    case "object": return null === t ? 1 : 6;
    default: return 6;
} } function b(t) { return void 0 === t; } function k(t) { return null === t; } function m(t) { return "object" == typeof t ? null !== t : "function" == typeof t; } function E(t, e) { switch (g(t)) {
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
    case 5: return t;
} var r = 3 === e ? "string" : 5 === e ? "number" : "default", n = M(t, i); if (void 0 !== n) {
    var o = n.call(t, r);
    if (m(o))
        throw new TypeError;
    return o;
} return function (t, e) { if ("string" === e) {
    var r = t.toString;
    if (j(r)) {
        var n = r.call(t);
        if (!m(n))
            return n;
    }
    var o = t.valueOf;
    if (j(o)) {
        var n = o.call(t);
        if (!m(n))
            return n;
    }
}
else {
    var o = t.valueOf;
    if (j(o)) {
        var n = o.call(t);
        if (!m(n))
            return n;
    }
    var i = t.toString;
    if (j(i)) {
        var n = i.call(t);
        if (!m(n))
            return n;
    }
} throw new TypeError; }(t, "default" === r ? "number" : r); } function T(t) { var e = E(t, 3); return "symbol" == typeof e ? e : "" + e; } function O(t) { return Array.isArray ? Array.isArray(t) : t instanceof Object ? t instanceof Array : "[object Array]" === Object.prototype.toString.call(t); } function j(t) { return "function" == typeof t; } function x(t) { return "function" == typeof t; } function M(t, e) { var r = t[e]; if (null != r) {
    if (!j(r))
        throw new TypeError;
    return r;
} } function A(t) { var e = t.return; e && e.call(t); } function P(t) { var e = Object.getPrototypeOf(t); if ("function" != typeof t || t === u)
    return e; if (e !== u)
    return e; var r = t.prototype, n = r && Object.getPrototypeOf(r); if (null == n || n === Object.prototype)
    return e; var o = n.constructor; return "function" != typeof o ? e : o === t ? e : o; } function S(t) { return t.__ = void 0, delete t.__, t; } t("decorate", function (t, e, r, n) { {
    if (b(r)) {
        if (!O(t))
            throw new TypeError;
        if (!x(e))
            throw new TypeError;
        return function (t, e) { for (var r = t.length - 1; 0 <= r; --r) {
            var n = t[r], o = n(e);
            if (!b(o) && !k(o)) {
                if (!x(o))
                    throw new TypeError;
                e = o;
            }
        } return e; }(t, e);
    }
    if (!O(t))
        throw new TypeError;
    if (!m(e))
        throw new TypeError;
    if (!m(n) && !b(n) && !k(n))
        throw new TypeError;
    return k(n) && (n = void 0), r = T(r), function (t, e, r, n) { for (var o = t.length - 1; 0 <= o; --o) {
        var i = t[o], u = i(e, r, n);
        if (!b(u) && !k(u)) {
            if (!m(u))
                throw new TypeError;
            n = u;
        }
    } return n; }(t, e, r, n);
} }), t("metadata", function (r, n) { return function (t, e) { if (!m(t))
    throw new TypeError; if (!b(e) && !function (t) { switch (g(t)) {
    case 3:
    case 4: return !0;
    default: return !1;
} }(e))
    throw new TypeError; d(r, n, t, e); }; }), t("defineMetadata", function (t, e, r, n) { if (!m(r))
    throw new TypeError; b(n) || (n = T(n)); return d(t, e, r, n); }), t("hasMetadata", function (t, e, r) { if (!m(e))
    throw new TypeError; b(r) || (r = T(r)); return function t(e, r, n) { var o = v(e, r, n); if (o)
    return !0; var i = P(r); if (!k(i))
    return t(e, i, n); return !1; }(t, e, r); }), t("hasOwnMetadata", function (t, e, r) { if (!m(e))
    throw new TypeError; b(r) || (r = T(r)); return v(t, e, r); }), t("getMetadata", function (t, e, r) { if (!m(e))
    throw new TypeError; b(r) || (r = T(r)); return function t(e, r, n) { var o = v(e, r, n); if (o)
    return _(e, r, n); var i = P(r); if (!k(i))
    return t(e, i, n); return; }(t, e, r); }), t("getOwnMetadata", function (t, e, r) { if (!m(e))
    throw new TypeError; b(r) || (r = T(r)); return _(t, e, r); }), t("getMetadataKeys", function (t, e) { if (!m(t))
    throw new TypeError; b(e) || (e = T(e)); return function t(e, r) { var n = w(e, r); var o = P(e); if (null === o)
    return n; var i = t(o, r); if (i.length <= 0)
    return n; if (n.length <= 0)
    return i; var u = new l; var a = []; for (var f = 0, c = n; f < c.length; f++) {
    var s = c[f], h = u.has(s);
    h || (u.add(s), a.push(s));
} for (var p = 0, y = i; p < y.length; p++) {
    var s = y[p], h = u.has(s);
    h || (u.add(s), a.push(s));
} return a; }(t, e); }), t("getOwnMetadataKeys", function (t, e) { if (!m(t))
    throw new TypeError; b(e) || (e = T(e)); return w(t, e); }), t("deleteMetadata", function (t, e, r) { if (!m(e))
    throw new TypeError; b(r) || (r = T(r)); var n = y(e, r, !1); if (b(n))
    return !1; if (!n.delete(t))
    return !1; if (0 < n.size)
    return !0; var o = p.get(e); return o.delete(r), 0 < o.size || p.delete(e), !0; }); }(r); }(); }(Reflect || (Reflect = {}));
if (!Array.prototype.find) {
    Array.prototype.find = function (predicate, thisArg) {
        for (let i = 0; i < this.length; i++) {
            const item = this[i];
            if (predicate(item, i, this)) {
                return item;
            }
        }
    };
}
if (!Array.prototype.findIndex) {
    Array.prototype.findIndex = function (predicate, thisArg) {
        for (let i = 0; i < this.length; i++) {
            const item = this[i];
            if (predicate(item, i, this)) {
                return i;
            }
        }
        return -1;
    };
}
if (!Array.prototype.map) {
    Array.prototype.map = function (callbackfn, thisArg) {
        const a = [];
        for (let i = 0; i < this.length; i++) {
            const r = callbackfn(this[i], i, this);
            if (r !== undefined) {
                a.push(r);
            }
        }
        return a;
    };
}
if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, endPosition) {
        const index = this.indexOf(searchString, endPosition);
        return index === 0;
    };
}
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, endPosition) {
        const index = this.lastIndexOf(searchString, endPosition);
        if (index === -1) {
            return false;
        }
        const l = this.length - index;
        return l === searchString.length;
    };
}
if (!Number.parseInt) {
    Number.parseInt = function (n) {
        return Math.floor(parseFloat(n));
    };
}
if (!Number.parseFloat) {
    Number.parseFloat = function (n) {
        return parseFloat(n);
    };
}
if (typeof Element !== "undefined") {
    if (!("remove" in Element.prototype)) {
        Element.prototype["remove"] = function () {
            if (this.parentNode) {
                this.parentNode.removeChild(this);
            }
        };
    }
}
class Module {
    constructor(name, folder) {
        this.name = name;
        this.folder = folder;
        this.emptyExports = {};
        this.ignoreModule = null;
        this.isLoaded = false;
        this.isResolved = false;
        this.dependencies = [];
        this.rID = null;
        const index = name.lastIndexOf("/");
        if (index === -1) {
            this.folder = "";
        }
        else {
            this.folder = name.substr(0, index);
        }
    }
    get filename() {
        return this.name;
    }
    get dependents() {
        const d = [];
        const v = {};
        const modules = AmdLoader.instance.modules;
        for (const key in modules) {
            if (modules.hasOwnProperty(key)) {
                const element = modules[key];
                if (element.isDependentOn(this, v)) {
                    d.push(element);
                }
            }
        }
        return d;
    }
    resolve(id) {
        if (!this.isLoaded) {
            return false;
        }
        if (this.isResolved) {
            return true;
        }
        if (!id) {
            id = Module.nextID++;
        }
        if (this.rID === id) {
            let childrenResolved = true;
            for (const iterator of this.dependencies) {
                if (iterator === this.ignoreModule) {
                    continue;
                }
                if (iterator.rID === id) {
                    continue;
                }
                if (!iterator.resolve(id)) {
                    childrenResolved = false;
                    break;
                }
            }
            return childrenResolved;
        }
        this.rID = id;
        let allResolved = true;
        for (const iterator of this.dependencies) {
            if (iterator === this.ignoreModule) {
                continue;
            }
            if (!iterator.resolve(id)) {
                allResolved = false;
                break;
            }
        }
        if (!allResolved) {
            this.rID = 0;
            return false;
        }
        const i = AmdLoader.instance;
        if (this.dependencyHooks) {
            this.dependencyHooks[0]();
            this.dependencyHooks = null;
        }
        if (this.resolveHooks) {
            this.resolveHooks[0](this.getExports());
            this.resolveHooks = null;
            i.remove(this);
            this.rID = 0;
            return true;
        }
        this.rID = 0;
        return false;
    }
    addDependency(d) {
        if (d === this.ignoreModule) {
            return;
        }
        this.dependencies.push(d);
    }
    getExports() {
        this.exports = this.emptyExports;
        if (this.factory) {
            try {
                const factory = this.factory;
                this.factory = null;
                delete this.factory;
                AmdLoader.instance.currentStack.push(this);
                const result = factory(this.require, this.exports);
                if (result) {
                    if (typeof result === "object") {
                        for (const key in result) {
                            if (result.hasOwnProperty(key)) {
                                const element = result[key];
                                this.exports[key] = element;
                            }
                        }
                    }
                    else if (!this.exports.default) {
                        this.exports.default = result;
                    }
                }
                AmdLoader.instance.currentStack.pop();
                delete this.factory;
                const def = this.exports.default;
                if (def && typeof def === "object") {
                    def[UMD.nameSymbol] = this.name;
                }
            }
            catch (e) {
                const em = e.stack ? (`${e}\n${e.stack}`) : e;
                const s = [];
                for (const iterator of AmdLoader.instance.currentStack) {
                    s.push(iterator.name);
                }
                const ne = new Error(`Failed loading module ${this.name} with error ${em}\nDependents: ${s.join("\n\t")}`);
                console.error(ne);
                throw ne;
            }
        }
        return this.exports;
    }
    isDependentOn(m, visited) {
        visited[this.name] = true;
        for (const iterator of this.dependencies) {
            if (iterator.name === m.name) {
                return true;
            }
            if (visited[iterator.name]) {
                continue;
            }
            if (iterator.isDependentOn(m, visited)) {
                return true;
            }
        }
        return false;
    }
}
Module.nextID = 1;
if (typeof require !== "undefined") {
    md = require("module").Module;
}
class AmdLoader {
    constructor() {
        this.root = null;
        this.defaultUrl = null;
        this.currentStack = [];
        this.nodeModules = [];
        this.modules = {};
        this.pathMap = {};
        this.mockTypes = [];
        this.lastTimeout = null;
        this.dirty = false;
    }
    register(packages, modules) {
        for (const iterator of packages) {
            if (!this.pathMap[iterator]) {
                this.map(iterator, "/");
            }
        }
        for (const iterator of modules) {
            this.get(iterator);
        }
    }
    setupRoot(root, url) {
        if (url.endsWith("/")) {
            url = url.substr(0, url.length - 1);
        }
        for (const key in this.pathMap) {
            if (this.pathMap.hasOwnProperty(key)) {
                const moduleUrl = key === root ? url : `${url}/node_modules/${key}`;
                this.map(key, moduleUrl);
            }
        }
        this.defaultUrl = `${url}/node_modules/`;
    }
    registerModule(name, moduleExports) {
        const m = this.get(name);
        m.package.url = "/";
        m.exports = Object.assign({ __esModule: true }, moduleExports);
        m.loader = Promise.resolve();
        m.resolver = Promise.resolve(m.exports);
        m.isLoaded = true;
        m.isResolved = true;
    }
    setup(name) {
        const jsModule = this.get(name);
        const define = this.define;
        jsModule.loader = Promise.resolve();
        AmdLoader.current = jsModule;
        if (define) {
            define();
        }
        if (jsModule.exportVar) {
            jsModule.exports = AmdLoader.globalVar[jsModule.exportVar];
        }
        this.push(jsModule);
        jsModule.isLoaded = true;
        setTimeout(() => {
            this.loadDependencies(jsModule);
        }, 1);
    }
    loadDependencies(m) {
        this.resolveModule(m).catch((e) => {
            console.error(e);
        });
        if (m.dependencies.length) {
            const all = m.dependencies.map((m1) => __awaiter(this, void 0, void 0, function* () {
                if (m1.isResolved) {
                    return;
                }
                yield this.import(m1);
            }));
            Promise.all(all).catch((e) => {
                console.error(e);
            }).then(() => {
                m.resolve();
            });
        }
        else {
            m.resolve();
        }
        this.queueResolveModules(1);
    }
    replace(type, name, mock) {
        if (mock && !this.enableMock) {
            return;
        }
        const peek = this.currentStack.length ? this.currentStack[this.currentStack.length - 1] : undefined;
        const rt = new MockType(peek, type, name, mock);
        this.mockTypes.push(rt);
    }
    resolveType(type) {
        const t = this.mockTypes.find((tx) => tx.type === type);
        return t ? t.replaced : type;
    }
    map(packageName, packageUrl, type = "amd", exportVar) {
        let existing = this.pathMap[packageName];
        if (existing) {
            existing.url = packageUrl;
            existing.exportVar = exportVar;
            existing.type = type;
            return existing;
        }
        existing = {
            name: packageName,
            url: packageUrl,
            type,
            exportVar,
            version: ""
        };
        if (packageName === "reflect-metadata") {
            type = "global";
        }
        this.pathMap[packageName] = existing;
        return existing;
    }
    resolveSource(name, defExt = ".js") {
        try {
            if (/^((\/)|((http|https)\:\/\/))/i.test(name)) {
                return name;
            }
            let path = null;
            for (const key in this.pathMap) {
                if (this.pathMap.hasOwnProperty(key)) {
                    const packageName = key;
                    if (name.startsWith(packageName)) {
                        path = this.pathMap[key].url;
                        if (name.length !== packageName.length) {
                            if (name[packageName.length] !== "/") {
                                continue;
                            }
                            name = name.substr(packageName.length + 1);
                        }
                        else {
                            return path;
                        }
                        if (path.endsWith("/")) {
                            path = path.substr(0, path.length - 1);
                        }
                        path = path + "/" + name;
                        const i = name.lastIndexOf("/");
                        const fileName = name.substr(i + 1);
                        if (fileName.indexOf(".") === -1) {
                            path = path + defExt;
                        }
                        return path;
                    }
                }
            }
            return name;
        }
        catch (e) {
            console.error(`Failed to resolve ${name} with error ${JSON.stringify(e)}`);
            console.error(e);
        }
    }
    resolveRelativePath(name, currentPackage) {
        if (name.charAt(0) !== ".") {
            return name;
        }
        const tokens = name.split("/");
        const currentTokens = currentPackage.split("/");
        currentTokens.pop();
        while (tokens.length) {
            const first = tokens[0];
            if (first === "..") {
                currentTokens.pop();
                tokens.splice(0, 1);
                continue;
            }
            if (first === ".") {
                tokens.splice(0, 1);
            }
            break;
        }
        return `${currentTokens.join("/")}/${tokens.join("/")}`;
    }
    getPackageVersion(name) {
        let [scope, packageName] = name.split("/", 3);
        let version = "";
        if (scope[0] !== "@") {
            packageName = scope;
            scope = "";
        }
        else {
            scope += "/";
        }
        const versionTokens = packageName.split("@");
        if (versionTokens.length > 1) {
            version = versionTokens[1];
            name = name.replace("@" + version, "");
        }
        packageName = scope + packageName;
        return { packageName, version, name };
    }
    get(name1) {
        let module = this.modules[name1];
        if (!module) {
            const { packageName, version, name } = this.getPackageVersion(name1);
            module = new Module(name);
            this.modules[name1] = module;
            module.package = this.pathMap[packageName] ||
                (this.pathMap[packageName] = {
                    type: "amd",
                    name: packageName,
                    version,
                    url: this.defaultUrl ?
                        (this.defaultUrl + packageName) : undefined
                });
            module.url = this.resolveSource(name);
            if (!module.url) {
                if (typeof require === "undefined") {
                    throw new Error(`No url mapped for ${name}`);
                }
            }
            module.require = (n) => {
                const an = this.resolveRelativePath(n, module.name);
                const resolvedModule = this.get(an);
                const m = resolvedModule.getExports();
                return m;
            };
            module.require.resolve = (n) => this.resolveRelativePath(n, module.name);
            this.modules[name] = module;
        }
        return module;
    }
    import(name) {
        return __awaiter(this, void 0, void 0, function* () {
            if (typeof require !== "undefined") {
                return Promise.resolve(require(name));
            }
            const module = typeof name === "object" ? name : this.get(name);
            yield this.load(module);
            const e = yield this.resolveModule(module);
            return e;
        });
    }
    load(module) {
        if (module.loader) {
            return module.loader;
        }
        this.push(module);
        if (AmdLoader.isJson.test(module.url)) {
            const mUrl = module.package.url + module.url;
            module.loader = new Promise((resolve, reject) => {
                try {
                    AmdLoader.httpTextLoader(mUrl, (r) => {
                        try {
                            module.exports = JSON.parse(r);
                            module.emptyExports = module.exports;
                            module.isLoaded = true;
                            setTimeout(() => this.loadDependencies(module), 1);
                            resolve();
                        }
                        catch (e) {
                            reject(e);
                        }
                    }, reject);
                }
                catch (e1) {
                    reject(e1);
                }
            });
        }
        if (AmdLoader.isMedia.test(module.url)) {
            const mUrl = !module.url.startsWith(module.package.url)
                ? (module.package.url + module.url)
                : module.url;
            const m = {
                url: mUrl,
                toString: () => mUrl
            };
            const e = { __esModule: true, default: m };
            module.exports = e;
            module.emptyExports = e;
            module.loader = Promise.resolve();
            module.isLoaded = true;
            return module.loader;
        }
        module.loader = new Promise((resolve, reject) => {
            AmdLoader.moduleLoader(module.name, module.url, () => {
                try {
                    AmdLoader.current = module;
                    if (AmdLoader.instance.define) {
                        AmdLoader.instance.define();
                        AmdLoader.instance.define = null;
                    }
                    if (module.exportVar) {
                        module.exports = AmdLoader.globalVar[module.exportVar];
                    }
                    if (AmdLoader.moduleProgress) {
                        AmdLoader.moduleProgress(module.name, this.modules, "loading");
                    }
                    module.isLoaded = true;
                    setTimeout(() => {
                        this.loadDependencies(module);
                    }, 1);
                    resolve();
                }
                catch (e) {
                    console.error(e);
                    reject(e);
                }
            }, (error) => {
                reject(error);
            });
        });
        return module.loader;
    }
    resolveModule(module) {
        if (module.resolver) {
            return module.resolver;
        }
        module.resolver = this._resolveModule(module);
        return module.resolver;
    }
    remove(m) {
        if (this.tail === m) {
            this.tail = m.previous;
        }
        if (m.next) {
            m.next.previous = m.previous;
        }
        if (m.previous) {
            m.previous.next = m.next;
        }
        m.next = null;
        m.previous = null;
        this.dirty = true;
        this.queueResolveModules();
    }
    queueResolveModules(n = 1) {
        if (this.lastTimeout) {
            return;
        }
        this.lastTimeout = setTimeout(() => {
            this.lastTimeout = 0;
            this.resolvePendingModules();
        }, n);
    }
    watch() {
        const id = setInterval(() => {
            if (this.tail) {
                const list = [];
                for (const key in this.modules) {
                    if (this.modules.hasOwnProperty(key)) {
                        const element = this.modules[key];
                        if (!element.isResolved) {
                            list.push({
                                name: element.name,
                                dependencies: element.dependencies.map((x) => x.name)
                            });
                        }
                    }
                }
                console.log("Pending modules");
                console.log(JSON.stringify(list));
                return;
            }
            clearInterval(id);
        }, 10000);
    }
    resolvePendingModules() {
        if (!this.tail) {
            return;
        }
        this.dirty = false;
        const pending = [];
        let m = this.tail;
        while (m) {
            if (!m.dependencies.length) {
                m.resolve();
            }
            else {
                pending.push(m);
            }
            m = m.previous;
        }
        if (this.dirty) {
            this.dirty = false;
            return;
        }
        for (const iterator of pending) {
            iterator.resolve();
        }
        if (this.dirty) {
            this.dirty = false;
            return;
        }
        if (this.tail) {
            this.queueResolveModules();
        }
    }
    push(m) {
        if (this.tail) {
            m.previous = this.tail;
            this.tail.next = m;
        }
        this.tail = m;
    }
    _resolveModule(module) {
        return __awaiter(this, void 0, void 0, function* () {
            if (!this.root) {
                this.root = module;
            }
            yield new Promise((resolve, reject) => {
                module.dependencyHooks = [resolve, reject];
            });
            const exports = module.getExports();
            const pendingList = this.mockTypes.filter((t) => !t.loaded);
            if (pendingList.length) {
                for (const iterator of pendingList) {
                    iterator.loaded = true;
                }
                const tasks = pendingList.map((iterator) => __awaiter(this, void 0, void 0, function* () {
                    const containerModule = iterator.module;
                    const resolvedName = this.resolveRelativePath(iterator.moduleName, containerModule.name);
                    const im = this.get(resolvedName);
                    im.ignoreModule = module;
                    const ex = yield this.import(im);
                    const type = ex[iterator.exportName];
                    iterator.replaced = type;
                }));
                yield Promise.all(tasks);
            }
            const setHooks = new Promise((resolve, reject) => {
                module.resolveHooks = [resolve, reject];
            });
            yield setHooks;
            if (this.root === module) {
                this.root = null;
                AmdLoader.moduleProgress(null, this.modules, "done");
            }
            module.isResolved = true;
            return exports;
        });
    }
}
AmdLoader.isMedia = /\.(jpg|jpeg|gif|png|mp4|mp3|css|html|svg)$/i;
AmdLoader.isJson = /\.json$/i;
AmdLoader.globalVar = {};
AmdLoader.instance = new AmdLoader();
AmdLoader.current = null;
const a = AmdLoader.instance;
a.map("global", "/", "global");
a.registerModule("global/document", { default: document });
a.registerModule("global/window", { default: typeof window !== "undefined" ? window : global });
a.map("reflect-metadata", "/", "global");
a.registerModule("reflect-metadata", Reflect);
AmdLoader.moduleLoader = (name, url, success, error) => {
    const script = document.createElement("script");
    script.type = "text/javascript";
    script.src = url;
    const s = script;
    script.onload = s.onreadystatechange = () => {
        if ((s.readyState && s.readyState !== "complete" && s.readyState !== "loaded")) {
            return;
        }
        script.onload = s.onreadystatechange = null;
        success();
    };
    script.onerror = (e) => { error(e); };
    document.body.appendChild(script);
};
AmdLoader.httpTextLoader = (url, success, error) => {
    const xhr = new XMLHttpRequest();
    xhr.onreadystatechange = (e) => {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                success(xhr.responseText);
            }
            else {
                error(xhr.responseText);
            }
        }
    };
    xhr.open("GET", url);
    xhr.send();
};
AmdLoader.moduleProgress = (() => {
    if (!document) {
        return (name, p) => {
            console.log(`${name} ${p}%`);
        };
    }
    const progressDiv = document.createElement("div");
    progressDiv.className = "web-atoms-progress-div";
    const style = progressDiv.style;
    style.position = "absolute";
    style.margin = "auto";
    style.width = "200px";
    style.height = "100px";
    style.top = style.right = style.left = style.bottom = "5px";
    style.borderStyle = "solid";
    style.borderWidth = "1px";
    style.borderColor = "#A0A0A0";
    style.borderRadius = "5px";
    style.padding = "5px";
    style.textAlign = "left";
    style.verticalAlign = "bottom";
    const progressLabel = document.createElement("pre");
    progressDiv.appendChild(progressLabel);
    progressLabel.style.color = "#A0A0A0";
    const ps = progressLabel.style;
    ps.position = "absolute";
    ps.left = "5px";
    ps.bottom = "0";
    function ready() {
        document.body.appendChild(progressDiv);
    }
    function completed() {
        document.removeEventListener("DOMContentLoaded", completed);
        window.removeEventListener("load", completed);
        ready();
    }
    if (document.readyState === "complete" ||
        (document.readyState !== "loading" && !document.documentElement["doScroll"])) {
        window.setTimeout(ready);
    }
    else {
        document.addEventListener("DOMContentLoaded", completed);
        window.addEventListener("load", completed);
    }
    return (name, n, status) => {
        if (status === "done") {
            progressDiv.style.display = "none";
            return;
        }
        else {
            progressDiv.style.display = "block";
        }
        name = name.split("/").pop();
        progressLabel.textContent = name;
    };
})();
var define = (requiresOrFactory, factory, nested) => {
    const loader = AmdLoader.instance;
    function bindFactory(module, requireList, fx) {
        if (module.factory) {
            return;
        }
        module.dependencies = [];
        let requires = requireList;
        requires = requireList;
        const args = [];
        for (const s of requires) {
            if (s === "require") {
                args.push(module.require);
                continue;
            }
            if (s === "exports") {
                args.push(module.emptyExports);
                continue;
            }
            if (/^global/.test(s)) {
                args.push(loader.get(s).exports);
            }
            const name = loader.resolveRelativePath(s, module.name);
            const child = loader.get(name);
            module.addDependency(child);
        }
        module.factory = () => {
            return fx.apply(module, args);
        };
    }
    if (nested) {
        const name = requiresOrFactory;
        const rList = factory;
        const f = nested;
        const module = AmdLoader.instance.get(name);
        bindFactory(module, rList, f);
        setTimeout(() => {
            loader.loadDependencies(module);
        }, 1);
        return;
    }
    AmdLoader.instance.define = () => {
        const current = AmdLoader.current;
        if (!current) {
            return;
        }
        if (current.factory) {
            return;
        }
        if (typeof requiresOrFactory === "function") {
            bindFactory(current, [], requiresOrFactory);
        }
        else {
            bindFactory(current, requiresOrFactory, factory);
        }
    };
};
define.amd = {};
class MockType {
    constructor(module, type, name, mock, moduleName, exportName) {
        this.module = module;
        this.type = type;
        this.name = name;
        this.mock = mock;
        this.moduleName = moduleName;
        this.exportName = exportName;
        this.loaded = false;
        this.name = name = name
            .replace("{lang}", UMD.lang)
            .replace("{platform}", UMD.viewPrefix);
        if (name.indexOf("$") !== -1) {
            const tokens = name.split("$");
            this.moduleName = tokens[0];
            this.exportName = tokens[1] || tokens[0].split("/").pop();
        }
        else {
            this.moduleName = name;
            this.exportName = "default";
        }
    }
}
class UMDClass {
    constructor() {
        this.viewPrefix = "web";
        this.defaultApp = "@web-atoms/core/dist/web/WebApp";
        this.lang = "en-US";
        this.nameSymbol = typeof Symbol !== "undefined" ? Symbol() : "_$_nameSymbol";
    }
    get mock() {
        return AmdLoader.instance.enableMock;
    }
    set mock(v) {
        AmdLoader.instance.enableMock = v;
    }
    resolvePath(n) {
        return AmdLoader.instance.resolveSource(n, null);
    }
    resolveViewPath(path) {
        return path.replace("{platform}", this.viewPrefix);
    }
    resolveType(type) {
        return AmdLoader.instance.resolveType(type);
    }
    map(name, path, type = "amd", exportVar) {
        AmdLoader.instance.map(name, path, type, exportVar);
    }
    setupRoot(name, url) {
        AmdLoader.instance.setupRoot(name, url);
    }
    mockType(type, name) {
        AmdLoader.instance.replace(type, name, true);
    }
    inject(type, name) {
        AmdLoader.instance.replace(type, name, false);
    }
    resolveViewClassAsync(path) {
        return __awaiter(this, void 0, void 0, function* () {
            path = this.resolveViewPath(path);
            const e = yield AmdLoader.instance.import(path);
            return e.default;
        });
    }
    import(path) {
        return AmdLoader.instance.import(path);
    }
    load(path, designMode) {
        return __awaiter(this, void 0, void 0, function* () {
            this.mock = designMode;
            const t = yield AmdLoader.instance.import("@web-atoms/core/dist/core/types");
            const a = yield AmdLoader.instance.import("@web-atoms/core/dist/Atom");
            a.Atom.designMode = designMode;
            const al = yield AmdLoader.instance.import("@web-atoms/core/dist/core/AtomList");
            return yield AmdLoader.instance.import(path);
        });
    }
    hostView(id, path, designMode) {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                this.mock = designMode;
                AmdLoader.instance.get(path);
                const m = yield this.load(this.defaultApp, designMode);
                const app = new (m.default)();
                app.onReady(() => __awaiter(this, void 0, void 0, function* () {
                    try {
                        const viewClass = yield AmdLoader.instance.import(path);
                        const view = new (viewClass.default)(app);
                        const element = document.getElementById(id);
                        element.appendChild(view.element);
                    }
                    catch (e) {
                        console.error(e);
                    }
                }));
            }
            catch (e) {
                console.error(e);
            }
        });
    }
    loadView(path, designMode, appPath) {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                this.mock = designMode;
                appPath = appPath || this.defaultApp;
                AmdLoader.instance.get(path);
                const m = yield this.load(appPath, designMode);
                const app = new (m.default)();
                return yield new Promise((resolve, reject) => {
                    app.onReady(() => __awaiter(this, void 0, void 0, function* () {
                        try {
                            const viewClass = yield AmdLoader.instance.import(path);
                            const view = new (viewClass.default)(app);
                            app.root = view;
                            resolve(view);
                        }
                        catch (e) {
                            console.error(e);
                            reject(e);
                        }
                    }));
                });
            }
            catch (er) {
                console.error(er);
                throw er;
            }
        });
    }
}
const UMD = new UMDClass();
((u) => {
    const globalNS = (typeof window !== "undefined" ? window : global);
    globalNS.UMD = u;
    globalNS.AmdLoader = AmdLoader;
})(UMD);
//# sourceMappingURL=umd.js.map

        AmdLoader.instance.register(
            ["@web-atoms/xf-samples","@web-atoms/core","@web-atoms/font-awesome","@web-atoms/xf-controls","@web-atoms/date-time","@web-atoms/storage"],
            ["@web-atoms/xf-samples/dist/Index","@web-atoms/core/dist/Atom","@web-atoms/core/dist/core/AtomList","@web-atoms/core/dist/core/AtomBinder","@web-atoms/core/dist/core/types","@web-atoms/core/dist/core/AtomMap","@web-atoms/xf-samples/dist/app-host/AppHost","@web-atoms/core/dist/core/Bind","@web-atoms/core/dist/core/ExpressionParser","@web-atoms/core/dist/core/XNode","@web-atoms/font-awesome/dist/FontAwesomeSolid","@web-atoms/xf-controls/dist/clr/WA","@web-atoms/xf-controls/dist/clr/XF","@web-atoms/xf-controls/dist/clr/X","@web-atoms/core/dist/core/AtomBridge","@web-atoms/core/dist/web/core/AtomUI","@web-atoms/xf-controls/dist/pages/AtomXFMasterDetailPage","@web-atoms/core/dist/xf/controls/AtomXFControl","@web-atoms/core/dist/core/AtomComponent","@web-atoms/core/dist/App","@web-atoms/core/dist/core/AtomDispatcher","@web-atoms/core/dist/di/RegisterSingleton","@web-atoms/core/dist/di/Register","@web-atoms/core/dist/di/ServiceCollection","@web-atoms/core/dist/di/TypeKey","@web-atoms/core/dist/di/ServiceProvider","@web-atoms/core/dist/core/TransientDisposable","@web-atoms/core/dist/di/Inject","@web-atoms/core/dist/services/BusyIndicatorService","@web-atoms/core/dist/core/PropertyBinding","@web-atoms/core/dist/core/AtomOnce","@web-atoms/core/dist/core/AtomWatcher","@web-atoms/core/dist/core/AtomDisposableList","@web-atoms/core/dist/core/InheritedProperty","@web-atoms/core/dist/core/PropertyMap","@web-atoms/core/dist/services/NavigationService","@web-atoms/core/dist/core/AtomUri","@web-atoms/core/dist/core/FormattedString","@web-atoms/core/dist/services/ReferenceService","@web-atoms/core/dist/di/DISingleton","@web-atoms/core/dist/web/styles/AtomStyle","@web-atoms/core/dist/core/StringHelper","@web-atoms/core/dist/web/styles/AtomStyleSheet","@web-atoms/xf-samples/dist/app-host/AppHostViewModel","@web-atoms/core/dist/core/AtomLoader","@web-atoms/core/dist/services/JsonService","@web-atoms/date-time/dist/DateTime","@web-atoms/date-time/dist/TimeSpan","@web-atoms/core/dist/view-model/AtomViewModel","@web-atoms/core/dist/core/BindableProperty","@web-atoms/core/dist/view-model/baseTypes","@web-atoms/core/dist/view-model/AtomWindowViewModel","@web-atoms/core/dist/view-model/Load","@web-atoms/xf-samples/dist/samples/alert/AlertSamplePage","@web-atoms/xf-samples/dist/samples/alert/AlertSample","@web-atoms/xf-controls/dist/pages/AtomXFContentPage","@web-atoms/xf-samples/dist/samples/alert/AlertSampleViewModel","@web-atoms/xf-samples/dist/samples/alert/custom-popup/CustomPopupSample","@web-atoms/xf-samples/dist/samples/alert/custom-popup/CustomPopupViewModel","@web-atoms/xf-samples/dist/samples/box/BoxViewSample","@web-atoms/xf-samples/dist/samples/box/BoxView","@web-atoms/xf-samples/dist/samples/brushes/addBrushSamples","@web-atoms/xf-samples/dist/samples/brushes/gradient/linear/LinearGradient","@web-atoms/xf-samples/dist/samples/brushes/gradient/radial/RadialGradient","@web-atoms/core/dist/core/Colors","@web-atoms/xf-samples/dist/samples/brushes/solid/SolidBrush","@web-atoms/xf-samples/dist/samples/calendar/calendarSamples","@web-atoms/xf-samples/dist/samples/calendar/Calendar","@web-atoms/xf-controls/dist/calendar/AtomXFCalendar","@web-atoms/xf-controls/dist/combo-box/AtomXFComboBox","@web-atoms/xf-controls/dist/clr/RgPluginsPopup","@web-atoms/xf-controls/dist/pages/AtomXFPopupPage","@web-atoms/xf-controls/dist/combo-box/SearchPageViewModel","@web-atoms/xf-controls/dist/combo-box/SelectionList","@web-atoms/xf-controls/dist/AtomContentView","@web-atoms/xf-controls/dist/controls/AtomXFGrid","@web-atoms/xf-controls/dist/calendar/AtomXFCalendarStyle","@web-atoms/xf-controls/dist/calendar/AtomXFCalendarViewModel","@web-atoms/xf-samples/dist/samples/carousel/carousel-page/CarouselPageSample","@web-atoms/xf-samples/dist/samples/carousel/carousel-page/CarouselPageView","@web-atoms/xf-controls/dist/pages/AtomXFCarouselPage","@web-atoms/xf-samples/dist/samples/carousel/carousel-view/CarouselSample","@web-atoms/xf-samples/dist/samples/carousel/carousel-view/CarouselView","@web-atoms/xf-samples/dist/samples/carousel/carousel-view/CarouselViewModel","@web-atoms/xf-samples/dist/samples/collection-view/CollectionViewSamplePage","@web-atoms/xf-samples/dist/samples/collection-view/grouping/GroupingSample","@web-atoms/xf-samples/dist/samples/collection-view/grouping/GroupingViewModel","@web-atoms/xf-samples/dist/samples/collection-view/header-footer/HeaderFooterSample","@web-atoms/xf-samples/dist/samples/collection-view/header-footer/HeaderFooterViewModel","@web-atoms/xf-samples/dist/samples/collection-view/horizontal-grid/HorizontalGridSample","@web-atoms/xf-samples/dist/samples/collection-view/horizontal-grid/HorizontalGridViewModel","@web-atoms/xf-samples/dist/samples/collection-view/horizontal-list/HorizontalListSample","@web-atoms/xf-samples/dist/samples/collection-view/horizontal-list/HorizontalListViewModel","@web-atoms/xf-samples/dist/samples/collection-view/vertical-grid/VerticalGridSample","@web-atoms/xf-samples/dist/samples/collection-view/vertical-grid/VerticalGridViewModel","@web-atoms/xf-samples/dist/samples/collection-view/vertical-list/VerticalListSample","@web-atoms/xf-samples/dist/samples/collection-view/vertical-list/VerticalListViewModel","@web-atoms/xf-samples/dist/samples/database/addDatabaseSamples","@web-atoms/xf-samples/dist/samples/database/web-sql/WebSqlSample","@web-atoms/font-awesome/dist/FontAwesomeRegular","@web-atoms/xf-samples/dist/samples/database/web-sql/WebSqlViewModel","@web-atoms/core/dist/view-model/Action","@web-atoms/storage/dist/database/sqlite/SqliteService","@web-atoms/storage/dist/query/Query","@web-atoms/xf-samples/dist/samples/database/web-sql/row-editor/RowEditor","@web-atoms/xf-samples/dist/samples/database/web-sql/row-editor/RowEditorViewModel","@web-atoms/xf-samples/dist/samples/form/FormSamples","@web-atoms/xf-samples/dist/samples/button/ButtonView","@web-atoms/xf-samples/dist/samples/button/ButtonViewModel","@web-atoms/xf-samples/dist/samples/button/image-button/ImageButtonView","@web-atoms/xf-samples/dist/samples/check-box/CheckBoxView","@web-atoms/xf-samples/dist/samples/check-box/CheckBoxSampleViewModel","@web-atoms/xf-samples/dist/samples/combo-box/ComboBoxSample","@web-atoms/xf-samples/dist/samples/combo-box/ComboBoxSampleViewModel","@web-atoms/xf-samples/dist/samples/date-picker/DatePickerView","@web-atoms/xf-samples/dist/samples/date-picker/DatePickerViewModel","@web-atoms/xf-samples/dist/samples/editor/EditorView","@web-atoms/xf-samples/dist/samples/editor/EditorViewModel","@web-atoms/xf-samples/dist/samples/entry/EntryView","@web-atoms/xf-samples/dist/samples/entry/EntryViewModel","@web-atoms/xf-samples/dist/samples/label/LabelView","@web-atoms/xf-samples/dist/samples/label/LabelViewModel","@web-atoms/xf-samples/dist/samples/search-bar/SearchBarView","@web-atoms/xf-samples/dist/samples/search-bar/SearchBarViewModel","@web-atoms/xf-samples/dist/service/http/MovieService","@web-atoms/core/dist/services/http/RestService","@web-atoms/core/dist/services/http/AjaxOptions","@web-atoms/core/dist/services/CacheService","@web-atoms/core/dist/services/http/JsonError","@web-atoms/xf-samples/dist/samples/slider/SliderView","@web-atoms/xf-samples/dist/samples/slider/SliderViewModel","@web-atoms/xf-samples/dist/samples/stepper/StepperView","@web-atoms/xf-samples/dist/samples/stepper/StepperViewModel","@web-atoms/xf-samples/dist/samples/form/simple/SimpleForm","@web-atoms/xf-samples/dist/samples/form/simple/SimpleFormViewModel","@web-atoms/xf-samples/dist/samples/image/ImageSample","@web-atoms/xf-samples/dist/samples/image/ImageView","@web-atoms/xf-samples/dist/samples/layout/multiple-content/LayoutSample","@web-atoms/xf-samples/dist/samples/layout/multiple-content/absolute-layout/AbsoluteLayoutView","@web-atoms/xf-samples/dist/samples/layout/multiple-content/flex-layout/FlexLayoutView","@web-atoms/xf-samples/dist/samples/layout/multiple-content/grid-layout/GridView","@web-atoms/xf-samples/dist/samples/layout/multiple-content/stack-layout/StackLayoutView","@web-atoms/xf-samples/dist/samples/layout/single-content/Sample","@web-atoms/xf-samples/dist/samples/layout/single-content/content-view/MainPage","@web-atoms/xf-samples/dist/samples/layout/single-content/content-view/ContentView","@web-atoms/xf-samples/dist/samples/layout/single-content/frame/FrameSample","@web-atoms/xf-samples/dist/samples/layout/single-content/scroll-view/ScrollViewSample","@web-atoms/xf-samples/dist/samples/list/ListSamples","@web-atoms/xf-samples/dist/samples/list/list-view/List","@web-atoms/xf-samples/dist/samples/list/list-view/ListViewModel","@web-atoms/xf-samples/dist/samples/list/template-selector/ListWithTemplates","@web-atoms/xf-samples/dist/samples/menu-item/MenuSample","@web-atoms/xf-samples/dist/samples/menu-item/MenuItemView","@web-atoms/xf-samples/dist/samples/menu-item/MenuItemViewModel","@web-atoms/xf-samples/dist/samples/popup/PopupSample","@web-atoms/xf-samples/dist/samples/popup/PopupCallingPage","@web-atoms/xf-samples/dist/samples/popup/PopupCallingPageViewModel","@web-atoms/xf-samples/dist/samples/popup/PopupView","@web-atoms/xf-samples/dist/samples/refresh-view/RefreshViewSample","@web-atoms/xf-samples/dist/samples/refresh-view/RefreshView","@web-atoms/xf-samples/dist/samples/refresh-view/RefreshViewModel","@web-atoms/xf-samples/dist/samples/switch/SwitchSamplePage","@web-atoms/xf-samples/dist/samples/switch/SwitchSample","@web-atoms/xf-samples/dist/samples/switch/SwitchViewModel","@web-atoms/xf-samples/dist/samples/tabbed-page/TabbedPageSample","@web-atoms/xf-samples/dist/samples/tabbed-page/TabbedPageView","@web-atoms/xf-controls/dist/pages/AtomXFTabbedPage","@web-atoms/xf-samples/dist/samples/table-view/TableViewSamplePage","@web-atoms/xf-samples/dist/samples/table-view/TableViewSample","@web-atoms/xf-samples/dist/samples/table-view/TableViewModel","@web-atoms/xf-samples/dist/samples/time-picker/TimePickerSamplePage","@web-atoms/xf-samples/dist/samples/time-picker/TimePickerSample","@web-atoms/xf-samples/dist/samples/time-picker/TimePickerViewModel","@web-atoms/xf-samples/dist/samples/toggle-button-bar/addToggleButtonBar","@web-atoms/xf-samples/dist/samples/toggle-button-bar/custom/CustomToggleButtonBar","@web-atoms/xf-controls/dist/toggle-button-bar/AtomXFToggleButtonBar","@web-atoms/xf-samples/dist/samples/toggle-button-bar/simple/ToggleButtonBarViewModel","@web-atoms/xf-samples/dist/samples/toggle-button-bar/simple/ToggleButtonBar","@web-atoms/xf-samples/dist/samples/toolbar-item/ToolbarItemSample","@web-atoms/xf-samples/dist/samples/toolbar-item/ToolbarItemView","@web-atoms/xf-samples/dist/samples/toolbar-item/ToolbarItemViewModel","@web-atoms/xf-samples/dist/samples/web-view/WebViewSample","@web-atoms/xf-samples/dist/samples/web-view/WebView","@web-atoms/xf-samples/dist/service/menu-service/MenuService","@web-atoms/xf-samples/dist/service/menu-service/MenuItem","@web-atoms/xf-samples/dist/app-host/home/Home","@web-atoms/xf-samples/dist/app-host/home/HomeViewModel","@web-atoms/core/dist/xf/XFApp","@web-atoms/core/dist/xf/services/XFBusyIndicatorService","@web-atoms/core/dist/xf/services/XFNavigationService"]);

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    if (typeof Map === "undefined") {
        class AtomMap {
            constructor() {
                this.map = [];
            }
            get size() {
                return this.map.length;
            }
            clear() {
                this.map.length = 0;
            }
            delete(key) {
                return this.map.remove((x) => x.key === key);
            }
            forEach(callbackfn, thisArg) {
                for (const iterator of this.map) {
                    callbackfn.call(thisArg, iterator.value, iterator.key, this);
                }
            }
            get(key) {
                const item = this.getItem(key, false);
                return item ? item.value : undefined;
            }
            has(key) {
                return this.map.find((x) => x.key === key) != null;
            }
            set(key, value) {
                const item = this.getItem(key, true);
                item.value = value;
                return this;
            }
            // public [Symbol.iterator](): IterableIterator<[K, V]> {
            //     throw new Error("Method not implemented.");
            // }
            // public keys(): IterableIterator<K> {
            //     throw new Error("Method not implemented.");
            // }
            // public values(): IterableIterator<V> {
            //     throw new Error("Method not implemented.");
            // }
            // public get [Symbol.toStringTag](): string {
            //     return "[Map]";
            // }
            getItem(key, create = false) {
                for (const iterator of this.map) {
                    if (iterator.key === key) {
                        return iterator;
                    }
                }
                if (create) {
                    const r = { key, value: undefined };
                    this.map.push(r);
                    return r;
                }
            }
        }
        // tslint:disable-next-line:no-string-literal
        window["Map"] = AtomMap;
    }
    // tslint:disable-next-line:only-arrow-functions
    Map.prototype.getOrCreate = function (key, factory) {
        let item = this.get(key);
        if (item === undefined) {
            item = factory(key);
            this.set(key, item);
        }
        return item;
    };
    exports.default = Map;
});
//# sourceMappingURL=AtomMap.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomMap");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "reflect-metadata", "./AtomMap"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.UMD = exports.DI = exports.ArrayHelper = exports.CancelToken = void 0;
    require("reflect-metadata");
    const AtomMap_1 = require("./AtomMap");
    /**
     *
     *
     * @export
     * @class CancelToken
     */
    class CancelToken {
        constructor(timeout = -1) {
            this.listeners = [];
            this.mCancelled = null;
            this.cancelTimeout = null;
            if (timeout > 0) {
                this.cancelTimeout = setTimeout(() => {
                    this.cancelTimeout = null;
                    this.cancel("timeout");
                }, timeout);
            }
        }
        get cancelled() {
            return this.mCancelled;
        }
        cancel(r = "cancelled") {
            this.mCancelled = r;
            const existing = this.listeners.slice(0);
            this.listeners.length = 0;
            for (const fx of existing) {
                fx(r);
            }
            this.dispose();
        }
        reset() {
            this.mCancelled = null;
            this.dispose();
        }
        dispose() {
            this.listeners.length = 0;
            if (this.cancelTimeout) {
                clearTimeout(this.cancelTimeout);
            }
        }
        registerForCancel(f) {
            if (this.mCancelled) {
                f(this.mCancelled);
                this.cancel();
                return;
            }
            this.listeners.push(f);
        }
    }
    exports.CancelToken = CancelToken;
    class ArrayHelper {
        static remove(a, filter) {
            for (let i = 0; i < a.length; i++) {
                const item = a[i];
                if (filter(item)) {
                    a.splice(i, 1);
                    return true;
                }
            }
            return false;
        }
    }
    exports.ArrayHelper = ArrayHelper;
    // tslint:disable-next-line
    Array.prototype["groupBy"] = function (keySelector) {
        const map = new AtomMap_1.default();
        const groups = [];
        for (const iterator of this) {
            const key = keySelector(iterator);
            let g = map.get(key);
            if (!g) {
                g = [];
                g.key = key;
                groups.push(g);
            }
            g.push(iterator);
        }
        map.clear();
        return groups;
    };
    const globalNS = (typeof window !== "undefined" ? window : global);
    exports.DI = (globalNS).UMD;
    exports.UMD = (globalNS).UMD;
});
//# sourceMappingURL=types.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/types");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./types"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomBinder = void 0;
    const types_1 = require("./types");
    class AtomBinder {
        static refreshValue(target, key) {
            const handlers = AtomBinder.get_WatchHandler(target, key);
            if (handlers === undefined || handlers == null) {
                return;
            }
            for (const item of handlers) {
                item(target, key);
            }
            if (target.onPropertyChanged) {
                target.onPropertyChanged(key);
            }
        }
        static add_WatchHandler(target, key, handler) {
            if (target == null) {
                return;
            }
            const handlers = AtomBinder.get_WatchHandler(target, key);
            handlers.push(handler);
            if (Array.isArray(target)) {
                return;
            }
            // get existing property definition if it ha any
            const pv = AtomBinder.getPropertyDescriptor(target, key);
            // return if it has a getter
            // in case of getter/setter, it is responsibility of setter to refresh
            // object
            if (pv && pv.get) {
                return;
            }
            const tw = target;
            if (!tw._$_bindable) {
                tw._$_bindable = {};
            }
            if (!tw._$_bindable[key]) {
                tw._$_bindable[key] = 1;
                const o = target[key];
                const nk = `_$_${key}`;
                target[nk] = o;
                const set = function (v) {
                    const ov = this[nk];
                    // tslint:disable-next-line:triple-equals
                    if (ov === undefined ? ov === v : ov == v) {
                        return;
                    }
                    this[nk] = v;
                    AtomBinder.refreshValue(this, key);
                };
                const get = function () {
                    return this[nk];
                };
                if (pv) {
                    delete target[key];
                    Object.defineProperty(target, key, {
                        get,
                        set,
                        configurable: true,
                        enumerable: true
                    });
                }
                else {
                    Object.defineProperty(target, key, {
                        get, set, enumerable: true, configurable: true
                    });
                }
            }
        }
        static getPropertyDescriptor(target, key) {
            const pv = Object.getOwnPropertyDescriptor(target, key);
            if (!pv) {
                const pt = Object.getPrototypeOf(target);
                if (pt) {
                    return AtomBinder.getPropertyDescriptor(pt, key);
                }
            }
            return pv;
        }
        static get_WatchHandler(target, key) {
            if (target == null) {
                return null;
            }
            let handlers = target._$_handlers;
            if (!handlers) {
                handlers = {};
                target._$_handlers = handlers;
            }
            let handlersForKey = handlers[key];
            if (handlersForKey === undefined || handlersForKey == null) {
                handlersForKey = [];
                handlers[key] = handlersForKey;
            }
            return handlersForKey;
        }
        static remove_WatchHandler(target, key, handler) {
            if (target == null) {
                return;
            }
            if (!target._$_handlers) {
                return;
            }
            const handlersForKey = target._$_handlers[key];
            if (handlersForKey === undefined || handlersForKey == null) {
                return;
            }
            // handlersForKey = handlersForKey.filter( (f) => f !== handler);
            types_1.ArrayHelper.remove(handlersForKey, (f) => f === handler);
            if (!handlersForKey.length) {
                target._$_handlers[key] = null;
                delete target._$_handlers[key];
            }
        }
        static invokeItemsEvent(target, mode, index, item) {
            const key = "_items";
            const handlers = AtomBinder.get_WatchHandler(target, key);
            if (!handlers) {
                return;
            }
            for (const obj of handlers) {
                obj(target, mode, index, item);
            }
            AtomBinder.refreshValue(target, "length");
        }
        static refreshItems(ary) {
            AtomBinder.invokeItemsEvent(ary, "refresh", -1, null);
        }
        static add_CollectionChanged(target, handler) {
            if (target == null) {
                throw new Error("Target Array to watch cannot be null");
            }
            if (handler == null) {
                throw new Error("Target handle to watch an Array cannot be null");
            }
            const handlers = AtomBinder.get_WatchHandler(target, "_items");
            handlers.push(handler);
            return { dispose: () => {
                    AtomBinder.remove_CollectionChanged(target, handler);
                }
            };
        }
        static remove_CollectionChanged(t, handler) {
            if (t == null) {
                return;
            }
            const target = t;
            if (!target._$_handlers) {
                return;
            }
            const key = "_items";
            const handlersForKey = target._$_handlers[key];
            if (handlersForKey === undefined || handlersForKey == null) {
                return;
            }
            types_1.ArrayHelper.remove(handlersForKey, (f) => f === handler);
            if (!handlersForKey.length) {
                target._$_handlers[key] = null;
                delete target._$_handlers[key];
            }
        }
        static watch(item, property, f) {
            AtomBinder.add_WatchHandler(item, property, f);
            return {
                dispose: () => {
                    AtomBinder.remove_WatchHandler(item, property, f);
                }
            };
        }
        static clear(a) {
            a.length = 0;
            this.invokeItemsEvent(a, "refresh", -1, null);
            AtomBinder.refreshValue(a, "length");
        }
        static addItem(a, item) {
            const index = a.length;
            a.push(item);
            this.invokeItemsEvent(a, "add", index, item);
            AtomBinder.refreshValue(a, "length");
        }
        static removeItem(a, item) {
            const i = a.findIndex((x) => x === item);
            if (i === -1) {
                return false;
            }
            a.splice(i, 1);
            AtomBinder.invokeItemsEvent(a, "remove", i, item);
            AtomBinder.refreshValue(a, "length");
            return true;
        }
    }
    exports.AtomBinder = AtomBinder;
});
//# sourceMappingURL=AtomBinder.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomBinder");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomBinder"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomList = void 0;
    const AtomBinder_1 = require("./AtomBinder");
    /**
     *
     *
     * @export
     * @class AtomList
     * @extends {Array<T>}
     * @template T
     */
    class AtomList extends Array {
        // private version: number = 1;
        constructor() {
            super();
            this.startValue = 0;
            this.totalValue = 0;
            this.sizeValue = 10;
            // tslint:disable-next-line
            this["__proto__"] = AtomList.prototype;
            this.next = () => {
                this.start = this.start + this.size;
            };
            this.prev = () => {
                if (this.start >= this.size) {
                    this.start = this.start - this.size;
                }
            };
        }
        get start() {
            return this.startValue;
        }
        set start(v) {
            if (v === this.startValue) {
                return;
            }
            this.startValue = v;
            AtomBinder_1.AtomBinder.refreshValue(this, "start");
        }
        get total() {
            return this.totalValue;
        }
        set total(v) {
            if (v === this.totalValue) {
                return;
            }
            this.totalValue = v;
            AtomBinder_1.AtomBinder.refreshValue(this, "total");
        }
        get size() {
            return this.sizeValue;
        }
        set size(v) {
            if (v === this.sizeValue) {
                return;
            }
            this.sizeValue = v;
            AtomBinder_1.AtomBinder.refreshValue(this, "size");
        }
        /**
         * Adds the item in the list and refresh bindings
         * @param {T} item
         * @returns {number}
         * @memberof AtomList
         */
        add(item) {
            const i = this.length;
            const n = this.push(item);
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "add", i, item);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
            // this.version++;
            return n;
        }
        /**
         * Add given items in the list and refresh bindings
         * @param {Array<T>} items
         * @memberof AtomList
         */
        addAll(items) {
            for (const item of items) {
                const i = this.length;
                this.push(item);
                AtomBinder_1.AtomBinder.invokeItemsEvent(this, "add", i, item);
                AtomBinder_1.AtomBinder.refreshValue(this, "length");
            }
            // tslint:disable-next-line:no-string-literal
            const t = items["total"];
            if (t) {
                this.total = t;
            }
            // this.version++;
        }
        /**
         * Replaces list with given items, use this
         * to avoid flickering in screen
         * @param {T[]} items
         * @memberof AtomList
         */
        replace(items, start, size) {
            this.length = items.length;
            for (let i = 0; i < items.length; i++) {
                this[i] = items[i];
            }
            this.refresh();
            // tslint:disable-next-line:no-string-literal
            const t = items["total"];
            if (t) {
                this.total = t;
            }
            if (start !== undefined) {
                this.start = start;
            }
            if (size !== undefined) {
                this.size = size;
            }
        }
        /**
         * Inserts given number in the list at position `i`
         * and refreshes the bindings.
         * @param {number} i
         * @param {T} item
         * @memberof AtomList
         */
        insert(i, item) {
            const n = this.splice(i, 0, item);
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "add", i, item);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
        }
        /**
         * Removes item at given index i and refresh the bindings
         * @param {number} i
         * @memberof AtomList
         */
        removeAt(i) {
            const item = this[i];
            this.splice(i, 1);
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "remove", i, item);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
        }
        /**
         * Removes given item or removes all items that match
         * given lambda as true and refresh the bindings
         * @param {(T | ((i:T) => boolean))} item
         * @returns {boolean} `true` if any item was removed
         * @memberof AtomList
         */
        remove(item) {
            if (item instanceof Function) {
                let index = 0;
                let removed = false;
                for (const it of this) {
                    if (item(it)) {
                        this.removeAt(index);
                        removed = true;
                        continue;
                    }
                    index++;
                }
                return removed;
            }
            const n = this.indexOf(item);
            if (n !== -1) {
                this.removeAt(n);
                return true;
            }
            return false;
        }
        /**
         * Removes all items from the list and refreshes the bindings
         * @memberof AtomList
         */
        clear() {
            this.length = 0;
            this.refresh();
        }
        refresh() {
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "refresh", -1, null);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
            // this.version++;
        }
        watch(f, wrap) {
            if (wrap) {
                const fx = f;
                f = (function () {
                    const p = [];
                    // tslint:disable-next-line:prefer-for-of
                    for (let i = 0; i < arguments.length; i++) {
                        const iterator = arguments[i];
                        p.push(iterator);
                    }
                    return fx.call(this, p);
                });
            }
            return AtomBinder_1.AtomBinder.add_CollectionChanged(this, f);
        }
    }
    exports.AtomList = AtomList;
    // tslint:disable
    Array.prototype["add"] = AtomList.prototype.add;
    Array.prototype["addAll"] = AtomList.prototype.addAll;
    Array.prototype["clear"] = AtomList.prototype.clear;
    Array.prototype["refresh"] = AtomList.prototype.refresh;
    Array.prototype["remove"] = AtomList.prototype.remove;
    Array.prototype["removeAt"] = AtomList.prototype.removeAt;
    Array.prototype["watch"] = AtomList.prototype.watch;
    Array.prototype["replace"] = AtomList.prototype.replace;
    Array.prototype["insert"] = AtomList.prototype.insert;
});
//# sourceMappingURL=AtomList.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomList");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.Atom = void 0;
    class Atom {
        // tslint:disable-next-line: ban-types
        static superProperty(tc, target, name) {
            let c = tc;
            do {
                c = Object.getPrototypeOf(c);
                if (!c) {
                    throw new Error("No property descriptor found for " + name);
                }
                const pd = Object.getOwnPropertyDescriptor(c.prototype, name);
                if (!pd) {
                    continue;
                }
                return pd.get.apply(target);
            } while (true);
        }
        //      public static set(arg0: any, arg1: any, arg2: any): any {
        //     throw new Error("Method not implemented.");
        // }
        static get(target, path) {
            const segments = path.split(".");
            for (const iterator of segments) {
                if (target === undefined || target === null) {
                    return target;
                }
                target = target[iterator];
            }
            return target;
        }
        /**
         * Await till given milliseconds have passed
         * @param n
         * @param ct
         */
        static delay(n, ct) {
            return new Promise((resolve, reject) => {
                const h = {};
                h.id = setTimeout(() => {
                    // if (ct && ct.cancelled) {
                    //     reject(new Error("cancelled"));
                    //     return;
                    // }
                    resolve();
                }, n);
                if (ct) {
                    ct.registerForCancel(() => {
                        clearTimeout(h.id);
                        reject(new Error("cancelled"));
                    });
                }
            });
        }
        // // tslint:disable-next-line:member-access
        // static query(arg0: any): any {
        //     throw new Error("Method not implemented.");
        // }
        static encodeParameters(p) {
            if (!p) {
                return "";
            }
            let s = "";
            for (const key in p) {
                if (p.hasOwnProperty(key)) {
                    const element = p[key];
                    let v = element;
                    if (v === undefined || v === null) {
                        continue;
                    }
                    if (v instanceof Date) {
                        v = v.toISOString();
                    }
                    else if (typeof element === "object") {
                        v = JSON.stringify(element);
                    }
                    if (s) {
                        s += "&";
                    }
                    s += `${key}=${encodeURIComponent(v)}`;
                }
            }
            return s;
        }
        static url(url, query, hash) {
            if (!url) {
                return url;
            }
            let p = this.encodeParameters(query);
            if (p) {
                if (url.indexOf("?") === -1) {
                    url += "?";
                }
                else {
                    url += "&";
                }
                url += p;
            }
            p = this.encodeParameters(hash);
            if (p) {
                if (url.indexOf("#") === -1) {
                    url += "#";
                }
                else {
                    url += "&";
                }
                url += p;
            }
            return url;
        }
        /**
         * Schedules given call in next available callLater slot and also returns
         * promise that can be awaited, calling `Atom.postAsync` inside `Atom.postAsync`
         * will create deadlock
         * @static
         * @param {()=>Promise<any>} f
         * @returns {Promise<any>}
         * @memberof Atom
         */
        static postAsync(app, f) {
            return new Promise((resolve, reject) => {
                app.callLater(() => __awaiter(this, void 0, void 0, function* () {
                    try {
                        resolve(yield f());
                    }
                    catch (error) {
                        reject(error);
                    }
                }));
            });
        }
    }
    exports.Atom = Atom;
    Atom.designMode = false;
});
//# sourceMappingURL=Atom.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/Atom");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.parsePathLists = exports.parsePath = void 0;
    const viewModelParseWatchCache = {};
    function trimRegEx(t, r) {
        const m = r.exec(t);
        if (m && m.length) {
            return [true, t.substring(m[0].length).trim(), m[0]];
        }
        return [false, t, ""];
    }
    function extractTill(text, search, returnOriginal = false) {
        const index = text.indexOf(search);
        if (index <= 0) {
            return returnOriginal ? text : "";
        }
        return text.substring(0, index);
    }
    function parsePath(f, parseThis) {
        let str = f.toString().trim();
        str = str.split("\n").filter((s) => !/^\/\//.test(s.trim())).join("\n");
        const key = (parseThis === undefined ? "un:" : (parseThis ? "_this:" : "_noThis:")) + str;
        const px1 = viewModelParseWatchCache[key];
        if (px1) {
            return px1;
        }
        if (str.endsWith("}")) {
            str = str.substr(0, str.length - 1);
        }
        const functionRegEx = /^\w+\s*\(/;
        const lambdaRegEx = /^\(?(\w+)?(\s?\,\s?\w+)*\)?\s?\=\>/;
        let arg = "";
        const original = str;
        // tslint:disable-next-line: prefer-const
        let [success, remaining] = trimRegEx(str, functionRegEx);
        if (success) {
            str = remaining;
            remaining = extractTill(remaining, ")");
            str = str.substring(remaining.length);
            arg = extractTill(remaining, ",", true);
        }
        else {
            // this must be a lambda..
            [success, str, remaining] = trimRegEx(str, lambdaRegEx);
            if (success) {
                remaining = remaining.trim();
                remaining = remaining.substring(0, remaining.length - 2);
                remaining = extractTill(remaining, ")", true);
                arg = extractTill(remaining, ",", true);
                if (arg.startsWith("(")) {
                    arg = arg.substring(1);
                }
            }
            else {
                if (parseThis !== undefined && parseThis === false) {
                    return [];
                }
                else {
                    parseThis = true;
                }
            }
        }
        // if (str.startsWith("function (")) {
        //     str = str.substr("function (".length);
        // } else if (str.startsWith("function(")) {
        //     str = str.substr("function(".length);
        // } else {
        //     const sb = str.indexOf("(");
        //     if (sb !== -1) {
        //         str = str.substr(sb + 1);
        //     } else {
        //         if (parseThis !== undefined && parseThis === false) {
        //             return [];
        //         } else {
        //             parseThis = true;
        //         }
        //     }
        // }
        str = str.trim();
        // const commaIndex = str.indexOf(",");
        // if (commaIndex !== -1 && commaIndex < index) {
        //     index = commaIndex;
        // }
        // const lambdaIndex = str.indexOf("=>");
        // if (lambdaIndex !== -1) {
        //     index = lambdaIndex;
        // }
        const isThis = parseThis === undefined
            ? ((arg ? false : true) || parseThis)
            : parseThis;
        const p = (isThis ? "(\\_this|this)" : (arg || "")).trim();
        /**
         * This is the case when there is no parameter to check and there `parseThis` is false
         */
        if (p.length === 0) {
            const empty = [];
            viewModelParseWatchCache[key] = empty;
            return empty;
        }
        // str = str.substr(index + 1);
        const regExp = `(?:(\\b${p})(?:(\\.[a-zA-Z_][a-zA-Z_0-9]*)+)\\s?(?:(\\(|\\=\\=\\=|\\=\\=|\\=)?))`;
        const re = new RegExp(regExp, "gi");
        let path = [];
        const ms = str.replace(re, (m) => {
            // console.log(`m: ${m}`);
            let px = m;
            if (px.startsWith("this.")) {
                if (parseThis !== true) {
                    px = px.substr(5);
                }
            }
            else if (px.startsWith("_this.")) {
                if (parseThis !== true) {
                    px = px.substr(6);
                }
                else {
                    // need to convert _this to this
                    px = px.substr(1);
                }
            }
            else {
                px = px.substr(p.length + 1);
            }
            px = px.split(".").filter((s) => !s.endsWith("(")).join(".");
            // console.log(px);
            if (!path.find((y) => y === px)) {
                path.push(px);
            }
            // path = path.filter( (f1) => f1.endsWith("==") || !(f1.endsWith("(") || f1.endsWith("=") ));
            // path = path.map(
            //     (px2) => (px2.endsWith("===") ? px2.substr(0, px2.length - 3) :
            //         ( px2.endsWith("==") ? px2.substr(0, px2.length - 2) : px2 )).trim() );
            const filtered = [];
            for (const iterator of path) {
                if (iterator.endsWith("==") || !(iterator.endsWith("(") || iterator.endsWith("="))) {
                    filtered.push((iterator.endsWith("===") ? iterator.substr(0, iterator.length - 3) :
                        (iterator.endsWith("==") ? iterator.substr(0, iterator.length - 2) : iterator)).trim());
                }
            }
            path = filtered.filter((px11) => {
                const search = px11 + ".";
                for (const iterator of filtered) {
                    if (px11 !== iterator && iterator.indexOf(search) !== -1) {
                        return false;
                    }
                }
                return true;
            });
            return m;
        });
        path = path.sort((a, b) => b.localeCompare(a));
        const duplicates = path;
        path = [];
        for (const iterator of duplicates) {
            if (path.find((px2) => px2 === iterator)) {
                continue;
            }
            path.push(iterator);
        }
        const rp = [];
        for (const rpItem of path) {
            if (rp.find((x) => x.startsWith(rpItem))) {
                continue;
            }
            rp.push(rpItem);
        }
        // tslint:disable-next-line: no-console
        // console.log(`Watching: ${path.join(", ")}`);
        const pl = path.filter((p1) => p1).map((p1) => p1.split("."));
        viewModelParseWatchCache[key] = pl;
        return pl;
    }
    exports.parsePath = parsePath;
    const viewModelParseWatchCache2 = {};
    function parsePathLists(f) {
        const str = f.toString().trim();
        const key = str;
        const px1 = viewModelParseWatchCache2[key];
        if (px1) {
            return px1;
        }
        // str = str.split("\n").filter((s) => !/^\/\//.test(s.trim())).join("\n");
        // if (str.endsWith("}")) {
        //     str = str.substr(0, str.length - 1);
        // }
        // if (str.startsWith("function (")) {
        //     str = str.substr("function (".length);
        // }
        // if (str.startsWith("function(")) {
        //     str = str.substr("function(".length);
        // }
        // str = str.trim();
        const pl = {
            pathList: parsePath(str, false),
            thisPath: parsePath(str, true),
            combined: []
        };
        if (pl.thisPath.length && pl.pathList.length) {
            // we need to combine this
            // pl.combinedPathList =
            pl.combined = pl.thisPath
                .map((x) => {
                x[0] = "t";
                x.splice(0, 0, "this");
                return x;
            })
                .concat(pl.pathList.map((x) => {
                x.splice(0, 0, "this", "x");
                return x;
            }));
            pl.thisPath = [];
            pl.pathList = [];
        }
        viewModelParseWatchCache2[key] = pl;
        return pl;
    }
    exports.parsePathLists = parsePathLists;
});
//# sourceMappingURL=ExpressionParser.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/ExpressionParser");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./ExpressionParser"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const ExpressionParser_1 = require("./ExpressionParser");
    const isEvent = /^event/i;
    function oneTime(name, b, control, e) {
        control.runAfterInit(() => {
            control.setLocalValue(e, name, b.sourcePath(control, e));
        });
    }
    function event(name, b, control, e) {
        control.runAfterInit(() => {
            if (isEvent.test(name)) {
                name = name.substr(5);
                name = (name[0].toLowerCase() + name.substr(1));
            }
            control.bindEvent(e, name, (e1) => {
                return b.sourcePath(control, e1);
            });
        });
    }
    function oneWay(name, b, control, e, creator) {
        if (b.pathList) {
            control.bind(e, name, b.pathList, false, () => {
                // tslint:disable-next-line: ban-types
                return b.sourcePath.call(creator, control, e);
            });
            return;
        }
        if (b.combined) {
            const a = {
                // it is `this`
                t: creator,
                // it is first parameter
                x: control
            };
            control.bind(e, name, b.combined, false, () => {
                // tslint:disable-next-line: ban-types
                return b.sourcePath.call(creator, control, e);
            }, a);
            return;
        }
        if (b.thisPathList) {
            control.bind(e, name, b.thisPathList, false, () => {
                // tslint:disable-next-line: ban-types
                return b.sourcePath.call(creator, control, e);
            }, creator);
            return;
        }
    }
    function twoWays(name, b, control, e, creator) {
        control.bind(e, name, b.thisPathList || b.pathList, b.eventList || true, null, b.thisPathList ? creator : undefined);
    }
    function presenter(name, b, control, e) {
        const n = b.name || name;
        let c = control.element;
        while (c) {
            if (c.atomControl && c.atomControl[n] !== undefined) {
                break;
            }
            c = c._logicalParent || c.parentElement;
        }
        ((c && c.atomControl) || control)[n] = e;
    }
    class Bind {
        constructor(setupFunction, sourcePath, name, eventList) {
            this.setupFunction = setupFunction;
            this.name = name;
            this.eventList = eventList;
            this.sourcePath = sourcePath;
            if (!this.sourcePath) {
                return;
            }
            if (Array.isArray(this.sourcePath)) {
                this.pathList = this.sourcePath;
                // this.setupFunction = null;
            }
            else {
                const lists = ExpressionParser_1.parsePathLists(this.sourcePath);
                if (lists.combined.length) {
                    this.combined = lists.combined;
                }
                if (lists.pathList.length) {
                    this.pathList = lists.pathList;
                }
                if (lists.thisPath.length) {
                    this.thisPathList = lists.thisPath;
                }
                if (setupFunction === oneWay) {
                    if (!(this.combined || this.pathList || this.thisPathList)) {
                        throw new Error(`Failed to setup binding for ${this.sourcePath}, parsing failed`);
                    }
                }
            }
        }
        static forControl() {
            return Bind;
        }
        static forData() {
            return Bind;
        }
        static forViewModel() {
            return Bind;
        }
        static forLocalViewModel() {
            return Bind;
        }
        static presenter(name) {
            return new Bind(presenter, null, name);
        }
        // tslint:disable-next-line: ban-types
        static event(sourcePath) {
            return new Bind(event, sourcePath);
        }
        static oneTime(sourcePath) {
            return new Bind(oneTime, sourcePath);
        }
        static oneWay(sourcePath) {
            return new Bind(oneWay, sourcePath);
        }
        static twoWays(sourcePath, events) {
            const b = new Bind(twoWays, sourcePath, null, events);
            if (!(b.thisPathList || b.pathList)) {
                throw new Error(`Failed to setup twoWay binding on ${sourcePath}`);
            }
            return b;
        }
        /**
         * Use this for HTML only, this will fire two way binding
         * as soon as the input/textarea box is updated
         * @param sourcePath binding lambda expression
         */
        static twoWaysImmediate(sourcePath) {
            const b = new Bind(twoWays, sourcePath, null, ["change", "input", "paste"]);
            if (!(b.thisPathList || b.pathList)) {
                throw new Error(`Failed to setup twoWay binding on ${sourcePath}`);
            }
            return b;
        }
    }
    exports.default = Bind;
});
//# sourceMappingURL=Bind.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/Bind");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.RootObject = void 0;
    class RootObject {
        get vsProps() {
            return undefined;
        }
        addEventListener(name, handler) {
            return bridge.addEventHandler(this, name, handler);
        }
        appendChild(e) {
            bridge.appendChild(this, e);
        }
        dispatchEvent(evt) {
            bridge.dispatchEvent(evt);
        }
    }
    exports.RootObject = RootObject;
    class XNode {
        constructor(
        // tslint:disable-next-line: ban-types
        name, attributes, children, isProperty, isTemplate) {
            this.name = name;
            this.attributes = attributes;
            this.children = children;
            this.isProperty = isProperty;
            this.isTemplate = isTemplate;
        }
        static attach(n, tag) {
            return {
                factory: (attributes, ...nodes) => new XNode(n, attributes
                    ? Object.assign(Object.assign({}, attributes), { for: tag }) : { for: tag }, nodes)
            };
        }
        static prepare(n, isProperty, isTemplate) {
            function px(v) {
                return ({
                    [n]: v
                });
            }
            px.factory = (a, ...nodes) => {
                return new XNode(n, a, nodes, isProperty, isTemplate);
            };
            px.toString = () => n;
            return px;
            // return {
            //     factory(a: any, ... nodes: any[]) {
            //         return new XNode(n, a, nodes, isProperty , isTemplate);
            //     },
            //     toString() {
            //         return n;
            //     }
            // } as any;
        }
        // public static property(): NodeFactory {
        //     return {
        //         factory: true
        //     } as any;
        // }
        static getClass(fullTypeName, assemblyName) {
            const n = fullTypeName + ";" + assemblyName;
            const cx = XNode.classes[n] || (XNode.classes[n] =
                bridge.getClass(fullTypeName, assemblyName, RootObject, (name, isProperty, isTemplate) => (a, ...nodes) => new XNode(name, a, nodes, isProperty, isTemplate)));
            return cx;
        }
        /**
         * Declares Root Namespace and Assembly. You can use return function to
         * to declare the type
         * @param ns Root Namespace
         */
        static namespace(ns, assemblyName) {
            return (type, isTemplate) => {
                return (c) => {
                    // static properties !!
                    for (const key in c) {
                        if (c.hasOwnProperty(key)) {
                            const element = c[key];
                            if (element) {
                                const n = ns + "." + type + ":" + key + ";" + assemblyName;
                                const af = (a) => {
                                    const r = {
                                        [n]: a
                                    };
                                    Object.defineProperty(r, "toString", {
                                        value: () => n,
                                        enumerable: false,
                                        configurable: false
                                    });
                                    return r;
                                };
                                af.factory = (a, ...nodes) => new XNode(n, a, nodes, true, element.isTemplate);
                                af.toString = () => n;
                                c[key] = af;
                            }
                        }
                    }
                    const tn = ns + "." + type + ";" + assemblyName;
                    c.factory = (a, ...nodes) => {
                        return new XNode(tn, a, nodes, false, isTemplate);
                    };
                    c.toString = () => tn;
                };
            };
        }
        static create(
        // tslint:disable-next-line: ban-types
        name, attributes, ...children) {
            if (name.factory) {
                return (name.factory)(attributes, ...children);
            }
            if (name.isControl) {
                return new XNode(name, attributes, children);
            }
            switch (typeof name) {
                case "object":
                    name = name.toString();
                    break;
                case "function":
                    return name(attributes, ...children);
            }
            return new XNode(name, attributes, children);
        }
        toString() {
            if (typeof this.name === "string") {
                return `name is of type string and value is ${this.name}`;
            }
            return `name is of type ${typeof this.name}`;
        }
    }
    exports.default = XNode;
    XNode.classes = {};
    // public static template(): NodeFactory {
    //     return {
    //         factory: true,
    //         isTemplate: true,
    //     } as any;
    // }
    XNode.attached = (name) => (n) => ({ [name]: n });
    XNode.factory = (name, isProperty, isTemplate) => (a, ...nodes) => {
        return new XNode(name, a, nodes, isProperty, isTemplate);
    };
    if (typeof bridge !== "undefined") {
        bridge.XNode = XNode;
    }
});
//# sourceMappingURL=XNode.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/XNode");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    var _this = this;
    Object.defineProperty(exports, "__esModule", { value: true });
    var FontAwesomeSolid = {
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ad.svg.png) Ad
         * Image Copyright FontAwesome.com
        */
        ad: "\uf641",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/address-book.svg.png) Address Book
         * Image Copyright FontAwesome.com
        */
        addressBook: "\uf2b9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/address-card.svg.png) Address Card
         * Image Copyright FontAwesome.com
        */
        addressCard: "\uf2bb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/adjust.svg.png) adjust
         * Image Copyright FontAwesome.com
        */
        adjust: "\uf042",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/air-freshener.svg.png) Air Freshener
         * Image Copyright FontAwesome.com
        */
        airFreshener: "\uf5d0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/align-center.svg.png) align-center
         * Image Copyright FontAwesome.com
        */
        alignCenter: "\uf037",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/align-justify.svg.png) align-justify
         * Image Copyright FontAwesome.com
        */
        alignJustify: "\uf039",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/align-left.svg.png) align-left
         * Image Copyright FontAwesome.com
        */
        alignLeft: "\uf036",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/align-right.svg.png) align-right
         * Image Copyright FontAwesome.com
        */
        alignRight: "\uf038",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/allergies.svg.png) Allergies
         * Image Copyright FontAwesome.com
        */
        allergies: "\uf461",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ambulance.svg.png) ambulance
         * Image Copyright FontAwesome.com
        */
        ambulance: "\uf0f9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/american-sign-language-interpreting.svg.png) American Sign Language Interpreting
         * Image Copyright FontAwesome.com
        */
        americanSignLanguageInterpreting: "\uf2a3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/anchor.svg.png) Anchor
         * Image Copyright FontAwesome.com
        */
        anchor: "\uf13d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-double-down.svg.png) Angle Double Down
         * Image Copyright FontAwesome.com
        */
        angleDoubleDown: "\uf103",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-double-left.svg.png) Angle Double Left
         * Image Copyright FontAwesome.com
        */
        angleDoubleLeft: "\uf100",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-double-right.svg.png) Angle Double Right
         * Image Copyright FontAwesome.com
        */
        angleDoubleRight: "\uf101",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-double-up.svg.png) Angle Double Up
         * Image Copyright FontAwesome.com
        */
        angleDoubleUp: "\uf102",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-down.svg.png) angle-down
         * Image Copyright FontAwesome.com
        */
        angleDown: "\uf107",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-left.svg.png) angle-left
         * Image Copyright FontAwesome.com
        */
        angleLeft: "\uf104",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-right.svg.png) angle-right
         * Image Copyright FontAwesome.com
        */
        angleRight: "\uf105",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angle-up.svg.png) angle-up
         * Image Copyright FontAwesome.com
        */
        angleUp: "\uf106",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/angry.svg.png) Angry Face
         * Image Copyright FontAwesome.com
        */
        angry: "\uf556",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ankh.svg.png) Ankh
         * Image Copyright FontAwesome.com
        */
        ankh: "\uf644",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/apple-alt.svg.png) Fruit Apple
         * Image Copyright FontAwesome.com
        */
        appleAlt: "\uf5d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/archive.svg.png) Archive
         * Image Copyright FontAwesome.com
        */
        archive: "\uf187",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/archway.svg.png) Archway
         * Image Copyright FontAwesome.com
        */
        archway: "\uf557",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-alt-circle-down.svg.png) Alternate Arrow Circle Down
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleDown: "\uf358",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-alt-circle-left.svg.png) Alternate Arrow Circle Left
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleLeft: "\uf359",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-alt-circle-right.svg.png) Alternate Arrow Circle Right
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleRight: "\uf35a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-alt-circle-up.svg.png) Alternate Arrow Circle Up
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleUp: "\uf35b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-circle-down.svg.png) Arrow Circle Down
         * Image Copyright FontAwesome.com
        */
        arrowCircleDown: "\uf0ab",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-circle-left.svg.png) Arrow Circle Left
         * Image Copyright FontAwesome.com
        */
        arrowCircleLeft: "\uf0a8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-circle-right.svg.png) Arrow Circle Right
         * Image Copyright FontAwesome.com
        */
        arrowCircleRight: "\uf0a9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-circle-up.svg.png) Arrow Circle Up
         * Image Copyright FontAwesome.com
        */
        arrowCircleUp: "\uf0aa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-down.svg.png) arrow-down
         * Image Copyright FontAwesome.com
        */
        arrowDown: "\uf063",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-left.svg.png) arrow-left
         * Image Copyright FontAwesome.com
        */
        arrowLeft: "\uf060",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-right.svg.png) arrow-right
         * Image Copyright FontAwesome.com
        */
        arrowRight: "\uf061",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrow-up.svg.png) arrow-up
         * Image Copyright FontAwesome.com
        */
        arrowUp: "\uf062",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrows-alt.svg.png) Alternate Arrows
         * Image Copyright FontAwesome.com
        */
        arrowsAlt: "\uf0b2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrows-alt-h.svg.png) Alternate Arrows Horizontal
         * Image Copyright FontAwesome.com
        */
        arrowsAltH: "\uf337",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/arrows-alt-v.svg.png) Alternate Arrows Vertical
         * Image Copyright FontAwesome.com
        */
        arrowsAltV: "\uf338",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/assistive-listening-systems.svg.png) Assistive Listening Systems
         * Image Copyright FontAwesome.com
        */
        assistiveListeningSystems: "\uf2a2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/asterisk.svg.png) asterisk
         * Image Copyright FontAwesome.com
        */
        asterisk: "\uf069",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/at.svg.png) At
         * Image Copyright FontAwesome.com
        */
        at: "\uf1fa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/atlas.svg.png) Atlas
         * Image Copyright FontAwesome.com
        */
        atlas: "\uf558",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/atom.svg.png) Atom
         * Image Copyright FontAwesome.com
        */
        atom: "\uf5d2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/audio-description.svg.png) Audio Description
         * Image Copyright FontAwesome.com
        */
        audioDescription: "\uf29e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/award.svg.png) Award
         * Image Copyright FontAwesome.com
        */
        award: "\uf559",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/baby.svg.png) Baby
         * Image Copyright FontAwesome.com
        */
        baby: "\uf77c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/baby-carriage.svg.png) Baby Carriage
         * Image Copyright FontAwesome.com
        */
        babyCarriage: "\uf77d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/backspace.svg.png) Backspace
         * Image Copyright FontAwesome.com
        */
        backspace: "\uf55a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/backward.svg.png) backward
         * Image Copyright FontAwesome.com
        */
        backward: "\uf04a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bacon.svg.png) Bacon
         * Image Copyright FontAwesome.com
        */
        bacon: "\uf7e5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bahai.svg.png) Bahá'í
         * Image Copyright FontAwesome.com
        */
        bahai: "\uf666",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/balance-scale.svg.png) Balance Scale
         * Image Copyright FontAwesome.com
        */
        balanceScale: "\uf24e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/balance-scale-left.svg.png) Balance Scale (Left-Weighted)
         * Image Copyright FontAwesome.com
        */
        balanceScaleLeft: "\uf515",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/balance-scale-right.svg.png) Balance Scale (Right-Weighted)
         * Image Copyright FontAwesome.com
        */
        balanceScaleRight: "\uf516",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ban.svg.png) ban
         * Image Copyright FontAwesome.com
        */
        ban: "\uf05e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/band-aid.svg.png) Band-Aid
         * Image Copyright FontAwesome.com
        */
        bandAid: "\uf462",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/barcode.svg.png) barcode
         * Image Copyright FontAwesome.com
        */
        barcode: "\uf02a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bars.svg.png) Bars
         * Image Copyright FontAwesome.com
        */
        bars: "\uf0c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/baseball-ball.svg.png) Baseball Ball
         * Image Copyright FontAwesome.com
        */
        baseballBall: "\uf433",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/basketball-ball.svg.png) Basketball Ball
         * Image Copyright FontAwesome.com
        */
        basketballBall: "\uf434",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bath.svg.png) Bath
         * Image Copyright FontAwesome.com
        */
        bath: "\uf2cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/battery-empty.svg.png) Battery Empty
         * Image Copyright FontAwesome.com
        */
        batteryEmpty: "\uf244",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/battery-full.svg.png) Battery Full
         * Image Copyright FontAwesome.com
        */
        batteryFull: "\uf240",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/battery-half.svg.png) Battery 1/2 Full
         * Image Copyright FontAwesome.com
        */
        batteryHalf: "\uf242",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/battery-quarter.svg.png) Battery 1/4 Full
         * Image Copyright FontAwesome.com
        */
        batteryQuarter: "\uf243",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/battery-three-quarters.svg.png) Battery 3/4 Full
         * Image Copyright FontAwesome.com
        */
        batteryThreeQuarters: "\uf241",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bed.svg.png) Bed
         * Image Copyright FontAwesome.com
        */
        bed: "\uf236",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/beer.svg.png) beer
         * Image Copyright FontAwesome.com
        */
        beer: "\uf0fc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bell.svg.png) bell
         * Image Copyright FontAwesome.com
        */
        bell: "\uf0f3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bell-slash.svg.png) Bell Slash
         * Image Copyright FontAwesome.com
        */
        bellSlash: "\uf1f6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bezier-curve.svg.png) Bezier Curve
         * Image Copyright FontAwesome.com
        */
        bezierCurve: "\uf55b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bible.svg.png) Bible
         * Image Copyright FontAwesome.com
        */
        bible: "\uf647",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bicycle.svg.png) Bicycle
         * Image Copyright FontAwesome.com
        */
        bicycle: "\uf206",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/biking.svg.png) Biking
         * Image Copyright FontAwesome.com
        */
        biking: "\uf84a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/binoculars.svg.png) Binoculars
         * Image Copyright FontAwesome.com
        */
        binoculars: "\uf1e5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/biohazard.svg.png) Biohazard
         * Image Copyright FontAwesome.com
        */
        biohazard: "\uf780",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/birthday-cake.svg.png) Birthday Cake
         * Image Copyright FontAwesome.com
        */
        birthdayCake: "\uf1fd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/blender.svg.png) Blender
         * Image Copyright FontAwesome.com
        */
        blender: "\uf517",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/blender-phone.svg.png) Blender Phone
         * Image Copyright FontAwesome.com
        */
        blenderPhone: "\uf6b6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/blind.svg.png) Blind
         * Image Copyright FontAwesome.com
        */
        blind: "\uf29d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/blog.svg.png) Blog
         * Image Copyright FontAwesome.com
        */
        blog: "\uf781",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bold.svg.png) bold
         * Image Copyright FontAwesome.com
        */
        bold: "\uf032",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bolt.svg.png) Lightning Bolt
         * Image Copyright FontAwesome.com
        */
        bolt: "\uf0e7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bomb.svg.png) Bomb
         * Image Copyright FontAwesome.com
        */
        bomb: "\uf1e2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bone.svg.png) Bone
         * Image Copyright FontAwesome.com
        */
        bone: "\uf5d7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bong.svg.png) Bong
         * Image Copyright FontAwesome.com
        */
        bong: "\uf55c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/book.svg.png) book
         * Image Copyright FontAwesome.com
        */
        book: "\uf02d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/book-dead.svg.png) Book of the Dead
         * Image Copyright FontAwesome.com
        */
        bookDead: "\uf6b7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/book-medical.svg.png) Medical Book
         * Image Copyright FontAwesome.com
        */
        bookMedical: "\uf7e6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/book-open.svg.png) Book Open
         * Image Copyright FontAwesome.com
        */
        bookOpen: "\uf518",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/book-reader.svg.png) Book Reader
         * Image Copyright FontAwesome.com
        */
        bookReader: "\uf5da",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bookmark.svg.png) bookmark
         * Image Copyright FontAwesome.com
        */
        bookmark: "\uf02e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/border-all.svg.png) Border All
         * Image Copyright FontAwesome.com
        */
        borderAll: "\uf84c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/border-none.svg.png) Border None
         * Image Copyright FontAwesome.com
        */
        borderNone: "\uf850",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/border-style.svg.png) Border Style
         * Image Copyright FontAwesome.com
        */
        borderStyle: "\uf853",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bowling-ball.svg.png) Bowling Ball
         * Image Copyright FontAwesome.com
        */
        bowlingBall: "\uf436",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/box.svg.png) Box
         * Image Copyright FontAwesome.com
        */
        box: "\uf466",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/box-open.svg.png) Box Open
         * Image Copyright FontAwesome.com
        */
        boxOpen: "\uf49e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/boxes.svg.png) Boxes
         * Image Copyright FontAwesome.com
        */
        boxes: "\uf468",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/braille.svg.png) Braille
         * Image Copyright FontAwesome.com
        */
        braille: "\uf2a1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/brain.svg.png) Brain
         * Image Copyright FontAwesome.com
        */
        brain: "\uf5dc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bread-slice.svg.png) Bread Slice
         * Image Copyright FontAwesome.com
        */
        breadSlice: "\uf7ec",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/briefcase.svg.png) Briefcase
         * Image Copyright FontAwesome.com
        */
        briefcase: "\uf0b1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/briefcase-medical.svg.png) Medical Briefcase
         * Image Copyright FontAwesome.com
        */
        briefcaseMedical: "\uf469",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/broadcast-tower.svg.png) Broadcast Tower
         * Image Copyright FontAwesome.com
        */
        broadcastTower: "\uf519",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/broom.svg.png) Broom
         * Image Copyright FontAwesome.com
        */
        broom: "\uf51a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/brush.svg.png) Brush
         * Image Copyright FontAwesome.com
        */
        brush: "\uf55d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bug.svg.png) Bug
         * Image Copyright FontAwesome.com
        */
        bug: "\uf188",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/building.svg.png) Building
         * Image Copyright FontAwesome.com
        */
        building: "\uf1ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bullhorn.svg.png) bullhorn
         * Image Copyright FontAwesome.com
        */
        bullhorn: "\uf0a1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bullseye.svg.png) Bullseye
         * Image Copyright FontAwesome.com
        */
        bullseye: "\uf140",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/burn.svg.png) Burn
         * Image Copyright FontAwesome.com
        */
        burn: "\uf46a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bus.svg.png) Bus
         * Image Copyright FontAwesome.com
        */
        bus: "\uf207",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/bus-alt.svg.png) Bus Alt
         * Image Copyright FontAwesome.com
        */
        busAlt: "\uf55e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/business-time.svg.png) Business Time
         * Image Copyright FontAwesome.com
        */
        businessTime: "\uf64a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calculator.svg.png) Calculator
         * Image Copyright FontAwesome.com
        */
        calculator: "\uf1ec",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar.svg.png) Calendar
         * Image Copyright FontAwesome.com
        */
        calendar: "\uf133",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-alt.svg.png) Alternate Calendar
         * Image Copyright FontAwesome.com
        */
        calendarAlt: "\uf073",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-check.svg.png) Calendar Check
         * Image Copyright FontAwesome.com
        */
        calendarCheck: "\uf274",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-day.svg.png) Calendar with Day Focus
         * Image Copyright FontAwesome.com
        */
        calendarDay: "\uf783",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-minus.svg.png) Calendar Minus
         * Image Copyright FontAwesome.com
        */
        calendarMinus: "\uf272",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-plus.svg.png) Calendar Plus
         * Image Copyright FontAwesome.com
        */
        calendarPlus: "\uf271",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-times.svg.png) Calendar Times
         * Image Copyright FontAwesome.com
        */
        calendarTimes: "\uf273",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/calendar-week.svg.png) Calendar with Week Focus
         * Image Copyright FontAwesome.com
        */
        calendarWeek: "\uf784",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/camera.svg.png) camera
         * Image Copyright FontAwesome.com
        */
        camera: "\uf030",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/camera-retro.svg.png) Retro Camera
         * Image Copyright FontAwesome.com
        */
        cameraRetro: "\uf083",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/campground.svg.png) Campground
         * Image Copyright FontAwesome.com
        */
        campground: "\uf6bb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/candy-cane.svg.png) Candy Cane
         * Image Copyright FontAwesome.com
        */
        candyCane: "\uf786",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cannabis.svg.png) Cannabis
         * Image Copyright FontAwesome.com
        */
        cannabis: "\uf55f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/capsules.svg.png) Capsules
         * Image Copyright FontAwesome.com
        */
        capsules: "\uf46b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/car.svg.png) Car
         * Image Copyright FontAwesome.com
        */
        car: "\uf1b9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/car-alt.svg.png) Alternate Car
         * Image Copyright FontAwesome.com
        */
        carAlt: "\uf5de",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/car-battery.svg.png) Car Battery
         * Image Copyright FontAwesome.com
        */
        carBattery: "\uf5df",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/car-crash.svg.png) Car Crash
         * Image Copyright FontAwesome.com
        */
        carCrash: "\uf5e1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/car-side.svg.png) Car Side
         * Image Copyright FontAwesome.com
        */
        carSide: "\uf5e4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caravan.svg.png) Caravan
         * Image Copyright FontAwesome.com
        */
        caravan: "\uf8ff",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-down.svg.png) Caret Down
         * Image Copyright FontAwesome.com
        */
        caretDown: "\uf0d7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-left.svg.png) Caret Left
         * Image Copyright FontAwesome.com
        */
        caretLeft: "\uf0d9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-right.svg.png) Caret Right
         * Image Copyright FontAwesome.com
        */
        caretRight: "\uf0da",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-square-down.svg.png) Caret Square Down
         * Image Copyright FontAwesome.com
        */
        caretSquareDown: "\uf150",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-square-left.svg.png) Caret Square Left
         * Image Copyright FontAwesome.com
        */
        caretSquareLeft: "\uf191",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-square-right.svg.png) Caret Square Right
         * Image Copyright FontAwesome.com
        */
        caretSquareRight: "\uf152",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-square-up.svg.png) Caret Square Up
         * Image Copyright FontAwesome.com
        */
        caretSquareUp: "\uf151",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/caret-up.svg.png) Caret Up
         * Image Copyright FontAwesome.com
        */
        caretUp: "\uf0d8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/carrot.svg.png) Carrot
         * Image Copyright FontAwesome.com
        */
        carrot: "\uf787",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cart-arrow-down.svg.png) Shopping Cart Arrow Down
         * Image Copyright FontAwesome.com
        */
        cartArrowDown: "\uf218",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cart-plus.svg.png) Add to Shopping Cart
         * Image Copyright FontAwesome.com
        */
        cartPlus: "\uf217",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cash-register.svg.png) Cash Register
         * Image Copyright FontAwesome.com
        */
        cashRegister: "\uf788",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cat.svg.png) Cat
         * Image Copyright FontAwesome.com
        */
        cat: "\uf6be",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/certificate.svg.png) certificate
         * Image Copyright FontAwesome.com
        */
        certificate: "\uf0a3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chair.svg.png) Chair
         * Image Copyright FontAwesome.com
        */
        chair: "\uf6c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chalkboard.svg.png) Chalkboard
         * Image Copyright FontAwesome.com
        */
        chalkboard: "\uf51b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chalkboard-teacher.svg.png) Chalkboard Teacher
         * Image Copyright FontAwesome.com
        */
        chalkboardTeacher: "\uf51c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/charging-station.svg.png) Charging Station
         * Image Copyright FontAwesome.com
        */
        chargingStation: "\uf5e7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chart-area.svg.png) Area Chart
         * Image Copyright FontAwesome.com
        */
        chartArea: "\uf1fe",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chart-bar.svg.png) Bar Chart
         * Image Copyright FontAwesome.com
        */
        chartBar: "\uf080",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chart-line.svg.png) Line Chart
         * Image Copyright FontAwesome.com
        */
        chartLine: "\uf201",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chart-pie.svg.png) Pie Chart
         * Image Copyright FontAwesome.com
        */
        chartPie: "\uf200",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/check.svg.png) Check
         * Image Copyright FontAwesome.com
        */
        check: "\uf00c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/check-circle.svg.png) Check Circle
         * Image Copyright FontAwesome.com
        */
        checkCircle: "\uf058",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/check-double.svg.png) Double Check
         * Image Copyright FontAwesome.com
        */
        checkDouble: "\uf560",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/check-square.svg.png) Check Square
         * Image Copyright FontAwesome.com
        */
        checkSquare: "\uf14a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cheese.svg.png) Cheese
         * Image Copyright FontAwesome.com
        */
        cheese: "\uf7ef",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess.svg.png) Chess
         * Image Copyright FontAwesome.com
        */
        chess: "\uf439",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-bishop.svg.png) Chess Bishop
         * Image Copyright FontAwesome.com
        */
        chessBishop: "\uf43a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-board.svg.png) Chess Board
         * Image Copyright FontAwesome.com
        */
        chessBoard: "\uf43c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-king.svg.png) Chess King
         * Image Copyright FontAwesome.com
        */
        chessKing: "\uf43f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-knight.svg.png) Chess Knight
         * Image Copyright FontAwesome.com
        */
        chessKnight: "\uf441",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-pawn.svg.png) Chess Pawn
         * Image Copyright FontAwesome.com
        */
        chessPawn: "\uf443",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-queen.svg.png) Chess Queen
         * Image Copyright FontAwesome.com
        */
        chessQueen: "\uf445",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chess-rook.svg.png) Chess Rook
         * Image Copyright FontAwesome.com
        */
        chessRook: "\uf447",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-circle-down.svg.png) Chevron Circle Down
         * Image Copyright FontAwesome.com
        */
        chevronCircleDown: "\uf13a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-circle-left.svg.png) Chevron Circle Left
         * Image Copyright FontAwesome.com
        */
        chevronCircleLeft: "\uf137",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-circle-right.svg.png) Chevron Circle Right
         * Image Copyright FontAwesome.com
        */
        chevronCircleRight: "\uf138",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-circle-up.svg.png) Chevron Circle Up
         * Image Copyright FontAwesome.com
        */
        chevronCircleUp: "\uf139",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-down.svg.png) chevron-down
         * Image Copyright FontAwesome.com
        */
        chevronDown: "\uf078",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-left.svg.png) chevron-left
         * Image Copyright FontAwesome.com
        */
        chevronLeft: "\uf053",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-right.svg.png) chevron-right
         * Image Copyright FontAwesome.com
        */
        chevronRight: "\uf054",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/chevron-up.svg.png) chevron-up
         * Image Copyright FontAwesome.com
        */
        chevronUp: "\uf077",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/child.svg.png) Child
         * Image Copyright FontAwesome.com
        */
        child: "\uf1ae",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/church.svg.png) Church
         * Image Copyright FontAwesome.com
        */
        church: "\uf51d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/circle.svg.png) Circle
         * Image Copyright FontAwesome.com
        */
        circle: "\uf111",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/circle-notch.svg.png) Circle Notched
         * Image Copyright FontAwesome.com
        */
        circleNotch: "\uf1ce",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/city.svg.png) City
         * Image Copyright FontAwesome.com
        */
        city: "\uf64f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/clinic-medical.svg.png) Medical Clinic
         * Image Copyright FontAwesome.com
        */
        clinicMedical: "\uf7f2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/clipboard.svg.png) Clipboard
         * Image Copyright FontAwesome.com
        */
        clipboard: "\uf328",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/clipboard-check.svg.png) Clipboard with Check
         * Image Copyright FontAwesome.com
        */
        clipboardCheck: "\uf46c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/clipboard-list.svg.png) Clipboard List
         * Image Copyright FontAwesome.com
        */
        clipboardList: "\uf46d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/clock.svg.png) Clock
         * Image Copyright FontAwesome.com
        */
        clock: "\uf017",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/clone.svg.png) Clone
         * Image Copyright FontAwesome.com
        */
        clone: "\uf24d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/closed-captioning.svg.png) Closed Captioning
         * Image Copyright FontAwesome.com
        */
        closedCaptioning: "\uf20a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud.svg.png) Cloud
         * Image Copyright FontAwesome.com
        */
        cloud: "\uf0c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-download-alt.svg.png) Alternate Cloud Download
         * Image Copyright FontAwesome.com
        */
        cloudDownloadAlt: "\uf381",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-meatball.svg.png) Cloud with (a chance of) Meatball
         * Image Copyright FontAwesome.com
        */
        cloudMeatball: "\uf73b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-moon.svg.png) Cloud with Moon
         * Image Copyright FontAwesome.com
        */
        cloudMoon: "\uf6c3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-moon-rain.svg.png) Cloud with Moon and Rain
         * Image Copyright FontAwesome.com
        */
        cloudMoonRain: "\uf73c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-rain.svg.png) Cloud with Rain
         * Image Copyright FontAwesome.com
        */
        cloudRain: "\uf73d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-showers-heavy.svg.png) Cloud with Heavy Showers
         * Image Copyright FontAwesome.com
        */
        cloudShowersHeavy: "\uf740",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-sun.svg.png) Cloud with Sun
         * Image Copyright FontAwesome.com
        */
        cloudSun: "\uf6c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-sun-rain.svg.png) Cloud with Sun and Rain
         * Image Copyright FontAwesome.com
        */
        cloudSunRain: "\uf743",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cloud-upload-alt.svg.png) Alternate Cloud Upload
         * Image Copyright FontAwesome.com
        */
        cloudUploadAlt: "\uf382",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cocktail.svg.png) Cocktail
         * Image Copyright FontAwesome.com
        */
        cocktail: "\uf561",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/code.svg.png) Code
         * Image Copyright FontAwesome.com
        */
        code: "\uf121",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/code-branch.svg.png) Code Branch
         * Image Copyright FontAwesome.com
        */
        codeBranch: "\uf126",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/coffee.svg.png) Coffee
         * Image Copyright FontAwesome.com
        */
        coffee: "\uf0f4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cog.svg.png) cog
         * Image Copyright FontAwesome.com
        */
        cog: "\uf013",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cogs.svg.png) cogs
         * Image Copyright FontAwesome.com
        */
        cogs: "\uf085",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/coins.svg.png) Coins
         * Image Copyright FontAwesome.com
        */
        coins: "\uf51e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/columns.svg.png) Columns
         * Image Copyright FontAwesome.com
        */
        columns: "\uf0db",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comment.svg.png) comment
         * Image Copyright FontAwesome.com
        */
        comment: "\uf075",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comment-alt.svg.png) Alternate Comment
         * Image Copyright FontAwesome.com
        */
        commentAlt: "\uf27a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comment-dollar.svg.png) Comment Dollar
         * Image Copyright FontAwesome.com
        */
        commentDollar: "\uf651",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comment-dots.svg.png) Comment Dots
         * Image Copyright FontAwesome.com
        */
        commentDots: "\uf4ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comment-medical.svg.png) Alternate Medical Chat
         * Image Copyright FontAwesome.com
        */
        commentMedical: "\uf7f5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comment-slash.svg.png) Comment Slash
         * Image Copyright FontAwesome.com
        */
        commentSlash: "\uf4b3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comments.svg.png) comments
         * Image Copyright FontAwesome.com
        */
        comments: "\uf086",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/comments-dollar.svg.png) Comments Dollar
         * Image Copyright FontAwesome.com
        */
        commentsDollar: "\uf653",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/compact-disc.svg.png) Compact Disc
         * Image Copyright FontAwesome.com
        */
        compactDisc: "\uf51f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/compass.svg.png) Compass
         * Image Copyright FontAwesome.com
        */
        compass: "\uf14e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/compress.svg.png) Compress
         * Image Copyright FontAwesome.com
        */
        compress: "\uf066",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/compress-alt.svg.png) Alternate Compress
         * Image Copyright FontAwesome.com
        */
        compressAlt: "\uf422",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/compress-arrows-alt.svg.png) Alternate Compress Arrows
         * Image Copyright FontAwesome.com
        */
        compressArrowsAlt: "\uf78c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/concierge-bell.svg.png) Concierge Bell
         * Image Copyright FontAwesome.com
        */
        conciergeBell: "\uf562",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cookie.svg.png) Cookie
         * Image Copyright FontAwesome.com
        */
        cookie: "\uf563",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cookie-bite.svg.png) Cookie Bite
         * Image Copyright FontAwesome.com
        */
        cookieBite: "\uf564",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/copy.svg.png) Copy
         * Image Copyright FontAwesome.com
        */
        copy: "\uf0c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/copyright.svg.png) Copyright
         * Image Copyright FontAwesome.com
        */
        copyright: "\uf1f9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/couch.svg.png) Couch
         * Image Copyright FontAwesome.com
        */
        couch: "\uf4b8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/credit-card.svg.png) Credit Card
         * Image Copyright FontAwesome.com
        */
        creditCard: "\uf09d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/crop.svg.png) crop
         * Image Copyright FontAwesome.com
        */
        crop: "\uf125",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/crop-alt.svg.png) Alternate Crop
         * Image Copyright FontAwesome.com
        */
        cropAlt: "\uf565",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cross.svg.png) Cross
         * Image Copyright FontAwesome.com
        */
        cross: "\uf654",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/crosshairs.svg.png) Crosshairs
         * Image Copyright FontAwesome.com
        */
        crosshairs: "\uf05b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/crow.svg.png) Crow
         * Image Copyright FontAwesome.com
        */
        crow: "\uf520",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/crown.svg.png) Crown
         * Image Copyright FontAwesome.com
        */
        crown: "\uf521",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/crutch.svg.png) Crutch
         * Image Copyright FontAwesome.com
        */
        crutch: "\uf7f7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cube.svg.png) Cube
         * Image Copyright FontAwesome.com
        */
        cube: "\uf1b2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cubes.svg.png) Cubes
         * Image Copyright FontAwesome.com
        */
        cubes: "\uf1b3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/cut.svg.png) Cut
         * Image Copyright FontAwesome.com
        */
        cut: "\uf0c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/database.svg.png) Database
         * Image Copyright FontAwesome.com
        */
        database: "\uf1c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/deaf.svg.png) Deaf
         * Image Copyright FontAwesome.com
        */
        deaf: "\uf2a4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/democrat.svg.png) Democrat
         * Image Copyright FontAwesome.com
        */
        democrat: "\uf747",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/desktop.svg.png) Desktop
         * Image Copyright FontAwesome.com
        */
        desktop: "\uf108",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dharmachakra.svg.png) Dharmachakra
         * Image Copyright FontAwesome.com
        */
        dharmachakra: "\uf655",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/diagnoses.svg.png) Diagnoses
         * Image Copyright FontAwesome.com
        */
        diagnoses: "\uf470",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice.svg.png) Dice
         * Image Copyright FontAwesome.com
        */
        dice: "\uf522",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-d20.svg.png) Dice D20
         * Image Copyright FontAwesome.com
        */
        diceD20: "\uf6cf",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-d6.svg.png) Dice D6
         * Image Copyright FontAwesome.com
        */
        diceD6: "\uf6d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-five.svg.png) Dice Five
         * Image Copyright FontAwesome.com
        */
        diceFive: "\uf523",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-four.svg.png) Dice Four
         * Image Copyright FontAwesome.com
        */
        diceFour: "\uf524",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-one.svg.png) Dice One
         * Image Copyright FontAwesome.com
        */
        diceOne: "\uf525",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-six.svg.png) Dice Six
         * Image Copyright FontAwesome.com
        */
        diceSix: "\uf526",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-three.svg.png) Dice Three
         * Image Copyright FontAwesome.com
        */
        diceThree: "\uf527",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dice-two.svg.png) Dice Two
         * Image Copyright FontAwesome.com
        */
        diceTwo: "\uf528",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/digital-tachograph.svg.png) Digital Tachograph
         * Image Copyright FontAwesome.com
        */
        digitalTachograph: "\uf566",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/directions.svg.png) Directions
         * Image Copyright FontAwesome.com
        */
        directions: "\uf5eb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/divide.svg.png) Divide
         * Image Copyright FontAwesome.com
        */
        divide: "\uf529",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dizzy.svg.png) Dizzy Face
         * Image Copyright FontAwesome.com
        */
        dizzy: "\uf567",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dna.svg.png) DNA
         * Image Copyright FontAwesome.com
        */
        dna: "\uf471",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dog.svg.png) Dog
         * Image Copyright FontAwesome.com
        */
        dog: "\uf6d3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dollar-sign.svg.png) Dollar Sign
         * Image Copyright FontAwesome.com
        */
        dollarSign: "\uf155",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dolly.svg.png) Dolly
         * Image Copyright FontAwesome.com
        */
        dolly: "\uf472",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dolly-flatbed.svg.png) Dolly Flatbed
         * Image Copyright FontAwesome.com
        */
        dollyFlatbed: "\uf474",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/donate.svg.png) Donate
         * Image Copyright FontAwesome.com
        */
        donate: "\uf4b9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/door-closed.svg.png) Door Closed
         * Image Copyright FontAwesome.com
        */
        doorClosed: "\uf52a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/door-open.svg.png) Door Open
         * Image Copyright FontAwesome.com
        */
        doorOpen: "\uf52b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dot-circle.svg.png) Dot Circle
         * Image Copyright FontAwesome.com
        */
        dotCircle: "\uf192",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dove.svg.png) Dove
         * Image Copyright FontAwesome.com
        */
        dove: "\uf4ba",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/download.svg.png) Download
         * Image Copyright FontAwesome.com
        */
        download: "\uf019",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/drafting-compass.svg.png) Drafting Compass
         * Image Copyright FontAwesome.com
        */
        draftingCompass: "\uf568",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dragon.svg.png) Dragon
         * Image Copyright FontAwesome.com
        */
        dragon: "\uf6d5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/draw-polygon.svg.png) Draw Polygon
         * Image Copyright FontAwesome.com
        */
        drawPolygon: "\uf5ee",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/drum.svg.png) Drum
         * Image Copyright FontAwesome.com
        */
        drum: "\uf569",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/drum-steelpan.svg.png) Drum Steelpan
         * Image Copyright FontAwesome.com
        */
        drumSteelpan: "\uf56a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/drumstick-bite.svg.png) Drumstick with Bite Taken Out
         * Image Copyright FontAwesome.com
        */
        drumstickBite: "\uf6d7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dumbbell.svg.png) Dumbbell
         * Image Copyright FontAwesome.com
        */
        dumbbell: "\uf44b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dumpster.svg.png) Dumpster
         * Image Copyright FontAwesome.com
        */
        dumpster: "\uf793",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dumpster-fire.svg.png) Dumpster Fire
         * Image Copyright FontAwesome.com
        */
        dumpsterFire: "\uf794",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/dungeon.svg.png) Dungeon
         * Image Copyright FontAwesome.com
        */
        dungeon: "\uf6d9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/edit.svg.png) Edit
         * Image Copyright FontAwesome.com
        */
        edit: "\uf044",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/egg.svg.png) Egg
         * Image Copyright FontAwesome.com
        */
        egg: "\uf7fb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/eject.svg.png) eject
         * Image Copyright FontAwesome.com
        */
        eject: "\uf052",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ellipsis-h.svg.png) Horizontal Ellipsis
         * Image Copyright FontAwesome.com
        */
        ellipsisH: "\uf141",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ellipsis-v.svg.png) Vertical Ellipsis
         * Image Copyright FontAwesome.com
        */
        ellipsisV: "\uf142",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/envelope.svg.png) Envelope
         * Image Copyright FontAwesome.com
        */
        envelope: "\uf0e0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/envelope-open.svg.png) Envelope Open
         * Image Copyright FontAwesome.com
        */
        envelopeOpen: "\uf2b6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/envelope-open-text.svg.png) Envelope Open-text
         * Image Copyright FontAwesome.com
        */
        envelopeOpenText: "\uf658",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/envelope-square.svg.png) Envelope Square
         * Image Copyright FontAwesome.com
        */
        envelopeSquare: "\uf199",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/equals.svg.png) Equals
         * Image Copyright FontAwesome.com
        */
        equals: "\uf52c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/eraser.svg.png) eraser
         * Image Copyright FontAwesome.com
        */
        eraser: "\uf12d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ethernet.svg.png) Ethernet
         * Image Copyright FontAwesome.com
        */
        ethernet: "\uf796",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/euro-sign.svg.png) Euro Sign
         * Image Copyright FontAwesome.com
        */
        euroSign: "\uf153",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/exchange-alt.svg.png) Alternate Exchange
         * Image Copyright FontAwesome.com
        */
        exchangeAlt: "\uf362",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/exclamation.svg.png) exclamation
         * Image Copyright FontAwesome.com
        */
        exclamation: "\uf12a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/exclamation-circle.svg.png) Exclamation Circle
         * Image Copyright FontAwesome.com
        */
        exclamationCircle: "\uf06a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/exclamation-triangle.svg.png) Exclamation Triangle
         * Image Copyright FontAwesome.com
        */
        exclamationTriangle: "\uf071",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/expand.svg.png) Expand
         * Image Copyright FontAwesome.com
        */
        expand: "\uf065",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/expand-alt.svg.png) Alternate Expand
         * Image Copyright FontAwesome.com
        */
        expandAlt: "\uf424",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/expand-arrows-alt.svg.png) Alternate Expand Arrows
         * Image Copyright FontAwesome.com
        */
        expandArrowsAlt: "\uf31e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/external-link-alt.svg.png) Alternate External Link
         * Image Copyright FontAwesome.com
        */
        externalLinkAlt: "\uf35d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/external-link-square-alt.svg.png) Alternate External Link Square
         * Image Copyright FontAwesome.com
        */
        externalLinkSquareAlt: "\uf360",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/eye.svg.png) Eye
         * Image Copyright FontAwesome.com
        */
        eye: "\uf06e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/eye-dropper.svg.png) Eye Dropper
         * Image Copyright FontAwesome.com
        */
        eyeDropper: "\uf1fb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/eye-slash.svg.png) Eye Slash
         * Image Copyright FontAwesome.com
        */
        eyeSlash: "\uf070",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fan.svg.png) Fan
         * Image Copyright FontAwesome.com
        */
        fan: "\uf863",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fast-backward.svg.png) fast-backward
         * Image Copyright FontAwesome.com
        */
        fastBackward: "\uf049",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fast-forward.svg.png) fast-forward
         * Image Copyright FontAwesome.com
        */
        fastForward: "\uf050",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fax.svg.png) Fax
         * Image Copyright FontAwesome.com
        */
        fax: "\uf1ac",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/feather.svg.png) Feather
         * Image Copyright FontAwesome.com
        */
        feather: "\uf52d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/feather-alt.svg.png) Alternate Feather
         * Image Copyright FontAwesome.com
        */
        featherAlt: "\uf56b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/female.svg.png) Female
         * Image Copyright FontAwesome.com
        */
        female: "\uf182",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fighter-jet.svg.png) fighter-jet
         * Image Copyright FontAwesome.com
        */
        fighterJet: "\uf0fb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file.svg.png) File
         * Image Copyright FontAwesome.com
        */
        file: "\uf15b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-alt.svg.png) Alternate File
         * Image Copyright FontAwesome.com
        */
        fileAlt: "\uf15c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-archive.svg.png) Archive File
         * Image Copyright FontAwesome.com
        */
        fileArchive: "\uf1c6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-audio.svg.png) Audio File
         * Image Copyright FontAwesome.com
        */
        fileAudio: "\uf1c7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-code.svg.png) Code File
         * Image Copyright FontAwesome.com
        */
        fileCode: "\uf1c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-contract.svg.png) File Contract
         * Image Copyright FontAwesome.com
        */
        fileContract: "\uf56c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-csv.svg.png) File CSV
         * Image Copyright FontAwesome.com
        */
        fileCsv: "\uf6dd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-download.svg.png) File Download
         * Image Copyright FontAwesome.com
        */
        fileDownload: "\uf56d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-excel.svg.png) Excel File
         * Image Copyright FontAwesome.com
        */
        fileExcel: "\uf1c3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-export.svg.png) File Export
         * Image Copyright FontAwesome.com
        */
        fileExport: "\uf56e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-image.svg.png) Image File
         * Image Copyright FontAwesome.com
        */
        fileImage: "\uf1c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-import.svg.png) File Import
         * Image Copyright FontAwesome.com
        */
        fileImport: "\uf56f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-invoice.svg.png) File Invoice
         * Image Copyright FontAwesome.com
        */
        fileInvoice: "\uf570",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-invoice-dollar.svg.png) File Invoice with US Dollar
         * Image Copyright FontAwesome.com
        */
        fileInvoiceDollar: "\uf571",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-medical.svg.png) Medical File
         * Image Copyright FontAwesome.com
        */
        fileMedical: "\uf477",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-medical-alt.svg.png) Alternate Medical File
         * Image Copyright FontAwesome.com
        */
        fileMedicalAlt: "\uf478",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-pdf.svg.png) PDF File
         * Image Copyright FontAwesome.com
        */
        filePdf: "\uf1c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-powerpoint.svg.png) Powerpoint File
         * Image Copyright FontAwesome.com
        */
        filePowerpoint: "\uf1c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-prescription.svg.png) File Prescription
         * Image Copyright FontAwesome.com
        */
        filePrescription: "\uf572",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-signature.svg.png) File Signature
         * Image Copyright FontAwesome.com
        */
        fileSignature: "\uf573",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-upload.svg.png) File Upload
         * Image Copyright FontAwesome.com
        */
        fileUpload: "\uf574",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-video.svg.png) Video File
         * Image Copyright FontAwesome.com
        */
        fileVideo: "\uf1c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/file-word.svg.png) Word File
         * Image Copyright FontAwesome.com
        */
        fileWord: "\uf1c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fill.svg.png) Fill
         * Image Copyright FontAwesome.com
        */
        fill: "\uf575",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fill-drip.svg.png) Fill Drip
         * Image Copyright FontAwesome.com
        */
        fillDrip: "\uf576",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/film.svg.png) Film
         * Image Copyright FontAwesome.com
        */
        film: "\uf008",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/filter.svg.png) Filter
         * Image Copyright FontAwesome.com
        */
        filter: "\uf0b0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fingerprint.svg.png) Fingerprint
         * Image Copyright FontAwesome.com
        */
        fingerprint: "\uf577",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fire.svg.png) fire
         * Image Copyright FontAwesome.com
        */
        fire: "\uf06d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fire-alt.svg.png) Alternate Fire
         * Image Copyright FontAwesome.com
        */
        fireAlt: "\uf7e4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fire-extinguisher.svg.png) fire-extinguisher
         * Image Copyright FontAwesome.com
        */
        fireExtinguisher: "\uf134",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/first-aid.svg.png) First Aid
         * Image Copyright FontAwesome.com
        */
        firstAid: "\uf479",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fish.svg.png) Fish
         * Image Copyright FontAwesome.com
        */
        fish: "\uf578",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/fist-raised.svg.png) Raised Fist
         * Image Copyright FontAwesome.com
        */
        fistRaised: "\uf6de",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/flag.svg.png) flag
         * Image Copyright FontAwesome.com
        */
        flag: "\uf024",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/flag-checkered.svg.png) flag-checkered
         * Image Copyright FontAwesome.com
        */
        flagCheckered: "\uf11e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/flag-usa.svg.png) United States of America Flag
         * Image Copyright FontAwesome.com
        */
        flagUsa: "\uf74d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/flask.svg.png) Flask
         * Image Copyright FontAwesome.com
        */
        flask: "\uf0c3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/flushed.svg.png) Flushed Face
         * Image Copyright FontAwesome.com
        */
        flushed: "\uf579",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/folder.svg.png) Folder
         * Image Copyright FontAwesome.com
        */
        folder: "\uf07b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/folder-minus.svg.png) Folder Minus
         * Image Copyright FontAwesome.com
        */
        folderMinus: "\uf65d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/folder-open.svg.png) Folder Open
         * Image Copyright FontAwesome.com
        */
        folderOpen: "\uf07c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/folder-plus.svg.png) Folder Plus
         * Image Copyright FontAwesome.com
        */
        folderPlus: "\uf65e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/font.svg.png) font
         * Image Copyright FontAwesome.com
        */
        font: "\uf031",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/font-awesome-logo-full.svg.png) Font Awesome Full Logo
         * Image Copyright FontAwesome.com
        */
        fontAwesomeLogoFull: "\uf4e6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/football-ball.svg.png) Football Ball
         * Image Copyright FontAwesome.com
        */
        footballBall: "\uf44e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/forward.svg.png) forward
         * Image Copyright FontAwesome.com
        */
        forward: "\uf04e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/frog.svg.png) Frog
         * Image Copyright FontAwesome.com
        */
        frog: "\uf52e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/frown.svg.png) Frowning Face
         * Image Copyright FontAwesome.com
        */
        frown: "\uf119",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/frown-open.svg.png) Frowning Face With Open Mouth
         * Image Copyright FontAwesome.com
        */
        frownOpen: "\uf57a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/funnel-dollar.svg.png) Funnel Dollar
         * Image Copyright FontAwesome.com
        */
        funnelDollar: "\uf662",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/futbol.svg.png) Futbol
         * Image Copyright FontAwesome.com
        */
        futbol: "\uf1e3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gamepad.svg.png) Gamepad
         * Image Copyright FontAwesome.com
        */
        gamepad: "\uf11b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gas-pump.svg.png) Gas Pump
         * Image Copyright FontAwesome.com
        */
        gasPump: "\uf52f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gavel.svg.png) Gavel
         * Image Copyright FontAwesome.com
        */
        gavel: "\uf0e3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gem.svg.png) Gem
         * Image Copyright FontAwesome.com
        */
        gem: "\uf3a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/genderless.svg.png) Genderless
         * Image Copyright FontAwesome.com
        */
        genderless: "\uf22d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ghost.svg.png) Ghost
         * Image Copyright FontAwesome.com
        */
        ghost: "\uf6e2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gift.svg.png) gift
         * Image Copyright FontAwesome.com
        */
        gift: "\uf06b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gifts.svg.png) Gifts
         * Image Copyright FontAwesome.com
        */
        gifts: "\uf79c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/glass-cheers.svg.png) Glass Cheers
         * Image Copyright FontAwesome.com
        */
        glassCheers: "\uf79f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/glass-martini.svg.png) Martini Glass
         * Image Copyright FontAwesome.com
        */
        glassMartini: "\uf000",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/glass-martini-alt.svg.png) Alternate Glass Martini
         * Image Copyright FontAwesome.com
        */
        glassMartiniAlt: "\uf57b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/glass-whiskey.svg.png) Glass Whiskey
         * Image Copyright FontAwesome.com
        */
        glassWhiskey: "\uf7a0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/glasses.svg.png) Glasses
         * Image Copyright FontAwesome.com
        */
        glasses: "\uf530",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/globe.svg.png) Globe
         * Image Copyright FontAwesome.com
        */
        globe: "\uf0ac",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/globe-africa.svg.png) Globe with Africa shown
         * Image Copyright FontAwesome.com
        */
        globeAfrica: "\uf57c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/globe-americas.svg.png) Globe with Americas shown
         * Image Copyright FontAwesome.com
        */
        globeAmericas: "\uf57d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/globe-asia.svg.png) Globe with Asia shown
         * Image Copyright FontAwesome.com
        */
        globeAsia: "\uf57e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/globe-europe.svg.png) Globe with Europe shown
         * Image Copyright FontAwesome.com
        */
        globeEurope: "\uf7a2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/golf-ball.svg.png) Golf Ball
         * Image Copyright FontAwesome.com
        */
        golfBall: "\uf450",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/gopuram.svg.png) Gopuram
         * Image Copyright FontAwesome.com
        */
        gopuram: "\uf664",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/graduation-cap.svg.png) Graduation Cap
         * Image Copyright FontAwesome.com
        */
        graduationCap: "\uf19d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/greater-than.svg.png) Greater Than
         * Image Copyright FontAwesome.com
        */
        greaterThan: "\uf531",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/greater-than-equal.svg.png) Greater Than Equal To
         * Image Copyright FontAwesome.com
        */
        greaterThanEqual: "\uf532",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grimace.svg.png) Grimacing Face
         * Image Copyright FontAwesome.com
        */
        grimace: "\uf57f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin.svg.png) Grinning Face
         * Image Copyright FontAwesome.com
        */
        grin: "\uf580",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-alt.svg.png) Alternate Grinning Face
         * Image Copyright FontAwesome.com
        */
        grinAlt: "\uf581",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-beam.svg.png) Grinning Face With Smiling Eyes
         * Image Copyright FontAwesome.com
        */
        grinBeam: "\uf582",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-beam-sweat.svg.png) Grinning Face With Sweat
         * Image Copyright FontAwesome.com
        */
        grinBeamSweat: "\uf583",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-hearts.svg.png) Smiling Face With Heart-Eyes
         * Image Copyright FontAwesome.com
        */
        grinHearts: "\uf584",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-squint.svg.png) Grinning Squinting Face
         * Image Copyright FontAwesome.com
        */
        grinSquint: "\uf585",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-squint-tears.svg.png) Rolling on the Floor Laughing
         * Image Copyright FontAwesome.com
        */
        grinSquintTears: "\uf586",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-stars.svg.png) Star-Struck
         * Image Copyright FontAwesome.com
        */
        grinStars: "\uf587",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-tears.svg.png) Face With Tears of Joy
         * Image Copyright FontAwesome.com
        */
        grinTears: "\uf588",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-tongue.svg.png) Face With Tongue
         * Image Copyright FontAwesome.com
        */
        grinTongue: "\uf589",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-tongue-squint.svg.png) Squinting Face With Tongue
         * Image Copyright FontAwesome.com
        */
        grinTongueSquint: "\uf58a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-tongue-wink.svg.png) Winking Face With Tongue
         * Image Copyright FontAwesome.com
        */
        grinTongueWink: "\uf58b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grin-wink.svg.png) Grinning Winking Face
         * Image Copyright FontAwesome.com
        */
        grinWink: "\uf58c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grip-horizontal.svg.png) Grip Horizontal
         * Image Copyright FontAwesome.com
        */
        gripHorizontal: "\uf58d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grip-lines.svg.png) Grip Lines
         * Image Copyright FontAwesome.com
        */
        gripLines: "\uf7a4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grip-lines-vertical.svg.png) Grip Lines Vertical
         * Image Copyright FontAwesome.com
        */
        gripLinesVertical: "\uf7a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/grip-vertical.svg.png) Grip Vertical
         * Image Copyright FontAwesome.com
        */
        gripVertical: "\uf58e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/guitar.svg.png) Guitar
         * Image Copyright FontAwesome.com
        */
        guitar: "\uf7a6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/h-square.svg.png) H Square
         * Image Copyright FontAwesome.com
        */
        hSquare: "\uf0fd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hamburger.svg.png) Hamburger
         * Image Copyright FontAwesome.com
        */
        hamburger: "\uf805",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hammer.svg.png) Hammer
         * Image Copyright FontAwesome.com
        */
        hammer: "\uf6e3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hamsa.svg.png) Hamsa
         * Image Copyright FontAwesome.com
        */
        hamsa: "\uf665",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-holding.svg.png) Hand Holding
         * Image Copyright FontAwesome.com
        */
        handHolding: "\uf4bd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-holding-heart.svg.png) Hand Holding Heart
         * Image Copyright FontAwesome.com
        */
        handHoldingHeart: "\uf4be",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-holding-usd.svg.png) Hand Holding US Dollar
         * Image Copyright FontAwesome.com
        */
        handHoldingUsd: "\uf4c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-lizard.svg.png) Lizard (Hand)
         * Image Copyright FontAwesome.com
        */
        handLizard: "\uf258",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-middle-finger.svg.png) Hand with Middle Finger Raised
         * Image Copyright FontAwesome.com
        */
        handMiddleFinger: "\uf806",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-paper.svg.png) Paper (Hand)
         * Image Copyright FontAwesome.com
        */
        handPaper: "\uf256",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-peace.svg.png) Peace (Hand)
         * Image Copyright FontAwesome.com
        */
        handPeace: "\uf25b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-point-down.svg.png) Hand Pointing Down
         * Image Copyright FontAwesome.com
        */
        handPointDown: "\uf0a7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-point-left.svg.png) Hand Pointing Left
         * Image Copyright FontAwesome.com
        */
        handPointLeft: "\uf0a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-point-right.svg.png) Hand Pointing Right
         * Image Copyright FontAwesome.com
        */
        handPointRight: "\uf0a4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-point-up.svg.png) Hand Pointing Up
         * Image Copyright FontAwesome.com
        */
        handPointUp: "\uf0a6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-pointer.svg.png) Pointer (Hand)
         * Image Copyright FontAwesome.com
        */
        handPointer: "\uf25a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-rock.svg.png) Rock (Hand)
         * Image Copyright FontAwesome.com
        */
        handRock: "\uf255",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-scissors.svg.png) Scissors (Hand)
         * Image Copyright FontAwesome.com
        */
        handScissors: "\uf257",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hand-spock.svg.png) Spock (Hand)
         * Image Copyright FontAwesome.com
        */
        handSpock: "\uf259",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hands.svg.png) Hands
         * Image Copyright FontAwesome.com
        */
        hands: "\uf4c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hands-helping.svg.png) Helping Hands
         * Image Copyright FontAwesome.com
        */
        handsHelping: "\uf4c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/handshake.svg.png) Handshake
         * Image Copyright FontAwesome.com
        */
        handshake: "\uf2b5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hanukiah.svg.png) Hanukiah
         * Image Copyright FontAwesome.com
        */
        hanukiah: "\uf6e6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hard-hat.svg.png) Hard Hat
         * Image Copyright FontAwesome.com
        */
        hardHat: "\uf807",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hashtag.svg.png) Hashtag
         * Image Copyright FontAwesome.com
        */
        hashtag: "\uf292",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hat-cowboy.svg.png) Cowboy Hat
         * Image Copyright FontAwesome.com
        */
        hatCowboy: "\uf8c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hat-cowboy-side.svg.png) Cowboy Hat Side
         * Image Copyright FontAwesome.com
        */
        hatCowboySide: "\uf8c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hat-wizard.svg.png) Wizard's Hat
         * Image Copyright FontAwesome.com
        */
        hatWizard: "\uf6e8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hdd.svg.png) HDD
         * Image Copyright FontAwesome.com
        */
        hdd: "\uf0a0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/heading.svg.png) heading
         * Image Copyright FontAwesome.com
        */
        heading: "\uf1dc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/headphones.svg.png) headphones
         * Image Copyright FontAwesome.com
        */
        headphones: "\uf025",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/headphones-alt.svg.png) Alternate Headphones
         * Image Copyright FontAwesome.com
        */
        headphonesAlt: "\uf58f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/headset.svg.png) Headset
         * Image Copyright FontAwesome.com
        */
        headset: "\uf590",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/heart.svg.png) Heart
         * Image Copyright FontAwesome.com
        */
        heart: "\uf004",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/heart-broken.svg.png) Heart Broken
         * Image Copyright FontAwesome.com
        */
        heartBroken: "\uf7a9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/heartbeat.svg.png) Heartbeat
         * Image Copyright FontAwesome.com
        */
        heartbeat: "\uf21e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/helicopter.svg.png) Helicopter
         * Image Copyright FontAwesome.com
        */
        helicopter: "\uf533",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/highlighter.svg.png) Highlighter
         * Image Copyright FontAwesome.com
        */
        highlighter: "\uf591",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hiking.svg.png) Hiking
         * Image Copyright FontAwesome.com
        */
        hiking: "\uf6ec",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hippo.svg.png) Hippo
         * Image Copyright FontAwesome.com
        */
        hippo: "\uf6ed",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/history.svg.png) History
         * Image Copyright FontAwesome.com
        */
        history: "\uf1da",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hockey-puck.svg.png) Hockey Puck
         * Image Copyright FontAwesome.com
        */
        hockeyPuck: "\uf453",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/holly-berry.svg.png) Holly Berry
         * Image Copyright FontAwesome.com
        */
        hollyBerry: "\uf7aa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/home.svg.png) home
         * Image Copyright FontAwesome.com
        */
        home: "\uf015",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/horse.svg.png) Horse
         * Image Copyright FontAwesome.com
        */
        horse: "\uf6f0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/horse-head.svg.png) Horse Head
         * Image Copyright FontAwesome.com
        */
        horseHead: "\uf7ab",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hospital.svg.png) hospital
         * Image Copyright FontAwesome.com
        */
        hospital: "\uf0f8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hospital-alt.svg.png) Alternate Hospital
         * Image Copyright FontAwesome.com
        */
        hospitalAlt: "\uf47d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hospital-symbol.svg.png) Hospital Symbol
         * Image Copyright FontAwesome.com
        */
        hospitalSymbol: "\uf47e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hot-tub.svg.png) Hot Tub
         * Image Copyright FontAwesome.com
        */
        hotTub: "\uf593",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hotdog.svg.png) Hot Dog
         * Image Copyright FontAwesome.com
        */
        hotdog: "\uf80f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hotel.svg.png) Hotel
         * Image Copyright FontAwesome.com
        */
        hotel: "\uf594",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hourglass.svg.png) Hourglass
         * Image Copyright FontAwesome.com
        */
        hourglass: "\uf254",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hourglass-end.svg.png) Hourglass End
         * Image Copyright FontAwesome.com
        */
        hourglassEnd: "\uf253",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hourglass-half.svg.png) Hourglass Half
         * Image Copyright FontAwesome.com
        */
        hourglassHalf: "\uf252",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hourglass-start.svg.png) Hourglass Start
         * Image Copyright FontAwesome.com
        */
        hourglassStart: "\uf251",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/house-damage.svg.png) Damaged House
         * Image Copyright FontAwesome.com
        */
        houseDamage: "\uf6f1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/hryvnia.svg.png) Hryvnia
         * Image Copyright FontAwesome.com
        */
        hryvnia: "\uf6f2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/i-cursor.svg.png) I Beam Cursor
         * Image Copyright FontAwesome.com
        */
        iCursor: "\uf246",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ice-cream.svg.png) Ice Cream
         * Image Copyright FontAwesome.com
        */
        iceCream: "\uf810",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/icicles.svg.png) Icicles
         * Image Copyright FontAwesome.com
        */
        icicles: "\uf7ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/icons.svg.png) Icons
         * Image Copyright FontAwesome.com
        */
        icons: "\uf86d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/id-badge.svg.png) Identification Badge
         * Image Copyright FontAwesome.com
        */
        idBadge: "\uf2c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/id-card.svg.png) Identification Card
         * Image Copyright FontAwesome.com
        */
        idCard: "\uf2c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/id-card-alt.svg.png) Alternate Identification Card
         * Image Copyright FontAwesome.com
        */
        idCardAlt: "\uf47f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/igloo.svg.png) Igloo
         * Image Copyright FontAwesome.com
        */
        igloo: "\uf7ae",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/image.svg.png) Image
         * Image Copyright FontAwesome.com
        */
        image: "\uf03e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/images.svg.png) Images
         * Image Copyright FontAwesome.com
        */
        images: "\uf302",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/inbox.svg.png) inbox
         * Image Copyright FontAwesome.com
        */
        inbox: "\uf01c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/indent.svg.png) Indent
         * Image Copyright FontAwesome.com
        */
        indent: "\uf03c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/industry.svg.png) Industry
         * Image Copyright FontAwesome.com
        */
        industry: "\uf275",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/infinity.svg.png) Infinity
         * Image Copyright FontAwesome.com
        */
        infinity: "\uf534",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/info.svg.png) Info
         * Image Copyright FontAwesome.com
        */
        info: "\uf129",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/info-circle.svg.png) Info Circle
         * Image Copyright FontAwesome.com
        */
        infoCircle: "\uf05a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/italic.svg.png) italic
         * Image Copyright FontAwesome.com
        */
        italic: "\uf033",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/jedi.svg.png) Jedi
         * Image Copyright FontAwesome.com
        */
        jedi: "\uf669",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/joint.svg.png) Joint
         * Image Copyright FontAwesome.com
        */
        joint: "\uf595",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/journal-whills.svg.png) Journal of the Whills
         * Image Copyright FontAwesome.com
        */
        journalWhills: "\uf66a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/kaaba.svg.png) Kaaba
         * Image Copyright FontAwesome.com
        */
        kaaba: "\uf66b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/key.svg.png) key
         * Image Copyright FontAwesome.com
        */
        key: "\uf084",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/keyboard.svg.png) Keyboard
         * Image Copyright FontAwesome.com
        */
        keyboard: "\uf11c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/khanda.svg.png) Khanda
         * Image Copyright FontAwesome.com
        */
        khanda: "\uf66d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/kiss.svg.png) Kissing Face
         * Image Copyright FontAwesome.com
        */
        kiss: "\uf596",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/kiss-beam.svg.png) Kissing Face With Smiling Eyes
         * Image Copyright FontAwesome.com
        */
        kissBeam: "\uf597",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/kiss-wink-heart.svg.png) Face Blowing a Kiss
         * Image Copyright FontAwesome.com
        */
        kissWinkHeart: "\uf598",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/kiwi-bird.svg.png) Kiwi Bird
         * Image Copyright FontAwesome.com
        */
        kiwiBird: "\uf535",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/landmark.svg.png) Landmark
         * Image Copyright FontAwesome.com
        */
        landmark: "\uf66f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/language.svg.png) Language
         * Image Copyright FontAwesome.com
        */
        language: "\uf1ab",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laptop.svg.png) Laptop
         * Image Copyright FontAwesome.com
        */
        laptop: "\uf109",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laptop-code.svg.png) Laptop Code
         * Image Copyright FontAwesome.com
        */
        laptopCode: "\uf5fc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laptop-medical.svg.png) Laptop Medical
         * Image Copyright FontAwesome.com
        */
        laptopMedical: "\uf812",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laugh.svg.png) Grinning Face With Big Eyes
         * Image Copyright FontAwesome.com
        */
        laugh: "\uf599",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laugh-beam.svg.png) Laugh Face with Beaming Eyes
         * Image Copyright FontAwesome.com
        */
        laughBeam: "\uf59a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laugh-squint.svg.png) Laughing Squinting Face
         * Image Copyright FontAwesome.com
        */
        laughSquint: "\uf59b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/laugh-wink.svg.png) Laughing Winking Face
         * Image Copyright FontAwesome.com
        */
        laughWink: "\uf59c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/layer-group.svg.png) Layer Group
         * Image Copyright FontAwesome.com
        */
        layerGroup: "\uf5fd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/leaf.svg.png) leaf
         * Image Copyright FontAwesome.com
        */
        leaf: "\uf06c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/lemon.svg.png) Lemon
         * Image Copyright FontAwesome.com
        */
        lemon: "\uf094",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/less-than.svg.png) Less Than
         * Image Copyright FontAwesome.com
        */
        lessThan: "\uf536",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/less-than-equal.svg.png) Less Than Equal To
         * Image Copyright FontAwesome.com
        */
        lessThanEqual: "\uf537",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/level-down-alt.svg.png) Alternate Level Down
         * Image Copyright FontAwesome.com
        */
        levelDownAlt: "\uf3be",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/level-up-alt.svg.png) Alternate Level Up
         * Image Copyright FontAwesome.com
        */
        levelUpAlt: "\uf3bf",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/life-ring.svg.png) Life Ring
         * Image Copyright FontAwesome.com
        */
        lifeRing: "\uf1cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/lightbulb.svg.png) Lightbulb
         * Image Copyright FontAwesome.com
        */
        lightbulb: "\uf0eb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/link.svg.png) Link
         * Image Copyright FontAwesome.com
        */
        link: "\uf0c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/lira-sign.svg.png) Turkish Lira Sign
         * Image Copyright FontAwesome.com
        */
        liraSign: "\uf195",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/list.svg.png) List
         * Image Copyright FontAwesome.com
        */
        list: "\uf03a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/list-alt.svg.png) Alternate List
         * Image Copyright FontAwesome.com
        */
        listAlt: "\uf022",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/list-ol.svg.png) list-ol
         * Image Copyright FontAwesome.com
        */
        listOl: "\uf0cb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/list-ul.svg.png) list-ul
         * Image Copyright FontAwesome.com
        */
        listUl: "\uf0ca",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/location-arrow.svg.png) location-arrow
         * Image Copyright FontAwesome.com
        */
        locationArrow: "\uf124",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/lock.svg.png) lock
         * Image Copyright FontAwesome.com
        */
        lock: "\uf023",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/lock-open.svg.png) Lock Open
         * Image Copyright FontAwesome.com
        */
        lockOpen: "\uf3c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/long-arrow-alt-down.svg.png) Alternate Long Arrow Down
         * Image Copyright FontAwesome.com
        */
        longArrowAltDown: "\uf309",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/long-arrow-alt-left.svg.png) Alternate Long Arrow Left
         * Image Copyright FontAwesome.com
        */
        longArrowAltLeft: "\uf30a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/long-arrow-alt-right.svg.png) Alternate Long Arrow Right
         * Image Copyright FontAwesome.com
        */
        longArrowAltRight: "\uf30b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/long-arrow-alt-up.svg.png) Alternate Long Arrow Up
         * Image Copyright FontAwesome.com
        */
        longArrowAltUp: "\uf30c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/low-vision.svg.png) Low Vision
         * Image Copyright FontAwesome.com
        */
        lowVision: "\uf2a8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/luggage-cart.svg.png) Luggage Cart
         * Image Copyright FontAwesome.com
        */
        luggageCart: "\uf59d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/magic.svg.png) magic
         * Image Copyright FontAwesome.com
        */
        magic: "\uf0d0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/magnet.svg.png) magnet
         * Image Copyright FontAwesome.com
        */
        magnet: "\uf076",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mail-bulk.svg.png) Mail Bulk
         * Image Copyright FontAwesome.com
        */
        mailBulk: "\uf674",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/male.svg.png) Male
         * Image Copyright FontAwesome.com
        */
        male: "\uf183",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map.svg.png) Map
         * Image Copyright FontAwesome.com
        */
        map: "\uf279",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map-marked.svg.png) Map Marked
         * Image Copyright FontAwesome.com
        */
        mapMarked: "\uf59f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map-marked-alt.svg.png) Alternate Map Marked
         * Image Copyright FontAwesome.com
        */
        mapMarkedAlt: "\uf5a0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map-marker.svg.png) map-marker
         * Image Copyright FontAwesome.com
        */
        mapMarker: "\uf041",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map-marker-alt.svg.png) Alternate Map Marker
         * Image Copyright FontAwesome.com
        */
        mapMarkerAlt: "\uf3c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map-pin.svg.png) Map Pin
         * Image Copyright FontAwesome.com
        */
        mapPin: "\uf276",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/map-signs.svg.png) Map Signs
         * Image Copyright FontAwesome.com
        */
        mapSigns: "\uf277",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/marker.svg.png) Marker
         * Image Copyright FontAwesome.com
        */
        marker: "\uf5a1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mars.svg.png) Mars
         * Image Copyright FontAwesome.com
        */
        mars: "\uf222",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mars-double.svg.png) Mars Double
         * Image Copyright FontAwesome.com
        */
        marsDouble: "\uf227",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mars-stroke.svg.png) Mars Stroke
         * Image Copyright FontAwesome.com
        */
        marsStroke: "\uf229",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mars-stroke-h.svg.png) Mars Stroke Horizontal
         * Image Copyright FontAwesome.com
        */
        marsStrokeH: "\uf22b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mars-stroke-v.svg.png) Mars Stroke Vertical
         * Image Copyright FontAwesome.com
        */
        marsStrokeV: "\uf22a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mask.svg.png) Mask
         * Image Copyright FontAwesome.com
        */
        mask: "\uf6fa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/medal.svg.png) Medal
         * Image Copyright FontAwesome.com
        */
        medal: "\uf5a2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/medkit.svg.png) medkit
         * Image Copyright FontAwesome.com
        */
        medkit: "\uf0fa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/meh.svg.png) Neutral Face
         * Image Copyright FontAwesome.com
        */
        meh: "\uf11a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/meh-blank.svg.png) Face Without Mouth
         * Image Copyright FontAwesome.com
        */
        mehBlank: "\uf5a4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/meh-rolling-eyes.svg.png) Face With Rolling Eyes
         * Image Copyright FontAwesome.com
        */
        mehRollingEyes: "\uf5a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/memory.svg.png) Memory
         * Image Copyright FontAwesome.com
        */
        memory: "\uf538",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/menorah.svg.png) Menorah
         * Image Copyright FontAwesome.com
        */
        menorah: "\uf676",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mercury.svg.png) Mercury
         * Image Copyright FontAwesome.com
        */
        mercury: "\uf223",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/meteor.svg.png) Meteor
         * Image Copyright FontAwesome.com
        */
        meteor: "\uf753",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/microchip.svg.png) Microchip
         * Image Copyright FontAwesome.com
        */
        microchip: "\uf2db",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/microphone.svg.png) microphone
         * Image Copyright FontAwesome.com
        */
        microphone: "\uf130",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/microphone-alt.svg.png) Alternate Microphone
         * Image Copyright FontAwesome.com
        */
        microphoneAlt: "\uf3c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/microphone-alt-slash.svg.png) Alternate Microphone Slash
         * Image Copyright FontAwesome.com
        */
        microphoneAltSlash: "\uf539",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/microphone-slash.svg.png) Microphone Slash
         * Image Copyright FontAwesome.com
        */
        microphoneSlash: "\uf131",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/microscope.svg.png) Microscope
         * Image Copyright FontAwesome.com
        */
        microscope: "\uf610",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/minus.svg.png) minus
         * Image Copyright FontAwesome.com
        */
        minus: "\uf068",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/minus-circle.svg.png) Minus Circle
         * Image Copyright FontAwesome.com
        */
        minusCircle: "\uf056",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/minus-square.svg.png) Minus Square
         * Image Copyright FontAwesome.com
        */
        minusSquare: "\uf146",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mitten.svg.png) Mitten
         * Image Copyright FontAwesome.com
        */
        mitten: "\uf7b5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mobile.svg.png) Mobile Phone
         * Image Copyright FontAwesome.com
        */
        mobile: "\uf10b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mobile-alt.svg.png) Alternate Mobile
         * Image Copyright FontAwesome.com
        */
        mobileAlt: "\uf3cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/money-bill.svg.png) Money Bill
         * Image Copyright FontAwesome.com
        */
        moneyBill: "\uf0d6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/money-bill-alt.svg.png) Alternate Money Bill
         * Image Copyright FontAwesome.com
        */
        moneyBillAlt: "\uf3d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/money-bill-wave.svg.png) Wavy Money Bill
         * Image Copyright FontAwesome.com
        */
        moneyBillWave: "\uf53a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/money-bill-wave-alt.svg.png) Alternate Wavy Money Bill
         * Image Copyright FontAwesome.com
        */
        moneyBillWaveAlt: "\uf53b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/money-check.svg.png) Money Check
         * Image Copyright FontAwesome.com
        */
        moneyCheck: "\uf53c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/money-check-alt.svg.png) Alternate Money Check
         * Image Copyright FontAwesome.com
        */
        moneyCheckAlt: "\uf53d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/monument.svg.png) Monument
         * Image Copyright FontAwesome.com
        */
        monument: "\uf5a6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/moon.svg.png) Moon
         * Image Copyright FontAwesome.com
        */
        moon: "\uf186",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mortar-pestle.svg.png) Mortar Pestle
         * Image Copyright FontAwesome.com
        */
        mortarPestle: "\uf5a7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mosque.svg.png) Mosque
         * Image Copyright FontAwesome.com
        */
        mosque: "\uf678",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/motorcycle.svg.png) Motorcycle
         * Image Copyright FontAwesome.com
        */
        motorcycle: "\uf21c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mountain.svg.png) Mountain
         * Image Copyright FontAwesome.com
        */
        mountain: "\uf6fc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mouse.svg.png) Mouse
         * Image Copyright FontAwesome.com
        */
        mouse: "\uf8cc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mouse-pointer.svg.png) Mouse Pointer
         * Image Copyright FontAwesome.com
        */
        mousePointer: "\uf245",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/mug-hot.svg.png) Mug Hot
         * Image Copyright FontAwesome.com
        */
        mugHot: "\uf7b6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/music.svg.png) Music
         * Image Copyright FontAwesome.com
        */
        music: "\uf001",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/network-wired.svg.png) Wired Network
         * Image Copyright FontAwesome.com
        */
        networkWired: "\uf6ff",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/neuter.svg.png) Neuter
         * Image Copyright FontAwesome.com
        */
        neuter: "\uf22c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/newspaper.svg.png) Newspaper
         * Image Copyright FontAwesome.com
        */
        newspaper: "\uf1ea",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/not-equal.svg.png) Not Equal
         * Image Copyright FontAwesome.com
        */
        notEqual: "\uf53e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/notes-medical.svg.png) Medical Notes
         * Image Copyright FontAwesome.com
        */
        notesMedical: "\uf481",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/object-group.svg.png) Object Group
         * Image Copyright FontAwesome.com
        */
        objectGroup: "\uf247",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/object-ungroup.svg.png) Object Ungroup
         * Image Copyright FontAwesome.com
        */
        objectUngroup: "\uf248",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/oil-can.svg.png) Oil Can
         * Image Copyright FontAwesome.com
        */
        oilCan: "\uf613",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/om.svg.png) Om
         * Image Copyright FontAwesome.com
        */
        om: "\uf679",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/otter.svg.png) Otter
         * Image Copyright FontAwesome.com
        */
        otter: "\uf700",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/outdent.svg.png) Outdent
         * Image Copyright FontAwesome.com
        */
        outdent: "\uf03b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pager.svg.png) Pager
         * Image Copyright FontAwesome.com
        */
        pager: "\uf815",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paint-brush.svg.png) Paint Brush
         * Image Copyright FontAwesome.com
        */
        paintBrush: "\uf1fc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paint-roller.svg.png) Paint Roller
         * Image Copyright FontAwesome.com
        */
        paintRoller: "\uf5aa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/palette.svg.png) Palette
         * Image Copyright FontAwesome.com
        */
        palette: "\uf53f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pallet.svg.png) Pallet
         * Image Copyright FontAwesome.com
        */
        pallet: "\uf482",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paper-plane.svg.png) Paper Plane
         * Image Copyright FontAwesome.com
        */
        paperPlane: "\uf1d8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paperclip.svg.png) Paperclip
         * Image Copyright FontAwesome.com
        */
        paperclip: "\uf0c6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/parachute-box.svg.png) Parachute Box
         * Image Copyright FontAwesome.com
        */
        parachuteBox: "\uf4cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paragraph.svg.png) paragraph
         * Image Copyright FontAwesome.com
        */
        paragraph: "\uf1dd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/parking.svg.png) Parking
         * Image Copyright FontAwesome.com
        */
        parking: "\uf540",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/passport.svg.png) Passport
         * Image Copyright FontAwesome.com
        */
        passport: "\uf5ab",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pastafarianism.svg.png) Pastafarianism
         * Image Copyright FontAwesome.com
        */
        pastafarianism: "\uf67b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paste.svg.png) Paste
         * Image Copyright FontAwesome.com
        */
        paste: "\uf0ea",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pause.svg.png) pause
         * Image Copyright FontAwesome.com
        */
        pause: "\uf04c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pause-circle.svg.png) Pause Circle
         * Image Copyright FontAwesome.com
        */
        pauseCircle: "\uf28b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/paw.svg.png) Paw
         * Image Copyright FontAwesome.com
        */
        paw: "\uf1b0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/peace.svg.png) Peace
         * Image Copyright FontAwesome.com
        */
        peace: "\uf67c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pen.svg.png) Pen
         * Image Copyright FontAwesome.com
        */
        pen: "\uf304",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pen-alt.svg.png) Alternate Pen
         * Image Copyright FontAwesome.com
        */
        penAlt: "\uf305",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pen-fancy.svg.png) Pen Fancy
         * Image Copyright FontAwesome.com
        */
        penFancy: "\uf5ac",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pen-nib.svg.png) Pen Nib
         * Image Copyright FontAwesome.com
        */
        penNib: "\uf5ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pen-square.svg.png) Pen Square
         * Image Copyright FontAwesome.com
        */
        penSquare: "\uf14b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pencil-alt.svg.png) Alternate Pencil
         * Image Copyright FontAwesome.com
        */
        pencilAlt: "\uf303",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pencil-ruler.svg.png) Pencil Ruler
         * Image Copyright FontAwesome.com
        */
        pencilRuler: "\uf5ae",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/people-carry.svg.png) People Carry
         * Image Copyright FontAwesome.com
        */
        peopleCarry: "\uf4ce",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pepper-hot.svg.png) Hot Pepper
         * Image Copyright FontAwesome.com
        */
        pepperHot: "\uf816",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/percent.svg.png) Percent
         * Image Copyright FontAwesome.com
        */
        percent: "\uf295",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/percentage.svg.png) Percentage
         * Image Copyright FontAwesome.com
        */
        percentage: "\uf541",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/person-booth.svg.png) Person Entering Booth
         * Image Copyright FontAwesome.com
        */
        personBooth: "\uf756",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/phone.svg.png) Phone
         * Image Copyright FontAwesome.com
        */
        phone: "\uf095",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/phone-alt.svg.png) Alternate Phone
         * Image Copyright FontAwesome.com
        */
        phoneAlt: "\uf879",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/phone-slash.svg.png) Phone Slash
         * Image Copyright FontAwesome.com
        */
        phoneSlash: "\uf3dd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/phone-square.svg.png) Phone Square
         * Image Copyright FontAwesome.com
        */
        phoneSquare: "\uf098",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/phone-square-alt.svg.png) Alternate Phone Square
         * Image Copyright FontAwesome.com
        */
        phoneSquareAlt: "\uf87b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/phone-volume.svg.png) Phone Volume
         * Image Copyright FontAwesome.com
        */
        phoneVolume: "\uf2a0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/photo-video.svg.png) Photo Video
         * Image Copyright FontAwesome.com
        */
        photoVideo: "\uf87c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/piggy-bank.svg.png) Piggy Bank
         * Image Copyright FontAwesome.com
        */
        piggyBank: "\uf4d3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pills.svg.png) Pills
         * Image Copyright FontAwesome.com
        */
        pills: "\uf484",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pizza-slice.svg.png) Pizza Slice
         * Image Copyright FontAwesome.com
        */
        pizzaSlice: "\uf818",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/place-of-worship.svg.png) Place of Worship
         * Image Copyright FontAwesome.com
        */
        placeOfWorship: "\uf67f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plane.svg.png) plane
         * Image Copyright FontAwesome.com
        */
        plane: "\uf072",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plane-arrival.svg.png) Plane Arrival
         * Image Copyright FontAwesome.com
        */
        planeArrival: "\uf5af",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plane-departure.svg.png) Plane Departure
         * Image Copyright FontAwesome.com
        */
        planeDeparture: "\uf5b0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/play.svg.png) play
         * Image Copyright FontAwesome.com
        */
        play: "\uf04b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/play-circle.svg.png) Play Circle
         * Image Copyright FontAwesome.com
        */
        playCircle: "\uf144",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plug.svg.png) Plug
         * Image Copyright FontAwesome.com
        */
        plug: "\uf1e6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plus.svg.png) plus
         * Image Copyright FontAwesome.com
        */
        plus: "\uf067",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plus-circle.svg.png) Plus Circle
         * Image Copyright FontAwesome.com
        */
        plusCircle: "\uf055",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/plus-square.svg.png) Plus Square
         * Image Copyright FontAwesome.com
        */
        plusSquare: "\uf0fe",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/podcast.svg.png) Podcast
         * Image Copyright FontAwesome.com
        */
        podcast: "\uf2ce",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/poll.svg.png) Poll
         * Image Copyright FontAwesome.com
        */
        poll: "\uf681",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/poll-h.svg.png) Poll H
         * Image Copyright FontAwesome.com
        */
        pollH: "\uf682",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/poo.svg.png) Poo
         * Image Copyright FontAwesome.com
        */
        poo: "\uf2fe",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/poo-storm.svg.png) Poo Storm
         * Image Copyright FontAwesome.com
        */
        pooStorm: "\uf75a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/poop.svg.png) Poop
         * Image Copyright FontAwesome.com
        */
        poop: "\uf619",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/portrait.svg.png) Portrait
         * Image Copyright FontAwesome.com
        */
        portrait: "\uf3e0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pound-sign.svg.png) Pound Sign
         * Image Copyright FontAwesome.com
        */
        poundSign: "\uf154",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/power-off.svg.png) Power Off
         * Image Copyright FontAwesome.com
        */
        powerOff: "\uf011",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/pray.svg.png) Pray
         * Image Copyright FontAwesome.com
        */
        pray: "\uf683",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/praying-hands.svg.png) Praying Hands
         * Image Copyright FontAwesome.com
        */
        prayingHands: "\uf684",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/prescription.svg.png) Prescription
         * Image Copyright FontAwesome.com
        */
        prescription: "\uf5b1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/prescription-bottle.svg.png) Prescription Bottle
         * Image Copyright FontAwesome.com
        */
        prescriptionBottle: "\uf485",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/prescription-bottle-alt.svg.png) Alternate Prescription Bottle
         * Image Copyright FontAwesome.com
        */
        prescriptionBottleAlt: "\uf486",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/print.svg.png) print
         * Image Copyright FontAwesome.com
        */
        print: "\uf02f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/procedures.svg.png) Procedures
         * Image Copyright FontAwesome.com
        */
        procedures: "\uf487",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/project-diagram.svg.png) Project Diagram
         * Image Copyright FontAwesome.com
        */
        projectDiagram: "\uf542",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/puzzle-piece.svg.png) Puzzle Piece
         * Image Copyright FontAwesome.com
        */
        puzzlePiece: "\uf12e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/qrcode.svg.png) qrcode
         * Image Copyright FontAwesome.com
        */
        qrcode: "\uf029",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/question.svg.png) Question
         * Image Copyright FontAwesome.com
        */
        question: "\uf128",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/question-circle.svg.png) Question Circle
         * Image Copyright FontAwesome.com
        */
        questionCircle: "\uf059",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/quidditch.svg.png) Quidditch
         * Image Copyright FontAwesome.com
        */
        quidditch: "\uf458",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/quote-left.svg.png) quote-left
         * Image Copyright FontAwesome.com
        */
        quoteLeft: "\uf10d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/quote-right.svg.png) quote-right
         * Image Copyright FontAwesome.com
        */
        quoteRight: "\uf10e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/quran.svg.png) Quran
         * Image Copyright FontAwesome.com
        */
        quran: "\uf687",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/radiation.svg.png) Radiation
         * Image Copyright FontAwesome.com
        */
        radiation: "\uf7b9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/radiation-alt.svg.png) Alternate Radiation
         * Image Copyright FontAwesome.com
        */
        radiationAlt: "\uf7ba",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/rainbow.svg.png) Rainbow
         * Image Copyright FontAwesome.com
        */
        rainbow: "\uf75b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/random.svg.png) random
         * Image Copyright FontAwesome.com
        */
        random: "\uf074",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/receipt.svg.png) Receipt
         * Image Copyright FontAwesome.com
        */
        receipt: "\uf543",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/record-vinyl.svg.png) Record Vinyl
         * Image Copyright FontAwesome.com
        */
        recordVinyl: "\uf8d9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/recycle.svg.png) Recycle
         * Image Copyright FontAwesome.com
        */
        recycle: "\uf1b8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/redo.svg.png) Redo
         * Image Copyright FontAwesome.com
        */
        redo: "\uf01e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/redo-alt.svg.png) Alternate Redo
         * Image Copyright FontAwesome.com
        */
        redoAlt: "\uf2f9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/registered.svg.png) Registered Trademark
         * Image Copyright FontAwesome.com
        */
        registered: "\uf25d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/remove-format.svg.png) Remove Format
         * Image Copyright FontAwesome.com
        */
        removeFormat: "\uf87d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/reply.svg.png) Reply
         * Image Copyright FontAwesome.com
        */
        reply: "\uf3e5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/reply-all.svg.png) reply-all
         * Image Copyright FontAwesome.com
        */
        replyAll: "\uf122",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/republican.svg.png) Republican
         * Image Copyright FontAwesome.com
        */
        republican: "\uf75e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/restroom.svg.png) Restroom
         * Image Copyright FontAwesome.com
        */
        restroom: "\uf7bd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/retweet.svg.png) Retweet
         * Image Copyright FontAwesome.com
        */
        retweet: "\uf079",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ribbon.svg.png) Ribbon
         * Image Copyright FontAwesome.com
        */
        ribbon: "\uf4d6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ring.svg.png) Ring
         * Image Copyright FontAwesome.com
        */
        ring: "\uf70b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/road.svg.png) road
         * Image Copyright FontAwesome.com
        */
        road: "\uf018",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/robot.svg.png) Robot
         * Image Copyright FontAwesome.com
        */
        robot: "\uf544",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/rocket.svg.png) rocket
         * Image Copyright FontAwesome.com
        */
        rocket: "\uf135",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/route.svg.png) Route
         * Image Copyright FontAwesome.com
        */
        route: "\uf4d7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/rss.svg.png) rss
         * Image Copyright FontAwesome.com
        */
        rss: "\uf09e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/rss-square.svg.png) RSS Square
         * Image Copyright FontAwesome.com
        */
        rssSquare: "\uf143",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ruble-sign.svg.png) Ruble Sign
         * Image Copyright FontAwesome.com
        */
        rubleSign: "\uf158",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ruler.svg.png) Ruler
         * Image Copyright FontAwesome.com
        */
        ruler: "\uf545",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ruler-combined.svg.png) Ruler Combined
         * Image Copyright FontAwesome.com
        */
        rulerCombined: "\uf546",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ruler-horizontal.svg.png) Ruler Horizontal
         * Image Copyright FontAwesome.com
        */
        rulerHorizontal: "\uf547",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ruler-vertical.svg.png) Ruler Vertical
         * Image Copyright FontAwesome.com
        */
        rulerVertical: "\uf548",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/running.svg.png) Running
         * Image Copyright FontAwesome.com
        */
        running: "\uf70c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/rupee-sign.svg.png) Indian Rupee Sign
         * Image Copyright FontAwesome.com
        */
        rupeeSign: "\uf156",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sad-cry.svg.png) Crying Face
         * Image Copyright FontAwesome.com
        */
        sadCry: "\uf5b3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sad-tear.svg.png) Loudly Crying Face
         * Image Copyright FontAwesome.com
        */
        sadTear: "\uf5b4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/satellite.svg.png) Satellite
         * Image Copyright FontAwesome.com
        */
        satellite: "\uf7bf",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/satellite-dish.svg.png) Satellite Dish
         * Image Copyright FontAwesome.com
        */
        satelliteDish: "\uf7c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/save.svg.png) Save
         * Image Copyright FontAwesome.com
        */
        save: "\uf0c7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/school.svg.png) School
         * Image Copyright FontAwesome.com
        */
        school: "\uf549",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/screwdriver.svg.png) Screwdriver
         * Image Copyright FontAwesome.com
        */
        screwdriver: "\uf54a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/scroll.svg.png) Scroll
         * Image Copyright FontAwesome.com
        */
        scroll: "\uf70e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sd-card.svg.png) Sd Card
         * Image Copyright FontAwesome.com
        */
        sdCard: "\uf7c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/search.svg.png) Search
         * Image Copyright FontAwesome.com
        */
        search: "\uf002",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/search-dollar.svg.png) Search Dollar
         * Image Copyright FontAwesome.com
        */
        searchDollar: "\uf688",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/search-location.svg.png) Search Location
         * Image Copyright FontAwesome.com
        */
        searchLocation: "\uf689",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/search-minus.svg.png) Search Minus
         * Image Copyright FontAwesome.com
        */
        searchMinus: "\uf010",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/search-plus.svg.png) Search Plus
         * Image Copyright FontAwesome.com
        */
        searchPlus: "\uf00e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/seedling.svg.png) Seedling
         * Image Copyright FontAwesome.com
        */
        seedling: "\uf4d8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/server.svg.png) Server
         * Image Copyright FontAwesome.com
        */
        server: "\uf233",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shapes.svg.png) Shapes
         * Image Copyright FontAwesome.com
        */
        shapes: "\uf61f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/share.svg.png) Share
         * Image Copyright FontAwesome.com
        */
        share: "\uf064",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/share-alt.svg.png) Alternate Share
         * Image Copyright FontAwesome.com
        */
        shareAlt: "\uf1e0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/share-alt-square.svg.png) Alternate Share Square
         * Image Copyright FontAwesome.com
        */
        shareAltSquare: "\uf1e1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/share-square.svg.png) Share Square
         * Image Copyright FontAwesome.com
        */
        shareSquare: "\uf14d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shekel-sign.svg.png) Shekel Sign
         * Image Copyright FontAwesome.com
        */
        shekelSign: "\uf20b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shield-alt.svg.png) Alternate Shield
         * Image Copyright FontAwesome.com
        */
        shieldAlt: "\uf3ed",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ship.svg.png) Ship
         * Image Copyright FontAwesome.com
        */
        ship: "\uf21a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shipping-fast.svg.png) Shipping Fast
         * Image Copyright FontAwesome.com
        */
        shippingFast: "\uf48b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shoe-prints.svg.png) Shoe Prints
         * Image Copyright FontAwesome.com
        */
        shoePrints: "\uf54b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shopping-bag.svg.png) Shopping Bag
         * Image Copyright FontAwesome.com
        */
        shoppingBag: "\uf290",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shopping-basket.svg.png) Shopping Basket
         * Image Copyright FontAwesome.com
        */
        shoppingBasket: "\uf291",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shopping-cart.svg.png) shopping-cart
         * Image Copyright FontAwesome.com
        */
        shoppingCart: "\uf07a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shower.svg.png) Shower
         * Image Copyright FontAwesome.com
        */
        shower: "\uf2cc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/shuttle-van.svg.png) Shuttle Van
         * Image Copyright FontAwesome.com
        */
        shuttleVan: "\uf5b6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sign.svg.png) Sign
         * Image Copyright FontAwesome.com
        */
        sign: "\uf4d9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sign-in-alt.svg.png) Alternate Sign In
         * Image Copyright FontAwesome.com
        */
        signInAlt: "\uf2f6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sign-language.svg.png) Sign Language
         * Image Copyright FontAwesome.com
        */
        signLanguage: "\uf2a7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sign-out-alt.svg.png) Alternate Sign Out
         * Image Copyright FontAwesome.com
        */
        signOutAlt: "\uf2f5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/signal.svg.png) signal
         * Image Copyright FontAwesome.com
        */
        signal: "\uf012",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/signature.svg.png) Signature
         * Image Copyright FontAwesome.com
        */
        signature: "\uf5b7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sim-card.svg.png) SIM Card
         * Image Copyright FontAwesome.com
        */
        simCard: "\uf7c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sitemap.svg.png) Sitemap
         * Image Copyright FontAwesome.com
        */
        sitemap: "\uf0e8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/skating.svg.png) Skating
         * Image Copyright FontAwesome.com
        */
        skating: "\uf7c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/skiing.svg.png) Skiing
         * Image Copyright FontAwesome.com
        */
        skiing: "\uf7c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/skiing-nordic.svg.png) Skiing Nordic
         * Image Copyright FontAwesome.com
        */
        skiingNordic: "\uf7ca",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/skull.svg.png) Skull
         * Image Copyright FontAwesome.com
        */
        skull: "\uf54c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/skull-crossbones.svg.png) Skull & Crossbones
         * Image Copyright FontAwesome.com
        */
        skullCrossbones: "\uf714",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/slash.svg.png) Slash
         * Image Copyright FontAwesome.com
        */
        slash: "\uf715",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sleigh.svg.png) Sleigh
         * Image Copyright FontAwesome.com
        */
        sleigh: "\uf7cc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sliders-h.svg.png) Horizontal Sliders
         * Image Copyright FontAwesome.com
        */
        slidersH: "\uf1de",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/smile.svg.png) Smiling Face
         * Image Copyright FontAwesome.com
        */
        smile: "\uf118",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/smile-beam.svg.png) Beaming Face With Smiling Eyes
         * Image Copyright FontAwesome.com
        */
        smileBeam: "\uf5b8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/smile-wink.svg.png) Winking Face
         * Image Copyright FontAwesome.com
        */
        smileWink: "\uf4da",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/smog.svg.png) Smog
         * Image Copyright FontAwesome.com
        */
        smog: "\uf75f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/smoking.svg.png) Smoking
         * Image Copyright FontAwesome.com
        */
        smoking: "\uf48d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/smoking-ban.svg.png) Smoking Ban
         * Image Copyright FontAwesome.com
        */
        smokingBan: "\uf54d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sms.svg.png) SMS
         * Image Copyright FontAwesome.com
        */
        sms: "\uf7cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/snowboarding.svg.png) Snowboarding
         * Image Copyright FontAwesome.com
        */
        snowboarding: "\uf7ce",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/snowflake.svg.png) Snowflake
         * Image Copyright FontAwesome.com
        */
        snowflake: "\uf2dc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/snowman.svg.png) Snowman
         * Image Copyright FontAwesome.com
        */
        snowman: "\uf7d0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/snowplow.svg.png) Snowplow
         * Image Copyright FontAwesome.com
        */
        snowplow: "\uf7d2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/socks.svg.png) Socks
         * Image Copyright FontAwesome.com
        */
        socks: "\uf696",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/solar-panel.svg.png) Solar Panel
         * Image Copyright FontAwesome.com
        */
        solarPanel: "\uf5ba",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort.svg.png) Sort
         * Image Copyright FontAwesome.com
        */
        sort: "\uf0dc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-alpha-down.svg.png) Sort Alphabetical Down
         * Image Copyright FontAwesome.com
        */
        sortAlphaDown: "\uf15d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-alpha-down-alt.svg.png) Alternate Sort Alphabetical Down
         * Image Copyright FontAwesome.com
        */
        sortAlphaDownAlt: "\uf881",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-alpha-up.svg.png) Sort Alphabetical Up
         * Image Copyright FontAwesome.com
        */
        sortAlphaUp: "\uf15e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-alpha-up-alt.svg.png) Alternate Sort Alphabetical Up
         * Image Copyright FontAwesome.com
        */
        sortAlphaUpAlt: "\uf882",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-amount-down.svg.png) Sort Amount Down
         * Image Copyright FontAwesome.com
        */
        sortAmountDown: "\uf160",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-amount-down-alt.svg.png) Alternate Sort Amount Down
         * Image Copyright FontAwesome.com
        */
        sortAmountDownAlt: "\uf884",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-amount-up.svg.png) Sort Amount Up
         * Image Copyright FontAwesome.com
        */
        sortAmountUp: "\uf161",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-amount-up-alt.svg.png) Alternate Sort Amount Up
         * Image Copyright FontAwesome.com
        */
        sortAmountUpAlt: "\uf885",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-down.svg.png) Sort Down (Descending)
         * Image Copyright FontAwesome.com
        */
        sortDown: "\uf0dd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-numeric-down.svg.png) Sort Numeric Down
         * Image Copyright FontAwesome.com
        */
        sortNumericDown: "\uf162",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-numeric-down-alt.svg.png) Alternate Sort Numeric Down
         * Image Copyright FontAwesome.com
        */
        sortNumericDownAlt: "\uf886",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-numeric-up.svg.png) Sort Numeric Up
         * Image Copyright FontAwesome.com
        */
        sortNumericUp: "\uf163",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-numeric-up-alt.svg.png) Alternate Sort Numeric Up
         * Image Copyright FontAwesome.com
        */
        sortNumericUpAlt: "\uf887",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sort-up.svg.png) Sort Up (Ascending)
         * Image Copyright FontAwesome.com
        */
        sortUp: "\uf0de",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/spa.svg.png) Spa
         * Image Copyright FontAwesome.com
        */
        spa: "\uf5bb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/space-shuttle.svg.png) Space Shuttle
         * Image Copyright FontAwesome.com
        */
        spaceShuttle: "\uf197",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/spell-check.svg.png) Spell Check
         * Image Copyright FontAwesome.com
        */
        spellCheck: "\uf891",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/spider.svg.png) Spider
         * Image Copyright FontAwesome.com
        */
        spider: "\uf717",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/spinner.svg.png) Spinner
         * Image Copyright FontAwesome.com
        */
        spinner: "\uf110",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/splotch.svg.png) Splotch
         * Image Copyright FontAwesome.com
        */
        splotch: "\uf5bc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/spray-can.svg.png) Spray Can
         * Image Copyright FontAwesome.com
        */
        sprayCan: "\uf5bd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/square.svg.png) Square
         * Image Copyright FontAwesome.com
        */
        square: "\uf0c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/square-full.svg.png) Square Full
         * Image Copyright FontAwesome.com
        */
        squareFull: "\uf45c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/square-root-alt.svg.png) Alternate Square Root
         * Image Copyright FontAwesome.com
        */
        squareRootAlt: "\uf698",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stamp.svg.png) Stamp
         * Image Copyright FontAwesome.com
        */
        stamp: "\uf5bf",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/star.svg.png) Star
         * Image Copyright FontAwesome.com
        */
        star: "\uf005",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/star-and-crescent.svg.png) Star and Crescent
         * Image Copyright FontAwesome.com
        */
        starAndCrescent: "\uf699",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/star-half.svg.png) star-half
         * Image Copyright FontAwesome.com
        */
        starHalf: "\uf089",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/star-half-alt.svg.png) Alternate Star Half
         * Image Copyright FontAwesome.com
        */
        starHalfAlt: "\uf5c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/star-of-david.svg.png) Star of David
         * Image Copyright FontAwesome.com
        */
        starOfDavid: "\uf69a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/star-of-life.svg.png) Star of Life
         * Image Copyright FontAwesome.com
        */
        starOfLife: "\uf621",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/step-backward.svg.png) step-backward
         * Image Copyright FontAwesome.com
        */
        stepBackward: "\uf048",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/step-forward.svg.png) step-forward
         * Image Copyright FontAwesome.com
        */
        stepForward: "\uf051",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stethoscope.svg.png) Stethoscope
         * Image Copyright FontAwesome.com
        */
        stethoscope: "\uf0f1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sticky-note.svg.png) Sticky Note
         * Image Copyright FontAwesome.com
        */
        stickyNote: "\uf249",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stop.svg.png) stop
         * Image Copyright FontAwesome.com
        */
        stop: "\uf04d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stop-circle.svg.png) Stop Circle
         * Image Copyright FontAwesome.com
        */
        stopCircle: "\uf28d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stopwatch.svg.png) Stopwatch
         * Image Copyright FontAwesome.com
        */
        stopwatch: "\uf2f2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/store.svg.png) Store
         * Image Copyright FontAwesome.com
        */
        store: "\uf54e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/store-alt.svg.png) Alternate Store
         * Image Copyright FontAwesome.com
        */
        storeAlt: "\uf54f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stream.svg.png) Stream
         * Image Copyright FontAwesome.com
        */
        stream: "\uf550",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/street-view.svg.png) Street View
         * Image Copyright FontAwesome.com
        */
        streetView: "\uf21d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/strikethrough.svg.png) Strikethrough
         * Image Copyright FontAwesome.com
        */
        strikethrough: "\uf0cc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/stroopwafel.svg.png) Stroopwafel
         * Image Copyright FontAwesome.com
        */
        stroopwafel: "\uf551",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/subscript.svg.png) subscript
         * Image Copyright FontAwesome.com
        */
        subscript: "\uf12c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/subway.svg.png) Subway
         * Image Copyright FontAwesome.com
        */
        subway: "\uf239",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/suitcase.svg.png) Suitcase
         * Image Copyright FontAwesome.com
        */
        suitcase: "\uf0f2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/suitcase-rolling.svg.png) Suitcase Rolling
         * Image Copyright FontAwesome.com
        */
        suitcaseRolling: "\uf5c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sun.svg.png) Sun
         * Image Copyright FontAwesome.com
        */
        sun: "\uf185",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/superscript.svg.png) superscript
         * Image Copyright FontAwesome.com
        */
        superscript: "\uf12b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/surprise.svg.png) Hushed Face
         * Image Copyright FontAwesome.com
        */
        surprise: "\uf5c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/swatchbook.svg.png) Swatchbook
         * Image Copyright FontAwesome.com
        */
        swatchbook: "\uf5c3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/swimmer.svg.png) Swimmer
         * Image Copyright FontAwesome.com
        */
        swimmer: "\uf5c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/swimming-pool.svg.png) Swimming Pool
         * Image Copyright FontAwesome.com
        */
        swimmingPool: "\uf5c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/synagogue.svg.png) Synagogue
         * Image Copyright FontAwesome.com
        */
        synagogue: "\uf69b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sync.svg.png) Sync
         * Image Copyright FontAwesome.com
        */
        sync: "\uf021",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/sync-alt.svg.png) Alternate Sync
         * Image Copyright FontAwesome.com
        */
        syncAlt: "\uf2f1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/syringe.svg.png) Syringe
         * Image Copyright FontAwesome.com
        */
        syringe: "\uf48e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/table.svg.png) table
         * Image Copyright FontAwesome.com
        */
        table: "\uf0ce",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/table-tennis.svg.png) Table Tennis
         * Image Copyright FontAwesome.com
        */
        tableTennis: "\uf45d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tablet.svg.png) tablet
         * Image Copyright FontAwesome.com
        */
        tablet: "\uf10a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tablet-alt.svg.png) Alternate Tablet
         * Image Copyright FontAwesome.com
        */
        tabletAlt: "\uf3fa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tablets.svg.png) Tablets
         * Image Copyright FontAwesome.com
        */
        tablets: "\uf490",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tachometer-alt.svg.png) Alternate Tachometer
         * Image Copyright FontAwesome.com
        */
        tachometerAlt: "\uf3fd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tag.svg.png) tag
         * Image Copyright FontAwesome.com
        */
        tag: "\uf02b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tags.svg.png) tags
         * Image Copyright FontAwesome.com
        */
        tags: "\uf02c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tape.svg.png) Tape
         * Image Copyright FontAwesome.com
        */
        tape: "\uf4db",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tasks.svg.png) Tasks
         * Image Copyright FontAwesome.com
        */
        tasks: "\uf0ae",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/taxi.svg.png) Taxi
         * Image Copyright FontAwesome.com
        */
        taxi: "\uf1ba",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/teeth.svg.png) Teeth
         * Image Copyright FontAwesome.com
        */
        teeth: "\uf62e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/teeth-open.svg.png) Teeth Open
         * Image Copyright FontAwesome.com
        */
        teethOpen: "\uf62f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/temperature-high.svg.png) High Temperature
         * Image Copyright FontAwesome.com
        */
        temperatureHigh: "\uf769",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/temperature-low.svg.png) Low Temperature
         * Image Copyright FontAwesome.com
        */
        temperatureLow: "\uf76b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tenge.svg.png) Tenge
         * Image Copyright FontAwesome.com
        */
        tenge: "\uf7d7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/terminal.svg.png) Terminal
         * Image Copyright FontAwesome.com
        */
        terminal: "\uf120",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/text-height.svg.png) text-height
         * Image Copyright FontAwesome.com
        */
        textHeight: "\uf034",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/text-width.svg.png) Text Width
         * Image Copyright FontAwesome.com
        */
        textWidth: "\uf035",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/th.svg.png) th
         * Image Copyright FontAwesome.com
        */
        th: "\uf00a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/th-large.svg.png) th-large
         * Image Copyright FontAwesome.com
        */
        thLarge: "\uf009",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/th-list.svg.png) th-list
         * Image Copyright FontAwesome.com
        */
        thList: "\uf00b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/theater-masks.svg.png) Theater Masks
         * Image Copyright FontAwesome.com
        */
        theaterMasks: "\uf630",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thermometer.svg.png) Thermometer
         * Image Copyright FontAwesome.com
        */
        thermometer: "\uf491",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thermometer-empty.svg.png) Thermometer Empty
         * Image Copyright FontAwesome.com
        */
        thermometerEmpty: "\uf2cb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thermometer-full.svg.png) Thermometer Full
         * Image Copyright FontAwesome.com
        */
        thermometerFull: "\uf2c7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thermometer-half.svg.png) Thermometer 1/2 Full
         * Image Copyright FontAwesome.com
        */
        thermometerHalf: "\uf2c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thermometer-quarter.svg.png) Thermometer 1/4 Full
         * Image Copyright FontAwesome.com
        */
        thermometerQuarter: "\uf2ca",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thermometer-three-quarters.svg.png) Thermometer 3/4 Full
         * Image Copyright FontAwesome.com
        */
        thermometerThreeQuarters: "\uf2c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thumbs-down.svg.png) thumbs-down
         * Image Copyright FontAwesome.com
        */
        thumbsDown: "\uf165",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thumbs-up.svg.png) thumbs-up
         * Image Copyright FontAwesome.com
        */
        thumbsUp: "\uf164",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/thumbtack.svg.png) Thumbtack
         * Image Copyright FontAwesome.com
        */
        thumbtack: "\uf08d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/ticket-alt.svg.png) Alternate Ticket
         * Image Copyright FontAwesome.com
        */
        ticketAlt: "\uf3ff",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/times.svg.png) Times
         * Image Copyright FontAwesome.com
        */
        times: "\uf00d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/times-circle.svg.png) Times Circle
         * Image Copyright FontAwesome.com
        */
        timesCircle: "\uf057",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tint.svg.png) tint
         * Image Copyright FontAwesome.com
        */
        tint: "\uf043",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tint-slash.svg.png) Tint Slash
         * Image Copyright FontAwesome.com
        */
        tintSlash: "\uf5c7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tired.svg.png) Tired Face
         * Image Copyright FontAwesome.com
        */
        tired: "\uf5c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/toggle-off.svg.png) Toggle Off
         * Image Copyright FontAwesome.com
        */
        toggleOff: "\uf204",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/toggle-on.svg.png) Toggle On
         * Image Copyright FontAwesome.com
        */
        toggleOn: "\uf205",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/toilet.svg.png) Toilet
         * Image Copyright FontAwesome.com
        */
        toilet: "\uf7d8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/toilet-paper.svg.png) Toilet Paper
         * Image Copyright FontAwesome.com
        */
        toiletPaper: "\uf71e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/toolbox.svg.png) Toolbox
         * Image Copyright FontAwesome.com
        */
        toolbox: "\uf552",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tools.svg.png) Tools
         * Image Copyright FontAwesome.com
        */
        tools: "\uf7d9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tooth.svg.png) Tooth
         * Image Copyright FontAwesome.com
        */
        tooth: "\uf5c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/torah.svg.png) Torah
         * Image Copyright FontAwesome.com
        */
        torah: "\uf6a0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/torii-gate.svg.png) Torii Gate
         * Image Copyright FontAwesome.com
        */
        toriiGate: "\uf6a1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tractor.svg.png) Tractor
         * Image Copyright FontAwesome.com
        */
        tractor: "\uf722",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trademark.svg.png) Trademark
         * Image Copyright FontAwesome.com
        */
        trademark: "\uf25c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/traffic-light.svg.png) Traffic Light
         * Image Copyright FontAwesome.com
        */
        trafficLight: "\uf637",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trailer.svg.png) Trailer
         * Image Copyright FontAwesome.com
        */
        trailer: "\uf941",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/train.svg.png) Train
         * Image Copyright FontAwesome.com
        */
        train: "\uf238",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tram.svg.png) Tram
         * Image Copyright FontAwesome.com
        */
        tram: "\uf7da",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/transgender.svg.png) Transgender
         * Image Copyright FontAwesome.com
        */
        transgender: "\uf224",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/transgender-alt.svg.png) Alternate Transgender
         * Image Copyright FontAwesome.com
        */
        transgenderAlt: "\uf225",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trash.svg.png) Trash
         * Image Copyright FontAwesome.com
        */
        trash: "\uf1f8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trash-alt.svg.png) Alternate Trash
         * Image Copyright FontAwesome.com
        */
        trashAlt: "\uf2ed",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trash-restore.svg.png) Trash Restore
         * Image Copyright FontAwesome.com
        */
        trashRestore: "\uf829",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trash-restore-alt.svg.png) Alternative Trash Restore
         * Image Copyright FontAwesome.com
        */
        trashRestoreAlt: "\uf82a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tree.svg.png) Tree
         * Image Copyright FontAwesome.com
        */
        tree: "\uf1bb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/trophy.svg.png) trophy
         * Image Copyright FontAwesome.com
        */
        trophy: "\uf091",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/truck.svg.png) truck
         * Image Copyright FontAwesome.com
        */
        truck: "\uf0d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/truck-loading.svg.png) Truck Loading
         * Image Copyright FontAwesome.com
        */
        truckLoading: "\uf4de",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/truck-monster.svg.png) Truck Monster
         * Image Copyright FontAwesome.com
        */
        truckMonster: "\uf63b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/truck-moving.svg.png) Truck Moving
         * Image Copyright FontAwesome.com
        */
        truckMoving: "\uf4df",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/truck-pickup.svg.png) Truck Side
         * Image Copyright FontAwesome.com
        */
        truckPickup: "\uf63c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tshirt.svg.png) T-Shirt
         * Image Copyright FontAwesome.com
        */
        tshirt: "\uf553",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tty.svg.png) TTY
         * Image Copyright FontAwesome.com
        */
        tty: "\uf1e4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/tv.svg.png) Television
         * Image Copyright FontAwesome.com
        */
        tv: "\uf26c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/umbrella.svg.png) Umbrella
         * Image Copyright FontAwesome.com
        */
        umbrella: "\uf0e9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/umbrella-beach.svg.png) Umbrella Beach
         * Image Copyright FontAwesome.com
        */
        umbrellaBeach: "\uf5ca",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/underline.svg.png) Underline
         * Image Copyright FontAwesome.com
        */
        underline: "\uf0cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/undo.svg.png) Undo
         * Image Copyright FontAwesome.com
        */
        undo: "\uf0e2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/undo-alt.svg.png) Alternate Undo
         * Image Copyright FontAwesome.com
        */
        undoAlt: "\uf2ea",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/universal-access.svg.png) Universal Access
         * Image Copyright FontAwesome.com
        */
        universalAccess: "\uf29a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/university.svg.png) University
         * Image Copyright FontAwesome.com
        */
        university: "\uf19c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/unlink.svg.png) unlink
         * Image Copyright FontAwesome.com
        */
        unlink: "\uf127",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/unlock.svg.png) unlock
         * Image Copyright FontAwesome.com
        */
        unlock: "\uf09c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/unlock-alt.svg.png) Alternate Unlock
         * Image Copyright FontAwesome.com
        */
        unlockAlt: "\uf13e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/upload.svg.png) Upload
         * Image Copyright FontAwesome.com
        */
        upload: "\uf093",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user.svg.png) User
         * Image Copyright FontAwesome.com
        */
        user: "\uf007",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-alt.svg.png) Alternate User
         * Image Copyright FontAwesome.com
        */
        userAlt: "\uf406",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-alt-slash.svg.png) Alternate User Slash
         * Image Copyright FontAwesome.com
        */
        userAltSlash: "\uf4fa",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-astronaut.svg.png) User Astronaut
         * Image Copyright FontAwesome.com
        */
        userAstronaut: "\uf4fb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-check.svg.png) User Check
         * Image Copyright FontAwesome.com
        */
        userCheck: "\uf4fc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-circle.svg.png) User Circle
         * Image Copyright FontAwesome.com
        */
        userCircle: "\uf2bd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-clock.svg.png) User Clock
         * Image Copyright FontAwesome.com
        */
        userClock: "\uf4fd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-cog.svg.png) User Cog
         * Image Copyright FontAwesome.com
        */
        userCog: "\uf4fe",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-edit.svg.png) User Edit
         * Image Copyright FontAwesome.com
        */
        userEdit: "\uf4ff",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-friends.svg.png) User Friends
         * Image Copyright FontAwesome.com
        */
        userFriends: "\uf500",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-graduate.svg.png) User Graduate
         * Image Copyright FontAwesome.com
        */
        userGraduate: "\uf501",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-injured.svg.png) User Injured
         * Image Copyright FontAwesome.com
        */
        userInjured: "\uf728",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-lock.svg.png) User Lock
         * Image Copyright FontAwesome.com
        */
        userLock: "\uf502",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-md.svg.png) Doctor
         * Image Copyright FontAwesome.com
        */
        userMd: "\uf0f0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-minus.svg.png) User Minus
         * Image Copyright FontAwesome.com
        */
        userMinus: "\uf503",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-ninja.svg.png) User Ninja
         * Image Copyright FontAwesome.com
        */
        userNinja: "\uf504",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-nurse.svg.png) Nurse
         * Image Copyright FontAwesome.com
        */
        userNurse: "\uf82f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-plus.svg.png) User Plus
         * Image Copyright FontAwesome.com
        */
        userPlus: "\uf234",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-secret.svg.png) User Secret
         * Image Copyright FontAwesome.com
        */
        userSecret: "\uf21b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-shield.svg.png) User Shield
         * Image Copyright FontAwesome.com
        */
        userShield: "\uf505",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-slash.svg.png) User Slash
         * Image Copyright FontAwesome.com
        */
        userSlash: "\uf506",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-tag.svg.png) User Tag
         * Image Copyright FontAwesome.com
        */
        userTag: "\uf507",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-tie.svg.png) User Tie
         * Image Copyright FontAwesome.com
        */
        userTie: "\uf508",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/user-times.svg.png) Remove User
         * Image Copyright FontAwesome.com
        */
        userTimes: "\uf235",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/users.svg.png) Users
         * Image Copyright FontAwesome.com
        */
        users: "\uf0c0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/users-cog.svg.png) Users Cog
         * Image Copyright FontAwesome.com
        */
        usersCog: "\uf509",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/utensil-spoon.svg.png) Utensil Spoon
         * Image Copyright FontAwesome.com
        */
        utensilSpoon: "\uf2e5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/utensils.svg.png) Utensils
         * Image Copyright FontAwesome.com
        */
        utensils: "\uf2e7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/vector-square.svg.png) Vector Square
         * Image Copyright FontAwesome.com
        */
        vectorSquare: "\uf5cb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/venus.svg.png) Venus
         * Image Copyright FontAwesome.com
        */
        venus: "\uf221",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/venus-double.svg.png) Venus Double
         * Image Copyright FontAwesome.com
        */
        venusDouble: "\uf226",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/venus-mars.svg.png) Venus Mars
         * Image Copyright FontAwesome.com
        */
        venusMars: "\uf228",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/vial.svg.png) Vial
         * Image Copyright FontAwesome.com
        */
        vial: "\uf492",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/vials.svg.png) Vials
         * Image Copyright FontAwesome.com
        */
        vials: "\uf493",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/video.svg.png) Video
         * Image Copyright FontAwesome.com
        */
        video: "\uf03d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/video-slash.svg.png) Video Slash
         * Image Copyright FontAwesome.com
        */
        videoSlash: "\uf4e2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/vihara.svg.png) Vihara
         * Image Copyright FontAwesome.com
        */
        vihara: "\uf6a7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/voicemail.svg.png) Voicemail
         * Image Copyright FontAwesome.com
        */
        voicemail: "\uf897",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/volleyball-ball.svg.png) Volleyball Ball
         * Image Copyright FontAwesome.com
        */
        volleyballBall: "\uf45f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/volume-down.svg.png) Volume Down
         * Image Copyright FontAwesome.com
        */
        volumeDown: "\uf027",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/volume-mute.svg.png) Volume Mute
         * Image Copyright FontAwesome.com
        */
        volumeMute: "\uf6a9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/volume-off.svg.png) Volume Off
         * Image Copyright FontAwesome.com
        */
        volumeOff: "\uf026",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/volume-up.svg.png) Volume Up
         * Image Copyright FontAwesome.com
        */
        volumeUp: "\uf028",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/vote-yea.svg.png) Vote Yea
         * Image Copyright FontAwesome.com
        */
        voteYea: "\uf772",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/vr-cardboard.svg.png) Cardboard VR
         * Image Copyright FontAwesome.com
        */
        vrCardboard: "\uf729",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/walking.svg.png) Walking
         * Image Copyright FontAwesome.com
        */
        walking: "\uf554",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wallet.svg.png) Wallet
         * Image Copyright FontAwesome.com
        */
        wallet: "\uf555",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/warehouse.svg.png) Warehouse
         * Image Copyright FontAwesome.com
        */
        warehouse: "\uf494",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/water.svg.png) Water
         * Image Copyright FontAwesome.com
        */
        water: "\uf773",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wave-square.svg.png) Square Wave
         * Image Copyright FontAwesome.com
        */
        waveSquare: "\uf83e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/weight.svg.png) Weight
         * Image Copyright FontAwesome.com
        */
        weight: "\uf496",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/weight-hanging.svg.png) Hanging Weight
         * Image Copyright FontAwesome.com
        */
        weightHanging: "\uf5cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wheelchair.svg.png) Wheelchair
         * Image Copyright FontAwesome.com
        */
        wheelchair: "\uf193",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wifi.svg.png) WiFi
         * Image Copyright FontAwesome.com
        */
        wifi: "\uf1eb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wind.svg.png) Wind
         * Image Copyright FontAwesome.com
        */
        wind: "\uf72e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/window-close.svg.png) Window Close
         * Image Copyright FontAwesome.com
        */
        windowClose: "\uf410",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/window-maximize.svg.png) Window Maximize
         * Image Copyright FontAwesome.com
        */
        windowMaximize: "\uf2d0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/window-minimize.svg.png) Window Minimize
         * Image Copyright FontAwesome.com
        */
        windowMinimize: "\uf2d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/window-restore.svg.png) Window Restore
         * Image Copyright FontAwesome.com
        */
        windowRestore: "\uf2d2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wine-bottle.svg.png) Wine Bottle
         * Image Copyright FontAwesome.com
        */
        wineBottle: "\uf72f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wine-glass.svg.png) Wine Glass
         * Image Copyright FontAwesome.com
        */
        wineGlass: "\uf4e3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wine-glass-alt.svg.png) Alternate Wine Glas
         * Image Copyright FontAwesome.com
        */
        wineGlassAlt: "\uf5ce",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/won-sign.svg.png) Won Sign
         * Image Copyright FontAwesome.com
        */
        wonSign: "\uf159",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/wrench.svg.png) Wrench
         * Image Copyright FontAwesome.com
        */
        wrench: "\uf0ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/x-ray.svg.png) X-Ray
         * Image Copyright FontAwesome.com
        */
        xRay: "\uf497",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/yen-sign.svg.png) Yen Sign
         * Image Copyright FontAwesome.com
        */
        yenSign: "\uf157",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/solid/yin-yang.svg.png) Yin Yang
         * Image Copyright FontAwesome.com
        */
        yinYang: "\uf6ad",
        toString: function () {
            var name = _this._fontName;
            if (name) {
                return name;
            }
            var p = bridge.platform;
            if (p) {
                if (/android/i.test(p)) {
                    name = "Font Awesome 5 Free-Solid-900.otf#Font Awesome 5 Free Solid";
                }
                else if (/ios/i.test(p)) {
                    name = "FontAwesome5Free-Solid";
                }
            }
            else {
                name = "Font Awesome 5 Free";
            }
            _this._fontName = name;
            return name;
        }
    };
    exports.default = FontAwesomeSolid;
});
//# sourceMappingURL=FontAwesomeSolid.js.map

    AmdLoader.instance.setup("@web-atoms/font-awesome/dist/FontAwesomeSolid");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    //tslint:disable
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF = { get BindableObject() { return this._BindableObject || (this._BindableObject = bridge.getClass('Xamarin.Forms.BindableObject, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Element() { return this._Element || (this._Element = bridge.getClass('Xamarin.Forms.Element, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get NavigableElement() { return this._NavigableElement || (this._NavigableElement = bridge.getClass('Xamarin.Forms.NavigableElement, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get VisualElement() { return this._VisualElement || (this._VisualElement = bridge.getClass('Xamarin.Forms.VisualElement, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get View() { return this._View || (this._View = bridge.getClass('Xamarin.Forms.View, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Layout() { return this._Layout || (this._Layout = bridge.getClass('Xamarin.Forms.Layout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get AbsoluteLayout() { return this._AbsoluteLayout || (this._AbsoluteLayout = bridge.getClass('Xamarin.Forms.AbsoluteLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ActivityIndicator() { return this._ActivityIndicator || (this._ActivityIndicator = bridge.getClass('Xamarin.Forms.ActivityIndicator, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get StateTriggerBase() { return this._StateTriggerBase || (this._StateTriggerBase = bridge.getClass('Xamarin.Forms.StateTriggerBase, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get AdaptiveTrigger() { return this._AdaptiveTrigger || (this._AdaptiveTrigger = bridge.getClass('Xamarin.Forms.AdaptiveTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Application() { return this._Application || (this._Application = bridge.getClass('Xamarin.Forms.Application, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get AppLinkEntry() { return this._AppLinkEntry || (this._AppLinkEntry = bridge.getClass('Xamarin.Forms.AppLinkEntry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get BaseMenuItem() { return this._BaseMenuItem || (this._BaseMenuItem = bridge.getClass('Xamarin.Forms.BaseMenuItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get BindableLayout() { return this._BindableLayout || (this._BindableLayout = bridge.getClass('Xamarin.Forms.BindableLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get BoxView() { return this._BoxView || (this._BoxView = bridge.getClass('Xamarin.Forms.BoxView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Brush() { return this._Brush || (this._Brush = bridge.getClass('Xamarin.Forms.Brush, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Button() { return this._Button || (this._Button = bridge.getClass('Xamarin.Forms.Button, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Page() { return this._Page || (this._Page = bridge.getClass('Xamarin.Forms.Page, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CarouselPage() { return this._CarouselPage || (this._CarouselPage = bridge.getClass('Xamarin.Forms.CarouselPage, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Cell() { return this._Cell || (this._Cell = bridge.getClass('Xamarin.Forms.Cell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get EntryCell() { return this._EntryCell || (this._EntryCell = bridge.getClass('Xamarin.Forms.EntryCell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TextCell() { return this._TextCell || (this._TextCell = bridge.getClass('Xamarin.Forms.TextCell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ImageCell() { return this._ImageCell || (this._ImageCell = bridge.getClass('Xamarin.Forms.ImageCell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SwitchCell() { return this._SwitchCell || (this._SwitchCell = bridge.getClass('Xamarin.Forms.SwitchCell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ViewCell() { return this._ViewCell || (this._ViewCell = bridge.getClass('Xamarin.Forms.ViewCell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CheckBox() { return this._CheckBox || (this._CheckBox = bridge.getClass('Xamarin.Forms.CheckBox, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GestureRecognizer() { return this._GestureRecognizer || (this._GestureRecognizer = bridge.getClass('Xamarin.Forms.GestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ClickGestureRecognizer() { return this._ClickGestureRecognizer || (this._ClickGestureRecognizer = bridge.getClass('Xamarin.Forms.ClickGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ColumnDefinition() { return this._ColumnDefinition || (this._ColumnDefinition = bridge.getClass('Xamarin.Forms.ColumnDefinition, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CompareStateTrigger() { return this._CompareStateTrigger || (this._CompareStateTrigger = bridge.getClass('Xamarin.Forms.CompareStateTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CompressedLayout() { return this._CompressedLayout || (this._CompressedLayout = bridge.getClass('Xamarin.Forms.CompressedLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TemplatedPage() { return this._TemplatedPage || (this._TemplatedPage = bridge.getClass('Xamarin.Forms.TemplatedPage, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ContentPage() { return this._ContentPage || (this._ContentPage = bridge.getClass('Xamarin.Forms.ContentPage, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ContentPresenter() { return this._ContentPresenter || (this._ContentPresenter = bridge.getClass('Xamarin.Forms.ContentPresenter, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TemplatedView() { return this._TemplatedView || (this._TemplatedView = bridge.getClass('Xamarin.Forms.TemplatedView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ContentView() { return this._ContentView || (this._ContentView = bridge.getClass('Xamarin.Forms.ContentView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ControlTemplate() { return this._ControlTemplate || (this._ControlTemplate = bridge.getClass('Xamarin.Forms.ControlTemplate, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DataTemplate() { return this._DataTemplate || (this._DataTemplate = bridge.getClass('Xamarin.Forms.DataTemplate, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DataTemplateSelector() { return this._DataTemplateSelector || (this._DataTemplateSelector = bridge.getClass('Xamarin.Forms.DataTemplateSelector, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DatePicker() { return this._DatePicker || (this._DatePicker = bridge.getClass('Xamarin.Forms.DatePicker, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DeviceStateTrigger() { return this._DeviceStateTrigger || (this._DeviceStateTrigger = bridge.getClass('Xamarin.Forms.DeviceStateTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DragGestureRecognizer() { return this._DragGestureRecognizer || (this._DragGestureRecognizer = bridge.getClass('Xamarin.Forms.DragGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DropGestureRecognizer() { return this._DropGestureRecognizer || (this._DropGestureRecognizer = bridge.getClass('Xamarin.Forms.DropGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get InputView() { return this._InputView || (this._InputView = bridge.getClass('Xamarin.Forms.InputView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Editor() { return this._Editor || (this._Editor = bridge.getClass('Xamarin.Forms.Editor, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Entry() { return this._Entry || (this._Entry = bridge.getClass('Xamarin.Forms.Entry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Expander() { return this._Expander || (this._Expander = bridge.getClass('Xamarin.Forms.Expander, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ImageSource() { return this._ImageSource || (this._ImageSource = bridge.getClass('Xamarin.Forms.ImageSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get FileImageSource() { return this._FileImageSource || (this._FileImageSource = bridge.getClass('Xamarin.Forms.FileImageSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get MediaSource() { return this._MediaSource || (this._MediaSource = bridge.getClass('Xamarin.Forms.MediaSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get FileMediaSource() { return this._FileMediaSource || (this._FileMediaSource = bridge.getClass('Xamarin.Forms.FileMediaSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get FlexLayout() { return this._FlexLayout || (this._FlexLayout = bridge.getClass('Xamarin.Forms.FlexLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get FontImageSource() { return this._FontImageSource || (this._FontImageSource = bridge.getClass('Xamarin.Forms.FontImageSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get FormattedString() { return this._FormattedString || (this._FormattedString = bridge.getClass('Xamarin.Forms.FormattedString, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Frame() { return this._Frame || (this._Frame = bridge.getClass('Xamarin.Forms.Frame, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GestureElement() { return this._GestureElement || (this._GestureElement = bridge.getClass('Xamarin.Forms.GestureElement, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GradientBrush() { return this._GradientBrush || (this._GradientBrush = bridge.getClass('Xamarin.Forms.GradientBrush, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GradientStop() { return this._GradientStop || (this._GradientStop = bridge.getClass('Xamarin.Forms.GradientStop, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Grid() { return this._Grid || (this._Grid = bridge.getClass('Xamarin.Forms.Grid, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get WebViewSource() { return this._WebViewSource || (this._WebViewSource = bridge.getClass('Xamarin.Forms.WebViewSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get HtmlWebViewSource() { return this._HtmlWebViewSource || (this._HtmlWebViewSource = bridge.getClass('Xamarin.Forms.HtmlWebViewSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Image() { return this._Image || (this._Image = bridge.getClass('Xamarin.Forms.Image, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ImageButton() { return this._ImageButton || (this._ImageButton = bridge.getClass('Xamarin.Forms.ImageButton, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get IndicatorView() { return this._IndicatorView || (this._IndicatorView = bridge.getClass('Xamarin.Forms.IndicatorView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Behavior() { return this._Behavior || (this._Behavior = bridge.getClass('Xamarin.Forms.Behavior, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TriggerBase() { return this._TriggerBase || (this._TriggerBase = bridge.getClass('Xamarin.Forms.TriggerBase, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get DataTrigger() { return this._DataTrigger || (this._DataTrigger = bridge.getClass('Xamarin.Forms.DataTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get EventTrigger() { return this._EventTrigger || (this._EventTrigger = bridge.getClass('Xamarin.Forms.EventTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get MultiTrigger() { return this._MultiTrigger || (this._MultiTrigger = bridge.getClass('Xamarin.Forms.MultiTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Trigger() { return this._Trigger || (this._Trigger = bridge.getClass('Xamarin.Forms.Trigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ItemsView() { return this._ItemsView || (this._ItemsView = bridge.getClass('Xamarin.Forms.ItemsView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CarouselView() { return this._CarouselView || (this._CarouselView = bridge.getClass('Xamarin.Forms.CarouselView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get StructuredItemsView() { return this._StructuredItemsView || (this._StructuredItemsView = bridge.getClass('Xamarin.Forms.StructuredItemsView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SelectableItemsView() { return this._SelectableItemsView || (this._SelectableItemsView = bridge.getClass('Xamarin.Forms.SelectableItemsView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GroupableItemsView() { return this._GroupableItemsView || (this._GroupableItemsView = bridge.getClass('Xamarin.Forms.GroupableItemsView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CollectionView() { return this._CollectionView || (this._CollectionView = bridge.getClass('Xamarin.Forms.CollectionView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ItemsLayout() { return this._ItemsLayout || (this._ItemsLayout = bridge.getClass('Xamarin.Forms.ItemsLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GridItemsLayout() { return this._GridItemsLayout || (this._GridItemsLayout = bridge.getClass('Xamarin.Forms.GridItemsLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get LinearItemsLayout() { return this._LinearItemsLayout || (this._LinearItemsLayout = bridge.getClass('Xamarin.Forms.LinearItemsLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Label() { return this._Label || (this._Label = bridge.getClass('Xamarin.Forms.Label, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get LinearGradientBrush() { return this._LinearGradientBrush || (this._LinearGradientBrush = bridge.getClass('Xamarin.Forms.LinearGradientBrush, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ListView() { return this._ListView || (this._ListView = bridge.getClass('Xamarin.Forms.ListView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get MasterDetailPage() { return this._MasterDetailPage || (this._MasterDetailPage = bridge.getClass('Xamarin.Forms.MasterDetailPage, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get MediaElement() { return this._MediaElement || (this._MediaElement = bridge.getClass('Xamarin.Forms.MediaElement, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Menu() { return this._Menu || (this._Menu = bridge.getClass('Xamarin.Forms.Menu, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get MenuItem() { return this._MenuItem || (this._MenuItem = bridge.getClass('Xamarin.Forms.MenuItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get NavigationPage() { return this._NavigationPage || (this._NavigationPage = bridge.getClass('Xamarin.Forms.NavigationPage, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get OpenGLView() { return this._OpenGLView || (this._OpenGLView = bridge.getClass('Xamarin.Forms.OpenGLView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get OrientationStateTrigger() { return this._OrientationStateTrigger || (this._OrientationStateTrigger = bridge.getClass('Xamarin.Forms.OrientationStateTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PanGestureRecognizer() { return this._PanGestureRecognizer || (this._PanGestureRecognizer = bridge.getClass('Xamarin.Forms.PanGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Picker() { return this._Picker || (this._Picker = bridge.getClass('Xamarin.Forms.Picker, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PinchGestureRecognizer() { return this._PinchGestureRecognizer || (this._PinchGestureRecognizer = bridge.getClass('Xamarin.Forms.PinchGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ProgressBar() { return this._ProgressBar || (this._ProgressBar = bridge.getClass('Xamarin.Forms.ProgressBar, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RadialGradientBrush() { return this._RadialGradientBrush || (this._RadialGradientBrush = bridge.getClass('Xamarin.Forms.RadialGradientBrush, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RadioButton() { return this._RadioButton || (this._RadioButton = bridge.getClass('Xamarin.Forms.RadioButton, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RefreshView() { return this._RefreshView || (this._RefreshView = bridge.getClass('Xamarin.Forms.RefreshView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RelativeLayout() { return this._RelativeLayout || (this._RelativeLayout = bridge.getClass('Xamarin.Forms.RelativeLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RowDefinition() { return this._RowDefinition || (this._RowDefinition = bridge.getClass('Xamarin.Forms.RowDefinition, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ScrollView() { return this._ScrollView || (this._ScrollView = bridge.getClass('Xamarin.Forms.ScrollView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SearchBar() { return this._SearchBar || (this._SearchBar = bridge.getClass('Xamarin.Forms.SearchBar, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get BackButtonBehavior() { return this._BackButtonBehavior || (this._BackButtonBehavior = bridge.getClass('Xamarin.Forms.BackButtonBehavior, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get BaseShellItem() { return this._BaseShellItem || (this._BaseShellItem = bridge.getClass('Xamarin.Forms.BaseShellItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SearchHandler() { return this._SearchHandler || (this._SearchHandler = bridge.getClass('Xamarin.Forms.SearchHandler, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Shell() { return this._Shell || (this._Shell = bridge.getClass('Xamarin.Forms.Shell, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ShellContent() { return this._ShellContent || (this._ShellContent = bridge.getClass('Xamarin.Forms.ShellContent, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ShellGroupItem() { return this._ShellGroupItem || (this._ShellGroupItem = bridge.getClass('Xamarin.Forms.ShellGroupItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ShellItem() { return this._ShellItem || (this._ShellItem = bridge.getClass('Xamarin.Forms.ShellItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get FlyoutItem() { return this._FlyoutItem || (this._FlyoutItem = bridge.getClass('Xamarin.Forms.FlyoutItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TabBar() { return this._TabBar || (this._TabBar = bridge.getClass('Xamarin.Forms.TabBar, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ShellSection() { return this._ShellSection || (this._ShellSection = bridge.getClass('Xamarin.Forms.ShellSection, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Tab() { return this._Tab || (this._Tab = bridge.getClass('Xamarin.Forms.Tab, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Slider() { return this._Slider || (this._Slider = bridge.getClass('Xamarin.Forms.Slider, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SolidColorBrush() { return this._SolidColorBrush || (this._SolidColorBrush = bridge.getClass('Xamarin.Forms.SolidColorBrush, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Span() { return this._Span || (this._Span = bridge.getClass('Xamarin.Forms.Span, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get StackLayout() { return this._StackLayout || (this._StackLayout = bridge.getClass('Xamarin.Forms.StackLayout, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get StateTrigger() { return this._StateTrigger || (this._StateTrigger = bridge.getClass('Xamarin.Forms.StateTrigger, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Stepper() { return this._Stepper || (this._Stepper = bridge.getClass('Xamarin.Forms.Stepper, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get StreamImageSource() { return this._StreamImageSource || (this._StreamImageSource = bridge.getClass('Xamarin.Forms.StreamImageSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SwipeGestureRecognizer() { return this._SwipeGestureRecognizer || (this._SwipeGestureRecognizer = bridge.getClass('Xamarin.Forms.SwipeGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SwipeItem() { return this._SwipeItem || (this._SwipeItem = bridge.getClass('Xamarin.Forms.SwipeItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SwipeItems() { return this._SwipeItems || (this._SwipeItems = bridge.getClass('Xamarin.Forms.SwipeItems, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SwipeItemView() { return this._SwipeItemView || (this._SwipeItemView = bridge.getClass('Xamarin.Forms.SwipeItemView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SwipeView() { return this._SwipeView || (this._SwipeView = bridge.getClass('Xamarin.Forms.SwipeView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Switch() { return this._Switch || (this._Switch = bridge.getClass('Xamarin.Forms.Switch, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TabbedPage() { return this._TabbedPage || (this._TabbedPage = bridge.getClass('Xamarin.Forms.TabbedPage, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TableSectionBase() { return this._TableSectionBase || (this._TableSectionBase = bridge.getClass('Xamarin.Forms.TableSectionBase, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TableRoot() { return this._TableRoot || (this._TableRoot = bridge.getClass('Xamarin.Forms.TableRoot, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TableSection() { return this._TableSection || (this._TableSection = bridge.getClass('Xamarin.Forms.TableSection, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TableView() { return this._TableView || (this._TableView = bridge.getClass('Xamarin.Forms.TableView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TapGestureRecognizer() { return this._TapGestureRecognizer || (this._TapGestureRecognizer = bridge.getClass('Xamarin.Forms.TapGestureRecognizer, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TimePicker() { return this._TimePicker || (this._TimePicker = bridge.getClass('Xamarin.Forms.TimePicker, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ToolbarItem() { return this._ToolbarItem || (this._ToolbarItem = bridge.getClass('Xamarin.Forms.ToolbarItem, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get UriImageSource() { return this._UriImageSource || (this._UriImageSource = bridge.getClass('Xamarin.Forms.UriImageSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get UriMediaSource() { return this._UriMediaSource || (this._UriMediaSource = bridge.getClass('Xamarin.Forms.UriMediaSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get UrlWebViewSource() { return this._UrlWebViewSource || (this._UrlWebViewSource = bridge.getClass('Xamarin.Forms.UrlWebViewSource, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get WebView() { return this._WebView || (this._WebView = bridge.getClass('Xamarin.Forms.WebView, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get StyleSheet() { return this._StyleSheet || (this._StyleSheet = bridge.getClass('Xamarin.Forms.StyleSheets.StyleSheet, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PathSegment() { return this._PathSegment || (this._PathSegment = bridge.getClass('Xamarin.Forms.Shapes.PathSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ArcSegment() { return this._ArcSegment || (this._ArcSegment = bridge.getClass('Xamarin.Forms.Shapes.ArcSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get BezierSegment() { return this._BezierSegment || (this._BezierSegment = bridge.getClass('Xamarin.Forms.Shapes.BezierSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Transform() { return this._Transform || (this._Transform = bridge.getClass('Xamarin.Forms.Shapes.Transform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get CompositeTransform() { return this._CompositeTransform || (this._CompositeTransform = bridge.getClass('Xamarin.Forms.Shapes.CompositeTransform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Shape() { return this._Shape || (this._Shape = bridge.getClass('Xamarin.Forms.Shapes.Shape, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Ellipse() { return this._Ellipse || (this._Ellipse = bridge.getClass('Xamarin.Forms.Shapes.Ellipse, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Geometry() { return this._Geometry || (this._Geometry = bridge.getClass('Xamarin.Forms.Shapes.Geometry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get EllipseGeometry() { return this._EllipseGeometry || (this._EllipseGeometry = bridge.getClass('Xamarin.Forms.Shapes.EllipseGeometry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get GeometryGroup() { return this._GeometryGroup || (this._GeometryGroup = bridge.getClass('Xamarin.Forms.Shapes.GeometryGroup, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Line() { return this._Line || (this._Line = bridge.getClass('Xamarin.Forms.Shapes.Line, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get LineGeometry() { return this._LineGeometry || (this._LineGeometry = bridge.getClass('Xamarin.Forms.Shapes.LineGeometry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get LineSegment() { return this._LineSegment || (this._LineSegment = bridge.getClass('Xamarin.Forms.Shapes.LineSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get MatrixTransform() { return this._MatrixTransform || (this._MatrixTransform = bridge.getClass('Xamarin.Forms.Shapes.MatrixTransform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Path() { return this._Path || (this._Path = bridge.getClass('Xamarin.Forms.Shapes.Path, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PathFigure() { return this._PathFigure || (this._PathFigure = bridge.getClass('Xamarin.Forms.Shapes.PathFigure, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PathGeometry() { return this._PathGeometry || (this._PathGeometry = bridge.getClass('Xamarin.Forms.Shapes.PathGeometry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PolyBezierSegment() { return this._PolyBezierSegment || (this._PolyBezierSegment = bridge.getClass('Xamarin.Forms.Shapes.PolyBezierSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Polygon() { return this._Polygon || (this._Polygon = bridge.getClass('Xamarin.Forms.Shapes.Polygon, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Polyline() { return this._Polyline || (this._Polyline = bridge.getClass('Xamarin.Forms.Shapes.Polyline, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PolyLineSegment() { return this._PolyLineSegment || (this._PolyLineSegment = bridge.getClass('Xamarin.Forms.Shapes.PolyLineSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get PolyQuadraticBezierSegment() { return this._PolyQuadraticBezierSegment || (this._PolyQuadraticBezierSegment = bridge.getClass('Xamarin.Forms.Shapes.PolyQuadraticBezierSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get QuadraticBezierSegment() { return this._QuadraticBezierSegment || (this._QuadraticBezierSegment = bridge.getClass('Xamarin.Forms.Shapes.QuadraticBezierSegment, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get Rectangle() { return this._Rectangle || (this._Rectangle = bridge.getClass('Xamarin.Forms.Shapes.Rectangle, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RectangleGeometry() { return this._RectangleGeometry || (this._RectangleGeometry = bridge.getClass('Xamarin.Forms.Shapes.RectangleGeometry, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get RotateTransform() { return this._RotateTransform || (this._RotateTransform = bridge.getClass('Xamarin.Forms.Shapes.RotateTransform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get ScaleTransform() { return this._ScaleTransform || (this._ScaleTransform = bridge.getClass('Xamarin.Forms.Shapes.ScaleTransform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get SkewTransform() { return this._SkewTransform || (this._SkewTransform = bridge.getClass('Xamarin.Forms.Shapes.SkewTransform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TransformGroup() { return this._TransformGroup || (this._TransformGroup = bridge.getClass('Xamarin.Forms.Shapes.TransformGroup, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); },
        get TranslateTransform() { return this._TranslateTransform || (this._TranslateTransform = bridge.getClass('Xamarin.Forms.Shapes.TranslateTransform, Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null')); }
    };
    exports.default = XF;
});
//# sourceMappingURL=XF.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/clr/XF");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "./XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    //tslint:disable
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("./XF");
    const NSAtoms = "WebAtoms.Controls";
    const NSAssembly = "WebAtoms.XF";
    ;
    const WA = {
        get AtomRepeater() { return this._AtomRepeater || (this._AtomRepeater = bridge.getClass(`${NSAtoms}.AtomRepeater, ${NSAssembly}`)); },
        get AtomToolbarItem() { return this._AtomToolbarItem || (this._AtomToolbarItem = bridge.getClass(`${NSAtoms}.AtomToolbarItem, ${NSAssembly}`)); },
        get AtomView() { return this._AtomView || (this._AtomView = bridge.getClass(`${NSAtoms}.AtomView, ${NSAssembly}`)); },
        get AtomViewCell() { return this._AtomViewCell || (this._AtomViewCell = bridge.getClass(`${NSAtoms}.AtomViewCell, ${NSAssembly}`)); },
        get AtomForm() { return this._AtomForm || (this._AtomForm = bridge.getClass(`${NSAtoms}.AtomForm, ${NSAssembly}`)); },
        get AtomField() { return this._AtomField || (this._AtomField = bridge.getClass(`${NSAtoms}.AtomField, ${NSAssembly}`)); },
        get AtomTemplateSelector() { return this._AtomTemplateSelector || (this._AtomTemplateSelector = bridge.getClass(`${NSAtoms}.AtomTemplateSelector, ${NSAssembly}`)); },
        get GroupBy() { return this._GroupBy || (this._GroupBy = bridge.getClass(`${NSAtoms}.GroupBy, ${NSAssembly}`)); },
        get Markdown() { return this._Markdown || (this._Markdown = bridge.getClass(`${NSAtoms}.Markdown, ${NSAssembly}`)); }
    };
    exports.default = WA;
});
//# sourceMappingURL=WA.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/clr/WA");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomUI = exports.ChildEnumerator = void 0;
    // refer http://youmightnotneedjquery.com/
    class ChildEnumerator {
        constructor(e) {
            this.e = e;
        }
        get current() {
            return this.item;
        }
        next() {
            if (!this.item) {
                this.item = this.e.firstElementChild;
            }
            else {
                this.item = this.item.nextElementSibling;
            }
            return this.item ? true : false;
        }
    }
    exports.ChildEnumerator = ChildEnumerator;
    class AtomUI {
        static outerHeight(el, margin = false) {
            let height = el.offsetHeight;
            if (!margin) {
                return height;
            }
            const style = getComputedStyle(el);
            height += parseInt(style.marginTop, 10) + parseInt(style.marginBottom, 10);
            return height;
        }
        static outerWidth(el, margin = false) {
            let width = el.offsetWidth;
            if (!margin) {
                return width;
            }
            const style = getComputedStyle(el);
            width += parseInt(style.marginLeft, 10) + parseInt(style.marginRight, 10);
            return width;
        }
        static innerWidth(el) {
            return el.clientWidth;
        }
        static innerHeight(el) {
            return el.clientHeight;
        }
        static scrollTop(el, y) {
            el.scrollTo(0, y);
        }
        static screenOffset(e) {
            const r = {
                x: e.offsetLeft,
                y: e.offsetTop,
                width: e.offsetWidth,
                height: e.offsetHeight
            };
            if (e.offsetParent) {
                const p = this.screenOffset(e.offsetParent);
                r.x += p.x;
                r.y += p.y;
            }
            return r;
        }
        static parseUrl(url) {
            const r = {};
            const plist = url.split("&");
            for (const item of plist) {
                const p = item.split("=");
                const key = decodeURIComponent(p[0]);
                if (!key) {
                    continue;
                }
                let val = p[1];
                if (val) {
                    val = decodeURIComponent(val);
                }
                // val = AtomUI.parseValue(val);
                r[key] = this.parseValue(val);
            }
            return r;
        }
        static parseValue(val) {
            let n;
            if (/^[0-9]+$/.test(val)) {
                n = parseInt(val, 10);
                if (!isNaN(n)) {
                    return n;
                }
                return val;
            }
            if (/^[0-9]+\.[0-9]+/gi.test(val)) {
                n = parseFloat(val);
                if (!isNaN(n)) {
                    return n;
                }
                return val;
            }
            if (val === "true") {
                return true;
            }
            if (val === "false") {
                return false;
            }
            return val;
        }
        static assignID(element) {
            if (!element.id) {
                element.id = "__waID" + AtomUI.getNewIndex();
            }
            return element.id;
        }
        static toNumber(text) {
            if (!text) {
                return 0;
            }
            if (text.constructor === String) {
                return parseFloat(text);
            }
            return 0;
        }
        static getNewIndex() {
            AtomUI.index = AtomUI.index + 1;
            return AtomUI.index;
        }
    }
    exports.AtomUI = AtomUI;
    AtomUI.index = 1001;
});
//# sourceMappingURL=AtomUI.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/core/AtomUI");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../web/core/AtomUI", "./AtomBinder"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomBridge = exports.AtomElementBridge = exports.BaseElementBridge = void 0;
    const AtomUI_1 = require("../web/core/AtomUI");
    const AtomBinder_1 = require("./AtomBinder");
    class BaseElementBridge {
        refreshInherited(target, name, fieldName) {
            AtomBinder_1.AtomBinder.refreshValue(target, name);
            if (!fieldName) {
                fieldName = "m" + name[0].toUpperCase() + name.substr(1);
            }
            if (!target.element) {
                return;
            }
            this.visitDescendents(target.element, (e, ac) => {
                if (ac) {
                    if (ac[fieldName] === undefined) {
                        this.refreshInherited(ac, name, fieldName);
                    }
                    return false;
                }
                return true;
            });
        }
        createNode(target, node, 
        // tslint:disable-next-line: ban-types
        binder, 
        // tslint:disable-next-line: ban-types
        xNodeClass, 
        // tslint:disable-next-line: ban-types
        creator) {
            throw new Error("Method not implemented.");
        }
    }
    exports.BaseElementBridge = BaseElementBridge;
    class AtomElementBridge extends BaseElementBridge {
        addEventHandler(element, name, handler, capture) {
            element.addEventListener(name, handler, capture);
            return {
                dispose: () => {
                    element.removeEventListener(name, handler, capture);
                }
            };
        }
        atomParent(element, climbUp = true) {
            const eAny = element;
            if (eAny.atomControl) {
                return eAny.atomControl;
            }
            if (!climbUp) {
                return null;
            }
            if (!element.parentNode) {
                return null;
            }
            return this.atomParent(this.elementParent(element));
        }
        elementParent(element) {
            const eAny = element;
            const lp = eAny._logicalParent;
            if (lp) {
                return lp;
            }
            return element.parentElement;
        }
        templateParent(element) {
            if (!element) {
                return null;
            }
            const eAny = element;
            if (eAny._templateParent) {
                return this.atomParent(element);
            }
            const parent = this.elementParent(element);
            if (!parent) {
                return null;
            }
            return this.templateParent(parent);
        }
        visitDescendents(element, action) {
            const en = new AtomUI_1.ChildEnumerator(element);
            while (en.next()) {
                const iterator = en.current;
                const eAny = iterator;
                const ac = eAny ? eAny.atomControl : undefined;
                if (!action(iterator, ac)) {
                    continue;
                }
                this.visitDescendents(iterator, action);
            }
        }
        dispose(element) {
            const eAny = element;
            eAny.atomControl = undefined;
            eAny.innerHTML = "";
            delete eAny.atomControl;
        }
        appendChild(parent, child) {
            parent.appendChild(child);
        }
        setValue(element, name, value) {
            element[name] = value;
        }
        getValue(element, name) {
            return element[name];
        }
        watchProperty(element, name, events, f) {
            if (events.indexOf("change") === -1) {
                events.push("change");
            }
            const l = (e) => {
                const e1 = element;
                const v = e1.type === "checkbox" ? e1.checked : e1.value;
                f(v);
            };
            for (const iterator of events) {
                element.addEventListener(iterator, l, false);
            }
            return {
                dispose: () => {
                    for (const iterator of events) {
                        element.removeEventListener(iterator, l, false);
                    }
                }
            };
        }
        attachControl(element, control) {
            element.atomControl = control;
        }
        create(type) {
            return document.createElement(type);
        }
        loadContent(element, text) {
            throw new Error("Not supported");
        }
        findChild(element, name) {
            throw new Error("Not supported");
        }
        close(element, success, error) {
            throw new Error("Not supported");
        }
        toTemplate(element, creator) {
            const templateNode = element;
            const name = templateNode.name;
            if (typeof name === "string") {
                element = ((bx, n) => class extends bx {
                    create() {
                        this.render(n);
                    }
                })(creator, templateNode.children[0]);
            }
            else {
                element = ((base, n) => class extends base {
                    create() {
                        this.render(n);
                    }
                })(name, templateNode.children[0]);
            }
            return element;
        }
        createNode(target, node, 
        // tslint:disable-next-line: ban-types
        binder, 
        // tslint:disable-next-line: ban-types
        xNodeClass, 
        // tslint:disable-next-line: ban-types
        creator) {
            let parent = null;
            const app = target.app;
            let e = null;
            const nn = node.attributes ? node.attributes.for : undefined;
            if (typeof node.name === "string") {
                // it is simple node..
                e = document.createElement(node.name);
                parent = e;
                if (nn) {
                    delete node.attributes.for;
                }
            }
            else {
                if (nn) {
                    target = new node.name(app, document.createElement(nn));
                    delete node.attributes.for;
                }
                target = new node.name(app);
                e = target.element;
                parent = target;
                // target.append(child);
                // const firstChild = node.children ? node.children[0] : null;
                // if (firstChild) {
                // 	const n = this.createNode(child, firstChild, binder, xNodeClass, creator);
                // 	child.append(n.atomControl || n);
                // }
                // return child.element;
            }
            const a = node.attributes;
            if (a) {
                for (const key in a) {
                    if (a.hasOwnProperty(key)) {
                        let element = a[key];
                        if (element instanceof binder) {
                            if (/^event/.test(key)) {
                                let ev = key.substr(5);
                                if (ev.startsWith("-")) {
                                    ev = ev.split("-").map((s) => s[0].toLowerCase() + s.substr(1)).join("");
                                }
                                else {
                                    ev = ev[0].toLowerCase() + ev.substr(1);
                                }
                                element.setupFunction(ev, element, target, e);
                            }
                            else {
                                element.setupFunction(key, element, target, e);
                            }
                        }
                        else {
                            // this is template...
                            if (element instanceof xNodeClass) {
                                element = this.toTemplate(element, creator);
                            }
                            target.setLocalValue(e, key, element);
                        }
                    }
                }
            }
            const children = node.children;
            if (children) {
                for (const iterator of children) {
                    if (typeof iterator === "string") {
                        e.appendChild(document.createTextNode(iterator));
                        continue;
                    }
                    const t = iterator.attributes ? iterator.attributes.template : null;
                    if (t) {
                        const tx = this.toTemplate(iterator, creator);
                        target[t] = tx;
                        continue;
                    }
                    if (typeof iterator.name === "string") {
                        e.appendChild(this.createNode(target, iterator, binder, xNodeClass, creator));
                        continue;
                    }
                    const child = this.createNode(target, iterator, binder, xNodeClass, creator);
                    if (parent.element && parent.element.atomControl === parent) {
                        parent.append(child.atomControl || child);
                    }
                    else {
                        parent.appendChild(child);
                    }
                }
            }
            return e;
        }
    }
    exports.AtomElementBridge = AtomElementBridge;
    class AtomBridge {
        static createNode(iterator, app) {
            if (typeof iterator.name === "string" || iterator.name.factory) {
                return { element: AtomBridge.instance.create(iterator.name.toString(), iterator, app) };
            }
            const fx = iterator.attributes ? iterator.attributes.for : undefined;
            const c = new iterator.name(app, fx ? AtomBridge.instance.create(fx, iterator, app) : undefined);
            return { element: c.element, control: c };
        }
        static toTemplate(app, n, creator) {
            if (n.isTemplate) {
                const t = AtomBridge.toTemplate(app, n.children[0], creator);
                return AtomBridge.instance.create(n.name.toString(), t, app);
            }
            const bridge = AtomBridge.instance;
            let fx;
            let en;
            if (typeof n.name === "function") {
                fx = n.name;
                en = (n.attributes && n.attributes.for) ? n.attributes.for : undefined;
            }
            else {
                fx = bridge.controlFactory;
                en = n.name;
            }
            return class Template extends fx {
                constructor(a, e1) {
                    super(a || app, e1 || (en ? bridge.create(en, null, app) : undefined));
                    // tslint:disable-next-line: variable-name
                    this._creator = fx;
                }
                create() {
                    super.create();
                    this.render(n, null, creator);
                }
            };
        }
        static refreshInherited(target, name, fieldName) {
            if (AtomBridge.instance.refreshInherited) {
                AtomBridge.instance.refreshInherited(target, name, fieldName);
                return;
            }
            AtomBinder_1.AtomBinder.refreshValue(target, name);
            if (!fieldName) {
                fieldName = "m" + name[0].toUpperCase() + name.substr(1);
            }
            if (!target.element) {
                return;
            }
            AtomBridge.instance.visitDescendents(target.element, (e, ac) => {
                if (ac) {
                    if (ac[fieldName] === undefined) {
                        AtomBridge.refreshInherited(ac, name, fieldName);
                    }
                    return false;
                }
                return true;
            });
        }
    }
    exports.AtomBridge = AtomBridge;
    const globalNS = (typeof window !== "undefined" ? window : global);
    globalNS.AtomBridge = AtomBridge;
    if (typeof window !== "undefined") {
        AtomBridge.instance = new AtomElementBridge();
        AtomBridge.platform = "web";
    }
});
//# sourceMappingURL=AtomBridge.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomBridge");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    function RelativeSource(mode, ancestorType) {
        if (ancestorType.factory) {
            ancestorType = ancestorType.toString();
        }
        return {
            mode,
            ancestorType
        };
    }
    RelativeSource.self = { mode: "Self" };
    RelativeSource.TemplatedParent = { mode: "TemplatedParent" };
    const X = {
        Name: (n) => ({ "WebAtoms.AtomX:Name": new Bind_1.default((nx, bx, c, e) => {
                (AtomBridge_1.AtomBridge.instance).setName(e, n);
            }, null) }),
        Arguments: (args, ...nodes) => new XNode_1.default("WebAtoms.AtomX:Arguments", {}, nodes),
        Type: (n) => ({ type: n }),
        Resource: (n) => ({ resource: n }),
        RelativeSource,
        Binding: (b) => {
            return new Bind_1.default((n, bx, c, e) => {
                AtomBridge_1.AtomBridge.instance.setBinding(e, n, b);
            }, null);
        },
        TemplateBinding: (path) => X.Binding({ path, source: RelativeSource.TemplatedParent })
        // Key: (n: string) => ({ "WebAtoms.AtomX:Key": n }),
    };
    exports.default = X;
});
//# sourceMappingURL=X.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/clr/X");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomDispatcher = void 0;
    class AtomDispatcher {
        constructor() {
            this.head = null;
            this.tail = null;
        }
        onTimeout() {
            if (this.paused) {
                return;
            }
            if (!this.head) {
                return;
            }
            const item = this.head;
            this.head = item.next;
            item.next = null;
            if (!this.head) {
                this.tail = null;
            }
            item();
            setTimeout(() => {
                this.onTimeout();
            }, 1);
        }
        pause() {
            this.paused = true;
        }
        start() {
            this.paused = false;
            setTimeout(() => {
                this.onTimeout();
            }, 1);
        }
        callLater(f) {
            if (this.tail) {
                this.tail.next = f;
                this.tail = f;
            }
            else {
                this.head = f;
                this.tail = f;
            }
            if (!this.paused) {
                this.start();
            }
        }
        waitForAll() {
            return new Promise((resolve, reject) => {
                this.callLater(() => {
                    resolve();
                });
            });
        }
    }
    exports.AtomDispatcher = AtomDispatcher;
});
//# sourceMappingURL=AtomDispatcher.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomDispatcher");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/AtomMap"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.TypeKey = void 0;
    const AtomMap_1 = require("../core/AtomMap");
    class TypeKey {
        static get(c) {
            // for (const iterator of this.keys) {
            //     if (iterator.c === c) {
            //         return iterator.key;
            //     }
            // }
            // const key = `${c.name || "key"}${this.keys.length}`;
            // this.keys.push({ c, key});
            // return key;
            return TypeKey.keys.getOrCreate(c, (c1) => {
                const key = `${c1.name || "key"}${TypeKey.keys.size}`;
                return key;
            });
        }
    }
    exports.TypeKey = TypeKey;
    TypeKey.keys = new AtomMap_1.default();
});
// if (Map !== undefined) {
//     const map = new Map<any, string>();
//     const oldGet = TypeKey.get;
//     TypeKey.get = (c: any): string => {
//         const v = map.get(c);
//         if (!v) {
//             return v;
//         }
//         const v1 = oldGet(c);
//         map.set(c, v1);
//         return v1;
//     };
// }
//# sourceMappingURL=TypeKey.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/TypeKey");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/types", "./TypeKey"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ServiceCollection = exports.ServiceDescription = exports.Scope = void 0;
    const types_1 = require("../core/types");
    const TypeKey_1 = require("./TypeKey");
    var Scope;
    (function (Scope) {
        Scope[Scope["Global"] = 1] = "Global";
        Scope[Scope["Scoped"] = 2] = "Scoped";
        Scope[Scope["Transient"] = 3] = "Transient";
    })(Scope = exports.Scope || (exports.Scope = {}));
    class ServiceDescription {
        constructor(id, scope, type, factory) {
            this.id = id;
            this.scope = scope;
            this.type = type;
            this.factory = factory;
            this.factory = this.factory || ((sp) => {
                return sp.create(type);
            });
        }
    }
    exports.ServiceDescription = ServiceDescription;
    class ServiceCollection {
        constructor() {
            this.registrations = [];
            this.ids = 1;
        }
        register(type, factory, scope = Scope.Transient, id) {
            types_1.ArrayHelper.remove(this.registrations, (r) => id ? r.id === id : r.type === type);
            if (!id) {
                id = TypeKey_1.TypeKey.get(type) + this.ids;
                this.ids++;
            }
            const sd = new ServiceDescription(id, scope, type, factory);
            this.registrations.push(sd);
            return sd;
        }
        registerScoped(type, factory, id) {
            return this.register(type, factory, Scope.Scoped, id);
        }
        registerSingleton(type, factory, id) {
            return this.register(type, factory, Scope.Global, id);
        }
        get(type) {
            return this.registrations.find((s) => s.id === type || s.type === type);
        }
    }
    exports.ServiceCollection = ServiceCollection;
    ServiceCollection.instance = new ServiceCollection();
});
//# sourceMappingURL=ServiceCollection.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/ServiceCollection");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/types", "./ServiceCollection"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.Register = void 0;
    const types_1 = require("../core/types");
    const ServiceCollection_1 = require("./ServiceCollection");
    const globalNS = (typeof global === "undefined") ? window : global;
    function evalGlobal(path) {
        if (typeof path === "string") {
            let r = globalNS;
            for (const iterator of path.split(".")) {
                r = r[iterator];
                if (r === undefined || r === null) {
                    return r;
                }
            }
            return r;
        }
        return path;
    }
    function Register(id, scope) {
        return (target) => {
            if (typeof id === "object") {
                if (scope) {
                    id.scope = scope;
                }
                ServiceCollection_1.ServiceCollection.instance.register(id.for || target, id.for ? (sp) => sp.create(target) : null, id.scope || ServiceCollection_1.Scope.Transient, id.id);
                if (id.mockOrInject) {
                    if (id.mockOrInject.inject) {
                        types_1.DI.inject(target, id.mockOrInject.inject);
                    }
                    else if (id.mockOrInject.mock) {
                        types_1.DI.mockType(target, id.mockOrInject.mock);
                    }
                    else if (id.mockOrInject.globalVar) {
                        ServiceCollection_1.ServiceCollection.instance.register(id.for || target, (sp) => evalGlobal(id.mockOrInject.globalVar), id.scope || ServiceCollection_1.Scope.Global, id.id);
                    }
                }
                return;
            }
            ServiceCollection_1.ServiceCollection.instance.register(target, null, scope, id);
        };
    }
    exports.Register = Register;
});
//# sourceMappingURL=Register.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/Register");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./Register", "./ServiceCollection"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.RegisterSingleton = void 0;
    const Register_1 = require("./Register");
    const ServiceCollection_1 = require("./ServiceCollection");
    function RegisterSingleton(target) {
        Register_1.Register({ scope: ServiceCollection_1.Scope.Global })(target);
    }
    exports.RegisterSingleton = RegisterSingleton;
});
//# sourceMappingURL=RegisterSingleton.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/RegisterSingleton");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    class TransientDisposable {
        constructor(owner) {
            if (owner) {
                this.registerIn(owner);
            }
        }
        registerIn(value) {
            const v = value.disposables;
            if (v) {
                v.push(this);
            }
            else {
                if (value.registerDisposable) {
                    value.registerDisposable(this);
                }
            }
        }
    }
    exports.default = TransientDisposable;
});
//# sourceMappingURL=TransientDisposable.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/TransientDisposable");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./TypeKey"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.Inject = exports.InjectedTypes = void 0;
    const TypeKey_1 = require("./TypeKey");
    class InjectedTypes {
        static getParamList(key, typeKey1) {
            let plist = InjectedTypes.paramList[typeKey1];
            // We need to find @Inject for base types if
            // current type does not define any constructor
            let type = key;
            while (plist === undefined) {
                type = Object.getPrototypeOf(type);
                if (!type) {
                    break;
                }
                const typeKey = TypeKey_1.TypeKey.get(type);
                plist = InjectedTypes.paramList[typeKey];
                if (!plist) {
                    InjectedTypes.paramList[typeKey] = plist;
                }
            }
            return plist;
        }
        static getPropertyList(key, typeKey1) {
            let plist = InjectedTypes.propertyList[typeKey1];
            // We need to find @Inject for base types if
            // current type does not define any constructor
            let type = key;
            while (plist === undefined) {
                type = Object.getPrototypeOf(type);
                if (!type) {
                    break;
                }
                const typeKey = TypeKey_1.TypeKey.get(type);
                plist = InjectedTypes.propertyList[typeKey];
                if (!plist) {
                    InjectedTypes.propertyList[typeKey] = plist;
                }
            }
            return plist;
        }
    }
    exports.InjectedTypes = InjectedTypes;
    InjectedTypes.paramList = {};
    InjectedTypes.propertyList = {};
    // export function Inject(target: any, name: string): void;
    function Inject(target, name, index) {
        if (index !== undefined) {
            const key = TypeKey_1.TypeKey.get(target);
            const plist = Reflect.getMetadata("design:paramtypes", target, name);
            if (typeof index === "number") {
                const pSavedList = InjectedTypes.paramList[key] || (InjectedTypes.paramList[key] = []);
                pSavedList[index] = plist[index];
            }
            else {
                throw new Error("Inject can only be applied on constructor" +
                    "parameter or a property without get/set methods");
            }
        }
        else {
            const key = TypeKey_1.TypeKey.get(target.constructor);
            const plist = Reflect.getMetadata("design:type", target, name);
            const p = InjectedTypes.propertyList[key] || (InjectedTypes.propertyList[key] = {});
            p[name] = plist;
            // need to merge base properties..
            let base = target.constructor;
            while (true) {
                base = Object.getPrototypeOf(base);
                if (!base) {
                    break;
                }
                const baseKey = TypeKey_1.TypeKey.get(base);
                const bp = InjectedTypes.propertyList[baseKey];
                if (bp) {
                    for (const pKey in bp) {
                        if (bp.hasOwnProperty(pKey)) {
                            const element = bp[pKey];
                            if (!p[pKey]) {
                                p[pKey] = element;
                            }
                        }
                    }
                }
            }
        }
    }
    exports.Inject = Inject;
});
//# sourceMappingURL=Inject.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/Inject");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/TransientDisposable", "../core/types", "./Inject", "./ServiceCollection", "./TypeKey"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ServiceProvider = void 0;
    const TransientDisposable_1 = require("../core/TransientDisposable");
    const types_1 = require("../core/types");
    const Inject_1 = require("./Inject");
    const ServiceCollection_1 = require("./ServiceCollection");
    const TypeKey_1 = require("./TypeKey");
    class ServiceProvider {
        constructor(parent) {
            this.parent = parent;
            this.instances = {};
            if (parent === null) {
                ServiceCollection_1.ServiceCollection.instance.registerScoped(ServiceProvider);
            }
            const sd = ServiceCollection_1.ServiceCollection.instance.get(ServiceProvider);
            this.instances[sd.id] = this;
        }
        get global() {
            return this.parent === null ? this : this.parent.global;
        }
        get(key) {
            return this.resolve(key, true);
        }
        put(key, value) {
            let sd = ServiceCollection_1.ServiceCollection.instance.get(key);
            if (!sd) {
                sd = ServiceCollection_1.ServiceCollection.instance.register(key, () => value, ServiceCollection_1.Scope.Global);
            }
            this.instances[sd.id] = value;
        }
        resolve(key, create = false, defValue) {
            const sd = ServiceCollection_1.ServiceCollection.instance.get(key);
            if (!sd) {
                if (!create) {
                    if (defValue !== undefined) {
                        return defValue;
                    }
                    throw new Error(`No service registered for type ${key}`);
                }
                return this.create(key);
            }
            if (sd.type === ServiceProvider) {
                return this;
            }
            if (sd.scope === ServiceCollection_1.Scope.Global) {
                return this.global.getValue(sd);
            }
            if (sd.scope === ServiceCollection_1.Scope.Scoped) {
                if (this.parent === null) {
                    throw new Error("Scoped dependency cannot be created on global");
                }
            }
            return this.getValue(sd);
        }
        getValue(sd) {
            if (sd.scope === ServiceCollection_1.Scope.Transient) {
                return sd.factory(this);
            }
            let v = this.instances[sd.id];
            if (!v) {
                this.instances[sd.id] = v = sd.factory(this);
            }
            return v;
        }
        newScope() {
            return new ServiceProvider(this);
        }
        dispose() {
            for (const key in this.instances) {
                if (this.instances.hasOwnProperty(key)) {
                    const element = this.instances[key];
                    if (element === this) {
                        continue;
                    }
                    const d = element;
                    if (d.dispose) {
                        d.dispose();
                    }
                }
            }
        }
        create(key) {
            const originalKey = key;
            const originalTypeKey = TypeKey_1.TypeKey.get(originalKey);
            if (types_1.DI.resolveType) {
                const mappedType = ServiceProvider.mappedTypes[originalTypeKey] || (ServiceProvider.mappedTypes[originalTypeKey] = types_1.DI.resolveType(originalKey));
                key = mappedType;
            }
            const typeKey1 = TypeKey_1.TypeKey.get(key);
            const plist = Inject_1.InjectedTypes.getParamList(key, typeKey1);
            let value = null;
            if (plist) {
                const pv = plist.map((x) => x ? this.resolve(x) : (void 0));
                pv.unshift(null);
                value = new (key.bind.apply(key, pv))();
                for (const iterator of pv) {
                    if (iterator && iterator instanceof TransientDisposable_1.default) {
                        iterator.registerIn(value);
                    }
                }
            }
            else {
                value = new (key)();
            }
            const propList = Inject_1.InjectedTypes.getPropertyList(key, typeKey1);
            if (propList) {
                for (const key1 in propList) {
                    if (propList.hasOwnProperty(key1)) {
                        const element = propList[key1];
                        const d = this.resolve(element);
                        value[key1] = d;
                        if (d && d instanceof TransientDisposable_1.default) {
                            d.registerIn(value);
                        }
                    }
                }
            }
            return value;
        }
    }
    exports.ServiceProvider = ServiceProvider;
    ServiceProvider.mappedTypes = {};
});
//# sourceMappingURL=ServiceProvider.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/ServiceProvider");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../di/RegisterSingleton"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.BusyIndicatorService = void 0;
    const RegisterSingleton_1 = require("../di/RegisterSingleton");
    let BusyIndicatorService = class BusyIndicatorService {
        createIndicator() {
            return {
                dispose() {
                    // do nothing.
                }
            };
        }
    };
    BusyIndicatorService = __decorate([
        RegisterSingleton_1.RegisterSingleton
    ], BusyIndicatorService);
    exports.BusyIndicatorService = BusyIndicatorService;
});
//# sourceMappingURL=BusyIndicatorService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/BusyIndicatorService");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./core/AtomBinder", "./core/AtomDispatcher", "./di/RegisterSingleton", "./di/ServiceProvider", "./services/BusyIndicatorService"], factory);
    }
})(function (require, exports) {
    "use strict";
    var App_1;
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.App = exports.AtomMessageAction = void 0;
    const AtomBinder_1 = require("./core/AtomBinder");
    const AtomDispatcher_1 = require("./core/AtomDispatcher");
    const RegisterSingleton_1 = require("./di/RegisterSingleton");
    const ServiceProvider_1 = require("./di/ServiceProvider");
    const BusyIndicatorService_1 = require("./services/BusyIndicatorService");
    class AtomHandler {
        constructor(message) {
            this.message = message;
            this.list = new Array();
        }
    }
    class AtomMessageAction {
        constructor(msg, a) {
            this.message = msg;
            this.action = a;
        }
    }
    exports.AtomMessageAction = AtomMessageAction;
    let App = App_1 = class App extends ServiceProvider_1.ServiceProvider {
        constructor() {
            super(null);
            /**
             * This must be set explicitly as it can be used outside to detect
             * if app is ready. This will not be set automatically by framework.
             */
            this.appReady = false;
            this.busyIndicators = [];
            // tslint:disable-next-line:ban-types
            this.readyHandlers = [];
            this.onError = (error) => {
                // tslint:disable-next-line:no-console
                console.log(error);
            };
            this.screen = {};
            this.bag = {};
            this.put(App_1, this);
            this.dispatcher = new AtomDispatcher_1.AtomDispatcher();
            this.dispatcher.start();
            this.put(AtomDispatcher_1.AtomDispatcher, this.dispatcher);
            setTimeout(() => {
                this.invokeReady();
            }, 5);
        }
        get url() {
            return this.mUrl;
        }
        set url(v) {
            this.mUrl = v;
            AtomBinder_1.AtomBinder.refreshValue(this, "url");
        }
        get contextId() {
            return "none";
        }
        createBusyIndicator() {
            this.busyIndicatorService = this.busyIndicatorService
                || this.resolve(BusyIndicatorService_1.BusyIndicatorService);
            return this.busyIndicatorService.createIndicator();
        }
        syncUrl() {
            // must be implemented by platform specific app
        }
        callLater(f) {
            this.dispatcher.callLater(f);
        }
        updateDefaultStyle(content) {
            throw new Error("Platform does not support StyleSheets");
        }
        waitForPendingCalls() {
            return this.dispatcher.waitForAll();
        }
        /**
         * This method will run any asynchronous method
         * and it will display an error if it will fail
         * asynchronously
         *
         * @template T
         * @param {() => Promise<T>} tf
         * @memberof AtomDevice
         */
        runAsync(tf) {
            try {
                const p = tf();
                if (p && p.then && p.catch) {
                    p.catch((error) => {
                        this.onError("runAsync");
                        this.onError(error);
                    });
                }
            }
            catch (e) {
                this.onError("runAsync");
                this.onError(e);
            }
        }
        /**
         * Broadcast given data to channel, only within the current window.
         *
         * @param {string} channel
         * @param {*} data
         * @returns
         * @memberof AtomDevice
         */
        broadcast(channel, data) {
            const ary = this.bag[channel];
            if (!ary) {
                return;
            }
            for (const entry of ary.list) {
                entry.call(this, channel, data);
            }
        }
        /**
         * Subscribe for given channel with action that will be
         * executed when anyone will broadcast (this only works within the
         * current browser window)
         *
         * This method returns a disposable, when you call `.dispose()` it will
         * unsubscribe for current subscription
         *
         * @param {string} channel
         * @param {AtomAction} action
         * @returns {AtomDisposable} Disposable that supports removal of subscription
         * @memberof AtomDevice
         */
        subscribe(channel, action) {
            let ary = this.bag[channel];
            if (!ary) {
                ary = new AtomHandler(channel);
                this.bag[channel] = ary;
            }
            ary.list.push(action);
            return {
                dispose: () => {
                    ary.list = ary.list.filter((a) => a !== action);
                    if (!ary.list.length) {
                        this.bag[channel] = null;
                    }
                }
            };
        }
        main() {
            // load app here..
        }
        // tslint:disable-next-line:no-empty
        onReady(f) {
            if (this.readyHandlers) {
                this.readyHandlers.push(f);
            }
            else {
                this.invokeReadyHandler(f);
            }
        }
        invokeReady() {
            for (const iterator of this.readyHandlers) {
                this.invokeReadyHandler(iterator);
            }
            this.readyHandlers = null;
        }
        // tslint:disable-next-line:ban-types
        invokeReadyHandler(f) {
            const indicator = this.createBusyIndicator();
            const a = f();
            if (a && a.then && a.catch) {
                a.then((r) => {
                    // do nothing
                    indicator.dispose();
                });
                a.catch((e) => {
                    indicator.dispose();
                    // tslint:disable-next-line:no-console
                    // console.error("XFApp.onReady");
                    // tslint:disable-next-line:no-console
                    console.error(typeof e === "string" ? e : JSON.stringify(e));
                });
            }
        }
    };
    App = App_1 = __decorate([
        RegisterSingleton_1.RegisterSingleton,
        __metadata("design:paramtypes", [])
    ], App);
    exports.App = App;
});
//# sourceMappingURL=App.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/App");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomOnce = void 0;
    /**
     * AtomOnce will execute given method only once and it will prevent recursive calls.
     * This is important when you want to update source and destination and prevent recursive calls.
     * @example
     *      private valueOnce: AtomOnce = new AtomOnce();
     *
     *      private onValueChanged(): void {
     *          valueOnce.run(()=> {
     *              this.value = compute(this.target);
     *          });
     *      }
     *
     *      private onTargetChanged(): void {
     *          valueOnce.run(() => {
     *              this.target = computeInverse(this.value);
     *          });
     *      }
     */
    class AtomOnce {
        /**
         * AtomOnce will execute given method only once and it will prevent recursive calls.
         * This is important when you want to update source and destination and prevent recursive calls.
         * @example
         *      private valueOnce: AtomOnce = new AtomOnce();
         *
         *      private onValueChanged(): void {
         *          valueOnce.run(()=> {
         *              this.value = compute(this.target);
         *          });
         *      }
         *
         *      private onTargetChanged(): void {
         *          valueOnce.run(() => {
         *              this.target = computeInverse(this.value);
         *          });
         *      }
         */
        run(f) {
            if (this.isRunning) {
                return;
            }
            let isAsync = false;
            try {
                this.isRunning = true;
                const p = f();
                if (p && p.then && p.catch) {
                    isAsync = true;
                    p.then(() => {
                        this.isRunning = false;
                    }).catch(() => {
                        this.isRunning = false;
                    });
                }
            }
            finally {
                if (!isAsync) {
                    this.isRunning = false;
                }
            }
        }
    }
    exports.AtomOnce = AtomOnce;
});
//# sourceMappingURL=AtomOnce.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomOnce");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomBinder", "./ExpressionParser"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomWatcher = exports.ObjectProperty = void 0;
    const AtomBinder_1 = require("./AtomBinder");
    const ExpressionParser_1 = require("./ExpressionParser");
    class ObjectProperty {
        constructor(name) {
            this.name = name;
        }
        toString() {
            return this.name;
        }
    }
    exports.ObjectProperty = ObjectProperty;
    /**
     *
     *
     * @export
     * @class AtomWatcher
     * @implements {IDisposable}
     * @template T
     */
    class AtomWatcher {
        /**
         * Creates an instance of AtomWatcher.
         *
         *      let w = new AtomWatcher(this, x => x.data.fullName = `${x.data.firstName} ${x.data.lastName}`);
         *
         * You must dispose `w` in order to avoid memory leaks.
         * Above method will set fullName whenever, data or its firstName,lastName property is modified.
         *
         * AtomWatcher will assign null if any expression results in null in single property path.
         *
         * In order to avoid null, you can rewrite above expression as,
         *
         *      let w = new AtomWatcher(this,
         *                  x => {
         *                      if(x.data.firstName && x.data.lastName){
         *                        x.data.fullName = `${x.data.firstName} ${x.data.lastName}`
         *                      }
         *                  });
         *
         * @param {T} target - Target on which watch will be set to observe given set of properties
         * @param {(PathList[] | ((x:T) => any))} path - Path is either lambda expression or array of
         *                      property path to watch, if path was lambda, it will be executed when any of
         *                      members will modify
         * @param {Function} onChanged - This function will be executed when any member in path is updated
         * @memberof AtomWatcher
         */
        constructor(target, path, onChanged, source) {
            this.source = source;
            this.isExecuting = false;
            this.target = target;
            this.forValidation = true;
            if (path instanceof Function) {
                const f = path;
                path = ExpressionParser_1.parsePath(path);
                this.func = onChanged || f;
                this.funcText = f.toString();
            }
            else {
                this.func = onChanged;
            }
            this.runEvaluate = () => {
                this.evaluate();
            };
            this.runEvaluate.watcher = this;
            this.path = path.map((x) => x.map((y) => new ObjectProperty(y)));
            if (!this.path.length) {
                // tslint:disable-next-line:no-debugger
                debugger;
                // tslint:disable-next-line:no-console
                console.warn("There is nothing to watch, do not use one way binding without any binding expression");
            }
        }
        toString() {
            return this.func.toString();
        }
        /**
         * This will dispose and unregister all watchers
         *
         * @memberof AtomWatcher
         */
        dispose() {
            if (!this.path) {
                return;
            }
            for (const p of this.path) {
                for (const op of p) {
                    if (op.watcher) {
                        op.watcher.dispose();
                        op.watcher = null;
                        op.target = null;
                    }
                }
            }
            // tslint:disable-next-line:no-string-literal
            // this["disposedPath"] = this.path;
            this.func = null;
            // this.path.length = 0;
            this.path = null;
            this.source = null;
        }
        /**
         * Initialize the path targets
         * @param evaluate if true, evaluate entire watch expression and run onChange method
         */
        init(evaluate) {
            if (evaluate) {
                this.evaluate(true);
            }
            else {
                for (const iterator of this.path) {
                    this.evaluatePath(this.target, iterator);
                }
            }
        }
        evaluatePath(target, path) {
            // console.log(`\tevaluatePath: ${path.map(op=>op.name).join(", ")}`);
            let newTarget = null;
            for (const p of path) {
                if (this.source && p.name === "this") {
                    target = this.source;
                    continue;
                }
                newTarget = target[p.name];
                if (!p.target) {
                    if (p.watcher) {
                        p.watcher.dispose();
                    }
                    p.watcher = AtomBinder_1.AtomBinder.watch(target, p.name, this.runEvaluate);
                }
                else if (p.target !== target) {
                    if (p.watcher) {
                        p.watcher.dispose();
                    }
                    p.watcher = AtomBinder_1.AtomBinder.watch(target, p.name, this.runEvaluate);
                }
                p.target = target;
                target = newTarget;
                if (newTarget === undefined || newTarget === null) {
                    break;
                }
            }
            return newTarget;
        }
        /**
         *
         *
         * @param {boolean} [force]
         * @returns {*}
         * @memberof AtomWatcher
         */
        evaluate(force) {
            if (!this.path) {
                // this watcher may have been disposed...
                // tslint:disable-next-line:no-console
                console.warn(`Watcher is not disposed properly, please watch for any memory leak`);
                return;
            }
            if (this.isExecuting) {
                return;
            }
            const disposeWatchers = [];
            this.isExecuting = true;
            try {
                const values = [];
                const logs = [];
                for (const p of this.path) {
                    values.push(this.evaluatePath(this.target, p));
                }
                // if (force === true) {
                //     this.forValidation = false;
                // }
                // if (this.forValidation) {
                //     const x: boolean = true;
                //     if (values.find( (x1) => x1 ? true : false)) {
                //         this.forValidation = false;
                //     } else {
                //         return;
                //     }
                // }
                try {
                    this.func.apply(this.target, values);
                }
                catch (e) {
                    // tslint:disable-next-line:no-console
                    console.warn(e);
                }
            }
            finally {
                this.isExecuting = false;
                for (const d of disposeWatchers) {
                    d.dispose();
                }
            }
        }
    }
    exports.AtomWatcher = AtomWatcher;
});
//# sourceMappingURL=AtomWatcher.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomWatcher");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomBridge", "./AtomComponent", "./AtomOnce", "./AtomWatcher"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.PropertyBinding = void 0;
    const AtomBridge_1 = require("./AtomBridge");
    const AtomComponent_1 = require("./AtomComponent");
    const AtomOnce_1 = require("./AtomOnce");
    const AtomWatcher_1 = require("./AtomWatcher");
    class PropertyBinding {
        constructor(target, element, name, path, twoWays, valueFunc, source) {
            this.target = target;
            this.element = element;
            this.name = name;
            this.twoWays = twoWays;
            this.source = source;
            this.isTwoWaySetup = false;
            this.name = name;
            this.twoWays = twoWays;
            this.target = target;
            this.element = element;
            this.updaterOnce = new AtomOnce_1.AtomOnce();
            if (valueFunc) {
                if (typeof valueFunc !== "function") {
                    this.fromSourceToTarget = valueFunc.fromSource;
                    this.fromTargetToSource = valueFunc.fromTarget;
                }
                else {
                    this.fromSourceToTarget = valueFunc;
                }
            }
            this.watcher = new AtomWatcher_1.AtomWatcher(target, path, (...v) => {
                this.updaterOnce.run(() => {
                    if (this.disposed) {
                        return;
                    }
                    // set value
                    for (const iterator of v) {
                        if (iterator === undefined) {
                            return;
                        }
                    }
                    const cv = this.fromSourceToTarget ? this.fromSourceToTarget.apply(this, v) : v[0];
                    if (this.target instanceof AtomComponent_1.AtomComponent) {
                        this.target.setLocalValue(this.element, this.name, cv);
                    }
                    else {
                        this.target[name] = cv;
                    }
                });
            }, source);
            this.path = this.watcher.path;
            if (this.target instanceof AtomComponent_1.AtomComponent) {
                this.target.runAfterInit(() => {
                    if (!this.watcher) {
                        // this is disposed ...
                        return;
                    }
                    this.watcher.init(true);
                    if (twoWays) {
                        this.setupTwoWayBinding();
                    }
                });
            }
            else {
                this.watcher.init(true);
                if (twoWays) {
                    this.setupTwoWayBinding();
                }
            }
        }
        setupTwoWayBinding() {
            if (this.target instanceof AtomComponent_1.AtomComponent) {
                if (this.element
                    && (this.element !== this.target.element || !this.target.hasProperty(this.name))) {
                    // most likely it has change event..
                    let events = [];
                    if (typeof this.twoWays !== "boolean") {
                        events = this.twoWays;
                    }
                    this.twoWaysDisposable = AtomBridge_1.AtomBridge.instance.watchProperty(this.element, this.name, events, (v) => {
                        this.setInverseValue(v);
                    });
                    return;
                }
            }
            const watcher = new AtomWatcher_1.AtomWatcher(this.target, [[this.name]], (...values) => {
                if (this.isTwoWaySetup) {
                    this.setInverseValue(values[0]);
                }
            });
            watcher.init(true);
            this.isTwoWaySetup = true;
            this.twoWaysDisposable = watcher;
        }
        setInverseValue(value) {
            if (!this.twoWays) {
                throw new Error("This Binding is not two ways.");
            }
            this.updaterOnce.run(() => {
                if (this.disposed) {
                    return;
                }
                const first = this.path[0];
                const length = first.length;
                let v = this.target;
                let i = 0;
                let name;
                for (i = 0; i < length - 1; i++) {
                    name = first[i].name;
                    if (name === "this") {
                        v = this.source || this.target;
                    }
                    else {
                        v = v[name];
                    }
                    if (!v) {
                        return;
                    }
                }
                name = first[i].name;
                v[name] = this.fromTargetToSource ? this.fromTargetToSource.call(this, value) : value;
            });
        }
        dispose() {
            if (this.twoWaysDisposable) {
                this.twoWaysDisposable.dispose();
                this.twoWaysDisposable = null;
            }
            this.watcher.dispose();
            this.disposed = true;
            this.watcher = null;
        }
    }
    exports.PropertyBinding = PropertyBinding;
});
//# sourceMappingURL=PropertyBinding.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/PropertyBinding");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomDisposableList = void 0;
    class AtomDisposableList {
        constructor() {
            // tslint:disable-next-line:ban-types
            this.disposables = [];
        }
        // tslint:disable-next-line:ban-types
        add(d) {
            if (typeof d === "function") {
                const fx = d;
                d = {
                    dispose: fx
                };
            }
            this.disposables.push(d);
            const dx = d;
            return {
                dispose: () => {
                    this.disposables = this.disposables.filter((x) => x !== dx);
                    dx.dispose();
                }
            };
        }
        dispose() {
            for (const iterator of this.disposables) {
                iterator.dispose();
            }
            this.disposables.length = 0;
        }
    }
    exports.AtomDisposableList = AtomDisposableList;
});
//# sourceMappingURL=AtomDisposableList.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomDisposableList");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomBridge"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.InheritedProperty = void 0;
    const AtomBridge_1 = require("./AtomBridge");
    /**
     * Use this decorator only to watch property changes in `onPropertyChanged` method.
     * This decorator also makes enumerable property.
     *
     * Do not use this on anything except UI control
     * @param target control
     * @param key name of property
     */
    function InheritedProperty(target, key) {
        // property value
        const iVal = target[key];
        const keyName = typeof Symbol === "undefined"
            ? ("_" + key)
            : Symbol(`${key}`);
        target[keyName] = iVal;
        // property getter
        const getter = function () {
            const p = this[keyName];
            if (p !== undefined) {
                return p;
            }
            if (this.element && this.parent) {
                return this.parent[key];
            }
            return undefined;
        };
        // property setter
        const setter = function (newVal) {
            const oldValue = this[keyName];
            if (oldValue && oldValue.dispose) {
                oldValue.dispose();
            }
            this[keyName] = newVal;
            AtomBridge_1.AtomBridge.refreshInherited(this, key);
        };
        // delete property
        if (delete target[key]) {
            // create new property with getter and setter
            Object.defineProperty(target, key, {
                get: getter,
                set: setter,
                enumerable: true,
                configurable: true
            });
        }
    }
    exports.InheritedProperty = InheritedProperty;
});
//# sourceMappingURL=InheritedProperty.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/InheritedProperty");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../di/TypeKey"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.PropertyMap = void 0;
    const TypeKey_1 = require("../di/TypeKey");
    class PropertyMap {
        // tslint:disable-next-line:ban-types
        static from(o) {
            const c = Object.getPrototypeOf(o);
            const key = TypeKey_1.TypeKey.get(c);
            const map = PropertyMap.map;
            const m = map[key] || (map[key] = PropertyMap.createMap(o));
            return m;
        }
        static createMap(c) {
            const map = {};
            const nameList = [];
            while (c) {
                const names = Object.getOwnPropertyNames(c);
                for (const name of names) {
                    if (/hasOwnProperty|constructor|toString|isValid|errors/i.test(name)) {
                        continue;
                    }
                    // // map[name] = Object.getOwnPropertyDescriptor(c, name) ? true : false;
                    // const pd = Object.getOwnPropertyDescriptor(c, name);
                    // // tslint:disable-next-line:no-console
                    // console.log(`${name} = ${c.enumerable}`);
                    map[name] = true;
                    nameList.push(name);
                }
                c = Object.getPrototypeOf(c);
            }
            const m = new PropertyMap();
            m.map = map;
            m.names = nameList;
            return m;
        }
    }
    exports.PropertyMap = PropertyMap;
    PropertyMap.map = {};
});
//# sourceMappingURL=PropertyMap.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/PropertyMap");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../App", "../core/AtomBridge", "../core/PropertyBinding", "../core/types", "../di/Inject", "./AtomDisposableList", "./Bind", "./InheritedProperty", "./PropertyMap", "./XNode"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomComponent = void 0;
    const App_1 = require("../App");
    const AtomBridge_1 = require("../core/AtomBridge");
    const PropertyBinding_1 = require("../core/PropertyBinding");
    // tslint:disable-next-line:import-spacing
    const types_1 = require("../core/types");
    const Inject_1 = require("../di/Inject");
    const AtomDisposableList_1 = require("./AtomDisposableList");
    const Bind_1 = require("./Bind");
    const InheritedProperty_1 = require("./InheritedProperty");
    const PropertyMap_1 = require("./PropertyMap");
    const XNode_1 = require("./XNode");
    let AtomComponent = class AtomComponent {
        constructor(app, element = null) {
            this.app = app;
            this.mInvalidated = 0;
            this.mPendingPromises = {};
            this.disposables = new AtomDisposableList_1.AtomDisposableList();
            this.bindings = [];
            this.eventHandlers = [];
            this.element = element;
            AtomBridge_1.AtomBridge.instance.attachControl(this.element, this);
            const a = this.beginEdit();
            this.preCreate();
            this.create();
            app.callLater(() => a.dispose());
        }
        /** Do not ever use, only available as intellisense feature for
         * vs code editor.
         */
        get vsProps() {
            return undefined;
        }
        bind(element, name, path, twoWays, valueFunc, source) {
            // remove existing binding if any
            // let binding = this.bindings.find( (x) => x.name === name && (element ? x.element === element : true));
            // if (binding) {
            //     binding.dispose();
            //     ArrayHelper.remove(this.bindings, (x) => x === binding);
            // }
            const binding = new PropertyBinding_1.PropertyBinding(this, element, name, path, twoWays, valueFunc, source);
            this.bindings.push(binding);
            return {
                dispose: () => {
                    binding.dispose();
                    types_1.ArrayHelper.remove(this.bindings, (x) => x === binding);
                }
            };
        }
        /**
         * Remove all bindings associated with given element and optional name
         * @param element T
         * @param name string
         */
        unbind(element, name) {
            const toDelete = this.bindings.filter((x) => x.element === element && (!name || (x.name === name)));
            for (const iterator of toDelete) {
                iterator.dispose();
                types_1.ArrayHelper.remove(this.bindings, (x) => x === iterator);
            }
        }
        bindEvent(element, name, method, key) {
            if (!element) {
                return;
            }
            if (!method) {
                return;
            }
            const be = {
                element,
                name,
                handler: method
            };
            if (key) {
                be.key = key;
            }
            be.disposable = AtomBridge_1.AtomBridge.instance.addEventHandler(element, name, method, false);
            this.eventHandlers.push(be);
            return {
                dispose: () => {
                    be.disposable.dispose();
                    types_1.ArrayHelper.remove(this.eventHandlers, (e) => e.disposable === be.disposable);
                }
            };
        }
        unbindEvent(element, name, method, key) {
            const deleted = [];
            for (const be of this.eventHandlers) {
                if (element && be.element !== element) {
                    return;
                }
                if (key && be.key !== key) {
                    return;
                }
                if (name && be.name !== name) {
                    return;
                }
                if (method && be.handler !== method) {
                    return;
                }
                be.disposable.dispose();
                be.handler = null;
                be.element = null;
                be.name = null;
                be.key = null;
                deleted.push(() => this.eventHandlers.remove(be));
            }
            for (const iterator of deleted) {
                iterator();
            }
        }
        /**
         * Control checks if property is declared on the control or not.
         * Since TypeScript no longer creates enumerable properties, we have
         * to inspect name and PropertyMap which is generated by `@BindableProperty`
         * or the value is not set to undefined.
         * @param name name of Property
         */
        hasProperty(name) {
            if (/^(data|viewModel|localViewModel|element)$/.test(name)) {
                return true;
            }
            const map = PropertyMap_1.PropertyMap.from(this);
            if (map.map[name]) {
                return true;
            }
            if (this[name] !== undefined) {
                return true;
            }
            return false;
        }
        /**
         * Use this method if you want to set attribute on HTMLElement immediately but
         * defer atom control property
         * @param element HTMLElement
         * @param name string
         * @param value any
         */
        setPrimitiveValue(element, name, value) {
            const p = value;
            if (p && p.then && p.catch) {
                this.mPendingPromises[name] = p;
                p.then((r) => {
                    if (this.mPendingPromises[name] !== p) {
                        return;
                    }
                    this.mPendingPromises[name] = null;
                    this.setPrimitiveValue(element, name, r);
                }).catch((e) => {
                    if (this.mPendingPromises[name] !== p) {
                        return;
                    }
                    this.mPendingPromises[name] = null;
                    // tslint:disable-next-line:no-console
                    console.error(e);
                });
                return;
            }
            if (/^(viewModel|localViewModel)$/.test(name)) {
                this[name] = value;
                return;
            }
            if ((!element || element === this.element) && this.hasProperty(name)) {
                this.runAfterInit(() => {
                    this[name] = value;
                });
            }
            else {
                this.setElementValue(element, name, value);
            }
        }
        setLocalValue(element, name, value) {
            // if value is a promise
            const p = value;
            if (p && p.then && p.catch) {
                this.mPendingPromises[name] = p;
                p.then((r) => {
                    if (this.mPendingPromises[name] !== p) {
                        return;
                    }
                    this.mPendingPromises[name] = null;
                    this.setLocalValue(element, name, r);
                }).catch((e) => {
                    if (this.mPendingPromises[name] !== p) {
                        return;
                    }
                    this.mPendingPromises[name] = null;
                    // tslint:disable-next-line:no-console
                    console.error(e);
                });
                return;
            }
            if ((!element || element === this.element) && this.hasProperty(name)) {
                this[name] = value;
            }
            else {
                this.setElementValue(element, name, value);
            }
        }
        dispose(e) {
            if (this.mInvalidated) {
                clearTimeout(this.mInvalidated);
                this.mInvalidated = 0;
            }
            AtomBridge_1.AtomBridge.instance.visitDescendents(e || this.element, (ex, ac) => {
                if (ac) {
                    ac.dispose();
                    return false;
                }
                return true;
            });
            if (!e) {
                this.unbindEvent(null, null, null);
                for (const binding of this.bindings) {
                    binding.dispose();
                }
                this.bindings.length = 0;
                this.bindings = null;
                AtomBridge_1.AtomBridge.instance.dispose(this.element);
                this.element = null;
                const lvm = this.localViewModel;
                if (lvm && lvm.dispose) {
                    lvm.dispose();
                    this.localViewModel = null;
                }
                const vm = this.viewModel;
                if (vm && vm.dispose) {
                    vm.dispose();
                    this.viewModel = null;
                }
                this.disposables.dispose();
                this.pendingInits = null;
            }
        }
        // tslint:disable-next-line:no-empty
        onPropertyChanged(name) {
        }
        beginEdit() {
            this.pendingInits = [];
            const a = this.pendingInits;
            return {
                dispose: () => {
                    if (this.pendingInits == null) {
                        // case where current control is disposed...
                        return;
                    }
                    this.pendingInits = null;
                    if (a) {
                        for (const iterator of a) {
                            iterator();
                        }
                    }
                    this.invalidate();
                }
            };
        }
        invalidate() {
            if (this.mInvalidated) {
                clearTimeout(this.mInvalidated);
            }
            this.mInvalidated = setTimeout(() => {
                this.mInvalidated = 0;
                this.app.callLater(() => {
                    this.onUpdateUI();
                });
            }, 5);
        }
        onUpdateUI() {
            // for implementors..
        }
        runAfterInit(f) {
            if (this.pendingInits) {
                this.pendingInits.push(f);
            }
            else {
                f();
            }
        }
        registerDisposable(d) {
            return this.disposables.add(d);
        }
        render(node, e, creator) {
            creator = creator || this;
            const bridge = AtomBridge_1.AtomBridge.instance;
            const app = this.app;
            const renderFirst = AtomBridge_1.AtomBridge.platform === "xf";
            e = e || this.element;
            const attr = node.attributes;
            if (attr) {
                for (const key in attr) {
                    if (attr.hasOwnProperty(key)) {
                        const item = attr[key];
                        if (item instanceof Bind_1.default) {
                            item.setupFunction(key, item, this, e, creator);
                        }
                        else if (item instanceof XNode_1.default) {
                            // this is template..
                            if (item.isTemplate) {
                                this.setLocalValue(e, key, AtomBridge_1.AtomBridge.toTemplate(app, item, creator));
                            }
                            else {
                                const child = AtomBridge_1.AtomBridge.createNode(item, app);
                                this.setLocalValue(e, key, child.element);
                            }
                        }
                        else {
                            this.setLocalValue(e, key, item);
                        }
                    }
                }
            }
            for (const iterator of node.children) {
                if (typeof iterator === "string") {
                    e.appendChild(document.createTextNode(iterator));
                    continue;
                }
                if (iterator.isTemplate) {
                    if (iterator.isProperty) {
                        this.setLocalValue(e, iterator.name, AtomBridge_1.AtomBridge.toTemplate(app, iterator.children[0], creator));
                    }
                    else {
                        e.appendChild(AtomBridge_1.AtomBridge.toTemplate(app, iterator, creator));
                    }
                    continue;
                }
                if (iterator.isProperty) {
                    for (const child of iterator.children) {
                        const pc = AtomBridge_1.AtomBridge.createNode(child, app);
                        (pc.control || this).render(child, pc.element, creator);
                        // in Xamarin.Forms certain properties are required to be
                        // set in advance, so we append the element after setting
                        // all children properties
                        bridge.append(e, iterator.name, pc.element);
                    }
                    continue;
                }
                const t = iterator.attributes && iterator.attributes.template;
                if (t) {
                    this.setLocalValue(e, t, AtomBridge_1.AtomBridge.toTemplate(app, iterator, creator));
                    continue;
                }
                const c = AtomBridge_1.AtomBridge.createNode(iterator, app);
                if (renderFirst) {
                    (c.control || this).render(iterator, c.element, creator);
                }
                if (this.element === e) {
                    this.append(c.control || c.element);
                }
                else {
                    e.appendChild(c.element);
                }
                if (!renderFirst) {
                    (c.control || this).render(iterator, c.element, creator);
                }
            }
        }
        // tslint:disable-next-line:no-empty
        create() {
        }
        // tslint:disable-next-line:no-empty
        preCreate() {
        }
        setElementValue(element, name, value) {
            AtomBridge_1.AtomBridge.instance.setValue(element, name, value);
        }
        resolve(c, selfName) {
            const result = this.app.resolve(c, true);
            if (selfName) {
                if (typeof selfName === "function") {
                    // this is required as parent is not available
                    // in items control so binding becomes difficult
                    this.runAfterInit(() => {
                        const v = selfName();
                        if (v) {
                            for (const key in v) {
                                if (v.hasOwnProperty(key)) {
                                    const element = v[key];
                                    result[key] = element;
                                }
                            }
                        }
                    });
                }
                else {
                    result[selfName] = this;
                }
            }
            return result;
        }
    };
    AtomComponent.isControl = true;
    __decorate([
        InheritedProperty_1.InheritedProperty,
        __metadata("design:type", Object)
    ], AtomComponent.prototype, "data", void 0);
    __decorate([
        InheritedProperty_1.InheritedProperty,
        __metadata("design:type", Object)
    ], AtomComponent.prototype, "viewModel", void 0);
    __decorate([
        InheritedProperty_1.InheritedProperty,
        __metadata("design:type", Object)
    ], AtomComponent.prototype, "localViewModel", void 0);
    AtomComponent = __decorate([
        __param(0, Inject_1.Inject),
        __metadata("design:paramtypes", [App_1.App, Object])
    ], AtomComponent);
    exports.AtomComponent = AtomComponent;
});
//# sourceMappingURL=AtomComponent.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomComponent");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../web/core/AtomUI"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomUri = void 0;
    const AtomUI_1 = require("../web/core/AtomUI");
    class AtomUri {
        /**
         *
         */
        constructor(url) {
            let path;
            let query = "";
            let hash = "";
            let t = url.split("?");
            path = t[0];
            if (t.length === 2) {
                query = t[1] || "";
                t = query.split("#");
                query = t[0];
                hash = t[1] || "";
            }
            else {
                t = path.split("#");
                path = t[0];
                hash = t[1] || "";
            }
            // extract protocol and domain...
            let scheme = "";
            let host = "";
            let port = "";
            let i = path.indexOf("//");
            if (i !== -1) {
                scheme = path.substr(0, i);
                path = path.substr(i + 2);
                i = path.indexOf("/");
                if (i !== -1) {
                    host = path.substr(0, i);
                    path = path.substr(i + 1);
                    t = host.split(":");
                    if (t.length > 1) {
                        host = t[0];
                        port = t[1];
                    }
                }
            }
            this.host = host;
            this.protocol = scheme;
            this.port = port;
            this.path = path;
            this.query = AtomUI_1.AtomUI.parseUrl(query);
            this.hash = AtomUI_1.AtomUI.parseUrl(hash);
        }
        get pathAndQuery() {
            const q = [];
            const h = [];
            for (const key in this.query) {
                if (this.query.hasOwnProperty(key)) {
                    const element = this.query[key];
                    if (element === undefined || element === null) {
                        continue;
                    }
                    q.push(`${encodeURIComponent(key)}=${encodeURIComponent(element.toString())}`);
                }
            }
            for (const key in this.hash) {
                if (this.hash.hasOwnProperty(key)) {
                    const element = this.hash[key];
                    if (element === undefined || element === null) {
                        continue;
                    }
                    h.push(`${encodeURIComponent(key)}=${encodeURIComponent(element.toString())}`);
                }
            }
            const query = q.length ? "?" + q.join("&") : "";
            const hash = h.length ? "#" + h.join("&") : "";
            let path = this.path || "/";
            if (path.startsWith("/")) {
                path = path.substr(1);
            }
            return `${path}${query}${hash}`;
        }
        toString() {
            const port = this.port ? ":" + this.port : "";
            return `${this.protocol}//${this.host}${port}/${this.pathAndQuery}`;
        }
    }
    exports.AtomUri = AtomUri;
});
//# sourceMappingURL=AtomUri.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomUri");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    class FormattedString {
        constructor(text) {
            this.text = text;
        }
    }
    exports.default = FormattedString;
});
//# sourceMappingURL=FormattedString.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/FormattedString");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./Register", "./ServiceCollection"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Register_1 = require("./Register");
    const ServiceCollection_1 = require("./ServiceCollection");
    function DISingleton(mockOrInject) {
        return (target) => {
            Register_1.Register({ scope: ServiceCollection_1.Scope.Global, mockOrInject })(target);
        };
    }
    exports.default = DISingleton;
});
//# sourceMappingURL=DISingleton.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/di/DISingleton");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../di/DISingleton"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ObjectReference = void 0;
    const DISingleton_1 = require("../di/DISingleton");
    class ObjectReference {
        constructor(key, value) {
            this.key = key;
            this.value = value;
        }
    }
    exports.ObjectReference = ObjectReference;
    let ReferenceService = class ReferenceService {
        constructor() {
            this.cache = {};
            this.id = 1;
        }
        get(key) {
            return this.cache[key];
        }
        put(item, ttl = 60) {
            const key = `k${this.id++}`;
            const r = new ObjectReference(key, item);
            r.consume = () => {
                delete this.cache[key];
                if (r.timeout) {
                    clearTimeout(r.timeout);
                }
                return r.value;
            };
            r.timeout = setTimeout(() => {
                r.timeout = 0;
                r.consume();
            }, ttl * 1000);
            this.cache[key] = r;
            return r;
        }
    };
    ReferenceService = __decorate([
        DISingleton_1.default()
    ], ReferenceService);
    exports.default = ReferenceService;
});
//# sourceMappingURL=ReferenceService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/ReferenceService");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/AtomComponent", "../core/AtomUri", "../core/FormattedString", "../core/types", "./ReferenceService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.NavigationService = exports.NotifyType = void 0;
    const AtomComponent_1 = require("../core/AtomComponent");
    const AtomUri_1 = require("../core/AtomUri");
    const FormattedString_1 = require("../core/FormattedString");
    const types_1 = require("../core/types");
    const ReferenceService_1 = require("./ReferenceService");
    // export interface ILocation {
    //     href?: string;
    //     hash?: INameValues;
    //     host?: string;
    //     hostName?: string;
    //     port?: string;
    //     protocol?: string;
    // }
    var NotifyType;
    (function (NotifyType) {
        NotifyType["Information"] = "info";
        NotifyType["Warning"] = "warn";
        NotifyType["Error"] = "error";
    })(NotifyType = exports.NotifyType || (exports.NotifyType = {}));
    const nameSymbol = UMD.nameSymbol;
    function hasPageUrl(target) {
        const url = target[nameSymbol];
        if (!url) {
            return false;
        }
        const baseClass = Object.getPrototypeOf(target);
        if (!baseClass) {
            // this is not possible...
            return false;
        }
        return baseClass[nameSymbol] !== url;
    }
    class NavigationService {
        constructor(app) {
            this.app = app;
            this.callbacks = [];
        }
        /**
         *
         * @param pageName node style package url or a class
         * @param viewModelParameters key value pair that will be injected on ViewModel when created
         * @param options {@link IPageOptions}
         */
        openPage(pageName, viewModelParameters, options) {
            options = options || {};
            if (typeof pageName !== "string") {
                if (hasPageUrl(pageName)) {
                    pageName = pageName[nameSymbol];
                }
                else {
                    const rs = this.app.resolve(ReferenceService_1.default);
                    const host = pageName instanceof AtomComponent_1.AtomComponent ? "reference" : "class";
                    const r = rs.put(pageName);
                    pageName = `ref://${host}/${r.key}`;
                }
            }
            const url = new AtomUri_1.AtomUri(pageName);
            if (viewModelParameters) {
                for (const key in viewModelParameters) {
                    if (viewModelParameters.hasOwnProperty(key)) {
                        const element = viewModelParameters[key];
                        if (element === undefined) {
                            continue;
                        }
                        if (element === null) {
                            url.query["json:" + key] = "null";
                            continue;
                        }
                        if (key.startsWith("ref:") || element instanceof FormattedString_1.default) {
                            const r = element instanceof ReferenceService_1.ObjectReference ?
                                element :
                                this.app.resolve(ReferenceService_1.default).put(element);
                            url.query[key.startsWith("ref:") ? key : `ref:${key}`] =
                                r.key;
                            continue;
                        }
                        if (typeof element !== "string" &&
                            (typeof element === "object" || Array.isArray(element))) {
                            url.query["json:" + key] = JSON.stringify(element);
                        }
                        else {
                            url.query[key] = element;
                        }
                    }
                }
            }
            for (const iterator of this.callbacks) {
                const r = iterator(url, options);
                if (r) {
                    return r;
                }
            }
            return this.openWindow(url, options);
        }
        /**
         * Sends signal to remove window/popup/frame, it will not immediately remove, because
         * it will identify whether it can remove or not by displaying cancellation warning. Only
         * if there is no cancellation warning or user chooses to force close, it will not remove.
         * @param id id of an element
         * @returns true if view was removed successfully
         */
        remove(view, force) {
            return __awaiter(this, void 0, void 0, function* () {
                if (force) {
                    this.app.broadcast(`atom-window-cancel:${view.id}`, "cancelled");
                    return true;
                }
                const vm = view.viewModel;
                if (vm && vm.cancel) {
                    const a = yield vm.cancel();
                    return a;
                }
                this.app.broadcast(`atom-window-cancel:${view.id}`, "cancelled");
                return true;
            });
        }
        registerNavigationHook(callback) {
            this.callbacks.push(callback);
            return {
                dispose: () => {
                    types_1.ArrayHelper.remove(this.callbacks, (a) => a === callback);
                }
            };
        }
    }
    exports.NavigationService = NavigationService;
});
// Do not mock Navigation unless you want it in design time..
// Mock.mock(NavigationService, "MockNavigationService");
//# sourceMappingURL=NavigationService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/NavigationService");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.StringHelper = void 0;
    class StringHelper {
        static fromCamelToHyphen(input) {
            return input.replace(/([a-z])([A-Z])/g, "$1-$2").toLowerCase();
        }
        static fromCamelToUnderscore(input) {
            return input.replace(/([a-z])([A-Z])/g, "$1_$2").toLowerCase();
        }
        static fromCamelToPascal(input) {
            return input[0].toUpperCase() + input.substr(1);
        }
        static fromHyphenToCamel(input) {
            return input.replace(/-([a-z])/g, (g) => g[1].toUpperCase());
        }
        static fromUnderscoreToCamel(input) {
            return input.replace(/\_([a-z])/g, (g) => g[1].toUpperCase());
        }
        static fromPascalToCamel(input) {
            return input[0].toLowerCase() + input.substr(1);
        }
    }
    exports.StringHelper = StringHelper;
});
//# sourceMappingURL=StringHelper.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/StringHelper");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/StringHelper"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomStyle = void 0;
    const StringHelper_1 = require("../../core/StringHelper");
    const emptyPrototype = Object.getPrototypeOf({});
    class AtomStyle {
        constructor(styleSheet, name) {
            this.styleSheet = styleSheet;
            this.name = name;
            this.styleText = null;
            this.name = this.name + "-root";
        }
        getBaseProperty(tc, name) {
            let c = tc;
            do {
                c = Object.getPrototypeOf(c);
                if (!c) {
                    throw new Error("No property descriptor found for " + name);
                }
                const pd = Object.getOwnPropertyDescriptor(c.prototype, name);
                if (!pd) {
                    continue;
                }
                return pd.get.apply(this);
            } while (true);
        }
        build() {
            if (this.styleText) {
                return;
            }
            this.styleText = this.createStyleText("", [], this.root).join("\n");
        }
        toString() {
            return this.styleText;
        }
        createStyleText(name, pairs, styles) {
            const styleList = [];
            for (const key in styles) {
                if (styles.hasOwnProperty(key)) {
                    if (/^(\_\$\_|className$|toString$)/i.test(key)) {
                        continue;
                    }
                    const element = styles[key];
                    if (element === undefined || element === null) {
                        continue;
                    }
                    const keyName = StringHelper_1.StringHelper.fromCamelToHyphen(key);
                    if (key === "subclasses") {
                        const n = name;
                        for (const subclassKey in element) {
                            if (element.hasOwnProperty(subclassKey)) {
                                const ve = element[subclassKey];
                                pairs = this.createStyleText(`${n}${subclassKey}`, pairs, ve);
                            }
                        }
                    }
                    else {
                        if (element.url) {
                            styleList.push(`${keyName}: url(${element})`);
                        }
                        else {
                            styleList.push(`${keyName}: ${element}`);
                        }
                    }
                }
            }
            const cname = StringHelper_1.StringHelper.fromCamelToHyphen(name);
            const styleClassName = `${this.name}${cname}`;
            if (styleList.length) {
                pairs.push(`.${styleClassName} { ${styleList.join(";\r\n")}; }`);
            }
            return pairs;
        }
    }
    exports.AtomStyle = AtomStyle;
});
//# sourceMappingURL=AtomStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomStyle");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../di/TypeKey"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomStyleSheet = void 0;
    const TypeKey_1 = require("../../di/TypeKey");
    class AtomStyleSheet {
        constructor(app, name) {
            this.app = app;
            this.name = name;
            this.styles = {};
            this.lastUpdateId = 0;
            this.isAttaching = false;
            this.pushUpdate(0);
        }
        getNamedStyle(c) {
            const name = TypeKey_1.TypeKey.get(c);
            return this.createNamedStyle(c, name);
        }
        createNamedStyle(c, name, updateTimeout) {
            const style = this.styles[name] = new (c)(this, `${this.name}-${name}`);
            style.build();
            this.pushUpdate(updateTimeout);
            return style;
        }
        onPropertyChanging(name, newValue, oldValue) {
            this.pushUpdate();
        }
        pushUpdate(delay = 1) {
            if (this.isAttaching) {
                return;
            }
            // special case for Xamarin Forms
            if (delay === 0) {
                this.attach();
                return;
            }
            if (this.lastUpdateId) {
                clearTimeout(this.lastUpdateId);
            }
            this.lastUpdateId = setTimeout(() => {
                this.attach();
            }, delay);
        }
        dispose() {
            if (this.styleElement) {
                this.styleElement.remove();
            }
        }
        attach() {
            this.isAttaching = true;
            const text = [];
            for (const key in this.styles) {
                if (this.styles.hasOwnProperty(key)) {
                    const element = this.styles[key];
                    text.push(element.toString());
                }
            }
            const textContent = text.join("\n");
            this.app.updateDefaultStyle(textContent);
            this.isAttaching = false;
        }
    }
    exports.AtomStyleSheet = AtomStyleSheet;
});
//# sourceMappingURL=AtomStyleSheet.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomStyleSheet");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/AtomBinder", "../../core/AtomBridge", "../../core/AtomComponent", "../../di/TypeKey", "../../services/NavigationService", "../../web/styles/AtomStyle", "../../web/styles/AtomStyleSheet"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomXFControl = void 0;
    const AtomBinder_1 = require("../../core/AtomBinder");
    const AtomBridge_1 = require("../../core/AtomBridge");
    const AtomComponent_1 = require("../../core/AtomComponent");
    const TypeKey_1 = require("../../di/TypeKey");
    const NavigationService_1 = require("../../services/NavigationService");
    const AtomStyle_1 = require("../../web/styles/AtomStyle");
    const AtomStyleSheet_1 = require("../../web/styles/AtomStyleSheet");
    UMD.defaultApp = "@web-atoms/core/dist/xf/XFApp";
    UMD.viewPrefix = "xf";
    AtomBridge_1.AtomBridge.platform = "xf";
    const defaultStyleSheets = {};
    class AtomXFControl extends AtomComponent_1.AtomComponent {
        get controlStyle() {
            if (this.mControlStyle === undefined) {
                const key = TypeKey_1.TypeKey.get(this.defaultControlStyle || this.constructor);
                this.mControlStyle = defaultStyleSheets[key];
                if (this.mControlStyle) {
                    return this.mControlStyle;
                }
                if (this.defaultControlStyle) {
                    this.mControlStyle = defaultStyleSheets[key] ||
                        (defaultStyleSheets[key] = this.theme.createNamedStyle(this.defaultControlStyle, key, 0));
                }
                this.mControlStyle = this.mControlStyle || null;
            }
            return this.mControlStyle;
        }
        set controlStyle(v) {
            if (v instanceof AtomStyle_1.AtomStyle) {
                this.mControlStyle = v;
            }
            else {
                const key = TypeKey_1.TypeKey.get(v);
                this.mControlStyle = defaultStyleSheets[key] ||
                    (defaultStyleSheets[key] = this.theme.createNamedStyle(v, key, 0));
            }
            AtomBinder_1.AtomBinder.refreshValue(this, "controlStyle");
            // this.invalidate();
        }
        get theme() {
            return this.mTheme ||
                this.mCachedTheme ||
                (this.mCachedTheme = (this.parent ? this.parent.theme : this.app.resolve(AtomStyleSheet_1.AtomStyleSheet, false, null)));
        }
        get parent() {
            return AtomBridge_1.AtomBridge.instance.atomParent(this.element, true);
        }
        atomParent(e) {
            return AtomBridge_1.AtomBridge.instance.atomParent(e, false);
        }
        append(element) {
            this.element.appendChild(element.element || element);
            return this;
        }
        dispose(e) {
            const el = this.element;
            super.dispose(e);
            AtomBridge_1.AtomBridge.instance.dispose(el);
        }
        invokeEvent(event) {
            AtomBridge_1.AtomBridge.instance.invokeEvent(this.element, event.type, event);
        }
        staticResource(name) {
            return AtomBridge_1.AtomBridge.instance.getStaticResource(this.element, name);
        }
        loadXaml(text) {
            AtomBridge_1.AtomBridge.instance.loadXaml(this.element, text);
        }
        setElementValue(element, name, value) {
            if (/^event/.test(name)) {
                this.bindEvent(element, name.substr(5), () => __awaiter(this, void 0, void 0, function* () {
                    try {
                        const p = value();
                        if (p) {
                            yield p;
                        }
                    }
                    catch (e) {
                        if (/canceled|cancelled/i.test(e)) {
                            return;
                        }
                        const nav = this.app.resolve(NavigationService_1.NavigationService);
                        nav.alert(e, "Error").catch(() => {
                            // nothing...
                        });
                    }
                }));
                return;
            }
            if (/^(class|styleClass)$/i.test(name)) {
                let classes;
                if (typeof value === "object") {
                    classes = [];
                    for (const key in value) {
                        if (value.hasOwnProperty(key)) {
                            const e1 = value[key];
                            if (e1) {
                                classes.push(key);
                            }
                        }
                    }
                }
                else {
                    classes = value.toString().split(" ");
                }
                value = classes.join(",");
            }
            AtomBridge_1.AtomBridge.instance.setValue(element, name, value);
        }
    }
    exports.AtomXFControl = AtomXFControl;
    bridge.controlFactory = AtomXFControl;
    global.CustomEvent = function (type, { detail }) {
        this.type = type;
        this.detail = detail;
    };
});
//# sourceMappingURL=AtomXFControl.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/xf/controls/AtomXFControl");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const XF_1 = require("../clr/XF");
    class AtomXFMasterDetailPage extends AtomXFControl_1.AtomXFControl {
        constructor(app, e) {
            super(app, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.MasterDetailPage));
        }
    }
    exports.default = AtomXFMasterDetailPage;
});
//# sourceMappingURL=AtomXFMasterDetailPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/pages/AtomXFMasterDetailPage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.msDays = exports.msHours = exports.msSeconds = exports.msMinutes = void 0;
    function isEmpty(n) {
        return n === undefined || n === null || n === 0 || isNaN(n);
    }
    exports.msMinutes = 60000;
    exports.msSeconds = 1000;
    exports.msHours = 3600000;
    exports.msDays = 24 * exports.msHours;
    /**
     * This is due to performance reason, copied from Source of TimeSpan from C# code.
     */
    var daysPerMS = 1 / exports.msDays;
    var hoursPerMS = 1 / exports.msHours;
    var minutesPerMS = 1 / exports.msMinutes;
    var secondsPerMS = 1 / exports.msSeconds;
    function padLeft(n, c, t) {
        if (c === void 0) { c = 2; }
        if (t === void 0) { t = "0"; }
        var s = n.toString();
        if (s.length < c) {
            s = t + s;
        }
        return s;
    }
    var TimeSpan = /** @class */ (function () {
        function TimeSpan(days, hours, minutes, seconds, milliseconds) {
            if (arguments.length === 1) {
                this.msSinceEpoch = days;
            }
            else {
                this.msSinceEpoch =
                    (days || 0) * exports.msDays +
                        (hours || 0) * exports.msHours +
                        (minutes || 0) * exports.msMinutes +
                        (seconds || 0) * exports.msSeconds +
                        (milliseconds || 0);
            }
        }
        TimeSpan.fromDays = function (n) {
            return new TimeSpan(n * exports.msDays);
        };
        TimeSpan.fromHours = function (n) {
            return new TimeSpan(n * exports.msHours);
        };
        TimeSpan.fromMinutes = function (n) {
            return new TimeSpan(n * exports.msMinutes);
        };
        TimeSpan.fromSeconds = function (n) {
            return new TimeSpan(n * exports.msSeconds);
        };
        TimeSpan.parse = function (text) {
            if (!text) {
                throw new Error("Invalid time format");
            }
            var isPM = false;
            // tslint:disable-next-line: one-variable-per-declaration
            var d, h, m, s, ms;
            var tokens = text.split(/:/);
            // split last...
            var last = tokens[tokens.length - 1];
            var lastParts = last.split(" ");
            if (lastParts.length > 1) {
                if (/pm/i.test(lastParts[1])) {
                    isPM = true;
                }
                tokens[tokens.length - 1] = lastParts[0];
            }
            var firstOfLast = lastParts[0];
            if (firstOfLast.indexOf(".") !== -1) {
                // it has ms...
                var secondParts = firstOfLast.split(".");
                if (secondParts.length > 1) {
                    tokens[tokens.length - 1] = secondParts[0];
                    ms = parseInt(secondParts[1], 10);
                }
            }
            if (tokens.length === 2) {
                // this is hour:min
                d = 0;
                h = parseInt(tokens[0], 10);
                m = parseInt(tokens[1], 10);
            }
            else if (tokens.length === 3) {
                d = 0;
                h = parseInt(tokens[0], 10);
                m = parseInt(tokens[1], 10);
                s = parseInt(tokens[2], 10);
            }
            else if (tokens.length === 4) {
                d = parseInt(tokens[0], 10);
                h = parseInt(tokens[1], 10);
                m = parseInt(tokens[2], 10);
                s = parseInt(tokens[3], 10);
            }
            return new TimeSpan(d, isPM ? h + 12 : h, m, s, ms);
        };
        Object.defineProperty(TimeSpan.prototype, "totalSeconds", {
            get: function () {
                return this.msSinceEpoch * secondsPerMS;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "totalMinutes", {
            get: function () {
                return this.msSinceEpoch * minutesPerMS;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "totalHours", {
            get: function () {
                return this.msSinceEpoch * hoursPerMS;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "totalDays", {
            get: function () {
                return this.msSinceEpoch * daysPerMS;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "totalMilliseconds", {
            get: function () {
                return this.msSinceEpoch;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "days", {
            get: function () {
                return Math.floor(this.msSinceEpoch / exports.msDays);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "hours", {
            get: function () {
                return Math.floor((this.msSinceEpoch / exports.msHours) % 24);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "minutes", {
            get: function () {
                return Math.floor((this.msSinceEpoch / exports.msMinutes) % 60);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "seconds", {
            get: function () {
                return Math.floor((this.msSinceEpoch / exports.msSeconds) % 60);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "milliseconds", {
            get: function () {
                return Math.floor(this.msSinceEpoch % 1000);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "duration", {
            /**
             * Duration is always positive TimeSpan
             */
            get: function () {
                var t = this.msSinceEpoch;
                return new TimeSpan(t > 0 ? t : -t);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(TimeSpan.prototype, "trimmedTime", {
            /**
             * Removes days and only trims given TimeSpan to TimeOfDay
             */
            get: function () {
                return new TimeSpan(Math.ceil(this.msSinceEpoch % exports.msDays));
            },
            enumerable: false,
            configurable: true
        });
        /**
         * Format the TimeSpan as time format
         * @param formatAs12 Display time as 12 hours with AM/PM (only if day is zero)
         */
        TimeSpan.prototype.toString = function (formatAs12) {
            if (formatAs12 === void 0) { formatAs12 = false; }
            var ams = this.msSinceEpoch;
            var text = [];
            var postFix = "";
            function format(max, f12) {
                if (f12 === void 0) { f12 = false; }
                var txt = null;
                if (ams > max) {
                    var n = Math.floor(ams / max);
                    ams = ams % max;
                    if (f12) {
                        if (n > 12) {
                            postFix = " PM";
                            txt = padLeft(n - 12);
                        }
                        else {
                            postFix = " AM";
                        }
                    }
                    if (!txt) {
                        txt = padLeft(n);
                    }
                }
                if (txt) {
                    text.push(txt);
                }
                return txt;
            }
            var d = format(exports.msDays);
            format(exports.msHours, formatAs12 && !d);
            format(exports.msMinutes);
            var s = format(exports.msSeconds);
            if (ams) {
                s += "." + ams;
                text[text.length - 1] = s;
            }
            return "" + text.join(":") + postFix;
        };
        TimeSpan.prototype.add = function (ts) {
            return new TimeSpan(this.msSinceEpoch + ts.msSinceEpoch);
        };
        TimeSpan.prototype.equals = function (ts) {
            return ts.msSinceEpoch === this.msSinceEpoch;
        };
        return TimeSpan;
    }());
    exports.default = TimeSpan;
    if (typeof window !== "undefined") {
        window.TimeSpan = TimeSpan;
    }
});
//# sourceMappingURL=TimeSpan.js.map

    AmdLoader.instance.setup("@web-atoms/date-time/dist/TimeSpan");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./TimeSpan"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var TimeSpan_1 = require("./TimeSpan");
    /**
     * DateTime differs from Date in following cases,
     * 1. DateTime is immutable, however underlying object is Date
     *    but all methods specific to DateTime are immutable
     * 2. DateTime has readonly properties for `day, month, year etc`
     * 3. DateTime is derived from Date so passing DateTime to existing
     *    code will not change anything, however intellisense does not display
     *    any methods of Date unless you explicity cast as Date, but instanceof
     *    works correctly
     * 4. DateTime does not modify underlying Date prototype or add any methods to it
     * ``` typescript
     * DateTime dt = DateTime.now();
     * (dt instanceof Date) // is true
     * (dt instanceof DateTime) // is also true
     * ```
     */
    var DateTime = /** @class */ (function () {
        function DateTime(a, b, c, d, e, f, g) {
            // super();
            // tslint:disable-next-line: no-string-literal
            this["__proto__"] = DateTime.prototype;
            var rd;
            switch (arguments.length) {
                case 0:
                    rd = new Date();
                    break;
                case 1:
                    rd = new Date(a);
                    break;
                case 2:
                    rd = new Date(a, b);
                    break;
                case 3:
                    rd = new Date(a, b, c);
                    break;
                case 4:
                    rd = new Date(a, b, c, d);
                    break;
                case 5:
                    rd = new Date(a, b, c, d, e);
                    break;
                case 6:
                    rd = new Date(a, b, c, d, e, f);
                    break;
                default:
                    rd = new Date(a, b, c, d, e, f, g);
            }
            rd.__proto__ = DateTime.prototype;
            return rd;
        }
        DateTime.from = function (d) {
            if (!(d instanceof DateTime)) {
                d = new DateTime(d.getTime());
            }
            return d;
        };
        Object.defineProperty(DateTime, "today", {
            /**
             * Current date without time
             */
            get: function () {
                var a = new DateTime();
                return a.date;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime, "utcNow", {
            /**
             * Current UTC Date
             */
            get: function () {
                var now = new Date();
                return new DateTime(now.getTime() + now.getTimezoneOffset());
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime, "now", {
            /**
             * DateTime at right now
             */
            get: function () {
                return new DateTime();
            },
            enumerable: false,
            configurable: true
        });
        DateTime.parse = function (s) {
            return new DateTime(s);
        };
        Object.defineProperty(DateTime.prototype, "day", {
            /** Day of month */
            get: function () {
                return this.getDate();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "dayOfWeek", {
            /** Day of week */
            get: function () {
                return this.getDay();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "month", {
            get: function () {
                return this.getMonth();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "year", {
            get: function () {
                return this.getFullYear();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "hour", {
            get: function () {
                return this.getHours();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "minute", {
            get: function () {
                return this.getMinutes();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "second", {
            get: function () {
                return this.getSeconds();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "milliSecond", {
            get: function () {
                return this.getMilliseconds();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "timeZoneOffset", {
            /**
             * Timezone offset as TimeSpan
             */
            get: function () {
                return TimeSpan_1.default.fromMinutes(this.getTimezoneOffset());
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "msSinceEpoch", {
            /**
             * Milliseconds since EPOCH, ie total number of milliseconds
             * of underlying Date object
             */
            get: function () {
                return this.getTime();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "date", {
            /**
             * Strips time of the day and returns Date only
             */
            get: function () {
                var d = new DateTime(this.getFullYear(), this.getMonth(), this.getDate(), 0, 0, 0, 0);
                return d;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "asJSDate", {
            /**
             * Just for convenience, avoid using this, instead use methods of DateTime
             * or suggest better method at our github repo
             */
            get: function () {
                return this;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTime.prototype, "time", {
            /**
             * Gets time of the day in TimeSpan format
             */
            get: function () {
                return new TimeSpan_1.default(0, this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
            },
            enumerable: false,
            configurable: true
        });
        DateTime.prototype.add = function (t, hours, minutes, seconds, milliseconds) {
            if (t instanceof Date) {
                return new DateTime(this.getTime() + t.getTime());
            }
            var days = 0;
            if (t instanceof TimeSpan_1.default) {
                days = t.days;
                hours = t.hours;
                minutes = t.minutes;
                seconds = t.seconds;
                milliseconds = t.milliseconds;
            }
            else {
                days = t;
            }
            function hasValue(n, name) {
                if (n === undefined) {
                    return false;
                }
                if (n === null) {
                    throw new Error(name + " cannot be null");
                }
                return n !== 0;
            }
            var d = new Date(this.getTime());
            if (hasValue(days, "days")) {
                d.setDate(d.getDate() + days);
            }
            if (hasValue(hours, "hours")) {
                d.setHours(d.getHours() + hours);
            }
            if (hasValue(minutes, "minutes")) {
                d.setMinutes(d.getMinutes() + minutes);
            }
            if (hasValue(seconds, "seconds")) {
                d.setSeconds(d.getSeconds() + seconds);
            }
            if (hasValue(milliseconds, "milliseconds")) {
                d.setMilliseconds(d.getMilliseconds() + milliseconds);
            }
            d.__proto__ = DateTime.prototype;
            return d;
        };
        DateTime.prototype.addMonths = function (m) {
            var d = new Date(this.msSinceEpoch);
            d.setMonth(d.getMonth() + m);
            d.__proto__ = DateTime.prototype;
            return d;
        };
        DateTime.prototype.addYears = function (y) {
            var d = new Date(this.msSinceEpoch);
            d.setFullYear(d.getFullYear() + y);
            d.__proto__ = DateTime.prototype;
            return d;
        };
        /**
         * Returns TimeSpan from subtracting rhs from this,
         * `const ts = lhs.diff(rhs); // ts = lhs - rhs`
         * @param rhs Right hand side
         * @returns TimeSpan
         */
        DateTime.prototype.diff = function (rhs) {
            return new TimeSpan_1.default(this.getTime() - rhs.getTime());
        };
        DateTime.prototype.equals = function (d) {
            if (!d) {
                return false;
            }
            return this.getTime() === d.getTime();
        };
        /**
         * Trims time part and compares the given dates
         * @param d date to test
         */
        DateTime.prototype.dateEquals = function (d) {
            if (!d) {
                return false;
            }
            return this.date.equals(DateTime.from(d));
        };
        DateTime.prototype.toRelativeString = function (dt) {
            if (!dt) {
                dt = DateTime.now;
            }
            else {
                if (dt instanceof Date && !(dt instanceof DateTime)) {
                    dt.__proto__ = DateTime.prototype;
                    dt = dt;
                }
            }
            var diff = this.diff(dt);
            if (dt.year !== this.year) {
                return this.toLocaleDateString();
            }
            if (Math.abs(diff.totalDays) > 6) {
                return this.toLocaleDateString(undefined, { month: "short", day: "numeric" });
            }
            if (Math.abs(diff.totalHours) > 23) {
                return this.toLocaleDateString(undefined, { weekday: "short" });
            }
            if (Math.abs(diff.totalMinutes) > 59) {
                return Math.floor(diff.totalHours) + " hours";
            }
            return Math.floor(diff.totalMinutes) + " mins";
        };
        return DateTime;
    }());
    exports.default = DateTime;
    // hack !! for ES5
    DateTime.prototype.__proto__ = Date.prototype;
    if (typeof window !== "undefined") {
        window.DateTime = DateTime;
    }
});
//# sourceMappingURL=DateTime.js.map

    AmdLoader.instance.setup("@web-atoms/date-time/dist/DateTime");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/StringHelper", "../di/RegisterSingleton", "@web-atoms/date-time/dist/DateTime"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.JsonService = exports.dateFormatMSRegEx = exports.dateFormatISORegEx = void 0;
    const StringHelper_1 = require("../core/StringHelper");
    const RegisterSingleton_1 = require("../di/RegisterSingleton");
    const DateTime_1 = require("@web-atoms/date-time/dist/DateTime");
    exports.dateFormatISORegEx = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/;
    exports.dateFormatMSRegEx = /^\/Date\((d|-|.*)\)[\/|\\]$/;
    const timeZoneDiff = (new Date()).getTimezoneOffset();
    let JsonService = class JsonService {
        constructor() {
            this.options = {
                indent: 2,
                namingStrategy: "none",
                dateConverter: [
                    {
                        regex: exports.dateFormatISORegEx,
                        valueConverter: {
                            fromSource(v) {
                                const d = new DateTime_1.default(v);
                                // if (/z$/i.test(v)) {
                                //     d.setMinutes( d.getMinutes() - timeZoneDiff );
                                // }
                                return d;
                            },
                            fromTarget(v) {
                                return v.toISOString();
                            }
                        }
                    }, {
                        regex: exports.dateFormatMSRegEx,
                        valueConverter: {
                            fromSource(v) {
                                const a = exports.dateFormatMSRegEx.exec(v);
                                const b = a[1].split(/[-+,.]/);
                                return new DateTime_1.default(b[0] ? +b[0] : 0 - +b[1]);
                            },
                            fromTarget(v) {
                                return v.toISOString();
                            }
                        }
                    }
                ]
            };
        }
        transformKeys(t, v) {
            if (!v) {
                return v;
            }
            if (typeof v !== "object") {
                return v;
            }
            if (v instanceof Date) {
                return v;
            }
            if (typeof v === "object" && v.length !== undefined && typeof v.length === "number") {
                const a = v;
                if (a.map) {
                    return a.map((x) => this.transformKeys(t, x));
                }
                const ra = [];
                // tslint:disable-next-line: prefer-for-of
                for (let i = 0; i < a.length; i++) {
                    const iterator = a[i];
                    ra.push(this.transformKeys(t, iterator));
                }
                return ra;
            }
            const r = {};
            for (const key in v) {
                if (v.hasOwnProperty(key)) {
                    const element = v[key];
                    r[t(key)] = this.transformKeys(t, element);
                }
            }
            return r;
        }
        parse(text, options) {
            const { dateConverter, namingStrategy } = Object.assign(Object.assign({}, this.options), options);
            const result = JSON.parse(text, (key, value) => {
                // transform date...
                if (typeof value === "string") {
                    for (const iterator of dateConverter) {
                        const a = iterator.regex.test(value);
                        if (a) {
                            const dv = iterator.valueConverter.fromSource(value);
                            return dv;
                        }
                    }
                }
                return value;
            });
            switch (namingStrategy) {
                case "hyphen":
                    return this.transformKeys(StringHelper_1.StringHelper.fromHyphenToCamel, result);
                case "underscore":
                    return this.transformKeys(StringHelper_1.StringHelper.fromUnderscoreToCamel, result);
                case "pascal":
                    return this.transformKeys(StringHelper_1.StringHelper.fromPascalToCamel, result);
            }
            return result;
        }
        stringify(v, options) {
            const { namingStrategy, dateConverter, indent } = Object.assign(Object.assign({}, this.options), options);
            switch (namingStrategy) {
                case "hyphen":
                    v = this.transformKeys(StringHelper_1.StringHelper.fromCamelToHyphen, v);
                    break;
                case "underscore":
                    v = this.transformKeys(StringHelper_1.StringHelper.fromCamelToUnderscore, v);
                    break;
                case "pascal":
                    v = this.transformKeys(StringHelper_1.StringHelper.fromCamelToPascal, v);
                    break;
            }
            return JSON.stringify(v, (key, value) => {
                if (key && /^\_\$\_/.test(key)) {
                    return undefined;
                }
                if (dateConverter && (value instanceof Date)) {
                    return dateConverter[0].valueConverter.fromTarget(value);
                }
                return value;
            }, indent);
        }
    };
    JsonService = __decorate([
        RegisterSingleton_1.RegisterSingleton
    ], JsonService);
    exports.JsonService = JsonService;
});
//# sourceMappingURL=JsonService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/JsonService");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../services/JsonService", "../services/ReferenceService", "./types"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomLoader = void 0;
    const JsonService_1 = require("../services/JsonService");
    const ReferenceService_1 = require("../services/ReferenceService");
    const types_1 = require("./types");
    class AtomLoader {
        static load(url, app) {
            return __awaiter(this, void 0, void 0, function* () {
                if (url.host === "reference") {
                    const r = app.get(ReferenceService_1.default).get(url.path);
                    if (!r) {
                        throw new Error("reference not found");
                    }
                    return r.consume();
                }
                if (url.host === "class") {
                    const r = app.get(ReferenceService_1.default).get(url.path);
                    if (!r) {
                        throw new Error("reference not found");
                    }
                    return app.resolve(r.consume(), true);
                }
                const type = yield types_1.DI.resolveViewClassAsync(url.path);
                if (!type) {
                    throw new Error(`Type not found for ${url}`);
                }
                const obj = app.resolve(type, true);
                return obj;
            });
        }
        static loadView(url, app, hookCloseEvents, vmFactory) {
            return __awaiter(this, void 0, void 0, function* () {
                const busyIndicator = app.createBusyIndicator();
                try {
                    const view = yield AtomLoader.load(url, app);
                    let vm = view.viewModel;
                    if (!vm) {
                        if (!vmFactory) {
                            return { view };
                        }
                        vm = vmFactory();
                        view.viewModel = vm;
                    }
                    if (vm) {
                        const jsonService = app.get(JsonService_1.JsonService);
                        for (const key in url.query) {
                            if (url.query.hasOwnProperty(key)) {
                                const element = url.query[key];
                                if (typeof element === "object") {
                                    vm[key] = jsonService.parse(jsonService.stringify(element));
                                    continue;
                                }
                                if (/^json\:/.test(key)) {
                                    const k = key.split(":")[1];
                                    vm[k] = jsonService.parse(element.toString());
                                    continue;
                                }
                                if (/^ref\:/.test(key)) {
                                    const rs = app.get(ReferenceService_1.default);
                                    const v = rs.get(element);
                                    vm[key.split(":", 2)[1]] = v.consume();
                                    continue;
                                }
                                try {
                                    vm[key] = element;
                                }
                                catch (e) {
                                    // tslint:disable-next-line: no-console
                                    console.error(e);
                                }
                            }
                        }
                    }
                    // register hooks !! if it is a window !!
                    if (hookCloseEvents && vm) {
                        const disposables = view.disposables;
                        const id = (AtomLoader.id++).toString();
                        view.id = id;
                        const returnPromise = new Promise((resolve, reject) => {
                            disposables.add(app.subscribe(`atom-window-close:${id}`, (m, r) => {
                                resolve(r);
                                view.dispose();
                            }));
                            disposables.add(app.subscribe(`atom-window-cancel:${id}`, () => {
                                reject("cancelled");
                                view.dispose();
                            }));
                        });
                        // it is responsibility of view holder to dispose the view
                        // disposables.add((view as any));
                        vm.windowName = id;
                        view.returnPromise = returnPromise;
                        return { view, disposables, returnPromise, id };
                    }
                    return { view };
                }
                finally {
                    busyIndicator.dispose();
                }
            });
        }
    }
    exports.AtomLoader = AtomLoader;
    AtomLoader.id = 1;
});
//# sourceMappingURL=AtomLoader.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomLoader");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomBinder"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.BindableProperty = void 0;
    const AtomBinder_1 = require("./AtomBinder");
    /**
     * Use this decorator only to watch property changes in `onPropertyChanged` method.
     * This decorator also makes enumerable property.
     *
     * Do not use this on anything except UI control
     * @param target control
     * @param key name of property
     */
    function BindableProperty(target, key) {
        // property value
        const iVal = target[key];
        const keyName = "_" + key;
        target[keyName] = iVal;
        // property getter
        const getter = function () {
            // console.log(`Get: ${key} => ${_val}`);
            return this[keyName];
        };
        // property setter
        const setter = function (newVal) {
            // console.log(`Set: ${key} => ${newVal}`);
            const oldValue = this[keyName];
            // tslint:disable-next-line:triple-equals
            if (oldValue === undefined ? oldValue === newVal : oldValue == newVal) {
                return;
            }
            const ce = this;
            if (ce.onPropertyChanging) {
                ce.onPropertyChanging(key, oldValue, newVal);
            }
            this[keyName] = newVal;
            AtomBinder_1.AtomBinder.refreshValue(this, key);
        };
        // delete property
        if (delete target[key]) {
            // create new property with getter and setter
            Object.defineProperty(target, key, {
                get: getter,
                set: setter,
                enumerable: true,
                configurable: true
            });
        }
    }
    exports.BindableProperty = BindableProperty;
});
//# sourceMappingURL=BindableProperty.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/BindableProperty");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.registerInit = void 0;
    function registerInit(target, fx) {
        const t = target;
        const inits = t._$_inits = t._$_inits || [];
        inits.push(fx);
    }
    exports.registerInit = registerInit;
});
//# sourceMappingURL=baseTypes.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/baseTypes");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../App", "../Atom", "../core/AtomBinder", "../core/AtomDisposableList", "../core/AtomWatcher", "../core/BindableProperty", "../di/Inject", "./baseTypes"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.Validate = exports.CachedWatch = exports.Watch = exports.BindableBroadcast = exports.BindableReceive = exports.Receive = exports.AtomViewModel = exports.waitForReady = void 0;
    const App_1 = require("../App");
    const Atom_1 = require("../Atom");
    const AtomBinder_1 = require("../core/AtomBinder");
    const AtomDisposableList_1 = require("../core/AtomDisposableList");
    const AtomWatcher_1 = require("../core/AtomWatcher");
    const BindableProperty_1 = require("../core/BindableProperty");
    const Inject_1 = require("../di/Inject");
    const baseTypes_1 = require("./baseTypes");
    function runDecoratorInits() {
        const v = this.constructor.prototype;
        if (!v) {
            return;
        }
        const ris = v._$_inits;
        if (ris) {
            for (const ri of ris) {
                ri.call(this, this);
            }
        }
    }
    function privateInit() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                yield Atom_1.Atom.postAsync(this.app, () => __awaiter(this, void 0, void 0, function* () {
                    runDecoratorInits.apply(this);
                    // this.registerWatchers();
                }));
                yield Atom_1.Atom.postAsync(this.app, () => __awaiter(this, void 0, void 0, function* () {
                    yield this.init();
                    this.onReady();
                }));
                if (this.postInit) {
                    for (const i of this.postInit) {
                        i();
                    }
                    this.postInit = null;
                }
            }
            finally {
                const pi = this.pendingInits;
                this.pendingInits = null;
                for (const iterator of pi) {
                    iterator();
                }
            }
        });
    }
    /**
     * Useful only for Unit testing, this function will await till initialization is
     * complete and all pending functions are executed
     */
    function waitForReady(vm) {
        return __awaiter(this, void 0, void 0, function* () {
            while (vm.pendingInits) {
                yield Atom_1.Atom.delay(100);
            }
        });
    }
    exports.waitForReady = waitForReady;
    /**
     * ViewModel class supports initialization and supports {@link IDisposable} dispose pattern.
     * @export
     * @class AtomViewModel
     */
    let AtomViewModel = class AtomViewModel {
        constructor(app) {
            this.app = app;
            this.disposables = null;
            this.validations = [];
            this.pendingInits = [];
            this.mShouldValidate = false;
            this.app.runAsync(() => privateInit.apply(this));
        }
        /**
         * If it returns true, it means all pending initializations have finished
         */
        get isReady() {
            return this.pendingInits === null;
        }
        get errors() {
            const e = [];
            if (!this.mShouldValidate) {
                return e;
            }
            for (const v of this.validations) {
                if (!v.initialized) {
                    return e;
                }
                const error = this[v.name];
                if (error) {
                    e.push({ name: v.name, error });
                }
            }
            return e;
        }
        /**
         * Returns parent AtomViewModel if it was initialized with one. This property is also
         * useful when you open an popup or window. Whenever a popup/window is opened, ViewModel
         * associated with the UI element that opened this popup/window becomes parent of ViewModel
         * of popup/window.
         */
        get parent() {
            return this.mParent;
        }
        set parent(v) {
            if (this.mParent && this.mParent.mChildren) {
                this.mParent.mChildren.remove(this);
            }
            this.mParent = v;
            if (v) {
                const c = v.mChildren || (v.mChildren = []);
                c.add(this);
                this.registerDisposable({
                    dispose: () => {
                        c.remove(this);
                    }
                });
            }
        }
        /**
         * Returns true if all validations didn't return any error. All validations
         * are decorated with @{@link Validate} decorator.
         */
        get isValid() {
            let valid = true;
            const validateWasFalse = this.mShouldValidate === false;
            this.mShouldValidate = true;
            for (const v of this.validations) {
                if (!v.initialized) {
                    v.watcher.init(true);
                    v.initialized = true;
                }
                if (this[v.name]) {
                    if (validateWasFalse) {
                        AtomBinder_1.AtomBinder.refreshValue(this, v.name);
                    }
                    valid = false;
                }
            }
            if (this.mChildren) {
                for (const child of this.mChildren) {
                    if (!child.isValid) {
                        valid = false;
                    }
                }
            }
            AtomBinder_1.AtomBinder.refreshValue(this, "errors");
            return valid;
        }
        /**
         * Resets validations and all errors are removed.
         * @param resetChildren reset child view models as well. Default is true.
         */
        resetValidations(resetChildren = true) {
            this.mShouldValidate = false;
            for (const v of this.validations) {
                this.refresh(v.name);
            }
            if (resetChildren && this.mChildren) {
                for (const iterator of this.mChildren) {
                    iterator.resetValidations(resetChildren);
                }
            }
        }
        /**
         * Runs function after initialization is complete.
         * @param f function to execute
         */
        runAfterInit(f) {
            if (this.pendingInits) {
                this.pendingInits.push(f);
                return;
            }
            f();
        }
        // /**
        //  * Binds source property to target property with optional two ways
        //  * @param target target whose property will be set
        //  * @param propertyName name of target property
        //  * @param source source to read property from
        //  * @param path property path of source
        //  * @param twoWays optional, two ways {@link IValueConverter}
        //  */
        // public bind(
        //     target: any,
        //     propertyName: string,
        //     source: any,
        //     path: string[][],
        //     twoWays?: IValueConverter | ((v: any) => any) ): IDisposable {
        //     const pb = new PropertyBinding(
        //         target,
        //         null,
        //         propertyName,
        //         path,
        //         (twoWays && typeof twoWays !== "function") ? true : false , twoWays, source);
        //     return this.registerDisposable(pb);
        // }
        /**
         * Refreshes bindings associated with given property name
         * @param name name of property
         */
        refresh(name) {
            AtomBinder_1.AtomBinder.refreshValue(this, name);
        }
        /**
         * Put your asynchronous initialization here
         *
         * @returns {Promise<any>}
         * @memberof AtomViewModel
         */
        // tslint:disable-next-line:no-empty
        init() {
            return __awaiter(this, void 0, void 0, function* () {
            });
        }
        /**
         * dispose method will be called when attached view will be disposed or
         * when a new view model will be assigned to view, old view model will be disposed.
         *
         * @memberof AtomViewModel
         */
        dispose() {
            if (this.disposables) {
                this.disposables.dispose();
            }
        }
        // /**
        //  * Internal method, do not use, instead use errors.hasErrors()
        //  *
        //  * @memberof AtomViewModel
        //  */
        // public runValidation(): void {
        //     for (const v of this.validations) {
        //         v.watcher.evaluate(true);
        //     }
        // }
        /**
         * Register a disposable to be disposed when view model will be disposed.
         *
         * @protected
         * @param {IDisposable} d
         * @memberof AtomViewModel
         */
        registerDisposable(d) {
            this.disposables = this.disposables || new AtomDisposableList_1.AtomDisposableList();
            return this.disposables.add(d);
        }
        // tslint:disable-next-line:no-empty
        onReady() { }
        /**
         * Execute given expression whenever any bindable expression changes
         * in the expression.
         *
         * For correct generic type resolution, target must always be `this`.
         *
         *      this.setupWatch(() => {
         *          if(!this.data.fullName){
         *              this.data.fullName = `${this.data.firstName} ${this.data.lastName}`;
         *          }
         *      });
         *
         * @protected
         * @template T
         * @param {() => any} ft
         * @returns {IDisposable}
         * @memberof AtomViewModel
         */
        setupWatch(ft, proxy, forValidation, name) {
            const d = new AtomWatcher_1.AtomWatcher(this, ft, proxy, this);
            if (forValidation) {
                this.validations = this.validations || [];
                this.validations.push({ name, watcher: d, initialized: false });
            }
            else {
                d.init();
            }
            return this.registerDisposable(d);
        }
        // tslint:disable-next-line:no-empty
        onPropertyChanged(name) { }
    };
    AtomViewModel = __decorate([
        __param(0, Inject_1.Inject),
        __metadata("design:paramtypes", [App_1.App])
    ], AtomViewModel);
    exports.AtomViewModel = AtomViewModel;
    /**
     * Receive messages for given channel
     * @param {(string | RegExp)} channel
     * @returns {Function}
     */
    function Receive(...channel) {
        return (target, key) => {
            baseTypes_1.registerInit(target, (vm) => {
                // tslint:disable-next-line:ban-types
                const fx = vm[key];
                const a = (ch, d) => {
                    const p = fx.call(vm, ch, d);
                    if (p && p.then && p.catch) {
                        p.catch((e) => {
                            // tslint:disable-next-line: no-console
                            console.warn(e);
                        });
                    }
                };
                const ivm = vm;
                for (const c of channel) {
                    ivm.registerDisposable(ivm.app.subscribe(c, a));
                }
            });
        };
    }
    exports.Receive = Receive;
    function BindableReceive(...channel) {
        return (target, key) => {
            const bp = BindableProperty_1.BindableProperty(target, key);
            baseTypes_1.registerInit(target, (vm) => {
                const fx = (cx, m) => {
                    vm[key] = m;
                };
                const ivm = vm;
                for (const c of channel) {
                    ivm.registerDisposable(ivm.app.subscribe(c, fx));
                }
            });
            return bp;
        };
    }
    exports.BindableReceive = BindableReceive;
    function BindableBroadcast(...channel) {
        return (target, key) => {
            const bp = BindableProperty_1.BindableProperty(target, key);
            baseTypes_1.registerInit(target, (vm) => {
                const fx = (t) => {
                    const v = vm[key];
                    for (const c of channel) {
                        vm.app.broadcast(c, v);
                    }
                };
                const d = new AtomWatcher_1.AtomWatcher(vm, [key.split(".")], fx);
                d.init();
                vm.registerDisposable(d);
            });
            return bp;
        };
    }
    exports.BindableBroadcast = BindableBroadcast;
    function Watch(target, key, descriptor) {
        baseTypes_1.registerInit(target, (vm) => {
            const ivm = vm;
            if (descriptor && descriptor.get) {
                ivm.setupWatch(descriptor.get, () => {
                    vm.refresh(key.toString());
                });
                return;
            }
            ivm.setupWatch(vm[key]);
        });
    }
    exports.Watch = Watch;
    /**
     * Cached watch must be used with async getters to avoid reloading of
     * resources unless the properties referenced are changed
     * @param target ViewModel
     * @param key name of property
     * @param descriptor descriptor of property
     */
    function CachedWatch(target, key, descriptor) {
        const getMethod = descriptor.get;
        descriptor.get = (() => null);
        baseTypes_1.registerInit(target, (vm) => {
            const ivm = vm;
            const fieldName = `_${key}`;
            Object.defineProperty(ivm, key, {
                enumerable: true,
                configurable: true,
                get() {
                    const c = ivm[fieldName] || (ivm[fieldName] = {
                        value: getMethod.apply(ivm)
                    });
                    return c.value;
                }
            });
            ivm.setupWatch(getMethod, () => {
                ivm[fieldName] = null;
                AtomBinder_1.AtomBinder.refreshValue(ivm, key);
            });
        });
    }
    exports.CachedWatch = CachedWatch;
    function Validate(target, key, descriptor) {
        // tslint:disable-next-line:ban-types
        const getMethod = descriptor.get;
        // // trick is to change property descriptor...
        // delete target[key];
        descriptor.get = () => null;
        // // replace it with dummy descriptor...
        // Object.defineProperty(target, key, descriptor);
        baseTypes_1.registerInit(target, (vm) => {
            const initialized = { i: false };
            const ivm = vm;
            Object.defineProperty(ivm, key, {
                enumerable: true,
                configurable: true,
                get() {
                    if (vm.mShouldValidate && initialized.i) {
                        return getMethod.apply(this);
                    }
                    return null;
                }
            });
            ivm.setupWatch(getMethod, () => {
                // descriptor.get = getMethod;
                // Object.defineProperty(target, key, descriptor);
                initialized.i = true;
                vm.refresh(key.toString());
            }, true, key.toString());
            return;
        });
    }
    exports.Validate = Validate;
});
//# sourceMappingURL=AtomViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/AtomViewModel");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../services/NavigationService", "./AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomWindowViewModel = void 0;
    const NavigationService_1 = require("../services/NavigationService");
    const AtomViewModel_1 = require("./AtomViewModel");
    /**
     * This view model should be used with WindowService to create and open window.
     *
     * This view model has `close` and `cancel` methods. `close` method will
     * close the window and will resolve the given result in promise. `cancel`
     * will reject the given promise.
     *
     * @example
     *
     *      @Inject windowService: NavigationService
     *      var result = await
     *          windowService.openPage(
     *              ModuleFiles.views.NewWindow,
     *              {
     *                  title: "Edit Object",
     *                  data: {
     *                      id: 4
     *                  }
     *              });
     *
     *
     *
     *      class NewTaskWindowViewModel extends AtomWindowViewModel{
     *
     *          ....
     *          save(){
     *
     *              // close and send result
     *              this.close(task);
     *
     *          }
     *          ....
     *
     *      }
     *
     * @export
     * @class AtomWindowViewModel
     * @extends {AtomViewModel}
     */
    class AtomWindowViewModel extends AtomViewModel_1.AtomViewModel {
        /**
         * This will broadcast `atom-window-close:windowName`.
         * WindowService will close the window on receipt of such message and
         * it will resolve the promise with given result.
         *
         *      this.close(someResult);
         *
         * @param {*} [result]
         * @memberof AtomWindowViewModel
         */
        close(result) {
            this.app.broadcast(`atom-window-close:${this.windowName}`, result);
        }
        /**
         * This will return true if this view model is safe to cancel and close
         */
        cancel() {
            return __awaiter(this, void 0, void 0, function* () {
                if (this.closeWarning) {
                    const navigationService = this.app.resolve(NavigationService_1.NavigationService);
                    if (!(yield navigationService.confirm(this.closeWarning, "Are you sure?"))) {
                        return false;
                    }
                }
                this.app.broadcast(`atom-window-cancel:${this.windowName}`, "cancelled");
                return true;
            });
        }
    }
    exports.AtomWindowViewModel = AtomWindowViewModel;
});
//# sourceMappingURL=AtomWindowViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/AtomWindowViewModel");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../core/ExpressionParser", "../core/types", "../services/NavigationService", "./baseTypes"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const ExpressionParser_1 = require("../core/ExpressionParser");
    const types_1 = require("../core/types");
    const NavigationService_1 = require("../services/NavigationService");
    const baseTypes_1 = require("./baseTypes");
    /**
     * Loads given method on based on init/watch properties.
     * If init is true, method will be executed when view model is initialized.
     * If watch is true, method will be executed when any of `this.*.*` properties are
     * modified. This method can be asynchronous. Watch will ignore all assignment
     * changes within the method.
     *
     * Every execution will be delayed by parameter specified in {@link ILoadOptions#watchDelayMS},
     * so multiple calls can be accumulated and only one final execution will proceed. This is useful
     * when you want to load items from API when user is continuously typing in search box.
     *
     * Method will have an input parameter for cancelToken {@link CancelToken} which you
     * can pass it to any REST Api call, before executing next method, cancelToken will
     * cancel previous execution.
     *
     * Either init or watch has to be true, or both can be true as well.
     */
    function Load({ init, showErrorOnInit, watch, watchDelayMS }) {
        // tslint:disable-next-line: only-arrow-functions
        return function (target, key) {
            baseTypes_1.registerInit(target, (vm) => {
                // tslint:disable-next-line: ban-types
                const oldMethod = vm[key];
                const app = vm.app;
                let showError = init ? (showErrorOnInit ? true : false) : true;
                let ct = new types_1.CancelToken();
                /**
                 * For the special case of init and watch both are true,
                 * we need to make sure that watch is ignored for first run
                 *
                 * So executing is set to true for the first time
                 */
                let executing = init;
                const m = (ctx) => __awaiter(this, void 0, void 0, function* () {
                    const ns = app.resolve(NavigationService_1.NavigationService);
                    try {
                        const pe = oldMethod.call(vm, ctx);
                        if (pe && pe.then) {
                            return yield pe;
                        }
                    }
                    catch (e) {
                        if (/^(cancelled|canceled)$/i.test(e.toString().trim())) {
                            // tslint:disable-next-line: no-console
                            console.warn(e);
                            return;
                        }
                        if (!showError) {
                            // tslint:disable-next-line: no-console
                            console.error(e);
                            return;
                        }
                        yield ns.alert(e, "Error");
                    }
                    finally {
                        showError = true;
                        executing = false;
                    }
                });
                if (watch) {
                    const fx = (c1) => __awaiter(this, void 0, void 0, function* () {
                        if (ct) {
                            ct.cancel();
                        }
                        const ct2 = ct = (c1 || new types_1.CancelToken());
                        if (executing) {
                            return;
                        }
                        executing = true;
                        try {
                            yield m(ct2);
                        }
                        catch (ex1) {
                            if (/^(cancelled|canceled)$/i.test(ex1.toString().trim())) {
                                // tslint:disable-next-line: no-console
                                console.warn(ex1);
                            }
                            else {
                                // tslint:disable-next-line: no-console
                                console.error(ex1);
                            }
                        }
                        finally {
                            executing = false;
                            ct = null;
                        }
                    });
                    let timeout = null;
                    // get path stripped as we are passing CancelToken, it will not
                    // parse for this. expressions..
                    const pathList = ExpressionParser_1.parsePath(oldMethod.toString(), true);
                    if (pathList.length === 0) {
                        throw new Error("Nothing to watch !!");
                    }
                    vm.setupWatch(pathList, () => {
                        if (executing) {
                            return;
                        }
                        if (timeout) {
                            clearTimeout(timeout);
                        }
                        timeout = setTimeout(() => {
                            timeout = null;
                            fx();
                        }, watchDelayMS || 100);
                    });
                    vm[key] = fx;
                }
                if (init) {
                    app.runAsync(() => m.call(vm, ct));
                }
            });
        };
    }
    exports.default = Load;
});
//# sourceMappingURL=Load.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/Load");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const XF_1 = require("../clr/XF");
    class AtomXFContentPage extends AtomXFControl_1.AtomXFControl {
        constructor(app, e) {
            super(app, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.ContentPage));
        }
    }
    exports.default = AtomXFContentPage;
});
//# sourceMappingURL=AtomXFContentPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class AlertSampleViewModel extends AtomViewModel_1.AtomViewModel {
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
            });
        }
        displayAlert() {
            this.navigationService.alert("This is an alert", "Alert");
        }
        displayAlertQuestion() {
            this.navigationService.confirm("Would you like to save your data ?", "Save ?");
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], AlertSampleViewModel.prototype, "navigationService", void 0);
    exports.default = AlertSampleViewModel;
});
//# sourceMappingURL=AlertSampleViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/alert/AlertSampleViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./AlertSampleViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const AlertSampleViewModel_1 = require("./AlertSampleViewModel");
    class AlertSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(AlertSampleViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Alert Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Button, { text: "Display alert", command: Bind_1.default.event(() => this.viewModel.displayAlert()) }),
                    XNode_1.default.create(XF_1.default.Button, { text: "Display alert question", command: Bind_1.default.event(() => this.viewModel.displayAlertQuestion()) }))));
        }
    }
    exports.default = AlertSample;
});
//# sourceMappingURL=AlertSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/alert/AlertSample");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class CustomPopupViewModel extends AtomViewModel_1.AtomViewModel {
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
            });
        }
        save() {
            this.navigationService
                .alert("name: " + this.userName + " password: " + this.password);
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], CustomPopupViewModel.prototype, "navigationService", void 0);
    exports.default = CustomPopupViewModel;
});
//# sourceMappingURL=CustomPopupViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/alert/custom-popup/CustomPopupViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./CustomPopupViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const CustomPopupViewModel_1 = require("./CustomPopupViewModel");
    class CustomPopupSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(CustomPopupViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { backgroundColor: "#C0808080", padding: "10,0", isVisible: true, title: "Custom popup sample" },
                XNode_1.default.create(XF_1.default.AbsoluteLayout, { layoutFlags: "All", layoutBounds: "0,0,1,1", verticalOptions: "Center", horizontalOptions: "Center" },
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical", heightRequest: 200, widthRequest: 300, backgroundColor: "White" },
                        XNode_1.default.create(XF_1.default.Entry, { margin: "20,20,20,10", placeholder: "Enter Username", text: Bind_1.default.twoWays(() => this.viewModel.userName) }),
                        XNode_1.default.create(XF_1.default.Entry, { margin: "20,0,20,0", placeholder: "Enter Password", isPassword: true, text: Bind_1.default.twoWays(() => this.viewModel.password) }),
                        XNode_1.default.create(XF_1.default.Button, { margin: "20,0,20,0", text: "Login", command: Bind_1.default.event(() => this.viewModel.save()) })))));
        }
    }
    exports.default = CustomPopupSample;
});
//# sourceMappingURL=CustomPopupSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/alert/custom-popup/CustomPopupSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AlertSample", "./custom-popup/CustomPopupSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AlertSample_1 = require("./AlertSample");
    const CustomPopupSample_1 = require("./custom-popup/CustomPopupSample");
    function addAlertSample(ms) {
        const alert = ms.addGroup("Alert");
        alert.addTabLink("Alert", AlertSample_1.default);
        alert.addTabLink("Custom popup", CustomPopupSample_1.default);
    }
    exports.default = addAlertSample;
});
//# sourceMappingURL=AlertSamplePage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/alert/AlertSamplePage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class BoxView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Box View" },
                XNode_1.default.create(XF_1.default.BoxView, { color: "CornflowerBlue", cornerRadius: "10", widthRequest: 160, heightRequest: 160, verticalOptions: "Center", horizontalOptions: "Center" })));
        }
    }
    exports.default = BoxView;
});
//# sourceMappingURL=BoxView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/box/BoxView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./BoxView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const BoxView_1 = require("./BoxView");
    function addBox(ms) {
        const box = ms.addGroup("Box");
        box.addTabLink("Box View", BoxView_1.default);
    }
    exports.default = addBox;
});
//# sourceMappingURL=BoxViewSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/box/BoxViewSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class LinearGradient extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, null,
                XNode_1.default.create(XF_1.default.Grid, { padding: 20 },
                    XNode_1.default.create(XF_1.default.Frame, { borderColor: "LightGray", hasShadow: "True", cornerRadius: "12", background: "DarkBlue" },
                        XNode_1.default.create(XF_1.default.Frame.background, null,
                            XNode_1.default.create(XF_1.default.LinearGradientBrush, { startPoint: "0,0", endPoint: "1,0" },
                                XNode_1.default.create(XF_1.default.GradientStop, { color: "Yellow", offset: 0.1 }),
                                XNode_1.default.create(XF_1.default.GradientStop, { color: "Green", offset: 1.0 })))))));
        }
    }
    exports.default = LinearGradient;
});
//# sourceMappingURL=LinearGradient.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/brushes/gradient/linear/LinearGradient");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.ColorItem = void 0;
    // tslint:disable:member-ordering
    class ColorItem {
        constructor(colorCodeOrRed, namedColorOrGreen, blue, alpha) {
            if (typeof colorCodeOrRed === "string") {
                this.colorCode = colorCodeOrRed;
                if (typeof namedColorOrGreen === "string") {
                    this.namedColor = namedColorOrGreen;
                }
                const r = ColorItem.parseRgb(this.colorCode);
                this.red = r.red;
                this.green = r.green;
                this.blue = r.blue;
                this.alpha = r.alpha;
            }
            else {
                this.red = colorCodeOrRed;
                if (typeof namedColorOrGreen === "number") {
                    this.green = namedColorOrGreen;
                }
                this.blue = blue;
                this.alpha = alpha;
                this.colorCode = ColorItem.rgb(this.red, this.green, this.blue, this.alpha);
            }
        }
        toString() {
            return this.colorCode;
        }
        withAlphaPercent(a) {
            // a = a * 255;
            return new ColorItem(this.red, this.green, this.blue, a);
        }
        static parseRgb(rgba) {
            if (/^\#/.test(rgba)) {
                rgba = rgba.substr(1);
                // this is hex...
                if (rgba.length === 3) {
                    rgba = rgba.split("").map((x) => x + x).join("");
                }
                const red = Number.parseInt(rgba[0] + rgba[1], 16);
                const green = Number.parseInt(rgba[2] + rgba[3], 16);
                const blue = Number.parseInt(rgba[4] + rgba[5], 16);
                if (rgba.length > 6) {
                    const alpha = Number.parseInt(rgba[6] + rgba[7], 16);
                    return { red, green, blue, alpha };
                }
                return { red, green, blue };
            }
            if (/^rgba/i.test(rgba)) {
                rgba = rgba.substr(5);
                rgba = rgba.substr(0, rgba.length - 1);
                const a = rgba.split(",").map((x, i) => i === 3 ? Number.parseFloat(x) : Number.parseInt(x, 10));
                return { red: a[0], green: a[1], blue: a[2], alpha: a[3] };
            }
            if (/^rgb/i.test(rgba)) {
                rgba = rgba.substr(4);
                rgba = rgba.substr(0, rgba.length - 1);
                const a = rgba.split(",").map((x) => Number.parseInt(x, 10));
                return { red: a[0], green: a[1], blue: a[2] };
            }
            throw new Error("Unknown color format " + rgba);
        }
        static rgb(r, g, b, a) {
            // if (!isInt(r)) {
            //     // all must be less than one...
            //     if (isInt(g) || isInt(b) || (a !== undefined && isInt(a))) {
            //         throw new Error("All color values must be either fractions or integers between 0 to 255");
            //     }
            //     r = r * 255;
            //     g = g * 255;
            //     b = b * 255;
            // }
            if (a !== undefined) {
                return `rgba(${r},${g},${b},${a})`;
            }
            return "#" + toFixedString(r) + toFixedString(g) + toFixedString(b);
        }
    }
    exports.ColorItem = ColorItem;
    // function isInt(n: number): boolean {
    //     return Number(n) === n && n % 1 === 0;
    // }
    function toFixedString(t) {
        return ("0" + t.toString(16)).slice(-2);
    }
    class Colors {
        static rgba(red, green, blue, alpha) {
            return new ColorItem(red, green, blue, alpha);
        }
        static parse(color) {
            if (!color) {
                return null;
            }
            color = color.toLowerCase();
            // check if exists in current...
            for (const key in Colors) {
                if (Colors.hasOwnProperty(key)) {
                    const element = Colors[key];
                    if (element instanceof ColorItem) {
                        const ci = element;
                        if (ci.namedColor === color) {
                            return ci;
                        }
                    }
                }
            }
            if (/^(\#|rgb\(|rgba\()/i.test(color)) {
                return new ColorItem(color);
            }
            throw new Error("Invalid color format " + color);
        }
    }
    exports.default = Colors;
    Colors.black = new ColorItem("#000000", "black");
    Colors.silver = new ColorItem("#c0c0c0", "silver");
    Colors.gray = new ColorItem("#808080", "gray");
    Colors.white = new ColorItem("#ffffff", "white");
    Colors.maroon = new ColorItem("#800000", "maroon");
    Colors.red = new ColorItem("#ff0000", "red");
    Colors.purple = new ColorItem("#800080", "purple");
    Colors.fuchsia = new ColorItem("#ff00ff", "fuchsia");
    Colors.green = new ColorItem("#008000", "green");
    Colors.lime = new ColorItem("#00ff00", "lime");
    Colors.olive = new ColorItem("#808000", "olive");
    Colors.yellow = new ColorItem("#ffff00", "yellow");
    Colors.navy = new ColorItem("#000080", "navy");
    Colors.blue = new ColorItem("#0000ff", "blue");
    Colors.teal = new ColorItem("#008080", "teal");
    Colors.aqua = new ColorItem("#00ffff", "aqua");
    Colors.orange = new ColorItem("#ffa500", "orange");
    Colors.aliceBlue = new ColorItem("#f0f8ff", "aliceblue");
    Colors.antiqueWhite = new ColorItem("#faebd7", "antiquewhite");
    Colors.aquaMarine = new ColorItem("#7fffd4", "aquamarine");
    Colors.azure = new ColorItem("#f0ffff", "azure");
    Colors.beige = new ColorItem("#f5f5dc", "beige");
    Colors.bisque = new ColorItem("#ffe4c4", "bisque");
    Colors.blanchedAlmond = new ColorItem("#ffebcd", "blanchedalmond");
    Colors.blueViolet = new ColorItem("#8a2be2", "blueviolet");
    Colors.brown = new ColorItem("#a52a2a", "brown");
    Colors.burlyWood = new ColorItem("#deb887", "burlywood");
    Colors.cadetBlue = new ColorItem("#5f9ea0", "cadetblue");
    Colors.chartReuse = new ColorItem("#7fff00", "chartreuse");
    Colors.chocolate = new ColorItem("#d2691e", "chocolate");
    Colors.coral = new ColorItem("#ff7f50", "coral");
    Colors.cornFlowerBlue = new ColorItem("#6495ed", "cornflowerblue");
    Colors.cornSilk = new ColorItem("#fff8dc", "cornsilk");
    Colors.crimson = new ColorItem("#dc143c", "crimson");
    Colors.cyan = new ColorItem("#00ffff", "cyan");
    Colors.darkBlue = new ColorItem("#00008b", "darkblue");
    Colors.darkCyan = new ColorItem("#008b8b", "darkcyan");
    Colors.darkGoldenRod = new ColorItem("#b8860b", "darkgoldenrod");
    Colors.darkGray = new ColorItem("#a9a9a9", "darkgray");
    Colors.darkGreen = new ColorItem("#006400", "darkgreen");
    Colors.darkGrey = new ColorItem("#a9a9a9", "darkgrey");
    Colors.darkKhaki = new ColorItem("#bdb76b", "darkkhaki");
    Colors.darkMagenta = new ColorItem("#8b008b", "darkmagenta");
    Colors.darkOliveGreen = new ColorItem("#556b2f", "darkolivegreen");
    Colors.darkOrange = new ColorItem("#ff8c00", "darkorange");
    Colors.darkOrchid = new ColorItem("#9932cc", "darkorchid");
    Colors.darkRed = new ColorItem("#8b0000", "darkred");
    Colors.darkSalmon = new ColorItem("#e9967a", "darksalmon");
    Colors.darkSeaGreen = new ColorItem("#8fbc8f", "darkseagreen");
    Colors.darkSlateBlue = new ColorItem("#483d8b", "darkslateblue");
    Colors.darkSlateGray = new ColorItem("#2f4f4f", "darkslategray");
    Colors.darkSlateGrey = new ColorItem("#2f4f4f", "darkslategrey");
    Colors.darkTurquoise = new ColorItem("#00ced1", "darkturquoise");
    Colors.darkViolet = new ColorItem("#9400d3", "darkviolet");
    Colors.deepPink = new ColorItem("#ff1493", "deeppink");
    Colors.deepSkyBlue = new ColorItem("#00bfff", "deepskyblue");
    Colors.dimGray = new ColorItem("#696969", "dimgray");
    Colors.dimGrey = new ColorItem("#696969", "dimgrey");
    Colors.dodgerBlue = new ColorItem("#1e90ff", "dodgerblue");
    Colors.fireBrick = new ColorItem("#b22222", "firebrick");
    Colors.floralWhite = new ColorItem("#fffaf0", "floralwhite");
    Colors.forestGreen = new ColorItem("#228b22", "forestgreen");
    Colors.gainsboro = new ColorItem("#dcdcdc", "gainsboro");
    Colors.ghostWhite = new ColorItem("#f8f8ff", "ghostwhite");
    Colors.gold = new ColorItem("#ffd700", "gold");
    Colors.goldenRod = new ColorItem("#daa520", "goldenrod");
    Colors.greenYellow = new ColorItem("#adff2f", "greenyellow");
    Colors.grey = new ColorItem("#808080", "grey");
    Colors.honeyDew = new ColorItem("#f0fff0", "honeydew");
    Colors.hotPink = new ColorItem("#ff69b4", "hotpink");
    Colors.indianRed = new ColorItem("#cd5c5c", "indianred");
    Colors.indigo = new ColorItem("#4b0082", "indigo");
    Colors.ivory = new ColorItem("#fffff0", "ivory");
    Colors.khaki = new ColorItem("#f0e68c", "khaki");
    Colors.lavender = new ColorItem("#e6e6fa", "lavender");
    Colors.lavenderBlush = new ColorItem("#fff0f5", "lavenderblush");
    Colors.lawnGreen = new ColorItem("#7cfc00", "lawngreen");
    Colors.lemonChiffon = new ColorItem("#fffacd", "lemonchiffon");
    Colors.lightBlue = new ColorItem("#add8e6", "lightblue");
    Colors.lightCoral = new ColorItem("#f08080", "lightcoral");
    Colors.lightCyan = new ColorItem("#e0ffff", "lightcyan");
    Colors.lightGoldenRodYellow = new ColorItem("#fafad2", "lightgoldenrodyellow");
    Colors.lightGray = new ColorItem("#d3d3d3", "lightgray");
    Colors.lightGreen = new ColorItem("#90ee90", "lightgreen");
    Colors.lightGrey = new ColorItem("#d3d3d3", "lightgrey");
    Colors.lightPink = new ColorItem("#ffb6c1", "lightpink");
    Colors.lightSalmon = new ColorItem("#ffa07a", "lightsalmon");
    Colors.lightSeaGreen = new ColorItem("#20b2aa", "lightseagreen");
    Colors.lightSkyBlue = new ColorItem("#87cefa", "lightskyblue");
    Colors.lightSlateGray = new ColorItem("#778899", "lightslategray");
    Colors.lightSlateGrey = new ColorItem("#778899", "lightslategrey");
    Colors.lightSteelBlue = new ColorItem("#b0c4de", "lightsteelblue");
    Colors.lightYellow = new ColorItem("#ffffe0", "lightyellow");
    Colors.limeGreen = new ColorItem("#32cd32", "limegreen");
    Colors.linen = new ColorItem("#faf0e6", "linen");
    Colors.magenta = new ColorItem("#ff00ff", "magenta");
    Colors.mediumAquaMarine = new ColorItem("#66cdaa", "mediumaquamarine");
    Colors.mediumBlue = new ColorItem("#0000cd", "mediumblue");
    Colors.mediumOrchid = new ColorItem("#ba55d3", "mediumorchid");
    Colors.mediumPurple = new ColorItem("#9370db", "mediumpurple");
    Colors.mediumSeaGreen = new ColorItem("#3cb371", "mediumseagreen");
    Colors.mediumSlateBlue = new ColorItem("#7b68ee", "mediumslateblue");
    Colors.mediumSpringGreen = new ColorItem("#00fa9a", "mediumspringgreen");
    Colors.mediumTurquoise = new ColorItem("#48d1cc", "mediumturquoise");
    Colors.mediumVioletred = new ColorItem("#c71585", "mediumvioletred");
    Colors.midNightBlue = new ColorItem("#191970", "midnightblue");
    Colors.mintCream = new ColorItem("#f5fffa", "mintcream");
    Colors.mistyRose = new ColorItem("#ffe4e1", "mistyrose");
    Colors.moccasin = new ColorItem("#ffe4b5", "moccasin");
    Colors.navajoWhite = new ColorItem("#ffdead", "navajowhite");
    Colors.oldLace = new ColorItem("#fdf5e6", "oldlace");
    Colors.oliveDrab = new ColorItem("#6b8e23", "olivedrab");
    Colors.orangeRed = new ColorItem("#ff4500", "orangered");
    Colors.orchid = new ColorItem("#da70d6", "orchid");
    Colors.paleGoldenRod = new ColorItem("#eee8aa", "palegoldenrod");
    Colors.paleGreen = new ColorItem("#98fb98", "palegreen");
    Colors.paleTurquoise = new ColorItem("#afeeee", "paleturquoise");
    Colors.paleVioletRed = new ColorItem("#db7093", "palevioletred");
    Colors.papayaWhip = new ColorItem("#ffefd5", "papayawhip");
    Colors.peachPuff = new ColorItem("#ffdab9", "peachpuff");
    Colors.peru = new ColorItem("#cd853f", "peru");
    Colors.pink = new ColorItem("#ffc0cb", "pink");
    Colors.plum = new ColorItem("#dda0dd", "plum");
    Colors.powderBlue = new ColorItem("#b0e0e6", "powderblue");
    Colors.rosyBrown = new ColorItem("#bc8f8f", "rosybrown");
    Colors.royalBlue = new ColorItem("#4169e1", "royalblue");
    Colors.saddleBrown = new ColorItem("#8b4513", "saddlebrown");
    Colors.salmon = new ColorItem("#fa8072", "salmon");
    Colors.sandyBrown = new ColorItem("#f4a460", "sandybrown");
    Colors.seaGreen = new ColorItem("#2e8b57", "seagreen");
    Colors.seaShell = new ColorItem("#fff5ee", "seashell");
    Colors.sienna = new ColorItem("#a0522d", "sienna");
    Colors.skyBlue = new ColorItem("#87ceeb", "skyblue");
    Colors.slateBlue = new ColorItem("#6a5acd", "slateblue");
    Colors.slateGray = new ColorItem("#708090", "slategray");
    Colors.slateGrey = new ColorItem("#708090", "slategrey");
    Colors.snow = new ColorItem("#fffafa", "snow");
    Colors.springGreen = new ColorItem("#00ff7f", "springgreen");
    Colors.steelBlue = new ColorItem("#4682b4", "steelblue");
    Colors.tan = new ColorItem("#d2b48c", "tan");
    Colors.thistle = new ColorItem("#d8bfd8", "thistle");
    Colors.tomato = new ColorItem("#ff6347", "tomato");
    Colors.turquoise = new ColorItem("#40e0d0", "turquoise");
    Colors.violet = new ColorItem("#ee82ee", "violet");
    Colors.wheat = new ColorItem("#f5deb3", "wheat");
    Colors.whiteSmoke = new ColorItem("#f5f5f5", "whitesmoke");
    Colors.yellowGreen = new ColorItem("#9acd32", "yellowgreen");
    Colors.rebeccaPurple = new ColorItem("#663399", "rebeccapurple");
});
//# sourceMappingURL=Colors.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/Colors");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class RadialGradient extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, null,
                XNode_1.default.create(XF_1.default.Grid, { padding: 20 },
                    XNode_1.default.create(XF_1.default.Frame, { borderColor: "LightGray", hasShadow: "True", cornerRadius: "12", background: "DarkBlue" },
                        XNode_1.default.create(XF_1.default.Frame.background, null,
                            XNode_1.default.create(XF_1.default.RadialGradientBrush, { center: "0.5,0.5", radius: 0.5 },
                                XNode_1.default.create(XF_1.default.GradientStop, { color: "Red", offset: 0.1 }),
                                XNode_1.default.create(XF_1.default.GradientStop, { color: Colors_1.default.darkOrange, offset: 1.0 })))))));
        }
    }
    exports.default = RadialGradient;
});
//# sourceMappingURL=RadialGradient.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/brushes/gradient/radial/RadialGradient");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    class SolidBrush extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, null,
                XNode_1.default.create(XF_1.default.Grid, { padding: 20 },
                    XNode_1.default.create(XF_1.default.Frame, { borderColor: "LightGray", hasShadow: "True", cornerRadius: "12", background: "DarkBlue" }))));
        }
    }
    exports.default = SolidBrush;
});
//# sourceMappingURL=SolidBrush.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/brushes/solid/SolidBrush");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./gradient/linear/LinearGradient", "./gradient/radial/RadialGradient", "./solid/SolidBrush"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const LinearGradient_1 = require("./gradient/linear/LinearGradient");
    const RadialGradient_1 = require("./gradient/radial/RadialGradient");
    const SolidBrush_1 = require("./solid/SolidBrush");
    function addBrushSamples(ms) {
        const group = ms.addGroup("Brushes");
        group.addTabLink("Solid", SolidBrush_1.default);
        group.addTabLink("Linear Gradient", LinearGradient_1.default);
        group.addTabLink("Radial Gradient", RadialGradient_1.default);
    }
    exports.default = addBrushSamples;
});
//# sourceMappingURL=addBrushSamples.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/brushes/addBrushSamples");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XF_1 = require("./XF");
    const RgPluginsPopup = {
        PopupPage: bridge.getClass("Rg.Plugins.Popup.Pages.PopupPage,Rg.Plugins.Popup")
    };
    exports.default = RgPluginsPopup;
});
//# sourceMappingURL=RgPluginsPopup.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/clr/RgPluginsPopup");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/RgPluginsPopup", "../clr/X", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const RgPluginsPopup_1 = require("../clr/RgPluginsPopup");
    const X_1 = require("../clr/X");
    const XF_1 = require("../clr/XF");
    const controlTemplate = XNode_1.default.create(XF_1.default.ControlTemplate, null,
        XNode_1.default.create(XF_1.default.Grid, null,
            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                XNode_1.default.create(XF_1.default.RowDefinition, { height: 20 }),
                XNode_1.default.create(XF_1.default.RowDefinition, { height: 30 }),
                XNode_1.default.create(XF_1.default.RowDefinition, null),
                XNode_1.default.create(XF_1.default.RowDefinition, { height: 50 })),
            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                XNode_1.default.create(XF_1.default.ColumnDefinition, { width: 50 }),
                XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                XNode_1.default.create(XF_1.default.ColumnDefinition, { width: 30 }),
                XNode_1.default.create(XF_1.default.ColumnDefinition, { width: 50 })),
            XNode_1.default.create(XF_1.default.BoxView, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), XF_1.default.Grid.rowSpan(2), XF_1.default.Grid.columnSpan(2), { backgroundColor: Colors_1.default.white })),
            XNode_1.default.create(XF_1.default.Label, Object.assign({ padding: 5 }, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { verticalOptions: "Center", text: X_1.default.TemplateBinding("Title") })),
            XNode_1.default.create(XF_1.default.ImageButton, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(2), { source: "res://WebAtoms.XF/Images.DeleteImage.png", command: Bind_1.default.event((x) => { var _a; return (_a = x.viewModel) === null || _a === void 0 ? void 0 : _a.cancel(); }) })),
            XNode_1.default.create(XF_1.default.ContentPresenter, Object.assign({ padding: 5 }, XF_1.default.Grid.row(2), XF_1.default.Grid.column(1), XF_1.default.Grid.columnSpan(2)))));
    class AtomXFPopupPage extends AtomXFControl_1.AtomXFControl {
        constructor(a, e) {
            super(a, e || AtomBridge_1.AtomBridge.instance.create(RgPluginsPopup_1.default.PopupPage));
        }
        preCreate() {
            super.preCreate();
            this.render(XNode_1.default.create(RgPluginsPopup_1.default.PopupPage, { title: Bind_1.default.oneWay(() => this.viewModel.title), controlTemplate: controlTemplate }));
        }
    }
    exports.default = AtomXFPopupPage;
});
//# sourceMappingURL=AtomXFPopupPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/pages/AtomXFPopupPage");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/view-model/AtomWindowViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const AtomWindowViewModel_1 = require("@web-atoms/core/dist/view-model/AtomWindowViewModel");
    class SearchPageViewModel extends AtomWindowViewModel_1.AtomWindowViewModel {
        init() {
            this.comboBox.windowViewModel = this;
            return super.init();
        }
        get items() {
            const items = this.comboBox.items;
            const s = this.comboBox.search;
            const st = this.comboBox.searchText;
            if (s && st) {
                return this.filtered(items, s, st);
            }
            return items;
        }
        filtered(items, search, searchText) {
            if (Array.isArray(search)) {
                const old = search;
                search = (item, st) => {
                    for (const iterator of old) {
                        const f = item[iterator];
                        if (!f) {
                            continue;
                        }
                        const sf = f.toString();
                        if (sf.toLocaleLowerCase().indexOf(st) !== -1) {
                            return true;
                        }
                    }
                    return false;
                };
            }
            return items.filter((item) => search(item, searchText.toLocaleLowerCase()));
        }
    }
    __decorate([
        AtomViewModel_1.Watch,
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [])
    ], SearchPageViewModel.prototype, "items", null);
    exports.default = SearchPageViewModel;
});
//# sourceMappingURL=SearchPageViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/combo-box/SearchPageViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/xf/controls/AtomXFControl", "./clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const XF_1 = require("./clr/XF");
    class AtomContentView extends AtomXFControl_1.AtomXFControl {
        constructor(a, e) {
            super(a, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.ContentView));
        }
    }
    exports.default = AtomContentView;
});
//# sourceMappingURL=AtomContentView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/AtomContentView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "../AtomContentView", "../clr/WA", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const AtomContentView_1 = require("../AtomContentView");
    const WA_1 = require("../clr/WA");
    const XF_1 = require("../clr/XF");
    class SelectionList extends AtomContentView_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { styleClass: Bind_1.default.oneWay(() => this.viewModel.comboBox.controlStyle.name) },
                XNode_1.default.create(XF_1.default.Grid, null,
                    XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                        XNode_1.default.create(XF_1.default.RowDefinition, { height: "Auto" }),
                        XNode_1.default.create(XF_1.default.RowDefinition, null)),
                    XNode_1.default.create(XF_1.default.SearchBar, { isVisible: Bind_1.default.oneWay(() => this.viewModel.comboBox.showSearch), text: Bind_1.default.twoWays(() => this.viewModel.comboBox.searchText) }),
                    XNode_1.default.create(XF_1.default.ListView, Object.assign({}, XF_1.default.Grid.row(1), { cachingStrategy: "RecycleElement", itemsSource: Bind_1.default.oneWay(() => this.viewModel.comboBox.items) }, WA_1.default.AtomViewCell.command((x) => {
                        this.viewModel.selectedItem = x;
                        setTimeout(() => this.viewModel.close(this.viewModel.selectedItem), 250);
                    }), WA_1.default.AtomViewCell.dataTemplate(Bind_1.default.oneWay(() => this.viewModel.comboBox.itemTemplate)))))));
        }
    }
    exports.default = SelectionList;
});
//# sourceMappingURL=SelectionList.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/combo-box/SelectionList");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBinder", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/BindableProperty", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/PropertyBinding", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/web/styles/AtomStyle", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/RgPluginsPopup", "../clr/WA", "../clr/XF", "../pages/AtomXFContentPage", "../pages/AtomXFPopupPage", "./SearchPageViewModel", "./SelectionList"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomXFComboBoxStyle = void 0;
    const AtomBinder_1 = require("@web-atoms/core/dist/core/AtomBinder");
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const BindableProperty_1 = require("@web-atoms/core/dist/core/BindableProperty");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const PropertyBinding_1 = require("@web-atoms/core/dist/core/PropertyBinding");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomStyle_1 = require("@web-atoms/core/dist/web/styles/AtomStyle");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const RgPluginsPopup_1 = require("../clr/RgPluginsPopup");
    const WA_1 = require("../clr/WA");
    const XF_1 = require("../clr/XF");
    const AtomXFContentPage_1 = require("../pages/AtomXFContentPage");
    const AtomXFPopupPage_1 = require("../pages/AtomXFPopupPage");
    const SearchPageViewModel_1 = require("./SearchPageViewModel");
    const SelectionList_1 = require("./SelectionList");
    class AtomXFComboBoxStyle extends AtomStyle_1.AtomStyle {
        get root() {
            return {
                subclasses: {
                    " .item": {
                        padding: 10
                    }
                }
            };
        }
    }
    exports.AtomXFComboBoxStyle = AtomXFComboBoxStyle;
    class AtomXFComboBox extends AtomXFControl_1.AtomXFControl {
        constructor(a, e) {
            super(a, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.Grid));
        }
        onPropertyChanged(name) {
            if (name === "items") {
                if (this.value !== undefined
                    && (this.items && this.items.indexOf(this.selectedItem) === -1)) {
                    AtomBinder_1.AtomBinder.refreshValue(this, "value");
                    return;
                }
            }
        }
        preCreate() {
            this.defaultControlStyle = AtomXFComboBoxStyle;
            this.prompt = "Select";
            this.showSearch = true;
            this.showAsPopup = true;
            this.searchText = "";
            this.itemPadding = 10;
            this.search = ["label", "value"];
            // this.value = null;
            this.valuePath = "value";
            // this.selectedItem = null;
            this.promptTemplate = null;
            this.itemTemplate = null;
            this.dropDownImage = "res://WebAtoms.XF/Images.DropDownImage.png";
            this.selectionViewTemplate = null;
            const vf = (item, def) => {
                if (item === undefined || item === null) {
                    return def;
                }
                const vp = this.valuePath;
                if (typeof vp === "function") {
                    return vp(item);
                }
                return item[vp];
            };
            this.registerDisposable(new PropertyBinding_1.PropertyBinding(this, this.element, "selectedItem", [["this", "value"]], true, {
                fromSource: (v) => (v !== undefined && v !== null && this.items)
                    ? (this.items.find((x) => vf(x) === v) || this.selectedItem)
                    : this.selectedItem,
                fromTarget: (v) => vf(v, this.value)
            }, this));
            // if the items were updated... we need to refresh the selected item...
            // this.registerDisposable(new AtomWatcher(this, () => this.items, () => {
            //     if (this.selectedItem === undefined
            //         || this.selectedItem === null
            //         || (this.items && this.items.indexOf(this.selectedItem) === -1)) {
            //         AtomBinder.refreshValue(this, "value");
            //         return;
            //     }
            // }));
        }
        create() {
            // const ImageButton = XNode.attach(AtomXFLink, XF.ImageButton);
            this.render(XNode_1.default.create(XF_1.default.Grid, { styleClass: this.controlStyle.name },
                XNode_1.default.create(AtomXFComboBox.promptTemplate, null,
                    XNode_1.default.create(XF_1.default.DataTemplate, null,
                        XNode_1.default.create(XF_1.default.Label, { verticalTextAlignment: "Center", styleClass: "item", text: Bind_1.default.oneWay(() => this.prompt) }))),
                XNode_1.default.create(AtomXFComboBox.itemTemplate, null,
                    XNode_1.default.create(XF_1.default.DataTemplate, null,
                        XNode_1.default.create(XF_1.default.Label, { styleClass: "item", verticalTextAlignment: "Center", text: Bind_1.default.oneWay((x) => (x.data ? (x.data.label) : null) || "Loading.."), backgroundColor: Bind_1.default.oneWay((x) => x.data === x.viewModel.selectedItem
                                ? Colors_1.default.lightBlue
                                : Colors_1.default.white) }))),
                XNode_1.default.create(AtomXFComboBox.selectionViewTemplate, null,
                    XNode_1.default.create(XF_1.default.DataTemplate, null,
                        XNode_1.default.create(SelectionList_1.default, null))),
                XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                    XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                    XNode_1.default.create(XF_1.default.ColumnDefinition, { width: "Auto" })),
                XNode_1.default.create(WA_1.default.AtomView, { bindingContext: Bind_1.default.oneWay(() => this.selectedItem), dataTemplate: Bind_1.default.oneWay(() => this.itemTemplate), emptyDataTemplate: Bind_1.default.oneWay(() => this.promptTemplate) }),
                XNode_1.default.create(XF_1.default.ImageButton, Object.assign({}, XF_1.default.Grid.column(1), { heightRequest: 40, widthRequest: 40, source: "res://WebAtoms.XF/Images.DropDownImage.png", command: () => this.app.runAsync(() => this.openPopup()) }))));
        }
        openPopup() {
            return __awaiter(this, void 0, void 0, function* () {
                try {
                    const ns = this.resolve(NavigationService_1.NavigationService);
                    const r = yield ns.openPage(this.showAsPopup ? SearchPopupPage : SearchPage, {
                        "title": this.prompt,
                        "ref:selectedItem": this.selectedItem,
                        "ref:comboBox": this
                    });
                    this.selectedItem = r;
                    this.element.dispatchEvent(new CustomEvent("selectionChanged", { detail: r }));
                }
                catch (e) {
                    // tslint:disable-next-line: no-console
                    console.error(e);
                }
            });
        }
    }
    AtomXFComboBox.itemTemplate = XNode_1.default.prepare("itemTemplate", true, true);
    AtomXFComboBox.promptTemplate = XNode_1.default.prepare("promptTemplate", true, true);
    AtomXFComboBox.selectionViewTemplate = XNode_1.default.prepare("selectionViewTemplate", true, true);
    __decorate([
        BindableProperty_1.BindableProperty,
        __metadata("design:type", Object)
    ], AtomXFComboBox.prototype, "selectedItem", void 0);
    __decorate([
        BindableProperty_1.BindableProperty,
        __metadata("design:type", Array)
    ], AtomXFComboBox.prototype, "items", void 0);
    __decorate([
        BindableProperty_1.BindableProperty,
        __metadata("design:type", Object)
    ], AtomXFComboBox.prototype, "value", void 0);
    exports.default = AtomXFComboBox;
    class SearchPage extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(SearchPageViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: Bind_1.default.oneWay(() => this.viewModel.title) },
                XNode_1.default.create(WA_1.default.AtomView, { bindingContext: "Empty", dataTemplate: Bind_1.default.oneWay(() => this.viewModel.comboBox.selectionViewTemplate) })));
        }
    }
    class SearchPopupPage extends AtomXFPopupPage_1.default {
        create() {
            this.viewModel = this.resolve(SearchPageViewModel_1.default);
            this.render(XNode_1.default.create(RgPluginsPopup_1.default.PopupPage, null,
                XNode_1.default.create(WA_1.default.AtomView, { bindingContext: "Empty", dataTemplate: Bind_1.default.oneWay(() => this.viewModel.comboBox.selectionViewTemplate) })));
        }
    }
});
//# sourceMappingURL=AtomXFComboBox.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/combo-box/AtomXFComboBox");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const XF_1 = require("../clr/XF");
    class AtomXFGrid extends AtomXFControl_1.AtomXFControl {
        constructor(app, e) {
            super(app, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.Grid));
        }
    }
    exports.default = AtomXFGrid;
});
//# sourceMappingURL=AtomXFGrid.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/controls/AtomXFGrid");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/web/styles/AtomStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const AtomStyle_1 = require("@web-atoms/core/dist/web/styles/AtomStyle");
    class AtomCalendarStyle extends AtomStyle_1.AtomStyle {
        get root() {
            return {
                columnGap: "0",
                rowGap: "0",
                subclasses: {
                    " grid": {
                        columnGap: "0",
                        rowGap: "0"
                    },
                    " .week-days": {
                        backgroundColor: Colors_1.default.orange,
                        padding: 10,
                        margin: 0,
                        color: Colors_1.default.white,
                        textAlign: "center"
                    },
                    " .date-css": {
                        margin: 0,
                        padding: 10,
                        backgroundColor: "initial",
                        color: "initial",
                        textAlign: "center"
                    },
                    " .is-today": {
                        backgroundColor: Colors_1.default.lightGreen
                    },
                    " .is-other-month": {
                        backgroundColor: Colors_1.default.lightGray
                    },
                    " .is-weekend": {
                        color: Colors_1.default.gray
                    },
                    " .is-selected": {
                        backgroundColor: Colors_1.default.blue,
                        color: Colors_1.default.white
                    }
                }
            };
        }
    }
    exports.default = AtomCalendarStyle;
});
//# sourceMappingURL=AtomXFCalendarStyle.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/calendar/AtomXFCalendarStyle");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/Atom", "@web-atoms/core/dist/core/types", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/view-model/Load", "@web-atoms/date-time/dist/DateTime"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Atom_1 = require("@web-atoms/core/dist/Atom");
    const types_1 = require("@web-atoms/core/dist/core/types");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const Load_1 = require("@web-atoms/core/dist/view-model/Load");
    const DateTime_1 = require("@web-atoms/date-time/dist/DateTime");
    class AtomCalendarViewModel extends AtomViewModel_1.AtomViewModel {
        get year() {
            return this.start.year;
        }
        set year(v) {
            if (this.start.year === v) {
                return;
            }
            const s = this.mStart;
            this.mStart = new DateTime_1.default(v, s.month, s.day);
            this.refresh("start");
        }
        get month() {
            return this.start.month;
        }
        set month(v) {
            if (this.start.month === v) {
                return;
            }
            const s = this.mStart;
            this.mStart = new DateTime_1.default(s.year, v, s.day);
            this.refresh("start");
        }
        get start() {
            return this.mStart || (this.mStart = this.owner.selectedDate || DateTime_1.default.today);
        }
        set start(value) {
            this.mStart = value;
            this.refresh("year");
            this.refresh("month");
            this.refresh("start");
        }
        get enableFunc() {
            return this.owner.enableFunc;
        }
        get yearList() {
            const start = this.year + this.owner.yearStart;
            const end = this.year + this.owner.yearEnd;
            const a = [];
            for (let index = start; index <= end; index++) {
                a.push({ label: index + "", value: index });
            }
            return a;
        }
        loadSelectedDate() {
            // tslint:disable-next-line: no-string-literal
            const s = this["start"];
            const sel = this.owner.selectedDate;
            if (sel) {
                if (s.getFullYear() !== sel.getFullYear()) {
                    // tslint:disable-next-line: no-string-literal
                    this["start"] = new DateTime_1.default(sel.getFullYear(), sel.getMonth(), sel.getDate());
                    return;
                }
                if (s.getMonth() !== sel.getMonth()) {
                    // tslint:disable-next-line: no-string-literal
                    this["start"] = new DateTime_1.default(sel.getFullYear(), sel.getMonth(), sel.getDate());
                    return;
                }
            }
        }
        loadItems(ct) {
            return __awaiter(this, void 0, void 0, function* () {
                yield Atom_1.Atom.delay(10);
                if (ct.cancelled) {
                    return;
                }
                const today = DateTime_1.default.today;
                const start = this.start;
                let startDate = new DateTime_1.default(start.year, start.month, 1);
                while (startDate.dayOfWeek !== 1) {
                    startDate = startDate.add(-1);
                }
                const a = [];
                const y = startDate.year;
                const m = startDate.month;
                for (let index = 0; index < 42; index++) {
                    const cd = startDate.add(index);
                    a.push({
                        y: Math.abs(index / 7),
                        x: index % 7,
                        label: cd.day + "",
                        type: null,
                        value: cd,
                        isToday: cd.equals(today),
                        isOtherMonth: start.month !== cd.month,
                        isWeekend: (cd.dayOfWeek === 0 || cd.dayOfWeek === 6)
                    });
                }
                const o = this.owner;
                o.element.dispatchEvent(new CustomEvent("refresh", {
                    detail: {
                        year: start.year,
                        month: start.month
                    }
                }));
                o.currentDate = startDate;
                this.items = a;
            });
        }
        // public async init() {
        //     if (this.owner.selectedDate) {
        //         this.start = this.selectedDate;
        //     }
        // }
        changeMonth(step) {
            const s = this.start.addMonths(step);
            const start = this.year + this.owner.yearStart;
            const end = this.year + this.owner.yearEnd;
            const sy = s.year;
            if (sy < start || sy > end) {
                return;
            }
            this.start = s;
            this.refresh("start");
        }
        dateClicked(item) {
            const e = this.owner.element;
            this.owner.selectedDate = item.value;
            e.dispatchEvent(new CustomEvent("dateClicked", { detail: item }));
        }
    }
    __decorate([
        AtomViewModel_1.Watch,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [])
    ], AtomCalendarViewModel.prototype, "enableFunc", null);
    __decorate([
        AtomViewModel_1.Watch,
        __metadata("design:type", Array),
        __metadata("design:paramtypes", [])
    ], AtomCalendarViewModel.prototype, "yearList", null);
    __decorate([
        Load_1.default({ watch: true }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], AtomCalendarViewModel.prototype, "loadSelectedDate", null);
    __decorate([
        Load_1.default({ init: true, watch: true, watchDelayMS: 1 }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [types_1.CancelToken]),
        __metadata("design:returntype", Promise)
    ], AtomCalendarViewModel.prototype, "loadItems", null);
    exports.default = AtomCalendarViewModel;
});
//# sourceMappingURL=AtomXFCalendarViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/calendar/AtomXFCalendarViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "../clr/XF", "../combo-box/AtomXFComboBox", "../controls/AtomXFGrid", "./AtomXFCalendarStyle", "./AtomXFCalendarViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("../clr/XF");
    const AtomXFComboBox_1 = require("../combo-box/AtomXFComboBox");
    const AtomXFGrid_1 = require("../controls/AtomXFGrid");
    const AtomXFCalendarStyle_1 = require("./AtomXFCalendarStyle");
    const AtomXFCalendarViewModel_1 = require("./AtomXFCalendarViewModel");
    function toCss(a) {
        let r = "";
        for (const key in a) {
            if (a.hasOwnProperty(key)) {
                const element = a[key];
                if (element) {
                    r += key + ",";
                }
            }
        }
        return r;
    }
    const monthList = [
        { label: "January", value: 0 },
        { label: "February", value: 1 },
        { label: "March", value: 2 },
        { label: "April", value: 3 },
        { label: "May", value: 4 },
        { label: "June", value: 5 },
        { label: "July", value: 6 },
        { label: "August", value: 7 },
        { label: "September", value: 8 },
        { label: "October", value: 9 },
        { label: "November", value: 10 },
        { label: "December", value: 11 }
    ];
    const weekDays = [
        { label: "Mon", value: "Mon" },
        { label: "Tue", value: "Tue" },
        { label: "Wed", value: "Wed" },
        { label: "Thu", value: "Thu" },
        { label: "Fri", value: "Fri" },
        { label: "Sat", value: "Sat" },
        { label: "Sun", value: "Sun" }
    ];
    const BindDay = Bind_1.default.forData();
    class AtomXFCalendar extends AtomXFGrid_1.default {
        create() {
            this.defaultControlStyle = AtomXFCalendarStyle_1.default;
            this.selectedDate = null;
            this.yearStart = -10;
            this.yearEnd = 10;
            this.enableFunc = null;
            this.itemTemplate = null;
            this.localViewModel = this.resolve(AtomXFCalendarViewModel_1.default, "owner");
            this.render(XNode_1.default.create(XF_1.default.Grid, { styleClass: this.controlStyle.name },
                XNode_1.default.create(AtomXFCalendar.itemTemplate, null,
                    XNode_1.default.create(XF_1.default.DataTemplate, null,
                        XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(BindDay.oneTime((x) => x.data.y)), XF_1.default.Grid.column(BindDay.oneTime((x) => x.data.x)), { styleClass: BindDay.oneWay((x) => toCss({
                                "date-css": 1,
                                "is-other-month": x.data.isOtherMonth,
                                "is-today": x.data.isToday,
                                "is-weekend": x.data.isWeekend,
                                "is-selected": x.data.value.dateEquals(this.selectedDate),
                                "is-disabled": this.localViewModel.enableFunc
                                    ? this.localViewModel.enableFunc(x.data)
                                    : 0
                            })), text: BindDay.oneTime((x) => x.data.label) }),
                            XNode_1.default.create(XF_1.default.Label.gestureRecognizers, null,
                                XNode_1.default.create(XF_1.default.TapGestureRecognizer, { command: BindDay.event((x) => this.localViewModel.dateClicked(x.data)) }))))),
                XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                    XNode_1.default.create(XF_1.default.RowDefinition, { height: "auto" }),
                    XNode_1.default.create(XF_1.default.RowDefinition, { height: "auto" }),
                    XNode_1.default.create(XF_1.default.RowDefinition, null)),
                XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                    XNode_1.default.create(XF_1.default.ColumnDefinition, { width: "auto" }),
                    XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                    XNode_1.default.create(XF_1.default.ColumnDefinition, { width: "auto" }),
                    XNode_1.default.create(XF_1.default.ColumnDefinition, { width: "auto" })),
                XNode_1.default.create(AtomXFComboBox_1.default, Object.assign({ items: monthList, value: Bind_1.default.twoWays(() => this.localViewModel.month) }, XF_1.default.Grid.column(1))),
                XNode_1.default.create(AtomXFComboBox_1.default, Object.assign({ items: Bind_1.default.oneWay(() => this.localViewModel.yearList), value: Bind_1.default.twoWays(() => this.localViewModel.year) }, XF_1.default.Grid.column(2))),
                XNode_1.default.create(XF_1.default.Grid, Object.assign({ class: "week-days" }, XF_1.default.Grid.row(1), XF_1.default.Grid.columnSpan(4)),
                    XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null)),
                    XNode_1.default.create(XF_1.default.Label, { text: weekDays[0].label }),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: weekDays[1].label })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(2), { text: weekDays[2].label })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(3), { text: weekDays[3].label })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(4), { text: weekDays[4].label })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(5), { text: weekDays[5].label })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(6), { text: weekDays[6].label }))),
                XNode_1.default.create(XF_1.default.Grid, Object.assign({}, XF_1.default.BindableLayout.itemTemplate(Bind_1.default.oneWay(() => this.itemTemplate)), XF_1.default.BindableLayout.itemsSource(Bind_1.default.oneWay(() => this.localViewModel.items)), XF_1.default.Grid.row(2), XF_1.default.Grid.columnSpan(4)),
                    XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, null)),
                    XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                        XNode_1.default.create(XF_1.default.RowDefinition, null),
                        XNode_1.default.create(XF_1.default.RowDefinition, null),
                        XNode_1.default.create(XF_1.default.RowDefinition, null),
                        XNode_1.default.create(XF_1.default.RowDefinition, null),
                        XNode_1.default.create(XF_1.default.RowDefinition, null),
                        XNode_1.default.create(XF_1.default.RowDefinition, null)))));
        }
    }
    exports.default = AtomXFCalendar;
    AtomXFCalendar.itemTemplate = XNode_1.default.prepare("itemTemplate", true, true);
});
//# sourceMappingURL=AtomXFCalendar.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/calendar/AtomXFCalendar");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/BindableProperty", "@web-atoms/core/dist/core/XNode", "@web-atoms/date-time/dist/DateTime", "@web-atoms/xf-controls/dist/calendar/AtomXFCalendar", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const BindableProperty_1 = require("@web-atoms/core/dist/core/BindableProperty");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const DateTime_1 = require("@web-atoms/date-time/dist/DateTime");
    const AtomXFCalendar_1 = require("@web-atoms/xf-controls/dist/calendar/AtomXFCalendar");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class Calendar extends AtomXFContentPage_1.default {
        create() {
            this.date = DateTime_1.default.today;
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Calendar" },
                XNode_1.default.create(XF_1.default.Grid, null,
                    XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                        XNode_1.default.create(XF_1.default.RowDefinition, null),
                        XNode_1.default.create(XF_1.default.RowDefinition, { height: "Auto" }),
                        XNode_1.default.create(XF_1.default.RowDefinition, null)),
                    XNode_1.default.create(AtomXFCalendar_1.default, { selectedDate: Bind_1.default.twoWays(() => this.date) }),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), { text: Bind_1.default.oneWay(() => this.date.toLocaleDateString()) })))));
        }
    }
    __decorate([
        BindableProperty_1.BindableProperty,
        __metadata("design:type", DateTime_1.default)
    ], Calendar.prototype, "date", void 0);
    exports.default = Calendar;
});
//# sourceMappingURL=Calendar.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/calendar/Calendar");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./Calendar"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Calendar_1 = require("./Calendar");
    function addCalendarSamples(ms) {
        const group = ms.addGroup("Calendar");
        group.addTabLink("Calendar", Calendar_1.default);
    }
    exports.default = addCalendarSamples;
});
//# sourceMappingURL=calendarSamples.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/calendar/calendarSamples");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const XF_1 = require("../clr/XF");
    class AtomXFCarouselPage extends AtomXFControl_1.AtomXFControl {
        constructor(app, e) {
            super(app, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.CarouselPage));
        }
    }
    exports.default = AtomXFCarouselPage;
});
//# sourceMappingURL=AtomXFCarouselPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/pages/AtomXFCarouselPage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFCarouselPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFCarouselPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFCarouselPage");
    class CarouselPageView extends AtomXFCarouselPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.CarouselPage, { title: "Carousel Page Sample" },
                XNode_1.default.create(XF_1.default.ContentPage, null,
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical" },
                        XNode_1.default.create(XF_1.default.Label, { horizontalTextAlignment: "Center", fontSize: 30, text: "Content 1" }),
                        XNode_1.default.create(XF_1.default.Image, { source: "https://cdn.jsdelivr.net/npm/@web-atoms/samples@1.1.45/src/web/images/logo.png", horizontalOptions: "Center", verticalOptions: "Center" }))),
                XNode_1.default.create(XF_1.default.ContentPage, null,
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical" },
                        XNode_1.default.create(XF_1.default.Label, { horizontalTextAlignment: "Center", fontSize: 30, text: "Content 2" }),
                        XNode_1.default.create(XF_1.default.Image, { source: "https://cdn.jsdelivr.net/npm/@web-atoms/samples@1.1.45/src/web/images/logo.png", horizontalOptions: "Center", verticalOptions: "Center" }))),
                XNode_1.default.create(XF_1.default.ContentPage, null,
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical" },
                        XNode_1.default.create(XF_1.default.Label, { horizontalTextAlignment: "Center", fontSize: 30, text: "Content 3" }),
                        XNode_1.default.create(XF_1.default.Image, { source: "https://cdn.jsdelivr.net/npm/@web-atoms/samples@1.1.45/src/web/images/logo.png", horizontalOptions: "Center", verticalOptions: "Center" })))));
        }
    }
    exports.default = CarouselPageView;
});
//# sourceMappingURL=CarouselPageView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/carousel/carousel-page/CarouselPageView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./CarouselPageView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const CarouselPageView_1 = require("./CarouselPageView");
    function addCarouselPage(ms) {
        const cp = ms.addGroup("Carousel Page");
        cp.addTabLink("Carousel Page", CarouselPageView_1.default);
    }
    exports.default = addCarouselPage;
});
//# sourceMappingURL=CarouselPageSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/carousel/carousel-page/CarouselPageSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class CarouselViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.items = [
                { label: "Item 1" },
                { label: "Item 2" },
                { label: "Item 3" },
                { label: "Item 4" },
                { label: "Item 5" }
            ];
            this.selectedItem = this.items[0];
        }
    }
    exports.default = CarouselViewModel;
});
//# sourceMappingURL=CarouselViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/carousel/carousel-view/CarouselViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./CarouselViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const CarouselViewModel_1 = require("./CarouselViewModel");
    class CarouselView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(CarouselViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Carousel View Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { padding: 20 },
                    XNode_1.default.create(XF_1.default.CarouselView, { currentItem: Bind_1.default.twoWays(() => this.viewModel.selectedItem), itemsSource: Bind_1.default.oneWay(() => this.viewModel.items) },
                        XNode_1.default.create(XF_1.default.CarouselView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.StackLayout, null,
                                    XNode_1.default.create(XF_1.default.Frame, { borderColor: "DarkGray", cornerRadius: 5, margin: "20", heightRequest: 30, horizontalOptions: "Center", verticalOptions: "CenterAndExpand" },
                                        XNode_1.default.create(XF_1.default.StackLayout, null,
                                            XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.label), fontAttributes: "Bold", fontSize: 20, horizontalOptions: "Center", verticalOptions: "Center" }))))))))));
        }
    }
    exports.default = CarouselView;
});
//# sourceMappingURL=CarouselView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/carousel/carousel-view/CarouselView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./CarouselView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const CarouselView_1 = require("./CarouselView");
    function addCarousel(ms) {
        const c = ms.addGroup("Carousel");
        c.addTabLink("Carousel", CarouselView_1.default);
    }
    exports.default = addCarousel;
});
//# sourceMappingURL=CarouselSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/carousel/carousel-view/CarouselSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class GroupingViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.items = [
                {
                    name: "Bears",
                    list: [
                        {
                            name: "American Black Bear",
                            image: "https://upload.wikimedia.org/wikipedia/commons/0/08/01_Schwarzbär.jpg",
                            location: "North America"
                        },
                        {
                            name: "Asian Black Bear",
                            image: "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/Ursus_thibetanus_3_%28Wroclaw_zoo%29.JPG/180px-Ursus_thibetanus_3_%28Wroclaw_zoo%29.JPG",
                            location: "Asia"
                        }
                    ]
                },
                {
                    name: "Monkeys",
                    list: [
                        {
                            name: "Baboon",
                            location: "Africa & Asia",
                            image: "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg"
                        },
                        {
                            name: "Capuchin Monkey",
                            location: "Central & South America",
                            image: "http://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg"
                        },
                        {
                            name: "Blue Monkey",
                            location: "Central and East Africa",
                            image: "http://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg"
                        }
                    ]
                }
            ];
        }
        get group() {
            const a = [];
            for (const iterator of this.items) {
                const list = iterator.list;
                list.key = iterator;
                a.push(list);
            }
            return a;
        }
    }
    exports.default = GroupingViewModel;
});
//# sourceMappingURL=GroupingViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/grouping/GroupingViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./GroupingViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const GroupingViewModel_1 = require("./GroupingViewModel");
    class GroupingSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(GroupingViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Grouping Sample" },
                XNode_1.default.create(XF_1.default.CollectionView, Object.assign({ itemSizingStrategy: "MeasureAllItems" }, WA_1.default.GroupBy.itemsSource(Bind_1.default.oneWay(() => this.viewModel.group)), { isGrouped: true, itemsLayout: "VerticalList" }),
                    XNode_1.default.create(XF_1.default.CollectionView.groupHeaderTemplate, null,
                        XNode_1.default.create(XF_1.default.DataTemplate, null,
                            XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.name), BackgroundColor: "LightGray", FontSize: "Large", FontAttributes: "Bold" }))),
                    XNode_1.default.create(XF_1.default.CollectionView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.Grid, { Padding: "10" },
                            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" }),
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" })),
                            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" }),
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" })),
                            XNode_1.default.create(XF_1.default.Image, Object.assign({}, XF_1.default.Grid.rowSpan(2), { Source: Bind_1.default.oneWay((x) => x.data.image), Aspect: "AspectFill", HeightRequest: "60", WidthRequest: "60" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: Bind_1.default.oneWay((x) => x.data.name), FontAttributes: "Bold" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { text: Bind_1.default.oneWay((x) => x.data.location), FontAttributes: "Italic", VerticalOptions: "End" })))))));
        }
    }
    exports.default = GroupingSample;
});
//# sourceMappingURL=GroupingSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/grouping/GroupingSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class HeaderFooterViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.list = [];
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
                for (let i = 0; i <= 6; i += 1) {
                    const element = {};
                    element.image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg";
                    element.location = "location " + i;
                    element.name = "name " + i;
                    this.list.add(element);
                }
            });
        }
    }
    exports.default = HeaderFooterViewModel;
});
//# sourceMappingURL=HeaderFooterViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/header-footer/HeaderFooterViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./HeaderFooterViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const HeaderFooterViewModel_1 = require("./HeaderFooterViewModel");
    class HeaderFooterSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(HeaderFooterViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Header and Footer Sample" },
                XNode_1.default.create(XF_1.default.CollectionView, { header: ({ label: "Monkeys" }), footer: ({ label: "Friends of Xamarin Monkey" }), itemsSource: Bind_1.default.oneWay(() => this.viewModel.list), itemsLayout: "VerticalList" },
                    XNode_1.default.create(XF_1.default.CollectionView.headerTemplate, null,
                        XNode_1.default.create(XF_1.default.DataTemplate, null,
                            XNode_1.default.create(XF_1.default.StackLayout, { backgroundColor: "LightGray" },
                                XNode_1.default.create(XF_1.default.Label, { Margin: "10,0,0,0", text: Bind_1.default.oneWay((x) => x.data.label), FontSize: "Small", fontAttributes: "Bold" })))),
                    XNode_1.default.create(XF_1.default.CollectionView.footerTemplate, null,
                        XNode_1.default.create(XF_1.default.DataTemplate, null,
                            XNode_1.default.create(XF_1.default.StackLayout, { BackgroundColor: "LightGray" },
                                XNode_1.default.create(XF_1.default.Label, { Margin: "10,0,0,0", text: Bind_1.default.oneWay((x) => x.data.label), FontSize: "Small", fontAttributes: "Bold" })))),
                    XNode_1.default.create(XF_1.default.CollectionView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.Grid, { Padding: "10" },
                            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" }),
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" })),
                            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" }),
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" })),
                            XNode_1.default.create(XF_1.default.Image, Object.assign({}, XF_1.default.Grid.rowSpan(2), { Source: Bind_1.default.twoWays((x) => x.data.image), Aspect: "AspectFill", HeightRequest: "60", WidthRequest: "60" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.name), FontAttributes: "Bold" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.location), FontAttributes: "Italic", VerticalOptions: "End" })))))));
        }
    }
    exports.default = HeaderFooterSample;
});
//# sourceMappingURL=HeaderFooterSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/header-footer/HeaderFooterSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class HorizontalGridViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.list = [];
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
                for (let i = 0; i <= 6; i += 1) {
                    const element = {};
                    element.image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg";
                    element.location = "location " + i;
                    element.name = "name " + i;
                    this.list.add(element);
                }
            });
        }
    }
    exports.default = HorizontalGridViewModel;
});
//# sourceMappingURL=HorizontalGridViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/horizontal-grid/HorizontalGridViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./HorizontalGridViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const HorizontalGridViewModel_1 = require("./HorizontalGridViewModel");
    class HorizontalGridSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(HorizontalGridViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Horizontal Grid Sample" },
                XNode_1.default.create(XF_1.default.CollectionView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.list) },
                    XNode_1.default.create(XF_1.default.CollectionView.itemsLayout, null,
                        XNode_1.default.create(XF_1.default.GridItemsLayout, { span: 4, orientation: "Horizontal" })),
                    XNode_1.default.create(XF_1.default.CollectionView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.Grid, { Padding: "10" },
                            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: 35 }),
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: 35 })),
                            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: 70 }),
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: 140 })),
                            XNode_1.default.create(XF_1.default.Image, Object.assign({}, XF_1.default.Grid.rowSpan(2), { Source: Bind_1.default.twoWays((x) => x.data.image), Aspect: "AspectFill", HeightRequest: "60", WidthRequest: "60" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.name), fontAttributes: "Bold", lineBreakMode: "TailTruncation" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.location), FontAttributes: "Italic", VerticalOptions: "End", LineBreakMode: "TailTruncation" })))))));
        }
    }
    exports.default = HorizontalGridSample;
});
//# sourceMappingURL=HorizontalGridSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/horizontal-grid/HorizontalGridSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class HorizontalListViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.list = [];
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
                for (let i = 0; i <= 6; i += 1) {
                    const element = {};
                    element.image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg";
                    element.location = "location " + i;
                    element.name = "name " + i;
                    this.list.add(element);
                }
            });
        }
    }
    exports.default = HorizontalListViewModel;
});
//# sourceMappingURL=HorizontalListViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/horizontal-list/HorizontalListViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./HorizontalListViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const HorizontalListViewModel_1 = require("./HorizontalListViewModel");
    class HorizontalListSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(HorizontalListViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Horizontal List Sample" },
                XNode_1.default.create(XF_1.default.CollectionView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.list), itemsLayout: "HorizontalList" },
                    XNode_1.default.create(XF_1.default.CollectionView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.Grid, { Padding: "10" },
                            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" }),
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" })),
                            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" }),
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" })),
                            XNode_1.default.create(XF_1.default.Image, Object.assign({}, XF_1.default.Grid.rowSpan(2), { Source: Bind_1.default.twoWays((x) => x.data.image), Aspect: "AspectFill", HeightRequest: "60", WidthRequest: "60" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.name), FontAttributes: "Bold", LineBreakMode: "TailTruncation" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.location), FontAttributes: "Italic", VerticalOptions: "End", LineBreakMode: "TailTruncation" })))))));
        }
    }
    exports.default = HorizontalListSample;
});
//# sourceMappingURL=HorizontalListSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/horizontal-list/HorizontalListSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class VerticalGridViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.list = [];
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
                for (let i = 0; i <= 6; i += 1) {
                    const element = {};
                    element.image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg";
                    element.location = "location " + i;
                    element.name = "name " + i;
                    this.list.add(element);
                }
            });
        }
    }
    exports.default = VerticalGridViewModel;
});
//# sourceMappingURL=VerticalGridViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/vertical-grid/VerticalGridViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./VerticalGridViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const VerticalGridViewModel_1 = require("./VerticalGridViewModel");
    class VerticalGridSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(VerticalGridViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Vertical Grid Sample" },
                XNode_1.default.create(XF_1.default.CollectionView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.list) },
                    XNode_1.default.create(XF_1.default.CollectionView.itemsLayout, null,
                        XNode_1.default.create(XF_1.default.GridItemsLayout, { span: 2, orientation: "Vertical" })),
                    XNode_1.default.create(XF_1.default.CollectionView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.Grid, { Padding: "10" },
                            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: 35 }),
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: 35 })),
                            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: 70 }),
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: 80 })),
                            XNode_1.default.create(XF_1.default.Image, Object.assign({}, XF_1.default.Grid.rowSpan(2), { Source: Bind_1.default.twoWays((x) => x.data.image), Aspect: "AspectFill", HeightRequest: "60", WidthRequest: "60" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.name), FontAttributes: "Bold", LineBreakMode: "TailTruncation" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.location), FontAttributes: "Italic", VerticalOptions: "End", LineBreakMode: "TailTruncation" })))))));
        }
    }
    exports.default = VerticalGridSample;
});
//# sourceMappingURL=VerticalGridSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/vertical-grid/VerticalGridSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class VerticalListViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.list = [];
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
                for (let i = 0; i <= 6; i += 1) {
                    const element = {};
                    element.image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg";
                    element.location = "location " + i;
                    element.name = "name " + i;
                    this.list.add(element);
                }
            });
        }
    }
    exports.default = VerticalListViewModel;
});
//# sourceMappingURL=VerticalListViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/vertical-list/VerticalListViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./VerticalListViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const VerticalListViewModel_1 = require("./VerticalListViewModel");
    class VerticalListSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(VerticalListViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Vertical List Sample" },
                XNode_1.default.create(XF_1.default.CollectionView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.list), itemsLayout: "VerticalList" },
                    XNode_1.default.create(XF_1.default.CollectionView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.Grid, { Padding: "10" },
                            XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" }),
                                XNode_1.default.create(XF_1.default.RowDefinition, { Height: "Auto" })),
                            XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" }),
                                XNode_1.default.create(XF_1.default.ColumnDefinition, { Width: "Auto" })),
                            XNode_1.default.create(XF_1.default.Image, Object.assign({}, XF_1.default.Grid.rowSpan(2), { Source: Bind_1.default.twoWays((x) => x.data.image), Aspect: "AspectFill", HeightRequest: "60", WidthRequest: "60" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.name), FontAttributes: "Bold" })),
                            XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), XF_1.default.Grid.column(1), { text: Bind_1.default.twoWays((x) => x.data.location), FontAttributes: "Italic", VerticalOptions: "End" })))))));
        }
    }
    exports.default = VerticalListSample;
});
//# sourceMappingURL=VerticalListSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/vertical-list/VerticalListSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./grouping/GroupingSample", "./header-footer/HeaderFooterSample", "./horizontal-grid/HorizontalGridSample", "./horizontal-list/HorizontalListSample", "./vertical-grid/VerticalGridSample", "./vertical-list/VerticalListSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const GroupingSample_1 = require("./grouping/GroupingSample");
    const HeaderFooterSample_1 = require("./header-footer/HeaderFooterSample");
    const HorizontalGridSample_1 = require("./horizontal-grid/HorizontalGridSample");
    const HorizontalListSample_1 = require("./horizontal-list/HorizontalListSample");
    const VerticalGridSample_1 = require("./vertical-grid/VerticalGridSample");
    const VerticalListSample_1 = require("./vertical-list/VerticalListSample");
    function addCollectionViewSample(ms) {
        const collection = ms.addGroup("Collection View");
        collection.addTabLink("Vertical List", VerticalListSample_1.default);
        collection.addTabLink("Horizontal List", HorizontalListSample_1.default);
        collection.addTabLink("Vertical Grid", VerticalGridSample_1.default);
        collection.addTabLink("Horizontal Grid", HorizontalGridSample_1.default);
        collection.addTabLink("Header & Footer", HeaderFooterSample_1.default);
        collection.addTabLink("Grouping", GroupingSample_1.default);
    }
    exports.default = addCollectionViewSample;
});
//# sourceMappingURL=CollectionViewSamplePage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/collection-view/CollectionViewSamplePage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    var _this = this;
    Object.defineProperty(exports, "__esModule", { value: true });
    var FontAwesomeRegular = {
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/address-book.svg.png) Address Book
         * Image Copyright FontAwesome.com
        */
        addressBook: "\uf2b9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/address-card.svg.png) Address Card
         * Image Copyright FontAwesome.com
        */
        addressCard: "\uf2bb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/angry.svg.png) Angry Face
         * Image Copyright FontAwesome.com
        */
        angry: "\uf556",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/arrow-alt-circle-down.svg.png) Alternate Arrow Circle Down
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleDown: "\uf358",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/arrow-alt-circle-left.svg.png) Alternate Arrow Circle Left
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleLeft: "\uf359",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/arrow-alt-circle-right.svg.png) Alternate Arrow Circle Right
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleRight: "\uf35a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/arrow-alt-circle-up.svg.png) Alternate Arrow Circle Up
         * Image Copyright FontAwesome.com
        */
        arrowAltCircleUp: "\uf35b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/bell.svg.png) bell
         * Image Copyright FontAwesome.com
        */
        bell: "\uf0f3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/bell-slash.svg.png) Bell Slash
         * Image Copyright FontAwesome.com
        */
        bellSlash: "\uf1f6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/bookmark.svg.png) bookmark
         * Image Copyright FontAwesome.com
        */
        bookmark: "\uf02e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/building.svg.png) Building
         * Image Copyright FontAwesome.com
        */
        building: "\uf1ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/calendar.svg.png) Calendar
         * Image Copyright FontAwesome.com
        */
        calendar: "\uf133",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/calendar-alt.svg.png) Alternate Calendar
         * Image Copyright FontAwesome.com
        */
        calendarAlt: "\uf073",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/calendar-check.svg.png) Calendar Check
         * Image Copyright FontAwesome.com
        */
        calendarCheck: "\uf274",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/calendar-minus.svg.png) Calendar Minus
         * Image Copyright FontAwesome.com
        */
        calendarMinus: "\uf272",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/calendar-plus.svg.png) Calendar Plus
         * Image Copyright FontAwesome.com
        */
        calendarPlus: "\uf271",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/calendar-times.svg.png) Calendar Times
         * Image Copyright FontAwesome.com
        */
        calendarTimes: "\uf273",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/caret-square-down.svg.png) Caret Square Down
         * Image Copyright FontAwesome.com
        */
        caretSquareDown: "\uf150",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/caret-square-left.svg.png) Caret Square Left
         * Image Copyright FontAwesome.com
        */
        caretSquareLeft: "\uf191",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/caret-square-right.svg.png) Caret Square Right
         * Image Copyright FontAwesome.com
        */
        caretSquareRight: "\uf152",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/caret-square-up.svg.png) Caret Square Up
         * Image Copyright FontAwesome.com
        */
        caretSquareUp: "\uf151",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/chart-bar.svg.png) Bar Chart
         * Image Copyright FontAwesome.com
        */
        chartBar: "\uf080",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/check-circle.svg.png) Check Circle
         * Image Copyright FontAwesome.com
        */
        checkCircle: "\uf058",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/check-square.svg.png) Check Square
         * Image Copyright FontAwesome.com
        */
        checkSquare: "\uf14a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/circle.svg.png) Circle
         * Image Copyright FontAwesome.com
        */
        circle: "\uf111",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/clipboard.svg.png) Clipboard
         * Image Copyright FontAwesome.com
        */
        clipboard: "\uf328",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/clock.svg.png) Clock
         * Image Copyright FontAwesome.com
        */
        clock: "\uf017",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/clone.svg.png) Clone
         * Image Copyright FontAwesome.com
        */
        clone: "\uf24d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/closed-captioning.svg.png) Closed Captioning
         * Image Copyright FontAwesome.com
        */
        closedCaptioning: "\uf20a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/comment.svg.png) comment
         * Image Copyright FontAwesome.com
        */
        comment: "\uf075",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/comment-alt.svg.png) Alternate Comment
         * Image Copyright FontAwesome.com
        */
        commentAlt: "\uf27a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/comment-dots.svg.png) Comment Dots
         * Image Copyright FontAwesome.com
        */
        commentDots: "\uf4ad",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/comments.svg.png) comments
         * Image Copyright FontAwesome.com
        */
        comments: "\uf086",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/compass.svg.png) Compass
         * Image Copyright FontAwesome.com
        */
        compass: "\uf14e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/copy.svg.png) Copy
         * Image Copyright FontAwesome.com
        */
        copy: "\uf0c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/copyright.svg.png) Copyright
         * Image Copyright FontAwesome.com
        */
        copyright: "\uf1f9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/credit-card.svg.png) Credit Card
         * Image Copyright FontAwesome.com
        */
        creditCard: "\uf09d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/dizzy.svg.png) Dizzy Face
         * Image Copyright FontAwesome.com
        */
        dizzy: "\uf567",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/dot-circle.svg.png) Dot Circle
         * Image Copyright FontAwesome.com
        */
        dotCircle: "\uf192",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/edit.svg.png) Edit
         * Image Copyright FontAwesome.com
        */
        edit: "\uf044",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/envelope.svg.png) Envelope
         * Image Copyright FontAwesome.com
        */
        envelope: "\uf0e0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/envelope-open.svg.png) Envelope Open
         * Image Copyright FontAwesome.com
        */
        envelopeOpen: "\uf2b6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/eye.svg.png) Eye
         * Image Copyright FontAwesome.com
        */
        eye: "\uf06e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/eye-slash.svg.png) Eye Slash
         * Image Copyright FontAwesome.com
        */
        eyeSlash: "\uf070",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file.svg.png) File
         * Image Copyright FontAwesome.com
        */
        file: "\uf15b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-alt.svg.png) Alternate File
         * Image Copyright FontAwesome.com
        */
        fileAlt: "\uf15c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-archive.svg.png) Archive File
         * Image Copyright FontAwesome.com
        */
        fileArchive: "\uf1c6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-audio.svg.png) Audio File
         * Image Copyright FontAwesome.com
        */
        fileAudio: "\uf1c7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-code.svg.png) Code File
         * Image Copyright FontAwesome.com
        */
        fileCode: "\uf1c9",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-excel.svg.png) Excel File
         * Image Copyright FontAwesome.com
        */
        fileExcel: "\uf1c3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-image.svg.png) Image File
         * Image Copyright FontAwesome.com
        */
        fileImage: "\uf1c5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-pdf.svg.png) PDF File
         * Image Copyright FontAwesome.com
        */
        filePdf: "\uf1c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-powerpoint.svg.png) Powerpoint File
         * Image Copyright FontAwesome.com
        */
        filePowerpoint: "\uf1c4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-video.svg.png) Video File
         * Image Copyright FontAwesome.com
        */
        fileVideo: "\uf1c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/file-word.svg.png) Word File
         * Image Copyright FontAwesome.com
        */
        fileWord: "\uf1c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/flag.svg.png) flag
         * Image Copyright FontAwesome.com
        */
        flag: "\uf024",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/flushed.svg.png) Flushed Face
         * Image Copyright FontAwesome.com
        */
        flushed: "\uf579",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/folder.svg.png) Folder
         * Image Copyright FontAwesome.com
        */
        folder: "\uf07b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/folder-open.svg.png) Folder Open
         * Image Copyright FontAwesome.com
        */
        folderOpen: "\uf07c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/font-awesome-logo-full.svg.png) Font Awesome Full Logo
         * Image Copyright FontAwesome.com
        */
        fontAwesomeLogoFull: "\uf4e6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/frown.svg.png) Frowning Face
         * Image Copyright FontAwesome.com
        */
        frown: "\uf119",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/frown-open.svg.png) Frowning Face With Open Mouth
         * Image Copyright FontAwesome.com
        */
        frownOpen: "\uf57a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/futbol.svg.png) Futbol
         * Image Copyright FontAwesome.com
        */
        futbol: "\uf1e3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/gem.svg.png) Gem
         * Image Copyright FontAwesome.com
        */
        gem: "\uf3a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grimace.svg.png) Grimacing Face
         * Image Copyright FontAwesome.com
        */
        grimace: "\uf57f",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin.svg.png) Grinning Face
         * Image Copyright FontAwesome.com
        */
        grin: "\uf580",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-alt.svg.png) Alternate Grinning Face
         * Image Copyright FontAwesome.com
        */
        grinAlt: "\uf581",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-beam.svg.png) Grinning Face With Smiling Eyes
         * Image Copyright FontAwesome.com
        */
        grinBeam: "\uf582",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-beam-sweat.svg.png) Grinning Face With Sweat
         * Image Copyright FontAwesome.com
        */
        grinBeamSweat: "\uf583",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-hearts.svg.png) Smiling Face With Heart-Eyes
         * Image Copyright FontAwesome.com
        */
        grinHearts: "\uf584",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-squint.svg.png) Grinning Squinting Face
         * Image Copyright FontAwesome.com
        */
        grinSquint: "\uf585",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-squint-tears.svg.png) Rolling on the Floor Laughing
         * Image Copyright FontAwesome.com
        */
        grinSquintTears: "\uf586",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-stars.svg.png) Star-Struck
         * Image Copyright FontAwesome.com
        */
        grinStars: "\uf587",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-tears.svg.png) Face With Tears of Joy
         * Image Copyright FontAwesome.com
        */
        grinTears: "\uf588",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-tongue.svg.png) Face With Tongue
         * Image Copyright FontAwesome.com
        */
        grinTongue: "\uf589",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-tongue-squint.svg.png) Squinting Face With Tongue
         * Image Copyright FontAwesome.com
        */
        grinTongueSquint: "\uf58a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-tongue-wink.svg.png) Winking Face With Tongue
         * Image Copyright FontAwesome.com
        */
        grinTongueWink: "\uf58b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/grin-wink.svg.png) Grinning Winking Face
         * Image Copyright FontAwesome.com
        */
        grinWink: "\uf58c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-lizard.svg.png) Lizard (Hand)
         * Image Copyright FontAwesome.com
        */
        handLizard: "\uf258",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-paper.svg.png) Paper (Hand)
         * Image Copyright FontAwesome.com
        */
        handPaper: "\uf256",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-peace.svg.png) Peace (Hand)
         * Image Copyright FontAwesome.com
        */
        handPeace: "\uf25b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-point-down.svg.png) Hand Pointing Down
         * Image Copyright FontAwesome.com
        */
        handPointDown: "\uf0a7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-point-left.svg.png) Hand Pointing Left
         * Image Copyright FontAwesome.com
        */
        handPointLeft: "\uf0a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-point-right.svg.png) Hand Pointing Right
         * Image Copyright FontAwesome.com
        */
        handPointRight: "\uf0a4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-point-up.svg.png) Hand Pointing Up
         * Image Copyright FontAwesome.com
        */
        handPointUp: "\uf0a6",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-pointer.svg.png) Pointer (Hand)
         * Image Copyright FontAwesome.com
        */
        handPointer: "\uf25a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-rock.svg.png) Rock (Hand)
         * Image Copyright FontAwesome.com
        */
        handRock: "\uf255",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-scissors.svg.png) Scissors (Hand)
         * Image Copyright FontAwesome.com
        */
        handScissors: "\uf257",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hand-spock.svg.png) Spock (Hand)
         * Image Copyright FontAwesome.com
        */
        handSpock: "\uf259",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/handshake.svg.png) Handshake
         * Image Copyright FontAwesome.com
        */
        handshake: "\uf2b5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hdd.svg.png) HDD
         * Image Copyright FontAwesome.com
        */
        hdd: "\uf0a0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/heart.svg.png) Heart
         * Image Copyright FontAwesome.com
        */
        heart: "\uf004",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hospital.svg.png) hospital
         * Image Copyright FontAwesome.com
        */
        hospital: "\uf0f8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/hourglass.svg.png) Hourglass
         * Image Copyright FontAwesome.com
        */
        hourglass: "\uf254",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/id-badge.svg.png) Identification Badge
         * Image Copyright FontAwesome.com
        */
        idBadge: "\uf2c1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/id-card.svg.png) Identification Card
         * Image Copyright FontAwesome.com
        */
        idCard: "\uf2c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/image.svg.png) Image
         * Image Copyright FontAwesome.com
        */
        image: "\uf03e",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/images.svg.png) Images
         * Image Copyright FontAwesome.com
        */
        images: "\uf302",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/keyboard.svg.png) Keyboard
         * Image Copyright FontAwesome.com
        */
        keyboard: "\uf11c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/kiss.svg.png) Kissing Face
         * Image Copyright FontAwesome.com
        */
        kiss: "\uf596",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/kiss-beam.svg.png) Kissing Face With Smiling Eyes
         * Image Copyright FontAwesome.com
        */
        kissBeam: "\uf597",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/kiss-wink-heart.svg.png) Face Blowing a Kiss
         * Image Copyright FontAwesome.com
        */
        kissWinkHeart: "\uf598",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/laugh.svg.png) Grinning Face With Big Eyes
         * Image Copyright FontAwesome.com
        */
        laugh: "\uf599",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/laugh-beam.svg.png) Laugh Face with Beaming Eyes
         * Image Copyright FontAwesome.com
        */
        laughBeam: "\uf59a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/laugh-squint.svg.png) Laughing Squinting Face
         * Image Copyright FontAwesome.com
        */
        laughSquint: "\uf59b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/laugh-wink.svg.png) Laughing Winking Face
         * Image Copyright FontAwesome.com
        */
        laughWink: "\uf59c",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/lemon.svg.png) Lemon
         * Image Copyright FontAwesome.com
        */
        lemon: "\uf094",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/life-ring.svg.png) Life Ring
         * Image Copyright FontAwesome.com
        */
        lifeRing: "\uf1cd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/lightbulb.svg.png) Lightbulb
         * Image Copyright FontAwesome.com
        */
        lightbulb: "\uf0eb",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/list-alt.svg.png) Alternate List
         * Image Copyright FontAwesome.com
        */
        listAlt: "\uf022",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/map.svg.png) Map
         * Image Copyright FontAwesome.com
        */
        map: "\uf279",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/meh.svg.png) Neutral Face
         * Image Copyright FontAwesome.com
        */
        meh: "\uf11a",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/meh-blank.svg.png) Face Without Mouth
         * Image Copyright FontAwesome.com
        */
        mehBlank: "\uf5a4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/meh-rolling-eyes.svg.png) Face With Rolling Eyes
         * Image Copyright FontAwesome.com
        */
        mehRollingEyes: "\uf5a5",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/minus-square.svg.png) Minus Square
         * Image Copyright FontAwesome.com
        */
        minusSquare: "\uf146",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/money-bill-alt.svg.png) Alternate Money Bill
         * Image Copyright FontAwesome.com
        */
        moneyBillAlt: "\uf3d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/moon.svg.png) Moon
         * Image Copyright FontAwesome.com
        */
        moon: "\uf186",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/newspaper.svg.png) Newspaper
         * Image Copyright FontAwesome.com
        */
        newspaper: "\uf1ea",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/object-group.svg.png) Object Group
         * Image Copyright FontAwesome.com
        */
        objectGroup: "\uf247",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/object-ungroup.svg.png) Object Ungroup
         * Image Copyright FontAwesome.com
        */
        objectUngroup: "\uf248",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/paper-plane.svg.png) Paper Plane
         * Image Copyright FontAwesome.com
        */
        paperPlane: "\uf1d8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/pause-circle.svg.png) Pause Circle
         * Image Copyright FontAwesome.com
        */
        pauseCircle: "\uf28b",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/play-circle.svg.png) Play Circle
         * Image Copyright FontAwesome.com
        */
        playCircle: "\uf144",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/plus-square.svg.png) Plus Square
         * Image Copyright FontAwesome.com
        */
        plusSquare: "\uf0fe",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/question-circle.svg.png) Question Circle
         * Image Copyright FontAwesome.com
        */
        questionCircle: "\uf059",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/registered.svg.png) Registered Trademark
         * Image Copyright FontAwesome.com
        */
        registered: "\uf25d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/sad-cry.svg.png) Crying Face
         * Image Copyright FontAwesome.com
        */
        sadCry: "\uf5b3",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/sad-tear.svg.png) Loudly Crying Face
         * Image Copyright FontAwesome.com
        */
        sadTear: "\uf5b4",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/save.svg.png) Save
         * Image Copyright FontAwesome.com
        */
        save: "\uf0c7",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/share-square.svg.png) Share Square
         * Image Copyright FontAwesome.com
        */
        shareSquare: "\uf14d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/smile.svg.png) Smiling Face
         * Image Copyright FontAwesome.com
        */
        smile: "\uf118",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/smile-beam.svg.png) Beaming Face With Smiling Eyes
         * Image Copyright FontAwesome.com
        */
        smileBeam: "\uf5b8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/smile-wink.svg.png) Winking Face
         * Image Copyright FontAwesome.com
        */
        smileWink: "\uf4da",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/snowflake.svg.png) Snowflake
         * Image Copyright FontAwesome.com
        */
        snowflake: "\uf2dc",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/square.svg.png) Square
         * Image Copyright FontAwesome.com
        */
        square: "\uf0c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/star.svg.png) Star
         * Image Copyright FontAwesome.com
        */
        star: "\uf005",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/star-half.svg.png) star-half
         * Image Copyright FontAwesome.com
        */
        starHalf: "\uf089",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/sticky-note.svg.png) Sticky Note
         * Image Copyright FontAwesome.com
        */
        stickyNote: "\uf249",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/stop-circle.svg.png) Stop Circle
         * Image Copyright FontAwesome.com
        */
        stopCircle: "\uf28d",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/sun.svg.png) Sun
         * Image Copyright FontAwesome.com
        */
        sun: "\uf185",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/surprise.svg.png) Hushed Face
         * Image Copyright FontAwesome.com
        */
        surprise: "\uf5c2",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/thumbs-down.svg.png) thumbs-down
         * Image Copyright FontAwesome.com
        */
        thumbsDown: "\uf165",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/thumbs-up.svg.png) thumbs-up
         * Image Copyright FontAwesome.com
        */
        thumbsUp: "\uf164",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/times-circle.svg.png) Times Circle
         * Image Copyright FontAwesome.com
        */
        timesCircle: "\uf057",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/tired.svg.png) Tired Face
         * Image Copyright FontAwesome.com
        */
        tired: "\uf5c8",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/trash-alt.svg.png) Alternate Trash
         * Image Copyright FontAwesome.com
        */
        trashAlt: "\uf2ed",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/user.svg.png) User
         * Image Copyright FontAwesome.com
        */
        user: "\uf007",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/user-circle.svg.png) User Circle
         * Image Copyright FontAwesome.com
        */
        userCircle: "\uf2bd",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/window-close.svg.png) Window Close
         * Image Copyright FontAwesome.com
        */
        windowClose: "\uf410",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/window-maximize.svg.png) Window Maximize
         * Image Copyright FontAwesome.com
        */
        windowMaximize: "\uf2d0",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/window-minimize.svg.png) Window Minimize
         * Image Copyright FontAwesome.com
        */
        windowMinimize: "\uf2d1",
        /** ![Image](https://cdn.jsdelivr.net/npm/@web-atoms/font-awesome-pngs@1.0.22/pngs/regular/window-restore.svg.png) Window Restore
         * Image Copyright FontAwesome.com
        */
        windowRestore: "\uf2d2",
        toString: function () {
            var name = _this._fontName;
            if (name) {
                return name;
            }
            var p = bridge.platform;
            if (p) {
                if (/android/i.test(p)) {
                    name = "Font Awesome 5 Free-Regular-400.otf#Font Awesome 5 Free Regular";
                }
                else if (/ios/i.test(p)) {
                    name = "FontAwesome5Free-Regular";
                }
            }
            else {
                name = "Font Awesome 5 Free";
            }
            _this._fontName = name;
            return name;
        }
    };
    exports.default = FontAwesomeRegular;
});
//# sourceMappingURL=FontAwesomeRegular.js.map

    AmdLoader.instance.setup("@web-atoms/font-awesome/dist/FontAwesomeRegular");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../services/NavigationService", "./baseTypes"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const NavigationService_1 = require("../services/NavigationService");
    const baseTypes_1 = require("./baseTypes");
    /**
     * Reports an alert to user when an error has occurred
     * or validation has failed.
     * If you set success message, it will display an alert with success message.
     * If you set confirm message, it will ask form confirmation before executing this method.
     * You can configure options to enable/disable certain
     * alerts.
     * @param reportOptions
     */
    function Action({ success = null, successTitle = "Done", confirm = null, confirmTitle = null, validate = false, validateTitle = null } = {}) {
        // tslint:disable-next-line: only-arrow-functions
        return function (target, key) {
            baseTypes_1.registerInit(target, (vm) => {
                // tslint:disable-next-line: ban-types
                const oldMethod = vm[key];
                const app = vm.app;
                // tslint:disable-next-line:only-arrow-functions
                vm[key] = function (...a) {
                    return __awaiter(this, void 0, void 0, function* () {
                        const ns = app.resolve(NavigationService_1.NavigationService);
                        try {
                            if (validate) {
                                if (!vm.isValid) {
                                    const vMsg = typeof validate === "boolean"
                                        ? "Please enter correct information"
                                        : validate;
                                    yield ns.alert(vMsg, validateTitle || "Error");
                                    return;
                                }
                            }
                            if (confirm) {
                                if (!(yield ns.confirm(confirm, confirmTitle || "Confirm"))) {
                                    return;
                                }
                            }
                            const pe = oldMethod.apply(vm, a);
                            if (pe && pe.then) {
                                const result = yield pe;
                                if (success) {
                                    yield ns.alert(success, successTitle);
                                }
                                return result;
                            }
                        }
                        catch (e) {
                            if (/^(cancelled|canceled)$/i.test(e.toString().trim())) {
                                // tslint:disable-next-line: no-console
                                console.warn(e);
                                return;
                            }
                            yield ns.alert(e, "Error");
                        }
                    });
                };
            });
        };
    }
    exports.default = Action;
});
//# sourceMappingURL=Action.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/Action");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    class QueryFragments {
        constructor(separator, prefix) {
            this.fragments = [];
            this.prefix = prefix;
            this.separator = separator;
        }
        add(query, ...args) {
            const isEmpty = this.fragments.length === 0;
            if (!(query instanceof QueryObject)) {
                query = QueryObject.create(query, ...args);
            }
            // this.fragments.push(query);
            const fa = query.fragments;
            this.fragments.push(fa);
            return this;
        }
        toQuery() {
            const f = [];
            if (this.prefix) {
                f.push({ literal: this.prefix });
            }
            let i = 0;
            for (const iterator of this.fragments) {
                if (i++) {
                    f.push({ literal: this.separator });
                }
                for (const fa of iterator) {
                    f.push(fa);
                }
            }
            return new QueryObject(f);
        }
    }
    function fragments(prefix, separator) {
        return new QueryFragments(separator ? separator : prefix, separator ? prefix : "");
    }
    class QueryObject {
        constructor(text, args) {
            this.fragments = [];
            if (!text) {
                return;
            }
            if (typeof text !== "string" && Array.isArray(text)) {
                this.fragments = text;
                return;
            }
            if (!args) {
                return;
            }
            for (let i = 0; i < args.length; i++) {
                const sep = `{${i}}`;
                const index = text.indexOf(sep);
                const prefix = text.substring(0, index);
                text = text.substring(index + sep.length);
                const arg = args[i];
                this.fragments.push({ literal: prefix });
                if (arg instanceof QueryObject) {
                    this.fragments = this.fragments.concat(arg.fragments);
                }
                else if (arg instanceof QueryFragments) {
                    const qf = arg.toQuery();
                    this.fragments = this.fragments.concat(qf.fragments);
                }
                else if (typeof arg !== "string" && Array.isArray(arg)) {
                    let i2 = 0;
                    for (const iterator of arg) {
                        if (i2++) {
                            this.fragments.push({ literal: "," });
                        }
                        this.fragments.push({ hasArgument: true, argument: iterator });
                    }
                }
                else {
                    this.fragments.push({ hasArgument: true, argument: arg });
                }
            }
            this.fragments.push({ literal: text });
        }
        static create(query, ...args) {
            const q = new QueryObject(null);
            for (let index = 0; index < args.length; index++) {
                const element = args[index];
                const raw = query.raw[index];
                if (raw) {
                    q.fragments.push({ literal: raw });
                }
                if (element instanceof QueryObject) {
                    q.fragments = q.fragments.concat(element.fragments);
                }
                else if (element instanceof QueryFragments) {
                    q.fragments = q.fragments.concat(element.toQuery().fragments);
                }
                else {
                    q.fragments.push({ hasArgument: true, argument: element });
                }
            }
            const last = query.raw[args.length];
            if (last) {
                q.fragments.push({ literal: last });
            }
            return q;
        }
        static literal(text, escape) {
            const q = new QueryObject(null);
            q.fragments.push({ literal: escape
                    ? escape(text)
                    : text });
            return q;
        }
        /**
         * Returns key value pair style parameters and command
         * @param prefix Parameter Prefix
         */
        toQuery(prefix) {
            const args = {};
            prefix = prefix || "@p";
            let s = "";
            let i = 0;
            for (const iterator of this.fragments) {
                if (iterator.hasArgument) {
                    const a = prefix + i;
                    args[a] = iterator.argument;
                    i++;
                    s += a;
                }
                else {
                    s += iterator.literal;
                }
            }
            return {
                command: s,
                parameters: args
            };
        }
        /**
         * Returns question mark and arguments array
         */
        toQueryArguments() {
            const args = [];
            let s = "";
            for (const iterator of this.fragments) {
                if (iterator.hasArgument) {
                    args.push(iterator.argument);
                    s += "?";
                }
                else {
                    s += iterator.literal;
                }
            }
            return {
                command: s,
                arguments: args
            };
        }
    }
    const Query = {
        create: QueryObject.create,
        literal: QueryObject.literal,
        fragments
    };
    exports.default = Query;
});
//# sourceMappingURL=Query.js.map

    AmdLoader.instance.setup("@web-atoms/storage/dist/query/Query");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/DISingleton", "../../query/Query"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.SqliteConnection = exports.SqliteTransaction = void 0;
    const DISingleton_1 = require("@web-atoms/core/dist/di/DISingleton");
    const Query_1 = require("../../query/Query");
    function escapeLiteral(name) {
        return `"${name.replace(/\"/g, "\\\"")}"`;
    }
    class SqliteTransaction {
        constructor(tx) {
            this.tx = tx;
        }
        executeSql(text, p, cb) {
            this.tx.executeSql(text, p, cb);
        }
        executeSqlAsync(text, p) {
            return this.tx.executeSqlAsync(text, p);
        }
        insertAsync(table, obj) {
            table = escapeLiteral(table);
            const fields = [];
            const values = [];
            const params = [];
            for (const key in obj) {
                if (obj.hasOwnProperty(key)) {
                    const element = obj[key];
                    if (key.startsWith("_$_")
                        || (typeof element === "object" && !(element instanceof Date))
                        || Array.isArray(element)
                        || element === undefined) {
                        continue;
                    }
                    params.push("?");
                    fields.push(escapeLiteral(key));
                    values.push(element);
                }
            }
            const sql = `INSERT INTO ${table} (${fields.join(",")}) VALUES (${params.join(",")})`;
            return this.executeSqlAsync(sql, values);
        }
        updateAsync(table, obj, filter) {
            let set = Query_1.default.fragments(" SET ", " , ");
            for (const key in obj) {
                if (obj.hasOwnProperty(key)) {
                    const element = obj[key];
                    if (key.startsWith("_$_")
                        || (typeof element === "object" && !(element instanceof Date))
                        || Array.isArray(element)
                        || element === undefined) {
                        continue;
                    }
                    const name = Query_1.default.literal(key, escapeLiteral);
                    set = set.add ` ${name} = ${element} `;
                }
            }
            const tableName = Query_1.default.literal(table, escapeLiteral);
            const sql = filter
                ? Query_1.default.create `UPDATE ${tableName} ${set} WHERE ${filter}`
                : Query_1.default.create `UPDATE ${tableName} ${set}`;
            const q = sql.toQueryArguments();
            return this.executeSqlAsync(q.command, q.arguments);
        }
        deleteAsync(table, filter) {
            const tableName = Query_1.default.literal(table, escapeLiteral);
            const sql = filter
                ? Query_1.default.create `DELETE FROM ${tableName} WHERE ${filter}`
                : Query_1.default.create `DELETE FROM ${tableName} `;
            const q = sql.toQueryArguments();
            return this.executeSqlAsync(q.command, q.arguments);
        }
    }
    exports.SqliteTransaction = SqliteTransaction;
    class SqliteConnection {
        constructor(conn) {
            this.conn = conn;
        }
        transaction(tx, callback) {
            return this.conn.transaction((x) => tx(new SqliteTransaction(x)), callback);
        }
        transactionAsync(tx) {
            return this.conn.transactionAsync((x) => tx(new SqliteTransaction(x)));
        }
    }
    exports.SqliteConnection = SqliteConnection;
    let SqliteService = class SqliteService {
        /**
         * Opens database on the device, if the version is different, old database is deleted
         * and new empty one is created
         * @param file file name of Database
         * @param version version
         * @param name Description
         * @param size Ignored on Web Atoms
         */
        openDatabase(file, version, name, size) {
            return new SqliteConnection(bridge.database.openDatabase(file, version, name, size));
        }
    };
    SqliteService = __decorate([
        DISingleton_1.default()
    ], SqliteService);
    exports.default = SqliteService;
});
//# sourceMappingURL=SqliteService.js.map

    AmdLoader.instance.setup("@web-atoms/storage/dist/database/sqlite/SqliteService");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomWindowViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomWindowViewModel_1 = require("@web-atoms/core/dist/view-model/AtomWindowViewModel");
    class RowEditorViewModel extends AtomWindowViewModel_1.AtomWindowViewModel {
        constructor() {
            super(...arguments);
            this.model = {};
        }
        save() {
            this.close(this.model);
        }
    }
    exports.default = RowEditorViewModel;
});
//# sourceMappingURL=RowEditorViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/database/web-sql/row-editor/RowEditorViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./RowEditorViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const RowEditorViewModel_1 = require("./RowEditorViewModel");
    class RowEditor extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(RowEditorViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, null,
                XNode_1.default.create(WA_1.default.AtomForm, null,
                    XNode_1.default.create(WA_1.default.AtomField, { label: "Id" },
                        XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => this.viewModel.model.id) })),
                    XNode_1.default.create(WA_1.default.AtomField, { label: "Name" },
                        XNode_1.default.create(XF_1.default.Entry, { text: Bind_1.default.twoWays(() => this.viewModel.model.name) })),
                    XNode_1.default.create(WA_1.default.AtomField, null,
                        XNode_1.default.create(XF_1.default.Button, { command: Bind_1.default.event(() => this.viewModel.save()), text: "Save" })))));
        }
    }
    exports.default = RowEditor;
});
//# sourceMappingURL=RowEditor.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/database/web-sql/row-editor/RowEditor");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/Action", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/view-model/Load", "@web-atoms/storage/dist/database/sqlite/SqliteService", "@web-atoms/storage/dist/query/Query", "./row-editor/RowEditor"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const Action_1 = require("@web-atoms/core/dist/view-model/Action");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const Load_1 = require("@web-atoms/core/dist/view-model/Load");
    const SqliteService_1 = require("@web-atoms/storage/dist/database/sqlite/SqliteService");
    const Query_1 = require("@web-atoms/storage/dist/query/Query");
    const RowEditor_1 = require("./row-editor/RowEditor");
    class WebSqlViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.rows = [];
            this.selectedRow = null;
        }
        loadDatabase() {
            return __awaiter(this, void 0, void 0, function* () {
                this.database = this.sqliteService.openDatabase("db", 1.1);
                yield this.database.transactionAsync((tx) => __awaiter(this, void 0, void 0, function* () {
                    let r = yield tx.executeSqlAsync("CREATE TABLE IF NOT EXISTS Customers(id INTEGER PRIMARY KEY AUTOINCREMENT,name text)", []);
                    r = yield tx.executeSqlAsync("SELECT * FROM Customers", []);
                    this.rows = r.rows;
                }));
            });
        }
        addRow() {
            return __awaiter(this, void 0, void 0, function* () {
                const model = yield this.navigationService.openPage(RowEditor_1.default, { model: {} });
                yield this.database.transactionAsync((tx) => __awaiter(this, void 0, void 0, function* () {
                    const c = yield tx.insertAsync("Customers", model);
                    model.id = c.insertId;
                    this.rows.add(model);
                    this.selectedRow = model;
                }));
            });
        }
        edit() {
            return __awaiter(this, void 0, void 0, function* () {
                const model = yield this.navigationService.openPage(RowEditor_1.default, { model: this.selectedRow });
                yield this.database.transactionAsync((tx) => __awaiter(this, void 0, void 0, function* () {
                    const filter = Query_1.default.create ` id = ${this.selectedRow.id}`;
                    yield tx.updateAsync("Customers", model, filter);
                    const r = yield tx.executeSqlAsync("SELECT * FROM Customers", []);
                    this.rows = r.rows;
                }));
            });
        }
        delete() {
            return __awaiter(this, void 0, void 0, function* () {
                yield this.database.transactionAsync((tx) => __awaiter(this, void 0, void 0, function* () {
                    const filter = Query_1.default.create ` id = ${this.selectedRow.id}`;
                    yield tx.deleteAsync("Customers", filter);
                    const r = yield tx.executeSqlAsync("SELECT * FROM Customers", []);
                    this.rows = r.rows;
                    this.selectedRow = null;
                }));
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", SqliteService_1.default)
    ], WebSqlViewModel.prototype, "sqliteService", void 0);
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], WebSqlViewModel.prototype, "navigationService", void 0);
    __decorate([
        Load_1.default({ init: true }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], WebSqlViewModel.prototype, "loadDatabase", null);
    __decorate([
        Action_1.default(),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], WebSqlViewModel.prototype, "addRow", null);
    __decorate([
        Action_1.default(),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], WebSqlViewModel.prototype, "edit", null);
    __decorate([
        Action_1.default({ confirm: "Are you sure you want to delete this?" }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], WebSqlViewModel.prototype, "delete", null);
    exports.default = WebSqlViewModel;
});
//# sourceMappingURL=WebSqlViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/database/web-sql/WebSqlViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/font-awesome/dist/FontAwesomeRegular", "@web-atoms/font-awesome/dist/FontAwesomeSolid", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./WebSqlViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const FontAwesomeRegular_1 = require("@web-atoms/font-awesome/dist/FontAwesomeRegular");
    const FontAwesomeSolid_1 = require("@web-atoms/font-awesome/dist/FontAwesomeSolid");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const WebSqlViewModel_1 = require("./WebSqlViewModel");
    function CommandButton(text, glyph, command) {
        return XNode_1.default.create(XF_1.default.Button, { text: text, command: command },
            XNode_1.default.create(XF_1.default.Button.imageSource, null,
                XNode_1.default.create(XF_1.default.FontImageSource, { fontFamily: FontAwesomeSolid_1.default, glyph: glyph })));
    }
    class WebSqlSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(WebSqlViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Sqlite Demo" },
                XNode_1.default.create(XF_1.default.ContentPage.toolbarItems, null,
                    XNode_1.default.create(WA_1.default.AtomToolbarItem, { command: Bind_1.default.event(() => this.viewModel.addRow()) },
                        XNode_1.default.create(XF_1.default.ToolbarItem.iconImageSource, null,
                            XNode_1.default.create(XF_1.default.FontImageSource, { fontFamily: FontAwesomeRegular_1.default, glyph: FontAwesomeRegular_1.default.plusSquare })))),
                XNode_1.default.create(XF_1.default.Grid, { rowDefinitions: "*, auto" },
                    XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.rows), selectedItem: Bind_1.default.twoWays(() => this.viewModel.selectedRow) },
                        XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.TextCell, { text: Bind_1.default.oneWay((x) => x.data.name), detail: Bind_1.default.oneWay((x) => x.data.id) })))),
                    XNode_1.default.create(XF_1.default.StackLayout, Object.assign({ isVisible: Bind_1.default.oneWay(() => this.viewModel.selectedRow ? true : false) }, XF_1.default.Grid.row(1), { orientation: "Horizontal" }),
                        CommandButton("Modify", FontAwesomeSolid_1.default.edit, Bind_1.default.event(() => this.viewModel.edit())),
                        CommandButton("Delete", FontAwesomeSolid_1.default.windowClose, Bind_1.default.event(() => this.viewModel.delete()))))));
        }
    }
    exports.default = WebSqlSample;
});
//# sourceMappingURL=WebSqlSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/database/web-sql/WebSqlSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./web-sql/WebSqlSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const WebSqlSample_1 = require("./web-sql/WebSqlSample");
    function addDatabaseSamples(ms) {
        const db = ms.addGroup("Database");
        db.addTabLink("Sqlite", WebSqlSample_1.default);
    }
    exports.default = addDatabaseSamples;
});
//# sourceMappingURL=addDatabaseSamples.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/database/addDatabaseSamples");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class ButtonViewModel extends AtomViewModel_1.AtomViewModel {
        clickEvent(str) {
            this.navigationService.alert(str + " is clicked..");
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], ButtonViewModel.prototype, "navigationService", void 0);
    exports.default = ButtonViewModel;
});
//# sourceMappingURL=ButtonViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/button/ButtonViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./ButtonViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ButtonViewModel_1 = require("./ButtonViewModel");
    class ButtonView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ButtonViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Button Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Label, { text: "Click the button", fontSize: 30, fontAttributes: "Bold", horizontalOptions: "Center" }),
                    XNode_1.default.create(XF_1.default.Button, { margin: 50, padding: 10, text: "Button Demo", command: Bind_1.default.event((s, e) => this.viewModel.clickEvent("Button")), borderRadius: "5", backgroundColor: "#ff5733", borderColor: "#ff5733", textColor: "white" }))));
        }
    }
    exports.default = ButtonView;
});
//# sourceMappingURL=ButtonView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/button/ButtonView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "../ButtonViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ButtonViewModel_1 = require("../ButtonViewModel");
    class ImageButtonView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ButtonViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "ImageButton Demo" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Label, { text: "Image Button", fontSize: 30, fontAttributes: "Bold", horizontalOptions: "Center" }),
                    XNode_1.default.create(XF_1.default.ImageButton, { source: "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Xamarin-logo.svg/1920px-Xamarin-logo.svg.png", command: Bind_1.default.event((s, e) => this.viewModel.clickEvent("Image-button")) }))));
        }
    }
    exports.default = ImageButtonView;
});
//# sourceMappingURL=ImageButtonView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/button/image-button/ImageButtonView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class CheckBoxSampleViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.isAgree = false;
        }
    }
    exports.default = CheckBoxSampleViewModel;
});
//# sourceMappingURL=CheckBoxSampleViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/check-box/CheckBoxSampleViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./CheckBoxSampleViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const CheckBoxSampleViewModel_1 = require("./CheckBoxSampleViewModel");
    class CheckBoxView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(CheckBoxSampleViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "CheckBox Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { padding: "10" },
                    XNode_1.default.create(XF_1.default.Label, { text: "" }),
                    XNode_1.default.create(XF_1.default.Label, { fontSize: "16" },
                        XNode_1.default.create(XF_1.default.Label.formattedText, null,
                            XNode_1.default.create(XF_1.default.FormattedString, null,
                                XNode_1.default.create(XF_1.default.Span, { textColor: "black", text: "Check to indicate that you have read and agree to the terms of " }),
                                XNode_1.default.create(XF_1.default.Span, { text: "XF Software Agreement", textColor: "blue" })))),
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Horizontal" },
                        XNode_1.default.create(XF_1.default.CheckBox, { isChecked: Bind_1.default.twoWays(() => this.viewModel.isAgree) }),
                        XNode_1.default.create(XF_1.default.Label, { text: "Agree the terms and condition", verticalTextAlignment: "Center" })),
                    XNode_1.default.create(XF_1.default.Label, { textColor: "Maroon", text: Bind_1.default.oneWay(() => "You have " + (this.viewModel.isAgree ? "agreed terms and condition" : "disagreed terms and condition")) }))));
        }
    }
    exports.default = CheckBoxView;
});
//# sourceMappingURL=CheckBoxView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/check-box/CheckBoxView");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class ComboBoxSampleViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.search = "";
            this.countryList = [
                {
                    label: "Select Country",
                    value: "SELECT"
                },
                {
                    label: "Afghanistan",
                    value: "AF"
                },
                {
                    label: "Albania",
                    value: "AL"
                },
                {
                    label: "Algeria",
                    value: "AG"
                },
                {
                    label: "Andorra",
                    value: "AN"
                },
                {
                    label: "Angola",
                    value: "AO"
                },
                {
                    label: "Anguilla",
                    value: "AV"
                },
                {
                    label: "Antarctica",
                    value: "AY"
                },
                {
                    label: "Antigua and Barbuda",
                    value: "AC"
                },
                {
                    label: "Argentina",
                    value: "AR"
                },
                {
                    label: "Armenia",
                    value: "AM"
                },
                {
                    label: "Aruba",
                    value: "AA"
                },
                {
                    label: "Ashmore and Cartier Islands",
                    value: "AT"
                },
                {
                    label: "Australia",
                    value: "AS"
                },
                {
                    label: "Austria",
                    value: "AU"
                },
                {
                    label: "Azerbaijan",
                    value: "AJ"
                },
                {
                    label: "Bahrain",
                    value: "BA"
                },
                {
                    label: "Bangladesh",
                    value: "BG"
                },
                {
                    label: "Barbados",
                    value: "BB"
                },
                {
                    label: "Bassas da India",
                    value: "BS"
                },
                {
                    label: "Belarus",
                    value: "BO"
                },
                {
                    label: "Belgium",
                    value: "BE"
                },
                {
                    label: "Belize",
                    value: "BH"
                },
                {
                    label: "Benin",
                    value: "BN"
                },
                {
                    label: "Bermuda",
                    value: "BD"
                },
                {
                    label: "Bhutan",
                    value: "BT"
                },
                {
                    label: "Bodies of water",
                    value: "OS"
                },
                {
                    label: "Bolivia",
                    value: "BL"
                },
                {
                    label: "Bosnia and Herzegovina",
                    value: "BK"
                },
                {
                    label: "Botswana",
                    value: "BC"
                },
                {
                    label: "Bouvet Island",
                    value: "BV"
                },
                {
                    label: "Brazil",
                    value: "BR"
                },
                {
                    label: "British Indian Ocean Territory",
                    value: "IO"
                },
                {
                    label: "British Virgin Islands",
                    value: "VI"
                },
                {
                    label: "Brunei",
                    value: "BX"
                },
                {
                    label: "Bulgaria",
                    value: "BU"
                },
                {
                    label: "Burkina Faso",
                    value: "UV"
                },
                {
                    label: "Burundi",
                    value: "BY"
                },
                {
                    label: "Cambodia",
                    value: "CB"
                },
                {
                    label: "Cameroon",
                    value: "CM"
                },
                {
                    label: "Canada",
                    value: "CA"
                },
                {
                    label: "Cape Verde",
                    value: "CV"
                },
                {
                    label: "Cayman Islands",
                    value: "CJ"
                },
                {
                    label: "Central African Republic",
                    value: "CT"
                },
                {
                    label: "Chad",
                    value: "CD"
                },
                {
                    label: "Chile",
                    value: "CI"
                },
                {
                    label: "China",
                    value: "CH"
                },
                {
                    label: "Christmas Island",
                    value: "KT"
                },
                {
                    label: "Clipperton Island",
                    value: "IP"
                },
                {
                    label: "Cocos (Keeling) Islands",
                    value: "CK"
                },
                {
                    label: "Colombia",
                    value: "CO"
                },
                {
                    label: "Comoros",
                    value: "CN"
                },
                {
                    label: "Congo (Brazzaville)",
                    value: "CF"
                },
                {
                    label: "Congo (Kinshasa)",
                    value: "CG"
                },
                {
                    label: "Cook Islands",
                    value: "CW"
                },
                {
                    label: "Coral Sea Islands",
                    value: "CR"
                },
                {
                    label: "Costa Rica",
                    value: "CS"
                },
                {
                    label: "Cote D'Ivoire",
                    value: "IV"
                },
                {
                    label: "Croatia",
                    value: "HR"
                },
                {
                    label: "Cuba",
                    value: "CU"
                },
                {
                    label: "Cyprus",
                    value: "CY"
                },
                {
                    label: "Czech Republic",
                    value: "EZ"
                },
                {
                    label: "Denmark",
                    value: "DA"
                },
                {
                    label: "Djibouti",
                    value: "DJ"
                },
                {
                    label: "Dominica",
                    value: "DO"
                },
                {
                    label: "Dominican Republic",
                    value: "DR"
                },
                {
                    label: "East Timor",
                    value: "TT"
                },
                {
                    label: "Ecuador",
                    value: "EC"
                },
                {
                    label: "Egypt",
                    value: "EG"
                },
                {
                    label: "El Salvador",
                    value: "ES"
                },
                {
                    label: "Equatorial Guinea",
                    value: "EK"
                },
                {
                    label: "Eritrea",
                    value: "ER"
                },
                {
                    label: "Estonia",
                    value: "EN"
                },
                {
                    label: "Ethiopia",
                    value: "ET"
                },
                {
                    label: "Europa Island",
                    value: "EU"
                },
                {
                    label: "Falkland Islands (Islas Malvinas)",
                    value: "FK"
                },
                {
                    label: "Faroe Islands",
                    value: "FO"
                },
                {
                    label: "Fiji",
                    value: "FJ"
                },
                {
                    label: "Finland",
                    value: "FI"
                },
                {
                    label: "France",
                    value: "FR"
                },
                {
                    label: "French Guiana",
                    value: "FG"
                },
                {
                    label: "French Polynesia",
                    value: "FP"
                },
                {
                    label: "French Southern and Antarctic Lands",
                    value: "FS"
                },
                {
                    label: "Gabon",
                    value: "GB"
                },
                {
                    label: "Gaza Strip",
                    value: "GZ"
                },
                {
                    label: "Georgia",
                    value: "GG"
                },
                {
                    label: "Germany",
                    value: "GM"
                },
                {
                    label: "Ghana",
                    value: "GH"
                },
                {
                    label: "Gibraltar",
                    value: "GI"
                },
                {
                    label: "Glorioso Islands",
                    value: "GO"
                },
                {
                    label: "Greece",
                    value: "GR"
                },
                {
                    label: "Greenland",
                    value: "GL"
                },
                {
                    label: "Grenada",
                    value: "GJ"
                },
                {
                    label: "Guadeloupe",
                    value: "GP"
                },
                {
                    label: "Guatemala",
                    value: "GT"
                },
                {
                    label: "Guernsey",
                    value: "GK"
                },
                {
                    label: "Guinea",
                    value: "GV"
                },
                {
                    label: "Guinea-Bissau",
                    value: "PU"
                },
                {
                    label: "Guyana",
                    value: "GY"
                },
                {
                    label: "Haiti",
                    value: "HA"
                },
                {
                    label: "Heard Island and McDonald Islands",
                    value: "HM"
                },
                {
                    label: "Holy See (Vatican City)",
                    value: "VT"
                },
                {
                    label: "Honduras",
                    value: "HO"
                },
                {
                    label: "Hong Kong",
                    value: "HK"
                },
                {
                    label: "Hungary",
                    value: "HU"
                },
                {
                    label: "Iceland",
                    value: "IC"
                },
                {
                    label: "India",
                    value: "IN"
                },
                {
                    label: "Indonesia",
                    value: "ID"
                },
                {
                    label: "Iran",
                    value: "IR"
                },
                {
                    label: "Iraq",
                    value: "IZ"
                },
                {
                    label: "Ireland",
                    value: "EI"
                },
                {
                    label: "Isle of Man",
                    value: "IM"
                },
                {
                    label: "Israel",
                    value: "IS"
                },
                {
                    label: "Italy",
                    value: "IT"
                },
                {
                    label: "Jamaica",
                    value: "JM"
                },
                {
                    label: "Jan Mayen",
                    value: "JN"
                },
                {
                    label: "Japan",
                    value: "JA"
                },
                {
                    label: "Jersey",
                    value: "JE"
                },
                {
                    label: "Jordan",
                    value: "JO"
                },
                {
                    label: "Juan de Nova Island",
                    value: "JU"
                },
                {
                    label: "Kazakhstan",
                    value: "KZ"
                },
                {
                    label: "Kenya",
                    value: "KE"
                },
                {
                    label: "Kiribati",
                    value: "KR"
                },
                {
                    label: "Kuwait",
                    value: "KU"
                },
                {
                    label: "Kyrgyzstan",
                    value: "KG"
                },
                {
                    label: "Laos",
                    value: "LA"
                },
                {
                    label: "Latvia",
                    value: "LG"
                },
                {
                    label: "Lebanon",
                    value: "LE"
                },
                {
                    label: "Lesotho",
                    value: "LT"
                },
                {
                    label: "Liberia",
                    value: "LI"
                },
                {
                    label: "Libya",
                    value: "LY"
                },
                {
                    label: "Liechtenstein",
                    value: "LS"
                },
                {
                    label: "Lithuania",
                    value: "LH"
                },
                {
                    label: "Luxembourg",
                    value: "LU"
                },
                {
                    label: "Macau",
                    value: "MC"
                },
                {
                    label: "Macedonia",
                    value: "MK"
                },
                {
                    label: "Madagascar",
                    value: "MA"
                },
                {
                    label: "Malawi",
                    value: "MI"
                },
                {
                    label: "Malaysia",
                    value: "MY"
                },
                {
                    label: "Maldives",
                    value: "MV"
                },
                {
                    label: "Mali",
                    value: "ML"
                },
                {
                    label: "Malta",
                    value: "MT"
                },
                {
                    label: "Martinique",
                    value: "MB"
                },
                {
                    label: "Mauritania",
                    value: "MR"
                },
                {
                    label: "Mauritius",
                    value: "MP"
                },
                {
                    label: "Mayotte",
                    value: "MF"
                },
                {
                    label: "Mexico",
                    value: "MX"
                },
                {
                    label: "Moldova",
                    value: "MD"
                },
                {
                    label: "Monaco",
                    value: "MN"
                },
                {
                    label: "Mongolia",
                    value: "MG"
                },
                {
                    label: "Montserrat",
                    value: "MH"
                },
                {
                    label: "Morocco",
                    value: "MO"
                },
                {
                    label: "Mozambique",
                    value: "MZ"
                },
                {
                    label: "Myanmar (Burma)",
                    value: "BM"
                },
                {
                    label: "Namibia",
                    value: "WA"
                },
                {
                    label: "Nauru",
                    value: "NR"
                },
                {
                    label: "Nepal",
                    value: "NP"
                },
                {
                    label: "Netherlands",
                    value: "NL"
                },
                {
                    label: "Netherlands Antilles",
                    value: "NT"
                },
                {
                    label: "New Caledonia",
                    value: "NC"
                },
                {
                    label: "New Zealand",
                    value: "NZ"
                },
                {
                    label: "Nicaragua",
                    value: "NU"
                },
                {
                    label: "Niger",
                    value: "NG"
                },
                {
                    label: "Nigeria",
                    value: "NI"
                },
                {
                    label: "Niue",
                    value: "NE"
                },
                {
                    label: "Norfolk Island",
                    value: "NF"
                },
                {
                    label: "North Korea",
                    value: "KN"
                },
                {
                    label: "Norway",
                    value: "NO"
                },
                {
                    label: "Oman",
                    value: "MU"
                },
                {
                    label: "Pakistan",
                    value: "PK"
                },
                {
                    label: "Panama",
                    value: "PM"
                },
                {
                    label: "Papua New Guinea",
                    value: "PP"
                },
                {
                    label: "Paracel Islands",
                    value: "PF"
                },
                {
                    label: "Paraguay",
                    value: "PA"
                },
                {
                    label: "Peru",
                    value: "PE"
                },
                {
                    label: "Philippines",
                    value: "RP"
                },
                {
                    label: "Pitcairn Islands",
                    value: "PC"
                },
                {
                    label: "Poland",
                    value: "PL"
                },
                {
                    label: "Portugal",
                    value: "PO"
                },
                {
                    label: "Qatar",
                    value: "QA"
                },
                {
                    label: "Reunion",
                    value: "RE"
                },
                {
                    label: "Romania",
                    value: "RO"
                },
                {
                    label: "Russia",
                    value: "RS"
                },
                {
                    label: "Rwanda",
                    value: "RW"
                },
                {
                    label: "Saint Helena",
                    value: "SH"
                },
                {
                    label: "Saint Kitts and Nevis",
                    value: "SC"
                },
                {
                    label: "Saint Lucia",
                    value: "ST"
                },
                {
                    label: "Saint Pierre and Miquelon",
                    value: "SB"
                },
                {
                    label: "Saint Vincent and the Grenadines",
                    value: "VC"
                },
                {
                    label: "Samoa",
                    value: "WS"
                },
                {
                    label: "San Marino",
                    value: "SM"
                },
                {
                    label: "Sao Tome and Principe",
                    value: "TP"
                },
                {
                    label: "Saudi Arabia",
                    value: "SA"
                },
                {
                    label: "Senegal",
                    value: "SG"
                },
                {
                    label: "Serbia and Montenegro",
                    value: "YI"
                },
                {
                    label: "Seychelles",
                    value: "SE"
                },
                {
                    label: "Sierra Leone",
                    value: "SL"
                },
                {
                    label: "Singapore",
                    value: "SN"
                },
                {
                    label: "Slovakia",
                    value: "LO"
                },
                {
                    label: "Slovenia",
                    value: "SI"
                },
                {
                    label: "Solomon Islands",
                    value: "BP"
                },
                {
                    label: "Somalia",
                    value: "SO"
                },
                {
                    label: "South Africa",
                    value: "SF"
                },
                {
                    label: "South Georgia and the South Sandwich Islands",
                    value: "SX"
                },
                {
                    label: "South Korea",
                    value: "KS"
                },
                {
                    label: "Spain",
                    value: "SP"
                },
                {
                    label: "Spratly Islands",
                    value: "PG"
                },
                {
                    label: "Sri Lanka",
                    value: "CE"
                },
                {
                    label: "Sudan",
                    value: "SU"
                },
                {
                    label: "Suriname",
                    value: "NS"
                },
                {
                    label: "Svalbard",
                    value: "SV"
                },
                {
                    label: "Swaziland",
                    value: "WZ"
                },
                {
                    label: "Sweden",
                    value: "SW"
                },
                {
                    label: "Switzerland",
                    value: "SZ"
                },
                {
                    label: "Syria",
                    value: "SY"
                },
                {
                    label: "Taiwan",
                    value: "TW"
                },
                {
                    label: "Tajikistan",
                    value: "TI"
                },
                {
                    label: "Tanzania",
                    value: "TZ"
                },
                {
                    label: "Thailand",
                    value: "TH"
                },
                {
                    label: "The Bahamas",
                    value: "BF"
                },
                {
                    label: "The Gambia",
                    value: "GA"
                },
                {
                    label: "Togo",
                    value: "TO"
                },
                {
                    label: "Tokelau",
                    value: "TL"
                },
                {
                    label: "Tonga",
                    value: "TN"
                },
                {
                    label: "Trinidad and Tobago",
                    value: "TD"
                },
                {
                    label: "Tromelin Island",
                    value: "TE"
                },
                {
                    label: "Tunisia",
                    value: "TS"
                },
                {
                    label: "Turkey",
                    value: "TU"
                },
                {
                    label: "Turkmenistan",
                    value: "TX"
                },
                {
                    label: "Turks and Caicos Islands",
                    value: "TK"
                },
                {
                    label: "Tuvalu",
                    value: "TV"
                },
                {
                    label: "Uganda",
                    value: "UG"
                },
                {
                    label: "Ukraine",
                    value: "UP"
                },
                {
                    label: "Undersea features",
                    value: "UF"
                },
                {
                    label: "United Arab Emirates",
                    value: "AE"
                },
                {
                    label: "United Kingdom",
                    value: "UK"
                },
                {
                    label: "United States",
                    value: "US"
                },
                {
                    label: "Uruguay",
                    value: "UY"
                },
                {
                    label: "Uzbekistan",
                    value: "UZ"
                },
                {
                    label: "Vanuatu",
                    value: "NH"
                },
                {
                    label: "Venezuela",
                    value: "VE"
                },
                {
                    label: "Vietnam",
                    value: "VM"
                },
                {
                    label: "Wallis and Futuna",
                    value: "WF"
                },
                {
                    label: "West Bank",
                    value: "WE"
                },
                {
                    label: "Western Sahara",
                    value: "WI"
                },
                {
                    label: "Yemen",
                    value: "YM"
                },
                {
                    label: "Zambia",
                    value: "ZA"
                },
                {
                    label: "Zimbabwe",
                    value: "ZI"
                }
            ];
            this.country = "IN";
        }
        get countries() {
            let s = this.search;
            if (!s) {
                return this.countryList;
            }
            s = s.toLowerCase();
            return this.countryList.filter((x) => x.label.toLowerCase().indexOf(s) !== -1);
        }
    }
    __decorate([
        AtomViewModel_1.Watch,
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [])
    ], ComboBoxSampleViewModel.prototype, "countries", null);
    exports.default = ComboBoxSampleViewModel;
});
//# sourceMappingURL=ComboBoxSampleViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/combo-box/ComboBoxSampleViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/combo-box/AtomXFComboBox", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./ComboBoxSampleViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFComboBox_1 = require("@web-atoms/xf-controls/dist/combo-box/AtomXFComboBox");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ComboBoxSampleViewModel_1 = require("./ComboBoxSampleViewModel");
    class ComboBoxSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ComboBoxSampleViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "ComboBox Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical" },
                    XNode_1.default.create(AtomXFComboBox_1.default, { showSearch: true, searchText: Bind_1.default.twoWays(() => this.viewModel.search), items: Bind_1.default.oneWay(() => this.viewModel.countries), value: Bind_1.default.twoWays(() => this.viewModel.country) }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => this.viewModel.country) }))));
        }
    }
    exports.default = ComboBoxSample;
});
//# sourceMappingURL=ComboBoxSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/combo-box/ComboBoxSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class DatePickerViewModel extends AtomViewModel_1.AtomViewModel {
    }
    exports.default = DatePickerViewModel;
});
//# sourceMappingURL=DatePickerViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/date-picker/DatePickerViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./DatePickerViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const DatePickerViewModel_1 = require("./DatePickerViewModel");
    class DatePickerView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(DatePickerViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Date Picker Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.DatePicker, { minimumDate: "01/01/2018", maximumDate: "12/31/2018", horizontalOptions: "Center", verticalOptions: "Center", date: Bind_1.default.twoWays(() => this.viewModel.date) }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => this.viewModel.date) }))));
        }
    }
    exports.default = DatePickerView;
});
//# sourceMappingURL=DatePickerView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/date-picker/DatePickerView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class EditorViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.value = "";
        }
    }
    exports.default = EditorViewModel;
});
//# sourceMappingURL=EditorViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/editor/EditorViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./EditorViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const EditorViewModel_1 = require("./EditorViewModel");
    class EditorView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(EditorViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Editor View Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { padding: "10" },
                    XNode_1.default.create(XF_1.default.Label, { text: "Address:", textColor: "Black" }),
                    XNode_1.default.create(XF_1.default.Editor, { text: Bind_1.default.twoWays(() => this.viewModel.value), placeholder: "Multi-line text editor", AutoSize: "TextChanges", MaxLength: "200", IsSpellCheckEnabled: "false", IsTextPredictionEnabled: "false" }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => `Address is ${this.viewModel.value}`), textColor: "Maroon" }))));
        }
    }
    exports.default = EditorView;
});
//# sourceMappingURL=EditorView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/editor/EditorView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class EntryViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.model = {
                username: "",
                password: ""
            };
        }
    }
    exports.default = EntryViewModel;
});
//# sourceMappingURL=EntryViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/entry/EntryViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./EntryViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const EntryViewModel_1 = require("./EntryViewModel");
    class EntryView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(EntryViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Entry View Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { padding: "10" },
                    XNode_1.default.create(XF_1.default.Label, { text: "First Name:", textColor: "Black" }),
                    XNode_1.default.create(XF_1.default.Entry, { text: Bind_1.default.twoWays(() => this.viewModel.model.username), placeholder: "Enter First Name" }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => `First name is ${this.viewModel.model.username}`), textColor: "Maroon" }))));
        }
    }
    exports.default = EntryView;
});
//# sourceMappingURL=EntryView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/entry/EntryView");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class LabelViewModel extends AtomViewModel_1.AtomViewModel {
        labelClick() {
            this.navigationService.alert("Label clicked");
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], LabelViewModel.prototype, "navigationService", void 0);
    exports.default = LabelViewModel;
});
//# sourceMappingURL=LabelViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/label/LabelViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./LabelViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const LabelViewModel_1 = require("./LabelViewModel");
    class LabelView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(LabelViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Label Sample" },
                XNode_1.default.create(XF_1.default.ScrollView, null,
                    XNode_1.default.create(XF_1.default.StackLayout, { margin: "10" },
                        XNode_1.default.create(XF_1.default.Label, { text: "Introduction", fontSize: "25", textColor: "#111", horizontalOptions: "Center" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "What is web atoms?", fontSize: "20", textColor: "#111", horizontalOptions: "Start" }),
                        XNode_1.default.create(XF_1.default.Label, { lineBreakMode: "WordWrap", fontSize: "14" },
                            XNode_1.default.create(XF_1.default.Label.formattedText, null,
                                XNode_1.default.create(XF_1.default.FormattedString, null,
                                    XNode_1.default.create(XF_1.default.Span, { text: "'Web Atoms' is an advanced MVVM framework to write cross platform applications in " }),
                                    XNode_1.default.create(XF_1.default.Span, { text: "HTML5 ", textColor: "red" }),
                                    XNode_1.default.create(XF_1.default.Span, { text: "and " }),
                                    XNode_1.default.create(XF_1.default.Span, { text: "Xamarin.Forms.", textColor: "red" }),
                                    XNode_1.default.create(XF_1.default.Span, { text: " Unlike other frameworks, Web Atoms lets you divide User Interface logic in strict MVVM fashion and separates View in HTML5 and Xaml. Benefit of separating User interface logic in ViewModel is you can individually unit test view model to make sure your logic is consistent across platforms. " }),
                                    XNode_1.default.create(XF_1.default.Span, { text: "Click me", textDecorations: "Underline", textColor: "Blue" },
                                        XNode_1.default.create(XF_1.default.Span.gestureRecognizers, null,
                                            XNode_1.default.create(XF_1.default.TapGestureRecognizer, { command: Bind_1.default.event((s, e) => this.viewModel.labelClick()) })))))),
                        XNode_1.default.create(XF_1.default.Label, { text: "Also everything is transpiled into JavaScript, your View Model and Services remain in JavaScript and in browser it works flawlessly." }),
                        XNode_1.default.create(XF_1.default.Label, { text: "Benefits of Web Atoms with Xamarin.Forms", fontSize: "20", textColor: "#111", horizontalOptions: "Start" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "1. Write TSX instead of XAML" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "2. Small application download size" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "3. Dynamic Module Loading from web" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "4. Reuse existing NuGet components by exposing via services" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "5. Host javascript on server with instant updates to apps" })))));
        }
    }
    exports.default = LabelView;
});
//# sourceMappingURL=LabelView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/label/LabelView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AjaxOptions = void 0;
    class AjaxOptions {
    }
    exports.AjaxOptions = AjaxOptions;
});
//# sourceMappingURL=AjaxOptions.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/http/AjaxOptions");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../App", "../di/DISingleton", "../di/Inject"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const App_1 = require("../App");
    const DISingleton_1 = require("../di/DISingleton");
    const Inject_1 = require("../di/Inject");
    let CacheService = class CacheService {
        constructor(app) {
            this.app = app;
            this.cache = {};
        }
        remove(key) {
            const v = this.cache[key];
            if (v) {
                this.clear(v);
                return v.value;
            }
            return null;
        }
        getOrCreate(key, task) {
            return __awaiter(this, void 0, void 0, function* () {
                const c = this.cache[key] || (this.cache[key] = {
                    key,
                    finalTTL: 3600
                });
                if (!c.value) {
                    c.value = task(c);
                }
                let v = null;
                try {
                    v = yield c.value;
                }
                catch (e) {
                    this.clear(c);
                    throw e;
                }
                if (c.ttlSeconds !== undefined) {
                    if (typeof c.ttlSeconds === "number") {
                        c.finalTTL = c.ttlSeconds;
                    }
                    else {
                        c.finalTTL = c.ttlSeconds(v);
                    }
                }
                if (c.timeout) {
                    clearTimeout(c.timeout);
                }
                if (c.finalTTL) {
                    this.cache[key] = c;
                    c.timeout = setTimeout(() => {
                        c.timeout = 0;
                        this.clear(c);
                    }, c.finalTTL * 1000);
                }
                else {
                    // this is the case where we do not want to store
                    this.clear(c);
                }
                return yield c.value;
            });
        }
        clear(ci) {
            if (ci.timeout) {
                clearTimeout(ci.timeout);
                ci.timeout = 0;
            }
            this.cache[ci.key] = null;
            delete this.cache[ci.key];
        }
    };
    CacheService = __decorate([
        DISingleton_1.default(),
        __param(0, Inject_1.Inject),
        __metadata("design:paramtypes", [App_1.App])
    ], CacheService);
    exports.default = CacheService;
});
//# sourceMappingURL=CacheService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/CacheService");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    class JsonError extends Error {
        constructor(message, json) {
            super(message);
            this.json = json;
        }
    }
    exports.default = JsonError;
});
//# sourceMappingURL=JsonError.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/http/JsonError");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AjaxOptions", "../../App", "../../Atom", "../../core/AtomBridge", "../../core/types", "../../di/Inject", "../../di/TypeKey", "../CacheService", "../JsonService", "./JsonError"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.BaseService = exports.ServiceParameter = exports.Cancel = exports.Patch = exports.Put = exports.Delete = exports.Get = exports.Post = exports.XmlBody = exports.BodyFormModel = exports.RawBody = exports.Body = exports.Queries = exports.Query = exports.Header = exports.Path = void 0;
    const AjaxOptions_1 = require("./AjaxOptions");
    const App_1 = require("../../App");
    const Atom_1 = require("../../Atom");
    const AtomBridge_1 = require("../../core/AtomBridge");
    const types_1 = require("../../core/types");
    const Inject_1 = require("../../di/Inject");
    const TypeKey_1 = require("../../di/TypeKey");
    const CacheService_1 = require("../CacheService");
    const JsonService_1 = require("../JsonService");
    const JsonError_1 = require("./JsonError");
    // tslint:disable-next-line
    function methodBuilder(method) {
        // tslint:disable-next-line
        return function (url, options) {
            // tslint:disable-next-line
            return function (target, propertyKey, descriptor) {
                target.methods = target.methods || {};
                const a = target.methods[propertyKey];
                const oldFunction = descriptor.value;
                // tslint:disable-next-line:typedef
                descriptor.value = function (...args) {
                    if (this.testMode || Atom_1.Atom.designMode) {
                        // tslint:disable-next-line:no-console
                        console.log(`Test Design Mode: ${url} .. ${args.join(",")}`);
                        const ro = oldFunction.apply(this, args);
                        if (ro) {
                            return ro;
                        }
                    }
                    const jsCache = options ? options.jsCacheSeconds : 0;
                    if (jsCache) {
                        const cacheService = this.app.resolve(CacheService_1.default);
                        const jArgs = args.map((arg) => arg instanceof types_1.CancelToken ? null : arg);
                        const key = `${this.constructor.name}:${method}:${url}:${JSON.stringify(jArgs)}`;
                        return cacheService.getOrCreate(key, (e) => {
                            e.ttlSeconds = jsCache;
                            return this.invoke(url, method, a, args, options);
                        });
                    }
                    return this.invoke(url, method, a, args, options);
                };
            };
        };
    }
    // tslint:disable-next-line
    function parameterBuilder(paramName, defaultValue) {
        // tslint:disable-next-line
        return function (key) {
            // console.log("Declaration");
            // console.log({ key:key});
            // tslint:disable-next-line
            return function (target, propertyKey, parameterIndex) {
                // console.log("Instance");
                // console.log({ key:key, propertyKey: propertyKey,parameterIndex: parameterIndex });
                target.methods = target.methods || {};
                let a = target.methods[propertyKey];
                if (!a) {
                    a = [];
                    target.methods[propertyKey] = a;
                }
                a[parameterIndex] = new ServiceParameter(paramName, key, defaultValue);
            };
        };
    }
    /**
     * This will register Url path fragment on parameter.
     *
     * @example
     *
     *      @Get("/api/products/{category}")
     *      async getProducts(
     *          @Path("category")  category: number
     *      ): Promise<Product[]> {
     *      }
     *
     * @export
     * @function Path
     * @param {name} - Name of the parameter
     */
    exports.Path = parameterBuilder("Path");
    /**
     * This will register header on parameter.
     *
     * @example
     *
     *      @Get("/api/products/{category}")
     *      async getProducts(
     *          @Header("x-http-auth")  category: number
     *      ): Promise<Product[]> {
     *      }
     *
     * @export
     * @function Path
     * @param {name} - Name of the parameter
     */
    exports.Header = parameterBuilder("Header");
    /**
     * This will register Url query fragment on parameter.
     *
     * @example
     *
     *      @Get("/api/products")
     *      async getProducts(
     *          @Query("category")  category: number
     *      ): Promise<Product[]> {
     *      }
     *
     * @export
     * @function Query
     * @param {name} - Name of the parameter
     */
    exports.Query = parameterBuilder("Query");
    /**
     * This will register Url query fragments on parameter of type object
     *
     * @example
     *
     *     @Get("/api/products")
     *     async getProducts(
     *          @Queries queries: { [key: string]: string | number | boolean | null }
     *     ): Promise<Product[]> {
     *         return null;
     * }
     */
    exports.Queries = parameterBuilder("Queries")("");
    /**
     * This will register data fragment on ajax.
     *
     * @example
     *
     *      @Post("/api/products")
     *      async getProducts(
     *          @Query("id")  id: number,
     *          @Body product: Product
     *      ): Promise<Product[]> {
     *      }
     *
     * @export
     * @function Body
     */
    exports.Body = parameterBuilder("Body")("");
    exports.RawBody = parameterBuilder("RawBody")("");
    /**
     * This will register data fragment on ajax in old formModel way.
     *
     * @example
     *
     *      @Post("/api/products")
     *      async getProducts(
     *          @Query("id")  id: number,
     *          @BodyFormModel product: Product
     *      ): Promise<Product[]> {
     *      }
     *
     * @export
     * @function BodyFormModel
     */
    exports.BodyFormModel = parameterBuilder("BodyFormModel")("");
    exports.XmlBody = parameterBuilder("XmlBody")("");
    /**
     * Http Post method
     * @example
     *
     *      @Post("/api/products")
     *      async saveProduct(
     *          @Body product: Product
     *      ): Promise<Product> {
     *      }
     *
     * @export
     * @function Post
     * @param {url} - Url for the operation
     */
    exports.Post = methodBuilder("Post");
    /**
     * Http Get Method
     *
     * @example
     *
     *      @Get("/api/products/{category}")
     *      async getProducts(
     *          @Path("category") category?:string
     *      ): Promise<Product[]> {
     *      }
     *
     * @export
     * @function Body
     */
    exports.Get = methodBuilder("Get");
    /**
     * Http Delete method
     * @example
     *
     *      @Delete("/api/products")
     *      async deleteProduct(
     *          @Body product: Product
     *      ): Promise<Product> {
     *      }
     *
     * @export
     * @function Delete
     * @param {url} - Url for the operation
     */
    exports.Delete = methodBuilder("Delete");
    /**
     * Http Put method
     * @example
     *
     *      @Put("/api/products")
     *      async saveProduct(
     *          @Body product: Product
     *      ): Promise<Product> {
     *      }
     *
     * @export
     * @function Put
     * @param {url} - Url for the operation
     */
    exports.Put = methodBuilder("Put");
    /**
     * Http Patch method
     * @example
     *
     *      @Patch("/api/products")
     *      async saveProduct(
     *          @Body product: any
     *      ): Promise<Product> {
     *      }
     *
     * @export
     * @function Patch
     * @param {url} - Url for the operation
     */
    exports.Patch = methodBuilder("Patch");
    /**
     * Cancellation token
     * @example
     *
     *      @Put("/api/products")
     *      async saveProduct(
     *          @Body product: Product
     *          @Cancel cancel: CancelToken
     *      ): Promise<Product> {
     *      }
     *
     * @export
     * @function Put
     * @param {url} - Url for the operation
     */
    function Cancel(target, propertyKey, parameterIndex) {
        if (!target.methods) {
            target.methods = {};
        }
        let a = target.methods[propertyKey];
        if (!a) {
            a = [];
            target.methods[propertyKey] = a;
        }
        a[parameterIndex] = new ServiceParameter("cancel", "");
    }
    exports.Cancel = Cancel;
    class ServiceParameter {
        constructor(type, key, defaultValue) {
            this.type = type;
            this.key = key;
            this.defaultValue = defaultValue;
            this.type = type.toLowerCase();
            this.key = key;
        }
    }
    exports.ServiceParameter = ServiceParameter;
    function BaseUrl(baseUrl) {
        return (target) => {
            const key = TypeKey_1.TypeKey.get(target);
            BaseService.baseUrls[key] = baseUrl;
        };
    }
    exports.default = BaseUrl;
    const globalNS = (typeof global !== "undefined") ? global : window;
    if (!globalNS.XMLHttpRequest) {
        // tslint:disable-next-line: no-var-requires
        globalNS.XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
    }
    /**
     *
     *
     * @export
     * @class BaseService
     */
    let BaseService = class BaseService {
        constructor(app, jsonService) {
            this.app = app;
            this.jsonService = jsonService;
            this.testMode = false;
            this.showProgress = true;
            this.showError = false;
            // bs
            this.methods = {};
            this.methodReturns = {};
            this.jsonOptions = null;
            this.jsonOptions = Object.assign({}, this.jsonService.options);
        }
        encodeData(o) {
            o.dataType = "application/json";
            o.data = this.jsonService.stringify(o.data, this.jsonOptions);
            o.contentType = "application/json";
            return o;
        }
        sendResult(result, error) {
            return new Promise((resolve, reject) => {
                if (error) {
                    setTimeout(() => {
                        reject(error);
                    }, 1);
                    return;
                }
                setTimeout(() => {
                    resolve(result);
                }, 1);
            });
        }
        invoke(url, method, bag, values, methodOptions) {
            return __awaiter(this, void 0, void 0, function* () {
                if (this.baseUrl === undefined) {
                    let p = Object.getPrototypeOf(this);
                    while (p) {
                        const t = TypeKey_1.TypeKey.get(p.constructor || p);
                        const bu = BaseService.baseUrls[t];
                        if (bu) {
                            this.baseUrl = bu;
                            break;
                        }
                        p = Object.getPrototypeOf(p);
                    }
                    if (this.baseUrl === undefined) {
                        this.baseUrl = null;
                    }
                }
                if (this.baseUrl) {
                    if (!/^\//.test(url)) {
                        url = `${this.baseUrl}${url}`;
                    }
                }
                const busyIndicator = this.showProgress ? (this.app.createBusyIndicator()) : null;
                try {
                    url = UMD.resolvePath(url);
                    let options = new AjaxOptions_1.AjaxOptions();
                    options.method = method;
                    if (methodOptions) {
                        options.headers = methodOptions.headers;
                        options.dataType = methodOptions.accept;
                    }
                    const headers = options.headers = options.headers || {};
                    // this is necessary to support IsAjaxRequest in ASP.NET MVC
                    if (!headers["X-Requested-With"]) {
                        headers["X-Requested-With"] = "XMLHttpRequest";
                    }
                    options.dataType = options.dataType || "application/json";
                    const jsonOptions = Object.assign(Object.assign({}, this.jsonOptions), (methodOptions ? methodOptions.jsonOptions : {}));
                    if (bag) {
                        for (let i = 0; i < bag.length; i++) {
                            const p = bag[i];
                            const vi = values[i];
                            const v = vi === undefined ? p.defaultValue : vi;
                            if (v instanceof types_1.CancelToken) {
                                options.cancel = v;
                                continue;
                            }
                            switch (p.type) {
                                case "path":
                                    if (v === undefined) {
                                        continue;
                                    }
                                    const vs = v + "";
                                    const replacer = `{${p.key}}`;
                                    url = url.split(replacer).join(vs);
                                    break;
                                case "query":
                                    if (v === undefined) {
                                        continue;
                                    }
                                    if (url.indexOf("?") === -1) {
                                        url += "?";
                                    }
                                    if (!/(\&|\?)$/.test(url)) {
                                        url += "&";
                                    }
                                    url += `${encodeURIComponent(p.key)}=${encodeURIComponent(v)}`;
                                    break;
                                case "queries":
                                    if (url.indexOf("?") === -1) {
                                        url += "?";
                                    }
                                    if (!/(\&|\?)$/.test(url)) {
                                        url += "&";
                                    }
                                    for (const key in v) {
                                        if (v.hasOwnProperty(key)) {
                                            const element = v[key];
                                            if (element !== undefined) {
                                                url += `${encodeURIComponent(key)}=${encodeURIComponent(element)}&`;
                                            }
                                        }
                                    }
                                    break;
                                case "body":
                                    options.data = v;
                                    options = this.encodeData(options);
                                    break;
                                case "bodyformmodel":
                                    options.data = v;
                                    break;
                                case "rawbody":
                                    options.data = v;
                                    break;
                                case "xmlbody":
                                    options.contentType = "text/xml";
                                    options.data = v;
                                    break;
                                case "cancel":
                                    options.cancel = v;
                                    break;
                                case "header":
                                    if (v === undefined) {
                                        continue;
                                    }
                                    headers[p.key] = v;
                                    break;
                            }
                        }
                    }
                    options.url = url;
                    const xhr = yield this.ajax(url, options);
                    if (/json/i.test(xhr.responseType)) {
                        const text = xhr.responseText;
                        const response = this.jsonService.parse(text, jsonOptions);
                        if (xhr.status >= 400) {
                            throw new JsonError_1.default(typeof response === "string"
                                ? response
                                : (response.exceptionMessage
                                    || response.message
                                    || text
                                    || "Json Server Error"), response);
                        }
                        if (methodOptions && methodOptions.returnHeaders) {
                            return {
                                headers: this.parseHeaders(xhr.responseHeaders),
                                value: response
                            };
                        }
                        return response;
                    }
                    if (xhr.status >= 400) {
                        throw new Error(xhr.responseText || "Server Error");
                    }
                    if (methodOptions && methodOptions.returnHeaders) {
                        return {
                            headers: this.parseHeaders(xhr.responseHeaders),
                            value: xhr.responseText
                        };
                    }
                    return xhr.responseText;
                }
                finally {
                    if (busyIndicator) {
                        busyIndicator.dispose();
                    }
                }
            });
        }
        parseHeaders(headers) {
            if (typeof headers === "object") {
                return headers;
            }
            return (headers || "")
                .split("\n")
                .reduce((pv, c) => {
                const cv = c.split(":");
                pv[cv[0]] = (cv[1] || "").trim();
                return pv;
            }, {});
        }
        ajax(url, options) {
            return __awaiter(this, void 0, void 0, function* () {
                // return new CancellablePromise();
                url = url || options.url;
                // no longer needed, watch must provide functionality of waiting and cancelling
                // await Atom.delay(1, options.cancel);
                if (options.cancel && options.cancel.cancelled) {
                    throw new Error("cancelled");
                }
                if (AtomBridge_1.AtomBridge.instance.ajax) {
                    return yield new Promise((resolve, reject) => {
                        AtomBridge_1.AtomBridge.instance.ajax(url, options, (r) => {
                            resolve(options);
                        }, (e) => {
                            reject(e);
                        }, null);
                    });
                }
                const xhr = new XMLHttpRequest();
                return yield new Promise((resolve, reject) => {
                    if (options.cancel && options.cancel.cancelled) {
                        reject(options.cancel.cancelled);
                        return;
                    }
                    if (options.cancel) {
                        options.cancel.registerForCancel((r) => {
                            xhr.abort();
                            reject(r);
                            return;
                        });
                    }
                    xhr.onreadystatechange = (e) => {
                        if (xhr.readyState === XMLHttpRequest.DONE) {
                            options.status = xhr.status;
                            options.responseText = xhr.responseText;
                            // options.responseHeaders = (xhr.getAllResponseHeaders())
                            //     .split("\n")
                            //     .map((s) => s.trim().split(":"))
                            //     .reduce((pv, cv) => pv[cv[0]] = cv[1], {});
                            options.responseHeaders = xhr.getAllResponseHeaders();
                            const ct = xhr.getResponseHeader("content-type");
                            options.responseType = ct || xhr.responseType;
                            resolve(options);
                        }
                    };
                    xhr.open(options.method, url, true);
                    if (options.dataType) {
                        xhr.setRequestHeader("accept", options.dataType);
                    }
                    if (options.contentType) {
                        xhr.setRequestHeader("content-type", options.contentType);
                    }
                    const h = options.headers;
                    if (h) {
                        for (const key in h) {
                            if (h.hasOwnProperty(key)) {
                                const element = h[key];
                                xhr.setRequestHeader(key, element.toString());
                            }
                        }
                    }
                    try {
                        xhr.send(options.data);
                    }
                    catch (e) {
                        options.status = xhr.status;
                        options.responseText = xhr.responseText;
                        // options.responseHeaders = (xhr.getAllResponseHeaders())
                        //     .split("\n")
                        //     .map((s) => s.trim().split(":"))
                        //     .reduce((pv, cv) => pv[cv[0]] = cv[1], {});
                        options.responseHeaders = xhr.getAllResponseHeaders();
                        const ct = xhr.getResponseHeader("content-type");
                        options.responseType = ct || xhr.responseType;
                        resolve(options);
                    }
                });
            });
        }
    };
    BaseService.baseUrls = {};
    BaseService = __decorate([
        __param(0, Inject_1.Inject),
        __param(1, Inject_1.Inject),
        __metadata("design:paramtypes", [App_1.App,
            JsonService_1.JsonService])
    ], BaseService);
    exports.BaseService = BaseService;
});
//# sourceMappingURL=RestService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/http/RestService");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/DISingleton", "@web-atoms/core/dist/services/http/RestService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const DISingleton_1 = require("@web-atoms/core/dist/di/DISingleton");
    const RestService_1 = require("@web-atoms/core/dist/services/http/RestService");
    /**
     * It is easy to mock any web service by specifying mock parameter while
     * setting it up as DISingleton.
     *
     * When designMode is true, mock web service will be used, please make sure
     * mock class must be set as default in mock module.
     */
    let MovieService = class MovieService extends RestService_1.BaseService {
        getMovies(category, search, start, size) {
            // don't worry about null, it will never execute..
            return null;
        }
    };
    __decorate([
        RestService_1.Get("/api/movies/{category}"),
        __param(0, RestService_1.Path("category")),
        __param(1, RestService_1.Query("search")),
        __param(2, RestService_1.Query("start")),
        __param(3, RestService_1.Query("size")),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String, String, Number, Number]),
        __metadata("design:returntype", Promise)
    ], MovieService.prototype, "getMovies", null);
    MovieService = __decorate([
        DISingleton_1.default({ mock: "./mocks/MockMovieService" })
    ], MovieService);
    exports.default = MovieService;
});
//# sourceMappingURL=MovieService.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/service/http/MovieService");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/view-model/AtomViewModel", "../../service/http/MovieService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const MovieService_1 = require("../../service/http/MovieService");
    class SearchBarViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.searchText = "";
        }
        init() {
            return __awaiter(this, void 0, void 0, function* () {
                this.movies = yield this.movieService.getMovies("", "", 0, 10);
                this.searchFromList = this.movies.value.map((x) => x);
            });
        }
        search() {
            this.movies.value.replace(this.searchFromList.filter((x) => x.name.toLowerCase().search(this.searchText.toLowerCase()) > -1
                || this.searchText === ""));
        }
        watchSearch() {
            if (this.searchText || this.searchText === "") {
                this.search();
            }
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MovieService_1.default)
    ], SearchBarViewModel.prototype, "movieService", void 0);
    __decorate([
        AtomViewModel_1.Watch,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], SearchBarViewModel.prototype, "watchSearch", null);
    exports.default = SearchBarViewModel;
});
//# sourceMappingURL=SearchBarViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/search-bar/SearchBarViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./SearchBarViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const SearchBarViewModel_1 = require("./SearchBarViewModel");
    class SearchBarView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(SearchBarViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "SearchBox Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.SearchBar, { text: Bind_1.default.twoWays((x) => this.viewModel.searchText), placeholder: "Search text", textColor: "White", placeholderColor: "White", backgroundColor: "#bbb" }),
                    XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.movies.value) },
                        XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.ViewCell, { height: 100 },
                                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.name), verticalOptions: "Center", textColor: "Black", margin: "10" }))))))));
        }
    }
    exports.default = SearchBarView;
});
//# sourceMappingURL=SearchBarView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/search-bar/SearchBarView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class SliderViewModel extends AtomViewModel_1.AtomViewModel {
    }
    exports.default = SliderViewModel;
});
//# sourceMappingURL=SliderViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/slider/SliderViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./SliderViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const SliderViewModel_1 = require("./SliderViewModel");
    class SliderView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(SliderViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Slider Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { margin: "100, 100, 100, 100" },
                    XNode_1.default.create(XF_1.default.Slider, { minimum: 0, maximum: 10, value: Bind_1.default.twoWays(() => this.viewModel.slide) }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => this.viewModel.slide) }))));
        }
    }
    exports.default = SliderView;
});
//# sourceMappingURL=SliderView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/slider/SliderView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class StepperViewModel extends AtomViewModel_1.AtomViewModel {
    }
    exports.default = StepperViewModel;
});
//# sourceMappingURL=StepperViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/stepper/StepperViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./StepperViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const StepperViewModel_1 = require("./StepperViewModel");
    class StepperView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(StepperViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Stepper Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { margin: "20" },
                    XNode_1.default.create(XF_1.default.Stepper, { maximum: 360, increment: 30, horizontalOptions: "Center", value: Bind_1.default.twoWays(() => this.viewModel.step) }),
                    XNode_1.default.create(XF_1.default.Label, { horizontalOptions: "Center", verticalOptions: "CenterAndExpand", text: Bind_1.default.oneWay(() => this.viewModel.step) }))));
        }
    }
    exports.default = StepperView;
});
//# sourceMappingURL=StepperView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/stepper/StepperView");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/Action", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Action_1 = require("@web-atoms/core/dist/view-model/Action");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class SimpleFormViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.model = {
                username: "",
                password: ""
            };
        }
        get errorUsername() {
            return this.model.username ? "" : "Username is required";
        }
        get errorPassword() {
            return this.model.password ? "" : "Password is required";
        }
        signup() {
            return __awaiter(this, void 0, void 0, function* () {
                // nothing
            });
        }
    }
    __decorate([
        AtomViewModel_1.Validate,
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [])
    ], SimpleFormViewModel.prototype, "errorUsername", null);
    __decorate([
        AtomViewModel_1.Validate,
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [])
    ], SimpleFormViewModel.prototype, "errorPassword", null);
    __decorate([
        Action_1.default({ validate: true, success: "Signup form is valid" }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], SimpleFormViewModel.prototype, "signup", null);
    exports.default = SimpleFormViewModel;
});
//# sourceMappingURL=SimpleFormViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/form/simple/SimpleFormViewModel");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./SimpleFormViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const SimpleFormViewModel_1 = require("./SimpleFormViewModel");
    const NSXF = XNode_1.default.namespace("Xamarin.Forms", "Xamarin.Forms.Core");
    let TemplateBinding = class TemplateBinding extends XNode_1.RootObject {
    };
    TemplateBinding = __decorate([
        NSXF("TemplateBinding")
    ], TemplateBinding);
    const XF2 = {
        TemplateBinding
    };
    class SimpleForm extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(SimpleFormViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Simple Form", backgroundColor: "White", padding: "10" },
                XNode_1.default.create(WA_1.default.AtomForm, null,
                    XNode_1.default.create(WA_1.default.AtomField, { label: "Username:", isRequired: true, error: Bind_1.default.oneWay(() => this.viewModel.errorUsername), labelColor: "#2e2e2e" },
                        XNode_1.default.create(XF_1.default.Entry, { text: Bind_1.default.twoWays(() => this.viewModel.model.username), placeholder: "Enter Username", placeholderColor: "#aaa", textColor: "#2e2e2e" })),
                    XNode_1.default.create(WA_1.default.AtomField, { label: "Password", isRequired: true, error: Bind_1.default.oneWay(() => this.viewModel.errorPassword), labelColor: "#2e2e2e" },
                        XNode_1.default.create(XF_1.default.Entry, { isPassword: true, text: Bind_1.default.twoWays(() => this.viewModel.model.password), placeholder: "Enter Password", placeholderColor: "#aaa", textColor: "#2e2e2e" })),
                    XNode_1.default.create(XF_1.default.Button, { command: Bind_1.default.event((s, e) => this.viewModel.signup()), text: "Signup", borderRadius: "5", backgroundColor: "#ff5733", borderColor: "#ff5733", textColor: "white" }))));
        }
    }
    exports.default = SimpleForm;
});
//# sourceMappingURL=SimpleForm.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/form/simple/SimpleForm");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../button/ButtonView", "../button/image-button/ImageButtonView", "../check-box/CheckBoxView", "../combo-box/ComboBoxSample", "../date-picker/DatePickerView", "../editor/EditorView", "../entry/EntryView", "../label/LabelView", "../search-bar/SearchBarView", "../slider/SliderView", "../stepper/StepperView", "./simple/SimpleForm"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const ButtonView_1 = require("../button/ButtonView");
    const ImageButtonView_1 = require("../button/image-button/ImageButtonView");
    const CheckBoxView_1 = require("../check-box/CheckBoxView");
    const ComboBoxSample_1 = require("../combo-box/ComboBoxSample");
    const DatePickerView_1 = require("../date-picker/DatePickerView");
    const EditorView_1 = require("../editor/EditorView");
    const EntryView_1 = require("../entry/EntryView");
    const LabelView_1 = require("../label/LabelView");
    const SearchBarView_1 = require("../search-bar/SearchBarView");
    const SliderView_1 = require("../slider/SliderView");
    const StepperView_1 = require("../stepper/StepperView");
    const SimpleForm_1 = require("./simple/SimpleForm");
    function addFormSamples(ms) {
        const form = ms.addGroup("Form");
        form.addTabLink("Simple Form", SimpleForm_1.default);
        form.addTabLink("Label", LabelView_1.default);
        form.addTabLink("CheckBox", CheckBoxView_1.default);
        form.addTabLink("Entry", EntryView_1.default);
        form.addTabLink("Editor", EditorView_1.default);
        form.addTabLink("Button", ButtonView_1.default);
        form.addTabLink("Image Button", ImageButtonView_1.default);
        form.addTabLink("Search Bar", SearchBarView_1.default);
        form.addTabLink("Date Picker", DatePickerView_1.default);
        form.addTabLink("Slider", SliderView_1.default);
        form.addTabLink("Stepper", StepperView_1.default);
        form.addTabLink("ComboBox", ComboBoxSample_1.default);
    }
    exports.default = addFormSamples;
});
//# sourceMappingURL=FormSamples.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/form/FormSamples");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class ImageView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Image View Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { margin: "20,35,20,20" },
                    XNode_1.default.create(XF_1.default.Image, { source: "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Xamarin-logo.svg/1920px-Xamarin-logo.svg.png", heightRequest: 300 }))));
        }
    }
    exports.default = ImageView;
});
//# sourceMappingURL=ImageView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/image/ImageView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./ImageView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const ImageView_1 = require("./ImageView");
    function addImage(ms) {
        const image = ms.addGroup("Image");
        image.addTabLink("Image", ImageView_1.default);
    }
    exports.default = addImage;
});
//# sourceMappingURL=ImageSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/image/ImageSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class AbsoluteLayoutView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Absolute Layout Sample" },
                XNode_1.default.create(XF_1.default.ContentPage.content, null,
                    XNode_1.default.create(XF_1.default.AbsoluteLayout, null,
                        XNode_1.default.create(XF_1.default.Label, Object.assign({ text: "I'm centered on iPhone 4 but no other device" }, XF_1.default.AbsoluteLayout.layoutBounds("115, 150, 100, 100"), { lineBreakMode: "WordWrap" })),
                        XNode_1.default.create(XF_1.default.Label, Object.assign({ text: "I'm bottom center on every device." }, XF_1.default.AbsoluteLayout.layoutBounds("0.5, 1, 0.5, .1"), XF_1.default.AbsoluteLayout.layoutFlags("All"), { lineBreakMode: "WordWrap" })),
                        XNode_1.default.create(XF_1.default.BoxView, Object.assign({ color: "Olive" }, XF_1.default.AbsoluteLayout.layoutBounds("1, 0.5, 25, 100"), XF_1.default.AbsoluteLayout.layoutFlags("PositionProportional"))),
                        XNode_1.default.create(XF_1.default.BoxView, Object.assign({ color: "Red" }, XF_1.default.AbsoluteLayout.layoutBounds("0, 0.5, 25, 100"), XF_1.default.AbsoluteLayout.layoutFlags("PositionProportional"))),
                        XNode_1.default.create(XF_1.default.BoxView, Object.assign({ color: "Blue" }, XF_1.default.AbsoluteLayout.layoutBounds("0.5, 0, 100, 25"), XF_1.default.AbsoluteLayout.layoutFlags("PositionProportional")))))));
        }
    }
    exports.default = AbsoluteLayoutView;
});
//# sourceMappingURL=AbsoluteLayoutView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/multiple-content/absolute-layout/AbsoluteLayoutView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class FlexLayoutView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Flex Layout Sample" },
                XNode_1.default.create(XF_1.default.FlexLayout, { Direction: "Column" },
                    XNode_1.default.create(XF_1.default.Label, { text: "HEADER", fontSize: 30, backgroundColor: "Aqua", horizontalTextAlignment: "Center" }),
                    XNode_1.default.create(XF_1.default.FlexLayout, Object.assign({}, XF_1.default.FlexLayout.grow(1)),
                        XNode_1.default.create(XF_1.default.Label, Object.assign({ text: "CONTENT", fontSize: "Large", backgroundColor: "Gray", horizontalTextAlignment: "Center", verticalTextAlignment: "Center" }, XF_1.default.FlexLayout.grow(1))),
                        XNode_1.default.create(XF_1.default.BoxView, Object.assign({}, XF_1.default.FlexLayout.basis(50), XF_1.default.FlexLayout.order(-1), { color: "Blue" })),
                        XNode_1.default.create(XF_1.default.BoxView, Object.assign({}, XF_1.default.FlexLayout.basis(50), { color: "Green" }))),
                    XNode_1.default.create(XF_1.default.Label, { text: "FOOTER", fontSize: 30, backgroundColor: "Pink", horizontalTextAlignment: "Center" }))));
        }
    }
    exports.default = FlexLayoutView;
});
//# sourceMappingURL=FlexLayoutView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/multiple-content/flex-layout/FlexLayoutView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class GridView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Grid Layout Sample" },
                XNode_1.default.create(XF_1.default.Grid, { margin: "20,35,20,20" },
                    XNode_1.default.create(XF_1.default.Grid.columnDefinitions, null,
                        XNode_1.default.create(XF_1.default.ColumnDefinition, { width: "0.5*" }),
                        XNode_1.default.create(XF_1.default.ColumnDefinition, { width: "0.5*" })),
                    XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                        XNode_1.default.create(XF_1.default.RowDefinition, { height: "100" }),
                        XNode_1.default.create(XF_1.default.RowDefinition, { height: "50" })),
                    XNode_1.default.create(XF_1.default.Label, { text: "Column 0, Row 0" }),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), { text: "Column 1, Row 0" })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.row(1), { text: "Column 0, Row 1" })),
                    XNode_1.default.create(XF_1.default.Label, Object.assign({}, XF_1.default.Grid.column(1), XF_1.default.Grid.row(1), { text: "Column 1, Row 1" })))));
        }
    }
    exports.default = GridView;
});
//# sourceMappingURL=GridView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/multiple-content/grid-layout/GridView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class StackLayoutView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Stack Layout Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { horizontalOptions: "Center" },
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Horizontal", margin: 20 },
                        XNode_1.default.create(XF_1.default.Label, { text: "Example" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "Of" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "Horizontal" })),
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical", margin: 20 },
                        XNode_1.default.create(XF_1.default.Label, { text: "Example" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "Of" }),
                        XNode_1.default.create(XF_1.default.Label, { text: "Vertical" })))));
        }
    }
    exports.default = StackLayoutView;
});
//# sourceMappingURL=StackLayoutView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/multiple-content/stack-layout/StackLayoutView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./absolute-layout/AbsoluteLayoutView", "./flex-layout/FlexLayoutView", "./grid-layout/GridView", "./stack-layout/StackLayoutView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AbsoluteLayoutView_1 = require("./absolute-layout/AbsoluteLayoutView");
    const FlexLayoutView_1 = require("./flex-layout/FlexLayoutView");
    const GridView_1 = require("./grid-layout/GridView");
    const StackLayoutView_1 = require("./stack-layout/StackLayoutView");
    function addLayoutSample(ms) {
        const layout = ms.addGroup("Layout");
        layout.addTabLink("Absolute Layout", AbsoluteLayoutView_1.default);
        layout.addTabLink("StackLayout", StackLayoutView_1.default);
        layout.addTabLink("Grid", GridView_1.default);
        layout.addTabLink("Flex Layout", FlexLayoutView_1.default);
    }
    exports.default = addLayoutSample;
});
//# sourceMappingURL=LayoutSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/multiple-content/LayoutSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/AtomContentView", "@web-atoms/xf-controls/dist/clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const AtomContentView_1 = require("@web-atoms/xf-controls/dist/AtomContentView");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    class ContentView extends AtomContentView_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentView, null,
                XNode_1.default.create(XF_1.default.StackLayout, { padding: 10 },
                    XNode_1.default.create(XF_1.default.Label, { text: "Item 1", fontSize: 20 }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Item 2", fontSize: 20 }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Item 3", fontSize: 20 }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Item 4", fontSize: 20 }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Item 5", fontSize: 20 }))));
        }
    }
    exports.default = ContentView;
});
//# sourceMappingURL=ContentView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/single-content/content-view/ContentView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./ContentView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ContentView_1 = require("./ContentView");
    class MainPage extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Content View Sample" },
                XNode_1.default.create(ContentView_1.default, null)));
        }
    }
    exports.default = MainPage;
});
//# sourceMappingURL=MainPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/single-content/content-view/MainPage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "../../../alert/AlertSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const AlertSample_1 = require("../../../alert/AlertSample");
    class FrameSample extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, null,
                XNode_1.default.create(XF_1.default.Button, { text: "Open Page", command: Bind_1.default.event(() => { this.viewModel.changeFrame(); }) }),
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Frame, null,
                        XNode_1.default.create(AlertSample_1.default, null)))));
        }
    }
    exports.default = FrameSample;
});
//# sourceMappingURL=FrameSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/single-content/frame/FrameSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    class ScrollViewSample extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Scroll View Sample" },
                XNode_1.default.create(XF_1.default.ScrollView, null,
                    XNode_1.default.create(XF_1.default.StackLayout, null,
                        XNode_1.default.create(XF_1.default.BoxView, { backgroundColor: "Red", heightRequest: 600, widthRequest: 150 }),
                        XNode_1.default.create(XF_1.default.Entry, null)))));
        }
    }
    exports.default = ScrollViewSample;
});
//# sourceMappingURL=ScrollViewSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/single-content/scroll-view/ScrollViewSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./content-view/MainPage", "./frame/FrameSample", "./scroll-view/ScrollViewSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const MainPage_1 = require("./content-view/MainPage");
    const FrameSample_1 = require("./frame/FrameSample");
    const ScrollViewSample_1 = require("./scroll-view/ScrollViewSample");
    function addSingleContentSample(ms) {
        const sv = ms.addGroup("Single Content View");
        sv.addTabLink("Scroll View", ScrollViewSample_1.default);
        sv.addTabLink("Frame Alert", FrameSample_1.default);
        sv.addTabLink("Content View", MainPage_1.default);
    }
    exports.default = addSingleContentSample;
});
//# sourceMappingURL=Sample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/layout/single-content/Sample");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel", "../../../service/http/MovieService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const MovieService_1 = require("../../../service/http/MovieService");
    class ListViewModel extends AtomViewModel_1.AtomViewModel {
        /**
         * You must initialize your model by calling web services in init method
         */
        init() {
            return __awaiter(this, void 0, void 0, function* () {
                this.movies = yield this.movieService.getMovies("", "", 0, 10);
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MovieService_1.default)
    ], ListViewModel.prototype, "movieService", void 0);
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], ListViewModel.prototype, "navigationService", void 0);
    exports.default = ListViewModel;
});
//# sourceMappingURL=ListViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/list/list-view/ListViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./ListViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ListViewModel_1 = require("./ListViewModel");
    class List extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ListViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "List Sample" },
                XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.movies.value) },
                    XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.DataTemplate, null,
                            XNode_1.default.create(XF_1.default.ViewCell, null,
                                XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.name) })))))));
        }
    }
    exports.default = List;
});
//# sourceMappingURL=List.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/list/list-view/List");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "../list-view/ListViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ListViewModel_1 = require("../list-view/ListViewModel");
    class ListWithTemplates extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ListViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "List with Template Selector" },
                XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.movies.value) },
                    XNode_1.default.create(WA_1.default.AtomTemplateSelector.templateSelector, null,
                        XNode_1.default.create(WA_1.default.AtomTemplateSelector, { selector: (d) => /horror/i.test(d.genre) ? 1 : 0 },
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.TextCell, { text: Bind_1.default.oneWay((x) => x.data.name), textColor: "black" })),
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.TextCell, { text: Bind_1.default.oneWay((x) => x.data.name), textColor: "red" })))))));
        }
    }
    exports.default = ListWithTemplates;
});
//# sourceMappingURL=ListWithTemplates.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/list/template-selector/ListWithTemplates");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./list-view/List", "./template-selector/ListWithTemplates"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const List_1 = require("./list-view/List");
    const ListWithTemplates_1 = require("./template-selector/ListWithTemplates");
    function addListSamples(ms) {
        const list = ms.addGroup("List");
        list.addTabLink("List View", List_1.default);
        list.addTabLink("List View  with Template Selector", ListWithTemplates_1.default);
    }
    exports.default = addListSamples;
});
//# sourceMappingURL=ListSamples.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/list/ListSamples");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel", "../../service/http/MovieService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const MovieService_1 = require("../../service/http/MovieService");
    class MenuItemViewModel extends AtomViewModel_1.AtomViewModel {
        /**
         * You must initialize your model by calling web services in init method
         */
        init() {
            return __awaiter(this, void 0, void 0, function* () {
                this.movies = yield this.movieService.getMovies("", "", 0, 10);
            });
        }
        edit() {
            return __awaiter(this, void 0, void 0, function* () {
                yield this.navigationService.alert("Edit clicked");
            });
        }
        delete() {
            return __awaiter(this, void 0, void 0, function* () {
                yield this.navigationService.alert("Delete clicked");
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MovieService_1.default)
    ], MenuItemViewModel.prototype, "movieService", void 0);
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], MenuItemViewModel.prototype, "navigationService", void 0);
    exports.default = MenuItemViewModel;
});
//# sourceMappingURL=MenuItemViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/menu-item/MenuItemViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./MenuItemViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const MenuItemViewModel_1 = require("./MenuItemViewModel");
    class MenuItemView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(MenuItemViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Menu Item" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.movies.value) },
                        XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.ViewCell, null,
                                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.name) }),
                                    XNode_1.default.create(XF_1.default.ViewCell.contextActions, null,
                                        XNode_1.default.create(XF_1.default.MenuItem, { text: "Edit", command: Bind_1.default.event(() => this.viewModel.edit()) }),
                                        XNode_1.default.create(XF_1.default.MenuItem, { text: "Delete", command: Bind_1.default.event(() => this.viewModel.delete()) })))))))));
        }
    }
    exports.default = MenuItemView;
});
//# sourceMappingURL=MenuItemView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/menu-item/MenuItemView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./MenuItemView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const MenuItemView_1 = require("./MenuItemView");
    function addMenuItem(ms) {
        const menuItem = ms.addGroup("MenuItem");
        menuItem.addTabLink("Menu Item", MenuItemView_1.default);
    }
    exports.default = addMenuItem;
});
//# sourceMappingURL=MenuSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/menu-item/MenuSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/RgPluginsPopup", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFPopupPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const RgPluginsPopup_1 = require("@web-atoms/xf-controls/dist/clr/RgPluginsPopup");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFPopupPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFPopupPage");
    class PopupView extends AtomXFPopupPage_1.default {
        create() {
            this.render(XNode_1.default.create(RgPluginsPopup_1.default.PopupPage, { title: "Popup Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, { padding: "20", backgroundColor: Colors_1.default.white, horizontalOptions: "Center", verticalOptions: "Center" },
                    XNode_1.default.create(XF_1.default.Label, { text: "Label 1" }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Label 2" }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Label 3" }),
                    XNode_1.default.create(XF_1.default.Button, { text: "Close", command: Bind_1.default.event(() => this.viewModel.close("Closed")) }))));
        }
    }
    exports.default = PopupView;
});
//# sourceMappingURL=PopupView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/popup/PopupView");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel", "./PopupView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const PopupView_1 = require("./PopupView");
    class PopupCallingPageViewModel extends AtomViewModel_1.AtomViewModel {
        clickEvent(str) {
            return __awaiter(this, void 0, void 0, function* () {
                const r = yield this.navigationService.openPage(PopupView_1.default, {}, { clearHistory: false });
                // tslint:disable-next-line: no-console
                console.log(r);
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], PopupCallingPageViewModel.prototype, "navigationService", void 0);
    exports.default = PopupCallingPageViewModel;
});
//# sourceMappingURL=PopupCallingPageViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/popup/PopupCallingPageViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./PopupCallingPageViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const PopupCallingPageViewModel_1 = require("./PopupCallingPageViewModel");
    class PopupCallingPage extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(PopupCallingPageViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Popup Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Label, { text: "Click the button", fontSize: 30, fontAttributes: "Bold", horizontalOptions: "Center" }),
                    XNode_1.default.create(XF_1.default.Button, { margin: 50, padding: 10, text: "Popup Demo", command: Bind_1.default.event((s, e) => this.viewModel.clickEvent("Button")) }))));
        }
    }
    exports.default = PopupCallingPage;
});
//# sourceMappingURL=PopupCallingPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/popup/PopupCallingPage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./PopupCallingPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const PopupCallingPage_1 = require("./PopupCallingPage");
    function addPopupSample(ms) {
        const popup = ms.addGroup("Popup Sample");
        popup.addTabLink("Popup", PopupCallingPage_1.default);
    }
    exports.default = addPopupSample;
});
//# sourceMappingURL=PopupSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/popup/PopupSample");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/view-model/AtomViewModel", "../../service/http/MovieService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const MovieService_1 = require("../../service/http/MovieService");
    class RefreshViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.isListRefreshing = false;
        }
        init() {
            return __awaiter(this, void 0, void 0, function* () {
                this.movies = yield this.movieService.getMovies("", "", 0, 10);
            });
        }
        refresh() {
            return __awaiter(this, void 0, void 0, function* () {
                this.movies.value.replace(this.movies.value.reverse());
                this.isListRefreshing = false;
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MovieService_1.default)
    ], RefreshViewModel.prototype, "movieService", void 0);
    exports.default = RefreshViewModel;
});
//# sourceMappingURL=RefreshViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/refresh-view/RefreshViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./RefreshViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const RefreshViewModel_1 = require("./RefreshViewModel");
    class RefreshView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(RefreshViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Refresh View Sample" },
                XNode_1.default.create(XF_1.default.RefreshView, { refreshColor: Colors_1.default.blue, isRefreshing: Bind_1.default.twoWays((x) => this.viewModel.isListRefreshing), command: Bind_1.default.event((s, e) => this.viewModel.refresh()) },
                    XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.movies.value) },
                        XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.ViewCell, { height: 100 },
                                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.name) }))))))));
        }
    }
    exports.default = RefreshView;
});
//# sourceMappingURL=RefreshView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/refresh-view/RefreshView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./RefreshView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const RefreshView_1 = require("./RefreshView");
    function addRefreshSample(ms) {
        const rs = ms.addGroup("Refresh-View");
        rs.addTabLink("Refresh View", RefreshView_1.default);
    }
    exports.default = addRefreshSample;
});
//# sourceMappingURL=RefreshViewSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/refresh-view/RefreshViewSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class SwitchViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.status = false;
            this.status2 = false;
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
            });
        }
        switchStatus() {
            this.status = !this.status;
        }
        switchStatus2() {
            this.status2 = !this.status2;
        }
    }
    exports.default = SwitchViewModel;
});
//# sourceMappingURL=SwitchViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/switch/SwitchViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./SwitchViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const SwitchViewModel_1 = require("./SwitchViewModel");
    class SwitchSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(SwitchViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Switch Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Switch, { isToggled: Bind_1.default.twoWays(() => this.viewModel.switchStatus) }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.twoWays(() => this.viewModel.status ? "On" : "Off") }),
                    XNode_1.default.create(XF_1.default.Switch, { onColor: "orange", thumbColor: "green", isToggled: Bind_1.default.event(() => this.viewModel.switchStatus2()) }),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.twoWays(() => this.viewModel.status2 ? "On" : "Off") }))));
        }
    }
    exports.default = SwitchSample;
});
//# sourceMappingURL=SwitchSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/switch/SwitchSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./SwitchSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const SwitchSample_1 = require("./SwitchSample");
    function addSwitchSample(ms) {
        const s = ms.addGroup("Switch");
        s.addTabLink("Switch", SwitchSample_1.default);
    }
    exports.default = addSwitchSample;
});
//# sourceMappingURL=SwitchSamplePage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/switch/SwitchSamplePage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const XF_1 = require("../clr/XF");
    class AtomXFTabbedPage extends AtomXFControl_1.AtomXFControl {
        constructor(app, e) {
            super(app, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.TabbedPage));
        }
    }
    exports.default = AtomXFTabbedPage;
});
//# sourceMappingURL=AtomXFTabbedPage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/pages/AtomXFTabbedPage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/XNode", "@web-atoms/font-awesome/dist/FontAwesomeRegular", "@web-atoms/font-awesome/dist/FontAwesomeSolid", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFTabbedPage", "../form/simple/SimpleForm", "../list/list-view/List"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const FontAwesomeRegular_1 = require("@web-atoms/font-awesome/dist/FontAwesomeRegular");
    const FontAwesomeSolid_1 = require("@web-atoms/font-awesome/dist/FontAwesomeSolid");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFTabbedPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFTabbedPage");
    const SimpleForm_1 = require("../form/simple/SimpleForm");
    const List_1 = require("../list/list-view/List");
    const Args = XNode_1.default.prepare("WebAtoms.AtomX:Arguments", true);
    class TabbedPageView extends AtomXFTabbedPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.TabbedPage, { title: "Tabbed Page Sample", selectedTabColor: Colors_1.default.black, unselectedTabColor: Colors_1.default.grey },
                XNode_1.default.create(XF_1.default.ContentPage, { title: "Tab 1" },
                    XNode_1.default.create(XF_1.default.ContentPage.iconImageSource, null,
                        XNode_1.default.create(XF_1.default.FontImageSource, { fontFamily: FontAwesomeRegular_1.default.toString(), glyph: FontAwesomeRegular_1.default.user })),
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical" },
                        XNode_1.default.create(XF_1.default.Label, { text: "Tab 1 Content" }),
                        XNode_1.default.create(XF_1.default.Image, null,
                            XNode_1.default.create(XF_1.default.Image.source, null,
                                XNode_1.default.create(XF_1.default.FontImageSource, { color: "#000000", fontFamily: FontAwesomeSolid_1.default.toString(), glyph: FontAwesomeSolid_1.default.gem, size: "20" }))))),
                XNode_1.default.create(XF_1.default.ContentPage, { title: "Tab 2" },
                    XNode_1.default.create(XF_1.default.ContentPage.iconImageSource, null,
                        XNode_1.default.create(XF_1.default.FontImageSource, { fontFamily: FontAwesomeRegular_1.default.toString(), glyph: FontAwesomeRegular_1.default.map })),
                    XNode_1.default.create(XF_1.default.StackLayout, { orientation: "Vertical" },
                        XNode_1.default.create(XF_1.default.Label, { text: "Tab 2 Content" }),
                        XNode_1.default.create(XF_1.default.Image, null,
                            XNode_1.default.create(XF_1.default.Image.source, null,
                                XNode_1.default.create(XF_1.default.FontImageSource, { color: "#000000", fontFamily: FontAwesomeSolid_1.default.toString(), glyph: FontAwesomeSolid_1.default.globe, size: "20" }))))),
                XNode_1.default.create(SimpleForm_1.default, null,
                    XNode_1.default.create(XF_1.default.ContentPage.iconImageSource, null,
                        XNode_1.default.create(XF_1.default.FontImageSource, { fontFamily: FontAwesomeRegular_1.default.toString(), glyph: FontAwesomeRegular_1.default.newspaper }))),
                XNode_1.default.create(List_1.default, null,
                    XNode_1.default.create(XF_1.default.ContentPage.iconImageSource, null,
                        XNode_1.default.create(XF_1.default.FontImageSource, { fontFamily: FontAwesomeRegular_1.default.toString(), glyph: FontAwesomeRegular_1.default.listAlt })))));
        }
    }
    exports.default = TabbedPageView;
});
//# sourceMappingURL=TabbedPageView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/tabbed-page/TabbedPageView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./TabbedPageView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const TabbedPageView_1 = require("./TabbedPageView");
    function addTabbedPage(ms) {
        const tp = ms.addGroup("Tabbed Page");
        tp.addTabLink("Tabbed Page Sample", TabbedPageView_1.default);
    }
    exports.default = addTabbedPage;
});
//# sourceMappingURL=TabbedPageSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/tabbed-page/TabbedPageSample");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class TableViewModel extends AtomViewModel_1.AtomViewModel {
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
            });
        }
    }
    exports.default = TableViewModel;
});
//# sourceMappingURL=TableViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/table-view/TableViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./TableViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const TableViewModel_1 = require("./TableViewModel");
    class TableViewSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(TableViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Table View Sample" },
                XNode_1.default.create(XF_1.default.TableView, null,
                    XNode_1.default.create(XF_1.default.TableSection, { title: "Switch Section" },
                        XNode_1.default.create(XF_1.default.SwitchCell, { text: "Off State" }),
                        XNode_1.default.create(XF_1.default.SwitchCell, { text: "On State", on: true })),
                    XNode_1.default.create(XF_1.default.TableSection, { title: "Entry Section" },
                        XNode_1.default.create(XF_1.default.EntryCell, { label: "Username", placeholder: "enter username", horizontalTextAlignment: "Center" }),
                        XNode_1.default.create(XF_1.default.EntryCell, { label: "Password", placeholder: "enter password", horizontalTextAlignment: "Center" })))));
        }
    }
    exports.default = TableViewSample;
});
//# sourceMappingURL=TableViewSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/table-view/TableViewSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./TableViewSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const TableViewSample_1 = require("./TableViewSample");
    function addTableView(ms) {
        const table = ms.addGroup("Table");
        table.addTabLink("Table View", TableViewSample_1.default);
    }
    exports.default = addTableView;
});
//# sourceMappingURL=TableViewSamplePage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/table-view/TableViewSamplePage");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class TimePickerViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.date = "11:00:00";
        }
        init() {
            const _super = Object.create(null, {
                init: { get: () => super.init }
            });
            return __awaiter(this, void 0, void 0, function* () {
                _super.init.call(this);
            });
        }
    }
    exports.default = TimePickerViewModel;
});
//# sourceMappingURL=TimePickerViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/time-picker/TimePickerViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./TimePickerViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const TimePickerViewModel_1 = require("./TimePickerViewModel");
    class TimePickerSample extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(TimePickerViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Time Picker Sample" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.Entry, { placeholder: "Enter the event to be reminded of" }),
                    XNode_1.default.create(XF_1.default.Label, { text: "Select the time below to be reminded at." }),
                    XNode_1.default.create(XF_1.default.TimePicker, { format: "T" }))));
        }
    }
    exports.default = TimePickerSample;
});
//# sourceMappingURL=TimePickerSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/time-picker/TimePickerSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./TimePickerSample"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const TimePickerSample_1 = require("./TimePickerSample");
    function addTimePicker(ms) {
        const time = ms.addGroup("Selector");
        time.addTabLink("Time Picker", TimePickerSample_1.default);
    }
    exports.default = addTimePicker;
});
//# sourceMappingURL=TimePickerSamplePage.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/time-picker/TimePickerSamplePage");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBinder", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/core/AtomWatcher", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/PropertyBinding", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/xf/controls/AtomXFControl", "../clr/WA", "../clr/XF"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBinder_1 = require("@web-atoms/core/dist/core/AtomBinder");
    const AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    const AtomWatcher_1 = require("@web-atoms/core/dist/core/AtomWatcher");
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const PropertyBinding_1 = require("@web-atoms/core/dist/core/PropertyBinding");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const AtomXFControl_1 = require("@web-atoms/core/dist/xf/controls/AtomXFControl");
    const WA_1 = require("../clr/WA");
    const XF_1 = require("../clr/XF");
    class ToggleButtonBarViewModel extends AtomViewModel_1.AtomViewModel {
    }
    class AtomXFToggleButtonBar extends AtomXFControl_1.AtomXFControl {
        constructor(app, e) {
            super(app, e || AtomBridge_1.AtomBridge.instance.create(XF_1.default.Frame));
        }
        preCreate() {
            this.items = null;
            this.value = null;
            this.labelPath = "label";
            this.valuePath = "value";
            this.itemTemplate = null;
            this.selectedItem = null;
            const vf = (item) => {
                if (!item) {
                    return null;
                }
                const vp = this.valuePath;
                if (typeof vp === "function") {
                    return vp(item);
                }
                return item[vp];
            };
            this.registerDisposable(new PropertyBinding_1.PropertyBinding(this, null, "value", [["this", "selectedItem"]], true, {
                fromSource: (v) => this.value = vf(this.selectedItem),
                fromTarget: (v) => this.selectedItem = this.items.find((x) => vf(x) === this.value)
            }, this));
            this.registerDisposable(new AtomWatcher_1.AtomWatcher(this, () => this.items, () => {
                if (this.selectedItem === null) {
                    AtomBinder_1.AtomBinder.refreshValue(this, "value");
                }
            }));
        }
        create() {
            this.localViewModel = this.resolve(ToggleButtonBarViewModel);
            this.localViewModel.owner = this;
            this.render(XNode_1.default.create(XF_1.default.Frame, { padding: 2, heightRequest: 40 },
                XNode_1.default.create(AtomXFToggleButtonBar.itemTemplate, null,
                    XNode_1.default.create(XF_1.default.DataTemplate, null,
                        XNode_1.default.create(XF_1.default.Label, { padding: 10, horizontalOptions: "FillAndExpand", horizontalTextAlignment: "Center", verticalTextAlignment: "Center", verticalOptions: "Center", text: Bind_1.default.oneWay((x) => x.data ? x.data[this.labelPath] : "."), backgroundColor: Bind_1.default.oneWay((x) => x.data === this.selectedItem
                                ? Colors_1.default.black
                                : Colors_1.default.white), textColor: Bind_1.default.oneWay((x) => x.data !== this.selectedItem
                                ? Colors_1.default.black
                                : Colors_1.default.white) }))),
                XNode_1.default.create(XF_1.default.StackLayout, Object.assign({ orientation: "Horizontal" }, XF_1.default.BindableLayout.itemsSource(Bind_1.default.oneWay(() => this.items))),
                    XNode_1.default.create(XF_1.default.BindableLayout.itemTemplate, null,
                        XNode_1.default.create(XF_1.default.DataTemplate, null,
                            XNode_1.default.create(WA_1.default.AtomView, { backgroundColor: Colors_1.default.lightBlue, horizontalOptions: "FillAndExpand", dataTemplate: Bind_1.default.oneWay(() => this.itemTemplate) },
                                XNode_1.default.create(XF_1.default.View.gestureRecognizers, null,
                                    XNode_1.default.create(XF_1.default.TapGestureRecognizer, { command: Bind_1.default.event((x) => this.selectedItem = x.data) }))))))));
        }
    }
    exports.default = AtomXFToggleButtonBar;
    AtomXFToggleButtonBar.itemTemplate = XNode_1.default.prepare("itemTemplate", true, true);
});
//# sourceMappingURL=AtomXFToggleButtonBar.js.map

    AmdLoader.instance.setup("@web-atoms/xf-controls/dist/toggle-button-bar/AtomXFToggleButtonBar");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    class ToggleButtonBarViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.genderList = [
                { label: "Male", value: "male" },
                { label: "Female", value: "female" }
            ];
            this.gender = "female";
        }
    }
    exports.default = ToggleButtonBarViewModel;
});
//# sourceMappingURL=ToggleButtonBarViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toggle-button-bar/simple/ToggleButtonBarViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/core/XNode", "@web-atoms/font-awesome/dist/FontAwesomeSolid", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "@web-atoms/xf-controls/dist/toggle-button-bar/AtomXFToggleButtonBar", "../simple/ToggleButtonBarViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const FontAwesomeSolid_1 = require("@web-atoms/font-awesome/dist/FontAwesomeSolid");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const AtomXFToggleButtonBar_1 = require("@web-atoms/xf-controls/dist/toggle-button-bar/AtomXFToggleButtonBar");
    const ToggleButtonBarViewModel_1 = require("../simple/ToggleButtonBarViewModel");
    class CustomToggleButtonBar extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ToggleButtonBarViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Custom Toggle Button Bar" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(AtomXFToggleButtonBar_1.default, { items: this.viewModel.genderList, value: Bind_1.default.twoWays(() => this.viewModel.gender) },
                        XNode_1.default.create(AtomXFToggleButtonBar_1.default.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.Label, { padding: 10, fontSize: 30, fontFamily: FontAwesomeSolid_1.default.toString(), text: Bind_1.default.oneTime((x) => /female/i.test(x.data.value)
                                        ? FontAwesomeSolid_1.default.female
                                        : FontAwesomeSolid_1.default.male), textColor: Bind_1.default.oneWay((x) => x.data === x.localViewModel.owner.selectedItem
                                        ? Colors_1.default.white
                                        : Colors_1.default.black), backgroundColor: Bind_1.default.oneWay((x) => x.data !== x.localViewModel.owner.selectedItem
                                        ? Colors_1.default.white
                                        : Colors_1.default.black) })))),
                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay(() => `Selected gender is ${this.viewModel.gender}`) }))));
        }
    }
    exports.default = CustomToggleButtonBar;
});
//# sourceMappingURL=CustomToggleButtonBar.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toggle-button-bar/custom/CustomToggleButtonBar");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "@web-atoms/xf-controls/dist/toggle-button-bar/AtomXFToggleButtonBar", "./ToggleButtonBarViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const AtomXFToggleButtonBar_1 = require("@web-atoms/xf-controls/dist/toggle-button-bar/AtomXFToggleButtonBar");
    const ToggleButtonBarViewModel_1 = require("./ToggleButtonBarViewModel");
    class ToggleButtonBar extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ToggleButtonBarViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Toggle Button Bar" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(AtomXFToggleButtonBar_1.default, { items: this.viewModel.genderList, value: Bind_1.default.twoWays(() => this.viewModel.gender) }),
                    XNode_1.default.create(XF_1.default.Label, { fontSize: "Large", text: Bind_1.default.oneWay(() => `Selected gender is ${this.viewModel.gender}`) }))));
        }
    }
    exports.default = ToggleButtonBar;
});
//# sourceMappingURL=ToggleButtonBar.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toggle-button-bar/simple/ToggleButtonBar");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./custom/CustomToggleButtonBar", "./simple/ToggleButtonBar"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const CustomToggleButtonBar_1 = require("./custom/CustomToggleButtonBar");
    const ToggleButtonBar_1 = require("./simple/ToggleButtonBar");
    function addToggleButtonBar(ms) {
        const g = ms.addGroup("Toggle Button Bar");
        g.addTabLink("Simple", ToggleButtonBar_1.default);
        g.addTabLink("Custom Template", CustomToggleButtonBar_1.default);
    }
    exports.default = addToggleButtonBar;
});
//# sourceMappingURL=addToggleButtonBar.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toggle-button-bar/addToggleButtonBar");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/view-model/AtomViewModel", "../../service/http/MovieService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const MovieService_1 = require("../../service/http/MovieService");
    class ToolbarItemViewModel extends AtomViewModel_1.AtomViewModel {
        /**
         * You must initialize your model by calling web services in init method
         */
        init() {
            return __awaiter(this, void 0, void 0, function* () {
                this.movies = yield this.movieService.getMovies("", "", 0, 10);
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MovieService_1.default)
    ], ToolbarItemViewModel.prototype, "movieService", void 0);
    exports.default = ToolbarItemViewModel;
});
//# sourceMappingURL=ToolbarItemViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toolbar-item/ToolbarItemViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./ToolbarItemViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const ToolbarItemViewModel_1 = require("./ToolbarItemViewModel");
    class ToolbarItemView extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(ToolbarItemViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Toolbar Item Sample" },
                XNode_1.default.create(XF_1.default.ContentPage.toolbarItems, null,
                    XNode_1.default.create(XF_1.default.ToolbarItem, { text: "Toolbar 1", order: "Primary", priority: 0 }),
                    XNode_1.default.create(XF_1.default.ToolbarItem, { text: "Toolbar 2", order: "Secondary", priority: 0 }),
                    XNode_1.default.create(XF_1.default.ToolbarItem, { text: "Toolbar 3", order: "Secondary", priority: 0 })),
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.ListView, { itemsSource: Bind_1.default.oneWay(() => this.viewModel.movies.value) },
                        XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.ViewCell, null,
                                    XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.name) }))))))));
        }
    }
    exports.default = ToolbarItemView;
});
//# sourceMappingURL=ToolbarItemView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toolbar-item/ToolbarItemView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./ToolbarItemView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const ToolbarItemView_1 = require("./ToolbarItemView");
    function addToolbarItem(ms) {
        const ti = ms.addGroup("ToolbarItem");
        ti.addTabLink("ToolbarItem", ToolbarItemView_1.default);
    }
    exports.default = addToolbarItem;
});
//# sourceMappingURL=ToolbarItemSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/toolbar-item/ToolbarItemSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/XNode", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const html = `
<html>
    <body>
        <div>Test</div>
    </body>
</html>
`;
    class WebView extends AtomXFContentPage_1.default {
        create() {
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Web View Demo" },
                XNode_1.default.create(XF_1.default.StackLayout, null,
                    XNode_1.default.create(XF_1.default.WebView, { heightRequest: 1000, widthRequest: 1000, source: html }))));
        }
    }
    exports.default = WebView;
});
//# sourceMappingURL=WebView.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/web-view/WebView");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./WebView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const WebView_1 = require("./WebView");
    function addWebViewSample(ms) {
        const wv = ms.addGroup("Web View");
        wv.addTabLink("Web View Sample", WebView_1.default);
    }
    exports.default = addWebViewSample;
});
//# sourceMappingURL=WebViewSample.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/samples/web-view/WebViewSample");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomList"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.asClass = exports.asView = void 0;
    const AtomList_1 = require("@web-atoms/core/dist/core/AtomList");
    function asView(a) {
        return {
            class: a,
            extension: ".html"
        };
    }
    exports.asView = asView;
    function asClass(a) {
        return {
            class: a,
            extension: ".ts"
        };
    }
    exports.asClass = asClass;
    class MenuItem {
        constructor(app, menuService) {
            this.app = app;
            this.menuService = menuService;
            this.require = null;
            this.enabled = true;
            this.children = new AtomList_1.AtomList();
        }
        click(fromHome) {
            // this is hack as MasterDetailPage's IsPresented is set only property
            if (!fromHome) {
                this.menuService.isOpen = true;
            }
            const r = this.action(this);
            this.menuService.isOpen = false;
            return r;
        }
        add(label, action, icon) {
            const m = this.menuService.create(label, action, icon);
            this.children.add(m);
            return m;
        }
        addGroup(label, icon, require) {
            const m = this.menuService.createGroup(label, icon);
            m.require = require;
            this.children.add(m);
            return m;
        }
        addLink(label, pageSrc, pageParameters, icon) {
            const m = this.menuService.createLink(label, pageSrc, pageParameters, icon);
            this.children.add(m);
            return m;
        }
        addTabLink(label, pageSrc, pageParameters, icon) {
            const m = this.menuService.createLink(label, pageSrc, pageParameters, icon, { target: "app" });
            this.children.add(m);
            return m;
        }
        toString() {
            return this.label;
        }
    }
    exports.default = MenuItem;
});
//# sourceMappingURL=MenuItem.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/service/menu-service/MenuItem");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/App", "@web-atoms/core/dist/core/AtomList", "@web-atoms/core/dist/core/BindableProperty", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/di/RegisterSingleton", "@web-atoms/core/dist/services/NavigationService", "./MenuItem"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const App_1 = require("@web-atoms/core/dist/App");
    const AtomList_1 = require("@web-atoms/core/dist/core/AtomList");
    const BindableProperty_1 = require("@web-atoms/core/dist/core/BindableProperty");
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const RegisterSingleton_1 = require("@web-atoms/core/dist/di/RegisterSingleton");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const MenuItem_1 = require("./MenuItem");
    let MenuService = class MenuService {
        constructor() {
            this.menus = new AtomList_1.AtomList();
            // public addSamples(
            //     require: any,
            //     label: string,
            //     samples: ISample[],
            //     icon?: string
            // ) {
            //     const g = this.addGroup(label, icon, require);
            //     for (const iterator of samples) {
            //         g.addSample(iterator.label, iterator.demo, iterator.files, iterator.designMode);
            //     }
            // }
        }
        get groupedMenus() {
            const a = [];
            for (const iterator of this.menus) {
                const g = iterator.children;
                g.key = iterator;
                a.push(g);
            }
            return a;
        }
        add(label, action, icon) {
            const m = this.create(label, action, icon);
            this.menus.add(m);
            return m;
        }
        addGroup(label, icon, require) {
            const m = this.createGroup(label, icon, require);
            this.menus.add(m);
            return m;
        }
        addLink(label, pageSrc, pageParameters, icon) {
            const m = this.createLink(label, pageSrc, pageParameters, icon);
            this.menus.add(m);
            return m;
        }
        createLink(label, pageSrc, pageParameters, icon, options) {
            const nav = this.app.resolve(NavigationService_1.NavigationService);
            const p = pageParameters || {};
            p.title = p.title || label;
            const m = this.create(label, () => nav.openPage(pageSrc, p, Object.assign(Object.assign({}, options), { clearHistory: true })), icon);
            return m;
        }
        createGroup(label, icon, require) {
            return this.create(label, (m) => m.expand = !m.expand, icon, require);
        }
        create(label, action, icon, require) {
            const menu = new MenuItem_1.default(this.app, this);
            menu.label = label;
            menu.action = action;
            if (menu.icon) {
                menu.icon = icon;
            }
            menu.require = require;
            return menu;
        }
    };
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", App_1.App)
    ], MenuService.prototype, "app", void 0);
    __decorate([
        BindableProperty_1.BindableProperty,
        __metadata("design:type", AtomList_1.AtomList)
    ], MenuService.prototype, "menus", void 0);
    MenuService = __decorate([
        RegisterSingleton_1.RegisterSingleton
    ], MenuService);
    exports.default = MenuService;
});
//# sourceMappingURL=MenuService.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/service/menu-service/MenuService");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/view-model/Load", "../../service/menu-service/MenuService", "@web-atoms/core/dist/view-model/Action"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const Load_1 = require("@web-atoms/core/dist/view-model/Load");
    const MenuService_1 = require("../../service/menu-service/MenuService");
    const Action_1 = require("@web-atoms/core/dist/view-model/Action");
    class HomeViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.menus = null;
        }
        load() {
            return __awaiter(this, void 0, void 0, function* () {
                const search = (this.search || "").toLowerCase();
                const filter = search
                    ? (item) => item.label.toLowerCase().indexOf(search) !== -1
                    : null;
                const r = [];
                for (const iterator of this.menuService.menus) {
                    if (iterator.label === "Home") {
                        continue;
                    }
                    const g = [];
                    for (const child of iterator.children) {
                        if (filter) {
                            if ((filter(iterator) || filter(child))) {
                                g.push(child);
                            }
                            continue;
                        }
                        g.push(child);
                    }
                    if (g.length) {
                        g.key = iterator;
                        r.push(g);
                    }
                }
                this.menus = r;
            });
        }
        scan() {
            return __awaiter(this, void 0, void 0, function* () {
                if (bridge.qRCodeService) {
                    yield bridge.qRCodeService.scanAsync();
                }
                else {
                    throw new Error("Please update your app");
                }
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MenuService_1.default)
    ], HomeViewModel.prototype, "menuService", void 0);
    __decorate([
        Load_1.default({ init: true, watch: true }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], HomeViewModel.prototype, "load", null);
    __decorate([
        Action_1.default(),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], HomeViewModel.prototype, "scan", null);
    exports.default = HomeViewModel;
});
//# sourceMappingURL=HomeViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/app-host/home/HomeViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/font-awesome/dist/FontAwesomeSolid", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFContentPage", "./HomeViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const FontAwesomeSolid_1 = require("@web-atoms/font-awesome/dist/FontAwesomeSolid");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFContentPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFContentPage");
    const HomeViewModel_1 = require("./HomeViewModel");
    const BindMenu = Bind_1.default.forData();
    class Home extends AtomXFContentPage_1.default {
        create() {
            this.viewModel = this.resolve(HomeViewModel_1.default);
            this.render(XNode_1.default.create(XF_1.default.ContentPage, { title: "Web Atoms Demo" },
                XNode_1.default.create(XF_1.default.ContentPage.toolbarItems, null,
                    XNode_1.default.create(XF_1.default.ToolbarItem, { command: Bind_1.default.event(() => this.viewModel.scan()) },
                        XNode_1.default.create(XF_1.default.ToolbarItem.iconImageSource, null,
                            XNode_1.default.create(XF_1.default.FontImageSource, { size: 25, fontFamily: FontAwesomeSolid_1.default, glyph: FontAwesomeSolid_1.default.qrcode })))),
                XNode_1.default.create(XF_1.default.Grid, null,
                    XNode_1.default.create(XF_1.default.Grid.rowDefinitions, null,
                        XNode_1.default.create(XF_1.default.RowDefinition, { height: "auto" }),
                        XNode_1.default.create(XF_1.default.RowDefinition, null)),
                    XNode_1.default.create(XF_1.default.SearchBar, { placeholder: "Search samples", text: Bind_1.default.twoWays(() => this.viewModel.search) }),
                    XNode_1.default.create(XF_1.default.ListView, Object.assign({}, XF_1.default.Grid.row(1), { isGroupingEnabled: true, cachingStrategy: "RecycleElement" }, WA_1.default.GroupBy.itemsSource(Bind_1.default.oneWay(() => this.viewModel.menus))),
                        XNode_1.default.create(XF_1.default.ListView.groupHeaderTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.ViewCell, null,
                                    XNode_1.default.create(XF_1.default.Grid, { backgroundColor: "#ccc" },
                                        XNode_1.default.create(XF_1.default.Label, { textColor: "#2e2e2e", margin: "10", verticalOptions: "Center", text: BindMenu.oneWay((x) => x.data.label) }))))),
                        XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                            XNode_1.default.create(XF_1.default.DataTemplate, null,
                                XNode_1.default.create(XF_1.default.ViewCell, null,
                                    XNode_1.default.create(XF_1.default.Label, { text: BindMenu.oneWay((x) => x.data.label), padding: "10", verticalOptions: "Center" },
                                        XNode_1.default.create(XF_1.default.Label.gestureRecognizers, null,
                                            XNode_1.default.create(XF_1.default.TapGestureRecognizer, { command: BindMenu.event((s) => s.data.click(true)) }))))))))));
        }
    }
    exports.default = Home;
});
//# sourceMappingURL=Home.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/app-host/home/Home");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomLoader", "@web-atoms/core/dist/di/DISingleton", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/view-model/AtomWindowViewModel", "@web-atoms/core/dist/view-model/Load", "../samples/alert/AlertSamplePage", "../samples/box/BoxViewSample", "../samples/brushes/addBrushSamples", "../samples/calendar/calendarSamples", "../samples/carousel/carousel-page/CarouselPageSample", "../samples/carousel/carousel-view/CarouselSample", "../samples/collection-view/CollectionViewSamplePage", "../samples/database/addDatabaseSamples", "../samples/form/FormSamples", "../samples/image/ImageSample", "../samples/layout/multiple-content/LayoutSample", "../samples/layout/single-content/Sample", "../samples/list/ListSamples", "../samples/menu-item/MenuSample", "../samples/popup/PopupSample", "../samples/refresh-view/RefreshViewSample", "../samples/switch/SwitchSamplePage", "../samples/tabbed-page/TabbedPageSample", "../samples/table-view/TableViewSamplePage", "../samples/time-picker/TimePickerSamplePage", "../samples/toggle-button-bar/addToggleButtonBar", "../samples/toolbar-item/ToolbarItemSample", "../samples/web-view/WebViewSample", "../service/menu-service/MenuService", "./home/Home"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomLoader_1 = require("@web-atoms/core/dist/core/AtomLoader");
    const DISingleton_1 = require("@web-atoms/core/dist/di/DISingleton");
    const Inject_1 = require("@web-atoms/core/dist/di/Inject");
    const NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    const AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    const AtomWindowViewModel_1 = require("@web-atoms/core/dist/view-model/AtomWindowViewModel");
    const Load_1 = require("@web-atoms/core/dist/view-model/Load");
    const AlertSamplePage_1 = require("../samples/alert/AlertSamplePage");
    const BoxViewSample_1 = require("../samples/box/BoxViewSample");
    const addBrushSamples_1 = require("../samples/brushes/addBrushSamples");
    const calendarSamples_1 = require("../samples/calendar/calendarSamples");
    const CarouselPageSample_1 = require("../samples/carousel/carousel-page/CarouselPageSample");
    const CarouselSample_1 = require("../samples/carousel/carousel-view/CarouselSample");
    const CollectionViewSamplePage_1 = require("../samples/collection-view/CollectionViewSamplePage");
    const addDatabaseSamples_1 = require("../samples/database/addDatabaseSamples");
    const FormSamples_1 = require("../samples/form/FormSamples");
    const ImageSample_1 = require("../samples/image/ImageSample");
    const LayoutSample_1 = require("../samples/layout/multiple-content/LayoutSample");
    const Sample_1 = require("../samples/layout/single-content/Sample");
    const ListSamples_1 = require("../samples/list/ListSamples");
    const MenuSample_1 = require("../samples/menu-item/MenuSample");
    const PopupSample_1 = require("../samples/popup/PopupSample");
    const RefreshViewSample_1 = require("../samples/refresh-view/RefreshViewSample");
    const SwitchSamplePage_1 = require("../samples/switch/SwitchSamplePage");
    const TabbedPageSample_1 = require("../samples/tabbed-page/TabbedPageSample");
    const TableViewSamplePage_1 = require("../samples/table-view/TableViewSamplePage");
    const TimePickerSamplePage_1 = require("../samples/time-picker/TimePickerSamplePage");
    const addToggleButtonBar_1 = require("../samples/toggle-button-bar/addToggleButtonBar");
    const ToolbarItemSample_1 = require("../samples/toolbar-item/ToolbarItemSample");
    const WebViewSample_1 = require("../samples/web-view/WebViewSample");
    const MenuService_1 = require("../service/menu-service/MenuService");
    const Home_1 = require("./home/Home");
    let CLRNavigationService = class CLRNavigationService {
        pushPageAsync() {
            return __awaiter(this, void 0, void 0, function* () {
                return null;
            });
        }
    };
    CLRNavigationService = __decorate([
        DISingleton_1.default({ globalVar: "bridge.navigationService" })
    ], CLRNavigationService);
    class AppHostViewModel extends AtomViewModel_1.AtomViewModel {
        constructor() {
            super(...arguments);
            this.message = "Tap on Hamburger to continue";
        }
        load() {
            return __awaiter(this, void 0, void 0, function* () {
                this.registerDisposable(this.navigationService.registerNavigationHook((url, options) => {
                    if (!options || !options.target || (options.target && options.target !== "app")) {
                        return;
                    }
                    return this.openPage(url, options);
                }));
                const homeGroup = this.menuService.addGroup("Home");
                homeGroup.addTabLink("Home", Home_1.default);
                FormSamples_1.default(this.menuService);
                addDatabaseSamples_1.default(this.menuService);
                calendarSamples_1.default(this.menuService);
                addBrushSamples_1.default(this.menuService);
                Sample_1.default(this.menuService);
                LayoutSample_1.default(this.menuService);
                ListSamples_1.default(this.menuService);
                RefreshViewSample_1.default(this.menuService);
                CarouselPageSample_1.default(this.menuService);
                TabbedPageSample_1.default(this.menuService);
                ImageSample_1.default(this.menuService);
                MenuSample_1.default(this.menuService);
                CarouselSample_1.default(this.menuService);
                BoxViewSample_1.default(this.menuService);
                addToggleButtonBar_1.default(this.menuService);
                SwitchSamplePage_1.default(this.menuService);
                TimePickerSamplePage_1.default(this.menuService);
                AlertSamplePage_1.default(this.menuService);
                CollectionViewSamplePage_1.default(this.menuService);
                ToolbarItemSample_1.default(this.menuService);
                TableViewSamplePage_1.default(this.menuService);
                WebViewSample_1.default(this.menuService);
                PopupSample_1.default(this.menuService);
            });
        }
        openPage(url, options) {
            return __awaiter(this, void 0, void 0, function* () {
                const { view, disposables } = yield AtomLoader_1.AtomLoader.loadView(url, this.app, true, () => new AtomWindowViewModel_1.AtomWindowViewModel(this.app));
                const urlString = url.toString();
                view._$_url = urlString;
                this.currentPage = view;
                yield bridge.navigationService.pushPageAsync(this.owner.element, view.element, options.clearHistory || false);
                disposables.add(() => {
                    if (this.currentPage === view) {
                        this.currentPage = null;
                    }
                });
                return view;
            });
        }
    }
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", MenuService_1.default)
    ], AppHostViewModel.prototype, "menuService", void 0);
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", NavigationService_1.NavigationService)
    ], AppHostViewModel.prototype, "navigationService", void 0);
    __decorate([
        Inject_1.Inject,
        __metadata("design:type", CLRNavigationService)
    ], AppHostViewModel.prototype, "clrNavigation", void 0);
    __decorate([
        Load_1.default({ init: true }),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", Promise)
    ], AppHostViewModel.prototype, "load", null);
    exports.default = AppHostViewModel;
});
//# sourceMappingURL=AppHostViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/app-host/AppHostViewModel");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/font-awesome/dist/FontAwesomeSolid", "@web-atoms/xf-controls/dist/clr/WA", "@web-atoms/xf-controls/dist/clr/X", "@web-atoms/xf-controls/dist/clr/XF", "@web-atoms/xf-controls/dist/pages/AtomXFMasterDetailPage", "./AppHostViewModel", "./home/Home", "@web-atoms/core/dist/core/Colors"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const Bind_1 = require("@web-atoms/core/dist/core/Bind");
    const XNode_1 = require("@web-atoms/core/dist/core/XNode");
    const FontAwesomeSolid_1 = require("@web-atoms/font-awesome/dist/FontAwesomeSolid");
    const WA_1 = require("@web-atoms/xf-controls/dist/clr/WA");
    const X_1 = require("@web-atoms/xf-controls/dist/clr/X");
    const XF_1 = require("@web-atoms/xf-controls/dist/clr/XF");
    const AtomXFMasterDetailPage_1 = require("@web-atoms/xf-controls/dist/pages/AtomXFMasterDetailPage");
    const AppHostViewModel_1 = require("./AppHostViewModel");
    const Home_1 = require("./home/Home");
    const Colors_1 = require("@web-atoms/core/dist/core/Colors");
    class AppHost extends AtomXFMasterDetailPage_1.default {
        create() {
            this.viewModel = this.resolve(AppHostViewModel_1.default);
            this.viewModel.owner = this;
            // tslint:disable-next-line: no-console
            console.log(`Render start`);
            this.render(XNode_1.default.create(XF_1.default.MasterDetailPage, { isPresented: Bind_1.default.twoWays(() => this.viewModel.menuService.isOpen), title: "Demo 1" },
                XNode_1.default.create(XF_1.default.MasterDetailPage.master, null,
                    XNode_1.default.create(XF_1.default.ContentPage, { title: "Home" },
                        XNode_1.default.create(XF_1.default.ContentPage.iconImageSource, null,
                            XNode_1.default.create(XF_1.default.FontImageSource, { size: 25, color: Colors_1.default.darkOrange, fontFamily: FontAwesomeSolid_1.default, glyph: FontAwesomeSolid_1.default.home })),
                        XNode_1.default.create(XF_1.default.ListView, Object.assign({ cachingStrategy: "RecycleElement", isGroupingEnabled: true, rowHeight: 50, margin: 0 }, WA_1.default.GroupBy.itemsSource(Bind_1.default.oneWay(() => this.viewModel.menuService.groupedMenus))),
                            XNode_1.default.create(XF_1.default.ListView.groupHeaderTemplate, null,
                                XNode_1.default.create(XF_1.default.DataTemplate, null,
                                    XNode_1.default.create(XF_1.default.ViewCell, null,
                                        XNode_1.default.create(XF_1.default.Grid, { backgroundColor: "#ccc" },
                                            XNode_1.default.create(XF_1.default.Label, { textColor: "#2e2e2e", margin: "10", verticalOptions: "Center", text: Bind_1.default.oneWay((x) => x.data.label) }))))),
                            XNode_1.default.create(XF_1.default.ListView.itemTemplate, null,
                                XNode_1.default.create(XF_1.default.DataTemplate, null,
                                    XNode_1.default.create(XF_1.default.ViewCell, null,
                                        XNode_1.default.create(XF_1.default.Label, { text: Bind_1.default.oneWay((x) => x.data.label), padding: "10", verticalOptions: "Center" },
                                            XNode_1.default.create(XF_1.default.Label.gestureRecognizers, null,
                                                XNode_1.default.create(XF_1.default.TapGestureRecognizer, { command: Bind_1.default.event((s) => s.data.click()) }))))))))),
                XNode_1.default.create(XF_1.default.MasterDetailPage.detail, null,
                    XNode_1.default.create(XF_1.default.NavigationPage, null,
                        XNode_1.default.create(X_1.default.Arguments, null,
                            XNode_1.default.create(Home_1.default, null))))));
        }
    }
    exports.default = AppHost;
});
//# sourceMappingURL=AppHost.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/app-host/AppHost");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./app-host/AppHost"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AppHost_1 = require("./app-host/AppHost");
    // @web-atoms-pack: true
    exports.default = AppHost_1.default;
});
//# sourceMappingURL=Index.js.map

    AmdLoader.instance.setup("@web-atoms/xf-samples/dist/Index");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/AtomBridge", "../../di/RegisterSingleton", "../../services/BusyIndicatorService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const AtomBridge_1 = require("../../core/AtomBridge");
    const RegisterSingleton_1 = require("../../di/RegisterSingleton");
    const BusyIndicatorService_1 = require("../../services/BusyIndicatorService");
    let XFBusyIndicatorService = class XFBusyIndicatorService extends BusyIndicatorService_1.BusyIndicatorService {
        createIndicator() {
            const popup = AtomBridge_1.AtomBridge.instance.createBusyIndicator();
            return {
                dispose: () => {
                    popup.dispose();
                }
            };
        }
    };
    XFBusyIndicatorService = __decorate([
        RegisterSingleton_1.RegisterSingleton
    ], XFBusyIndicatorService);
    exports.default = XFBusyIndicatorService;
});
//# sourceMappingURL=XFBusyIndicatorService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/xf/services/XFBusyIndicatorService");

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../App", "../../core/AtomBridge", "../../core/AtomLoader", "../../core/AtomUri", "../../di/Inject", "../../di/RegisterSingleton", "../../services/JsonService", "../../services/NavigationService", "../../view-model/AtomWindowViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const App_1 = require("../../App");
    const AtomBridge_1 = require("../../core/AtomBridge");
    const AtomLoader_1 = require("../../core/AtomLoader");
    const AtomUri_1 = require("../../core/AtomUri");
    const Inject_1 = require("../../di/Inject");
    const RegisterSingleton_1 = require("../../di/RegisterSingleton");
    const JsonService_1 = require("../../services/JsonService");
    const NavigationService_1 = require("../../services/NavigationService");
    const AtomWindowViewModel_1 = require("../../view-model/AtomWindowViewModel");
    let XFNavigationService = class XFNavigationService extends NavigationService_1.NavigationService {
        constructor(app, jsonService) {
            super(app);
            this.jsonService = jsonService;
            this.stack = [];
        }
        get title() {
            // return bridge.getTitle();
            // throw new Error("Not supported");
            return undefined;
        }
        set title(v) {
            // bridge.setTitle(v);
            // throw new Error("Not supported");
        }
        // private mLocation: ILocation;
        get location() {
            return new AtomUri_1.AtomUri(bridge.navigationService.getLocation());
        }
        set location(v) {
            bridge.navigationService.setLocation(v.toString());
        }
        alert(message, title) {
            if (typeof message !== "string") {
                message = message.toString();
            }
            return new Promise((resolve, reject) => {
                bridge.alert(message, title, () => {
                    resolve();
                }, (f) => {
                    reject(f);
                });
            });
        }
        confirm(message, title) {
            return new Promise((resolve, reject) => {
                bridge.confirm(message, title, (r) => {
                    resolve(r);
                }, (f) => {
                    reject(f);
                });
            });
        }
        notify(message, title, type, delay) {
            // display toast pending..
            // tslint:disable-next-line: no-console
            console.warn("Display toast not yet implemented");
        }
        navigate(url) {
            const uri = new AtomUri_1.AtomUri(url);
            this.stack.push(url);
            this.app.runAsync(() => __awaiter(this, void 0, void 0, function* () {
                const { view: popup } = yield AtomLoader_1.AtomLoader.loadView(uri, this.app, true);
                bridge.setRoot(popup.element);
            }));
        }
        back() {
            if (this.stack.length) {
                const url = this.stack.pop();
                this.app.runAsync(() => __awaiter(this, void 0, void 0, function* () {
                    const uri = new AtomUri_1.AtomUri(url);
                    const { view: popup } = yield AtomLoader_1.AtomLoader.loadView(uri, this.app, true);
                    bridge.setRoot(popup.element);
                }));
            }
        }
        refresh() {
            AtomBridge_1.AtomBridge.instance.reset();
        }
        openWindow(url, options) {
            return __awaiter(this, void 0, void 0, function* () {
                const { view: popup, disposables, returnPromise, id } = yield AtomLoader_1.AtomLoader.loadView(url, this.app, true, () => this.app.resolve(AtomWindowViewModel_1.AtomWindowViewModel, true));
                if (options && options.onInit) {
                    options.onInit(popup);
                }
                const cancelToken = options.cancelToken;
                if (cancelToken) {
                    if (cancelToken.cancelled) {
                        this.app.callLater(() => {
                            this.remove(popup, true);
                        });
                    }
                    cancelToken.registerForCancel(() => {
                        this.remove(popup, true);
                    });
                }
                const ve = popup.element;
                AtomBridge_1.AtomBridge.instance.setValue(ve, "name", id);
                bridge.pushPage(ve, options || {}, () => {
                    // reject("cancelled");
                    // do nothing...
                }, (e) => {
                    this.remove(popup, true);
                });
                disposables.add(() => {
                    AtomBridge_1.AtomBridge.instance.popPage(ve, () => {
                        // do nothing
                    }, (e) => {
                        // tslint:disable-next-line: no-console
                        console.error(e);
                    });
                });
                return returnPromise;
            });
        }
    };
    XFNavigationService = __decorate([
        RegisterSingleton_1.RegisterSingleton,
        __param(0, Inject_1.Inject),
        __param(1, Inject_1.Inject),
        __metadata("design:paramtypes", [App_1.App,
            JsonService_1.JsonService])
    ], XFNavigationService);
    exports.default = XFNavigationService;
});
//# sourceMappingURL=XFNavigationService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/xf/services/XFNavigationService");

(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../App", "../core/AtomBridge", "../services/BusyIndicatorService", "../services/NavigationService", "../web/styles/AtomStyleSheet", "./services/XFBusyIndicatorService", "./services/XFNavigationService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    const A = require("../App");
    const AtomBridge_1 = require("../core/AtomBridge");
    const BusyIndicatorService_1 = require("../services/BusyIndicatorService");
    const NavigationService_1 = require("../services/NavigationService");
    const AtomStyleSheet_1 = require("../web/styles/AtomStyleSheet");
    const XFBusyIndicatorService_1 = require("./services/XFBusyIndicatorService");
    const XFNavigationService_1 = require("./services/XFNavigationService");
    class XFApp extends A.App {
        constructor() {
            super();
            this.mLastStyle = null;
            AtomBridge_1.AtomBridge.instance = bridge;
            this.put(NavigationService_1.NavigationService, this.resolve(XFNavigationService_1.default));
            this.put(BusyIndicatorService_1.BusyIndicatorService, this.resolve(XFBusyIndicatorService_1.default));
            this.put(AtomStyleSheet_1.AtomStyleSheet, new AtomStyleSheet_1.AtomStyleSheet(this, "WA_"));
            const s = bridge.subscribe((channel, data) => {
                this.broadcast(channel, data);
            });
            // register for messaging...
            const oldDispose = this.dispose;
            this.dispose = () => {
                s.dispose();
                oldDispose.call(this);
            };
        }
        get root() {
            return this.mRoot;
        }
        set root(v) {
            this.mRoot = v;
            bridge.setRoot(v.element);
        }
        updateDefaultStyle(textContent) {
            if (!textContent) {
                return;
            }
            if (this.mLastStyle && this.mLastStyle === textContent) {
                return;
            }
            this.mLastStyle = textContent;
            bridge.updateDefaultStyle(textContent);
        }
        broadcast(channel, data) {
            super.broadcast(channel, data);
            bridge.broadcast(channel, data);
        }
    }
    exports.default = XFApp;
});
//# sourceMappingURL=XFApp.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/xf/XFApp");

        //# sourceMappingURL=Index.pack.js.map
        