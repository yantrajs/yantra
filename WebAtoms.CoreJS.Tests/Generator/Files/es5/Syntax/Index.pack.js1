var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
// @ts-ignore
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory() :
        typeof define === 'function' && define.amd ? define(factory) :
            (factory());
}(this, (function () {
    'use strict';
    /**
     * @this {Promise}
     */
    function finallyConstructor(callback) {
        var constructor = this.constructor;
        return this.then(function (value) {
            // @ts-ignore
            return constructor.resolve(callback()).then(function () {
                return value;
            });
        }, function (reason) {
            // @ts-ignore
            return constructor.resolve(callback()).then(function () {
                // @ts-ignore
                return constructor.reject(reason);
            });
        });
    }
    // Store setTimeout reference so promise-polyfill will be unaffected by
    // other code modifying setTimeout (like sinon.useFakeTimers())
    var setTimeoutFunc = setTimeout;
    function isArray(x) {
        return Boolean(x && typeof x.length !== 'undefined');
    }
    function noop() { }
    // Polyfill for Function.prototype.bind
    function bind(fn, thisArg) {
        return function () {
            fn.apply(thisArg, arguments);
        };
    }
    /**
     * @constructor
     * @param {Function} fn
     */
    function Promise(fn) {
        if (!(this instanceof Promise))
            throw new TypeError('Promises must be constructed via new');
        if (typeof fn !== 'function')
            throw new TypeError('not a function');
        /** @type {!number} */
        this._state = 0;
        /** @type {!boolean} */
        this._handled = false;
        /** @type {Promise|undefined} */
        this._value = undefined;
        /** @type {!Array<!Function>} */
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
            // Promise Resolution Procedure: https://github.com/promises-aplus/promises-spec#the-promise-resolution-procedure
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
    /**
     * @constructor
     */
    function Handler(onFulfilled, onRejected, promise) {
        this.onFulfilled = typeof onFulfilled === 'function' ? onFulfilled : null;
        this.onRejected = typeof onRejected === 'function' ? onRejected : null;
        this.promise = promise;
    }
    /**
     * Take a potentially misbehaving resolver function and make sure
     * onFulfilled and onRejected are only called once.
     *
     * Makes no guarantees about asynchrony.
     */
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
        // @ts-ignore
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
    // Use polyfill for setImmediate for performance gains
    Promise._immediateFn =
        // @ts-ignore
        (typeof setImmediate === 'function' &&
            function (fn) {
                // @ts-ignore
                setImmediate(fn);
            }) ||
            function (fn) {
                setTimeoutFunc(fn, 0);
            };
    Promise._unhandledRejectionFn = function _unhandledRejectionFn(err) {
        if (typeof console !== 'undefined' && console) {
            console.warn('Possible Unhandled Promise Rejection:', err); // eslint-disable-line no-console
        }
    };
    /** @suppress {undefinedVars} */
    var globalNS = (function () {
        // the only reliable means to get the global object is
        // `Function('return this')()`
        // However, this causes CSP violations in Chrome apps.
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
// tslint:disable
// @ts-ignore
var Reflect;
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
//# sourceMappingURL=/sm/d3ba45b98814b993d6aab5ce31c491c459ff57773960f74725c98a635912413a.map
if (!Array.prototype.find) {
    Array.prototype.find = function (predicate, thisArg) {
        for (var i = 0; i < this.length; i++) {
            var item = this[i];
            if (predicate(item, i, this)) {
                return item;
            }
        }
    };
}
if (!Array.prototype.findIndex) {
    Array.prototype.findIndex = function (predicate, thisArg) {
        for (var i = 0; i < this.length; i++) {
            var item = this[i];
            if (predicate(item, i, this)) {
                return i;
            }
        }
        return -1;
    };
}
if (!Array.prototype.map) {
    Array.prototype.map = function (callbackfn, thisArg) {
        var a = [];
        for (var i = 0; i < this.length; i++) {
            var r = callbackfn(this[i], i, this);
            if (r !== undefined) {
                a.push(r);
            }
        }
        return a;
    };
}
if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, endPosition) {
        var index = this.indexOf(searchString, endPosition);
        return index === 0;
    };
}
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, endPosition) {
        var index = this.lastIndexOf(searchString, endPosition);
        if (index === -1) {
            return false;
        }
        var l = this.length - index;
        return l === searchString.length;
    };
}
if (!Number.parseInt) {
    // tslint:disable-next-line:only-arrow-functions
    Number.parseInt = function (n) {
        return Math.floor(parseFloat(n));
    };
}
if (!Number.parseFloat) {
    // tslint:disable-next-line:only-arrow-functions
    Number.parseFloat = function (n) {
        return parseFloat(n);
    };
}
if (typeof Element !== "undefined") {
    if (!("remove" in Element.prototype)) {
        // tslint:disable-next-line
        Element.prototype["remove"] = function () {
            if (this.parentNode) {
                this.parentNode.removeChild(this);
            }
        };
    }
}
var Module = /** @class */ (function () {
    function Module(name, folder) {
        this.name = name;
        this.folder = folder;
        this.emptyExports = {};
        this.ignoreModule = null;
        this.isLoaded = false;
        this.isResolved = false;
        this.dependencies = [];
        this.rID = null;
        var index = name.lastIndexOf("/");
        if (index === -1) {
            this.folder = "";
        }
        else {
            this.folder = name.substr(0, index);
        }
    }
    Object.defineProperty(Module.prototype, "filename", {
        get: function () {
            return this.name;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(Module.prototype, "dependents", {
        get: function () {
            var d = [];
            var v = {};
            var modules = AmdLoader.instance.modules;
            for (var key in modules) {
                if (modules.hasOwnProperty(key)) {
                    var element = modules[key];
                    if (element.isDependentOn(this, v)) {
                        d.push(element);
                    }
                }
            }
            return d;
        },
        enumerable: false,
        configurable: true
    });
    Module.prototype.resolve = function (id) {
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
            // circular dependency found...
            var childrenResolved = true;
            for (var _i = 0, _a = this.dependencies; _i < _a.length; _i++) {
                var iterator = _a[_i];
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
        var allResolved = true;
        for (var _b = 0, _c = this.dependencies; _b < _c.length; _b++) {
            var iterator = _c[_b];
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
        var i = AmdLoader.instance;
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
    };
    Module.prototype.addDependency = function (d) {
        // ignore module contains dependency resolution module
        if (d === this.ignoreModule) {
            return;
        }
        // if (d.isDependentOn(this)) {
        //     return;
        // }
        this.dependencies.push(d);
    };
    Module.prototype.getExports = function () {
        this.exports = this.emptyExports;
        if (this.factory) {
            try {
                var factory = this.factory;
                this.factory = null;
                delete this.factory;
                AmdLoader.instance.currentStack.push(this);
                var result = factory(this.require, this.exports);
                if (result) {
                    if (typeof result === "object") {
                        for (var key in result) {
                            if (result.hasOwnProperty(key)) {
                                var element = result[key];
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
                if (this.exports.default) {
                    this.exports.default[UMD.nameSymbol] = this.name;
                }
            }
            catch (e) {
                var em = e.stack ? (e + "\n" + e.stack) : e;
                var s = [];
                // modules in the stack...
                for (var _i = 0, _a = AmdLoader.instance.currentStack; _i < _a.length; _i++) {
                    var iterator = _a[_i];
                    s.push(iterator.name);
                }
                var ne = new Error("Failed loading module " + this.name + " with error " + em + "\nDependents: " + s.join("\n\t"));
                // tslint:disable-next-line: no-console
                console.error(ne);
                throw ne;
            }
        }
        return this.exports;
    };
    /**
     * Displays list of all dependents (including nested)
     */
    Module.prototype.isDependentOn = function (m, visited) {
        visited[this.name] = true;
        for (var _i = 0, _a = this.dependencies; _i < _a.length; _i++) {
            var iterator = _a[_i];
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
    };
    Module.nextID = 1;
    return Module;
}());
/// <reference path="./Promise.ts"/>
/// <reference path="./ReflectMetadata.ts"/>
/// <reference path="./ArrayHelper.ts"/>
/// <reference path="./Module.ts"/>
if (typeof require !== "undefined") {
    md = require("module").Module;
}
var AmdLoader = /** @class */ (function () {
    function AmdLoader() {
        this.root = null;
        this.defaultUrl = null;
        this.currentStack = [];
        // public pendingModules: Module[] = [];
        // public resolverStack: Module[] = [];
        // only useful in node environment
        this.nodeModules = [];
        this.modules = {};
        this.pathMap = {};
        this.mockTypes = [];
        this.lastTimeout = null;
        this.dirty = false;
    }
    AmdLoader.prototype.register = function (packages, modules) {
        for (var _i = 0, packages_1 = packages; _i < packages_1.length; _i++) {
            var iterator = packages_1[_i];
            if (!this.pathMap[iterator]) {
                this.map(iterator, "/");
            }
        }
        for (var _a = 0, modules_1 = modules; _a < modules_1.length; _a++) {
            var iterator = modules_1[_a];
            this.get(iterator);
        }
    };
    AmdLoader.prototype.setupRoot = function (root, url) {
        if (url.endsWith("/")) {
            url = url.substr(0, url.length - 1);
        }
        for (var key in this.pathMap) {
            if (this.pathMap.hasOwnProperty(key)) {
                var moduleUrl = key === root ? url : url + "/node_modules/" + key;
                this.map(key, moduleUrl);
            }
        }
        this.defaultUrl = url + "/node_modules/";
    };
    AmdLoader.prototype.registerModule = function (name, moduleExports) {
        var m = this.get(name);
        m.package.url = "/";
        m.exports = __assign({ __esModule: true }, moduleExports);
        m.loader = Promise.resolve();
        m.resolver = Promise.resolve(m.exports);
        m.isLoaded = true;
        m.isResolved = true;
    };
    AmdLoader.prototype.setup = function (name) {
        var _this = this;
        var jsModule = this.get(name);
        // tslint:disable-next-line:ban-types
        var define = this.define;
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
        setTimeout(function () {
            _this.loadDependencies(jsModule);
        }, 1);
    };
    AmdLoader.prototype.loadDependencies = function (m) {
        var _this = this;
        this.resolveModule(m).catch(function (e) {
            // tslint:disable-next-line:no-console
            console.error(e);
        });
        if (m.dependencies.length) {
            var all = m.dependencies.map(function (m1) { return __awaiter(_this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (m1.isResolved) {
                                return [2 /*return*/];
                            }
                            return [4 /*yield*/, this.import(m1)];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            }); });
            Promise.all(all).catch(function (e) {
                // tslint:disable-next-line:no-console
                console.error(e);
            }).then(function () {
                m.resolve();
            });
        }
        else {
            m.resolve();
        }
        this.queueResolveModules(1);
    };
    AmdLoader.prototype.replace = function (type, name, mock) {
        if (mock && !this.enableMock) {
            return;
        }
        var peek = this.currentStack.length ? this.currentStack[this.currentStack.length - 1] : undefined;
        var rt = new MockType(peek, type, name, mock);
        this.mockTypes.push(rt);
    };
    AmdLoader.prototype.resolveType = function (type) {
        var t = this.mockTypes.find(function (tx) { return tx.type === type; });
        return t ? t.replaced : type;
    };
    AmdLoader.prototype.map = function (packageName, packageUrl, type, exportVar) {
        if (type === void 0) { type = "amd"; }
        // ignore map if it exists already...
        var existing = this.pathMap[packageName];
        if (existing) {
            existing.url = packageUrl;
            existing.exportVar = exportVar;
            existing.type = type;
            return existing;
        }
        existing = {
            name: packageName,
            url: packageUrl,
            type: type,
            exportVar: exportVar,
            version: ""
        };
        if (packageName === "reflect-metadata") {
            type = "global";
        }
        this.pathMap[packageName] = existing;
        return existing;
    };
    AmdLoader.prototype.resolveSource = function (name, defExt) {
        if (defExt === void 0) { defExt = ".js"; }
        try {
            if (/^((\/)|((http|https)\:\/\/))/i.test(name)) {
                // console.log(`ResolveSource fail: ${name}`);
                return name;
            }
            var path = null;
            for (var key in this.pathMap) {
                if (this.pathMap.hasOwnProperty(key)) {
                    var packageName = key;
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
                        var i = name.lastIndexOf("/");
                        var fileName = name.substr(i + 1);
                        if (fileName.indexOf(".") === -1) {
                            path = path + defExt;
                        }
                        // if (defExt && !path.endsWith(defExt)) {
                        //     path = path + defExt;
                        // }
                        return path;
                    }
                }
            }
            return name;
        }
        catch (e) {
            // tslint:disable-next-line:no-console
            console.error("Failed to resolve " + name + " with error " + JSON.stringify(e));
            // tslint:disable-next-line:no-console
            console.error(e);
        }
    };
    AmdLoader.prototype.resolveRelativePath = function (name, currentPackage) {
        if (name.charAt(0) !== ".") {
            return name;
        }
        var tokens = name.split("/");
        var currentTokens = currentPackage.split("/");
        currentTokens.pop();
        while (tokens.length) {
            var first = tokens[0];
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
        return currentTokens.join("/") + "/" + tokens.join("/");
    };
    AmdLoader.prototype.getPackageVersion = function (name) {
        var _a = name.split("/", 3), scope = _a[0], packageName = _a[1];
        var version = "";
        if (scope[0] !== "@") {
            packageName = scope;
            scope = "";
        }
        else {
            scope += "/";
        }
        var versionTokens = packageName.split("@");
        if (versionTokens.length > 1) {
            // remove version and map it..
            version = versionTokens[1];
            name = name.replace("@" + version, "");
        }
        packageName = scope + packageName;
        return { packageName: packageName, version: version, name: name };
    };
    AmdLoader.prototype.get = function (name1) {
        var _this = this;
        var module = this.modules[name1];
        if (!module) {
            // strip '@' version info
            var _a = this.getPackageVersion(name1), packageName = _a.packageName, version = _a.version, name_1 = _a.name;
            module = new Module(name_1);
            this.modules[name1] = module;
            module.package = this.pathMap[packageName] ||
                (this.pathMap[packageName] = {
                    type: "amd",
                    name: packageName,
                    version: version,
                    url: this.defaultUrl ?
                        (this.defaultUrl + packageName) : undefined
                });
            module.url = this.resolveSource(name_1);
            if (!module.url) {
                if (typeof require === "undefined") {
                    throw new Error("No url mapped for " + name_1);
                }
            }
            module.require = function (n) {
                var an = _this.resolveRelativePath(n, module.name);
                var resolvedModule = _this.get(an);
                var m = resolvedModule.getExports();
                return m;
            };
            module.require.resolve = function (n) { return _this.resolveRelativePath(n, module.name); };
            this.modules[name_1] = module;
        }
        return module;
    };
    AmdLoader.prototype.import = function (name) {
        return __awaiter(this, void 0, void 0, function () {
            var module, e;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (typeof require !== "undefined") {
                            return [2 /*return*/, Promise.resolve(require(name))];
                        }
                        module = typeof name === "object" ? name : this.get(name);
                        return [4 /*yield*/, this.load(module)];
                    case 1:
                        _a.sent();
                        return [4 /*yield*/, this.resolveModule(module)];
                    case 2:
                        e = _a.sent();
                        return [2 /*return*/, e];
                }
            });
        });
    };
    AmdLoader.prototype.load = function (module) {
        var _this = this;
        if (module.loader) {
            return module.loader;
        }
        this.push(module);
        if (AmdLoader.isJson.test(module.url)) {
            var mUrl_1 = module.package.url + module.url;
            module.loader = new Promise(function (resolve, reject) {
                try {
                    AmdLoader.httpTextLoader(mUrl_1, function (r) {
                        try {
                            module.exports = JSON.parse(r);
                            module.emptyExports = module.exports;
                            module.isLoaded = true;
                            setTimeout(function () { return _this.loadDependencies(module); }, 1);
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
            var mUrl_2 = !module.url.startsWith(module.package.url)
                ? (module.package.url + module.url)
                : module.url;
            var m = {
                url: mUrl_2,
                toString: function () { return mUrl_2; }
            };
            var e = { __esModule: true, default: m };
            module.exports = e;
            module.emptyExports = e;
            module.loader = Promise.resolve();
            module.isLoaded = true;
            return module.loader;
        }
        module.loader = new Promise(function (resolve, reject) {
            AmdLoader.moduleLoader(module.name, module.url, function () {
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
                        AmdLoader.moduleProgress(module.name, _this.modules, "loading");
                    }
                    module.isLoaded = true;
                    setTimeout(function () {
                        _this.loadDependencies(module);
                    }, 1);
                    resolve();
                }
                catch (e) {
                    // tslint:disable-next-line: no-console
                    console.error(e);
                    reject(e);
                }
            }, function (error) {
                reject(error);
            });
        });
        return module.loader;
    };
    AmdLoader.prototype.resolveModule = function (module) {
        if (module.resolver) {
            return module.resolver;
        }
        module.resolver = this._resolveModule(module);
        return module.resolver;
    };
    AmdLoader.prototype.remove = function (m) {
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
    };
    AmdLoader.prototype.queueResolveModules = function (n) {
        var _this = this;
        if (n === void 0) { n = 1; }
        if (this.lastTimeout) {
            // clearTimeout(this.lastTimeout);
            // this.lastTimeout = null;
            return;
        }
        this.lastTimeout = setTimeout(function () {
            _this.lastTimeout = 0;
            _this.resolvePendingModules();
        }, n);
    };
    AmdLoader.prototype.watch = function () {
        var _this = this;
        var id = setInterval(function () {
            if (_this.tail) {
                var list = [];
                for (var key in _this.modules) {
                    if (_this.modules.hasOwnProperty(key)) {
                        var element = _this.modules[key];
                        if (!element.isResolved) {
                            list.push({
                                name: element.name,
                                dependencies: element.dependencies.map(function (x) { return x.name; })
                            });
                        }
                    }
                }
                // tslint:disable-next-line: no-console
                console.log("Pending modules");
                // tslint:disable-next-line: no-console
                console.log(JSON.stringify(list));
                return;
            }
            clearInterval(id);
        }, 10000);
    };
    AmdLoader.prototype.resolvePendingModules = function () {
        if (!this.tail) {
            return;
        }
        this.dirty = false;
        // first resolve modules without any
        // dependencies
        var pending = [];
        var m = this.tail;
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
        for (var _i = 0, pending_1 = pending; _i < pending_1.length; _i++) {
            var iterator = pending_1[_i];
            iterator.resolve();
        }
        if (this.dirty) {
            this.dirty = false;
            return;
        }
        if (this.tail) {
            this.queueResolveModules();
        }
    };
    AmdLoader.prototype.push = function (m) {
        if (this.tail) {
            m.previous = this.tail;
            this.tail.next = m;
        }
        this.tail = m;
    };
    AmdLoader.prototype._resolveModule = function (module) {
        return __awaiter(this, void 0, void 0, function () {
            var exports, pendingList, _i, pendingList_1, iterator, tasks, setHooks;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!this.root) {
                            this.root = module;
                        }
                        return [4 /*yield*/, new Promise(function (resolve, reject) {
                                module.dependencyHooks = [resolve, reject];
                            })];
                    case 1:
                        _a.sent();
                        exports = module.getExports();
                        pendingList = this.mockTypes.filter(function (t) { return !t.loaded; });
                        if (!pendingList.length) return [3 /*break*/, 3];
                        for (_i = 0, pendingList_1 = pendingList; _i < pendingList_1.length; _i++) {
                            iterator = pendingList_1[_i];
                            iterator.loaded = true;
                        }
                        tasks = pendingList.map(function (iterator) { return __awaiter(_this, void 0, void 0, function () {
                            var containerModule, resolvedName, im, ex, type;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0:
                                        containerModule = iterator.module;
                                        resolvedName = this.resolveRelativePath(iterator.moduleName, containerModule.name);
                                        im = this.get(resolvedName);
                                        im.ignoreModule = module;
                                        return [4 /*yield*/, this.import(im)];
                                    case 1:
                                        ex = _a.sent();
                                        type = ex[iterator.exportName];
                                        iterator.replaced = type;
                                        return [2 /*return*/];
                                }
                            });
                        }); });
                        return [4 /*yield*/, Promise.all(tasks)];
                    case 2:
                        _a.sent();
                        _a.label = 3;
                    case 3:
                        setHooks = new Promise(function (resolve, reject) {
                            module.resolveHooks = [resolve, reject];
                        });
                        return [4 /*yield*/, setHooks];
                    case 4:
                        _a.sent();
                        if (this.root === module) {
                            this.root = null;
                            AmdLoader.moduleProgress(null, this.modules, "done");
                        }
                        module.isResolved = true;
                        return [2 /*return*/, exports];
                }
            });
        });
    };
    AmdLoader.isMedia = /\.(jpg|jpeg|gif|png|mp4|mp3|css|html|svg)$/i;
    AmdLoader.isJson = /\.json$/i;
    AmdLoader.globalVar = {};
    AmdLoader.instance = new AmdLoader();
    AmdLoader.current = null;
    return AmdLoader;
}());
var a = AmdLoader.instance;
a.map("global", "/", "global");
a.registerModule("global/document", { default: document });
a.registerModule("global/window", { default: typeof window !== "undefined" ? window : global });
a.map("reflect-metadata", "/", "global");
a.registerModule("reflect-metadata", Reflect);
// a.watch();
AmdLoader.moduleLoader = function (name, url, success, error) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = url;
    var s = script;
    script.onload = s.onreadystatechange = function () {
        if ((s.readyState && s.readyState !== "complete" && s.readyState !== "loaded")) {
            return;
        }
        script.onload = s.onreadystatechange = null;
        success();
    };
    script.onerror = function (e) { error(e); };
    document.body.appendChild(script);
};
AmdLoader.httpTextLoader = function (url, success, error) {
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function (e) {
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
AmdLoader.moduleProgress = (function () {
    if (!document) {
        return function (name, p) {
            // tslint:disable-next-line:no-console
            console.log(name + " " + p + "%");
        };
    }
    var progressDiv = document.createElement("div");
    progressDiv.className = "web-atoms-progress-div";
    var style = progressDiv.style;
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
    var progressLabel = document.createElement("pre");
    progressDiv.appendChild(progressLabel);
    progressLabel.style.color = "#A0A0A0";
    var ps = progressLabel.style;
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
        // tslint:disable-next-line:no-string-literal
        (document.readyState !== "loading" && !document.documentElement["doScroll"])) {
        window.setTimeout(ready);
    }
    else {
        document.addEventListener("DOMContentLoaded", completed);
        window.addEventListener("load", completed);
    }
    return function (name, n, status) {
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
/// <reference path="./AmdLoader.ts"/>
// tslint:disable-next-line:no-var-keyword
var define = function (requiresOrFactory, factory, nested) {
    var loader = AmdLoader.instance;
    function bindFactory(module, requireList, fx) {
        if (module.factory) {
            return;
        }
        module.dependencies = [];
        var requires = requireList;
        requires = requireList;
        var args = [];
        for (var _i = 0, requires_1 = requires; _i < requires_1.length; _i++) {
            var s = requires_1[_i];
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
            var name_2 = loader.resolveRelativePath(s, module.name);
            var child = loader.get(name_2);
            module.addDependency(child);
        }
        module.factory = function () {
            return fx.apply(module, args);
        };
    }
    if (nested) {
        // this means this was executed as packed modules..
        // first parameter is name, second is array and third is factory...
        var name_3 = requiresOrFactory;
        var rList = factory;
        var f = nested;
        var module_1 = AmdLoader.instance.get(name_3);
        bindFactory(module_1, rList, f);
        // we must call load modules after this..
        setTimeout(function () {
            loader.loadDependencies(module_1);
        }, 1);
        return;
    }
    AmdLoader.instance.define = function () {
        var current = AmdLoader.current;
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
// pending...
var MockType = /** @class */ (function () {
    function MockType(module, type, name, mock, moduleName, exportName) {
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
            var tokens = name.split("$");
            this.moduleName = tokens[0];
            this.exportName = tokens[1] || tokens[0].split("/").pop();
        }
        else {
            this.moduleName = name;
            this.exportName = "default";
        }
    }
    return MockType;
}());
/// <reference path="./AmdLoader.ts"/>
var UMDClass = /** @class */ (function () {
    function UMDClass() {
        this.viewPrefix = "web";
        this.defaultApp = "@web-atoms/core/dist/web/WebApp";
        this.lang = "en-US";
        this.nameSymbol = typeof Symbol !== "undefined" ? Symbol() : "_$_nameSymbol";
    }
    Object.defineProperty(UMDClass.prototype, "mock", {
        get: function () {
            return AmdLoader.instance.enableMock;
        },
        set: function (v) {
            AmdLoader.instance.enableMock = v;
        },
        enumerable: false,
        configurable: true
    });
    UMDClass.prototype.resolvePath = function (n) {
        return AmdLoader.instance.resolveSource(n, null);
    };
    UMDClass.prototype.resolveViewPath = function (path) {
        return path.replace("{platform}", this.viewPrefix);
    };
    UMDClass.prototype.resolveType = function (type) {
        return AmdLoader.instance.resolveType(type);
    };
    UMDClass.prototype.map = function (name, path, type, exportVar) {
        if (type === void 0) { type = "amd"; }
        AmdLoader.instance.map(name, path, type, exportVar);
    };
    UMDClass.prototype.setupRoot = function (name, url) {
        AmdLoader.instance.setupRoot(name, url);
    };
    UMDClass.prototype.mockType = function (type, name) {
        AmdLoader.instance.replace(type, name, true);
    };
    UMDClass.prototype.inject = function (type, name) {
        AmdLoader.instance.replace(type, name, false);
    };
    UMDClass.prototype.resolveViewClassAsync = function (path) {
        return __awaiter(this, void 0, void 0, function () {
            var e;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        path = this.resolveViewPath(path);
                        return [4 /*yield*/, AmdLoader.instance.import(path)];
                    case 1:
                        e = _a.sent();
                        return [2 /*return*/, e.default];
                }
            });
        });
    };
    UMDClass.prototype.import = function (path) {
        return AmdLoader.instance.import(path);
    };
    UMDClass.prototype.load = function (path, designMode) {
        return __awaiter(this, void 0, void 0, function () {
            var t, a, al;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.mock = designMode;
                        return [4 /*yield*/, AmdLoader.instance.import("@web-atoms/core/dist/core/types")];
                    case 1:
                        t = _a.sent();
                        return [4 /*yield*/, AmdLoader.instance.import("@web-atoms/core/dist/Atom")];
                    case 2:
                        a = _a.sent();
                        a.Atom.designMode = designMode;
                        return [4 /*yield*/, AmdLoader.instance.import("@web-atoms/core/dist/core/AtomList")];
                    case 3:
                        al = _a.sent();
                        return [4 /*yield*/, AmdLoader.instance.import(path)];
                    case 4: return [2 /*return*/, _a.sent()];
                }
            });
        });
    };
    /**
     * Host the view inside given element with given id
     * @param id id of element to host view in
     * @param path path of module
     * @param designMode true/false (default false)
     */
    UMDClass.prototype.hostView = function (id, path, designMode) {
        return __awaiter(this, void 0, void 0, function () {
            var m, app_1, e_1;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        this.mock = designMode;
                        AmdLoader.instance.get(path);
                        return [4 /*yield*/, this.load(this.defaultApp, designMode)];
                    case 1:
                        m = _a.sent();
                        app_1 = new (m.default)();
                        app_1.onReady(function () { return __awaiter(_this, void 0, void 0, function () {
                            var viewClass, view, element, e_2;
                            return __generator(this, function (_a) {
                                switch (_a.label) {
                                    case 0:
                                        _a.trys.push([0, 2, , 3]);
                                        return [4 /*yield*/, AmdLoader.instance.import(path)];
                                    case 1:
                                        viewClass = _a.sent();
                                        view = new (viewClass.default)(app_1);
                                        element = document.getElementById(id);
                                        element.appendChild(view.element);
                                        return [3 /*break*/, 3];
                                    case 2:
                                        e_2 = _a.sent();
                                        // tslint:disable-next-line:no-console
                                        console.error(e_2);
                                        return [3 /*break*/, 3];
                                    case 3: return [2 /*return*/];
                                }
                            });
                        }); });
                        return [3 /*break*/, 3];
                    case 2:
                        e_1 = _a.sent();
                        // tslint:disable-next-line:no-console
                        console.error(e_1);
                        return [3 /*break*/, 3];
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    UMDClass.prototype.loadView = function (path, designMode, appPath) {
        return __awaiter(this, void 0, void 0, function () {
            var m, app_2, er_1;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 3, , 4]);
                        this.mock = designMode;
                        appPath = appPath || this.defaultApp;
                        AmdLoader.instance.get(path);
                        return [4 /*yield*/, this.load(appPath, designMode)];
                    case 1:
                        m = _a.sent();
                        app_2 = new (m.default)();
                        return [4 /*yield*/, new Promise(function (resolve, reject) {
                                app_2.onReady(function () { return __awaiter(_this, void 0, void 0, function () {
                                    var viewClass, view, e_3;
                                    return __generator(this, function (_a) {
                                        switch (_a.label) {
                                            case 0:
                                                _a.trys.push([0, 2, , 3]);
                                                return [4 /*yield*/, AmdLoader.instance.import(path)];
                                            case 1:
                                                viewClass = _a.sent();
                                                view = new (viewClass.default)(app_2);
                                                app_2.root = view;
                                                resolve(view);
                                                return [3 /*break*/, 3];
                                            case 2:
                                                e_3 = _a.sent();
                                                // tslint:disable-next-line:no-console
                                                console.error(e_3);
                                                reject(e_3);
                                                return [3 /*break*/, 3];
                                            case 3: return [2 /*return*/];
                                        }
                                    });
                                }); });
                            })];
                    case 2: return [2 /*return*/, _a.sent()];
                    case 3:
                        er_1 = _a.sent();
                        // tslint:disable-next-line: no-console
                        console.error(er_1);
                        throw er_1;
                    case 4: return [2 /*return*/];
                }
            });
        });
    };
    return UMDClass;
}());
var UMD = new UMDClass();
(function (u) {
    var globalNS = (typeof window !== "undefined" ? window : global);
    globalNS.UMD = u;
})(UMD);
//# sourceMappingURL=umd.js.map

        AmdLoader.instance.register(
            ["@web-atoms/samples","@web-atoms/core","@web-atoms/date-time","@web-atoms/web-controls"],
            ["@web-atoms/samples/dist/web/Index","@web-atoms/core/dist/Atom","@web-atoms/core/dist/core/AtomList","@web-atoms/core/dist/core/AtomBinder","@web-atoms/core/dist/core/types","@web-atoms/core/dist/core/AtomMap","@web-atoms/core/dist/core/Bind","@web-atoms/core/dist/core/ExpressionParser","@web-atoms/core/dist/core/XNode","@web-atoms/core/dist/web/controls/AtomControl","@web-atoms/core/dist/core/AtomBridge","@web-atoms/core/dist/web/core/AtomUI","@web-atoms/core/dist/core/AtomComponent","@web-atoms/core/dist/App","@web-atoms/core/dist/core/AtomDispatcher","@web-atoms/core/dist/di/RegisterSingleton","@web-atoms/core/dist/di/Register","@web-atoms/core/dist/di/ServiceCollection","@web-atoms/core/dist/di/TypeKey","@web-atoms/core/dist/di/ServiceProvider","@web-atoms/core/dist/core/TransientDisposable","@web-atoms/core/dist/di/Inject","@web-atoms/core/dist/services/BusyIndicatorService","@web-atoms/core/dist/core/PropertyBinding","@web-atoms/core/dist/core/AtomOnce","@web-atoms/core/dist/core/AtomWatcher","@web-atoms/core/dist/core/AtomDisposableList","@web-atoms/core/dist/core/PropertyMap","@web-atoms/core/dist/core/FormattedString","@web-atoms/core/dist/core/WebImage","@web-atoms/core/dist/services/NavigationService","@web-atoms/core/dist/core/AtomUri","@web-atoms/core/dist/services/ReferenceService","@web-atoms/core/dist/di/DISingleton","@web-atoms/core/dist/web/styles/AtomStyle","@web-atoms/core/dist/core/StringHelper","@web-atoms/core/dist/web/styles/AtomStyleSheet","@web-atoms/samples/src/web/images/about-img.svg","@web-atoms/samples/src/web/images/hero-img.svg","@web-atoms/samples/src/web/images/logo.png","@web-atoms/samples/dist/samples/web/form/FromDemo","@web-atoms/samples/dist/core/web/FileViewer","@web-atoms/core/dist/web/controls/AtomGridSplitter","@web-atoms/core/dist/web/controls/AtomGridView","@web-atoms/core/dist/web/controls/AtomToggleButtonBar","@web-atoms/core/dist/web/styles/AtomToggleButtonBarStyle","@web-atoms/core/dist/web/styles/AtomListBoxStyle","@web-atoms/core/dist/web/controls/AtomListBox","@web-atoms/core/dist/web/controls/AtomItemsControl","@web-atoms/core/dist/core/AtomEnumerator","@web-atoms/samples/dist/core/web/FileViewerStyle","@web-atoms/core/dist/core/Colors","@web-atoms/samples/dist/core/web/CodeView","@web-atoms/core/dist/services/http/RestService","@web-atoms/core/dist/services/http/AjaxOptions","@web-atoms/core/dist/services/CacheService","@web-atoms/core/dist/services/JsonService","@web-atoms/date-time/dist/DateTime","@web-atoms/date-time/dist/TimeSpan","@web-atoms/core/dist/services/http/JsonError","@web-atoms/samples/dist/core/web/resolveModulePath","@web-atoms/samples/dist/samples/web/form/simple/MockSignupService","@web-atoms/samples/dist/samples/web/form/simple/SignupService","@web-atoms/samples/dist/samples/web/form/simple/SimpleForm","@web-atoms/core/dist/web/controls/AtomComboBox","@web-atoms/web-controls/dist/form/AtomField","@web-atoms/web-controls/dist/form/HelpPopup","@web-atoms/web-controls/dist/form/AtomForm","@web-atoms/web-controls/dist/form/AtomFormStyle","@web-atoms/web-controls/dist/form/DefaultFieldTemplate","@web-atoms/web-controls/dist/form/AtomFieldTemplate","@web-atoms/samples/dist/samples/web/form/simple/SimpleViewModel","@web-atoms/core/dist/view-model/Action","@web-atoms/core/dist/view-model/baseTypes","@web-atoms/core/dist/view-model/AtomViewModel","@web-atoms/core/dist/core/BindableProperty","@web-atoms/core/dist/view-model/Load","@web-atoms/samples/dist/view-models/IndexViewModel","@web-atoms/samples/dist/web/styles/IndexStyle","@web-atoms/core/dist/web/WebApp","@web-atoms/core/dist/web/services/WebBusyIndicatorService","@web-atoms/core/dist/web/styles/StyleBuilder","@web-atoms/core/dist/web/services/WindowService","@web-atoms/core/dist/core/AtomLoader","@web-atoms/core/dist/core/FormattedError","@web-atoms/core/dist/view-model/AtomWindowViewModel","@web-atoms/core/dist/web/controls/AtomAlertWindow","@web-atoms/core/dist/web/styles/AtomAlertWindowStyle","@web-atoms/core/dist/web/styles/AtomWindowStyle","@web-atoms/core/src/web/images/close-button-hover.svg","@web-atoms/core/dist/web/controls/AtomWindow","@web-atoms/core/dist/web/controls/AtomTemplate","@web-atoms/core/dist/web/controls/AtomNotification","@web-atoms/core/dist/web/styles/AtomNotificationStyle","@web-atoms/core/dist/web/styles/AtomPopupStyle","@web-atoms/core/dist/web/styles/AtomTheme","@web-atoms/samples/src/images/cs.png"]);

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
        var AtomMap = /** @class */ (function () {
            function AtomMap() {
                this.map = [];
            }
            Object.defineProperty(AtomMap.prototype, "size", {
                get: function () {
                    return this.map.length;
                },
                enumerable: false,
                configurable: true
            });
            AtomMap.prototype.clear = function () {
                this.map.length = 0;
            };
            AtomMap.prototype.delete = function (key) {
                return this.map.remove(function (x) { return x.key === key; });
            };
            AtomMap.prototype.forEach = function (callbackfn, thisArg) {
                for (var _i = 0, _a = this.map; _i < _a.length; _i++) {
                    var iterator = _a[_i];
                    callbackfn.call(thisArg, iterator.value, iterator.key, this);
                }
            };
            AtomMap.prototype.get = function (key) {
                var item = this.getItem(key, false);
                return item ? item.value : undefined;
            };
            AtomMap.prototype.has = function (key) {
                return this.map.find(function (x) { return x.key === key; }) != null;
            };
            AtomMap.prototype.set = function (key, value) {
                var item = this.getItem(key, true);
                item.value = value;
                return this;
            };
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
            AtomMap.prototype.getItem = function (key, create) {
                if (create === void 0) { create = false; }
                for (var _i = 0, _a = this.map; _i < _a.length; _i++) {
                    var iterator = _a[_i];
                    if (iterator.key === key) {
                        return iterator;
                    }
                }
                if (create) {
                    var r = { key: key, value: undefined };
                    this.map.push(r);
                    return r;
                }
            };
            return AtomMap;
        }());
        // tslint:disable-next-line:no-string-literal
        window["Map"] = AtomMap;
    }
    // tslint:disable-next-line:only-arrow-functions
    Map.prototype.getOrCreate = function (key, factory) {
        var item = this.get(key);
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
    var AtomMap_1 = require("./AtomMap");
    /**
     *
     *
     * @export
     * @class CancelToken
     */
    var CancelToken = /** @class */ (function () {
        function CancelToken(timeout) {
            var _this = this;
            if (timeout === void 0) { timeout = -1; }
            this.listeners = [];
            this.mCancelled = null;
            this.cancelTimeout = null;
            if (timeout > 0) {
                this.cancelTimeout = setTimeout(function () {
                    _this.cancelTimeout = null;
                    _this.cancel("timeout");
                }, timeout);
            }
        }
        Object.defineProperty(CancelToken.prototype, "cancelled", {
            get: function () {
                return this.mCancelled;
            },
            enumerable: false,
            configurable: true
        });
        CancelToken.prototype.cancel = function (r) {
            if (r === void 0) { r = "cancelled"; }
            this.mCancelled = r;
            var existing = this.listeners.slice(0);
            this.listeners.length = 0;
            for (var _i = 0, existing_1 = existing; _i < existing_1.length; _i++) {
                var fx = existing_1[_i];
                fx(r);
            }
            this.dispose();
        };
        CancelToken.prototype.reset = function () {
            this.mCancelled = null;
            this.dispose();
        };
        CancelToken.prototype.dispose = function () {
            this.listeners.length = 0;
            if (this.cancelTimeout) {
                clearTimeout(this.cancelTimeout);
            }
        };
        CancelToken.prototype.registerForCancel = function (f) {
            if (this.mCancelled) {
                f(this.mCancelled);
                this.cancel();
                return;
            }
            this.listeners.push(f);
        };
        return CancelToken;
    }());
    exports.CancelToken = CancelToken;
    var ArrayHelper = /** @class */ (function () {
        function ArrayHelper() {
        }
        ArrayHelper.remove = function (a, filter) {
            for (var i = 0; i < a.length; i++) {
                var item = a[i];
                if (filter(item)) {
                    a.splice(i, 1);
                    return true;
                }
            }
            return false;
        };
        return ArrayHelper;
    }());
    exports.ArrayHelper = ArrayHelper;
    // tslint:disable-next-line
    Array.prototype["groupBy"] = function (keySelector) {
        var map = new AtomMap_1.default();
        var groups = [];
        for (var _i = 0, _a = this; _i < _a.length; _i++) {
            var iterator = _a[_i];
            var key = keySelector(iterator);
            var g = map.get(key);
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
    var globalNS = (typeof window !== "undefined" ? window : global);
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
    var types_1 = require("./types");
    var AtomBinder = /** @class */ (function () {
        function AtomBinder() {
        }
        AtomBinder.refreshValue = function (target, key) {
            var handlers = AtomBinder.get_WatchHandler(target, key);
            if (handlers === undefined || handlers == null) {
                return;
            }
            for (var _i = 0, handlers_1 = handlers; _i < handlers_1.length; _i++) {
                var item = handlers_1[_i];
                item(target, key);
            }
            if (target.onPropertyChanged) {
                target.onPropertyChanged(key);
            }
        };
        AtomBinder.add_WatchHandler = function (target, key, handler) {
            if (target == null) {
                return;
            }
            var handlers = AtomBinder.get_WatchHandler(target, key);
            handlers.push(handler);
            if (Array.isArray(target)) {
                return;
            }
            // get existing property definition if it ha any
            var pv = AtomBinder.getPropertyDescriptor(target, key);
            // return if it has a getter
            // in case of getter/setter, it is responsibility of setter to refresh
            // object
            if (pv && pv.get) {
                return;
            }
            var tw = target;
            if (!tw._$_bindable) {
                tw._$_bindable = {};
            }
            if (!tw._$_bindable[key]) {
                tw._$_bindable[key] = 1;
                var o = target[key];
                var nk_1 = "_$_" + key;
                target[nk_1] = o;
                var set = function (v) {
                    var ov = this[nk_1];
                    // tslint:disable-next-line:triple-equals
                    if (ov === undefined ? ov === v : ov == v) {
                        return;
                    }
                    this[nk_1] = v;
                    AtomBinder.refreshValue(this, key);
                };
                var get = function () {
                    return this[nk_1];
                };
                if (pv) {
                    delete target[key];
                    Object.defineProperty(target, key, {
                        get: get,
                        set: set,
                        configurable: true,
                        enumerable: true
                    });
                }
                else {
                    Object.defineProperty(target, key, {
                        get: get, set: set,
                        enumerable: true, configurable: true
                    });
                }
            }
        };
        AtomBinder.getPropertyDescriptor = function (target, key) {
            var pv = Object.getOwnPropertyDescriptor(target, key);
            if (!pv) {
                var pt = Object.getPrototypeOf(target);
                if (pt) {
                    return AtomBinder.getPropertyDescriptor(pt, key);
                }
            }
            return pv;
        };
        AtomBinder.get_WatchHandler = function (target, key) {
            if (target == null) {
                return null;
            }
            var handlers = target._$_handlers;
            if (!handlers) {
                handlers = {};
                target._$_handlers = handlers;
            }
            var handlersForKey = handlers[key];
            if (handlersForKey === undefined || handlersForKey == null) {
                handlersForKey = [];
                handlers[key] = handlersForKey;
            }
            return handlersForKey;
        };
        AtomBinder.remove_WatchHandler = function (target, key, handler) {
            if (target == null) {
                return;
            }
            if (!target._$_handlers) {
                return;
            }
            var handlersForKey = target._$_handlers[key];
            if (handlersForKey === undefined || handlersForKey == null) {
                return;
            }
            // handlersForKey = handlersForKey.filter( (f) => f !== handler);
            types_1.ArrayHelper.remove(handlersForKey, function (f) { return f === handler; });
            if (!handlersForKey.length) {
                target._$_handlers[key] = null;
                delete target._$_handlers[key];
            }
        };
        AtomBinder.invokeItemsEvent = function (target, mode, index, item) {
            var key = "_items";
            var handlers = AtomBinder.get_WatchHandler(target, key);
            if (!handlers) {
                return;
            }
            for (var _i = 0, handlers_2 = handlers; _i < handlers_2.length; _i++) {
                var obj = handlers_2[_i];
                obj(target, mode, index, item);
            }
            AtomBinder.refreshValue(target, "length");
        };
        AtomBinder.refreshItems = function (ary) {
            AtomBinder.invokeItemsEvent(ary, "refresh", -1, null);
        };
        AtomBinder.add_CollectionChanged = function (target, handler) {
            if (target == null) {
                throw new Error("Target Array to watch cannot be null");
            }
            if (handler == null) {
                throw new Error("Target handle to watch an Array cannot be null");
            }
            var handlers = AtomBinder.get_WatchHandler(target, "_items");
            handlers.push(handler);
            return { dispose: function () {
                    AtomBinder.remove_CollectionChanged(target, handler);
                }
            };
        };
        AtomBinder.remove_CollectionChanged = function (t, handler) {
            if (t == null) {
                return;
            }
            var target = t;
            if (!target._$_handlers) {
                return;
            }
            var key = "_items";
            var handlersForKey = target._$_handlers[key];
            if (handlersForKey === undefined || handlersForKey == null) {
                return;
            }
            types_1.ArrayHelper.remove(handlersForKey, function (f) { return f === handler; });
            if (!handlersForKey.length) {
                target._$_handlers[key] = null;
                delete target._$_handlers[key];
            }
        };
        AtomBinder.watch = function (item, property, f) {
            AtomBinder.add_WatchHandler(item, property, f);
            return {
                dispose: function () {
                    AtomBinder.remove_WatchHandler(item, property, f);
                }
            };
        };
        AtomBinder.clear = function (a) {
            a.length = 0;
            this.invokeItemsEvent(a, "refresh", -1, null);
            AtomBinder.refreshValue(a, "length");
        };
        AtomBinder.addItem = function (a, item) {
            var index = a.length;
            a.push(item);
            this.invokeItemsEvent(a, "add", index, item);
            AtomBinder.refreshValue(a, "length");
        };
        AtomBinder.removeItem = function (a, item) {
            var i = a.findIndex(function (x) { return x === item; });
            if (i === -1) {
                return false;
            }
            a.splice(i, 1);
            AtomBinder.invokeItemsEvent(a, "remove", i, item);
            AtomBinder.refreshValue(a, "length");
            return true;
        };
        return AtomBinder;
    }());
    exports.AtomBinder = AtomBinder;
});
//# sourceMappingURL=AtomBinder.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomBinder");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
    var AtomBinder_1 = require("./AtomBinder");
    /**
     *
     *
     * @export
     * @class AtomList
     * @extends {Array<T>}
     * @template T
     */
    var AtomList = /** @class */ (function (_super) {
        __extends(AtomList, _super);
        // private version: number = 1;
        function AtomList() {
            var _this = _super.call(this) || this;
            _this.startValue = 0;
            _this.totalValue = 0;
            _this.sizeValue = 10;
            // tslint:disable-next-line
            _this["__proto__"] = AtomList.prototype;
            _this.next = function () {
                _this.start = _this.start + _this.size;
            };
            _this.prev = function () {
                if (_this.start >= _this.size) {
                    _this.start = _this.start - _this.size;
                }
            };
            return _this;
        }
        Object.defineProperty(AtomList.prototype, "start", {
            get: function () {
                return this.startValue;
            },
            set: function (v) {
                if (v === this.startValue) {
                    return;
                }
                this.startValue = v;
                AtomBinder_1.AtomBinder.refreshValue(this, "start");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomList.prototype, "total", {
            get: function () {
                return this.totalValue;
            },
            set: function (v) {
                if (v === this.totalValue) {
                    return;
                }
                this.totalValue = v;
                AtomBinder_1.AtomBinder.refreshValue(this, "total");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomList.prototype, "size", {
            get: function () {
                return this.sizeValue;
            },
            set: function (v) {
                if (v === this.sizeValue) {
                    return;
                }
                this.sizeValue = v;
                AtomBinder_1.AtomBinder.refreshValue(this, "size");
            },
            enumerable: false,
            configurable: true
        });
        /**
         * Adds the item in the list and refresh bindings
         * @param {T} item
         * @returns {number}
         * @memberof AtomList
         */
        AtomList.prototype.add = function (item) {
            var i = this.length;
            var n = this.push(item);
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "add", i, item);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
            // this.version++;
            return n;
        };
        /**
         * Add given items in the list and refresh bindings
         * @param {Array<T>} items
         * @memberof AtomList
         */
        AtomList.prototype.addAll = function (items) {
            for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
                var item = items_1[_i];
                var i = this.length;
                this.push(item);
                AtomBinder_1.AtomBinder.invokeItemsEvent(this, "add", i, item);
                AtomBinder_1.AtomBinder.refreshValue(this, "length");
            }
            // tslint:disable-next-line:no-string-literal
            var t = items["total"];
            if (t) {
                this.total = t;
            }
            // this.version++;
        };
        /**
         * Replaces list with given items, use this
         * to avoid flickering in screen
         * @param {T[]} items
         * @memberof AtomList
         */
        AtomList.prototype.replace = function (items, start, size) {
            this.length = items.length;
            for (var i = 0; i < items.length; i++) {
                this[i] = items[i];
            }
            this.refresh();
            // tslint:disable-next-line:no-string-literal
            var t = items["total"];
            if (t) {
                this.total = t;
            }
            if (start !== undefined) {
                this.start = start;
            }
            if (size !== undefined) {
                this.size = size;
            }
        };
        /**
         * Inserts given number in the list at position `i`
         * and refreshes the bindings.
         * @param {number} i
         * @param {T} item
         * @memberof AtomList
         */
        AtomList.prototype.insert = function (i, item) {
            var n = this.splice(i, 0, item);
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "add", i, item);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
        };
        /**
         * Removes item at given index i and refresh the bindings
         * @param {number} i
         * @memberof AtomList
         */
        AtomList.prototype.removeAt = function (i) {
            var item = this[i];
            this.splice(i, 1);
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "remove", i, item);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
        };
        /**
         * Removes given item or removes all items that match
         * given lambda as true and refresh the bindings
         * @param {(T | ((i:T) => boolean))} item
         * @returns {boolean} `true` if any item was removed
         * @memberof AtomList
         */
        AtomList.prototype.remove = function (item) {
            if (item instanceof Function) {
                var index = 0;
                var removed = false;
                for (var _i = 0, _a = this; _i < _a.length; _i++) {
                    var it = _a[_i];
                    if (item(it)) {
                        this.removeAt(index);
                        removed = true;
                        continue;
                    }
                    index++;
                }
                return removed;
            }
            var n = this.indexOf(item);
            if (n !== -1) {
                this.removeAt(n);
                return true;
            }
            return false;
        };
        /**
         * Removes all items from the list and refreshes the bindings
         * @memberof AtomList
         */
        AtomList.prototype.clear = function () {
            this.length = 0;
            this.refresh();
        };
        AtomList.prototype.refresh = function () {
            AtomBinder_1.AtomBinder.invokeItemsEvent(this, "refresh", -1, null);
            AtomBinder_1.AtomBinder.refreshValue(this, "length");
            // this.version++;
        };
        AtomList.prototype.watch = function (f, wrap) {
            if (wrap) {
                var fx_1 = f;
                f = (function () {
                    var p = [];
                    // tslint:disable-next-line:prefer-for-of
                    for (var i = 0; i < arguments.length; i++) {
                        var iterator = arguments[i];
                        p.push(iterator);
                    }
                    return fx_1.call(this, p);
                });
            }
            return AtomBinder_1.AtomBinder.add_CollectionChanged(this, f);
        };
        return AtomList;
    }(Array));
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var Atom = /** @class */ (function () {
        function Atom() {
        }
        // tslint:disable-next-line: ban-types
        Atom.superProperty = function (tc, target, name) {
            var c = tc;
            do {
                c = Object.getPrototypeOf(c);
                if (!c) {
                    throw new Error("No property descriptor found for " + name);
                }
                var pd = Object.getOwnPropertyDescriptor(c.prototype, name);
                if (!pd) {
                    continue;
                }
                return pd.get.apply(target);
            } while (true);
        };
        //      public static set(arg0: any, arg1: any, arg2: any): any {
        //     throw new Error("Method not implemented.");
        // }
        Atom.get = function (target, path) {
            var segments = path.split(".");
            for (var _i = 0, segments_1 = segments; _i < segments_1.length; _i++) {
                var iterator = segments_1[_i];
                if (target === undefined || target === null) {
                    return target;
                }
                target = target[iterator];
            }
            return target;
        };
        /**
         * Await till given milliseconds have passed
         * @param n
         * @param ct
         */
        Atom.delay = function (n, ct) {
            return new Promise(function (resolve, reject) {
                var h = {};
                h.id = setTimeout(function () {
                    // if (ct && ct.cancelled) {
                    //     reject(new Error("cancelled"));
                    //     return;
                    // }
                    resolve();
                }, n);
                if (ct) {
                    ct.registerForCancel(function () {
                        clearTimeout(h.id);
                        reject(new Error("cancelled"));
                    });
                }
            });
        };
        // // tslint:disable-next-line:member-access
        // static query(arg0: any): any {
        //     throw new Error("Method not implemented.");
        // }
        Atom.encodeParameters = function (p) {
            if (!p) {
                return "";
            }
            var s = "";
            for (var key in p) {
                if (p.hasOwnProperty(key)) {
                    var element = p[key];
                    var v = element;
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
                    s += key + "=" + encodeURIComponent(v);
                }
            }
            return s;
        };
        Atom.url = function (url, query, hash) {
            if (!url) {
                return url;
            }
            var p = this.encodeParameters(query);
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
        };
        /**
         * Schedules given call in next available callLater slot and also returns
         * promise that can be awaited, calling `Atom.postAsync` inside `Atom.postAsync`
         * will create deadlock
         * @static
         * @param {()=>Promise<any>} f
         * @returns {Promise<any>}
         * @memberof Atom
         */
        Atom.postAsync = function (app, f) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                app.callLater(function () { return __awaiter(_this, void 0, void 0, function () {
                    var _a, error_1;
                    return __generator(this, function (_b) {
                        switch (_b.label) {
                            case 0:
                                _b.trys.push([0, 2, , 3]);
                                _a = resolve;
                                return [4 /*yield*/, f()];
                            case 1:
                                _a.apply(void 0, [_b.sent()]);
                                return [3 /*break*/, 3];
                            case 2:
                                error_1 = _b.sent();
                                reject(error_1);
                                return [3 /*break*/, 3];
                            case 3: return [2 /*return*/];
                        }
                    });
                }); });
            });
        };
        Atom.designMode = false;
        return Atom;
    }());
    exports.Atom = Atom;
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
    var viewModelParseWatchCache = {};
    function parsePath(f, parseThis) {
        var str = f.toString().trim();
        str = str.split("\n").filter(function (s) { return !/^\/\//.test(s.trim()); }).join("\n");
        var key = (parseThis === undefined ? "un:" : (parseThis ? "_this:" : "_noThis:")) + str;
        var px1 = viewModelParseWatchCache[key];
        if (px1) {
            return px1;
        }
        if (str.endsWith("}")) {
            str = str.substr(0, str.length - 1);
        }
        if (str.startsWith("function (")) {
            str = str.substr("function (".length);
        }
        if (str.startsWith("function(")) {
            str = str.substr("function(".length);
        }
        str = str.trim();
        var index = str.indexOf(")");
        var commaIndex = str.indexOf(",");
        if (commaIndex !== -1 && commaIndex < index) {
            index = commaIndex;
        }
        var isThis = parseThis === undefined ? (index === 0 || parseThis) : parseThis;
        var p = (isThis ? "(\\_this|this)" : (str.substr(0, index) || "")).trim();
        /**
         * This is the case when there is no parameter to check and there `parseThis` is false
         */
        if (p.length === 0) {
            var empty = [];
            viewModelParseWatchCache[key] = empty;
            return empty;
        }
        str = str.substr(index + 1);
        var regExp = "(?:(\\b" + p + ")(?:(\\.[a-zA-Z_][a-zA-Z_0-9]*)+)\\s?(?:(\\(|\\=\\=\\=|\\=\\=|\\=)?))";
        var re = new RegExp(regExp, "gi");
        var path = [];
        var ms = str.replace(re, function (m) {
            // console.log(`m: ${m}`);
            var px = m;
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
            px = px.split(".").filter(function (s) { return !s.endsWith("("); }).join(".");
            // console.log(px);
            if (!path.find(function (y) { return y === px; })) {
                path.push(px);
            }
            path = path.filter(function (f1) { return f1.endsWith("==") || !(f1.endsWith("(") || f1.endsWith("=")); });
            path = path.map(function (px2) { return px2.endsWith("===") ? px2.substr(0, px2.length - 3) :
                (px2.endsWith("==") ? px2.substr(0, px2.length - 2) : px2); })
                .map(function (px2) { return px2.trim(); });
            return m;
        });
        path = path.sort(function (a, b) { return b.localeCompare(a); });
        var duplicates = path;
        path = [];
        var _loop_1 = function (iterator) {
            if (path.find(function (px2) { return px2 === iterator; })) {
                return "continue";
            }
            path.push(iterator);
        };
        for (var _i = 0, duplicates_1 = duplicates; _i < duplicates_1.length; _i++) {
            var iterator = duplicates_1[_i];
            _loop_1(iterator);
        }
        var rp = [];
        var _loop_2 = function (rpItem) {
            if (rp.find(function (x) { return x.startsWith(rpItem); })) {
                return "continue";
            }
            rp.push(rpItem);
        };
        for (var _a = 0, path_1 = path; _a < path_1.length; _a++) {
            var rpItem = path_1[_a];
            _loop_2(rpItem);
        }
        // tslint:disable-next-line: no-console
        // console.log(`Watching: ${path.join(", ")}`);
        var pl = path.filter(function (p1) { return p1; }).map(function (p1) { return p1.split("."); });
        viewModelParseWatchCache[key] = pl;
        return pl;
    }
    exports.parsePath = parsePath;
    var viewModelParseWatchCache2 = {};
    function parsePathLists(f) {
        var str = f.toString().trim();
        var key = str;
        var px1 = viewModelParseWatchCache2[key];
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
        var pl = {
            pathList: parsePath(str, false),
            thisPath: parsePath(str, true),
            combined: []
        };
        if (pl.thisPath.length && pl.pathList.length) {
            // we need to combine this
            // pl.combinedPathList =
            pl.combined = pl.thisPath
                .map(function (x) {
                x[0] = "t";
                x.splice(0, 0, "this");
                return x;
            })
                .concat(pl.pathList.map(function (x) {
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
    var ExpressionParser_1 = require("./ExpressionParser");
    var isEvent = /^event/i;
    function oneTime(name, b, control, e) {
        control.runAfterInit(function () {
            control.setLocalValue(e, name, b.sourcePath(control, e));
        });
    }
    function event(name, b, control, e) {
        control.runAfterInit(function () {
            if (isEvent.test(name)) {
                name = name.substr(5);
                name = (name[0].toLowerCase() + name.substr(1));
            }
            control.bindEvent(e, name, function (e1) {
                return b.sourcePath(control, e1);
            });
        });
    }
    function oneWay(name, b, control, e, creator) {
        if (b.pathList) {
            control.bind(e, name, b.pathList, false, function () {
                // tslint:disable-next-line: ban-types
                return b.sourcePath.call(creator, control, e);
            });
            return;
        }
        if (b.combined) {
            var a = {
                // it is `this`
                t: creator,
                // it is first parameter
                x: control
            };
            control.bind(e, name, b.combined, false, function () {
                // tslint:disable-next-line: ban-types
                return b.sourcePath.call(creator, control, e);
            }, a);
            return;
        }
        if (b.thisPathList) {
            control.bind(e, name, b.thisPathList, false, function () {
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
        var n = b.name || name;
        var c = control.element;
        while (c) {
            if (c.atomControl && c.atomControl[n] !== undefined) {
                break;
            }
            c = c._logicalParent || c.parentElement;
        }
        ((c && c.atomControl) || control)[n] = e;
    }
    var Bind = /** @class */ (function () {
        function Bind(setupFunction, sourcePath, name, eventList) {
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
                var lists = ExpressionParser_1.parsePathLists(this.sourcePath);
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
                        throw new Error("Failed to setup binding for " + this.sourcePath + ", parsing failed");
                    }
                }
            }
        }
        Bind.forControl = function () {
            return Bind;
        };
        Bind.forData = function () {
            return Bind;
        };
        Bind.forViewModel = function () {
            return Bind;
        };
        Bind.forLocalViewModel = function () {
            return Bind;
        };
        Bind.presenter = function (name) {
            return new Bind(presenter, null, name);
        };
        // tslint:disable-next-line: ban-types
        Bind.event = function (sourcePath) {
            return new Bind(event, sourcePath);
        };
        Bind.oneTime = function (sourcePath) {
            return new Bind(oneTime, sourcePath);
        };
        Bind.oneWay = function (sourcePath) {
            return new Bind(oneWay, sourcePath);
        };
        Bind.twoWays = function (sourcePath, events) {
            var b = new Bind(twoWays, sourcePath, null, events);
            if (!(b.thisPathList || b.pathList)) {
                throw new Error("Failed to setup twoWay binding on " + sourcePath);
            }
            return b;
        };
        /**
         * Use this for HTML only, this will fire two way binding
         * as soon as the input/textarea box is updated
         * @param sourcePath binding lambda expression
         */
        Bind.twoWaysImmediate = function (sourcePath) {
            var b = new Bind(twoWays, sourcePath, null, ["change", "input", "paste"]);
            if (!(b.thisPathList || b.pathList)) {
                throw new Error("Failed to setup twoWay binding on " + sourcePath);
            }
            return b;
        };
        return Bind;
    }());
    exports.default = Bind;
});
//# sourceMappingURL=Bind.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/Bind");

var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
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
    exports.RootObject = void 0;
    var RootObject = /** @class */ (function () {
        function RootObject() {
        }
        Object.defineProperty(RootObject.prototype, "vsProps", {
            get: function () {
                return undefined;
            },
            enumerable: false,
            configurable: true
        });
        RootObject.prototype.addEventListener = function (name, handler) {
            return bridge.addEventHandler(this, name, handler);
        };
        RootObject.prototype.appendChild = function (e) {
            bridge.appendChild(this, e);
        };
        RootObject.prototype.dispatchEvent = function (evt) {
            bridge.dispatchEvent(evt);
        };
        return RootObject;
    }());
    exports.RootObject = RootObject;
    var XNode = /** @class */ (function () {
        function XNode(
        // tslint:disable-next-line: ban-types
        name, attributes, children, isProperty, isTemplate) {
            this.name = name;
            this.attributes = attributes;
            this.children = children;
            this.isProperty = isProperty;
            this.isTemplate = isTemplate;
        }
        XNode.attach = function (n, tag) {
            return {
                factory: function (attributes) {
                    var nodes = [];
                    for (var _i = 1; _i < arguments.length; _i++) {
                        nodes[_i - 1] = arguments[_i];
                    }
                    return new XNode(n, attributes
                        ? __assign(__assign({}, attributes), { for: tag }) : { for: tag }, nodes);
                }
            };
        };
        XNode.prepare = function (n, isProperty, isTemplate) {
            function px(v) {
                var _a;
                return (_a = {},
                    _a[n] = v,
                    _a);
            }
            px.factory = function (a) {
                var nodes = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    nodes[_i - 1] = arguments[_i];
                }
                return new XNode(n, a, nodes, isProperty, isTemplate);
            };
            px.toString = function () { return n; };
            return px;
            // return {
            //     factory(a: any, ... nodes: any[]) {
            //         return new XNode(n, a, nodes, isProperty , isTemplate);
            //     },
            //     toString() {
            //         return n;
            //     }
            // } as any;
        };
        // public static property(): NodeFactory {
        //     return {
        //         factory: true
        //     } as any;
        // }
        XNode.getClass = function (fullTypeName, assemblyName) {
            var n = fullTypeName + ";" + assemblyName;
            var cx = XNode.classes[n] || (XNode.classes[n] =
                bridge.getClass(fullTypeName, assemblyName, RootObject, function (name, isProperty, isTemplate) {
                    return function (a) {
                        var nodes = [];
                        for (var _i = 1; _i < arguments.length; _i++) {
                            nodes[_i - 1] = arguments[_i];
                        }
                        return new XNode(name, a, nodes, isProperty, isTemplate);
                    };
                }));
            return cx;
        };
        /**
         * Declares Root Namespace and Assembly. You can use return function to
         * to declare the type
         * @param ns Root Namespace
         */
        XNode.namespace = function (ns, assemblyName) {
            return function (type, isTemplate) {
                return function (c) {
                    var _loop_1 = function (key) {
                        if (c.hasOwnProperty(key)) {
                            var element_1 = c[key];
                            if (element_1) {
                                var n_1 = ns + "." + type + ":" + key + ";" + assemblyName;
                                var af = function (a) {
                                    var _a;
                                    var r = (_a = {},
                                        _a[n_1] = a,
                                        _a);
                                    Object.defineProperty(r, "toString", {
                                        value: function () { return n_1; },
                                        enumerable: false,
                                        configurable: false
                                    });
                                    return r;
                                };
                                af.factory = function (a) {
                                    var nodes = [];
                                    for (var _i = 1; _i < arguments.length; _i++) {
                                        nodes[_i - 1] = arguments[_i];
                                    }
                                    return new XNode(n_1, a, nodes, true, element_1.isTemplate);
                                };
                                af.toString = function () { return n_1; };
                                c[key] = af;
                            }
                        }
                    };
                    // static properties !!
                    for (var key in c) {
                        _loop_1(key);
                    }
                    var tn = ns + "." + type + ";" + assemblyName;
                    c.factory = function (a) {
                        var nodes = [];
                        for (var _i = 1; _i < arguments.length; _i++) {
                            nodes[_i - 1] = arguments[_i];
                        }
                        return new XNode(tn, a, nodes, false, isTemplate);
                    };
                    c.toString = function () { return tn; };
                };
            };
        };
        XNode.create = function (
        // tslint:disable-next-line: ban-types
        name, attributes) {
            var _a;
            var children = [];
            for (var _i = 2; _i < arguments.length; _i++) {
                children[_i - 2] = arguments[_i];
            }
            if (name.factory) {
                return (_a = name).factory.apply(_a, __spreadArrays([attributes], children));
            }
            if (name.isControl) {
                return new XNode(name, attributes, children);
            }
            switch (typeof name) {
                case "object":
                    name = name.toString();
                    break;
                case "function":
                    return name.apply(void 0, __spreadArrays([attributes], children));
            }
            return new XNode(name, attributes, children);
        };
        XNode.prototype.toString = function () {
            if (typeof this.name === "string") {
                return "name is of type string and value is " + this.name;
            }
            return "name is of type " + typeof this.name;
        };
        XNode.classes = {};
        // public static template(): NodeFactory {
        //     return {
        //         factory: true,
        //         isTemplate: true,
        //     } as any;
        // }
        XNode.attached = function (name) { return function (n) {
            var _a;
            return (_a = {}, _a[name] = n, _a);
        }; };
        XNode.factory = function (name, isProperty, isTemplate) { return function (a) {
            var nodes = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                nodes[_i - 1] = arguments[_i];
            }
            return new XNode(name, a, nodes, isProperty, isTemplate);
        }; };
        return XNode;
    }());
    exports.default = XNode;
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
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomUI = exports.ChildEnumerator = void 0;
    // refer http://youmightnotneedjquery.com/
    var ChildEnumerator = /** @class */ (function () {
        function ChildEnumerator(e) {
            this.e = e;
        }
        Object.defineProperty(ChildEnumerator.prototype, "current", {
            get: function () {
                return this.item;
            },
            enumerable: false,
            configurable: true
        });
        ChildEnumerator.prototype.next = function () {
            if (!this.item) {
                this.item = this.e.firstElementChild;
            }
            else {
                this.item = this.item.nextElementSibling;
            }
            return this.item ? true : false;
        };
        return ChildEnumerator;
    }());
    exports.ChildEnumerator = ChildEnumerator;
    var AtomUI = /** @class */ (function () {
        function AtomUI() {
        }
        AtomUI.outerHeight = function (el, margin) {
            if (margin === void 0) { margin = false; }
            var height = el.offsetHeight;
            if (!margin) {
                return height;
            }
            var style = getComputedStyle(el);
            height += parseInt(style.marginTop, 10) + parseInt(style.marginBottom, 10);
            return height;
        };
        AtomUI.outerWidth = function (el, margin) {
            if (margin === void 0) { margin = false; }
            var width = el.offsetWidth;
            if (!margin) {
                return width;
            }
            var style = getComputedStyle(el);
            width += parseInt(style.marginLeft, 10) + parseInt(style.marginRight, 10);
            return width;
        };
        AtomUI.innerWidth = function (el) {
            return el.clientWidth;
        };
        AtomUI.innerHeight = function (el) {
            return el.clientHeight;
        };
        AtomUI.scrollTop = function (el, y) {
            el.scrollTo(0, y);
        };
        AtomUI.screenOffset = function (e) {
            var r = {
                x: e.offsetLeft,
                y: e.offsetTop,
                width: e.offsetWidth,
                height: e.offsetHeight
            };
            if (e.offsetParent) {
                var p = this.screenOffset(e.offsetParent);
                r.x += p.x;
                r.y += p.y;
            }
            return r;
        };
        AtomUI.parseUrl = function (url) {
            var r = {};
            var plist = url.split("&");
            for (var _i = 0, plist_1 = plist; _i < plist_1.length; _i++) {
                var item = plist_1[_i];
                var p = item.split("=");
                var key = decodeURIComponent(p[0]);
                if (!key) {
                    continue;
                }
                var val = p[1];
                if (val) {
                    val = decodeURIComponent(val);
                }
                // val = AtomUI.parseValue(val);
                r[key] = this.parseValue(val);
            }
            return r;
        };
        AtomUI.parseValue = function (val) {
            var n;
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
        };
        AtomUI.assignID = function (element) {
            if (!element.id) {
                element.id = "__waID" + AtomUI.getNewIndex();
            }
            return element.id;
        };
        AtomUI.toNumber = function (text) {
            if (!text) {
                return 0;
            }
            if (text.constructor === String) {
                return parseFloat(text);
            }
            return 0;
        };
        AtomUI.getNewIndex = function () {
            AtomUI.index = AtomUI.index + 1;
            return AtomUI.index;
        };
        AtomUI.index = 1001;
        return AtomUI;
    }());
    exports.AtomUI = AtomUI;
});
//# sourceMappingURL=AtomUI.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/core/AtomUI");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
    var AtomUI_1 = require("../web/core/AtomUI");
    var AtomBinder_1 = require("./AtomBinder");
    var BaseElementBridge = /** @class */ (function () {
        function BaseElementBridge() {
        }
        BaseElementBridge.prototype.refreshInherited = function (target, name, fieldName) {
            var _this = this;
            AtomBinder_1.AtomBinder.refreshValue(target, name);
            if (!fieldName) {
                fieldName = "m" + name[0].toUpperCase() + name.substr(1);
            }
            this.visitDescendents(target.element, function (e, ac) {
                if (ac) {
                    if (ac[fieldName] === undefined) {
                        _this.refreshInherited(ac, name, fieldName);
                    }
                    return false;
                }
                return true;
            });
        };
        BaseElementBridge.prototype.createNode = function (target, node, 
        // tslint:disable-next-line: ban-types
        binder, 
        // tslint:disable-next-line: ban-types
        xNodeClass, 
        // tslint:disable-next-line: ban-types
        creator) {
            throw new Error("Method not implemented.");
        };
        return BaseElementBridge;
    }());
    exports.BaseElementBridge = BaseElementBridge;
    var AtomElementBridge = /** @class */ (function (_super) {
        __extends(AtomElementBridge, _super);
        function AtomElementBridge() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        AtomElementBridge.prototype.addEventHandler = function (element, name, handler, capture) {
            element.addEventListener(name, handler, capture);
            return {
                dispose: function () {
                    element.removeEventListener(name, handler, capture);
                }
            };
        };
        AtomElementBridge.prototype.atomParent = function (element, climbUp) {
            if (climbUp === void 0) { climbUp = true; }
            var eAny = element;
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
        };
        AtomElementBridge.prototype.elementParent = function (element) {
            var eAny = element;
            var lp = eAny._logicalParent;
            if (lp) {
                return lp;
            }
            return element.parentElement;
        };
        AtomElementBridge.prototype.templateParent = function (element) {
            if (!element) {
                return null;
            }
            var eAny = element;
            if (eAny._templateParent) {
                return this.atomParent(element);
            }
            var parent = this.elementParent(element);
            if (!parent) {
                return null;
            }
            return this.templateParent(parent);
        };
        AtomElementBridge.prototype.visitDescendents = function (element, action) {
            var en = new AtomUI_1.ChildEnumerator(element);
            while (en.next()) {
                var iterator = en.current;
                var eAny = iterator;
                var ac = eAny ? eAny.atomControl : undefined;
                if (!action(iterator, ac)) {
                    continue;
                }
                this.visitDescendents(iterator, action);
            }
        };
        AtomElementBridge.prototype.dispose = function (element) {
            var eAny = element;
            eAny.atomControl = undefined;
            eAny.innerHTML = "";
            delete eAny.atomControl;
        };
        AtomElementBridge.prototype.appendChild = function (parent, child) {
            parent.appendChild(child);
        };
        AtomElementBridge.prototype.setValue = function (element, name, value) {
            element[name] = value;
        };
        AtomElementBridge.prototype.getValue = function (element, name) {
            return element[name];
        };
        AtomElementBridge.prototype.watchProperty = function (element, name, events, f) {
            if (events.indexOf("change") === -1) {
                events.push("change");
            }
            var l = function (e) {
                var e1 = element;
                var v = e1.type === "checkbox" ? e1.checked : e1.value;
                f(v);
            };
            for (var _i = 0, events_1 = events; _i < events_1.length; _i++) {
                var iterator = events_1[_i];
                element.addEventListener(iterator, l, false);
            }
            return {
                dispose: function () {
                    for (var _i = 0, events_2 = events; _i < events_2.length; _i++) {
                        var iterator = events_2[_i];
                        element.removeEventListener(iterator, l, false);
                    }
                }
            };
        };
        AtomElementBridge.prototype.attachControl = function (element, control) {
            element.atomControl = control;
        };
        AtomElementBridge.prototype.create = function (type) {
            return document.createElement(type);
        };
        AtomElementBridge.prototype.loadContent = function (element, text) {
            throw new Error("Not supported");
        };
        AtomElementBridge.prototype.findChild = function (element, name) {
            throw new Error("Not supported");
        };
        AtomElementBridge.prototype.close = function (element, success, error) {
            throw new Error("Not supported");
        };
        AtomElementBridge.prototype.toTemplate = function (element, creator) {
            var templateNode = element;
            var name = templateNode.name;
            if (typeof name === "string") {
                element = (function (bx, n) { return /** @class */ (function (_super) {
                    __extends(class_1, _super);
                    function class_1() {
                        return _super !== null && _super.apply(this, arguments) || this;
                    }
                    class_1.prototype.create = function () {
                        this.render(n);
                    };
                    return class_1;
                }(bx)); })(creator, templateNode.children[0]);
            }
            else {
                element = (function (base, n) { return /** @class */ (function (_super) {
                    __extends(class_2, _super);
                    function class_2() {
                        return _super !== null && _super.apply(this, arguments) || this;
                    }
                    class_2.prototype.create = function () {
                        this.render(n);
                    };
                    return class_2;
                }(base)); })(name, templateNode.children[0]);
            }
            return element;
        };
        AtomElementBridge.prototype.createNode = function (target, node, 
        // tslint:disable-next-line: ban-types
        binder, 
        // tslint:disable-next-line: ban-types
        xNodeClass, 
        // tslint:disable-next-line: ban-types
        creator) {
            var parent = null;
            var app = target.app;
            var e = null;
            var nn = node.attributes ? node.attributes.for : undefined;
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
            var a = node.attributes;
            if (a) {
                for (var key in a) {
                    if (a.hasOwnProperty(key)) {
                        var element = a[key];
                        if (element instanceof binder) {
                            if (/^event/.test(key)) {
                                var ev = key.substr(5);
                                if (ev.startsWith("-")) {
                                    ev = ev.split("-").map(function (s) { return s[0].toLowerCase() + s.substr(1); }).join("");
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
            var children = node.children;
            if (children) {
                for (var _i = 0, children_1 = children; _i < children_1.length; _i++) {
                    var iterator = children_1[_i];
                    if (typeof iterator === "string") {
                        e.appendChild(document.createTextNode(iterator));
                        continue;
                    }
                    var t = iterator.attributes ? iterator.attributes.template : null;
                    if (t) {
                        var tx = this.toTemplate(iterator, creator);
                        target[t] = tx;
                        continue;
                    }
                    if (typeof iterator.name === "string") {
                        e.appendChild(this.createNode(target, iterator, binder, xNodeClass, creator));
                        continue;
                    }
                    var child = this.createNode(target, iterator, binder, xNodeClass, creator);
                    if (parent.element && parent.element.atomControl === parent) {
                        parent.append(child.atomControl || child);
                    }
                    else {
                        parent.appendChild(child);
                    }
                }
            }
            return e;
        };
        return AtomElementBridge;
    }(BaseElementBridge));
    exports.AtomElementBridge = AtomElementBridge;
    var AtomBridge = /** @class */ (function () {
        function AtomBridge() {
        }
        AtomBridge.createNode = function (iterator, app) {
            if (typeof iterator.name === "string" || iterator.name.factory) {
                return { element: AtomBridge.instance.create(iterator.name.toString(), iterator, app) };
            }
            var fx = iterator.attributes ? iterator.attributes.for : undefined;
            var c = new iterator.name(app, fx ? AtomBridge.instance.create(fx, iterator, app) : undefined);
            return { element: c.element, control: c };
        };
        AtomBridge.toTemplate = function (app, n, creator) {
            if (n.isTemplate) {
                var t = AtomBridge.toTemplate(app, n.children[0], creator);
                return AtomBridge.instance.create(n.name.toString(), t, app);
            }
            var bridge = AtomBridge.instance;
            var fx;
            var en;
            if (typeof n.name === "function") {
                fx = n.name;
                en = (n.attributes && n.attributes.for) ? n.attributes.for : undefined;
            }
            else {
                fx = bridge.controlFactory;
                en = n.name;
            }
            return /** @class */ (function (_super) {
                __extends(Template, _super);
                function Template(a, e1) {
                    var _this = _super.call(this, a || app, e1 || (en ? bridge.create(en, null, app) : undefined)) || this;
                    // tslint:disable-next-line: variable-name
                    _this._creator = fx;
                    return _this;
                }
                Template.prototype.create = function () {
                    _super.prototype.create.call(this);
                    this.render(n, null, creator);
                };
                return Template;
            }(fx));
        };
        AtomBridge.refreshInherited = function (target, name, fieldName) {
            if (AtomBridge.instance.refreshInherited) {
                AtomBridge.instance.refreshInherited(target, name, fieldName);
                return;
            }
            AtomBinder_1.AtomBinder.refreshValue(target, name);
            if (!fieldName) {
                fieldName = "m" + name[0].toUpperCase() + name.substr(1);
            }
            AtomBridge.instance.visitDescendents(target.element, function (e, ac) {
                if (ac) {
                    if (ac[fieldName] === undefined) {
                        AtomBridge.refreshInherited(ac, name, fieldName);
                    }
                    return false;
                }
                return true;
            });
        };
        return AtomBridge;
    }());
    exports.AtomBridge = AtomBridge;
    var globalNS = (typeof window !== "undefined" ? window : global);
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
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomDispatcher = void 0;
    var AtomDispatcher = /** @class */ (function () {
        function AtomDispatcher() {
            this.head = null;
            this.tail = null;
        }
        AtomDispatcher.prototype.onTimeout = function () {
            var _this = this;
            if (this.paused) {
                return;
            }
            if (!this.head) {
                return;
            }
            var item = this.head;
            this.head = item.next;
            item.next = null;
            if (!this.head) {
                this.tail = null;
            }
            item();
            setTimeout(function () {
                _this.onTimeout();
            }, 1);
        };
        AtomDispatcher.prototype.pause = function () {
            this.paused = true;
        };
        AtomDispatcher.prototype.start = function () {
            var _this = this;
            this.paused = false;
            setTimeout(function () {
                _this.onTimeout();
            }, 1);
        };
        AtomDispatcher.prototype.callLater = function (f) {
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
        };
        AtomDispatcher.prototype.waitForAll = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                _this.callLater(function () {
                    resolve();
                });
            });
        };
        return AtomDispatcher;
    }());
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
    var AtomMap_1 = require("../core/AtomMap");
    var TypeKey = /** @class */ (function () {
        function TypeKey() {
        }
        TypeKey.get = function (c) {
            // for (const iterator of this.keys) {
            //     if (iterator.c === c) {
            //         return iterator.key;
            //     }
            // }
            // const key = `${c.name || "key"}${this.keys.length}`;
            // this.keys.push({ c, key});
            // return key;
            return TypeKey.keys.getOrCreate(c, function (c1) {
                var key = "" + (c1.name || "key") + TypeKey.keys.size;
                return key;
            });
        };
        TypeKey.keys = new AtomMap_1.default();
        return TypeKey;
    }());
    exports.TypeKey = TypeKey;
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
    var types_1 = require("../core/types");
    var TypeKey_1 = require("./TypeKey");
    var Scope;
    (function (Scope) {
        Scope[Scope["Global"] = 1] = "Global";
        Scope[Scope["Scoped"] = 2] = "Scoped";
        Scope[Scope["Transient"] = 3] = "Transient";
    })(Scope = exports.Scope || (exports.Scope = {}));
    var ServiceDescription = /** @class */ (function () {
        function ServiceDescription(id, scope, type, factory) {
            this.id = id;
            this.scope = scope;
            this.type = type;
            this.factory = factory;
            this.factory = this.factory || (function (sp) {
                return sp.create(type);
            });
        }
        return ServiceDescription;
    }());
    exports.ServiceDescription = ServiceDescription;
    var ServiceCollection = /** @class */ (function () {
        function ServiceCollection() {
            this.registrations = [];
            this.ids = 1;
        }
        ServiceCollection.prototype.register = function (type, factory, scope, id) {
            if (scope === void 0) { scope = Scope.Transient; }
            types_1.ArrayHelper.remove(this.registrations, function (r) { return id ? r.id === id : r.type === type; });
            if (!id) {
                id = TypeKey_1.TypeKey.get(type) + this.ids;
                this.ids++;
            }
            var sd = new ServiceDescription(id, scope, type, factory);
            this.registrations.push(sd);
            return sd;
        };
        ServiceCollection.prototype.registerScoped = function (type, factory, id) {
            return this.register(type, factory, Scope.Scoped, id);
        };
        ServiceCollection.prototype.registerSingleton = function (type, factory, id) {
            return this.register(type, factory, Scope.Global, id);
        };
        ServiceCollection.prototype.get = function (type) {
            return this.registrations.find(function (s) { return s.id === type || s.type === type; });
        };
        ServiceCollection.instance = new ServiceCollection();
        return ServiceCollection;
    }());
    exports.ServiceCollection = ServiceCollection;
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
    var types_1 = require("../core/types");
    var ServiceCollection_1 = require("./ServiceCollection");
    var globalNS = (typeof global === "undefined") ? window : global;
    function evalGlobal(path) {
        if (typeof path === "string") {
            var r = globalNS;
            for (var _i = 0, _a = path.split("."); _i < _a.length; _i++) {
                var iterator = _a[_i];
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
        return function (target) {
            if (typeof id === "object") {
                if (scope) {
                    id.scope = scope;
                }
                ServiceCollection_1.ServiceCollection.instance.register(id.for || target, id.for ? function (sp) { return sp.create(target); } : null, id.scope || ServiceCollection_1.Scope.Transient, id.id);
                if (id.mockOrInject) {
                    if (id.mockOrInject.inject) {
                        types_1.DI.inject(target, id.mockOrInject.inject);
                    }
                    else if (id.mockOrInject.mock) {
                        types_1.DI.mockType(target, id.mockOrInject.mock);
                    }
                    else if (id.mockOrInject.globalVar) {
                        ServiceCollection_1.ServiceCollection.instance.register(id.for || target, function (sp) { return evalGlobal(id.mockOrInject.globalVar); }, id.scope || ServiceCollection_1.Scope.Global, id.id);
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
    var Register_1 = require("./Register");
    var ServiceCollection_1 = require("./ServiceCollection");
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
    var TransientDisposable = /** @class */ (function () {
        function TransientDisposable(owner) {
            if (owner) {
                this.registerIn(owner);
            }
        }
        TransientDisposable.prototype.registerIn = function (value) {
            var v = value.disposables;
            if (v) {
                v.push(this);
            }
            else {
                if (value.registerDisposable) {
                    value.registerDisposable(this);
                }
            }
        };
        return TransientDisposable;
    }());
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
    var TypeKey_1 = require("./TypeKey");
    var InjectedTypes = /** @class */ (function () {
        function InjectedTypes() {
        }
        InjectedTypes.getParamList = function (key, typeKey1) {
            var plist = InjectedTypes.paramList[typeKey1];
            // We need to find @Inject for base types if
            // current type does not define any constructor
            var type = key;
            while (plist === undefined) {
                type = Object.getPrototypeOf(type);
                if (!type) {
                    break;
                }
                var typeKey = TypeKey_1.TypeKey.get(type);
                plist = InjectedTypes.paramList[typeKey];
                if (!plist) {
                    InjectedTypes.paramList[typeKey] = plist;
                }
            }
            return plist;
        };
        InjectedTypes.getPropertyList = function (key, typeKey1) {
            var plist = InjectedTypes.propertyList[typeKey1];
            // We need to find @Inject for base types if
            // current type does not define any constructor
            var type = key;
            while (plist === undefined) {
                type = Object.getPrototypeOf(type);
                if (!type) {
                    break;
                }
                var typeKey = TypeKey_1.TypeKey.get(type);
                plist = InjectedTypes.propertyList[typeKey];
                if (!plist) {
                    InjectedTypes.propertyList[typeKey] = plist;
                }
            }
            return plist;
        };
        InjectedTypes.paramList = {};
        InjectedTypes.propertyList = {};
        return InjectedTypes;
    }());
    exports.InjectedTypes = InjectedTypes;
    // export function Inject(target: any, name: string): void;
    function Inject(target, name, index) {
        if (index !== undefined) {
            var key = TypeKey_1.TypeKey.get(target);
            var plist = Reflect.getMetadata("design:paramtypes", target, name);
            if (typeof index === "number") {
                var pSavedList = InjectedTypes.paramList[key] || (InjectedTypes.paramList[key] = []);
                pSavedList[index] = plist[index];
            }
            else {
                throw new Error("Inject can only be applied on constructor" +
                    "parameter or a property without get/set methods");
            }
        }
        else {
            var key = TypeKey_1.TypeKey.get(target.constructor);
            var plist = Reflect.getMetadata("design:type", target, name);
            var p = InjectedTypes.propertyList[key] || (InjectedTypes.propertyList[key] = {});
            p[name] = plist;
            // need to merge base properties..
            var base = target.constructor;
            while (true) {
                base = Object.getPrototypeOf(base);
                if (!base) {
                    break;
                }
                var baseKey = TypeKey_1.TypeKey.get(base);
                var bp = InjectedTypes.propertyList[baseKey];
                if (bp) {
                    for (var pKey in bp) {
                        if (bp.hasOwnProperty(pKey)) {
                            var element = bp[pKey];
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
    var TransientDisposable_1 = require("../core/TransientDisposable");
    var types_1 = require("../core/types");
    var Inject_1 = require("./Inject");
    var ServiceCollection_1 = require("./ServiceCollection");
    var TypeKey_1 = require("./TypeKey");
    var ServiceProvider = /** @class */ (function () {
        function ServiceProvider(parent) {
            this.parent = parent;
            this.instances = {};
            if (parent === null) {
                ServiceCollection_1.ServiceCollection.instance.registerScoped(ServiceProvider);
            }
            var sd = ServiceCollection_1.ServiceCollection.instance.get(ServiceProvider);
            this.instances[sd.id] = this;
        }
        Object.defineProperty(ServiceProvider.prototype, "global", {
            get: function () {
                return this.parent === null ? this : this.parent.global;
            },
            enumerable: false,
            configurable: true
        });
        ServiceProvider.prototype.get = function (key) {
            return this.resolve(key, true);
        };
        ServiceProvider.prototype.put = function (key, value) {
            var sd = ServiceCollection_1.ServiceCollection.instance.get(key);
            if (!sd) {
                sd = ServiceCollection_1.ServiceCollection.instance.register(key, function () { return value; }, ServiceCollection_1.Scope.Global);
            }
            this.instances[sd.id] = value;
        };
        ServiceProvider.prototype.resolve = function (key, create, defValue) {
            if (create === void 0) { create = false; }
            var sd = ServiceCollection_1.ServiceCollection.instance.get(key);
            if (!sd) {
                if (!create) {
                    if (defValue !== undefined) {
                        return defValue;
                    }
                    throw new Error("No service registered for type " + key);
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
        };
        ServiceProvider.prototype.getValue = function (sd) {
            if (sd.scope === ServiceCollection_1.Scope.Transient) {
                return sd.factory(this);
            }
            var v = this.instances[sd.id];
            if (!v) {
                this.instances[sd.id] = v = sd.factory(this);
            }
            return v;
        };
        ServiceProvider.prototype.newScope = function () {
            return new ServiceProvider(this);
        };
        ServiceProvider.prototype.dispose = function () {
            for (var key in this.instances) {
                if (this.instances.hasOwnProperty(key)) {
                    var element = this.instances[key];
                    if (element === this) {
                        continue;
                    }
                    var d = element;
                    if (d.dispose) {
                        d.dispose();
                    }
                }
            }
        };
        ServiceProvider.prototype.create = function (key) {
            var _this = this;
            var originalKey = key;
            var originalTypeKey = TypeKey_1.TypeKey.get(originalKey);
            if (types_1.DI.resolveType) {
                var mappedType = ServiceProvider.mappedTypes[originalTypeKey] || (ServiceProvider.mappedTypes[originalTypeKey] = types_1.DI.resolveType(originalKey));
                key = mappedType;
            }
            var typeKey1 = TypeKey_1.TypeKey.get(key);
            var plist = Inject_1.InjectedTypes.getParamList(key, typeKey1);
            var value = null;
            if (plist) {
                var pv = plist.map(function (x) { return x ? _this.resolve(x) : (void 0); });
                pv.unshift(null);
                value = new (key.bind.apply(key, pv))();
                for (var _i = 0, pv_1 = pv; _i < pv_1.length; _i++) {
                    var iterator = pv_1[_i];
                    if (iterator && iterator instanceof TransientDisposable_1.default) {
                        iterator.registerIn(value);
                    }
                }
            }
            else {
                value = new (key)();
            }
            var propList = Inject_1.InjectedTypes.getPropertyList(key, typeKey1);
            if (propList) {
                for (var key1 in propList) {
                    if (propList.hasOwnProperty(key1)) {
                        var element = propList[key1];
                        var d = this.resolve(element);
                        value[key1] = d;
                        if (d && d instanceof TransientDisposable_1.default) {
                            d.registerIn(value);
                        }
                    }
                }
            }
            return value;
        };
        ServiceProvider.mappedTypes = {};
        return ServiceProvider;
    }());
    exports.ServiceProvider = ServiceProvider;
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
    var RegisterSingleton_1 = require("../di/RegisterSingleton");
    var BusyIndicatorService = /** @class */ (function () {
        function BusyIndicatorService() {
        }
        BusyIndicatorService.prototype.createIndicator = function () {
            return {
                dispose: function () {
                    // do nothing.
                }
            };
        };
        BusyIndicatorService = __decorate([
            RegisterSingleton_1.RegisterSingleton
        ], BusyIndicatorService);
        return BusyIndicatorService;
    }());
    exports.BusyIndicatorService = BusyIndicatorService;
});
//# sourceMappingURL=BusyIndicatorService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/BusyIndicatorService");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.App = exports.AtomMessageAction = void 0;
    var AtomBinder_1 = require("./core/AtomBinder");
    var AtomDispatcher_1 = require("./core/AtomDispatcher");
    var RegisterSingleton_1 = require("./di/RegisterSingleton");
    var ServiceProvider_1 = require("./di/ServiceProvider");
    var BusyIndicatorService_1 = require("./services/BusyIndicatorService");
    var AtomHandler = /** @class */ (function () {
        function AtomHandler(message) {
            this.message = message;
            this.list = new Array();
        }
        return AtomHandler;
    }());
    var AtomMessageAction = /** @class */ (function () {
        function AtomMessageAction(msg, a) {
            this.message = msg;
            this.action = a;
        }
        return AtomMessageAction;
    }());
    exports.AtomMessageAction = AtomMessageAction;
    var App = /** @class */ (function (_super) {
        __extends(App, _super);
        function App() {
            var _this = _super.call(this, null) || this;
            /**
             * This must be set explicitly as it can be used outside to detect
             * if app is ready. This will not be set automatically by framework.
             */
            _this.appReady = false;
            _this.busyIndicators = [];
            // tslint:disable-next-line:ban-types
            _this.readyHandlers = [];
            _this.onError = function (error) {
                // tslint:disable-next-line:no-console
                console.log(error);
            };
            _this.screen = {};
            _this.bag = {};
            _this.put(App_1, _this);
            _this.dispatcher = new AtomDispatcher_1.AtomDispatcher();
            _this.dispatcher.start();
            _this.put(AtomDispatcher_1.AtomDispatcher, _this.dispatcher);
            setTimeout(function () {
                _this.invokeReady();
            }, 5);
            return _this;
        }
        App_1 = App;
        Object.defineProperty(App.prototype, "url", {
            get: function () {
                return this.mUrl;
            },
            set: function (v) {
                this.mUrl = v;
                AtomBinder_1.AtomBinder.refreshValue(this, "url");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(App.prototype, "contextId", {
            get: function () {
                return "none";
            },
            enumerable: false,
            configurable: true
        });
        App.prototype.createBusyIndicator = function () {
            this.busyIndicatorService = this.busyIndicatorService
                || this.resolve(BusyIndicatorService_1.BusyIndicatorService);
            return this.busyIndicatorService.createIndicator();
        };
        App.prototype.syncUrl = function () {
            // must be implemented by platform specific app
        };
        App.prototype.callLater = function (f) {
            this.dispatcher.callLater(f);
        };
        App.prototype.updateDefaultStyle = function (content) {
            throw new Error("Platform does not support StyleSheets");
        };
        App.prototype.waitForPendingCalls = function () {
            return this.dispatcher.waitForAll();
        };
        /**
         * This method will run any asynchronous method
         * and it will display an error if it will fail
         * asynchronously
         *
         * @template T
         * @param {() => Promise<T>} tf
         * @memberof AtomDevice
         */
        App.prototype.runAsync = function (tf) {
            var _this = this;
            try {
                var p = tf();
                if (p && p.then && p.catch) {
                    p.catch(function (error) {
                        _this.onError("runAsync");
                        _this.onError(error);
                    });
                }
            }
            catch (e) {
                this.onError("runAsync");
                this.onError(e);
            }
        };
        /**
         * Broadcast given data to channel, only within the current window.
         *
         * @param {string} channel
         * @param {*} data
         * @returns
         * @memberof AtomDevice
         */
        App.prototype.broadcast = function (channel, data) {
            var ary = this.bag[channel];
            if (!ary) {
                return;
            }
            for (var _i = 0, _a = ary.list; _i < _a.length; _i++) {
                var entry = _a[_i];
                entry.call(this, channel, data);
            }
        };
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
        App.prototype.subscribe = function (channel, action) {
            var _this = this;
            var ary = this.bag[channel];
            if (!ary) {
                ary = new AtomHandler(channel);
                this.bag[channel] = ary;
            }
            ary.list.push(action);
            return {
                dispose: function () {
                    ary.list = ary.list.filter(function (a) { return a !== action; });
                    if (!ary.list.length) {
                        _this.bag[channel] = null;
                    }
                }
            };
        };
        App.prototype.main = function () {
            // load app here..
        };
        // tslint:disable-next-line:no-empty
        App.prototype.onReady = function (f) {
            if (this.readyHandlers) {
                this.readyHandlers.push(f);
            }
            else {
                this.invokeReadyHandler(f);
            }
        };
        App.prototype.invokeReady = function () {
            for (var _i = 0, _a = this.readyHandlers; _i < _a.length; _i++) {
                var iterator = _a[_i];
                this.invokeReadyHandler(iterator);
            }
            this.readyHandlers = null;
        };
        // tslint:disable-next-line:ban-types
        App.prototype.invokeReadyHandler = function (f) {
            var indicator = this.createBusyIndicator();
            var a = f();
            if (a && a.then && a.catch) {
                a.then(function (r) {
                    // do nothing
                    indicator.dispose();
                });
                a.catch(function (e) {
                    indicator.dispose();
                    // tslint:disable-next-line:no-console
                    // console.error("XFApp.onReady");
                    // tslint:disable-next-line:no-console
                    console.error(typeof e === "string" ? e : JSON.stringify(e));
                });
            }
        };
        var App_1;
        App = App_1 = __decorate([
            RegisterSingleton_1.RegisterSingleton,
            __metadata("design:paramtypes", [])
        ], App);
        return App;
    }(ServiceProvider_1.ServiceProvider));
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
    var AtomOnce = /** @class */ (function () {
        function AtomOnce() {
        }
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
        AtomOnce.prototype.run = function (f) {
            var _this = this;
            if (this.isRunning) {
                return;
            }
            var isAsync = false;
            try {
                this.isRunning = true;
                var p = f();
                if (p && p.then && p.catch) {
                    isAsync = true;
                    p.then(function () {
                        _this.isRunning = false;
                    }).catch(function () {
                        _this.isRunning = false;
                    });
                }
            }
            finally {
                if (!isAsync) {
                    this.isRunning = false;
                }
            }
        };
        return AtomOnce;
    }());
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
    var AtomBinder_1 = require("./AtomBinder");
    var ExpressionParser_1 = require("./ExpressionParser");
    var ObjectProperty = /** @class */ (function () {
        function ObjectProperty(name) {
            this.name = name;
        }
        ObjectProperty.prototype.toString = function () {
            return this.name;
        };
        return ObjectProperty;
    }());
    exports.ObjectProperty = ObjectProperty;
    /**
     *
     *
     * @export
     * @class AtomWatcher
     * @implements {IDisposable}
     * @template T
     */
    var AtomWatcher = /** @class */ (function () {
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
        function AtomWatcher(target, path, onChanged, source) {
            var _this = this;
            this.source = source;
            this.isExecuting = false;
            this.target = target;
            this.forValidation = true;
            if (path instanceof Function) {
                var f = path;
                path = ExpressionParser_1.parsePath(path);
                this.func = onChanged || f;
                this.funcText = f.toString();
            }
            else {
                this.func = onChanged;
            }
            this.runEvaluate = function () {
                _this.evaluate();
            };
            this.runEvaluate.watcher = this;
            this.path = path.map(function (x) { return x.map(function (y) { return new ObjectProperty(y); }); });
            if (!this.path.length) {
                // tslint:disable-next-line:no-debugger
                debugger;
                // tslint:disable-next-line:no-console
                console.warn("There is nothing to watch, do not use one way binding without any binding expression");
            }
        }
        AtomWatcher.prototype.toString = function () {
            return this.func.toString();
        };
        /**
         * This will dispose and unregister all watchers
         *
         * @memberof AtomWatcher
         */
        AtomWatcher.prototype.dispose = function () {
            if (!this.path) {
                return;
            }
            for (var _i = 0, _a = this.path; _i < _a.length; _i++) {
                var p = _a[_i];
                for (var _b = 0, p_1 = p; _b < p_1.length; _b++) {
                    var op = p_1[_b];
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
        };
        /**
         * Initialize the path targets
         * @param evaluate if true, evaluate entire watch expression and run onChange method
         */
        AtomWatcher.prototype.init = function (evaluate) {
            if (evaluate) {
                this.evaluate(true);
            }
            else {
                for (var _i = 0, _a = this.path; _i < _a.length; _i++) {
                    var iterator = _a[_i];
                    this.evaluatePath(this.target, iterator);
                }
            }
        };
        AtomWatcher.prototype.evaluatePath = function (target, path) {
            // console.log(`\tevaluatePath: ${path.map(op=>op.name).join(", ")}`);
            var newTarget = null;
            for (var _i = 0, path_1 = path; _i < path_1.length; _i++) {
                var p = path_1[_i];
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
        };
        /**
         *
         *
         * @param {boolean} [force]
         * @returns {*}
         * @memberof AtomWatcher
         */
        AtomWatcher.prototype.evaluate = function (force) {
            if (!this.path) {
                // this watcher may have been disposed...
                // tslint:disable-next-line:no-console
                console.warn("Watcher is not disposed properly, please watch for any memory leak");
                return;
            }
            if (this.isExecuting) {
                return;
            }
            var disposeWatchers = [];
            this.isExecuting = true;
            try {
                var values = [];
                var logs = [];
                for (var _i = 0, _a = this.path; _i < _a.length; _i++) {
                    var p = _a[_i];
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
                for (var _b = 0, disposeWatchers_1 = disposeWatchers; _b < disposeWatchers_1.length; _b++) {
                    var d = disposeWatchers_1[_b];
                    d.dispose();
                }
            }
        };
        return AtomWatcher;
    }());
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
    var AtomBridge_1 = require("./AtomBridge");
    var AtomComponent_1 = require("./AtomComponent");
    var AtomOnce_1 = require("./AtomOnce");
    var AtomWatcher_1 = require("./AtomWatcher");
    var PropertyBinding = /** @class */ (function () {
        function PropertyBinding(target, element, name, path, twoWays, valueFunc, source) {
            var _this = this;
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
            this.watcher = new AtomWatcher_1.AtomWatcher(target, path, function () {
                var v = [];
                for (var _i = 0; _i < arguments.length; _i++) {
                    v[_i] = arguments[_i];
                }
                _this.updaterOnce.run(function () {
                    if (_this.disposed) {
                        return;
                    }
                    // set value
                    for (var _i = 0, v_1 = v; _i < v_1.length; _i++) {
                        var iterator = v_1[_i];
                        if (iterator === undefined) {
                            return;
                        }
                    }
                    var cv = _this.fromSourceToTarget ? _this.fromSourceToTarget.apply(_this, v) : v[0];
                    if (_this.target instanceof AtomComponent_1.AtomComponent) {
                        _this.target.setLocalValue(_this.element, _this.name, cv);
                    }
                    else {
                        _this.target[name] = cv;
                    }
                });
            }, source);
            this.path = this.watcher.path;
            if (this.target instanceof AtomComponent_1.AtomComponent) {
                this.target.runAfterInit(function () {
                    if (!_this.watcher) {
                        // this is disposed ...
                        return;
                    }
                    _this.watcher.init(true);
                    if (twoWays) {
                        _this.setupTwoWayBinding();
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
        PropertyBinding.prototype.setupTwoWayBinding = function () {
            var _this = this;
            if (this.target instanceof AtomComponent_1.AtomComponent) {
                if (this.element
                    && (this.element !== this.target.element || !this.target.hasProperty(this.name))) {
                    // most likely it has change event..
                    var events = [];
                    if (typeof this.twoWays !== "boolean") {
                        events = this.twoWays;
                    }
                    this.twoWaysDisposable = AtomBridge_1.AtomBridge.instance.watchProperty(this.element, this.name, events, function (v) {
                        _this.setInverseValue(v);
                    });
                    return;
                }
            }
            var watcher = new AtomWatcher_1.AtomWatcher(this.target, [[this.name]], function () {
                var values = [];
                for (var _i = 0; _i < arguments.length; _i++) {
                    values[_i] = arguments[_i];
                }
                if (_this.isTwoWaySetup) {
                    _this.setInverseValue(values[0]);
                }
            });
            watcher.init(true);
            this.isTwoWaySetup = true;
            this.twoWaysDisposable = watcher;
        };
        PropertyBinding.prototype.setInverseValue = function (value) {
            var _this = this;
            if (!this.twoWays) {
                throw new Error("This Binding is not two ways.");
            }
            this.updaterOnce.run(function () {
                if (_this.disposed) {
                    return;
                }
                var first = _this.path[0];
                var length = first.length;
                var v = _this.target;
                var i = 0;
                var name;
                for (i = 0; i < length - 1; i++) {
                    name = first[i].name;
                    if (name === "this") {
                        v = _this.source || _this.target;
                    }
                    else {
                        v = v[name];
                    }
                    if (!v) {
                        return;
                    }
                }
                name = first[i].name;
                v[name] = _this.fromTargetToSource ? _this.fromTargetToSource.call(_this, value) : value;
            });
        };
        PropertyBinding.prototype.dispose = function () {
            if (this.twoWaysDisposable) {
                this.twoWaysDisposable.dispose();
                this.twoWaysDisposable = null;
            }
            this.watcher.dispose();
            this.disposed = true;
            this.watcher = null;
        };
        return PropertyBinding;
    }());
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
    var AtomDisposableList = /** @class */ (function () {
        function AtomDisposableList() {
            // tslint:disable-next-line:ban-types
            this.disposables = [];
        }
        // tslint:disable-next-line:ban-types
        AtomDisposableList.prototype.add = function (d) {
            var _this = this;
            if (typeof d === "function") {
                var fx = d;
                d = {
                    dispose: fx
                };
            }
            this.disposables.push(d);
            var dx = d;
            return {
                dispose: function () {
                    _this.disposables = _this.disposables.filter(function (x) { return x !== dx; });
                    dx.dispose();
                }
            };
        };
        AtomDisposableList.prototype.dispose = function () {
            for (var _i = 0, _a = this.disposables; _i < _a.length; _i++) {
                var iterator = _a[_i];
                iterator.dispose();
            }
            this.disposables.length = 0;
        };
        return AtomDisposableList;
    }());
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
        define(["require", "exports", "../di/TypeKey"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.PropertyMap = void 0;
    var TypeKey_1 = require("../di/TypeKey");
    var PropertyMap = /** @class */ (function () {
        function PropertyMap() {
        }
        // tslint:disable-next-line:ban-types
        PropertyMap.from = function (o) {
            var c = Object.getPrototypeOf(o);
            var key = TypeKey_1.TypeKey.get(c);
            var map = PropertyMap.map;
            var m = map[key] || (map[key] = PropertyMap.createMap(o));
            return m;
        };
        PropertyMap.createMap = function (c) {
            var map = {};
            var nameList = [];
            while (c) {
                var names = Object.getOwnPropertyNames(c);
                for (var _i = 0, names_1 = names; _i < names_1.length; _i++) {
                    var name_1 = names_1[_i];
                    if (/hasOwnProperty|constructor|toString|isValid|errors/i.test(name_1)) {
                        continue;
                    }
                    // // map[name] = Object.getOwnPropertyDescriptor(c, name) ? true : false;
                    // const pd = Object.getOwnPropertyDescriptor(c, name);
                    // // tslint:disable-next-line:no-console
                    // console.log(`${name} = ${c.enumerable}`);
                    map[name_1] = true;
                    nameList.push(name_1);
                }
                c = Object.getPrototypeOf(c);
            }
            var m = new PropertyMap();
            m.map = map;
            m.names = nameList;
            return m;
        };
        PropertyMap.map = {};
        return PropertyMap;
    }());
    exports.PropertyMap = PropertyMap;
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
        define(["require", "exports", "../App", "../core/AtomBridge", "../core/PropertyBinding", "../core/types", "../di/Inject", "./AtomDisposableList", "./Bind", "./PropertyMap", "./XNode"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomComponent = void 0;
    var App_1 = require("../App");
    var AtomBridge_1 = require("../core/AtomBridge");
    var PropertyBinding_1 = require("../core/PropertyBinding");
    // tslint:disable-next-line:import-spacing
    var types_1 = require("../core/types");
    var Inject_1 = require("../di/Inject");
    var AtomDisposableList_1 = require("./AtomDisposableList");
    var Bind_1 = require("./Bind");
    var PropertyMap_1 = require("./PropertyMap");
    var XNode_1 = require("./XNode");
    var AtomComponent = /** @class */ (function () {
        function AtomComponent(app, element) {
            if (element === void 0) { element = null; }
            this.app = app;
            this.mInvalidated = 0;
            this.mPendingPromises = {};
            this.mData = undefined;
            this.mViewModel = undefined;
            this.mLocalViewModel = undefined;
            this.disposables = new AtomDisposableList_1.AtomDisposableList();
            this.bindings = [];
            this.eventHandlers = [];
            this.element = element;
            AtomBridge_1.AtomBridge.instance.attachControl(this.element, this);
            var a = this.beginEdit();
            this.preCreate();
            this.create();
            app.callLater(function () { return a.dispose(); });
        }
        Object.defineProperty(AtomComponent.prototype, "data", {
            get: function () {
                if (this.mData !== undefined) {
                    return this.mData;
                }
                var parent = this.parent;
                if (parent) {
                    return parent.data;
                }
                return undefined;
            },
            set: function (v) {
                this.mData = v;
                AtomBridge_1.AtomBridge.refreshInherited(this, "data");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomComponent.prototype, "viewModel", {
            get: function () {
                if (this.mViewModel !== undefined) {
                    return this.mViewModel;
                }
                var parent = this.parent;
                if (parent) {
                    return parent.viewModel;
                }
                return undefined;
            },
            set: function (v) {
                var old = this.mViewModel;
                if (old && old.dispose) {
                    old.dispose();
                }
                this.mViewModel = v;
                AtomBridge_1.AtomBridge.refreshInherited(this, "viewModel");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomComponent.prototype, "localViewModel", {
            get: function () {
                if (this.mLocalViewModel !== undefined) {
                    return this.mLocalViewModel;
                }
                var parent = this.parent;
                if (parent) {
                    return parent.localViewModel;
                }
                return undefined;
            },
            set: function (v) {
                var old = this.mLocalViewModel;
                if (old && old.dispose) {
                    old.dispose();
                }
                this.mLocalViewModel = v;
                AtomBridge_1.AtomBridge.refreshInherited(this, "localViewModel");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomComponent.prototype, "vsProps", {
            /** Do not ever use, only available as intellisense feature for
             * vs code editor.
             */
            get: function () {
                return undefined;
            },
            enumerable: false,
            configurable: true
        });
        AtomComponent.prototype.bind = function (element, name, path, twoWays, valueFunc, source) {
            var _this = this;
            // remove existing binding if any
            var binding = this.bindings.find(function (x) { return x.name === name && (element ? x.element === element : true); });
            if (binding) {
                binding.dispose();
                types_1.ArrayHelper.remove(this.bindings, function (x) { return x === binding; });
            }
            binding = new PropertyBinding_1.PropertyBinding(this, element, name, path, twoWays, valueFunc, source);
            this.bindings.push(binding);
            return {
                dispose: function () {
                    binding.dispose();
                    types_1.ArrayHelper.remove(_this.bindings, function (x) { return x === binding; });
                }
            };
        };
        /**
         * Remove all bindings associated with given element and optional name
         * @param element T
         * @param name string
         */
        AtomComponent.prototype.unbind = function (element, name) {
            var toDelete = this.bindings.filter(function (x) { return x.element === element && (!name || (x.name === name)); });
            var _loop_1 = function (iterator) {
                iterator.dispose();
                types_1.ArrayHelper.remove(this_1.bindings, function (x) { return x === iterator; });
            };
            var this_1 = this;
            for (var _i = 0, toDelete_1 = toDelete; _i < toDelete_1.length; _i++) {
                var iterator = toDelete_1[_i];
                _loop_1(iterator);
            }
        };
        AtomComponent.prototype.bindEvent = function (element, name, method, key) {
            var _this = this;
            if (!element) {
                return;
            }
            if (!method) {
                return;
            }
            var be = {
                element: element,
                name: name,
                handler: method
            };
            if (key) {
                be.key = key;
            }
            be.disposable = AtomBridge_1.AtomBridge.instance.addEventHandler(element, name, method, false);
            this.eventHandlers.push(be);
            return {
                dispose: function () {
                    be.disposable.dispose();
                    types_1.ArrayHelper.remove(_this.eventHandlers, function (e) { return e.disposable === be.disposable; });
                }
            };
        };
        AtomComponent.prototype.unbindEvent = function (element, name, method, key) {
            var _this = this;
            var deleted = [];
            var _loop_2 = function (be) {
                if (element && be.element !== element) {
                    return { value: void 0 };
                }
                if (key && be.key !== key) {
                    return { value: void 0 };
                }
                if (name && be.name !== name) {
                    return { value: void 0 };
                }
                if (method && be.handler !== method) {
                    return { value: void 0 };
                }
                be.disposable.dispose();
                be.handler = null;
                be.element = null;
                be.name = null;
                be.key = null;
                deleted.push(function () { return _this.eventHandlers.remove(be); });
            };
            for (var _i = 0, _a = this.eventHandlers; _i < _a.length; _i++) {
                var be = _a[_i];
                var state_1 = _loop_2(be);
                if (typeof state_1 === "object")
                    return state_1.value;
            }
            for (var _b = 0, deleted_1 = deleted; _b < deleted_1.length; _b++) {
                var iterator = deleted_1[_b];
                iterator();
            }
        };
        /**
         * Control checks if property is declared on the control or not.
         * Since TypeScript no longer creates enumerable properties, we have
         * to inspect name and PropertyMap which is generated by `@BindableProperty`
         * or the value is not set to undefined.
         * @param name name of Property
         */
        AtomComponent.prototype.hasProperty = function (name) {
            if (/^(data|viewModel|localViewModel|element)$/.test(name)) {
                return true;
            }
            var map = PropertyMap_1.PropertyMap.from(this);
            if (map.map[name]) {
                return true;
            }
            if (this[name] !== undefined) {
                return true;
            }
            return false;
        };
        /**
         * Use this method if you want to set attribute on HTMLElement immediately but
         * defer atom control property
         * @param element HTMLElement
         * @param name string
         * @param value any
         */
        AtomComponent.prototype.setPrimitiveValue = function (element, name, value) {
            var _this = this;
            var p = value;
            if (p && p.then && p.catch) {
                this.mPendingPromises[name] = p;
                p.then(function (r) {
                    if (_this.mPendingPromises[name] !== p) {
                        return;
                    }
                    _this.mPendingPromises[name] = null;
                    _this.setPrimitiveValue(element, name, r);
                }).catch(function (e) {
                    if (_this.mPendingPromises[name] !== p) {
                        return;
                    }
                    _this.mPendingPromises[name] = null;
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
                this.runAfterInit(function () {
                    _this[name] = value;
                });
            }
            else {
                this.setElementValue(element, name, value);
            }
        };
        AtomComponent.prototype.setLocalValue = function (element, name, value) {
            var _this = this;
            // if value is a promise
            var p = value;
            if (p && p.then && p.catch) {
                this.mPendingPromises[name] = p;
                p.then(function (r) {
                    if (_this.mPendingPromises[name] !== p) {
                        return;
                    }
                    _this.mPendingPromises[name] = null;
                    _this.setLocalValue(element, name, r);
                }).catch(function (e) {
                    if (_this.mPendingPromises[name] !== p) {
                        return;
                    }
                    _this.mPendingPromises[name] = null;
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
        };
        AtomComponent.prototype.dispose = function (e) {
            if (this.mInvalidated) {
                clearTimeout(this.mInvalidated);
                this.mInvalidated = 0;
            }
            AtomBridge_1.AtomBridge.instance.visitDescendents(e || this.element, function (ex, ac) {
                if (ac) {
                    ac.dispose();
                    return false;
                }
                return true;
            });
            if (!e) {
                this.unbindEvent(null, null, null);
                for (var _i = 0, _a = this.bindings; _i < _a.length; _i++) {
                    var binding = _a[_i];
                    binding.dispose();
                }
                this.bindings.length = 0;
                this.bindings = null;
                AtomBridge_1.AtomBridge.instance.dispose(this.element);
                this.element = null;
                var lvm = this.mLocalViewModel;
                if (lvm && lvm.dispose) {
                    lvm.dispose();
                    this.mLocalViewModel = null;
                }
                var vm = this.mViewModel;
                if (vm && vm.dispose) {
                    vm.dispose();
                    this.mViewModel = null;
                }
                this.disposables.dispose();
                this.pendingInits = null;
            }
        };
        // tslint:disable-next-line:no-empty
        AtomComponent.prototype.onPropertyChanged = function (name) {
        };
        AtomComponent.prototype.beginEdit = function () {
            var _this = this;
            this.pendingInits = [];
            var a = this.pendingInits;
            return {
                dispose: function () {
                    if (_this.pendingInits == null) {
                        // case where current control is disposed...
                        return;
                    }
                    _this.pendingInits = null;
                    if (a) {
                        for (var _i = 0, a_1 = a; _i < a_1.length; _i++) {
                            var iterator = a_1[_i];
                            iterator();
                        }
                    }
                    _this.invalidate();
                }
            };
        };
        AtomComponent.prototype.invalidate = function () {
            var _this = this;
            if (this.mInvalidated) {
                clearTimeout(this.mInvalidated);
            }
            this.mInvalidated = setTimeout(function () {
                _this.mInvalidated = 0;
                _this.app.callLater(function () {
                    _this.onUpdateUI();
                });
            }, 5);
        };
        AtomComponent.prototype.onUpdateUI = function () {
            // for implementors..
        };
        AtomComponent.prototype.runAfterInit = function (f) {
            if (this.pendingInits) {
                this.pendingInits.push(f);
            }
            else {
                f();
            }
        };
        AtomComponent.prototype.registerDisposable = function (d) {
            return this.disposables.add(d);
        };
        AtomComponent.prototype.render = function (node, e, creator) {
            creator = creator || this;
            var bridge = AtomBridge_1.AtomBridge.instance;
            var app = this.app;
            var renderFirst = AtomBridge_1.AtomBridge.platform === "xf";
            e = e || this.element;
            var attr = node.attributes;
            if (attr) {
                for (var key in attr) {
                    if (attr.hasOwnProperty(key)) {
                        var item = attr[key];
                        if (item instanceof Bind_1.default) {
                            item.setupFunction(key, item, this, e, creator);
                        }
                        else if (item instanceof XNode_1.default) {
                            // this is template..
                            if (item.isTemplate) {
                                this.setLocalValue(e, key, AtomBridge_1.AtomBridge.toTemplate(app, item, creator));
                            }
                            else {
                                var child = AtomBridge_1.AtomBridge.createNode(item, app);
                                this.setLocalValue(e, key, child.element);
                            }
                        }
                        else {
                            this.setLocalValue(e, key, item);
                        }
                    }
                }
            }
            for (var _i = 0, _a = node.children; _i < _a.length; _i++) {
                var iterator = _a[_i];
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
                    for (var _b = 0, _c = iterator.children; _b < _c.length; _b++) {
                        var child = _c[_b];
                        var pc = AtomBridge_1.AtomBridge.createNode(child, app);
                        (pc.control || this).render(child, pc.element, creator);
                        // in Xamarin.Forms certain properties are required to be
                        // set in advance, so we append the element after setting
                        // all children properties
                        bridge.append(e, iterator.name, pc.element);
                    }
                    continue;
                }
                var t = iterator.attributes && iterator.attributes.template;
                if (t) {
                    this.setLocalValue(e, t, AtomBridge_1.AtomBridge.toTemplate(app, iterator, creator));
                    continue;
                }
                var c = AtomBridge_1.AtomBridge.createNode(iterator, app);
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
        };
        // tslint:disable-next-line:no-empty
        AtomComponent.prototype.create = function () {
        };
        // tslint:disable-next-line:no-empty
        AtomComponent.prototype.preCreate = function () {
        };
        AtomComponent.prototype.setElementValue = function (element, name, value) {
            AtomBridge_1.AtomBridge.instance.setValue(element, name, value);
        };
        AtomComponent.prototype.resolve = function (c, selfName) {
            var result = this.app.resolve(c, true);
            if (selfName) {
                if (typeof selfName === "function") {
                    // this is required as parent is not available
                    // in items control so binding becomes difficult
                    this.runAfterInit(function () {
                        var v = selfName();
                        if (v) {
                            for (var key in v) {
                                if (v.hasOwnProperty(key)) {
                                    var element = v[key];
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
        };
        AtomComponent.isControl = true;
        AtomComponent = __decorate([
            __param(0, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App, Object])
        ], AtomComponent);
        return AtomComponent;
    }());
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
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var FormattedString = /** @class */ (function () {
        function FormattedString(text) {
            this.text = text;
        }
        return FormattedString;
    }());
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
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var WebImage = /** @class */ (function () {
        function WebImage(url) {
            this.url = url;
        }
        WebImage.prototype.toString = function () {
            return this.url;
        };
        return WebImage;
    }());
    exports.default = WebImage;
});
//# sourceMappingURL=WebImage.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/WebImage");

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
    var AtomUI_1 = require("../web/core/AtomUI");
    var AtomUri = /** @class */ (function () {
        /**
         *
         */
        function AtomUri(url) {
            var path;
            var query = "";
            var hash = "";
            var t = url.split("?");
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
            var scheme = "";
            var host = "";
            var port = "";
            var i = path.indexOf("//");
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
        Object.defineProperty(AtomUri.prototype, "pathAndQuery", {
            get: function () {
                var q = [];
                var h = [];
                for (var key in this.query) {
                    if (this.query.hasOwnProperty(key)) {
                        var element = this.query[key];
                        if (element === undefined || element === null) {
                            continue;
                        }
                        q.push(encodeURIComponent(key) + "=" + encodeURIComponent(element.toString()));
                    }
                }
                for (var key in this.hash) {
                    if (this.hash.hasOwnProperty(key)) {
                        var element = this.hash[key];
                        if (element === undefined || element === null) {
                            continue;
                        }
                        h.push(encodeURIComponent(key) + "=" + encodeURIComponent(element.toString()));
                    }
                }
                var query = q.length ? "?" + q.join("&") : "";
                var hash = h.length ? "#" + h.join("&") : "";
                var path = this.path || "/";
                if (path.startsWith("/")) {
                    path = path.substr(1);
                }
                return "" + path + query + hash;
            },
            enumerable: false,
            configurable: true
        });
        AtomUri.prototype.toString = function () {
            var port = this.port ? ":" + this.port : "";
            return this.protocol + "//" + this.host + port + "/" + this.pathAndQuery;
        };
        return AtomUri;
    }());
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
        define(["require", "exports", "./Register", "./ServiceCollection"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Register_1 = require("./Register");
    var ServiceCollection_1 = require("./ServiceCollection");
    function DISingleton(mockOrInject) {
        return function (target) {
            Register_1.Register({ scope: ServiceCollection_1.Scope.Global, mockOrInject: mockOrInject })(target);
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
    var DISingleton_1 = require("../di/DISingleton");
    var ObjectReference = /** @class */ (function () {
        function ObjectReference(key, value) {
            this.key = key;
            this.value = value;
        }
        return ObjectReference;
    }());
    exports.ObjectReference = ObjectReference;
    var ReferenceService = /** @class */ (function () {
        function ReferenceService() {
            this.cache = {};
            this.id = 1;
        }
        ReferenceService.prototype.get = function (key) {
            return this.cache[key];
        };
        ReferenceService.prototype.put = function (item, ttl) {
            var _this = this;
            if (ttl === void 0) { ttl = 60; }
            var key = "k" + this.id++;
            var r = new ObjectReference(key, item);
            r.consume = function () {
                delete _this.cache[key];
                if (r.timeout) {
                    clearTimeout(r.timeout);
                }
                return r.value;
            };
            r.timeout = setTimeout(function () {
                r.timeout = 0;
                r.consume();
            }, ttl * 1000);
            this.cache[key] = r;
            return r;
        };
        ReferenceService = __decorate([
            DISingleton_1.default()
        ], ReferenceService);
        return ReferenceService;
    }());
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var AtomComponent_1 = require("../core/AtomComponent");
    var AtomUri_1 = require("../core/AtomUri");
    var FormattedString_1 = require("../core/FormattedString");
    var types_1 = require("../core/types");
    var ReferenceService_1 = require("./ReferenceService");
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
    var nameSymbol = UMD.nameSymbol;
    function hasPageUrl(target) {
        var url = target[nameSymbol];
        if (!url) {
            return false;
        }
        var baseClass = Object.getPrototypeOf(target);
        if (!baseClass) {
            // this is not possible...
            return false;
        }
        return baseClass[nameSymbol] !== url;
    }
    var NavigationService = /** @class */ (function () {
        function NavigationService(app) {
            this.app = app;
            this.callbacks = [];
        }
        /**
         *
         * @param pageName node style package url or a class
         * @param viewModelParameters key value pair that will be injected on ViewModel when created
         * @param options {@link IPageOptions}
         */
        NavigationService.prototype.openPage = function (pageName, viewModelParameters, options) {
            options = options || {};
            if (typeof pageName !== "string") {
                if (hasPageUrl(pageName)) {
                    pageName = pageName[nameSymbol];
                }
                else {
                    var rs = this.app.resolve(ReferenceService_1.default);
                    var host = pageName instanceof AtomComponent_1.AtomComponent ? "reference" : "class";
                    var r = rs.put(pageName);
                    pageName = "ref://" + host + "/" + r.key;
                }
            }
            var url = new AtomUri_1.AtomUri(pageName);
            if (viewModelParameters) {
                for (var key in viewModelParameters) {
                    if (viewModelParameters.hasOwnProperty(key)) {
                        var element = viewModelParameters[key];
                        if (element === undefined) {
                            continue;
                        }
                        if (element === null) {
                            url.query["json:" + key] = "null";
                            continue;
                        }
                        if (key.startsWith("ref:") || element instanceof FormattedString_1.default) {
                            var r = element instanceof ReferenceService_1.ObjectReference ?
                                element :
                                this.app.resolve(ReferenceService_1.default).put(element);
                            url.query[key.startsWith("ref:") ? key : "ref:" + key] =
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
            for (var _i = 0, _a = this.callbacks; _i < _a.length; _i++) {
                var iterator = _a[_i];
                var r = iterator(url, options);
                if (r) {
                    return r;
                }
            }
            return this.openWindow(url, options);
        };
        /**
         * Sends signal to remove window/popup/frame, it will not immediately remove, because
         * it will identify whether it can remove or not by displaying cancellation warning. Only
         * if there is no cancellation warning or user chooses to force close, it will not remove.
         * @param id id of an element
         * @returns true if view was removed successfully
         */
        NavigationService.prototype.remove = function (view, force) {
            return __awaiter(this, void 0, void 0, function () {
                var vm, a;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (force) {
                                this.app.broadcast("atom-window-cancel:" + view.id, "cancelled");
                                return [2 /*return*/, true];
                            }
                            vm = view.viewModel;
                            if (!(vm && vm.cancel)) return [3 /*break*/, 2];
                            return [4 /*yield*/, vm.cancel()];
                        case 1:
                            a = _a.sent();
                            return [2 /*return*/, a];
                        case 2:
                            this.app.broadcast("atom-window-cancel:" + view.id, "cancelled");
                            return [2 /*return*/, true];
                    }
                });
            });
        };
        NavigationService.prototype.registerNavigationHook = function (callback) {
            var _this = this;
            this.callbacks.push(callback);
            return {
                dispose: function () {
                    types_1.ArrayHelper.remove(_this.callbacks, function (a) { return a === callback; });
                }
            };
        };
        return NavigationService;
    }());
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
    var StringHelper = /** @class */ (function () {
        function StringHelper() {
        }
        StringHelper.fromCamelToHyphen = function (input) {
            return input.replace(/([a-z])([A-Z])/g, "$1-$2").toLowerCase();
        };
        StringHelper.fromCamelToUnderscore = function (input) {
            return input.replace(/([a-z])([A-Z])/g, "$1_$2").toLowerCase();
        };
        StringHelper.fromCamelToPascal = function (input) {
            return input[0].toUpperCase() + input.substr(1);
        };
        StringHelper.fromHyphenToCamel = function (input) {
            return input.replace(/-([a-z])/g, function (g) { return g[1].toUpperCase(); });
        };
        StringHelper.fromUnderscoreToCamel = function (input) {
            return input.replace(/\_([a-z])/g, function (g) { return g[1].toUpperCase(); });
        };
        StringHelper.fromPascalToCamel = function (input) {
            return input[0].toLowerCase() + input.substr(1);
        };
        return StringHelper;
    }());
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
    var StringHelper_1 = require("../../core/StringHelper");
    var emptyPrototype = Object.getPrototypeOf({});
    var AtomStyle = /** @class */ (function () {
        function AtomStyle(styleSheet, name) {
            this.styleSheet = styleSheet;
            this.name = name;
            this.styleText = null;
            this.name = this.name + "-root";
        }
        AtomStyle.prototype.getBaseProperty = function (tc, name) {
            var c = tc;
            do {
                c = Object.getPrototypeOf(c);
                if (!c) {
                    throw new Error("No property descriptor found for " + name);
                }
                var pd = Object.getOwnPropertyDescriptor(c.prototype, name);
                if (!pd) {
                    continue;
                }
                return pd.get.apply(this);
            } while (true);
        };
        AtomStyle.prototype.build = function () {
            if (this.styleText) {
                return;
            }
            this.styleText = this.createStyleText("", [], this.root).join("\n");
        };
        AtomStyle.prototype.toString = function () {
            return this.styleText;
        };
        AtomStyle.prototype.createStyleText = function (name, pairs, styles) {
            var styleList = [];
            for (var key in styles) {
                if (styles.hasOwnProperty(key)) {
                    if (/^(\_\$\_|className$|toString$)/i.test(key)) {
                        continue;
                    }
                    var element = styles[key];
                    if (element === undefined || element === null) {
                        continue;
                    }
                    var keyName = StringHelper_1.StringHelper.fromCamelToHyphen(key);
                    if (key === "subclasses") {
                        var n = name;
                        for (var subclassKey in element) {
                            if (element.hasOwnProperty(subclassKey)) {
                                var ve = element[subclassKey];
                                pairs = this.createStyleText("" + n + subclassKey, pairs, ve);
                            }
                        }
                    }
                    else {
                        if (element.url) {
                            styleList.push(keyName + ": url(" + element + ")");
                        }
                        else {
                            styleList.push(keyName + ": " + element);
                        }
                    }
                }
            }
            var cname = StringHelper_1.StringHelper.fromCamelToHyphen(name);
            var styleClassName = "" + this.name + cname;
            if (styleList.length) {
                pairs.push("." + styleClassName + " { " + styleList.join(";\r\n") + "; }");
            }
            return pairs;
        };
        return AtomStyle;
    }());
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
    var TypeKey_1 = require("../../di/TypeKey");
    var AtomStyleSheet = /** @class */ (function () {
        function AtomStyleSheet(app, name) {
            this.app = app;
            this.name = name;
            this.styles = {};
            this.lastUpdateId = 0;
            this.isAttaching = false;
            this.pushUpdate(0);
        }
        AtomStyleSheet.prototype.getNamedStyle = function (c) {
            var name = TypeKey_1.TypeKey.get(c);
            return this.createNamedStyle(c, name);
        };
        AtomStyleSheet.prototype.createNamedStyle = function (c, name, updateTimeout) {
            var style = this.styles[name] = new (c)(this, this.name + "-" + name);
            style.build();
            this.pushUpdate(updateTimeout);
            return style;
        };
        AtomStyleSheet.prototype.onPropertyChanging = function (name, newValue, oldValue) {
            this.pushUpdate();
        };
        AtomStyleSheet.prototype.pushUpdate = function (delay) {
            var _this = this;
            if (delay === void 0) { delay = 1; }
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
            this.lastUpdateId = setTimeout(function () {
                _this.attach();
            }, delay);
        };
        AtomStyleSheet.prototype.dispose = function () {
            if (this.styleElement) {
                this.styleElement.remove();
            }
        };
        AtomStyleSheet.prototype.attach = function () {
            this.isAttaching = true;
            var text = [];
            for (var key in this.styles) {
                if (this.styles.hasOwnProperty(key)) {
                    var element = this.styles[key];
                    text.push(element.toString());
                }
            }
            var textContent = text.join("\n");
            this.app.updateDefaultStyle(textContent);
            this.isAttaching = false;
        };
        return AtomStyleSheet;
    }());
    exports.AtomStyleSheet = AtomStyleSheet;
});
//# sourceMappingURL=AtomStyleSheet.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomStyleSheet");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/AtomBinder", "../../core/AtomBridge", "../../core/AtomComponent", "../../core/FormattedString", "../../core/WebImage", "../../di/TypeKey", "../../services/NavigationService", "../styles/AtomStyle", "../styles/AtomStyleSheet"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomControl = void 0;
    var AtomBinder_1 = require("../../core/AtomBinder");
    var AtomBridge_1 = require("../../core/AtomBridge");
    var AtomComponent_1 = require("../../core/AtomComponent");
    var FormattedString_1 = require("../../core/FormattedString");
    var WebImage_1 = require("../../core/WebImage");
    var TypeKey_1 = require("../../di/TypeKey");
    var NavigationService_1 = require("../../services/NavigationService");
    var AtomStyle_1 = require("../styles/AtomStyle");
    var AtomStyleSheet_1 = require("../styles/AtomStyleSheet");
    // export { default as WebApp } from "../WebApp";
    // if (!AtomBridge.platform) {
    //     AtomBridge.platform = "web";
    //     AtomBridge.instance = new AtomElementBridge();
    // } else {
    //     console.log(`Platform is ${AtomBridge.platform}`);
    // }
    var bridge = AtomBridge_1.AtomBridge.instance;
    var defaultStyleSheets = {};
    /**
     * AtomControl class represents UI Component for a web browser.
     */
    var AtomControl = /** @class */ (function (_super) {
        __extends(AtomControl, _super);
        function AtomControl(app, e) {
            return _super.call(this, app, e || document.createElement("div")) || this;
        }
        Object.defineProperty(AtomControl.prototype, "controlStyle", {
            get: function () {
                if (this.mControlStyle === undefined) {
                    var key = TypeKey_1.TypeKey.get(this.defaultControlStyle || this.constructor);
                    this.mControlStyle = defaultStyleSheets[key];
                    if (this.mControlStyle) {
                        return this.mControlStyle;
                    }
                    if (this.defaultControlStyle) {
                        this.mControlStyle = defaultStyleSheets[key] ||
                            (defaultStyleSheets[key] = this.theme.createNamedStyle(this.defaultControlStyle, key));
                    }
                    this.mControlStyle = this.mControlStyle || null;
                }
                return this.mControlStyle;
            },
            set: function (v) {
                if (v instanceof AtomStyle_1.AtomStyle) {
                    this.mControlStyle = v;
                }
                else {
                    var key = TypeKey_1.TypeKey.get(v);
                    this.mControlStyle = defaultStyleSheets[key] ||
                        (defaultStyleSheets[key] = this.theme.createNamedStyle(v, key));
                }
                AtomBinder_1.AtomBinder.refreshValue(this, "controlStyle");
                this.invalidate();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomControl.prototype, "theme", {
            /**
             * Represents associated AtomStyleSheet with this visual hierarchy. AtomStyleSheet is
             * inherited by default.
             */
            get: function () {
                return this.mTheme ||
                    this.mCachedTheme ||
                    (this.mCachedTheme = (this.parent ? this.parent.theme : this.app.resolve(AtomStyleSheet_1.AtomStyleSheet, false, null)));
            },
            set: function (v) {
                this.mTheme = v;
                bridge.refreshInherited(this, "theme");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomControl.prototype, "parent", {
            /**
             * Gets Parent AtomControl of this control.
             */
            get: function () {
                var ep = this.element._logicalParent || this.element.parentElement;
                if (!ep) {
                    return null;
                }
                return this.atomParent(ep);
            },
            enumerable: false,
            configurable: true
        });
        AtomControl.prototype.onPropertyChanged = function (name) {
            _super.prototype.onPropertyChanged.call(this, name);
            switch (name) {
                case "theme":
                    this.mCachedTheme = null;
                    AtomBinder_1.AtomBinder.refreshValue(this, "style");
                    break;
            }
        };
        AtomControl.prototype.atomParent = function (e) {
            if (!e) {
                return;
            }
            var ep = e;
            if (ep.atomControl) {
                return ep.atomControl;
            }
            return this.atomParent(ep._logicalParent || ep.parentElement);
        };
        AtomControl.prototype.append = function (element) {
            if (element instanceof AtomControl) {
                this.element.appendChild(element.element);
            }
            else {
                this.element.appendChild(element);
            }
            return this;
        };
        AtomControl.prototype.updateSize = function () {
            this.onUpdateSize();
            bridge.visitDescendents(this.element, function (e, ac) {
                if (ac) {
                    ac.updateSize();
                    return false;
                }
                return true;
            });
        };
        AtomControl.prototype.preCreate = function () {
            // if (!this.element) {
            //     this.element = document.createElement("div");
            // }
        };
        AtomControl.prototype.setElementValue = function (element, name, value) {
            var _this = this;
            if (value === undefined) {
                return;
            }
            if (/^style/.test(name)) {
                if (name.length === 5) {
                    element.setAttribute("style", value);
                    return;
                }
                name = name.substr(5);
                name = name.charAt(0).toLowerCase() + name.substr(1);
                // this is style class...
                if (name === "class") {
                    this.setElementClass(element, value);
                    return;
                }
                if (value instanceof WebImage_1.default) {
                    value = "url(" + value + ")";
                }
                element.style[name] = value;
                return;
            }
            if (/^event/.test(name)) {
                name = name.substr(5);
                name = name.charAt(0).toLowerCase() + name.substr(1);
                this.bindEvent(element, name, function () {
                    var e = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        e[_i] = arguments[_i];
                    }
                    return __awaiter(_this, void 0, void 0, function () {
                        var f, pr, error_1, nav, er1_1;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0:
                                    _a.trys.push([0, 6, , 7]);
                                    f = value;
                                    pr = f.apply(this, e);
                                    if (!pr) return [3 /*break*/, 5];
                                    _a.label = 1;
                                case 1:
                                    _a.trys.push([1, 3, , 5]);
                                    return [4 /*yield*/, pr];
                                case 2:
                                    _a.sent();
                                    return [3 /*break*/, 5];
                                case 3:
                                    error_1 = _a.sent();
                                    if (/canceled|cancelled/i.test(error_1)) {
                                        return [2 /*return*/];
                                    }
                                    nav = this.app.resolve(NavigationService_1.NavigationService);
                                    return [4 /*yield*/, nav.alert(error_1, "Error")];
                                case 4:
                                    _a.sent();
                                    return [3 /*break*/, 5];
                                case 5: return [3 /*break*/, 7];
                                case 6:
                                    er1_1 = _a.sent();
                                    // tslint:disable-next-line:no-console
                                    console.error(er1_1);
                                    return [3 /*break*/, 7];
                                case 7: return [2 /*return*/];
                            }
                        });
                    });
                });
                return;
            }
            switch (name) {
                case "text":
                    element.textContent = value;
                    break;
                case "formattedText":
                    if (value instanceof FormattedString_1.default) {
                        value.applyTo(this.app, element);
                    }
                    else {
                        element.textContent = (value || "").toString();
                    }
                    break;
                case "class":
                    this.setElementClass(element, value, true);
                    break;
                case "autofocus":
                    this.app.callLater(function () {
                        var ie = element;
                        if (ie) {
                            ie.focus();
                        }
                    });
                case "src":
                    if (value && /^http\:/i.test(value)) {
                        element.src = value.substr(5);
                    }
                    else {
                        element.src = value;
                    }
                    break;
                default:
                    element[name] = value;
            }
        };
        AtomControl.prototype.setElementClass = function (element, value, clear) {
            var s = value;
            if (s && typeof s === "object") {
                if (!s.className) {
                    if (clear) {
                        var sr = "";
                        for (var key in s) {
                            if (s.hasOwnProperty(key)) {
                                var sv = s[key];
                                if (sv) {
                                    sr += (sr ? (" " + key) : key);
                                }
                            }
                        }
                        element.className = sr;
                        return;
                    }
                    for (var key in s) {
                        if (s.hasOwnProperty(key)) {
                            var sv = s[key];
                            if (sv) {
                                if (!element.classList.contains(key)) {
                                    element.classList.add(key);
                                }
                            }
                            else {
                                if (element.classList.contains(key)) {
                                    element.classList.remove(key);
                                }
                            }
                        }
                    }
                    return;
                }
            }
            var sv1 = s ? (s.className || s.toString()) : "";
            element.className = sv1;
        };
        AtomControl.prototype.onUpdateSize = function () {
            // pending !!
        };
        AtomControl.prototype.removeAllChildren = function (e) {
            var child = e.firstElementChild;
            while (child) {
                var c = child;
                child = child.nextElementSibling;
                var ac = c;
                if (ac && ac.atomControl) {
                    ac.atomControl.dispose();
                }
                else {
                    // remove all children events
                    this.unbindEvent(child);
                    // remove all bindings
                    this.unbind(child);
                }
                c.remove();
            }
        };
        return AtomControl;
    }(AtomComponent_1.AtomComponent));
    exports.AtomControl = AtomControl;
    bridge.controlFactory = AtomControl;
});
//# sourceMappingURL=AtomControl.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomControl");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/AtomBridge", "./AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomGridView = void 0;
    var AtomBridge_1 = require("../../core/AtomBridge");
    var AtomControl_1 = require("./AtomControl");
    /**
     * GridView columns or rows can accept comma separated strings with
     * absolute pixel value, percent value and star (*).
     *
     * For example, 20% of total width for first column, 200 pixel for last column
     * and rest of the space is for middle = "20%, *, 200"
     *
     * You can have only one star specification.
     * @example
     *  <AtomGridView
     *     rows="50,*"
     *     columns="20%, 5, *, 200">
     *
     *      <!-- Header spans for three columns in first row -->
     *      <header row="0" column="0:3"></header>
     *
     *      <!-- menu is on first column -->
     *      <menu row="1" column="0"></menu>
     *
     *      <!-- Grid splitter splits 1st and 3rd column and itself lies in 2nd column -->
     *      <AtomGridSplitter row="1" column="1" direction="vertical" />
     *
     *      <!-- Section fills remaining area -->
     *      <section row="1" column="2"></section>
     *
     *      <!-- Help sits on last column -->
     *      <Help row="1" column="3"></Help>
     *  </AtomGridView>
     */
    var AtomGridView = /** @class */ (function (_super) {
        __extends(AtomGridView, _super);
        function AtomGridView() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.attempt = 0;
            _this.availableRect = null;
            _this.childrenReady = false;
            return _this;
        }
        AtomGridView.getCellInfo = function (e) {
            var row = 0;
            var column = 0;
            var rowSpan = 1;
            var colSpan = 1;
            var cell = e.cell;
            if (cell) {
                // tslint:disable-next-line:no-console
                console.warn("Attribute `cell` is obsolete, please use row and column attributes separately");
                var tokens = cell.split(",")
                    .map(function (s) { return s.trim().split(":").map(function (st) { return parseInt(st.trim(), 10); }); });
                column = tokens[0][0];
                row = tokens[1][0];
                colSpan = tokens[0][1] || 1;
                rowSpan = tokens[1][1] || 1;
            }
            else {
                var c = ((e.row) || "0");
                var tokens = c.split(":").map(function (st) { return parseInt(st.trim(), 10); });
                row = tokens[0];
                rowSpan = tokens[1] || 1;
                c = ((e.column) || "0");
                tokens = c.split(":").map(function (st) { return parseInt(st.trim(), 10); });
                column = tokens[0];
                colSpan = tokens[1] || 1;
            }
            return {
                row: row,
                rowSpan: rowSpan,
                column: column,
                colSpan: colSpan,
            };
        };
        AtomGridView.prototype.append = function (e) {
            var ee = e instanceof AtomControl_1.AtomControl ? e.element : e;
            ee._logicalParent = this.element;
            this.children = this.children || [];
            this.children.push(e instanceof AtomControl_1.AtomControl ? e.element : e);
            return this;
        };
        AtomGridView.prototype.onUpdateUI = function () {
            var _this = this;
            this.attempt++;
            // this.removeAllChildren(this.element);
            var child = this.element.firstElementChild;
            while (child) {
                var c = child;
                child = child.nextElementSibling;
                c.remove();
            }
            var width = this.element.offsetWidth ||
                this.element.clientWidth ||
                parseFloat(this.element.style.width) ||
                0;
            var height = this.element.offsetHeight ||
                this.element.clientHeight ||
                parseFloat(this.element.style.height) ||
                0;
            if (!(width && height)) {
                if (this.childrenReady) {
                    // this is the time parent is hidden
                    setTimeout(function () {
                        _this.invalidate();
                    }, 5000);
                    return;
                }
                if (this.attempt > 100) {
                    // tslint:disable-next-line:no-console
                    console.error("AtomDockPanel (" + width + ", " + height + ") must both have non zero width and height");
                    return;
                }
                // AtomDispatcher.instance.callLater(() => this.invalidate());
                setTimeout(function () {
                    _this.invalidate();
                }, 100);
                return;
            }
            if (!this.children) {
                return;
            }
            this.attempt = 0;
            this.availableRect = { width: width, height: height, x: 0, y: 0 };
            this.columnSizes = (this.columns || "*").split(",")
                .map(function (s) { return _this.toSize(s.trim(), _this.availableRect.width); });
            this.rowSizes = (this.rows || "*").split(",")
                .map(function (s) { return _this.toSize(s.trim(), _this.availableRect.height); });
            this.assignOffsets(this.columnSizes, this.availableRect.width);
            this.assignOffsets(this.rowSizes, this.availableRect.height);
            for (var _i = 0, _a = this.children; _i < _a.length; _i++) {
                var iterator = _a[_i];
                var host = document.createElement("section");
                host.appendChild(iterator);
                this.element.appendChild(host);
            }
            _super.prototype.onUpdateUI.call(this);
            this.updateSize();
            this.childrenReady = true;
        };
        AtomGridView.prototype.resize = function (item, index, delta) {
            var a = item === "column" ? this.columnSizes : this.rowSizes;
            var prev = a[index - 1];
            var next = a[index + 1];
            if ((!prev) || (!next)) {
                throw new Error("Grid Splitter cannot be start or end element in GridView");
            }
            var current = a[index];
            prev.size += delta;
            current.offset += delta;
            next.offset += delta;
            next.size -= delta;
            this.updateSize();
        };
        AtomGridView.prototype.onPropertyChanged = function (name) {
            switch (name) {
                case "rows":
                case "columns":
                    if (this.childrenReady) {
                        this.invalidate();
                    }
                    break;
            }
        };
        AtomGridView.prototype.onUpdateSize = function () {
            if (!this.children) {
                return;
            }
            for (var _i = 0, _a = this.children; _i < _a.length; _i++) {
                var iterator = _a[_i];
                this.updateStyle(iterator);
            }
        };
        AtomGridView.prototype.preCreate = function () {
            var _this = this;
            this.columns = null;
            this.rows = null;
            var style = this.element.style;
            style.position = "absolute";
            style.left = style.right = style.top = style.bottom = "0";
            style.overflow = "hidden";
            this.bindEvent(window, "resize", function () {
                _this.updateSize();
            });
            this.bindEvent(document.body, "resize", function () {
                _this.updateSize();
            });
        };
        AtomGridView.prototype.updateStyle = function (e) {
            var _a = AtomGridView.getCellInfo(e), colSpan = _a.colSpan, column = _a.column, row = _a.row, rowSpan = _a.rowSpan;
            var host = e.parentElement;
            if (!host) {
                return;
            }
            host.style.position = "absolute";
            host.style.overflow = "hidden";
            host.style.padding = "0";
            host.style.margin = "0";
            if (this.rowSizes.length <= row || this.columnSizes.length <= column) {
                return;
            }
            var rowStart = this.rowSizes[row].offset;
            var rowSize = 0;
            for (var i = row; i < row + rowSpan; i++) {
                rowSize += this.rowSizes[i].size;
            }
            host.style.top = rowStart + "px";
            host.style.height = rowSize + "px";
            var colStart = this.columnSizes[column].offset;
            var colSize = 0;
            for (var i = column; i < column + colSpan; i++) {
                colSize += this.columnSizes[i].size;
            }
            host.style.left = colStart + "px";
            host.style.width = colSize + "px";
            AtomBridge_1.AtomBridge.instance.visitDescendents(host, function (el, ac) {
                if (ac) {
                    ac.invalidate();
                    return false;
                }
                return true;
            });
        };
        AtomGridView.prototype.toSize = function (s, total) {
            if (!s || s === "*") {
                return { offset: -1, size: NaN };
            }
            var n = 0;
            if (s.endsWith("%")) {
                s = s.substr(0, s.length - 1);
                n = parseFloat(s);
                return { offset: -1, size: total * n / 100 };
            }
            return { offset: -1, size: parseFloat(s) };
        };
        AtomGridView.prototype.assignOffsets = function (a, end) {
            var start = 0;
            var fill = null;
            for (var _i = 0, a_1 = a; _i < a_1.length; _i++) {
                var item = a_1[_i];
                item.offset = start;
                if (isNaN(item.size)) {
                    fill = item;
                    break;
                }
                start += item.size;
            }
            if (!fill) {
                return;
            }
            var lastStart = start;
            start = end;
            var r = a.map(function (x) { return x; }).reverse();
            for (var _a = 0, r_1 = r; _a < r_1.length; _a++) {
                var item = r_1[_a];
                if (isNaN(item.size)) {
                    if (fill !== item) {
                        throw new Error("Multiple * cannot be defined");
                    }
                    break;
                }
                start -= item.size;
                item.offset = start;
            }
            fill.offset = lastStart;
            fill.size = start - lastStart;
        };
        return AtomGridView;
    }(AtomControl_1.AtomControl));
    exports.AtomGridView = AtomGridView;
});
//# sourceMappingURL=AtomGridView.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomGridView");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomControl", "./AtomGridView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomGridSplitter = void 0;
    var AtomControl_1 = require("./AtomControl");
    var AtomGridView_1 = require("./AtomGridView");
    /**
     * Grid Splitter can only be added inside a Grid
     */
    var AtomGridSplitter = /** @class */ (function (_super) {
        __extends(AtomGridSplitter, _super);
        function AtomGridSplitter() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.dragging = false;
            return _this;
        }
        AtomGridSplitter.prototype.preCreate = function () {
            this.direction = "vertical";
            this.dragging = false;
        };
        AtomGridSplitter.prototype.create = function () {
            var _this = this;
            this.bind(this.element, "styleCursor", [["direction"]], false, function (v) { return v === "vertical" ? "ew-resize" : "ns-resize"; });
            this.bind(this.element, "styleBackgroundColor", [["dragging"]], false, function (v) { return v ? "blue" : "lightgray"; });
            var style = this.element.style;
            style.position = "absolute";
            style.left = style.top = style.bottom = style.right = "0";
            this.bindEvent(this.element, "mousedown", function (e) {
                e.preventDefault();
                _this.dragging = true;
                var parent = _this.parent;
                var isVertical = _this.direction === "vertical";
                var disposables = [];
                var rect = { x: e.screenX, y: e.screenY };
                var _a = AtomGridView_1.AtomGridView.getCellInfo(_this.element), column = _a.column, row = _a.row;
                var ss = document.createElement("style");
                ss.textContent = "iframe { pointer-events: none }";
                document.head.appendChild(ss);
                disposables.push({
                    dispose: function () { return ss.remove(); }
                });
                disposables.push(_this.bindEvent(document.body, "mousemove", function (me) {
                    // do drag....
                    var screenX = me.screenX, screenY = me.screenY;
                    var dx = screenX - rect.x;
                    var dy = screenY - rect.y;
                    if (isVertical) {
                        parent.resize("column", column, dx);
                    }
                    else {
                        parent.resize("row", row, dy);
                    }
                    rect.x = screenX;
                    rect.y = screenY;
                }));
                disposables.push(_this.bindEvent(document.body, "mouseup", function (mup) {
                    // stop
                    _this.dragging = false;
                    for (var _i = 0, disposables_1 = disposables; _i < disposables_1.length; _i++) {
                        var iterator = disposables_1[_i];
                        iterator.dispose();
                    }
                }));
            });
        };
        return AtomGridSplitter;
    }(AtomControl_1.AtomControl));
    exports.AtomGridSplitter = AtomGridSplitter;
});
//# sourceMappingURL=AtomGridSplitter.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomGridSplitter");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomListBoxStyle = void 0;
    var AtomStyle_1 = require("./AtomStyle");
    var AtomListBoxStyle = /** @class */ (function (_super) {
        __extends(AtomListBoxStyle, _super);
        function AtomListBoxStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomListBoxStyle.prototype, "root", {
            get: function () {
                return {
                    subclasses: {
                        " .item": this.item,
                        " .selected-item": this.selectedItem
                    }
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomListBoxStyle.prototype, "theme", {
            get: function () {
                return this.styleSheet;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomListBoxStyle.prototype, "item", {
            get: function () {
                return {
                    backgroundColor: this.theme.bgColor,
                    color: this.theme.color,
                    padding: (this.padding || this.theme.padding) + "px",
                    borderRadius: (this.padding || this.theme.padding) + "px",
                    cursor: "pointer"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomListBoxStyle.prototype, "selectedItem", {
            get: function () {
                return __assign(__assign({}, this.item), { backgroundColor: this.theme.selectedBgColor, color: this.theme.selectedColor, cursor: "pointer" });
            },
            enumerable: false,
            configurable: true
        });
        return AtomListBoxStyle;
    }(AtomStyle_1.AtomStyle));
    exports.AtomListBoxStyle = AtomListBoxStyle;
});
//# sourceMappingURL=AtomListBoxStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomListBoxStyle");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomListBoxStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomToggleButtonBarStyle = void 0;
    var AtomListBoxStyle_1 = require("./AtomListBoxStyle");
    var AtomToggleButtonBarStyle = /** @class */ (function (_super) {
        __extends(AtomToggleButtonBarStyle, _super);
        function AtomToggleButtonBarStyle() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.toggleColor = "blue";
            return _this;
        }
        Object.defineProperty(AtomToggleButtonBarStyle.prototype, "root", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(AtomToggleButtonBarStyle, "root")), { display: "inline-block", paddingInlineStart: 0 });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomToggleButtonBarStyle.prototype, "item", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(AtomToggleButtonBarStyle, "item")), { borderRadius: 0, display: "inline-block", border: "1px solid", borderLeft: "none", color: this.toggleColor, borderColor: this.toggleColor, cursor: "pointer", subclasses: {
                        ":first-child": {
                            borderTopLeftRadius: (this.padding || this.theme.padding) + "px",
                            borderBottomLeftRadius: (this.padding || this.theme.padding) + "px",
                            borderTopRightRadius: 0,
                            borderBottomRightRadius: 0,
                            borderLeft: "1px solid"
                        },
                        ":last-child": {
                            borderTopLeftRadius: 0,
                            borderBottomLeftRadius: 0,
                            borderTopRightRadius: (this.padding || this.theme.padding) + "px",
                            borderBottomRightRadius: (this.padding || this.theme.padding) + "px"
                        }
                    } });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomToggleButtonBarStyle.prototype, "selectedItem", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(AtomToggleButtonBarStyle, "selectedItem")), { borderRadius: 0, display: "inline-block", border: "1px solid", borderLeft: "none", borderColor: this.toggleColor, cursor: "pointer", subclasses: {
                        ":first-child": {
                            borderTopLeftRadius: (this.padding || this.theme.padding) + "px",
                            borderBottomLeftRadius: (this.padding || this.theme.padding) + "px",
                            borderTopRightRadius: 0,
                            borderBottomRightRadius: 0
                        },
                        ":last-child": {
                            borderTopLeftRadius: 0,
                            borderBottomLeftRadius: 0,
                            borderTopRightRadius: (this.padding || this.theme.padding) + "px",
                            borderBottomRightRadius: (this.padding || this.theme.padding) + "px"
                        }
                    } });
            },
            enumerable: false,
            configurable: true
        });
        return AtomToggleButtonBarStyle;
    }(AtomListBoxStyle_1.AtomListBoxStyle));
    exports.AtomToggleButtonBarStyle = AtomToggleButtonBarStyle;
});
//# sourceMappingURL=AtomToggleButtonBarStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomToggleButtonBarStyle");

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
    var AtomEnumerator = /** @class */ (function () {
        function AtomEnumerator(items) {
            this.items = items;
            this.index = -1;
        }
        AtomEnumerator.prototype.next = function () {
            this.index++;
            return this.index < this.items.length;
        };
        Object.defineProperty(AtomEnumerator.prototype, "current", {
            get: function () {
                return this.items[this.index];
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomEnumerator.prototype, "currentIndex", {
            get: function () {
                return this.index;
            },
            enumerable: false,
            configurable: true
        });
        return AtomEnumerator;
    }());
    exports.default = AtomEnumerator;
});
//# sourceMappingURL=AtomEnumerator.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomEnumerator");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/AtomBinder", "../../core/AtomEnumerator", "../../core/AtomList", "../../core/XNode", "../../web/core/AtomUI", "./AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomItemsControl = void 0;
    var AtomBinder_1 = require("../../core/AtomBinder");
    var AtomEnumerator_1 = require("../../core/AtomEnumerator");
    require("../../core/AtomList");
    var XNode_1 = require("../../core/XNode");
    var AtomUI_1 = require("../../web/core/AtomUI");
    var AtomControl_1 = require("./AtomControl");
    var AtomItemsControl = /** @class */ (function (_super) {
        __extends(AtomItemsControl, _super);
        function AtomItemsControl() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.mValue = undefined;
            // private mFilteredItems: any[] = [];
            // private mSelectedItem: any = undefined;
            _this.mFilter = undefined;
            _this.mFirstChild = null;
            _this.mLastChild = null;
            _this.mScrollerSetup = false;
            _this.mScopes = null;
            _this.mItemsDisposable = null;
            _this.isUpdating = false;
            return _this;
        }
        Object.defineProperty(AtomItemsControl.prototype, "itemsPresenter", {
            get: function () {
                return this.mItemsPresenter || (this.mItemsPresenter = this.element);
            },
            set: function (v) {
                this.mItemsPresenter = v;
                AtomBinder_1.AtomBinder.refreshValue(this, "itemsPresenter");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomItemsControl.prototype, "value", {
            get: function () {
                var _this = this;
                if (this.allowMultipleSelection) {
                    var items = this.mSelectedItems;
                    if (items.length === 0) {
                        if (this.mValue !== undefined) {
                            return this.mValue;
                        }
                        return null;
                    }
                    items = items.map(function (m) { return m[_this.valuePath]; });
                    if (this.valueSeparator) {
                        items = items.join(this.valueSeparator);
                    }
                    return items;
                }
                var s = this.selectedItem;
                if (!s) {
                    if (this.mValue !== undefined) {
                        return this.mValue;
                    }
                    return null;
                }
                if (this.valuePath) {
                    s = s[this.valuePath];
                }
                return s;
            },
            set: function (v) {
                this.mValue = v;
                var dataItems = this.items;
                if (!dataItems) {
                    return;
                }
                var sItems = this.selectedItems;
                if (v === undefined || v === null) {
                    // reset...
                    AtomBinder_1.AtomBinder.clear(sItems);
                    return;
                }
                if (this.allowMultipleSelection && this.valueSeparator) {
                    if (typeof v !== "string") {
                        v = "" + v;
                    }
                    v = v.split(this.valueSeparator);
                }
                else {
                    v = [v];
                }
                // const items = AtomArray.intersect(dataItems, this._valuePath, v);
                sItems.length = 0;
                var vp = this.valuePath;
                for (var _i = 0, v_1 = v; _i < v_1.length; _i++) {
                    var item = v_1[_i];
                    // tslint:disable-next-line:triple-equals
                    var dataItem = dataItems.find(function (i) { return i[vp] == v; });
                    if (dataItem) {
                        sItems.push(dataItem);
                    }
                }
                // this.updateSelectionBindings();
                AtomBinder_1.AtomBinder.refreshItems(sItems);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomItemsControl.prototype, "items", {
            get: function () {
                return this.mItems;
            },
            set: function (v) {
                var _this = this;
                if (this.mItemsDisposable) {
                    this.mItemsDisposable.dispose();
                    this.mItemsDisposable = null;
                }
                this.mItems = v;
                // this.mFilteredItems = null;
                if (v != null) {
                    this.mItemsDisposable = this.registerDisposable(AtomBinder_1.AtomBinder.add_CollectionChanged(v, function (target, key, index, item) {
                        _this.onCollectionChangedInternal(key, index, item);
                    }));
                    // this.onCollectionChangedInternal("refresh", -1, null);
                }
                AtomBinder_1.AtomBinder.refreshValue(this, "items");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomItemsControl.prototype, "selectedItem", {
            get: function () {
                if (this.selectedItems.length > 0) {
                    return this.selectedItems[0];
                }
                return null;
            },
            set: function (value) {
                if (value !== undefined && value !== null) {
                    this.mSelectedItems.length = 1;
                    this.mSelectedItems[0] = value;
                }
                else {
                    this.mSelectedItems.length = 0;
                }
                AtomBinder_1.AtomBinder.refreshItems(this.mSelectedItems);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomItemsControl.prototype, "selectedItems", {
            get: function () {
                return this.mSelectedItems || (this.selectedItems = []);
            },
            set: function (v) {
                var _this = this;
                if (this.mSelectedItemsWatcher) {
                    this.mSelectedItemsWatcher.dispose();
                    this.mSelectedItemsWatcher = null;
                }
                this.mSelectedItems = v;
                if (v) {
                    this.mSelectedItemsWatcher = this.registerDisposable(AtomBinder_1.AtomBinder.add_CollectionChanged(v, function (t, k, i, item) {
                        _this.onSelectedItemsChanged(k, i, item);
                    }));
                }
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomItemsControl.prototype, "selectedIndex", {
            get: function () {
                if (!this.mItems) {
                    return -1;
                }
                var item = this.selectedItem;
                return this.mItems.indexOf(item);
            },
            set: function (n) {
                if (!this.mItems) {
                    return;
                }
                if (n <= -1 || n >= this.mItems.length) {
                    this.selectedItem = null;
                    return;
                }
                this.selectedItem = this.mItems[n];
            },
            enumerable: false,
            configurable: true
        });
        AtomItemsControl.prototype.hasProperty = function (name) {
            // tslint:disable-next-line: max-line-length
            if (/^(items|itemsPresenter|value|valuePath|valueSeparator|label|labelPath|selectedItems|selectedItem|selectedIndex|uiVirtualize|viewModel|localViewModel|data)$/.test(name)) {
                return true;
            }
            return _super.prototype.hasProperty.call(this, name);
        };
        AtomItemsControl.prototype.dispose = function (e) {
            this.items = null;
            this.selectedItems = null;
            // this.mFilteredItems = null;
            _super.prototype.dispose.call(this, e);
        };
        AtomItemsControl.prototype.onPropertyChanged = function (name) {
            switch (name) {
                case "itemsPresenter":
                case "itemTemplate":
                case "labelPath":
                case "valuePath":
                case "items":
                case "filter":
                case "sort":
                    if (this.mItems) {
                        this.invalidateItems();
                    }
                    // this.runAfterInit(() => {
                    //     if (this.mItems) {
                    //         this.onCollectionChangedInternal("refresh", -1, null);
                    //     }
                    // });
                    break;
            }
        };
        Object.defineProperty(AtomItemsControl.prototype, "selectAll", {
            set: function (v) {
                if (v === undefined || v === null) {
                    return;
                }
                this.mSelectedItems.length = 0;
                var items = this.mItems;
                if (v && items) {
                    for (var _i = 0, items_1 = items; _i < items_1.length; _i++) {
                        var itm = items_1[_i];
                        this.mSelectedItems.push(itm);
                    }
                }
                this.mSelectAll = true;
                AtomBinder_1.AtomBinder.refreshItems(this.mSelectedItems);
            },
            enumerable: false,
            configurable: true
        });
        AtomItemsControl.prototype.resetVirtualContainer = function () {
            var ip = this.itemsPresenter;
            if (ip) {
                this.disposeChildren(ip);
            }
            this.mFirstChild = null;
            this.mLastChild = null;
            this.mScrollerSetup = false;
            this.mScopes = null;
            this.unbindEvent(this.mVirtualContainer, "scroll");
        };
        AtomItemsControl.prototype.postVirtualCollectionChanged = function () {
            var _this = this;
            this.app.callLater(function () {
                _this.onVirtualCollectionChanged();
            });
        };
        AtomItemsControl.prototype.onVirtualCollectionChanged = function () {
            var _this = this;
            var ip = this.itemsPresenter;
            var items = this.items;
            if (!items.length) {
                this.resetVirtualContainer();
                return;
            }
            this.validateScroller();
            var fc = this.mFirstChild;
            var lc = this.mLastChild;
            var vc = this.mVirtualContainer;
            var vcHeight = AtomUI_1.AtomUI.innerHeight(vc);
            var vcScrollHeight = vc.scrollHeight;
            if (isNaN(vcHeight) || vcHeight <= 0 || vcScrollHeight <= 0) {
                setTimeout(function () {
                    _this.onVirtualCollectionChanged();
                }, 1000);
                return;
            }
            var vcWidth = AtomUI_1.AtomUI.innerWidth(vc);
            var avgHeight = this.mAvgHeight;
            var avgWidth = this.mAvgWidth;
            var itemsHeight = vc.scrollHeight - AtomUI_1.AtomUI.outerHeight(fc) - AtomUI_1.AtomUI.outerHeight(lc);
            var itemsWidth = AtomUI_1.AtomUI.innerWidth(ip);
            var element = this.element;
            var ce;
            var ae = new AtomEnumerator_1.default(items);
            if (this.mTraining) {
                if (vcHeight >= itemsHeight) {
                    // lets add item...
                    ce = lc.previousElementSibling;
                    if (ce !== fc) {
                        var data = ce.atomControl.data;
                        while (ae.next()) {
                            if (ae.current === data) {
                                break;
                            }
                        }
                    }
                    if (ae.next()) {
                        var data = ae.current;
                        var elementChild = this.createChild(null, data);
                        ip.insertBefore(elementChild.element, lc);
                        this.postVirtualCollectionChanged();
                    }
                }
                else {
                    // calculate avg height
                    var totalVisibleItems = 0;
                    ce = fc.nextElementSibling;
                    var allHeight = 0;
                    var allWidth = 0;
                    while (ce !== lc) {
                        totalVisibleItems++;
                        allHeight += AtomUI_1.AtomUI.outerHeight(ce);
                        allWidth += AtomUI_1.AtomUI.outerWidth(ce);
                        ce = ce.nextElementSibling;
                    }
                    avgHeight = allHeight / totalVisibleItems;
                    avgWidth = allWidth / totalVisibleItems;
                    totalVisibleItems--;
                    this.mAvgHeight = avgHeight;
                    this.mAvgWidth = avgWidth;
                    var columns = Math.floor(vcWidth / avgWidth);
                    var allRows = Math.ceil(items.length / columns);
                    var visibleRows = Math.ceil(totalVisibleItems / columns);
                    // tslint:disable-next-line:no-console
                    console.log({
                        avgWidth: avgWidth,
                        avgHeight: avgHeight,
                        totalVisibleItems: totalVisibleItems,
                        allRows: allRows,
                        columns: columns
                    });
                    this.mAllRows = allRows;
                    this.mColumns = columns;
                    this.mVisibleRows = visibleRows;
                    this.mVisibleHeight = visibleRows * avgHeight;
                    // set height of last child... to increase padding
                    lc.style.height = ((allRows - visibleRows + 1) * avgHeight) + "px";
                    this.mTraining = false;
                    this.mReady = true;
                    this.postVirtualCollectionChanged();
                }
                return;
            }
            var self = this;
            this.lastScrollTop = vc.scrollTop;
            if (this.mIsChanging) {
                // setTimeout(function () {
                //    self.onVirtualCollectionChanged();
                // }, 100);
                return;
            }
            this.mIsChanging = true;
            var block = Math.floor(this.mVisibleHeight / avgHeight);
            var itemsInBlock = this.mVisibleRows * this.mColumns;
            // lets simply recreate the view... if we are out of the scroll bounds...
            var index = Math.floor(vc.scrollTop / this.mVisibleHeight);
            var itemIndex = index * itemsInBlock;
            // console.log("First block index is " + index + " item index is " + index * itemsInBlock);
            if (itemIndex >= items.length) {
                this.mIsChanging = false;
                return;
            }
            var lastIndex = Math.min((Math.max(index, 0) + 3) * itemsInBlock - 1, items.length - 1);
            var firstIndex = Math.max(0, (index) * itemsInBlock);
            ce = fc.nextElementSibling;
            var firstItem = fc.nextElementSibling;
            var lastItem = lc.previousElementSibling;
            if (firstItem !== lastItem) {
                var firstVisibleIndex = items.indexOf(firstItem.atomControl.data);
                var lastVisibleIndex = items.indexOf(lastItem.atomControl.data);
                // tslint:disable-next-line:no-console
                console.log({
                    firstVisibleIndex: firstVisibleIndex,
                    firstIndex: firstIndex,
                    lastVisibleIndex: lastVisibleIndex,
                    lastIndex: lastIndex
                });
                if (firstIndex >= firstVisibleIndex && lastIndex <= lastVisibleIndex) {
                    // tslint:disable-next-line:no-console
                    console.log("All items are visible...");
                    this.mIsChanging = false;
                    return;
                }
            }
            var remove = [];
            var cache = {};
            while (ce !== lc) {
                var c = ce;
                ce = ce.nextElementSibling;
                var s = items.indexOf(c.atomControl.data);
                cache[s] = c;
                remove.push(c);
            }
            this.app.dispatcher.pause();
            ae = new AtomEnumerator_1.default(items);
            for (var i = 0; i < firstIndex; i++) {
                ae.next();
            }
            var after = fc;
            var last = null;
            var add = [];
            for (var i = firstIndex; i <= lastIndex; i++) {
                if (!ae.next()) {
                    break;
                }
                var index2 = ae.currentIndex;
                var data = ae.current;
                var elementChild = cache[index2];
                if (elementChild && element.atomControl.data === data) {
                    cache[index2] = null;
                }
                else {
                    elementChild = this.createChild(null, data).element;
                }
                elementChild.before = after;
                add.push(elementChild);
                after = elementChild;
                last = index2;
            }
            var h = (this.mAllRows - block * 3) * avgHeight - index * this.mVisibleHeight;
            // tslint:disable-next-line:no-console
            console.log("last child height = " + h);
            this.app.callLater(function () {
                var oldHeight = AtomUI_1.AtomUI.outerHeight(fc);
                var newHeight = index * _this.mVisibleHeight;
                var diff = newHeight - oldHeight;
                var oldScrollTop = vc.scrollTop;
                var a = new AtomEnumerator_1.default(add);
                while (a.next()) {
                    var ec = a.current;
                    ip.insertBefore(ec, ec.before.nextElementSibling);
                    ec.before = null;
                }
                fc.style.height = newHeight + "px";
                for (var _i = 0, remove_1 = remove; _i < remove_1.length; _i++) {
                    var iterator = remove_1[_i];
                    if (!iterator.before) {
                        iterator.atomControl.dispose();
                    }
                    iterator.remove();
                }
                // const a = new AtomEnumerator(remove);
                // while (a.next()) {
                //     const ec = a.current();
                //     if (!ec.before) {
                //         ec.atomControl.dispose();
                //     }
                //     ec.remove();
                // }
                // vc.scrollTop = oldScrollTop - diff;
                lc.style.height = h + "px";
                // tslint:disable-next-line:no-console
                console.log("Old: " + oldScrollTop + " Diff: " + diff + " Old Height: " + oldHeight + " Height: " + newHeight);
                _this.mIsChanging = false;
            });
            this.app.dispatcher.start();
            AtomBinder_1.AtomBinder.refreshValue(this, "childAtomControls");
        };
        AtomItemsControl.prototype.isSelected = function (item) {
            var selectedItem = null;
            for (var _i = 0, _a = this.mSelectedItems; _i < _a.length; _i++) {
                var iterator = _a[_i];
                selectedItem = iterator;
                if (selectedItem === item) {
                    return true;
                }
            }
            return false;
        };
        AtomItemsControl.prototype.bringIntoView = function (data) {
            var _this = this;
            this.app.callLater(function () {
                var en = new AtomUI_1.ChildEnumerator(_this.itemsPresenter || _this.element);
                while (en.next()) {
                    var item = en.current;
                    var dataItem = item.atomControl ? item.atomControl.data : item;
                    if (dataItem === data) {
                        item.scrollIntoView();
                        return;
                    }
                }
            });
        };
        AtomItemsControl.prototype.bringSelectionIntoView = function () {
            // do not scroll for first auto select
            // if (this.mAllowSelectFirst && this.get_selectedIndex() === 0) {
            //     return;
            // }
            var _this = this;
            if (this.uiVirtualize) {
                var index = this.selectedIndex;
                if (!this.mReady) {
                    setTimeout(function () {
                        _this.bringSelectionIntoView();
                    }, 1000);
                    return;
                }
                var avgHeight = this.mAvgHeight;
                var vcHeight = AtomUI_1.AtomUI.innerHeight(this.mVirtualContainer);
                var block = Math.ceil(vcHeight / avgHeight);
                var itemsInBlock = block * this.mColumns;
                var scrollTop = Math.floor(index / itemsInBlock);
                AtomUI_1.AtomUI.scrollTop(this.mVirtualContainer, scrollTop * vcHeight);
                return;
            }
            var en = new AtomUI_1.ChildEnumerator(this.itemsPresenter || this.element);
            var _loop_1 = function () {
                var item = en.current;
                var dataItem = item.atomControl ? item.atomControl.data : item;
                if (this_1.isSelected(dataItem)) {
                    setTimeout(function () {
                        item.scrollIntoView();
                    }, 1000);
                    return { value: void 0 };
                }
            };
            var this_1 = this;
            while (en.next()) {
                var state_1 = _loop_1();
                if (typeof state_1 === "object")
                    return state_1.value;
            }
        };
        AtomItemsControl.prototype.updateSelectionBindings = function () {
            this.version = this.version + 1;
            if (this.mSelectedItems && this.mSelectedItems.length) {
                this.mValue = undefined;
            }
            AtomBinder_1.AtomBinder.refreshValue(this, "value");
            AtomBinder_1.AtomBinder.refreshValue(this, "selectedItem");
            AtomBinder_1.AtomBinder.refreshValue(this, "selectedItems");
            AtomBinder_1.AtomBinder.refreshValue(this, "selectedIndex");
            if (!this.mSelectedItems.length) {
                if (this.mSelectAll === true) {
                    this.mSelectAll = false;
                    AtomBinder_1.AtomBinder.refreshValue(this, "selectAll");
                }
            }
        };
        AtomItemsControl.prototype.onSelectedItemsChanged = function (type, index, item) {
            if (!this.mOnUIChanged) {
                // this.updateChildSelections(type, index, item);
                if (this.autoScrollToSelection) {
                    this.bringSelectionIntoView();
                }
            }
            this.updateSelectionBindings();
            // AtomControl.updateUI();
            // this.invokePost();
        };
        AtomItemsControl.prototype.hasItems = function () {
            return this.mItems !== undefined && this.mItems !== null;
        };
        AtomItemsControl.prototype.invalidateItems = function () {
            var _this = this;
            if (this.pendingInits || this.isUpdating) {
                setTimeout(function () {
                    _this.invalidateItems();
                }, 5);
                return;
            }
            if (this.itemsInvalidated) {
                clearTimeout(this.itemsInvalidated);
                this.itemsInvalidated = 0;
            }
            this.itemsInvalidated = setTimeout(function () {
                _this.itemsInvalidated = 0;
                _this.onCollectionChangedInternal("refresh", -1, null);
            }, 5);
            // this.registerDisposable({
            //     dispose: () => {
            //         if (this.itemsInvalidated) {
            //             clearTimeout(this.itemsInvalidated);
            //         }
            //     }
            // });
        };
        AtomItemsControl.prototype.onCollectionChanged = function (key, index, item) {
            if (!this.mItems) {
                return;
            }
            if (!this.itemTemplate) {
                return;
            }
            if (!this.itemsPresenter) {
                this.itemsPresenter = this.element;
            }
            this.version = this.version + 1;
            if (/reset|refresh/i.test(key)) {
                this.resetVirtualContainer();
            }
            if (/remove/gi.test(key)) {
                // tslint:disable-next-line:no-shadowed-variable
                var ip_1 = this.itemsPresenter || this.element;
                var en = new AtomUI_1.ChildEnumerator(ip_1);
                while (en.next()) {
                    var ce = en.current;
                    // tslint:disable-next-line:no-shadowed-variable
                    var c = ce;
                    if (c.atomControl && c.atomControl.data === item) {
                        c.atomControl.dispose();
                        ce.remove();
                        break;
                    }
                }
                // AtomControl.updateUI();
                return;
            }
            if (this.uiVirtualize) {
                this.onVirtualCollectionChanged();
                return;
            }
            // AtomUIComponent
            var parentScope = undefined;
            // const parentScope = this.get_scope();
            // const et = this.getTemplate("itemTemplate");
            // if (et) {
            //     et = AtomUI.getAtomType(et);
            //     if (et) {
            //         this._childItemType = et;
            //     }
            // }
            var items = this.mFilter ? this.mItems.filter(this.mFilter) : this.mItems;
            var s = this.sort;
            if (s) {
                if (typeof s === "string") {
                    var sp_1 = s;
                    s = function (l, r) {
                        var lv = (l[sp_1] || "").toString();
                        var rv = (r[sp_1] || "").toString();
                        return lv.toLowerCase().localeCompare(rv.toLowerCase());
                    };
                }
                items = items.sort(s);
            }
            if (/add/gi.test(key)) {
                // WebAtoms.dispatcher.pause();
                // for (const aeItem of this.mItems) {
                //     for (const ceItem of AtomUI.childEnumerator(this.itemsPresenter)) {
                //         const d: any = ceItem;
                //         if (aeItem.currentIndex() === index) {
                //             const ctl: any = this.createChildElement(parentScope, this.itemsPresenter, item, aeItem, d);
                //             this.applyItemStyle(ctl, item, aeItem.isFirst(), aeItem.isLast());
                //             break;
                //         }
                //         if (aeItem.isLast()) {
                // tslint:disable-next-line:max-line-length
                //             const ctl: any = this.createChildElement(parentScope, this.itemsPresenter, item, aeItem, null);
                //             this.applyItemStyle(ctl, item, aeItem.isFirst(), aeItem.isLast());
                //             break;
                //         }
                //     }
                // }
                // WebAtoms.dispatcher.start();
                // AtomControl.updateUI();
                var lastItem = items[index];
                var last = null;
                var cIndex = 0;
                var en = new AtomUI_1.ChildEnumerator(this.itemsPresenter);
                while (en.next()) {
                    if (cIndex === index) {
                        last = en.current;
                        break;
                    }
                    cIndex++;
                }
                var df2 = document.createDocumentFragment();
                this.createChild(df2, lastItem);
                if (last) {
                    this.itemsPresenter.insertBefore(df2, last);
                }
                else {
                    this.itemsPresenter.appendChild(df2);
                }
                return;
            }
            var element = this.itemsPresenter;
            // const dataItems = this.get_dataItems();
            // AtomControl.disposeChildren(element);
            this.disposeChildren(this.itemsPresenter);
            // WebAtoms.dispatcher.pause();
            // const items = this.get_dataItems(true);
            var added = [];
            // this.getTemplate("itemTemplate");
            // tslint:disable-next-line:no-console
            // console.log("Started");
            // const df = document.createDocumentFragment();
            var ip = this.itemsPresenter || this.element;
            for (var _i = 0, items_2 = items; _i < items_2.length; _i++) {
                var mItem = items_2[_i];
                var data = mItem;
                // const elementChild = this.createChildElement(parentScope, element, data, mItem, null);
                // added.push(elementChild);
                // this.applyItemStyle(elementChild, data, mItem.isFirst(), mItem.isLast());
                var ac = this.createChild(null, data);
                ip.appendChild(ac.element);
            }
            // (this.element as HTMLElement).appendChild(df);
            // tslint:disable-next-line:no-console
            // console.log("Ended");
            // const self = this;
            // WebAtoms.dispatcher.callLater(() => {
            //     const dirty = [];
            //     for (const elementItem of AtomUI.childEnumerator(element)) {
            //         const ct = elementItem;
            //         const func = added.filter((fx) => ct === fx);
            //         if (func.pop() !== ct) {
            //             dirty.push(ct);
            //         }
            //     }
            //     for (const dirtyItem of dirty) {
            //         const drt = dirtyItem;
            //         if (drt.atomControl) {
            //             drt.atomControl.dispose();
            //         }
            //         AtomUI.remove(item);
            //     }
            //     });
            // WebAtoms.dispatcher.start();
            // AtomBinder.refreshValue(this, "childAtomControls");
        };
        AtomItemsControl.prototype.preCreate = function () {
            this.mAllowSelectFirst = false;
            this.allowMultipleSelection = false;
            this.valuePath = "value";
            this.labelPath = "label";
            this.version = 1;
            this.autoScrollToSelection = false;
            this.sort = null;
            this.valueSeparator = ", ";
            this.uiVirtualize = false;
            this.mSelectAll = false;
            this.mItems = null;
            this.selectedItems = [];
            this.itemTemplate = AtomItemsControlItemTemplate;
            _super.prototype.preCreate.call(this);
        };
        AtomItemsControl.prototype.onCollectionChangedInternal = function (key, index, item) {
            var _this = this;
            // Atom.refresh(this, "allValues");
            // AtomBinder.refreshValue(this, "allValues");
            var value = this.value;
            try {
                this.isUpdating = true;
                this.onCollectionChanged(key, index, item);
                if (value) {
                    if (!(value || this.mAllowSelectFirst)) {
                        AtomBinder_1.AtomBinder.clear(this.mSelectedItems);
                    }
                }
                if (value != null) {
                    this.value = value;
                    if (this.selectedIndex !== -1) {
                        return;
                    }
                    else {
                        this.mValue = undefined;
                    }
                }
            }
            finally {
                this.app.callLater(function () {
                    _this.isUpdating = false;
                });
            }
            // this.selectDefault();
        };
        Object.defineProperty(AtomItemsControl.prototype, "allowSelectFirst", {
            set: function (b) {
                b = b ? b !== "false" : b;
                this.mAllowSelectFirst = b;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomItemsControl.prototype, "filter", {
            set: function (f) {
                if (f === this.mFilter) {
                    return;
                }
                this.mFilter = f;
                // this.mFilteredItems = null;
                AtomBinder_1.AtomBinder.refreshValue(this, "filter");
            },
            enumerable: false,
            configurable: true
        });
        AtomItemsControl.prototype.onScroll = function () {
            var _this = this;
            if (this.scrollTimeout) {
                clearTimeout(this.scrollTimeout);
            }
            this.scrollTimeout = setTimeout(function () {
                _this.scrollTimeout = 0;
                _this.onVirtualCollectionChanged();
            }, 10);
        };
        AtomItemsControl.prototype.toggleSelection = function (data) {
            this.mOnUIChanged = true;
            this.mValue = undefined;
            if (this.allowMultipleSelection) {
                if (this.mSelectedItems.indexOf(data) !== -1) {
                    AtomBinder_1.AtomBinder.removeItem(this.mSelectedItems, data);
                }
                else {
                    AtomBinder_1.AtomBinder.addItem(this.mSelectedItems, data);
                }
            }
            else {
                this.mSelectedItems.length = 1;
                this.mSelectedItems[0] = data;
                AtomBinder_1.AtomBinder.refreshItems(this.mSelectedItems);
            }
            this.mOnUIChanged = false;
        };
        AtomItemsControl.prototype.validateScroller = function () {
            var _this = this;
            if (this.mScrollerSetup) {
                return;
            }
            var ip = this.itemsPresenter;
            var e = this.element;
            var vc = this.mVirtualContainer;
            if (!vc) {
                if (ip === e && !/table/i.test(e.nodeName)) {
                    throw new Error("virtualContainer presenter not found,"
                        + "you must put itemsPresenter inside a virtualContainer in order for Virtualization to work");
                }
                else {
                    vc = this.mVirtualContainer = this.element;
                }
            }
            vc.style.overflow = "auto";
            this.bindEvent(vc, "scroll", function () {
                _this.onScroll();
            });
            ip.style.overflow = "hidden";
            // this.validateScroller = null;
            var isTable = /tbody/i.test(ip.nodeName);
            var fc;
            var lc;
            if (isTable) {
                fc = document.createElement("TR");
                lc = document.createElement("TR");
            }
            else {
                fc = document.createElement("DIV");
                lc = document.createElement("DIV");
            }
            fc.classList.add("sticky");
            fc.classList.add("first-child");
            lc.classList.add("sticky");
            lc.classList.add("last-child");
            fc.style.position = "relative";
            fc.style.height = "0";
            fc.style.width = "100%";
            fc.style.clear = "both";
            lc.style.position = "relative";
            lc.style.height = "0";
            lc.style.width = "100%";
            lc.style.clear = "both";
            this.mFirstChild = fc;
            this.mLastChild = lc;
            ip.appendChild(fc);
            ip.appendChild(lc);
            // let us train ourselves to find average height/width
            this.mTraining = true;
            this.mScrollerSetup = true;
        };
        AtomItemsControl.prototype.createChild = function (df, data) {
            var t = this.itemTemplate;
            var ac = this.app.resolve(t, true);
            var e = ac.element;
            e._logicalParent = this.element;
            e._templateParent = this;
            if (df) {
                df.appendChild(ac.element);
            }
            ac.data = data;
            this.element.dispatchEvent(new CustomEvent("item-created", {
                bubbles: false,
                cancelable: false,
                detail: data
            }));
            return ac;
        };
        AtomItemsControl.prototype.disposeChildren = function (e) {
            var en = new AtomUI_1.ChildEnumerator(e);
            while (en.next()) {
                var iterator = en.current;
                var ac = iterator.atomControl;
                if (ac) {
                    ac.dispose();
                }
            }
            e.innerHTML = "";
        };
        /** Item Template for displaying individual items */
        AtomItemsControl.itemTemplate = XNode_1.default.prepare("itemTemplate", true, true);
        return AtomItemsControl;
    }(AtomControl_1.AtomControl));
    exports.AtomItemsControl = AtomItemsControl;
    var AtomItemsControlItemTemplate = /** @class */ (function (_super) {
        __extends(AtomItemsControlItemTemplate, _super);
        function AtomItemsControlItemTemplate() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        AtomItemsControlItemTemplate.prototype.create = function () {
            var _this = this;
            this.runAfterInit(function () {
                var tp = _this.element._templateParent;
                _this.element.textContent = _this.data[tp.valuePath];
            });
        };
        return AtomItemsControlItemTemplate;
    }(AtomControl_1.AtomControl));
});
//# sourceMappingURL=AtomItemsControl.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomItemsControl");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../styles/AtomListBoxStyle", "./AtomItemsControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomListBox = void 0;
    var AtomListBoxStyle_1 = require("../styles/AtomListBoxStyle");
    var AtomItemsControl_1 = require("./AtomItemsControl");
    var AtomListBox = /** @class */ (function (_super) {
        __extends(AtomListBox, _super);
        function AtomListBox() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        AtomListBox.prototype.preCreate = function () {
            var _this = this;
            this.selectItemOnClick = true;
            _super.prototype.preCreate.call(this);
            this.defaultControlStyle = AtomListBoxStyle_1.AtomListBoxStyle;
            this.registerItemClick();
            this.runAfterInit(function () {
                var _a;
                return _this.setElementClass(_this.element, (_a = {},
                    _a[_this.controlStyle.name] = 1,
                    _a["atom-list-box"] = 1,
                    _a));
            });
        };
        AtomListBox.prototype.registerItemClick = function () {
            var _this = this;
            this.bindEvent(this.element, "click", function (e) {
                var p = _this.atomParent(e.target);
                if (p === _this) {
                    return;
                }
                if (p.element._logicalParent === _this.element) {
                    // this is child..
                    var data = p.data;
                    if (!data) {
                        return;
                    }
                    if (_this.selectItemOnClick) {
                        _this.toggleSelection(data);
                        var ce = new CustomEvent("selectionChanged", {
                            bubbles: false,
                            cancelable: false,
                            detail: data
                        });
                        _this.element.dispatchEvent(ce);
                    }
                }
            });
        };
        AtomListBox.prototype.createChild = function (df, data) {
            var child = _super.prototype.createChild.call(this, df, data);
            child.bind(child.element, "styleClass", [
                ["this", "version"],
                ["data"],
                ["this", "selectedItems"]
            ], false, function (version, itemData, selectedItems) {
                return {
                    "list-item": true,
                    "item": true,
                    "selected-item": selectedItems
                        && selectedItems.find(function (x) { return x === itemData; }),
                    "selected-list-item": selectedItems
                        && selectedItems.find(function (x) { return x === itemData; })
                };
            }, this);
            return child;
        };
        return AtomListBox;
    }(AtomItemsControl_1.AtomItemsControl));
    exports.AtomListBox = AtomListBox;
});
//# sourceMappingURL=AtomListBox.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomListBox");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../styles/AtomToggleButtonBarStyle", "./AtomControl", "./AtomListBox"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomToggleButtonBar = void 0;
    var AtomToggleButtonBarStyle_1 = require("../styles/AtomToggleButtonBarStyle");
    var AtomControl_1 = require("./AtomControl");
    var AtomListBox_1 = require("./AtomListBox");
    var AtomToggleButtonBar = /** @class */ (function (_super) {
        __extends(AtomToggleButtonBar, _super);
        function AtomToggleButtonBar(app, e) {
            return _super.call(this, app, e || document.createElement("ul")) || this;
        }
        AtomToggleButtonBar.prototype.preCreate = function () {
            var _this = this;
            _super.prototype.preCreate.call(this);
            this.allowMultipleSelection = false;
            this.allowSelectFirst = true;
            this.itemTemplate = AtomToggleButtonBarItemTemplate;
            this.defaultControlStyle = AtomToggleButtonBarStyle_1.AtomToggleButtonBarStyle;
            this.registerItemClick();
            this.runAfterInit(function () {
                var _a;
                return _this.setElementClass(_this.element, (_a = {},
                    _a[_this.controlStyle.name] = 1,
                    _a["atom-toggle-button-bar"] = 1,
                    _a), true);
            });
        };
        return AtomToggleButtonBar;
    }(AtomListBox_1.AtomListBox));
    exports.AtomToggleButtonBar = AtomToggleButtonBar;
    var AtomToggleButtonBarItemTemplate = /** @class */ (function (_super) {
        __extends(AtomToggleButtonBarItemTemplate, _super);
        function AtomToggleButtonBarItemTemplate(app, e) {
            return _super.call(this, app, e || document.createElement("li")) || this;
        }
        AtomToggleButtonBarItemTemplate.prototype.create = function () {
            var _this = this;
            this.bind(this.element, "text", [["data"]], false, function (v) {
                var p = _this.parent;
                return v[p.labelPath];
            });
        };
        return AtomToggleButtonBarItemTemplate;
    }(AtomControl_1.AtomControl));
});
//# sourceMappingURL=AtomToggleButtonBar.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomToggleButtonBar");

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
    var ColorItem = /** @class */ (function () {
        function ColorItem(colorCodeOrRed, namedColorOrGreen, blue, alpha) {
            if (typeof colorCodeOrRed === "string") {
                this.colorCode = colorCodeOrRed;
                if (typeof namedColorOrGreen === "string") {
                    this.namedColor = namedColorOrGreen;
                }
                var r = ColorItem.parseRgb(this.colorCode);
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
        ColorItem.prototype.toString = function () {
            return this.colorCode;
        };
        ColorItem.prototype.withAlphaPercent = function (a) {
            // a = a * 255;
            return new ColorItem(this.red, this.green, this.blue, a);
        };
        ColorItem.parseRgb = function (rgba) {
            if (/^\#/.test(rgba)) {
                rgba = rgba.substr(1);
                // this is hex...
                if (rgba.length === 3) {
                    rgba = rgba.split("").map(function (x) { return x + x; }).join("");
                }
                var red = Number.parseInt(rgba[0] + rgba[1], 16);
                var green = Number.parseInt(rgba[2] + rgba[3], 16);
                var blue = Number.parseInt(rgba[4] + rgba[5], 16);
                if (rgba.length > 6) {
                    var alpha = Number.parseInt(rgba[6] + rgba[7], 16);
                    return { red: red, green: green, blue: blue, alpha: alpha };
                }
                return { red: red, green: green, blue: blue };
            }
            if (/^rgba/i.test(rgba)) {
                rgba = rgba.substr(5);
                rgba = rgba.substr(0, rgba.length - 1);
                var a = rgba.split(",").map(function (x, i) { return i === 3 ? Number.parseFloat(x) : Number.parseInt(x, 10); });
                return { red: a[0], green: a[1], blue: a[2], alpha: a[3] };
            }
            if (/^rgb/i.test(rgba)) {
                rgba = rgba.substr(4);
                rgba = rgba.substr(0, rgba.length - 1);
                var a = rgba.split(",").map(function (x) { return Number.parseInt(x, 10); });
                return { red: a[0], green: a[1], blue: a[2] };
            }
            throw new Error("Unknown color format " + rgba);
        };
        ColorItem.rgb = function (r, g, b, a) {
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
                return "rgba(" + r + "," + g + "," + b + "," + a + ")";
            }
            return "#" + toFixedString(r) + toFixedString(g) + toFixedString(b);
        };
        return ColorItem;
    }());
    exports.ColorItem = ColorItem;
    // function isInt(n: number): boolean {
    //     return Number(n) === n && n % 1 === 0;
    // }
    function toFixedString(t) {
        return ("0" + t.toString(16)).slice(-2);
    }
    var Colors = /** @class */ (function () {
        function Colors() {
        }
        Colors.rgba = function (red, green, blue, alpha) {
            return new ColorItem(red, green, blue, alpha);
        };
        Colors.parse = function (color) {
            if (!color) {
                return null;
            }
            color = color.toLowerCase();
            // check if exists in current...
            for (var key in Colors) {
                if (Colors.hasOwnProperty(key)) {
                    var element = Colors[key];
                    if (element instanceof ColorItem) {
                        var ci = element;
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
        };
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
        return Colors;
    }());
    exports.default = Colors;
});
//# sourceMappingURL=Colors.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/Colors");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Colors", "@web-atoms/core/dist/web/styles/AtomStyle", "@web-atoms/core/dist/web/styles/AtomToggleButtonBarStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Colors_1 = require("@web-atoms/core/dist/core/Colors");
    var AtomStyle_1 = require("@web-atoms/core/dist/web/styles/AtomStyle");
    var AtomToggleButtonBarStyle_1 = require("@web-atoms/core/dist/web/styles/AtomToggleButtonBarStyle");
    var FileViewerStyle = /** @class */ (function (_super) {
        __extends(FileViewerStyle, _super);
        function FileViewerStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(FileViewerStyle.prototype, "root", {
            get: function () {
                return {
                    position: "relative",
                    display: "inline-block",
                    backgroundColor: Colors_1.default.whiteSmoke,
                    borderColor: Colors_1.default.lightGray,
                    borderWidth: "1px",
                    borderStyle: "solid",
                    borderRadius: "5px",
                    padding: "5px",
                    margin: "5px",
                    minHeight: "600px",
                    minWidth: "90%",
                    marginBottom: "15px",
                    subclasses: {
                        " > ul": {
                            position: "absolute",
                            margin: 0,
                            padding: 0
                        },
                        " .preview": {
                            backgroundColor: Colors_1.default.white,
                            margin: "5px",
                            overflow: "auto",
                            borderTop: "solid 1px lightgray",
                            subclasses: {
                                " img": {
                                    height: "530px"
                                }
                            }
                        },
                        " > * > .code": {
                            position: "absolute",
                            backgroundColor: Colors_1.default.white,
                            left: 0,
                            top: 0,
                            right: 0,
                            bottom: 0,
                            margin: "5px",
                            overflow: "auto",
                            borderTop: "solid 1px lightgray",
                            subclasses: {
                                " > *": {
                                    width: "max-content",
                                    subclasses: {
                                        "> pre": {
                                            margin: 0
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            },
            enumerable: true,
            configurable: true
        });
        return FileViewerStyle;
    }(AtomStyle_1.AtomStyle));
    exports.default = FileViewerStyle;
    var MobileFileViewerStyle = /** @class */ (function (_super) {
        __extends(MobileFileViewerStyle, _super);
        function MobileFileViewerStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(MobileFileViewerStyle.prototype, "root", {
            get: function () {
                return {
                    position: "relative",
                    display: "inline-block",
                    backgroundColor: Colors_1.default.whiteSmoke,
                    borderColor: Colors_1.default.lightGray,
                    borderWidth: "1px",
                    borderStyle: "solid",
                    borderRadius: "5px",
                    padding: "5px",
                    margin: "5px",
                    minHeight: "600px",
                    minWidth: "90%",
                    subclasses: {
                        " > ul": {
                            position: "absolute",
                            margin: 0,
                            padding: 0
                        },
                        " > * > .code": {
                            position: "absolute",
                            backgroundColor: Colors_1.default.white,
                            left: 0,
                            top: 0,
                            right: 0,
                            bottom: 0,
                            overflow: "auto",
                            borderTop: "solid 1px lightgray",
                            subclasses: {
                                " > *": {
                                    width: "max-content",
                                    subclasses: {
                                        "> pre": {
                                            margin: 0
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            },
            enumerable: true,
            configurable: true
        });
        return MobileFileViewerStyle;
    }(AtomStyle_1.AtomStyle));
    exports.MobileFileViewerStyle = MobileFileViewerStyle;
    var FileBarStyle = /** @class */ (function (_super) {
        __extends(FileBarStyle, _super);
        function FileBarStyle() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.screen = _this.styleSheet.app.screen;
            return _this;
        }
        Object.defineProperty(FileBarStyle.prototype, "root", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(FileBarStyle, "root")), { padding: "2px 5px" });
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(FileBarStyle.prototype, "item", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(FileBarStyle, "item")), { marginRight: "1px", marginBottom: "1px", padding: this.screen.screenType === "mobile" ? "8px 0 8px 5px" : "8px 16px", minWidth: "50px", width: this.screen.screenType === "mobile" ? "49%" : "auto", background: Colors_1.default.gray, border: "none", borderTop: "1px solid #444857", boxSizing: "border-box", borderRadius: 0, display: "inline-block", color: Colors_1.default.lightGray, fontSize: "11pt", fontFamily: "'Muli', sans-serif", subclasses: {
                        ":first-child": {
                            borderTopLeftRadius: "0",
                            borderBottomLeftRadius: "0;",
                            borderTopRightRadius: 0,
                            borderBottomRightRadius: 0
                        },
                        ":last-child": {
                            borderTopLeftRadius: 0,
                            borderBottomLeftRadius: 0,
                            borderTopRightRadius: "0",
                            borderBottomRightRadius: "0"
                        }
                    } });
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(FileBarStyle.prototype, "selectedItem", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(FileBarStyle, "selectedItem")), { marginRight: "1px", padding: this.screen.screenType === "mobile" ? "8px 0 8px 5px" : "8px 16px", minWidth: "50px", background: "#2c303a", border: "none", borderTop: "1px solid #d5d7de", boxSizing: "border-box", borderRadius: 0, display: "inline-block", color: "white", fontSize: "11pt", fontFamily: "'Muli', sans-serif", subclasses: {
                        ":first-child": {
                            borderTopLeftRadius: "0",
                            borderBottomLeftRadius: "0;",
                            borderTopRightRadius: 0,
                            borderBottomRightRadius: 0,
                        },
                        ":last-child": {
                            borderTopLeftRadius: 0,
                            borderBottomLeftRadius: 0,
                            borderTopRightRadius: "0",
                            borderBottomRightRadius: "0"
                        }
                    } });
            },
            enumerable: true,
            configurable: true
        });
        return FileBarStyle;
    }(AtomToggleButtonBarStyle_1.AtomToggleButtonBarStyle));
    exports.FileBarStyle = FileBarStyle;
});
//# sourceMappingURL=FileViewerStyle.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/core/web/FileViewerStyle");

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
    var AjaxOptions = /** @class */ (function () {
        function AjaxOptions() {
        }
        return AjaxOptions;
    }());
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var App_1 = require("../App");
    var DISingleton_1 = require("../di/DISingleton");
    var Inject_1 = require("../di/Inject");
    var CacheService = /** @class */ (function () {
        function CacheService(app) {
            this.app = app;
            this.cache = {};
        }
        CacheService.prototype.remove = function (key) {
            var v = this.cache[key];
            if (v) {
                this.clear(v);
                return v.value;
            }
            return null;
        };
        CacheService.prototype.getOrCreate = function (key, task) {
            return __awaiter(this, void 0, void 0, function () {
                var c, v, e_1;
                var _this = this;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            c = this.cache[key] || (this.cache[key] = {
                                key: key,
                                finalTTL: 3600
                            });
                            if (!c.value) {
                                c.value = task(c);
                            }
                            v = null;
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, 3, , 4]);
                            return [4 /*yield*/, c.value];
                        case 2:
                            v = _a.sent();
                            return [3 /*break*/, 4];
                        case 3:
                            e_1 = _a.sent();
                            this.clear(c);
                            throw e_1;
                        case 4:
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
                                c.timeout = setTimeout(function () {
                                    c.timeout = 0;
                                    _this.clear(c);
                                }, c.finalTTL * 1000);
                            }
                            else {
                                // this is the case where we do not want to store
                                this.clear(c);
                            }
                            return [4 /*yield*/, c.value];
                        case 5: return [2 /*return*/, _a.sent()];
                    }
                });
            });
        };
        CacheService.prototype.clear = function (ci) {
            if (ci.timeout) {
                clearTimeout(ci.timeout);
                ci.timeout = 0;
            }
            this.cache[ci.key] = null;
            delete this.cache[ci.key];
        };
        CacheService = __decorate([
            DISingleton_1.default(),
            __param(0, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App])
        ], CacheService);
        return CacheService;
    }());
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

var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
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
    var StringHelper_1 = require("../core/StringHelper");
    var RegisterSingleton_1 = require("../di/RegisterSingleton");
    var DateTime_1 = require("@web-atoms/date-time/dist/DateTime");
    exports.dateFormatISORegEx = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/;
    exports.dateFormatMSRegEx = /^\/Date\((d|-|.*)\)[\/|\\]$/;
    var timeZoneDiff = (new Date()).getTimezoneOffset();
    var JsonService = /** @class */ (function () {
        function JsonService() {
            this.options = {
                indent: 2,
                namingStrategy: "none",
                dateConverter: [
                    {
                        regex: exports.dateFormatISORegEx,
                        valueConverter: {
                            fromSource: function (v) {
                                var d = new DateTime_1.default(v);
                                // if (/z$/i.test(v)) {
                                //     d.setMinutes( d.getMinutes() - timeZoneDiff );
                                // }
                                return d;
                            },
                            fromTarget: function (v) {
                                return v.toISOString();
                            }
                        }
                    }, {
                        regex: exports.dateFormatMSRegEx,
                        valueConverter: {
                            fromSource: function (v) {
                                var a = exports.dateFormatMSRegEx.exec(v);
                                var b = a[1].split(/[-+,.]/);
                                return new DateTime_1.default(b[0] ? +b[0] : 0 - +b[1]);
                            },
                            fromTarget: function (v) {
                                return v.toISOString();
                            }
                        }
                    }
                ]
            };
        }
        JsonService.prototype.transformKeys = function (t, v) {
            var _this = this;
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
                var a = v;
                if (a.map) {
                    return a.map(function (x) { return _this.transformKeys(t, x); });
                }
                var ra = [];
                for (var _i = 0, a_1 = a; _i < a_1.length; _i++) {
                    var iterator = a_1[_i];
                    ra.push(this.transformKeys(t, iterator));
                }
                return ra;
            }
            var r = {};
            for (var key in v) {
                if (v.hasOwnProperty(key)) {
                    var element = v[key];
                    r[t(key)] = this.transformKeys(t, element);
                }
            }
            return r;
        };
        JsonService.prototype.parse = function (text, options) {
            var _a = __assign(__assign({}, this.options), options), dateConverter = _a.dateConverter, namingStrategy = _a.namingStrategy;
            var result = JSON.parse(text, function (key, value) {
                // transform date...
                if (typeof value === "string") {
                    for (var _i = 0, dateConverter_1 = dateConverter; _i < dateConverter_1.length; _i++) {
                        var iterator = dateConverter_1[_i];
                        var a = iterator.regex.test(value);
                        if (a) {
                            var dv = iterator.valueConverter.fromSource(value);
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
        };
        JsonService.prototype.stringify = function (v, options) {
            var _a = __assign(__assign({}, this.options), options), namingStrategy = _a.namingStrategy, dateConverter = _a.dateConverter, indent = _a.indent;
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
            return JSON.stringify(v, function (key, value) {
                if (key && /^\_\$\_/.test(key)) {
                    return undefined;
                }
                if (dateConverter && (value instanceof Date)) {
                    return dateConverter[0].valueConverter.fromTarget(value);
                }
                return value;
            }, indent);
        };
        JsonService = __decorate([
            RegisterSingleton_1.RegisterSingleton
        ], JsonService);
        return JsonService;
    }());
    exports.JsonService = JsonService;
});
//# sourceMappingURL=JsonService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/JsonService");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
    var JsonError = /** @class */ (function (_super) {
        __extends(JsonError, _super);
        function JsonError(message, json) {
            var _this = _super.call(this, message) || this;
            _this.json = json;
            return _this;
        }
        return JsonError;
    }(Error));
    exports.default = JsonError;
});
//# sourceMappingURL=JsonError.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/http/JsonError");

var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var AjaxOptions_1 = require("./AjaxOptions");
    var App_1 = require("../../App");
    var Atom_1 = require("../../Atom");
    var AtomBridge_1 = require("../../core/AtomBridge");
    var types_1 = require("../../core/types");
    var Inject_1 = require("../../di/Inject");
    var TypeKey_1 = require("../../di/TypeKey");
    var CacheService_1 = require("../CacheService");
    var JsonService_1 = require("../JsonService");
    var JsonError_1 = require("./JsonError");
    // tslint:disable-next-line
    function methodBuilder(method) {
        // tslint:disable-next-line
        return function (url, options) {
            // tslint:disable-next-line
            return function (target, propertyKey, descriptor) {
                target.methods = target.methods || {};
                var a = target.methods[propertyKey];
                var oldFunction = descriptor.value;
                // tslint:disable-next-line:typedef
                descriptor.value = function () {
                    var _this = this;
                    var args = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        args[_i] = arguments[_i];
                    }
                    if (this.testMode || Atom_1.Atom.designMode) {
                        // tslint:disable-next-line:no-console
                        console.log("Test Design Mode: " + url + " .. " + args.join(","));
                        var ro = oldFunction.apply(this, args);
                        if (ro) {
                            return ro;
                        }
                    }
                    var jsCache = options ? options.jsCacheSeconds : 0;
                    if (jsCache) {
                        var cacheService = this.app.resolve(CacheService_1.default);
                        var jArgs = args.map(function (arg) { return arg instanceof types_1.CancelToken ? null : arg; });
                        var key = this.constructor.name + ":" + method + ":" + url + ":" + JSON.stringify(jArgs);
                        return cacheService.getOrCreate(key, function (e) {
                            e.ttlSeconds = jsCache;
                            return _this.invoke(url, method, a, args, options);
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
                var a = target.methods[propertyKey];
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
        var a = target.methods[propertyKey];
        if (!a) {
            a = [];
            target.methods[propertyKey] = a;
        }
        a[parameterIndex] = new ServiceParameter("cancel", "");
    }
    exports.Cancel = Cancel;
    var ServiceParameter = /** @class */ (function () {
        function ServiceParameter(type, key, defaultValue) {
            this.type = type;
            this.key = key;
            this.defaultValue = defaultValue;
            this.type = type.toLowerCase();
            this.key = key;
        }
        return ServiceParameter;
    }());
    exports.ServiceParameter = ServiceParameter;
    function BaseUrl(baseUrl) {
        return function (target) {
            var key = TypeKey_1.TypeKey.get(target);
            BaseService.baseUrls[key] = baseUrl;
        };
    }
    exports.default = BaseUrl;
    var globalNS = (typeof global !== "undefined") ? global : window;
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
    var BaseService = /** @class */ (function () {
        function BaseService(app, jsonService) {
            this.app = app;
            this.jsonService = jsonService;
            this.testMode = false;
            this.showProgress = true;
            this.showError = false;
            // bs
            this.methods = {};
            this.methodReturns = {};
            this.jsonOptions = null;
            this.jsonOptions = __assign({}, this.jsonService.options);
        }
        BaseService.prototype.encodeData = function (o) {
            o.dataType = "application/json";
            o.data = this.jsonService.stringify(o.data, this.jsonOptions);
            o.contentType = "application/json";
            return o;
        };
        BaseService.prototype.sendResult = function (result, error) {
            return new Promise(function (resolve, reject) {
                if (error) {
                    setTimeout(function () {
                        reject(error);
                    }, 1);
                    return;
                }
                setTimeout(function () {
                    resolve(result);
                }, 1);
            });
        };
        BaseService.prototype.invoke = function (url, method, bag, values, methodOptions) {
            return __awaiter(this, void 0, void 0, function () {
                var p, t, bu, busyIndicator, options, headers, jsonOptions, i, p, vi, v, vs, replacer, key, element, xhr, text, response;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (this.baseUrl === undefined) {
                                p = Object.getPrototypeOf(this);
                                while (p) {
                                    t = TypeKey_1.TypeKey.get(p.constructor || p);
                                    bu = BaseService.baseUrls[t];
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
                                    url = "" + this.baseUrl + url;
                                }
                            }
                            busyIndicator = this.showProgress ? (this.app.createBusyIndicator()) : null;
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, , 3, 4]);
                            url = UMD.resolvePath(url);
                            options = new AjaxOptions_1.AjaxOptions();
                            options.method = method;
                            if (methodOptions) {
                                options.headers = methodOptions.headers;
                                options.dataType = methodOptions.accept;
                            }
                            headers = options.headers = options.headers || {};
                            // this is necessary to support IsAjaxRequest in ASP.NET MVC
                            if (!headers["X-Requested-With"]) {
                                headers["X-Requested-With"] = "XMLHttpRequest";
                            }
                            options.dataType = options.dataType || "application/json";
                            jsonOptions = __assign(__assign({}, this.jsonOptions), (methodOptions ? methodOptions.jsonOptions : {}));
                            if (bag) {
                                for (i = 0; i < bag.length; i++) {
                                    p = bag[i];
                                    vi = values[i];
                                    v = vi === undefined ? p.defaultValue : vi;
                                    if (v instanceof types_1.CancelToken) {
                                        options.cancel = v;
                                        continue;
                                    }
                                    switch (p.type) {
                                        case "path":
                                            if (v === undefined) {
                                                continue;
                                            }
                                            vs = v + "";
                                            replacer = "{" + p.key + "}";
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
                                            url += encodeURIComponent(p.key) + "=" + encodeURIComponent(v);
                                            break;
                                        case "queries":
                                            if (url.indexOf("?") === -1) {
                                                url += "?";
                                            }
                                            if (!/(\&|\?)$/.test(url)) {
                                                url += "&";
                                            }
                                            for (key in v) {
                                                if (v.hasOwnProperty(key)) {
                                                    element = v[key];
                                                    if (element) {
                                                        url += encodeURIComponent(key) + "=" + encodeURIComponent(element) + "&";
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
                            return [4 /*yield*/, this.ajax(url, options)];
                        case 2:
                            xhr = _a.sent();
                            if (/json/i.test(xhr.responseType)) {
                                text = xhr.responseText;
                                response = this.jsonService.parse(text, jsonOptions);
                                if (xhr.status >= 400) {
                                    throw new JsonError_1.default(typeof response === "string"
                                        ? response
                                        : (response.exceptionMessage
                                            || response.message
                                            || text
                                            || "Json Server Error"), response);
                                }
                                if (methodOptions && methodOptions.returnHeaders) {
                                    return [2 /*return*/, {
                                            headers: this.parseHeaders(xhr.responseHeaders),
                                            value: response
                                        }];
                                }
                                return [2 /*return*/, response];
                            }
                            if (xhr.status >= 400) {
                                throw new Error(xhr.responseText || "Server Error");
                            }
                            if (methodOptions && methodOptions.returnHeaders) {
                                return [2 /*return*/, {
                                        headers: this.parseHeaders(xhr.responseHeaders),
                                        value: xhr.responseText
                                    }];
                            }
                            return [2 /*return*/, xhr.responseText];
                        case 3:
                            if (busyIndicator) {
                                busyIndicator.dispose();
                            }
                            return [7 /*endfinally*/];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        };
        BaseService.prototype.parseHeaders = function (headers) {
            if (typeof headers === "object") {
                return headers;
            }
            return (headers || "")
                .split("\n")
                .reduce(function (pv, c) {
                var cv = c.split(":");
                pv[cv[0]] = (cv[1] || "").trim();
                return pv;
            }, {});
        };
        BaseService.prototype.ajax = function (url, options) {
            return __awaiter(this, void 0, void 0, function () {
                var xhr;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            // return new CancellablePromise();
                            url = url || options.url;
                            // no longer needed, watch must provide functionality of waiting and cancelling
                            // await Atom.delay(1, options.cancel);
                            if (options.cancel && options.cancel.cancelled) {
                                throw new Error("cancelled");
                            }
                            if (!AtomBridge_1.AtomBridge.instance.ajax) return [3 /*break*/, 2];
                            return [4 /*yield*/, new Promise(function (resolve, reject) {
                                    AtomBridge_1.AtomBridge.instance.ajax(url, options, function (r) {
                                        resolve(options);
                                    }, function (e) {
                                        reject(e);
                                    }, null);
                                })];
                        case 1: return [2 /*return*/, _a.sent()];
                        case 2:
                            xhr = new XMLHttpRequest();
                            return [4 /*yield*/, new Promise(function (resolve, reject) {
                                    if (options.cancel && options.cancel.cancelled) {
                                        reject(options.cancel.cancelled);
                                        return;
                                    }
                                    if (options.cancel) {
                                        options.cancel.registerForCancel(function (r) {
                                            xhr.abort();
                                            reject(r);
                                            return;
                                        });
                                    }
                                    xhr.onreadystatechange = function (e) {
                                        if (xhr.readyState === XMLHttpRequest.DONE) {
                                            options.status = xhr.status;
                                            options.responseText = xhr.responseText;
                                            // options.responseHeaders = (xhr.getAllResponseHeaders())
                                            //     .split("\n")
                                            //     .map((s) => s.trim().split(":"))
                                            //     .reduce((pv, cv) => pv[cv[0]] = cv[1], {});
                                            options.responseHeaders = xhr.getAllResponseHeaders();
                                            var ct = xhr.getResponseHeader("content-type");
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
                                    var h = options.headers;
                                    if (h) {
                                        for (var key in h) {
                                            if (h.hasOwnProperty(key)) {
                                                var element = h[key];
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
                                        var ct = xhr.getResponseHeader("content-type");
                                        options.responseType = ct || xhr.responseType;
                                        resolve(options);
                                    }
                                })];
                        case 3: return [2 /*return*/, _a.sent()];
                    }
                });
            });
        };
        BaseService.baseUrls = {};
        BaseService = __decorate([
            __param(0, Inject_1.Inject),
            __param(1, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App,
                JsonService_1.JsonService])
        ], BaseService);
        return BaseService;
    }());
    exports.BaseService = BaseService;
});
//# sourceMappingURL=RestService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/services/http/RestService");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/Atom", "@web-atoms/core/dist/core/types", "@web-atoms/core/dist/di/DISingleton", "@web-atoms/core/dist/services/http/RestService", "@web-atoms/core/dist/web/controls/AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Atom_1 = require("@web-atoms/core/dist/Atom");
    var types_1 = require("@web-atoms/core/dist/core/types");
    var DISingleton_1 = require("@web-atoms/core/dist/di/DISingleton");
    var RestService_1 = require("@web-atoms/core/dist/services/http/RestService");
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var MDService = /** @class */ (function (_super) {
        __extends(MDService, _super);
        function MDService() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        MDService.prototype.getUrl = function (url) {
            return __awaiter(this, void 0, void 0, function () {
                var b, a;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            b = this.app.createBusyIndicator();
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, , 3, 4]);
                            return [4 /*yield*/, this.ajax(url, { method: "GET" })];
                        case 2:
                            a = _a.sent();
                            return [2 /*return*/, a.responseText];
                        case 3:
                            b.dispose();
                            return [7 /*endfinally*/];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        };
        MDService = __decorate([
            DISingleton_1.default()
        ], MDService);
        return MDService;
    }(RestService_1.BaseService));
    var CodeView = /** @class */ (function (_super) {
        __extends(CodeView, _super);
        function CodeView() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.mSrc = null;
            return _this;
        }
        Object.defineProperty(CodeView.prototype, "src", {
            get: function () {
                return this.mSrc;
            },
            set: function (value) {
                var _this = this;
                this.mSrc = value;
                this.app.runAsync(function () { return _this.generate(value); });
            },
            enumerable: true,
            configurable: true
        });
        CodeView.prototype.preCreate = function () {
            this.require = null;
        };
        CodeView.prototype.generate = function (src) {
            return __awaiter(this, void 0, void 0, function () {
                var last, ss, app, highlight, md, text, pre, code, language;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!src) {
                                return [2 /*return*/];
                            }
                            last = src;
                            return [4 /*yield*/, Atom_1.Atom.delay(1)];
                        case 1:
                            _a.sent();
                            if (/^\./.test(src)) {
                                src = this.require.resolve(src);
                                src = src.replace("/dist/", "/src/");
                            }
                            ss = types_1.UMD.resolvePath("@web-atoms/samples/scripts/highlight/styles/vs.css");
                            app = this.app;
                            app.installStyleSheet(ss);
                            return [4 /*yield*/, types_1.UMD.import("@web-atoms/samples/scripts/highlight/highlight.pack.js")];
                        case 2:
                            highlight = _a.sent();
                            md = this.app.resolve(MDService);
                            src = types_1.UMD.resolvePath(src);
                            return [4 /*yield*/, md.getUrl(src)];
                        case 3:
                            text = _a.sent();
                            if (last !== this.src) {
                                return [2 /*return*/];
                            }
                            this.removeAllChildren(this.element);
                            pre = document.createElement("pre");
                            code = document.createElement("code");
                            code.textContent = text
                                .split("\n")
                                .map(function (s) {
                                while (s.endsWith("\r")) {
                                    s = s.substr(0, s.length - 1);
                                }
                                s = s.replace(/\t/g, "   ");
                                return s;
                            })
                                .join("\n");
                            language = this.getLanguage(src);
                            pre.classList.add(language);
                            pre.classList.add("language-" + language);
                            pre.appendChild(code);
                            highlight.highlightBlock(pre);
                            this.element.appendChild(pre);
                            return [2 /*return*/];
                    }
                });
            });
        };
        CodeView.prototype.getLanguage = function (path) {
            if (/\.tsx$/gi.test(path)) {
                return "typescript";
            }
            if (/\.ts$/gi.test(path)) {
                return "typescript";
            }
            if (/\.js$/gi.test(path)) {
                return "javascript";
            }
            if (/\.json$/gi.test(path)) {
                return "json";
            }
            return "plain";
        };
        return CodeView;
    }(AtomControl_1.AtomControl));
    exports.default = CodeView;
});
//# sourceMappingURL=CodeView.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/core/web/CodeView");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/web/controls/AtomGridSplitter", "@web-atoms/core/dist/web/controls/AtomGridView", "@web-atoms/core/dist/web/controls/AtomToggleButtonBar", "./FileViewerStyle", "./CodeView"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("@web-atoms/core/dist/core/Bind");
    var XNode_1 = require("@web-atoms/core/dist/core/XNode");
    var AtomGridSplitter_1 = require("@web-atoms/core/dist/web/controls/AtomGridSplitter");
    var AtomGridView_1 = require("@web-atoms/core/dist/web/controls/AtomGridView");
    var AtomToggleButtonBar_1 = require("@web-atoms/core/dist/web/controls/AtomToggleButtonBar");
    var FileViewerStyle_1 = require("./FileViewerStyle");
    var CodeView_1 = require("./CodeView");
    function fromPath(e, files) {
        if (!e || !files || !files.length) {
            return null;
        }
        var owner = e.atomControl;
        owner.file = files[0];
        return files.map(function (p) {
            var t = p.split("/");
            var n = t[t.length - 1];
            return {
                label: n,
                value: p
            };
        });
    }
    function setView(e, d) {
        if (!e || !d) {
            return;
        }
        var old = UMD.mock;
        UMD.mock = e.atomControl.designMode;
        var c = new (d)(e.atomControl.app);
        e.atomControl.demoPresenter.appendChild(c.element);
        UMD.mock = old;
    }
    var FileViewer = /** @class */ (function (_super) {
        __extends(FileViewer, _super);
        function FileViewer() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        FileViewer.prototype.create = function () {
            var _this = this;
            this.defaultControlStyle = this.app.screen.screenType === "mobile" ? FileViewerStyle_1.MobileFileViewerStyle : FileViewerStyle_1.default;
            this.files = null;
            this.file = null;
            this.require = null;
            this.demo = null;
            this.designMode = true;
            this.demoPresenter = null;
            this.render(this.app.screen.screenType !== "mobile"
                ? XNode_1.default.create("div", { rows: "36, *", columns: "*, 5, 50%", styleClass: Bind_1.default.oneTime(function () { return _this.controlStyle.name; }), none: Bind_1.default.oneWay(function () { return setView(_this.element, _this.demo); }) },
                    XNode_1.default.create(AtomToggleButtonBar_1.AtomToggleButtonBar, { controlStyle: FileViewerStyle_1.FileBarStyle, column: "0: 3", items: Bind_1.default.oneWay(function () { return fromPath(_this.element, _this.files); }), value: Bind_1.default.twoWays(function () { return _this.file; }) }),
                    XNode_1.default.create("div", { row: "1", class: "code" },
                        XNode_1.default.create(CodeView_1.default, { require: Bind_1.default.oneWay(function () { return _this.require; }), style: "overflow: auto", src: Bind_1.default.oneWay(function () { return _this.file; }) })),
                    XNode_1.default.create(AtomGridSplitter_1.AtomGridSplitter, { row: "1", column: "1" }),
                    XNode_1.default.create("div", { row: "1", column: "2", class: "preview", style: "margin: 5px;  overflow: auto", presenter: Bind_1.default.presenter("demoPresenter") }))
                :
                    XNode_1.default.create("div", { rows: "120, 400, 5, 590", columns: "100%", styleClass: Bind_1.default.oneTime(function () { return _this.controlStyle.name; }), none: Bind_1.default.oneWay(function () { return setView(_this.element, _this.demo); }) },
                        XNode_1.default.create(AtomToggleButtonBar_1.AtomToggleButtonBar, { items: Bind_1.default.oneWay(function () { return fromPath(_this.element, _this.files); }), value: Bind_1.default.twoWays(function () { return _this.file; }), controlStyle: FileViewerStyle_1.FileBarStyle, style: "padding: 0; background: #222" }),
                        XNode_1.default.create("div", { row: "1", class: "code" },
                            XNode_1.default.create(CodeView_1.default, { require: Bind_1.default.oneWay(function () { return _this.require; }), style: "overflow: auto", src: Bind_1.default.oneWay(function () { return _this.file; }) })),
                        XNode_1.default.create(AtomGridSplitter_1.AtomGridSplitter, { row: "2" }),
                        XNode_1.default.create("div", { row: "3", style: "padding: 0.5rem; overflow: auto", class: "preview", presenter: Bind_1.default.presenter("demoPresenter") })));
        };
        return FileViewer;
    }(AtomGridView_1.AtomGridView));
    exports.default = FileViewer;
});
//# sourceMappingURL=FileViewer.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/core/web/FileViewer");

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
    function resolveModulePath(require, def) {
        var m = AmdLoader.instance.modules;
        for (var key in m) {
            if (m.hasOwnProperty(key)) {
                var element = m[key];
                for (var k in element.exports) {
                    if (element.exports.hasOwnProperty(k)) {
                        var ex = element.exports[k];
                        if (ex === def) {
                            return element.name;
                        }
                    }
                }
            }
        }
    }
    exports.default = resolveModulePath;
});
//# sourceMappingURL=resolveModulePath.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/core/web/resolveModulePath");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/types", "@web-atoms/core/dist/di/DISingleton", "@web-atoms/core/dist/services/http/RestService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var types_1 = require("@web-atoms/core/dist/core/types");
    var DISingleton_1 = require("@web-atoms/core/dist/di/DISingleton");
    var RestService_1 = require("@web-atoms/core/dist/services/http/RestService");
    var SignupService = /** @class */ (function (_super) {
        __extends(SignupService, _super);
        function SignupService() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        SignupService.prototype.signup = function (user) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, null];
                });
            });
        };
        SignupService.prototype.states = function (country, ct) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, null];
                });
            });
        };
        SignupService.prototype.countries = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, null];
                });
            });
        };
        __decorate([
            RestService_1.Post("/user/signup"),
            __param(0, RestService_1.Body),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", [Object]),
            __metadata("design:returntype", Promise)
        ], SignupService.prototype, "signup", null);
        __decorate([
            RestService_1.Get("/user/locations/{country}/states"),
            __param(0, RestService_1.Path("country")),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", [String, types_1.CancelToken]),
            __metadata("design:returntype", Promise)
        ], SignupService.prototype, "states", null);
        __decorate([
            RestService_1.Get("/user/locations/countries"),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", []),
            __metadata("design:returntype", Promise)
        ], SignupService.prototype, "countries", null);
        SignupService = __decorate([
            DISingleton_1.default({
                mock: "./MockSignupService"
            })
        ], SignupService);
        return SignupService;
    }(RestService_1.BaseService));
    exports.default = SignupService;
});
//# sourceMappingURL=SignupService.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/samples/web/form/simple/SignupService");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./SignupService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var SignupService_1 = require("./SignupService");
    var MockSignupService = /** @class */ (function (_super) {
        __extends(MockSignupService, _super);
        function MockSignupService() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        MockSignupService.prototype.signup = function (user) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    if (user.emailAddress === "exists@exists.com") {
                        return [2 /*return*/, this.sendResult(null, "Email address already exists")];
                    }
                    return [2 /*return*/, this.sendResult(user)];
                });
            });
        };
        MockSignupService.prototype.states = function (country) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, this.sendResult(country === "IN" ? [
                            { label: "Maharashtra", value: "MH" },
                            { label: "Gujarat", value: "GJ" },
                            { label: "Punjab", value: "PB" }
                        ] : [
                            { label: "State1", value: "State1" },
                            { label: "State2", value: "State2" },
                            { label: "State3", value: "State3" },
                            { label: "State4", value: "State4" }
                        ])];
                });
            });
        };
        MockSignupService.prototype.countries = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/, this.sendResult([
                            { label: "India", value: "IN" },
                            { label: "Country1", value: "Country1" },
                            { label: "Country2", value: "Country2" }
                        ])];
                });
            });
        };
        return MockSignupService;
    }(SignupService_1.default));
    exports.default = MockSignupService;
});
//# sourceMappingURL=MockSignupService.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/samples/web/form/simple/MockSignupService");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
        define(["require", "exports", "../../App", "../../di/Inject", "./AtomControl", "./AtomItemsControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomComboBox = void 0;
    var App_1 = require("../../App");
    var Inject_1 = require("../../di/Inject");
    var AtomControl_1 = require("./AtomControl");
    var AtomItemsControl_1 = require("./AtomItemsControl");
    var AtomComboBox = /** @class */ (function (_super) {
        __extends(AtomComboBox, _super);
        function AtomComboBox(app, e) {
            var _this = _super.call(this, app, e || document.createElement("select")) || this;
            _this.allowMultipleSelection = false;
            return _this;
        }
        AtomComboBox.prototype.onCollectionChanged = function (key, index, item) {
            _super.prototype.onCollectionChanged.call(this, key, index, item);
            try {
                this.isChanging = true;
                var se = this.element;
                se.selectedIndex = this.selectedIndex;
            }
            finally {
                this.isChanging = false;
            }
        };
        AtomComboBox.prototype.updateSelectionBindings = function () {
            _super.prototype.updateSelectionBindings.call(this);
            try {
                if (this.isChanging) {
                    return;
                }
                this.isChanging = true;
                var se = this.element;
                se.selectedIndex = this.selectedIndex;
            }
            finally {
                this.isChanging = false;
            }
        };
        AtomComboBox.prototype.preCreate = function () {
            var _this = this;
            _super.prototype.preCreate.call(this);
            this.itemTemplate = AtomComboBoxItemTemplate;
            this.runAfterInit(function () {
                _this.bindEvent(_this.element, "change", function (s) {
                    if (_this.isChanging) {
                        return;
                    }
                    try {
                        _this.isChanging = true;
                        var index = _this.element.selectedIndex;
                        if (index === -1) {
                            _this.selectedItems.clear();
                            return;
                        }
                        _this.selectedItem = _this.items[index];
                        // this.selectedIndex = (this.element as HTMLSelectElement).selectedIndex;
                    }
                    finally {
                        _this.isChanging = false;
                    }
                });
            });
        };
        AtomComboBox = __decorate([
            __param(0, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App, HTMLElement])
        ], AtomComboBox);
        return AtomComboBox;
    }(AtomItemsControl_1.AtomItemsControl));
    exports.AtomComboBox = AtomComboBox;
    var AtomComboBoxItemTemplate = /** @class */ (function (_super) {
        __extends(AtomComboBoxItemTemplate, _super);
        function AtomComboBoxItemTemplate(app, e) {
            return _super.call(this, app, e || document.createElement("option")) || this;
        }
        AtomComboBoxItemTemplate.prototype.create = function () {
            var _this = this;
            this.bind(this.element, "text", [["data"]], false, function (v) {
                var ip = _this.element._templateParent;
                return v[ip.labelPath];
            });
        };
        return AtomComboBoxItemTemplate;
    }(AtomControl_1.AtomControl));
});
//# sourceMappingURL=AtomComboBox.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomComboBox");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/web/controls/AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("@web-atoms/core/dist/core/Bind");
    var XNode_1 = require("@web-atoms/core/dist/core/XNode");
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var HelpPopup = /** @class */ (function (_super) {
        __extends(HelpPopup, _super);
        function HelpPopup() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        HelpPopup.prototype.create = function () {
            var _this = this;
            this.render(XNode_1.default.create("div", { style: "padding:10px; margin:5px; border: 1px solid lightgray; background-color: white; border-radius: 5px;" },
                XNode_1.default.create("span", { text: Bind_1.default.oneWay(function () { return _this.viewModel.message; }) })));
        };
        return HelpPopup;
    }(AtomControl_1.AtomControl));
    exports.default = HelpPopup;
});
//# sourceMappingURL=HelpPopup.js.map

    AmdLoader.instance.setup("@web-atoms/web-controls/dist/form/HelpPopup");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBinder", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/web/controls/AtomControl", "./HelpPopup"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AtomBinder_1 = require("@web-atoms/core/dist/core/AtomBinder");
    var NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var HelpPopup_1 = require("./HelpPopup");
    Object.defineProperty(exports, "HP", { enumerable: true, get: function () { return HelpPopup_1.default; } });
    var AtomField = /** @class */ (function (_super) {
        __extends(AtomField, _super);
        function AtomField() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomField.prototype, "hasError", {
            get: function () {
                return this.error ? true : false;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomField.prototype, "hasHelp", {
            get: function () {
                return (this.helpText || this.helpLink) ? true : false;
            },
            enumerable: false,
            configurable: true
        });
        AtomField.prototype.onPropertyChanged = function (name) {
            switch (name) {
                case "error":
                    AtomBinder_1.AtomBinder.refreshValue(this, "hasError");
                    break;
                case "helpLink":
                case "helpText":
                    AtomBinder_1.AtomBinder.refreshValue(this, "hasHelp");
                    break;
            }
        };
        AtomField.prototype.openHelp = function () {
            return __awaiter(this, void 0, void 0, function () {
                var n;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            n = this.app.resolve(NavigationService_1.NavigationService);
                            if (!this.helpText) return [3 /*break*/, 2];
                            return [4 /*yield*/, n.openPage("@web-atoms/web-controls/dist/form/HelpPopup", { message: this.helpText })];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                        case 2:
                            if (!this.helpLink) return [3 /*break*/, 4];
                            // if it is http, then open it inside iframe..
                            // pending
                            // else
                            return [4 /*yield*/, n.openPage(this.helpLink)];
                        case 3:
                            // if it is http, then open it inside iframe..
                            // pending
                            // else
                            _a.sent();
                            _a.label = 4;
                        case 4: return [2 /*return*/];
                    }
                });
            });
        };
        AtomField.prototype.preCreate = function () {
            this.label = null;
            this.error = null;
            this.helpText = null;
            this.helpLink = null;
            this.helpIcon = null;
            this.required = false;
            this.visible = true;
            this.fieldClass = "";
        };
        return AtomField;
    }(AtomControl_1.AtomControl));
    exports.default = AtomField;
});
//# sourceMappingURL=AtomField.js.map

    AmdLoader.instance.setup("@web-atoms/web-controls/dist/form/AtomField");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
    var Colors_1 = require("@web-atoms/core/dist/core/Colors");
    var AtomStyle_1 = require("@web-atoms/core/dist/web/styles/AtomStyle");
    var AtomFormStyle = /** @class */ (function (_super) {
        __extends(AtomFormStyle, _super);
        function AtomFormStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomFormStyle.prototype, "root", {
            get: function () {
                return {
                    subclasses: {
                        " > .form-field": this.field
                    }
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomFormStyle.prototype, "field", {
            get: function () {
                return {
                    subclasses: {
                        ".has-error": {
                            backgroundColor: Colors_1.default.red.withAlphaPercent(0.1)
                        },
                        ".field-hidden": {
                            display: "none"
                        },
                        " > .help": {
                            marginLeft: "5px",
                            borderRadius: "50%",
                            display: "inline-block",
                            width: "10px",
                            height: "10px",
                            padding: "3px",
                            textAlign: "center",
                            color: Colors_1.default.white,
                            backgroundColor: Colors_1.default.limeGreen,
                            cursor: "pointer",
                            fontSize: "70%"
                        },
                        " > .label": {
                            fontSize: "70%"
                        },
                        " > .required": {
                            fontSize: "70%",
                            color: Colors_1.default.red
                        },
                        " > .presenter": {
                            display: "block",
                            clear: "both"
                        },
                        " > .error": {
                            display: "inline-block",
                            clear: "both",
                            color: Colors_1.default.white,
                            backgroundColor: Colors_1.default.red,
                            fontWeight: "bold",
                            padding: "3px",
                            borderRadius: "3px",
                            fontSize: "70%"
                        }
                    }
                };
            },
            enumerable: false,
            configurable: true
        });
        return AtomFormStyle;
    }(AtomStyle_1.AtomStyle));
    exports.default = AtomFormStyle;
});
//# sourceMappingURL=AtomFormStyle.js.map

    AmdLoader.instance.setup("@web-atoms/web-controls/dist/form/AtomFormStyle");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/web/controls/AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var AtomFieldTemplate = /** @class */ (function (_super) {
        __extends(AtomFieldTemplate, _super);
        function AtomFieldTemplate() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        AtomFieldTemplate.prototype.preCreate = function () {
            var _this = this;
            _super.prototype.preCreate.call(this);
            this.contentPresenter = null;
            this.labelPresenter = null;
            this.runAfterInit(function () {
                _this.contentPresenter.appendChild(_this.field.element);
                var input = _this.field.element.getElementsByTagName("input")[0];
                if (input) {
                    var label = _this.labelPresenter;
                    input.id = input.id || (input.id = "__id__" + AtomFieldTemplate.labelIDs++);
                    label.htmlFor = input.id;
                }
            });
        };
        AtomFieldTemplate.labelIDs = 1;
        return AtomFieldTemplate;
    }(AtomControl_1.AtomControl));
    exports.default = AtomFieldTemplate;
});
//# sourceMappingURL=AtomFieldTemplate.js.map

    AmdLoader.instance.setup("@web-atoms/web-controls/dist/form/AtomFieldTemplate");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "./AtomFieldTemplate"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("@web-atoms/core/dist/core/Bind");
    var XNode_1 = require("@web-atoms/core/dist/core/XNode");
    var AtomFieldTemplate_1 = require("./AtomFieldTemplate");
    var DefaultFieldTemplate = /** @class */ (function (_super) {
        __extends(DefaultFieldTemplate, _super);
        function DefaultFieldTemplate() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        DefaultFieldTemplate.prototype.create = function () {
            var _this = this;
            this.render(XNode_1.default.create("div", { class: Bind_1.default.oneWay(function () {
                    var _a;
                    return (_a = {
                            "form-field": 1
                        },
                        _a[_this.field.fieldClass] = _this.field.fieldClass,
                        _a["hasError"] = _this.field.hasError,
                        _a["field-hidden"] = !_this.field.visible,
                        _a);
                }) },
                XNode_1.default.create("label", { presenter: Bind_1.default.presenter("labelPresenter"), class: "label", text: Bind_1.default.oneWay(function () { return _this.field.label; }) }),
                XNode_1.default.create("span", { class: "required", styleDisplay: Bind_1.default.oneTime(function () { return _this.field.required ? "" : "none"; }) }, "*"),
                XNode_1.default.create("div", { presenter: Bind_1.default.presenter("contentPresenter"), class: "presenter" }),
                XNode_1.default.create("span", { class: "error", styleDisplay: Bind_1.default.oneWay(function () { return _this.field.hasError ? "" : "none"; }), text: Bind_1.default.oneWay(function () { return _this.field.error; }) })));
        };
        return DefaultFieldTemplate;
    }(AtomFieldTemplate_1.default));
    exports.default = DefaultFieldTemplate;
});
//# sourceMappingURL=DefaultFieldTemplate.js.map

    AmdLoader.instance.setup("@web-atoms/web-controls/dist/form/DefaultFieldTemplate");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/AtomBridge", "@web-atoms/core/dist/web/controls/AtomControl", "./AtomField", "./AtomFormStyle", "./DefaultFieldTemplate"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AtomBridge_1 = require("@web-atoms/core/dist/core/AtomBridge");
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var AtomField_1 = require("./AtomField");
    var AtomFormStyle_1 = require("./AtomFormStyle");
    var DefaultFieldTemplate_1 = require("./DefaultFieldTemplate");
    var AtomForm = /** @class */ (function (_super) {
        __extends(AtomForm, _super);
        function AtomForm() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.focusNextOnEnter = /mobile/i.test(navigator.userAgent);
            return _this;
        }
        AtomForm.prototype.append = function (e) {
            // you can create nested AtomForm
            if (e instanceof AtomForm) {
                return _super.prototype.append.call(this, e);
            }
            if (!(e instanceof AtomField_1.default)) {
                throw new Error("Only AtomField or AtomFormGroup can be added inside AtomForm");
            }
            var fieldContainer = this.createField(e);
            return _super.prototype.append.call(this, fieldContainer);
        };
        AtomForm.prototype.createField = function (e) {
            var field = new (this.fieldTemplate)(this.app);
            field.field = e;
            return field;
        };
        AtomForm.prototype.preCreate = function () {
            var _this = this;
            _super.prototype.preCreate.call(this);
            this.defaultControlStyle = AtomFormStyle_1.default;
            this.fieldTemplate = DefaultFieldTemplate_1.default;
            this.runAfterInit(function () {
                _this.element.classList.add(_this.controlStyle.name);
                _this.app.callLater(function () {
                    AtomBridge_1.AtomBridge.instance.refreshInherited(_this, "viewModel");
                    AtomBridge_1.AtomBridge.instance.refreshInherited(_this, "localViewModel");
                    AtomBridge_1.AtomBridge.instance.refreshInherited(_this, "data");
                });
                _this.watchKeyInput();
            });
        };
        AtomForm.prototype.watchKeyInput = function () {
            var _this = this;
            this.bindEvent(this.element, "keypress", function (e) {
                _this.onKeyPress(e);
            });
        };
        AtomForm.prototype.onKeyPress = function (e) {
            if (!this.focusNextOnEnter) {
                return;
            }
            var target = e.target;
            if (!/input/i.test(target.tagName)) {
                return;
            }
            var input = target;
            if (e.keyCode === 13) {
                if (/submit/i.test(input.className)) {
                    this.fireSubmitEvent(input);
                    return;
                }
                var next = this.focusNextInput(target);
                if (next) {
                    next.focus();
                }
                else {
                    this.fireSubmitEvent(input);
                }
            }
        };
        AtomForm.prototype.fireSubmitEvent = function (target) {
            var _this = this;
            // fire change event...
            target.dispatchEvent(new Event("change"));
            this.app.callLater(function () {
                var e = new CustomEvent("submit", { bubbles: false, cancelable: false });
                _this.element.dispatchEvent(e);
            });
        };
        AtomForm.prototype.focusNextInput = function (target) {
            var found = false;
            var result = null;
            function find(e) {
                if (result) {
                    return;
                }
                var isText = /input|textarea/i.test(e.tagName);
                if (found) {
                    if (isText) {
                        result = e;
                        return;
                    }
                }
                if (e === target) {
                    found = true;
                }
                var child = e.firstElementChild;
                if (child) {
                    find(child);
                }
                var next = e.nextElementSibling;
                if (next) {
                    find(next);
                }
            }
            find(document.body);
            return result;
        };
        return AtomForm;
    }(AtomControl_1.AtomControl));
    exports.default = AtomForm;
});
//# sourceMappingURL=AtomForm.js.map

    AmdLoader.instance.setup("@web-atoms/web-controls/dist/form/AtomForm");

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
        var t = target;
        var inits = t._$_inits = t._$_inits || [];
        inits.push(fx);
    }
    exports.registerInit = registerInit;
});
//# sourceMappingURL=baseTypes.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/baseTypes");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var NavigationService_1 = require("../services/NavigationService");
    var baseTypes_1 = require("./baseTypes");
    /**
     * Reports an alert to user when an error has occurred
     * or validation has failed.
     * If you set success message, it will display an alert with success message.
     * If you set confirm message, it will ask form confirmation before executing this method.
     * You can configure options to enable/disable certain
     * alerts.
     * @param reportOptions
     */
    function Action(_a) {
        var _b = _a === void 0 ? {} : _a, _c = _b.success, success = _c === void 0 ? null : _c, _d = _b.successTitle, successTitle = _d === void 0 ? "Done" : _d, _e = _b.confirm, confirm = _e === void 0 ? null : _e, _f = _b.confirmTitle, confirmTitle = _f === void 0 ? null : _f, _g = _b.validate, validate = _g === void 0 ? false : _g, _h = _b.validateTitle, validateTitle = _h === void 0 ? null : _h;
        // tslint:disable-next-line: only-arrow-functions
        return function (target, key) {
            baseTypes_1.registerInit(target, function (vm) {
                // tslint:disable-next-line: ban-types
                var oldMethod = vm[key];
                var app = vm.app;
                // tslint:disable-next-line:only-arrow-functions
                vm[key] = function () {
                    var a = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        a[_i] = arguments[_i];
                    }
                    return __awaiter(this, void 0, void 0, function () {
                        var ns, vMsg, pe, result, e_1;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0:
                                    ns = app.resolve(NavigationService_1.NavigationService);
                                    _a.label = 1;
                                case 1:
                                    _a.trys.push([1, 10, , 12]);
                                    if (!validate) return [3 /*break*/, 3];
                                    if (!!vm.isValid) return [3 /*break*/, 3];
                                    vMsg = typeof validate === "boolean"
                                        ? "Please enter correct information"
                                        : validate;
                                    return [4 /*yield*/, ns.alert(vMsg, validateTitle || "Error")];
                                case 2:
                                    _a.sent();
                                    return [2 /*return*/];
                                case 3:
                                    if (!confirm) return [3 /*break*/, 5];
                                    return [4 /*yield*/, ns.confirm(confirm, confirmTitle || "Confirm")];
                                case 4:
                                    if (!(_a.sent())) {
                                        return [2 /*return*/];
                                    }
                                    _a.label = 5;
                                case 5:
                                    pe = oldMethod.apply(vm, a);
                                    if (!(pe && pe.then)) return [3 /*break*/, 9];
                                    return [4 /*yield*/, pe];
                                case 6:
                                    result = _a.sent();
                                    if (!success) return [3 /*break*/, 8];
                                    return [4 /*yield*/, ns.alert(success, successTitle)];
                                case 7:
                                    _a.sent();
                                    _a.label = 8;
                                case 8: return [2 /*return*/, result];
                                case 9: return [3 /*break*/, 12];
                                case 10:
                                    e_1 = _a.sent();
                                    if (/^(cancelled|canceled)$/i.test(e_1.toString().trim())) {
                                        // tslint:disable-next-line: no-console
                                        console.warn(e_1);
                                        return [2 /*return*/];
                                    }
                                    return [4 /*yield*/, ns.alert(e_1, "Error")];
                                case 11:
                                    _a.sent();
                                    return [3 /*break*/, 12];
                                case 12: return [2 /*return*/];
                            }
                        });
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
        define(["require", "exports", "./AtomBinder"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.BindableProperty = void 0;
    var AtomBinder_1 = require("./AtomBinder");
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
        var iVal = target[key];
        var keyName = "_" + key;
        target[keyName] = iVal;
        // property getter
        var getter = function () {
            // console.log(`Get: ${key} => ${_val}`);
            return this[keyName];
        };
        // property setter
        var setter = function (newVal) {
            // console.log(`Set: ${key} => ${newVal}`);
            var oldValue = this[keyName];
            // tslint:disable-next-line:triple-equals
            if (oldValue === undefined ? oldValue === newVal : oldValue == newVal) {
                return;
            }
            var ce = this;
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var App_1 = require("../App");
    var Atom_1 = require("../Atom");
    var AtomBinder_1 = require("../core/AtomBinder");
    var AtomDisposableList_1 = require("../core/AtomDisposableList");
    var AtomWatcher_1 = require("../core/AtomWatcher");
    var BindableProperty_1 = require("../core/BindableProperty");
    var Inject_1 = require("../di/Inject");
    var baseTypes_1 = require("./baseTypes");
    function runDecoratorInits() {
        var v = this.constructor.prototype;
        if (!v) {
            return;
        }
        var ris = v._$_inits;
        if (ris) {
            for (var _i = 0, ris_1 = ris; _i < ris_1.length; _i++) {
                var ri = ris_1[_i];
                ri.call(this, this);
            }
        }
    }
    function privateInit() {
        return __awaiter(this, void 0, void 0, function () {
            var _i, _a, i, pi, _b, pi_1, iterator;
            var _this = this;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0:
                        _c.trys.push([0, , 3, 4]);
                        return [4 /*yield*/, Atom_1.Atom.postAsync(this.app, function () { return __awaiter(_this, void 0, void 0, function () {
                                return __generator(this, function (_a) {
                                    runDecoratorInits.apply(this);
                                    return [2 /*return*/];
                                });
                            }); })];
                    case 1:
                        _c.sent();
                        return [4 /*yield*/, Atom_1.Atom.postAsync(this.app, function () { return __awaiter(_this, void 0, void 0, function () {
                                return __generator(this, function (_a) {
                                    switch (_a.label) {
                                        case 0: return [4 /*yield*/, this.init()];
                                        case 1:
                                            _a.sent();
                                            this.onReady();
                                            return [2 /*return*/];
                                    }
                                });
                            }); })];
                    case 2:
                        _c.sent();
                        if (this.postInit) {
                            for (_i = 0, _a = this.postInit; _i < _a.length; _i++) {
                                i = _a[_i];
                                i();
                            }
                            this.postInit = null;
                        }
                        return [3 /*break*/, 4];
                    case 3:
                        pi = this.pendingInits;
                        this.pendingInits = null;
                        for (_b = 0, pi_1 = pi; _b < pi_1.length; _b++) {
                            iterator = pi_1[_b];
                            iterator();
                        }
                        return [7 /*endfinally*/];
                    case 4: return [2 /*return*/];
                }
            });
        });
    }
    /**
     * Useful only for Unit testing, this function will await till initialization is
     * complete and all pending functions are executed
     */
    function waitForReady(vm) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!vm.pendingInits) return [3 /*break*/, 2];
                        return [4 /*yield*/, Atom_1.Atom.delay(100)];
                    case 1:
                        _a.sent();
                        return [3 /*break*/, 0];
                    case 2: return [2 /*return*/];
                }
            });
        });
    }
    exports.waitForReady = waitForReady;
    /**
     * ViewModel class supports initialization and supports {@link IDisposable} dispose pattern.
     * @export
     * @class AtomViewModel
     */
    var AtomViewModel = /** @class */ (function () {
        function AtomViewModel(app) {
            var _this = this;
            this.app = app;
            this.disposables = null;
            this.validations = [];
            this.pendingInits = [];
            this.mShouldValidate = false;
            this.app.runAsync(function () { return privateInit.apply(_this); });
        }
        Object.defineProperty(AtomViewModel.prototype, "isReady", {
            /**
             * If it returns true, it means all pending initializations have finished
             */
            get: function () {
                return this.pendingInits === null;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomViewModel.prototype, "errors", {
            get: function () {
                var e = [];
                if (!this.mShouldValidate) {
                    return e;
                }
                for (var _i = 0, _a = this.validations; _i < _a.length; _i++) {
                    var v = _a[_i];
                    if (!v.initialized) {
                        return e;
                    }
                    var error = this[v.name];
                    if (error) {
                        e.push({ name: v.name, error: error });
                    }
                }
                return e;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomViewModel.prototype, "parent", {
            /**
             * Returns parent AtomViewModel if it was initialized with one. This property is also
             * useful when you open an popup or window. Whenever a popup/window is opened, ViewModel
             * associated with the UI element that opened this popup/window becomes parent of ViewModel
             * of popup/window.
             */
            get: function () {
                return this.mParent;
            },
            set: function (v) {
                var _this = this;
                if (this.mParent && this.mParent.mChildren) {
                    this.mParent.mChildren.remove(this);
                }
                this.mParent = v;
                if (v) {
                    var c_1 = v.mChildren || (v.mChildren = []);
                    c_1.add(this);
                    this.registerDisposable({
                        dispose: function () {
                            c_1.remove(_this);
                        }
                    });
                }
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomViewModel.prototype, "isValid", {
            /**
             * Returns true if all validations didn't return any error. All validations
             * are decorated with @{@link Validate} decorator.
             */
            get: function () {
                var valid = true;
                var validateWasFalse = this.mShouldValidate === false;
                this.mShouldValidate = true;
                for (var _i = 0, _a = this.validations; _i < _a.length; _i++) {
                    var v = _a[_i];
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
                    for (var _b = 0, _c = this.mChildren; _b < _c.length; _b++) {
                        var child = _c[_b];
                        if (!child.isValid) {
                            valid = false;
                        }
                    }
                }
                AtomBinder_1.AtomBinder.refreshValue(this, "errors");
                return valid;
            },
            enumerable: false,
            configurable: true
        });
        /**
         * Resets validations and all errors are removed.
         * @param resetChildren reset child view models as well. Default is true.
         */
        AtomViewModel.prototype.resetValidations = function (resetChildren) {
            if (resetChildren === void 0) { resetChildren = true; }
            this.mShouldValidate = false;
            for (var _i = 0, _a = this.validations; _i < _a.length; _i++) {
                var v = _a[_i];
                this.refresh(v.name);
            }
            if (resetChildren && this.mChildren) {
                for (var _b = 0, _c = this.mChildren; _b < _c.length; _b++) {
                    var iterator = _c[_b];
                    iterator.resetValidations(resetChildren);
                }
            }
        };
        /**
         * Runs function after initialization is complete.
         * @param f function to execute
         */
        AtomViewModel.prototype.runAfterInit = function (f) {
            if (this.pendingInits) {
                this.pendingInits.push(f);
                return;
            }
            f();
        };
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
        AtomViewModel.prototype.refresh = function (name) {
            AtomBinder_1.AtomBinder.refreshValue(this, name);
        };
        /**
         * Put your asynchronous initialization here
         *
         * @returns {Promise<any>}
         * @memberof AtomViewModel
         */
        // tslint:disable-next-line:no-empty
        AtomViewModel.prototype.init = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/];
                });
            });
        };
        /**
         * dispose method will be called when attached view will be disposed or
         * when a new view model will be assigned to view, old view model will be disposed.
         *
         * @memberof AtomViewModel
         */
        AtomViewModel.prototype.dispose = function () {
            if (this.disposables) {
                this.disposables.dispose();
            }
        };
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
        AtomViewModel.prototype.registerDisposable = function (d) {
            this.disposables = this.disposables || new AtomDisposableList_1.AtomDisposableList();
            return this.disposables.add(d);
        };
        // tslint:disable-next-line:no-empty
        AtomViewModel.prototype.onReady = function () { };
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
        AtomViewModel.prototype.setupWatch = function (ft, proxy, forValidation, name) {
            var d = new AtomWatcher_1.AtomWatcher(this, ft, proxy, this);
            if (forValidation) {
                this.validations = this.validations || [];
                this.validations.push({ name: name, watcher: d, initialized: false });
            }
            else {
                d.init();
            }
            return this.registerDisposable(d);
        };
        // tslint:disable-next-line:no-empty
        AtomViewModel.prototype.onPropertyChanged = function (name) { };
        AtomViewModel = __decorate([
            __param(0, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App])
        ], AtomViewModel);
        return AtomViewModel;
    }());
    exports.AtomViewModel = AtomViewModel;
    /**
     * Receive messages for given channel
     * @param {(string | RegExp)} channel
     * @returns {Function}
     */
    function Receive() {
        var channel = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            channel[_i] = arguments[_i];
        }
        return function (target, key) {
            baseTypes_1.registerInit(target, function (vm) {
                // tslint:disable-next-line:ban-types
                var fx = vm[key];
                var a = function (ch, d) {
                    var p = fx.call(vm, ch, d);
                    if (p && p.then && p.catch) {
                        p.catch(function (e) {
                            // tslint:disable-next-line: no-console
                            console.warn(e);
                        });
                    }
                };
                var ivm = vm;
                for (var _i = 0, channel_1 = channel; _i < channel_1.length; _i++) {
                    var c = channel_1[_i];
                    ivm.registerDisposable(ivm.app.subscribe(c, a));
                }
            });
        };
    }
    exports.Receive = Receive;
    function BindableReceive() {
        var channel = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            channel[_i] = arguments[_i];
        }
        return function (target, key) {
            var bp = BindableProperty_1.BindableProperty(target, key);
            baseTypes_1.registerInit(target, function (vm) {
                var fx = function (cx, m) {
                    vm[key] = m;
                };
                var ivm = vm;
                for (var _i = 0, channel_2 = channel; _i < channel_2.length; _i++) {
                    var c = channel_2[_i];
                    ivm.registerDisposable(ivm.app.subscribe(c, fx));
                }
            });
            return bp;
        };
    }
    exports.BindableReceive = BindableReceive;
    function BindableBroadcast() {
        var channel = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            channel[_i] = arguments[_i];
        }
        return function (target, key) {
            var bp = BindableProperty_1.BindableProperty(target, key);
            baseTypes_1.registerInit(target, function (vm) {
                var fx = function (t) {
                    var v = vm[key];
                    for (var _i = 0, channel_3 = channel; _i < channel_3.length; _i++) {
                        var c = channel_3[_i];
                        vm.app.broadcast(c, v);
                    }
                };
                var d = new AtomWatcher_1.AtomWatcher(vm, [key.split(".")], fx);
                d.init();
                vm.registerDisposable(d);
            });
            return bp;
        };
    }
    exports.BindableBroadcast = BindableBroadcast;
    function Watch(target, key, descriptor) {
        baseTypes_1.registerInit(target, function (vm) {
            var ivm = vm;
            if (descriptor && descriptor.get) {
                ivm.setupWatch(descriptor.get, function () {
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
        var getMethod = descriptor.get;
        descriptor.get = (function () { return null; });
        baseTypes_1.registerInit(target, function (vm) {
            var ivm = vm;
            var fieldName = "_" + key;
            Object.defineProperty(ivm, key, {
                enumerable: true,
                configurable: true,
                get: function () {
                    var c = ivm[fieldName] || (ivm[fieldName] = {
                        value: getMethod.apply(ivm)
                    });
                    return c.value;
                }
            });
            ivm.setupWatch(getMethod, function () {
                ivm[fieldName] = null;
                AtomBinder_1.AtomBinder.refreshValue(ivm, key);
            });
        });
    }
    exports.CachedWatch = CachedWatch;
    function Validate(target, key, descriptor) {
        // tslint:disable-next-line:ban-types
        var getMethod = descriptor.get;
        // // trick is to change property descriptor...
        // delete target[key];
        descriptor.get = function () { return null; };
        // // replace it with dummy descriptor...
        // Object.defineProperty(target, key, descriptor);
        baseTypes_1.registerInit(target, function (vm) {
            var initialized = { i: false };
            var ivm = vm;
            Object.defineProperty(ivm, key, {
                enumerable: true,
                configurable: true,
                get: function () {
                    if (vm.mShouldValidate && initialized.i) {
                        return getMethod.apply(this);
                    }
                    return null;
                }
            });
            ivm.setupWatch(getMethod, function () {
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var ExpressionParser_1 = require("../core/ExpressionParser");
    var types_1 = require("../core/types");
    var NavigationService_1 = require("../services/NavigationService");
    var baseTypes_1 = require("./baseTypes");
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
    function Load(_a) {
        var init = _a.init, showErrorOnInit = _a.showErrorOnInit, watch = _a.watch, watchDelayMS = _a.watchDelayMS;
        // tslint:disable-next-line: only-arrow-functions
        return function (target, key) {
            var _this = this;
            baseTypes_1.registerInit(target, function (vm) {
                // tslint:disable-next-line: ban-types
                var oldMethod = vm[key];
                var app = vm.app;
                var showError = init ? (showErrorOnInit ? true : false) : true;
                var ct = new types_1.CancelToken();
                /**
                 * For the special case of init and watch both are true,
                 * we need to make sure that watch is ignored for first run
                 *
                 * So executing is set to true for the first time
                 */
                var executing = init;
                var m = function (ctx) { return __awaiter(_this, void 0, void 0, function () {
                    var ns, pe, e_1;
                    return __generator(this, function (_a) {
                        switch (_a.label) {
                            case 0:
                                ns = app.resolve(NavigationService_1.NavigationService);
                                _a.label = 1;
                            case 1:
                                _a.trys.push([1, 4, 6, 7]);
                                pe = oldMethod.call(vm, ctx);
                                if (!(pe && pe.then)) return [3 /*break*/, 3];
                                return [4 /*yield*/, pe];
                            case 2: return [2 /*return*/, _a.sent()];
                            case 3: return [3 /*break*/, 7];
                            case 4:
                                e_1 = _a.sent();
                                if (/^(cancelled|canceled)$/i.test(e_1.toString().trim())) {
                                    // tslint:disable-next-line: no-console
                                    console.warn(e_1);
                                    return [2 /*return*/];
                                }
                                if (!showError) {
                                    // tslint:disable-next-line: no-console
                                    console.error(e_1);
                                    return [2 /*return*/];
                                }
                                return [4 /*yield*/, ns.alert(e_1, "Error")];
                            case 5:
                                _a.sent();
                                return [3 /*break*/, 7];
                            case 6:
                                showError = true;
                                executing = false;
                                return [7 /*endfinally*/];
                            case 7: return [2 /*return*/];
                        }
                    });
                }); };
                if (watch) {
                    var fx_1 = function (c1) { return __awaiter(_this, void 0, void 0, function () {
                        var ct2, ex1_1;
                        return __generator(this, function (_a) {
                            switch (_a.label) {
                                case 0:
                                    if (ct) {
                                        ct.cancel();
                                    }
                                    ct2 = ct = (c1 || new types_1.CancelToken());
                                    if (executing) {
                                        return [2 /*return*/];
                                    }
                                    executing = true;
                                    _a.label = 1;
                                case 1:
                                    _a.trys.push([1, 3, 4, 5]);
                                    return [4 /*yield*/, m(ct2)];
                                case 2:
                                    _a.sent();
                                    return [3 /*break*/, 5];
                                case 3:
                                    ex1_1 = _a.sent();
                                    if (/^(cancelled|canceled)$/i.test(ex1_1.toString().trim())) {
                                        // tslint:disable-next-line: no-console
                                        console.warn(ex1_1);
                                    }
                                    else {
                                        // tslint:disable-next-line: no-console
                                        console.error(ex1_1);
                                    }
                                    return [3 /*break*/, 5];
                                case 4:
                                    executing = false;
                                    ct = null;
                                    return [7 /*endfinally*/];
                                case 5: return [2 /*return*/];
                            }
                        });
                    }); };
                    var timeout_1 = null;
                    // get path stripped as we are passing CancelToken, it will not
                    // parse for this. expressions..
                    var pathList = ExpressionParser_1.parsePath(oldMethod.toString(), true);
                    if (pathList.length === 0) {
                        throw new Error("Nothing to watch !!");
                    }
                    vm.setupWatch(pathList, function () {
                        if (executing) {
                            return;
                        }
                        if (timeout_1) {
                            clearTimeout(timeout_1);
                        }
                        timeout_1 = setTimeout(function () {
                            timeout_1 = null;
                            fx_1();
                        }, watchDelayMS || 100);
                    });
                    vm[key] = fx_1;
                }
                if (init) {
                    app.runAsync(function () { return m.call(vm, ct); });
                }
            });
        };
    }
    exports.default = Load;
});
//# sourceMappingURL=Load.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/Load");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/types", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/view-model/Action", "@web-atoms/core/dist/view-model/AtomViewModel", "@web-atoms/core/dist/view-model/Load", "./SignupService"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var types_1 = require("@web-atoms/core/dist/core/types");
    var Inject_1 = require("@web-atoms/core/dist/di/Inject");
    var Action_1 = require("@web-atoms/core/dist/view-model/Action");
    var AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    var Load_1 = require("@web-atoms/core/dist/view-model/Load");
    var SignupService_1 = require("./SignupService");
    var SimpleViewModel = /** @class */ (function (_super) {
        __extends(SimpleViewModel, _super);
        function SimpleViewModel() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.model = {
                firstName: "",
                lastName: "",
                emailAddress: "",
                password: "",
                passwordAgain: "",
                country: "IN"
            };
            return _this;
        }
        Object.defineProperty(SimpleViewModel.prototype, "errorFirstName", {
            /**
             * Validate decorator will begin watching changes in property and it will
             * return error if validation failed. First time, when the form is empty,
             * no error will be displayed.
             *
             * But as soon as you hit `this.isValid` or method is decorated with `@Action({ validate: true})`
             * the errors bound to UI element will display an error. And it will automatically remove when
             * the property is modified.
             */
            get: function () {
                return this.model.firstName ? null : "First name is required";
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(SimpleViewModel.prototype, "errorLastName", {
            get: function () {
                return this.model.lastName ? null : "Last name is required";
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(SimpleViewModel.prototype, "errorEmailAddress", {
            get: function () {
                var email = this.model.emailAddress;
                if (!email) {
                    return "Email address is required";
                }
                if (!/@/i.test(email)) {
                    return "Email address is invalid";
                }
                return null;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(SimpleViewModel.prototype, "errorPassword", {
            get: function () {
                return this.model.password ? null : "Password is required";
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(SimpleViewModel.prototype, "errorPasswordAgain", {
            get: function () {
                return this.model.passwordAgain === this.model.password ? null : "Passwords do not match";
            },
            enumerable: true,
            configurable: true
        });
        /**
         * This method will be executed automatically when view model is initialized.
         */
        SimpleViewModel.prototype.loadCountries = function () {
            return __awaiter(this, void 0, void 0, function () {
                var _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            _a = this;
                            return [4 /*yield*/, this.signupService.countries()];
                        case 1:
                            _a.countryList = _b.sent();
                            return [2 /*return*/];
                    }
                });
            });
        };
        /**
         * This method will be executed when view model is initialized. This method will
         * also be executed when any property chain of `this` e.g. `this.model.country` is
         * modified.
         */
        SimpleViewModel.prototype.loadStates = function (ct) {
            return __awaiter(this, void 0, void 0, function () {
                var _a;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            _a = this;
                            return [4 /*yield*/, this.signupService.states(this.model.country, ct)];
                        case 1:
                            _a.stateList = _b.sent();
                            this.model.state = this.stateList[0].value;
                            return [2 /*return*/];
                    }
                });
            });
        };
        /**
         * Argument `validate` true will force validation error to be displayed and it will stop execution if
         * there are any validations that have failed.
         *
         * Argument `success` will display an alert with message if the execution was successful.
         *
         * By default this method will display an alert if there was any exception while trying to execute
         * this method.
         */
        SimpleViewModel.prototype.signup = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, this.signupService.signup(this.model)];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        };
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], SimpleViewModel.prototype, "errorFirstName", null);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], SimpleViewModel.prototype, "errorLastName", null);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], SimpleViewModel.prototype, "errorEmailAddress", null);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], SimpleViewModel.prototype, "errorPassword", null);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], SimpleViewModel.prototype, "errorPasswordAgain", null);
        __decorate([
            Inject_1.Inject,
            __metadata("design:type", SignupService_1.default)
        ], SimpleViewModel.prototype, "signupService", void 0);
        __decorate([
            Load_1.default({ init: true }),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", []),
            __metadata("design:returntype", Promise)
        ], SimpleViewModel.prototype, "loadCountries", null);
        __decorate([
            Load_1.default({ init: true, watch: true, watchDelayMS: 1 }),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", [types_1.CancelToken]),
            __metadata("design:returntype", Promise)
        ], SimpleViewModel.prototype, "loadStates", null);
        __decorate([
            Action_1.default({ validate: true, success: "Signup success" }),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", []),
            __metadata("design:returntype", Promise)
        ], SimpleViewModel.prototype, "signup", null);
        return SimpleViewModel;
    }(AtomViewModel_1.AtomViewModel));
    exports.default = SimpleViewModel;
});
//# sourceMappingURL=SimpleViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/samples/web/form/simple/SimpleViewModel");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/web/controls/AtomComboBox", "@web-atoms/core/dist/web/controls/AtomControl", "@web-atoms/web-controls/dist/form/AtomField", "@web-atoms/web-controls/dist/form/AtomForm", "./SimpleViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("@web-atoms/core/dist/core/Bind");
    var XNode_1 = require("@web-atoms/core/dist/core/XNode");
    var AtomComboBox_1 = require("@web-atoms/core/dist/web/controls/AtomComboBox");
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var AtomField_1 = require("@web-atoms/web-controls/dist/form/AtomField");
    var AtomForm_1 = require("@web-atoms/web-controls/dist/form/AtomForm");
    var SimpleViewModel_1 = require("./SimpleViewModel");
    var SimpleForm = /** @class */ (function (_super) {
        __extends(SimpleForm, _super);
        function SimpleForm() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        SimpleForm.prototype.create = function () {
            var _this = this;
            this.viewModel = this.resolve(SimpleViewModel_1.default);
            this.render(XNode_1.default.create("div", null,
                XNode_1.default.create(AtomForm_1.default, null,
                    XNode_1.default.create(AtomField_1.default, { label: "First name:", required: "true", error: Bind_1.default.oneWay(function (x) { return x.viewModel.errorFirstName; }) },
                        XNode_1.default.create("input", { type: "text", value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.firstName; }) })),
                    XNode_1.default.create(AtomField_1.default, { label: "Last name:", required: "true", error: Bind_1.default.oneWay(function (x) { return x.viewModel.errorLastName; }) },
                        XNode_1.default.create("input", { type: "text", value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.lastName; }) })),
                    XNode_1.default.create(AtomField_1.default, { label: "Email Address:", required: "true", error: Bind_1.default.oneWay(function (x) { return x.viewModel.errorEmailAddress; }), helpText: "We will send you email to verify your account." },
                        XNode_1.default.create("input", { type: "text", value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.emailAddress; }) })),
                    XNode_1.default.create(AtomField_1.default, { label: "Country:" },
                        XNode_1.default.create(AtomComboBox_1.AtomComboBox, { items: Bind_1.default.oneWay(function (x) { return x.viewModel.countryList; }), value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.country; }) })),
                    XNode_1.default.create(AtomField_1.default, { label: "State:" },
                        XNode_1.default.create(AtomComboBox_1.AtomComboBox, { items: Bind_1.default.oneWay(function (x) { return x.viewModel.stateList; }), value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.state; }) })),
                    XNode_1.default.create(AtomField_1.default, { label: "Password:", required: "true", error: Bind_1.default.oneWay(function (x) { return x.viewModel.errorPassword; }) },
                        XNode_1.default.create("input", { type: "password", value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.password; }) })),
                    XNode_1.default.create(AtomField_1.default, { label: "Password (Again):", required: "true", error: Bind_1.default.oneWay(function (x) { return x.viewModel.errorPasswordAgain; }) },
                        XNode_1.default.create("input", { type: "password", value: Bind_1.default.twoWays(function (x) { return x.viewModel.model.passwordAgain; }) }))),
                XNode_1.default.create("button", { eventClick: Bind_1.default.event(function (x) { return _this.viewModel.signup(); }) }, "Signup")));
        };
        return SimpleForm;
    }(AtomControl_1.AtomControl));
    exports.default = SimpleForm;
});
//# sourceMappingURL=SimpleForm.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/samples/web/form/simple/SimpleForm");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../../core/web/FileViewer", "../../../core/web/resolveModulePath", "./simple/MockSignupService", "./simple/SignupService", "./simple/SimpleForm", "./simple/SimpleViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var FileViewer_1 = require("../../../core/web/FileViewer");
    var resolveModulePath_1 = require("../../../core/web/resolveModulePath");
    var MockSignupService_1 = require("./simple/MockSignupService");
    var SignupService_1 = require("./simple/SignupService");
    var SimpleForm_1 = require("./simple/SimpleForm");
    var SimpleViewModel_1 = require("./simple/SimpleViewModel");
    UMD.designMode = true;
    var FormDemo = /** @class */ (function (_super) {
        __extends(FormDemo, _super);
        function FormDemo() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        FormDemo.prototype.create = function () {
            _super.prototype.create.call(this);
            this.require = require;
            this.demo = SimpleForm_1.default;
            this.files = [
                "@web-atoms/samples/src/samples/web/form/simple/SimpleFormXF.tsx",
                resolveModulePath_1.default(require, SimpleForm_1.default).replace("/dist/", "/src/") + ".tsx",
                resolveModulePath_1.default(require, SimpleViewModel_1.default).replace("/dist/", "/src/") + ".ts",
                resolveModulePath_1.default(require, SignupService_1.default).replace("/dist/", "/src/") + ".ts",
                resolveModulePath_1.default(require, MockSignupService_1.default).replace("/dist/", "/src/") + ".ts"
            ];
        };
        return FormDemo;
    }(FileViewer_1.default));
    exports.default = FormDemo;
});
//# sourceMappingURL=FromDemo.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/samples/web/form/FromDemo");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/di/Inject", "@web-atoms/core/dist/services/NavigationService", "@web-atoms/core/dist/view-model/Action", "@web-atoms/core/dist/view-model/AtomViewModel"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Inject_1 = require("@web-atoms/core/dist/di/Inject");
    var NavigationService_1 = require("@web-atoms/core/dist/services/NavigationService");
    var Action_1 = require("@web-atoms/core/dist/view-model/Action");
    var AtomViewModel_1 = require("@web-atoms/core/dist/view-model/AtomViewModel");
    var IndexViewModel = /** @class */ (function (_super) {
        __extends(IndexViewModel, _super);
        function IndexViewModel() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.model = {};
            _this.collapsed = false;
            return _this;
        }
        Object.defineProperty(IndexViewModel.prototype, "errorName", {
            get: function () {
                return this.model.name ? null : "Last name is required";
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(IndexViewModel.prototype, "errorEmailAddress", {
            get: function () {
                var email = this.model.emailAddress;
                if (!email) {
                    return "Email address is required";
                }
                if (!/@/i.test(email)) {
                    return "Email address is invalid";
                }
                return null;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(IndexViewModel.prototype, "errorMessage", {
            get: function () {
                return this.model.message ? null : "Message is required";
            },
            enumerable: true,
            configurable: true
        });
        IndexViewModel.prototype.onSubmit = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/];
                });
            });
        };
        IndexViewModel.prototype.onSubscribe = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    return [2 /*return*/];
                });
            });
        };
        IndexViewModel.prototype.menuClick = function () {
            if (this.app.screen.screenType === "mobile") {
                this.collapsed = !this.collapsed;
            }
        };
        __decorate([
            Inject_1.Inject,
            __metadata("design:type", NavigationService_1.NavigationService)
        ], IndexViewModel.prototype, "windowService", void 0);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], IndexViewModel.prototype, "errorName", null);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], IndexViewModel.prototype, "errorEmailAddress", null);
        __decorate([
            AtomViewModel_1.Validate,
            __metadata("design:type", String),
            __metadata("design:paramtypes", [])
        ], IndexViewModel.prototype, "errorMessage", null);
        __decorate([
            Action_1.default({ validate: true, successTitle: "  ", success: "Email sent successfully." }),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", []),
            __metadata("design:returntype", Promise)
        ], IndexViewModel.prototype, "onSubmit", null);
        __decorate([
            Action_1.default({ successTitle: "  ", success: "Subscribed successfully." }),
            __metadata("design:type", Function),
            __metadata("design:paramtypes", []),
            __metadata("design:returntype", Promise)
        ], IndexViewModel.prototype, "onSubscribe", null);
        return IndexViewModel;
    }(AtomViewModel_1.AtomViewModel));
    exports.default = IndexViewModel;
});
//# sourceMappingURL=IndexViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/view-models/IndexViewModel");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/web/styles/AtomStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var AtomStyle_1 = require("@web-atoms/core/dist/web/styles/AtomStyle");
    var IndexStyle = /** @class */ (function (_super) {
        __extends(IndexStyle, _super);
        function IndexStyle() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.screen = _this.styleSheet.app.screen;
            return _this;
        }
        Object.defineProperty(IndexStyle.prototype, "root", {
            get: function () {
                return {
                    fontFamily: "'Merriweather Sans',-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,'Noto Sans',sans-serif,'Apple Color Emoji','Segoe UI Emoji','Segoe UI Symbol','Noto Color Emoji'",
                    subclasses: {
                        " #header": {
                            height: "72px",
                            zIndex: "997",
                            transition: "all 0.5s",
                            padding: "15px 0",
                            background: "#fff",
                            boxShadow: "0px 2px 15px rgba(0, 0, 0, 0.1)",
                            subclasses: {
                                " .logo h1": {
                                    fontSize: "30px",
                                    margin: "0",
                                    padding: "6px 0",
                                    lineHeight: "1",
                                    fontWeight: "400",
                                    letterSpacing: "2px"
                                },
                                " .logo h1 a, #header .logo h1 a:hover": {
                                    color: "#242424",
                                    textDecoration: "none"
                                },
                                " .logo img": {
                                    padding: "0",
                                    margin: "0",
                                    maxHeight: "40px"
                                }
                            }
                        },
                        " .nav-menu": {
                            margin: "0",
                            padding: "0",
                            listStyle: "none",
                            subclasses: {
                                " *": {
                                    margin: "0",
                                    padding: "0",
                                    listStyle: "none"
                                },
                                " ul > li": {
                                    position: "relative",
                                    whiteSpace: "nowrap",
                                    float: "left"
                                },
                                " a": {
                                    display: "block",
                                    position: "relative",
                                    color: "#242424",
                                    padding: "10px 15px",
                                    transition: "0.3s",
                                    fontSize: "15px",
                                    fontFamily: "'Open Sans', sans-serif"
                                },
                                " a:hover,  .active > a,  li:hover > a": {
                                    color: "#eb5d1e",
                                    textDecoration: "none"
                                },
                                " .get-started a": {
                                    background: "#eb5d1e",
                                    color: "#fff",
                                    borderRadius: "50px",
                                    margin: "0 15px",
                                    padding: "10px 25px"
                                },
                                " .get-started a:hover": {
                                    background: "#ee7843",
                                    color: "#fff"
                                },
                                " .drop-down ul": {
                                    display: "block",
                                    position: "absolute",
                                    left: "0",
                                    top: "calc(100% - 30px)",
                                    zIndex: "99",
                                    opacity: "0",
                                    visibility: "hidden",
                                    padding: "10px 0",
                                    background: "#fff",
                                    boxShadow: "0px 0px 30px rgba(127, 137, 161, 0.25)",
                                    transition: "ease all 0.3s"
                                },
                                " .drop-down:hover > ul": {
                                    opacity: "1",
                                    top: "100%",
                                    visibility: "visible"
                                },
                                " .drop-down li": {
                                    minWidth: "180px",
                                    position: "relative"
                                },
                                " .drop-down ul a": {
                                    padding: "10px 20px",
                                    fontSize: "14px",
                                    fontWeight: "500",
                                    textTransform: "none",
                                    color: "#3c1300"
                                },
                                " .drop-down ul a:hover,  .drop-down ul .active > a,  .drop-down ul li:hover > a": {
                                    color: "#eb5d1e"
                                },
                                " .drop-down > a:after": {
                                    content: "'\ea99'",
                                    fontFamily: "IcoFont",
                                    paddingLeft: "5px"
                                },
                                " .drop-down .drop-down ul": {
                                    top: "0",
                                    left: "calc(100% - 30px)"
                                },
                                " .drop-down .drop-down:hover > ul": {
                                    opacity: "1",
                                    top: "0",
                                    left: "100%"
                                },
                                " .drop-down .drop-down > a": {
                                    paddingRight: "35px"
                                },
                                " .drop-down .drop-down > a:after": {
                                    content: "'\eaa0'",
                                    fontFamily: "IcoFont",
                                    position: "absolute",
                                    right: "15px"
                                }
                            }
                        },
                        " section": {
                            padding: "60px 0"
                        },
                        " .section-bg": {
                            backgroundColor: "#f9e8df",
                        },
                        " .section-title": {
                            textAlign: "center",
                            paddingBottom: "30px"
                        },
                        " .section-title h2": {
                            fontSize: "24px",
                            fontWeight: "700",
                            paddingBottom: "0",
                            lineHeight: "1px",
                            marginBottom: "15px",
                            color: "#c2b7b1",
                        },
                        " .section-title p": {
                            paddingBottom: "15px",
                            marginBottom: "15px",
                            position: "relative",
                            fontSize: "32px",
                            fontWeight: "700",
                            color: "#242424",
                        },
                        " .section-title p::after": {
                            content: "",
                            position: "absolute",
                            display: "block",
                            width: "60px",
                            height: "2px",
                            background: "#eb5d1e",
                            bottom: "0",
                            left: "calc(50% - 30px)",
                        },
                        " #hero": {
                            width: "100%",
                            height: this.screen.screenType === "mobile" ? "auto" : "70vh",
                            background: "#fef8f5",
                            borderBottom: "2px solid #fcebe3",
                            margin: "72px 0 -72px 0",
                            backgroundAttachment: this.screen.screenType === "mobile" ? "fixed" : "",
                            subclasses: {
                                " h1": {
                                    margin: "0 0 10px 0",
                                    fontSize: this.screen.screenType === "mobile" ? "28px" : "45px",
                                    fontWeight: "700",
                                    lineHeight: this.screen.screenType === "mobile" ? "36px" : "56px",
                                    color: "#242424"
                                },
                                " h2": {
                                    color: "#242424",
                                    marginBottom: this.screen.screenType === "mobile" ? "30px" : "50px",
                                    lineHeight: this.screen.screenType === "mobile" ? "24px" : "1.2",
                                    fontSize: this.screen.screenType === "mobile" ? "18px" : "24px",
                                },
                                " .btn-get-started": {
                                    fontFamily: "'Raleway', sans-serif",
                                    fontWeight: "500",
                                    fontSize: "16px",
                                    letterSpacing: "1px",
                                    display: "inline-block",
                                    padding: "8px 28px",
                                    borderRadius: "3px",
                                    transition: "0.5s",
                                    margin: "10px",
                                    color: "#fff",
                                    background: "#eb5d1e"
                                },
                                " .btn-get-started:hover": {
                                    background: "#ef7f4d"
                                },
                                " .animated": {
                                    animation: "up-down 2s ease-in-out infinite alternate-reverse both"
                                },
                                " .hero-img": {
                                    textAlign: this.screen.screenType === "mobile" ? "center" : ""
                                },
                                " .hero-img img": {
                                    width: this.screen.screenType === "mobile" ? "70%" : ""
                                }
                            }
                        },
                        " #main": {
                            marginTop: "72px"
                        },
                        " .about": {
                            subclasses: {
                                " h3": {
                                    fontWeight: "700",
                                    fontSize: "34px",
                                    color: "#242424",
                                },
                                " h4": {
                                    fontSize: "20px",
                                    fontWeight: "700",
                                    marginTop: "5px",
                                    color: "#7a6960",
                                },
                                " i": {
                                    fontSize: "48px",
                                    marginTop: "15px",
                                    color: "#f39e7a",
                                },
                                " p": {
                                    fontSize: "15px",
                                    color: "#5a6570"
                                },
                                " .about-img img": {
                                    maxWidth: this.screen.screenType === "mobile" ? "70%" : ""
                                }
                            }
                        },
                        " .services": {
                            subclasses: {
                                " .icon-box": {
                                    padding: "30px",
                                    position: "relative",
                                    overflow: "hidden",
                                    margin: "0  0 40px 0",
                                    background: "#fff",
                                    boxShadow: "0 10px 29px 0 rgba(68, 88, 144, 0.1)",
                                    transition: "all 0.3s ease-in-out",
                                    borderRadius: "15px",
                                    textAlign: "center",
                                    borderBottom: "3px solid #fff",
                                },
                                " .icon-box:hover": {
                                    transform: "translateY(-5px)",
                                    borderColor: "#ef7f4d",
                                },
                                " .icon i": {
                                    fontSize: "48px",
                                    lineHeight: "1",
                                    marginBottom: "15px",
                                    color: "#ef7f4d",
                                },
                                " .title": {
                                    fontWeight: "700",
                                    marginBottom: "15px",
                                    fontSize: "18px",
                                },
                                " .title a": {
                                    color: "#111",
                                },
                                " .description": {
                                    fontSize: "15px",
                                    lineHeight: "28px",
                                    marginBottom: "0"
                                }
                            }
                        },
                        " .team": {
                            background: "#f9e8df",
                            padding: "60px 0",
                            subclasses: {
                                " table": {
                                    overflow: "hidden",
                                    width: "100%",
                                    margin: "0 auto",
                                    backgroundColor: "#fff",
                                    border: "1px solid #000000",
                                    marginBottom: "3em",
                                    borderRadius: "5px",
                                    padding: "0",
                                    boxShadow: "0 0 24px 0 rgba(0, 0, 0, 0.12)"
                                },
                                " table > thead > tr": {
                                    backgroundColor: "#270c00"
                                },
                                " table.tr.td.a": {
                                    maxWidth: "180px"
                                },
                                " table > tbody > tr:nth-child(even)": {
                                    backgroundColor: "#f5f5f5"
                                },
                                " td, th": {
                                    padding: this.screen.screenType === "mobile" ? "12px 5px" : "0.75em",
                                    fontSize: this.screen.screenType === "mobile" ? "13px" : "inherit"
                                },
                                " td.err": {
                                    backgroundColor: "#e992b9 ",
                                    color: "#fff",
                                    fontSize: "0.75em",
                                    textAlign: "center",
                                    lineHeight: "0",
                                },
                                "tbody tr:nth-child(2n-1)": {
                                    backgroundColor: "#f5f5f5",
                                },
                                " tbody tr:hover": {
                                    backgroundColor: "rgba(255,209,202,.3)",
                                },
                                " th": {
                                    fontWeight: "500",
                                    color: "#fff",
                                    whiteSpace: "no",
                                },
                                " .btn-primary": {
                                    color: "#fff",
                                    backgroundColor: "#eb5d1e",
                                    borderColor: "#eb5d1e",
                                    width: this.screen.screenType === "mobile" ? "100px" : "140px",
                                    fontSize: this.screen.screenType === "mobile" ? "13px" : "",
                                    margin: this.screen.screenType === "mobile" ? "5px" : "",
                                }
                            }
                        },
                        " .contact": {
                            subclasses: {
                                " .info": {
                                    borderTop: "3px solid #eb5d1e",
                                    borderBottom: "3px solid #eb5d1e",
                                    padding: "30px",
                                    background: "#fff",
                                    width: "100%",
                                    boxShadow: "0 0 24px 0 rgba(0, 0, 0, 0.12)",
                                },
                                " .info i": {
                                    fontSize: "20px",
                                    color: "#eb5d1e",
                                    float: "left",
                                    width: "44px",
                                    height: "44px",
                                    background: "#fdf1ec",
                                    display: "flex",
                                    justifyContent: "center",
                                    alignItems: "center",
                                    borderRadius: "50px",
                                    transition: "all 0.3s ease-in-out"
                                },
                                " .info h4": {
                                    padding: "0 0 0 60px",
                                    fontSize: "22px",
                                    fontWeight: "600",
                                    marginBottom: "5px",
                                    color: "#7a6960",
                                },
                                " .info p": {
                                    padding: "0 0 10px 60px",
                                    marginBottom: "20px",
                                    fontSize: "14px",
                                    color: "#ab9d95"
                                },
                                " .info .email p": {
                                    paddingTop: "5px",
                                },
                                " .info .social-links": {
                                    paddingLeft: "60px",
                                },
                                " .info .social-links a": {
                                    fontSize: "18px",
                                    display: "inline-block",
                                    background: "#333",
                                    color: "#fff",
                                    lineHeight: "1",
                                    padding: "8px 0",
                                    borderRadius: "50%",
                                    textAlign: "center",
                                    width: "36px",
                                    height: "36px",
                                    transition: "0.3s",
                                    marginRight: "10px",
                                },
                                " .info .social-links a:hover": {
                                    background: "#eb5d1e",
                                    color: "#fff",
                                },
                                " .info .email:hover i,.info .address:hover i,.info .phone:hover i": {
                                    background: "#eb5d1e",
                                    color: "#fff",
                                },
                                " .php-email-form": {
                                    width: "100%",
                                    borderTop: "3px solid #eb5d1e",
                                    borderBottom: "3px solid #eb5d1e",
                                    padding: "30px",
                                    background: "#fff",
                                    boxShadow: "0 0 24px 0 rgba(0, 0, 0, 0.12)",
                                },
                                " .php-email-form .form-group": {
                                    paddingBottom: "8px",
                                },
                                " .php-email-form .validate": {
                                    color: "red",
                                    margin: "0",
                                    fontWeight: "400",
                                    fontSize: "13px",
                                },
                                " .php-email-form .error-message": {
                                    display: "none",
                                    color: "#fff",
                                    background: "#ed3c0d",
                                    textAlign: "center",
                                    padding: "15px",
                                    fontWeight: "600",
                                },
                                " .php-email-form .sent-message": {
                                    display: "none",
                                    color: "#fff",
                                    background: "#18d26e",
                                    textAlign: "center",
                                    padding: "15px",
                                    fontWeight: "600",
                                },
                                " .php-email-form .loading": {
                                    display: "none",
                                    background: "#fff",
                                    textAlign: "center",
                                    padding: "15px",
                                },
                                " .php-email-form .loading:before": {
                                    content: "",
                                    display: "inline-block",
                                    borderRadius: "50%",
                                    width: "24px",
                                    height: "24px",
                                    margin: "0 10px -6px 0",
                                    border: "3px solid #18d26e",
                                    borderTopColor: "#eee",
                                    animation: "animate-loading 1s linear infinite",
                                },
                                " .php-email-form input,.php-email-form textarea": {
                                    borderRadius: "0",
                                    boxShadow: "none",
                                    fontSize: "14px",
                                },
                                " .php-email-form input": {
                                    height: "44px",
                                },
                                " .php-email-form textarea": {
                                    padding: "10px 12px",
                                },
                                " .php-email-form button ": {
                                    background: "#eb5d1e",
                                    border: "0",
                                    padding: "10px 24px",
                                    color: "#fff",
                                    transition: "0.4s",
                                    borderRadius: "4px",
                                },
                                " .php-email-form button:hover": {
                                    background: "#ef7f4d"
                                }
                            }
                        },
                        " #footer": {
                            padding: "0 0 30px 0",
                            color: "#212529",
                            fontSize: "14px",
                            background: "#f9e8df",
                            subclasses: {
                                " .footer-newsletter": {
                                    padding: "50px 0",
                                    background: "#f9e8df",
                                    textAlign: "center",
                                    fontSize: "15px",
                                },
                                " .footer-newsletter h4": {
                                    fontSize: "24px",
                                    margin: "0 0 20px 0",
                                    padding: "0",
                                    lineHeight: "1",
                                    fontWeight: "600",
                                    color: "#242424",
                                },
                                " .footer-newsletter form": {
                                    marginTop: "30px",
                                    background: "#fff",
                                    padding: "6px 10px",
                                    position: "relative",
                                    borderRadius: "4px",
                                    boxShadow: "0px 2px 15px rgba(0, 0, 0, 0.1)",
                                    textAlign: "left",
                                },
                                " .footer-newsletter form input[type='email']": {
                                    border: "0",
                                    padding: "4px 4px",
                                    width: "calc(100% - 100px)",
                                },
                                " .footer-newsletter form button": {
                                    position: "absolute",
                                    top: "0",
                                    right: "0",
                                    bottom: "0",
                                    border: "0",
                                    fontSize: "16px",
                                    padding: "0 20px",
                                    background: "#eb5d1e",
                                    color: "#fff",
                                    transition: "0.3s",
                                    borderRadius: "4px",
                                    boxShadow: "0px 2px 15px rgba(0, 0, 0, 0.1)",
                                },
                                " .footer-newsletter form button:hover": {
                                    background: "#c54811",
                                },
                                " .footer-top": {
                                    padding: "60px 0 30px 0",
                                    background: "#fff",
                                },
                                " .footer-top .footer-contact": {
                                    marginBottom: "30px",
                                },
                                " .footer-top .footer-contact h4": {
                                    fontSize: "22px",
                                    margin: "0 0 30px 0",
                                    padding: "2px 0 2px 0",
                                    lineHeight: "1",
                                    fontWeight: "700",
                                },
                                " .footer-top .footer-contact p": {
                                    fontSize: "14px",
                                    lineHeight: "24px",
                                    marginBottom: "0",
                                    fontFamily: "'Raleway', sans-serif",
                                    color: "#5c5c5c",
                                },
                                " .footer-top h4": {
                                    fontSize: "16px",
                                    fontWeight: "bold",
                                    color: "#212529",
                                    position: "relative",
                                    paddingBottom: "12px",
                                },
                                " .footer-top .footer-links": {
                                    marginBottom: "30px",
                                },
                                " .footer-top .footer-links ul": {
                                    listStyle: "none",
                                    padding: "0",
                                    margin: "0",
                                },
                                " .footer-top .footer-links ul i": {
                                    paddingRight: "2px",
                                    color: "#f39e7a",
                                    fontSize: "11px",
                                    lineHeight: "1",
                                    marginRight: "5px"
                                },
                                " .footer-top .footer-links ul li": {
                                    padding: "10px 0",
                                    display: "flex",
                                    alignItems: "center",
                                },
                                " .footer-top .footer-links ul li:first-child": {
                                    paddingTop: "0",
                                },
                                " .footer-top .footer-links ul a": {
                                    color: "#5c5c5c",
                                    transition: "0.3s",
                                    display: "inline-block",
                                    lineHeight: "1",
                                },
                                " .footer-top .footer-links ul a:hover": {
                                    textDecoration: "none",
                                    color: "#eb5d1e",
                                },
                                " .footer-top .social-links a": {
                                    fontSize: "18px",
                                    display: "inline-block",
                                    background: "#eb5d1e",
                                    color: "#fff",
                                    lineHeight: "1",
                                    padding: "8px 0",
                                    marginRight: "4px",
                                    borderRadius: "50%",
                                    textAlign: "center",
                                    width: "36px",
                                    height: "36px",
                                    transition: "0.3s",
                                },
                                " .footer-top .social-links a:hover": {
                                    background: "#ef7f4d",
                                    color: "#fff",
                                    textDecoration: "none",
                                },
                                " .copyright": {
                                    textAlign: "center",
                                    float: "left",
                                },
                                " .credits": {
                                    float: "right",
                                    textAlign: "center",
                                    fontSize: "13px",
                                    color: "#212529",
                                },
                                " .credits a": {
                                    color: "#eb5d1e",
                                }
                            }
                        },
                        " .back-to-top": {
                            position: "fixed",
                            display: "none",
                            width: "40px",
                            height: "40px",
                            borderRadius: "3px",
                            right: "15px",
                            bottom: "15px",
                            background: "#eb5d1e",
                            color: "#fff",
                            transition: "display 0.5s ease-in-out",
                            zIndex: "99999",
                            subclasses: {
                                " i": {
                                    fontSize: "30px",
                                    position: "absolute",
                                    top: "5px",
                                    left: "11px",
                                },
                                ":hover": {
                                    color: "#fff",
                                    background: "#ee7843",
                                    transition: "background 0.2s ease-in-out",
                                }
                            }
                        },
                        " .mobile-nav": {
                            position: "fixed",
                            top: "0",
                            bottom: "0",
                            zIndex: "9999",
                            overflowY: "auto",
                            left: "0",
                            width: "260px",
                            paddingTop: "18px",
                            background: "#242424",
                            transition: "0.4s",
                            subclasses: {
                                " *": {
                                    margin: "0",
                                    padding: "0",
                                    listStyle: "none",
                                },
                                " a": {
                                    display: "block",
                                    position: "relative",
                                    color: "#f9d1c0",
                                    padding: "10px 20px",
                                    fontWeight: "500",
                                    transition: "0.3s",
                                },
                                " a:hover, .mobile-nav .active > a, .mobile-nav li:hover > a": {
                                    color: "#f39e7a",
                                    textDecoration: "none",
                                },
                                " .get-started a": {
                                    background: "#eb5d1e",
                                    color: "#fff",
                                    borderRadius: "50px",
                                    margin: "15px",
                                    padding: "10px 25px",
                                },
                                " .get-started a:hover": {
                                    background: "#ee7843",
                                    color: "#fff",
                                },
                                " .drop-down > a:after": {
                                    content: "'\ea99'",
                                    fontFamily: "IcoFont",
                                    paddingLeft: "10px",
                                    position: "absolute",
                                    right: "15px",
                                },
                                " .active.drop-down > a:after": {
                                    content: "'\eaa0'",
                                },
                                " .drop-down > a": {
                                    paddingRight: "35px",
                                },
                                " .drop-down ul": {
                                    display: "none",
                                    overflow: "hidden",
                                },
                                " .drop-down li": {
                                    paddingLeft: "20px",
                                }
                            }
                        },
                        " .mobile-nav-toggle": {
                            position: "fixed",
                            right: "15px",
                            top: "22px",
                            zIndex: "9998",
                            border: "0",
                            background: "none",
                            fontSize: "24px",
                            transition: "all 0.4s",
                            outline: "none !important",
                            lineHeight: "1",
                            cursor: "pointer",
                            textAlign: "right",
                        },
                        " .mobile-nav-toggle i": {
                            color: "#7a6960"
                        },
                        " .mobile-nav-overly": {
                            width: "100%",
                            height: "100%",
                            zIndex: "9997",
                            top: "0",
                            left: "0",
                            position: "fixed",
                            background: "rgba(78, 64, 57, 0.9)",
                            overflow: "hidden",
                            display: "none",
                        },
                        " .mobile-nav-active": {
                            overflow: "hidden",
                        },
                        " .mobile-nav-active  .mobile-nav": {
                            left: "0",
                        },
                        " .mobile-nav-active  .mobile-nav-toggle i": {
                            color: "#fff",
                        },
                        " a": {
                            color: "#007bff",
                        },
                        " a:hover": {
                            color: "#0b6bd3",
                            textDecoration: "none"
                        },
                        " h1, h2, h3, h4, h5, h6, .font-primary": {
                            fontFamily: "'Raleway', sans-serif"
                        }
                    }
                };
            },
            enumerable: true,
            configurable: true
        });
        return IndexStyle;
    }(AtomStyle_1.AtomStyle));
    exports.default = IndexStyle;
});
//# sourceMappingURL=IndexStyle.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/web/styles/IndexStyle");

var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
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
    exports.cssNumberToString = void 0;
    function cssNumberToString(n, unit) {
        if (unit === void 0) { unit = "px"; }
        if (typeof n === "number") {
            if (n === 0) {
                return n + "";
            }
            return n + unit;
        }
        return n;
    }
    exports.cssNumberToString = cssNumberToString;
    var StyleBuilder = /** @class */ (function () {
        function StyleBuilder(style) {
            this.style = style;
            this.style = this.style || {};
        }
        Object.defineProperty(StyleBuilder, "newStyle", {
            get: function () {
                return new StyleBuilder();
            },
            enumerable: false,
            configurable: true
        });
        StyleBuilder.prototype.toStyle = function () {
            return this.style;
        };
        StyleBuilder.prototype.size = function (width, height) {
            width = cssNumberToString(width);
            height = cssNumberToString(height);
            return this.merge({
                width: width,
                height: height
            });
        };
        StyleBuilder.prototype.roundBorder = function (radius) {
            radius = cssNumberToString(radius);
            return this.merge({
                borderRadius: radius,
                padding: radius
            });
        };
        StyleBuilder.prototype.border = function (borderWidth, borderColor, borderStyle) {
            if (borderStyle === void 0) { borderStyle = "solid"; }
            borderWidth = cssNumberToString(borderWidth);
            return this.merge({
                borderWidth: borderWidth,
                borderStyle: borderStyle,
                borderColor: borderColor
            });
        };
        StyleBuilder.prototype.center = function (width, height) {
            width = cssNumberToString(width);
            height = cssNumberToString(height);
            return this.merge({
                position: "absolute",
                left: 0,
                right: 0,
                top: 0,
                bottom: 0,
                width: width,
                height: height,
                margin: "auto"
            });
        };
        StyleBuilder.prototype.absolute = function (left, top, right, bottom) {
            left = cssNumberToString(left);
            top = cssNumberToString(top);
            if (right !== undefined) {
                right = cssNumberToString(right);
                bottom = cssNumberToString(bottom);
                return this.merge({
                    position: "absolute",
                    left: left,
                    top: top,
                    right: right,
                    bottom: bottom
                });
            }
            return this.merge({
                position: "absolute",
                left: left,
                top: top
            });
        };
        StyleBuilder.prototype.merge = function (style) {
            return new StyleBuilder(__assign(__assign({}, this.style), style));
        };
        return StyleBuilder;
    }());
    exports.default = StyleBuilder;
});
//# sourceMappingURL=StyleBuilder.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/StyleBuilder");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
        define(["require", "exports", "../../App", "../../di/Inject", "../../di/RegisterSingleton", "../../services/BusyIndicatorService", "../../services/NavigationService", "../controls/AtomControl", "../styles/StyleBuilder"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.WebBusyIndicatorService = void 0;
    var App_1 = require("../../App");
    var Inject_1 = require("../../di/Inject");
    var RegisterSingleton_1 = require("../../di/RegisterSingleton");
    var BusyIndicatorService_1 = require("../../services/BusyIndicatorService");
    var NavigationService_1 = require("../../services/NavigationService");
    var AtomControl_1 = require("../controls/AtomControl");
    var StyleBuilder_1 = require("../styles/StyleBuilder");
    var WebBusyIndicatorService = /** @class */ (function (_super) {
        __extends(WebBusyIndicatorService, _super);
        function WebBusyIndicatorService() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.zIndex = 50000;
            return _this;
        }
        WebBusyIndicatorService.prototype.createIndicator = function () {
            var host = document.createElement("div");
            var popup = new AtomControl_1.AtomControl(this.app, host);
            host.className = "indicator-host";
            var span = document.createElement("i");
            var divStyle = host.style;
            divStyle.position = "absolute";
            divStyle.left = divStyle.right = divStyle.bottom = divStyle.top = "0";
            divStyle.zIndex = (this.zIndex++) + "";
            var spanStyle = span.style;
            spanStyle.position = "absolute";
            spanStyle.margin = "auto";
            spanStyle.width = "16px";
            spanStyle.height = "16px";
            spanStyle.maxHeight = "100%";
            spanStyle.maxWidth = "100%";
            spanStyle.left = spanStyle.right = spanStyle.bottom = spanStyle.top = "0";
            // span.src = ModuleFiles.src.web.images.busy_gif;
            span.className = "fas fa-spinner fa-spin";
            host.appendChild(span);
            var ws = this.navigationService;
            var e = ws.getHostForElement();
            if (e) {
                e.appendChild(host);
            }
            else {
                document.body.appendChild(host);
                ws.refreshScreen();
                popup.bind(host, "styleLeft", [["this", "scrollLeft"]], false, StyleBuilder_1.cssNumberToString, ws.screen);
                popup.bind(host, "styleTop", [["this", "scrollTop"]], false, StyleBuilder_1.cssNumberToString, ws.screen);
                popup.bind(host, "styleWidth", [["this", "width"]], false, StyleBuilder_1.cssNumberToString, ws.screen);
                popup.bind(host, "styleHeight", [["this", "height"]], false, StyleBuilder_1.cssNumberToString, ws.screen);
            }
            popup.registerDisposable({
                dispose: function () {
                    host.remove();
                }
            });
            return popup;
        };
        __decorate([
            Inject_1.Inject,
            __metadata("design:type", NavigationService_1.NavigationService)
        ], WebBusyIndicatorService.prototype, "navigationService", void 0);
        __decorate([
            Inject_1.Inject,
            __metadata("design:type", App_1.App)
        ], WebBusyIndicatorService.prototype, "app", void 0);
        WebBusyIndicatorService = __decorate([
            RegisterSingleton_1.RegisterSingleton
        ], WebBusyIndicatorService);
        return WebBusyIndicatorService;
    }(BusyIndicatorService_1.BusyIndicatorService));
    exports.WebBusyIndicatorService = WebBusyIndicatorService;
});
//# sourceMappingURL=WebBusyIndicatorService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/services/WebBusyIndicatorService");

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var JsonService_1 = require("../services/JsonService");
    var ReferenceService_1 = require("../services/ReferenceService");
    var types_1 = require("./types");
    var AtomLoader = /** @class */ (function () {
        function AtomLoader() {
        }
        AtomLoader.load = function (url, app) {
            return __awaiter(this, void 0, void 0, function () {
                var r, r, type, obj;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (url.host === "reference") {
                                r = app.get(ReferenceService_1.default).get(url.path);
                                if (!r) {
                                    throw new Error("reference not found");
                                }
                                return [2 /*return*/, r.consume()];
                            }
                            if (url.host === "class") {
                                r = app.get(ReferenceService_1.default).get(url.path);
                                if (!r) {
                                    throw new Error("reference not found");
                                }
                                return [2 /*return*/, app.resolve(r.consume(), true)];
                            }
                            return [4 /*yield*/, types_1.DI.resolveViewClassAsync(url.path)];
                        case 1:
                            type = _a.sent();
                            if (!type) {
                                throw new Error("Type not found for " + url);
                            }
                            obj = app.resolve(type, true);
                            return [2 /*return*/, obj];
                    }
                });
            });
        };
        AtomLoader.loadView = function (url, app, hookCloseEvents, vmFactory) {
            return __awaiter(this, void 0, void 0, function () {
                var busyIndicator, view_1, vm, jsonService, key, element, k, rs, v, disposables_1, id_1, returnPromise;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            busyIndicator = app.createBusyIndicator();
                            _a.label = 1;
                        case 1:
                            _a.trys.push([1, , 3, 4]);
                            return [4 /*yield*/, AtomLoader.load(url, app)];
                        case 2:
                            view_1 = _a.sent();
                            vm = view_1.viewModel;
                            if (!vm) {
                                if (!vmFactory) {
                                    return [2 /*return*/, { view: view_1 }];
                                }
                                vm = vmFactory();
                                view_1.viewModel = vm;
                            }
                            if (vm) {
                                jsonService = app.get(JsonService_1.JsonService);
                                for (key in url.query) {
                                    if (url.query.hasOwnProperty(key)) {
                                        element = url.query[key];
                                        if (typeof element === "object") {
                                            vm[key] = jsonService.parse(jsonService.stringify(element));
                                            continue;
                                        }
                                        if (/^json\:/.test(key)) {
                                            k = key.split(":")[1];
                                            vm[k] = jsonService.parse(element.toString());
                                            continue;
                                        }
                                        if (/^ref\:/.test(key)) {
                                            rs = app.get(ReferenceService_1.default);
                                            v = rs.get(element);
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
                                disposables_1 = view_1.disposables;
                                id_1 = (AtomLoader.id++).toString();
                                view_1.id = id_1;
                                returnPromise = new Promise(function (resolve, reject) {
                                    disposables_1.add(app.subscribe("atom-window-close:" + id_1, function (m, r) {
                                        resolve(r);
                                        view_1.dispose();
                                    }));
                                    disposables_1.add(app.subscribe("atom-window-cancel:" + id_1, function () {
                                        reject("cancelled");
                                        view_1.dispose();
                                    }));
                                });
                                // it is responsibility of view holder to dispose the view
                                // disposables.add((view as any));
                                vm.windowName = id_1;
                                view_1.returnPromise = returnPromise;
                                return [2 /*return*/, { view: view_1, disposables: disposables_1, returnPromise: returnPromise, id: id_1 }];
                            }
                            return [2 /*return*/, { view: view_1 }];
                        case 3:
                            busyIndicator.dispose();
                            return [7 /*endfinally*/];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        };
        AtomLoader.id = 1;
        return AtomLoader;
    }());
    exports.AtomLoader = AtomLoader;
});
//# sourceMappingURL=AtomLoader.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/AtomLoader");

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
    var FormattedError = /** @class */ (function () {
        function FormattedError(msg) {
            var e = new Error(msg.toString());
            e.formattedMessage = msg;
            e.__proto__ = FormattedError.prototype;
            return e;
        }
        return FormattedError;
    }());
    exports.default = FormattedError;
});
//# sourceMappingURL=FormattedError.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/core/FormattedError");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
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
    var NavigationService_1 = require("../services/NavigationService");
    var AtomViewModel_1 = require("./AtomViewModel");
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
    var AtomWindowViewModel = /** @class */ (function (_super) {
        __extends(AtomWindowViewModel, _super);
        function AtomWindowViewModel() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
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
        AtomWindowViewModel.prototype.close = function (result) {
            this.app.broadcast("atom-window-close:" + this.windowName, result);
        };
        /**
         * This will return true if this view model is safe to cancel and close
         */
        AtomWindowViewModel.prototype.cancel = function () {
            return __awaiter(this, void 0, void 0, function () {
                var navigationService;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!this.closeWarning) return [3 /*break*/, 2];
                            navigationService = this.app.resolve(NavigationService_1.NavigationService);
                            return [4 /*yield*/, navigationService.confirm(this.closeWarning, "Are you sure?")];
                        case 1:
                            if (!(_a.sent())) {
                                return [2 /*return*/, false];
                            }
                            _a.label = 2;
                        case 2:
                            this.app.broadcast("atom-window-cancel:" + this.windowName, "cancelled");
                            return [2 /*return*/, true];
                    }
                });
            });
        };
        return AtomWindowViewModel;
    }(AtomViewModel_1.AtomViewModel));
    exports.AtomWindowViewModel = AtomWindowViewModel;
});
//# sourceMappingURL=AtomWindowViewModel.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/view-model/AtomWindowViewModel");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/src/web/images/close-button-hover.svg", "../styles/AtomStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomWindowStyle = void 0;
    var close_button_hover_svg_1 = require("@web-atoms/core/src/web/images/close-button-hover.svg");
    var AtomStyle_1 = require("../styles/AtomStyle");
    /**
     * Represents Window Style, in order to add more subclasses
     * you can override content style
     */
    var AtomWindowStyle = /** @class */ (function (_super) {
        __extends(AtomWindowStyle, _super);
        function AtomWindowStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomWindowStyle.prototype, "root", {
            get: function () {
                return __assign(__assign({}, this.frameHost), { subclasses: {
                        " .close-button": this.closeButton,
                        " .command-bar-presenter": this.commandBarPresenter,
                        " .command-bar": this.commandBar,
                        " .content-presenter": this.contentPresenter,
                        " .content": this.content,
                        " .frame": this.frame,
                        " .title": this.title,
                        " .title-host": this.titleHost,
                        " .title-presenter": this.titlePresenter,
                    } });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "frameHost", {
            get: function () {
                return {
                    position: "absolute",
                    left: 0,
                    right: 0,
                    top: 0,
                    bottom: 0,
                    backgroundColor: "#50505080"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "frame", {
            get: function () {
                return {
                    position: "absolute",
                    minHeight: "100px",
                    minWidth: "300px",
                    margin: "auto",
                    border: "solid 1px #808080",
                    borderRadius: "5px",
                    backgroundColor: "white"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "titlePresenter", {
            get: function () {
                return {
                    position: "relative",
                    left: 0,
                    right: 0,
                    top: 0,
                    height: "37px"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "titleHost", {
            get: function () {
                return {
                    position: "absolute",
                    left: 0,
                    right: 0,
                    padding: "7px",
                    minHeight: "32px",
                    backgroundColor: "#404040",
                    color: "white",
                    top: 0,
                    borderTopRightRadius: "4px",
                    borderTopLeftRadius: "4px"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "title", {
            get: function () {
                return {
                    margin: "auto"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "closeButton", {
            get: function () {
                return {
                    position: "absolute",
                    right: "6px",
                    top: "7px",
                    width: "0",
                    height: "0",
                    padding: "9px",
                    border: "none",
                    backgroundColor: "transparent",
                    backgroundImage: close_button_hover_svg_1.default
                    // As suggested by srikanth sir
                    //  subclasses: {
                    //      ":hover": {
                    //          backgroundImage: closeButtonHover
                    //      }
                    // }
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "content", {
            get: function () {
                return {};
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "contentPresenter", {
            get: function () {
                return {
                    position: "relative",
                    padding: "10px",
                    background: "white"
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "commandBarPresenter", {
            get: function () {
                return {
                    left: 0,
                    right: 0,
                    bottom: 0,
                    padding: "5px",
                    backgroundColor: "#d4d4d4",
                    textAlign: "right",
                    borderBottomRightRadius: "4px",
                    borderBottomLeftRadius: "4px",
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomWindowStyle.prototype, "commandBar", {
            get: function () {
                return {
                    subclasses: {
                        " button": {
                            borderRadius: "3px",
                            marginLeft: "5px",
                            marginRight: "5px",
                            padding: "4px 16px",
                            backgroundColor: "whitesmoke",
                            border: "1px solid gray"
                        }
                    },
                };
            },
            enumerable: false,
            configurable: true
        });
        return AtomWindowStyle;
    }(AtomStyle_1.AtomStyle));
    exports.AtomWindowStyle = AtomWindowStyle;
});
//# sourceMappingURL=AtomWindowStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomWindowStyle");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/Colors", "./AtomWindowStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Colors_1 = require("../../core/Colors");
    var AtomWindowStyle_1 = require("./AtomWindowStyle");
    var AtomAlertWindowStyle = /** @class */ (function (_super) {
        __extends(AtomAlertWindowStyle, _super);
        function AtomAlertWindowStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomAlertWindowStyle.prototype, "titleHost", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(AtomAlertWindowStyle, "titleHost")), { color: Colors_1.default.black, backgroundColor: Colors_1.default.white });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomAlertWindowStyle.prototype, "contentPresenter", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(AtomAlertWindowStyle, "contentPresenter")), { padding: "0px 10px 30px 10px", textAlign: "center", color: Colors_1.default.rgba(51, 51, 51) });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomAlertWindowStyle.prototype, "commandBar", {
            get: function () {
                return __assign(__assign({}, this.getBaseProperty(AtomAlertWindowStyle, "commandBar")), { textAlign: "center", subclasses: {
                        " button": this.buttonStyle,
                        " .yes-button": {
                            backgroundColor: Colors_1.default.rgba(0, 128, 0)
                        },
                        " .no-button": {
                            backgroundColor: Colors_1.default.rgba(255, 0, 0)
                        }
                    } });
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(AtomAlertWindowStyle.prototype, "buttonStyle", {
            get: function () {
                return {
                    border: "none",
                    color: Colors_1.default.white,
                    width: "50%",
                    height: "40px"
                };
            },
            enumerable: false,
            configurable: true
        });
        return AtomAlertWindowStyle;
    }(AtomWindowStyle_1.AtomWindowStyle));
    exports.default = AtomAlertWindowStyle;
});
//# sourceMappingURL=AtomAlertWindowStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomAlertWindowStyle");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomTemplate = void 0;
    var AtomControl_1 = require("./AtomControl");
    var AtomTemplate = /** @class */ (function (_super) {
        __extends(AtomTemplate, _super);
        function AtomTemplate() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        return AtomTemplate;
    }(AtomControl_1.AtomControl));
    exports.AtomTemplate = AtomTemplate;
});
//# sourceMappingURL=AtomTemplate.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomTemplate");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../App", "../../core/Bind", "../../core/XNode", "../styles/AtomWindowStyle", "./AtomControl", "./AtomTemplate"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomWindow = exports.AtomWindowFrameTemplate = exports.getTemplateParent = void 0;
    var App_1 = require("../../App");
    var Bind_1 = require("../../core/Bind");
    var XNode_1 = require("../../core/XNode");
    var AtomWindowStyle_1 = require("../styles/AtomWindowStyle");
    var AtomControl_1 = require("./AtomControl");
    var AtomTemplate_1 = require("./AtomTemplate");
    function getTemplateParent(e) {
        var tp = e._templateParent;
        if (tp) {
            return tp;
        }
        var p = e._logicalParent || e.parentElement;
        if (p) {
            return getTemplateParent(p);
        }
    }
    exports.getTemplateParent = getTemplateParent;
    var AtomWindowFrameTemplate = /** @class */ (function (_super) {
        __extends(AtomWindowFrameTemplate, _super);
        function AtomWindowFrameTemplate() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomWindowFrameTemplate.prototype, "templateParent", {
            get: function () {
                return getTemplateParent(this.element);
            },
            enumerable: false,
            configurable: true
        });
        AtomWindowFrameTemplate.prototype.preCreate = function () {
            this.titlePresenter = null;
            this.commandPresenter = null;
            this.contentPresenter = null;
            _super.prototype.preCreate.call(this);
        };
        AtomWindowFrameTemplate.prototype.create = function () {
            var _this = this;
            // remember, if you do not wish to use dynamic themes
            // then use one time binding
            this.render(XNode_1.default.create("div", { class: "frame", styleWidth: Bind_1.default.oneWay(function () { return _this.templateParent.width || undefined; }), styleHeight: Bind_1.default.oneWay(function () { return _this.templateParent.height || undefined; }), styleLeft: Bind_1.default.oneWay(function () { return _this.templateParent.x >= 0 ? _this.templateParent.x + "px" : undefined; }), styleTop: Bind_1.default.oneWay(function () { return _this.templateParent.y >= 0 ? _this.templateParent.y + "px" : undefined; }), styleMarginTop: Bind_1.default.oneWay(function () { return _this.templateParent.x >= 0 ? "0" : undefined; }), styleMarginLeft: Bind_1.default.oneWay(function () { return _this.templateParent.x >= 0 ? "0" : undefined; }), styleMarginRight: Bind_1.default.oneWay(function () { return _this.templateParent.x >= 0 ? "0" : undefined; }), styleMarginBottom: Bind_1.default.oneWay(function () { return _this.templateParent.x >= 0 ? "0" : undefined; }) },
                XNode_1.default.create("div", { class: "title-presenter", presenter: Bind_1.default.presenter("titlePresenter") }),
                XNode_1.default.create("div", { class: "content-presenter", presenter: Bind_1.default.presenter("contentPresenter") }),
                XNode_1.default.create("div", { class: "command-bar-presenter", presenter: Bind_1.default.presenter("commandPresenter") })));
            // this.bind(this.element, "styleClass", [["templateParent", "controlStyle", "frame"]]);
            // this.bind(this.element, "styleWidth", [["templateParent", "width"]], false, (v) => v || undefined);
            // this.bind(this.element, "styleHeight", [["templateParent", "height"]], false, (v) => v || undefined);
            // this.bind(this.element, "styleLeft", [["templateParent", "x"]],
            //     false, (v) => v >= 0 ? v + "px" : undefined);
            // this.bind(this.element, "styleTop", [["templateParent", "y"]],
            //     false, (v) => v >= 0 ? v + "px" : undefined);
            // this.bind(this.element, "styleMarginTop", [["templateParent", "x"]], false, (v) => v >= 0 ? "0" : undefined);
            // this.bind(this.element, "styleMarginLeft", [["templateParent", "x"]],
            //  false, (v) => v >= 0 ? "0" : undefined);
            // this.bind(this.element, "styleMarginRight", [["templateParent", "x"]],
            // false, (v) => v >= 0 ? "0" : undefined);
            // this.bind(this.element, "styleMarginBottom", [["templateParent", "x"]],
            // false, (v) => v >= 0 ? "0" : undefined);
            // // add title host
            // const titlePresenter = document.createElement("div");
            // this.bind(titlePresenter, "styleClass", [["templateParent", "controlStyle", "titlePresenter"]]);
            // // titleHost.classList.add(style.titleHost.className);
            // this.titlePresenter = titlePresenter;
            // this.element.appendChild(titlePresenter);
            // // add content presenter
            // const cp = document.createElement("div");
            // this.bind(cp, "styleClass", [["templateParent", "controlStyle", "content"]]);
            // // cp.classList.add(style.content.className);
            // this.contentPresenter = cp;
            // this.element.appendChild(cp);
            // // create command presenter
            // const cdp = document.createElement("div");
            // // cdp.classList.add(style.commandBar.className);
            // this.bind(cdp, "styleClass", [["templateParent", "controlStyle", "commandBar"]]);
            // this.commandPresenter = cdp;
            // this.element.appendChild(cdp);
        };
        return AtomWindowFrameTemplate;
    }(AtomTemplate_1.AtomTemplate));
    exports.AtomWindowFrameTemplate = AtomWindowFrameTemplate;
    var AtomWindowTitleTemplate = /** @class */ (function (_super) {
        __extends(AtomWindowTitleTemplate, _super);
        function AtomWindowTitleTemplate() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomWindowTitleTemplate.prototype, "templateParent", {
            get: function () {
                return getTemplateParent(this.element);
            },
            enumerable: false,
            configurable: true
        });
        AtomWindowTitleTemplate.prototype.create = function () {
            var _this = this;
            this.render(XNode_1.default.create("div", { class: "title-host" },
                XNode_1.default.create("span", { class: "title", text: Bind_1.default.oneWay(function () { return _this.templateParent.title; }) }),
                XNode_1.default.create("button", { class: "close-button", eventClick: Bind_1.default.event(function () { return _this.templateParent.close(); }) })));
            // this.bind(this.element, "styleClass", [["templateParent", "controlStyle", "titleHost"]]);
            // // add title
            // const title = document.createElement("span");
            // this.bind(title, "styleClass", [["templateParent", "controlStyle", "title"]]);
            // // title.classList.add(style.title.className);
            // this.bind(title, "text", [["templateParent", "title"]], false);
            // // add close button
            // const closeButton = document.createElement("button");
            // this.bind(closeButton, "styleClass", [["templateParent", "controlStyle", "closeButton"]]);
            // // closeButton.textContent = "x";
            // this.bindEvent(closeButton, "click", (e) => {
            //     const w = getTemplateParent(this.element) as AtomWindow;
            //     w.close();
            // });
            // // append title host > title
            // this.append(title);
            // this.append(closeButton);
        };
        return AtomWindowTitleTemplate;
    }(AtomControl_1.AtomControl));
    var AtomWindow = /** @class */ (function (_super) {
        __extends(AtomWindow, _super);
        function AtomWindow() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.title = "";
            _this.width = "";
            _this.height = "";
            _this.x = -1;
            _this.y = -1;
            _this.titleTemplate = AtomWindowTitleTemplate;
            _this.frameTemplate = AtomWindowFrameTemplate;
            _this.isReady = false;
            return _this;
        }
        Object.defineProperty(AtomWindow.prototype, "templateParent", {
            get: function () {
                return getTemplateParent(this.element);
            },
            enumerable: false,
            configurable: true
        });
        AtomWindow.prototype.onPropertyChanged = function (name) {
            switch (name) {
                case "windowTemplate":
                case "commandTemplate":
                case "frameTemplate":
                    this.invalidate();
                    break;
            }
        };
        AtomWindow.prototype.close = function () {
            var vm = this.viewModel;
            if (vm.cancel) {
                this.app.runAsync(function () { return vm.cancel(); });
                return;
            }
            var message = "atom-window-cancel:" + this.id;
            var device = this.app.resolve(App_1.App);
            device.broadcast(message, "cancelled");
        };
        AtomWindow.prototype.onUpdateUI = function () {
            var _this = this;
            if (!(this.windowTemplate && this.frameTemplate)) {
                return;
            }
            if (this.isReady) {
                return;
            }
            this.bind(this.element, "title", [["viewModel", "title"]]);
            // let us create frame first...
            var frame = new (this.frameTemplate)(this.app);
            var fe = frame.element;
            // setup drag and drop for the frame...
            var titleContent = new (this.titleTemplate)(this.app);
            (titleContent.element)._templateParent = this;
            frame.titlePresenter.appendChild(titleContent.element);
            this.setupDragging(frame.titlePresenter);
            this.element.classList.add("frame-host");
            fe._logicalParent = this.element;
            fe._templateParent = this;
            if (!frame.contentPresenter) {
                throw new Error("ContentPresenter must be set inside frameTemplate before creating window");
            }
            var content = new (this.windowTemplate)(this.app);
            (content.element)._templateParent = this;
            this.setElementClass(content.element, { content: 1 });
            frame.contentPresenter.appendChild(content.element);
            if (this.commandTemplate) {
                if (!frame.commandPresenter) {
                    throw new Error("CommandPresenter must be set inside frameTemplate" +
                        "before creating window if command template is present");
                }
                var command = new (this.commandTemplate)(this.app);
                (command.element)._templateParent = this;
                this.setElementClass(command.element, { "command-bar": 1 });
                frame.commandPresenter.appendChild(command.element);
            }
            this.append(frame);
            // lets center frame...
            setTimeout(function () {
                _this.centerFrame(frame.element);
            }, 100);
            this.isReady = true;
        };
        AtomWindow.prototype.preCreate = function () {
            var _this = this;
            this.defaultControlStyle = AtomWindowStyle_1.AtomWindowStyle;
            this.title = null;
            this.width = "";
            this.height = "";
            this.x = -1;
            this.y = -1;
            this.windowTemplate = null;
            this.commandTemplate = null;
            this.titleTemplate = AtomWindowTitleTemplate;
            this.frameTemplate = AtomWindowFrameTemplate;
            _super.prototype.preCreate.call(this);
            this.render(XNode_1.default.create("div", { styleClass: Bind_1.default.oneTime(function () { return _this.controlStyle.name; }) }));
        };
        AtomWindow.prototype.centerFrame = function (e) {
            var _this = this;
            /// window is destroyed probably..
            if (!this.element) {
                return;
            }
            var parent = this.element.parentElement;
            if (parent === window || parent === document.body) {
                return;
            }
            if (parent.offsetWidth <= 0 || parent.offsetHeight <= 0) {
                setTimeout(function () {
                    _this.centerFrame(e);
                }, 100);
                return;
            }
            if (e.offsetWidth <= 0 || e.offsetHeight <= 0) {
                setTimeout(function () {
                    _this.centerFrame(e);
                }, 100);
                return;
            }
            var x = (parent.offsetWidth - e.offsetWidth) / 2;
            var y = (parent.offsetHeight - e.offsetHeight) / 2;
            this.x = x;
            this.y = y;
            e.style.opacity = "1";
            this.element.style.removeProperty("opacity");
        };
        AtomWindow.prototype.setupDragging = function (tp) {
            var _this = this;
            this.bindEvent(tp, "mousedown", function (startEvent) {
                startEvent.preventDefault();
                var disposables = [];
                // const offset = AtomUI.screenOffset(tp);
                var offset = { x: tp.parentElement.offsetLeft, y: tp.parentElement.offsetTop };
                var rect = { x: startEvent.clientX, y: startEvent.clientY };
                var cursor = tp.style.cursor;
                tp.style.cursor = "move";
                disposables.push(_this.bindEvent(document.body, "mousemove", function (moveEvent) {
                    var clientX = moveEvent.clientX, clientY = moveEvent.clientY;
                    var dx = clientX - rect.x;
                    var dy = clientY - rect.y;
                    offset.x += dx;
                    offset.y += dy;
                    _this.x = offset.x;
                    _this.y = offset.y;
                    rect.x = clientX;
                    rect.y = clientY;
                }));
                disposables.push(_this.bindEvent(document.body, "mouseup", function (endEvent) {
                    tp.style.cursor = cursor;
                    for (var _i = 0, disposables_1 = disposables; _i < disposables_1.length; _i++) {
                        var iterator = disposables_1[_i];
                        iterator.dispose();
                    }
                }));
            });
        };
        AtomWindow.windowTemplate = XNode_1.default.prepare("windowTemplate", true, true);
        AtomWindow.commandTemplate = XNode_1.default.prepare("commandTemplate", true, true);
        AtomWindow.titleTemplate = XNode_1.default.prepare("titleTemplate", true, true);
        AtomWindow.frameTemplate = XNode_1.default.prepare("frameTemplate", true, true);
        return AtomWindow;
    }(AtomControl_1.AtomControl));
    exports.AtomWindow = AtomWindow;
});
//# sourceMappingURL=AtomWindow.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomWindow");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
        define(["require", "exports", "../../core/Bind", "../../core/BindableProperty", "../../core/XNode", "../../view-model/AtomWindowViewModel", "../styles/AtomAlertWindowStyle", "./AtomWindow"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("../../core/Bind");
    var BindableProperty_1 = require("../../core/BindableProperty");
    var XNode_1 = require("../../core/XNode");
    var AtomWindowViewModel_1 = require("../../view-model/AtomWindowViewModel");
    var AtomAlertWindowStyle_1 = require("../styles/AtomAlertWindowStyle");
    var AtomWindow_1 = require("./AtomWindow");
    var AtomAlertWindow = /** @class */ (function (_super) {
        __extends(AtomAlertWindow, _super);
        function AtomAlertWindow() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        AtomAlertWindow.prototype.create = function () {
            var _this = this;
            this.defaultControlStyle = AtomAlertWindowStyle_1.default;
            this.viewModel = this.resolve(AtomAlertViewModel);
            // this.windowTemplate = AtomAlertWindowTemplate;
            // this.commandTemplate =  AtomAlertWindowCommandBar;
            // this.bind(this.element, "title", [["viewModel", "title"]]);
            this.render(XNode_1.default.create(AtomWindow_1.AtomWindow, { title: Bind_1.default.oneWay(function () { return _this.viewModel.title; }) },
                XNode_1.default.create(AtomWindow_1.AtomWindow.windowTemplate, null,
                    XNode_1.default.create("div", { formattedText: Bind_1.default.oneWay(function () { return _this.viewModel.message; }) })),
                XNode_1.default.create(AtomWindow_1.AtomWindow.commandTemplate, null,
                    XNode_1.default.create("div", null,
                        XNode_1.default.create("button", { class: "yes-button", styleDisplay: Bind_1.default.oneWay(function () { return _this.viewModel.okTitle ? "" : "none"; }), text: Bind_1.default.oneWay(function () { return _this.viewModel.okTitle; }), eventClick: function () { return _this.viewModel.onOkClicked(); } }),
                        XNode_1.default.create("button", { class: "no-button", styleMarginBottom: Bind_1.default.oneWay(function () { return _this.viewModel.cancelTitle ? "0" : "10px"; }), styleDisplay: Bind_1.default.oneWay(function () { return _this.viewModel.cancelTitle ? "" : "none"; }), text: Bind_1.default.oneWay(function () { return _this.viewModel.cancelTitle; }), eventClick: function () { return _this.viewModel.onCancelClicked(); } })))));
        };
        return AtomAlertWindow;
    }(AtomWindow_1.AtomWindow));
    exports.default = AtomAlertWindow;
    // class AtomAlertWindowTemplate extends AtomControl {
    //     protected create(): void {
    //         const div = document.createElement("div");
    //         this.append(div);
    //         this.bind(div, "formattedText", [["viewModel", "message"]]);
    //     }
    // }
    // class AtomAlertWindowCommandBar extends AtomControl {
    //     protected create(): void {
    //         const okButton = document.createElement("button");
    //         const cancelButton = document.createElement("button");
    //         this.append(okButton);
    //         this.append(cancelButton);
    //         this.setPrimitiveValue(okButton, "class", "yes-button" );
    //         this.setPrimitiveValue(cancelButton, "class", "no-button" );
    //         this.bind(okButton, "text", [["viewModel", "okTitle"]]);
    //         this.bind(cancelButton, "text", [["viewModel", "cancelTitle"]]);
    //         this.bind(okButton, "styleDisplay", [["viewModel", "okTitle"]], false, (v) => v ? "" : "none");
    //         this.bind(okButton, "styleMarginBottom", [["viewModel", "cancelTitle"]], false, (v) => v ? "0" : "10px");
    //         this.bind(cancelButton, "styleDisplay", [["viewModel", "cancelTitle"]], false, (v) => v ? "" : "none");
    //         this.bindEvent(okButton, "click", (e) => {
    //             this.viewModel.onOkClicked();
    //         });
    //         this.bindEvent(cancelButton, "click", (e) => {
    //             this.viewModel.onCancelClicked();
    //         });
    //     }
    // }
    var AtomAlertViewModel = /** @class */ (function (_super) {
        __extends(AtomAlertViewModel, _super);
        function AtomAlertViewModel() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        AtomAlertViewModel.prototype.onOkClicked = function () {
            this.close(true);
        };
        AtomAlertViewModel.prototype.onCancelClicked = function () {
            this.cancel();
        };
        __decorate([
            BindableProperty_1.BindableProperty,
            __metadata("design:type", String)
        ], AtomAlertViewModel.prototype, "title", void 0);
        __decorate([
            BindableProperty_1.BindableProperty,
            __metadata("design:type", String)
        ], AtomAlertViewModel.prototype, "message", void 0);
        __decorate([
            BindableProperty_1.BindableProperty,
            __metadata("design:type", String)
        ], AtomAlertViewModel.prototype, "okTitle", void 0);
        __decorate([
            BindableProperty_1.BindableProperty,
            __metadata("design:type", String)
        ], AtomAlertViewModel.prototype, "cancelTitle", void 0);
        return AtomAlertViewModel;
    }(AtomWindowViewModel_1.AtomWindowViewModel));
});
//# sourceMappingURL=AtomAlertWindow.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomAlertWindow");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../core/Colors", "./AtomStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Colors_1 = require("../../core/Colors");
    var AtomStyle_1 = require("./AtomStyle");
    var AtomNotificationStyle = /** @class */ (function (_super) {
        __extends(AtomNotificationStyle, _super);
        function AtomNotificationStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomNotificationStyle.prototype, "root", {
            get: function () {
                return {
                    padding: "5px",
                    borderRadius: "5px",
                    border: "solid 1px lightgray",
                    fontFamily: "Verdana, Geneva, sans-serif",
                    fontSize: "16px",
                    subclasses: {
                        ".error": {
                            borderColor: Colors_1.default.red,
                            color: Colors_1.default.red,
                        },
                        ".warning": {
                            backgroundColor: Colors_1.default.lightYellow
                        }
                    }
                };
            },
            enumerable: false,
            configurable: true
        });
        return AtomNotificationStyle;
    }(AtomStyle_1.AtomStyle));
    exports.default = AtomNotificationStyle;
});
//# sourceMappingURL=AtomNotificationStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomNotificationStyle");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
        define(["require", "exports", "../../core/Bind", "../../core/BindableProperty", "../../core/XNode", "../styles/AtomNotificationStyle", "./AtomControl"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("../../core/Bind");
    var BindableProperty_1 = require("../../core/BindableProperty");
    var XNode_1 = require("../../core/XNode");
    var AtomNotificationStyle_1 = require("../styles/AtomNotificationStyle");
    var AtomControl_1 = require("./AtomControl");
    var AtomNotification = /** @class */ (function (_super) {
        __extends(AtomNotification, _super);
        function AtomNotification() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.timeout = 5000;
            _this.timeoutKey = null;
            return _this;
        }
        AtomNotification.prototype.onPropertyChanged = function (name) {
            switch (name) {
                case "timeout":
                    this.setupTimeout();
                    break;
            }
        };
        AtomNotification.prototype.create = function () {
            var _this = this;
            this.defaultControlStyle = AtomNotificationStyle_1.default;
            this.render(XNode_1.default.create("div", { formattedText: Bind_1.default.oneWay(function () { return _this.viewModel.message; }), timeout: Bind_1.default.oneWay(function () { return _this.viewModel.timeout || 5000; }), styleClass: Bind_1.default.oneWay(function () {
                    var _a;
                    return (_a = {},
                        _a[_this.controlStyle.name] = 1,
                        _a.error = _this.viewModel.type && /error/i.test(_this.viewModel.type),
                        _a.warning = _this.viewModel.type && /warn/i.test(_this.viewModel.type),
                        _a);
                }) }));
        };
        AtomNotification.prototype.setupTimeout = function () {
            var _this = this;
            if (this.timeoutKey) {
                clearTimeout(this.timeoutKey);
            }
            this.timeoutKey = setTimeout(function () {
                if (_this.element) {
                    _this.app.broadcast("atom-window-close:" + _this.id, "");
                }
            }, this.timeout);
        };
        __decorate([
            BindableProperty_1.BindableProperty,
            __metadata("design:type", Number)
        ], AtomNotification.prototype, "timeout", void 0);
        return AtomNotification;
    }(AtomControl_1.AtomControl));
    exports.default = AtomNotification;
});
//# sourceMappingURL=AtomNotification.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/controls/AtomNotification");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../styles/AtomStyle"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomPopupStyle = void 0;
    var AtomStyle_1 = require("../styles/AtomStyle");
    var AtomPopupStyle = /** @class */ (function (_super) {
        __extends(AtomPopupStyle, _super);
        function AtomPopupStyle() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Object.defineProperty(AtomPopupStyle.prototype, "root", {
            get: function () {
                return {
                    backgroundColor: "white",
                    border: "solid 1px lightgray",
                    padding: "5px",
                    borderRadius: "5px"
                };
            },
            enumerable: false,
            configurable: true
        });
        return AtomPopupStyle;
    }(AtomStyle_1.AtomStyle));
    exports.AtomPopupStyle = AtomPopupStyle;
});
//# sourceMappingURL=AtomPopupStyle.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomPopupStyle");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../../App", "../../Atom", "../../core/AtomLoader", "../../core/AtomUri", "../../core/FormattedError", "../../core/FormattedString", "../../di/Inject", "../../di/RegisterSingleton", "../../di/ServiceCollection", "../../services/JsonService", "../../services/NavigationService", "../../view-model/AtomWindowViewModel", "../../web/core/AtomUI", "../controls/AtomAlertWindow", "../controls/AtomNotification", "../controls/AtomWindow", "../styles/AtomPopupStyle", "../styles/AtomStyleSheet", "../styles/StyleBuilder"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.WindowService = void 0;
    var App_1 = require("../../App");
    var Atom_1 = require("../../Atom");
    var AtomLoader_1 = require("../../core/AtomLoader");
    var AtomUri_1 = require("../../core/AtomUri");
    var FormattedError_1 = require("../../core/FormattedError");
    var FormattedString_1 = require("../../core/FormattedString");
    var Inject_1 = require("../../di/Inject");
    var RegisterSingleton_1 = require("../../di/RegisterSingleton");
    var ServiceCollection_1 = require("../../di/ServiceCollection");
    var JsonService_1 = require("../../services/JsonService");
    var NavigationService_1 = require("../../services/NavigationService");
    var AtomWindowViewModel_1 = require("../../view-model/AtomWindowViewModel");
    var AtomUI_1 = require("../../web/core/AtomUI");
    var AtomAlertWindow_1 = require("../controls/AtomAlertWindow");
    var AtomNotification_1 = require("../controls/AtomNotification");
    var AtomWindow_1 = require("../controls/AtomWindow");
    var AtomPopupStyle_1 = require("../styles/AtomPopupStyle");
    var AtomStyleSheet_1 = require("../styles/AtomStyleSheet");
    var StyleBuilder_1 = require("../styles/StyleBuilder");
    var WindowService = /** @class */ (function (_super) {
        __extends(WindowService, _super);
        function WindowService(app, jsonService) {
            var _this = _super.call(this, app) || this;
            _this.jsonService = jsonService;
            _this.targetStack = [];
            _this.popups = [];
            _this.hostForElementFunc = [];
            _this.lastPopupID = 0;
            _this.screen = app.screen;
            var st = "desktop";
            if (/mobile|android|ios/i.test(window.navigator.userAgent)) {
                st = "mobile";
                if (/tablet/i.test(window.navigator.userAgent)) {
                    st = "tablet";
                }
            }
            _this.screen.screenType = st;
            if (window) {
                window.addEventListener("click", function (e) {
                    // this.currentTarget = e.target as HTMLElement;
                    _this.closePopup(e);
                });
                var update_1 = function (e) {
                    _this.refreshScreen();
                };
                // we don't do this in mobile..
                if (st !== "mobile") {
                    window.addEventListener("resize", update_1);
                    window.addEventListener("scroll", update_1);
                    document.body.addEventListener("scroll", update_1);
                    document.body.addEventListener("resize", update_1);
                }
                setTimeout(function () {
                    update_1(null);
                }, 1000);
            }
            return _this;
        }
        Object.defineProperty(WindowService.prototype, "currentTarget", {
            get: function () {
                var ts = this.targetStack;
                return ts.length > 0 ? ts[ts.length - 1] : undefined;
            },
            set: function (v) {
                var ts = this.targetStack;
                var nts = [];
                if (v === null) {
                    // special case... remove all non existent elements...
                    for (var _i = 0, ts_1 = ts; _i < ts_1.length; _i++) {
                        var iterator = ts_1[_i];
                        if (iterator.parentElement) {
                            nts.push(iterator);
                        }
                    }
                    this.targetStack = nts;
                    return;
                }
                if (ts.length === 0 && ts[ts.length - 1] === v) {
                    return;
                }
                ts.push(v);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WindowService.prototype, "title", {
            /**
             * Get current window title
             *
             * @type {string}
             * @memberof BrowserService
             */
            get: function () {
                return window.document.title;
            },
            /**
             * Set current window title
             * @memberof BrowserService
             */
            set: function (v) {
                window.document.title = v;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WindowService.prototype, "location", {
            /**
             * Gets current location of browser, this does not return
             * actual location but it returns values of browser location.
             * This is done to provide mocking behavior for unit testing.
             *
             * @readonly
             * @type {AtomLocation}
             * @memberof BrowserService
             */
            get: function () {
                return new AtomUri_1.AtomUri(location.href);
            },
            set: function (v) {
                location.href = v.toString();
            },
            enumerable: false,
            configurable: true
        });
        WindowService.prototype.registerHostForWindow = function (f) {
            var _this = this;
            this.hostForElementFunc.push(f);
            return {
                dispose: function () {
                    _this.hostForElementFunc.remove(f);
                }
            };
        };
        /**
         * Navigate current browser to given url.
         * @param {string} url
         * @memberof BrowserService
         */
        WindowService.prototype.navigate = function (url) {
            location.href = url;
        };
        WindowService.prototype.back = function () {
            window.history.back();
        };
        WindowService.prototype.register = function (id, type) {
            ServiceCollection_1.ServiceCollection.instance.register(type, null, ServiceCollection_1.Scope.Transient, id);
        };
        WindowService.prototype.confirm = function (message, title) {
            return __awaiter(this, void 0, void 0, function () {
                var e_1;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            _a.trys.push([0, 2, , 3]);
                            return [4 /*yield*/, this.openPage(AtomAlertWindow_1.default, {
                                    okTitle: "Yes",
                                    cancelTitle: "No",
                                    title: title || "Confirm",
                                    message: message
                                })];
                        case 1: return [2 /*return*/, _a.sent()];
                        case 2:
                            e_1 = _a.sent();
                            if (/canceled|cancelled/i.test(e_1)) {
                                return [2 /*return*/, false];
                            }
                            throw e_1;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        };
        WindowService.prototype.alert = function (message, title) {
            if (!(message instanceof FormattedString_1.default || typeof message === "string")) {
                if (message instanceof FormattedError_1.default) {
                    message = message.formattedMessage;
                }
                else {
                    message = message.message ? message.message : message.toString();
                }
            }
            return this.openPage(AtomAlertWindow_1.default, {
                message: message,
                title: title,
                okTitle: "Ok",
                cancelTitle: ""
            }).catch(function (e) {
                // do nothing...
                // tslint:disable-next-line: no-console
                console.warn(e);
            });
        };
        WindowService.prototype.closePopup = function (e) {
            // need to simulate parent click if we are inside an iframe...
            var fe = typeof frameElement !== "undefined" ? frameElement : null;
            if (fe) {
                fe.click();
                var pe = fe.ownerDocument ? fe.ownerDocument.defaultView : null;
                if (pe && pe.simulateParentClick) {
                    pe.simulateParentClick();
                }
            }
            var target = this.currentTarget;
            var et = e.target;
            if (!et.parentElement) {
                // probably the window/popup was just disposed..
                // ignore it...
                // if mouse click was outside body and within the window
                // target element will be HTML
                // in that case we have to dispose the top popup
                if (!/html/i.test(et.tagName)) {
                    return;
                }
                // we need to manually override target so popup will be disposed
                target = et;
            }
            this.currentTarget = e.target;
            if (!this.popups.length) {
                return;
            }
            var peek = this.popups[this.popups.length - 1];
            var element = peek.element;
            while (target) {
                if (target === element) {
                    // do not close this popup....
                    return;
                }
                if (element._logicalParent === target) {
                    return;
                }
                target = target.parentElement;
            }
            this.remove(peek);
        };
        WindowService.prototype.refresh = function () {
            location.reload(true);
        };
        WindowService.prototype.getHostForElement = function () {
            var ce = this.currentTarget;
            if (!ce) {
                return null;
            }
            for (var _i = 0, _a = this.hostForElementFunc; _i < _a.length; _i++) {
                var iterator = _a[_i];
                var e = iterator(ce);
                if (e) {
                    return e;
                }
            }
            return null;
        };
        WindowService.prototype.refreshScreen = function () {
            var height = this.screen.height = window.innerHeight || document.body.clientHeight;
            var width = this.screen.width = window.innerWidth || document.body.clientWidth;
            this.screen.scrollLeft = window.scrollX || document.body.scrollLeft || 0;
            this.screen.scrollTop = window.scrollY || document.body.scrollTop || 0;
            this.screen.orientation = width > height ? "landscape" : "portrait";
        };
        WindowService.prototype.notify = function (message, title, type, delay) {
            var _this = this;
            this.app.runAsync(function () { return _this.openPage(AtomNotification_1.default, {
                message: message,
                title: title,
                type: type || NavigationService_1.NotifyType.Information,
                timeout: delay
            }); });
        };
        WindowService.prototype.registerForPopup = function () {
            var _this = this;
            if (window) {
                window.addEventListener("click", function (e) {
                    _this.closePopup(e);
                });
            }
        };
        WindowService.prototype.openWindow = function (url, options) {
            return __awaiter(this, void 0, void 0, function () {
                var lastTarget, _a, popup, returnPromise, disposables, cancelToken, e, isPopup, pvm, ce, theme, isNotification, sr, x, y, h, eHost, host_1;
                var _this = this;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0: 
                        // this is because current target is not yet set
                        return [4 /*yield*/, Atom_1.Atom.delay(1)];
                        case 1:
                            // this is because current target is not yet set
                            _b.sent();
                            lastTarget = this.currentTarget;
                            return [4 /*yield*/, AtomLoader_1.AtomLoader.loadView(url, this.app, true, function () { return _this.app.resolve(AtomWindowViewModel_1.AtomWindowViewModel, true); })];
                        case 2:
                            _a = _b.sent(), popup = _a.view, returnPromise = _a.returnPromise, disposables = _a.disposables;
                            if (options && options.onInit) {
                                options.onInit(popup);
                            }
                            cancelToken = options.cancelToken;
                            if (cancelToken) {
                                if (cancelToken.cancelled) {
                                    this.app.callLater(function () {
                                        _this.remove(popup, true);
                                    });
                                }
                                cancelToken.registerForCancel(function () {
                                    _this.remove(popup, true);
                                });
                            }
                            e = popup.element;
                            isPopup = true;
                            if (popup instanceof AtomWindow_1.AtomWindow) {
                                isPopup = false;
                                e.style.opacity = "0";
                            }
                            e._logicalParent = lastTarget;
                            e.sourceUrl = url;
                            pvm = popup.viewModel;
                            if (pvm) {
                                ce = this.currentTarget;
                                if (ce) {
                                    while (!ce.atomControl) {
                                        ce = ce.parentElement;
                                        if (!ce) {
                                            break;
                                        }
                                    }
                                    if (ce && ce.atomControl && ce.atomControl.viewModel) {
                                        pvm.parent = ce.atomControl.viewModel;
                                    }
                                }
                            }
                            theme = this.app.get(AtomStyleSheet_1.AtomStyleSheet).getNamedStyle(AtomPopupStyle_1.AtomPopupStyle);
                            e.style.zIndex = 10000 + this.lastPopupID + "";
                            isNotification = popup instanceof AtomNotification_1.default;
                            if (isPopup) {
                                sr = AtomUI_1.AtomUI.screenOffset(this.currentTarget);
                                x = sr.x;
                                y = sr.y;
                                h = sr.height;
                                e.style.position = "absolute";
                                e.style.left = x + "px";
                                e.style.top = (y + h) + "px";
                                e.classList.add(theme.name);
                                this.popups.push(popup);
                                disposables.add(function () {
                                    _this.popups.remove(popup);
                                });
                                document.body.appendChild(e);
                                if (isNotification) {
                                    e.style.opacity = "0";
                                    this.centerElement(popup);
                                }
                            }
                            else {
                                eHost = this.getHostForElement();
                                if (eHost) {
                                    eHost.appendChild(e);
                                }
                                else {
                                    host_1 = document.createElement("div");
                                    document.body.appendChild(host_1);
                                    host_1.style.position = "absolute";
                                    host_1.appendChild(e);
                                    disposables.add({
                                        dispose: function () {
                                            host_1.remove();
                                        }
                                    });
                                    this.refreshScreen();
                                    popup.bind(host_1, "styleLeft", [["this", "scrollLeft"]], false, StyleBuilder_1.cssNumberToString, this.screen);
                                    popup.bind(host_1, "styleTop", [["this", "scrollTop"]], false, StyleBuilder_1.cssNumberToString, this.screen);
                                    popup.bind(host_1, "styleWidth", [["this", "width"]], false, StyleBuilder_1.cssNumberToString, this.screen);
                                    popup.bind(host_1, "styleHeight", [["this", "height"]], false, StyleBuilder_1.cssNumberToString, this.screen);
                                }
                            }
                            this.currentTarget = e;
                            popup.bindEvent(document.body, "keyup", function (keyboardEvent) {
                                if (keyboardEvent.key === "Escape") {
                                    _this.app.runAsync(function () { return _this.remove(popup); });
                                }
                            });
                            disposables.add({
                                dispose: function () {
                                    e.innerHTML = "";
                                    e.remove();
                                    _this.currentTarget = null;
                                }
                            });
                            return [4 /*yield*/, returnPromise];
                        case 3: return [2 /*return*/, _b.sent()];
                    }
                });
            });
        };
        WindowService.prototype.centerElement = function (c) {
            var _this = this;
            var e = c.element;
            var parent = e.parentElement;
            if (parent === window || parent === document.body) {
                setTimeout(function () {
                    var ew = (document.body.offsetWidth - e.offsetWidth) / 2;
                    var eh = window.scrollY + ((window.innerHeight - e.offsetHeight) / 2);
                    e.style.left = ew + "px";
                    e.style.top = eh + "px";
                    e.style.removeProperty("opacity");
                }, 200);
                return;
            }
            if (parent.offsetWidth <= 0 || parent.offsetHeight <= 0) {
                setTimeout(function () {
                    _this.centerElement(c);
                }, 100);
                return;
            }
            if (e.offsetWidth <= 0 || e.offsetHeight <= 0) {
                setTimeout(function () {
                    _this.centerElement(c);
                }, 100);
                return;
            }
            var x = (parent.offsetWidth - e.offsetWidth) / 2;
            var y = (parent.offsetHeight - e.offsetHeight) / 2;
            e.style.left = x + "px";
            e.style.top = y + "px";
            e.style.removeProperty("opacity");
        };
        /**
         * This is just to preload Alert window.
         */
        WindowService.alertWindow = AtomAlertWindow_1.default;
        WindowService = __decorate([
            RegisterSingleton_1.RegisterSingleton,
            __param(0, Inject_1.Inject),
            __param(1, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App,
                JsonService_1.JsonService])
        ], WindowService);
        return WindowService;
    }(NavigationService_1.NavigationService));
    exports.WindowService = WindowService;
});
//# sourceMappingURL=WindowService.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/services/WindowService");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
        define(["require", "exports", "../../App", "../../core/Colors", "../../di/Inject", "../../di/RegisterSingleton", "../../services/NavigationService", "../styles/AtomStyleSheet"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.AtomTheme = void 0;
    var App_1 = require("../../App");
    var Colors_1 = require("../../core/Colors");
    var Inject_1 = require("../../di/Inject");
    var RegisterSingleton_1 = require("../../di/RegisterSingleton");
    var NavigationService_1 = require("../../services/NavigationService");
    var AtomStyleSheet_1 = require("../styles/AtomStyleSheet");
    var AtomTheme = /** @class */ (function (_super) {
        __extends(AtomTheme, _super);
        // public readonly window = this.createStyle(AtomWindow, AtomWindowStyle, "window");
        // public readonly popup = this.createNamedStyle(AtomPopupStyle, "popup");
        function AtomTheme(app, navigationService) {
            var _this = _super.call(this, app, "atom-theme") || this;
            _this.navigationService = navigationService;
            _this.bgColor = Colors_1.default.white;
            _this.color = Colors_1.default.gray;
            _this.hoverColor = Colors_1.default.lightGray;
            _this.activeColor = Colors_1.default.lightBlue;
            _this.selectedBgColor = Colors_1.default.blue;
            _this.selectedColor = Colors_1.default.white;
            _this.padding = 5;
            setTimeout(function () {
                window.addEventListener("resize", function () {
                    setTimeout(function () {
                        _this.pushUpdate();
                    }, 10);
                });
                document.body.addEventListener("resize", function () {
                    setTimeout(function () {
                        _this.pushUpdate();
                    }, 10);
                });
            }, 1000);
            return _this;
        }
        AtomTheme = __decorate([
            RegisterSingleton_1.RegisterSingleton,
            __param(0, Inject_1.Inject),
            __param(1, Inject_1.Inject),
            __metadata("design:paramtypes", [App_1.App,
                NavigationService_1.NavigationService])
        ], AtomTheme);
        return AtomTheme;
    }(AtomStyleSheet_1.AtomStyleSheet));
    exports.AtomTheme = AtomTheme;
});
//# sourceMappingURL=AtomTheme.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/styles/AtomTheme");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "../App", "../core/AtomOnce", "../core/AtomUri", "../di/ServiceCollection", "../services/BusyIndicatorService", "../services/NavigationService", "./core/AtomUI", "./services/WebBusyIndicatorService", "./services/WindowService", "./styles/AtomStyleSheet", "./styles/AtomTheme"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var App_1 = require("../App");
    var AtomOnce_1 = require("../core/AtomOnce");
    var AtomUri_1 = require("../core/AtomUri");
    var ServiceCollection_1 = require("../di/ServiceCollection");
    var BusyIndicatorService_1 = require("../services/BusyIndicatorService");
    var NavigationService_1 = require("../services/NavigationService");
    var AtomUI_1 = require("./core/AtomUI");
    var WebBusyIndicatorService_1 = require("./services/WebBusyIndicatorService");
    var WindowService_1 = require("./services/WindowService");
    var AtomStyleSheet_1 = require("./styles/AtomStyleSheet");
    var AtomTheme_1 = require("./styles/AtomTheme");
    var WebApp = /** @class */ (function (_super) {
        __extends(WebApp, _super);
        function WebApp() {
            var _this = _super.call(this) || this;
            _this.mContextId = 1;
            _this.hashUpdater = new AtomOnce_1.AtomOnce();
            _this.url = new AtomUri_1.AtomUri(location.href);
            _this.put(NavigationService_1.NavigationService, _this.resolve(WindowService_1.WindowService));
            _this.put(WebApp, _this);
            _this.put(BusyIndicatorService_1.BusyIndicatorService, _this.resolve(WebBusyIndicatorService_1.WebBusyIndicatorService));
            ServiceCollection_1.ServiceCollection.instance.registerSingleton(AtomStyleSheet_1.AtomStyleSheet, function (sp) { return sp.resolve(AtomTheme_1.AtomTheme); });
            // let us set contextId
            _this.mContextId = parseInt((_this.url.hash.contextId || "0").toString(), 10);
            if (!_this.mContextId) {
                //  create new context Id in session...
                for (var index = 0; index < 100; index++) {
                    var cid = "contextId" + index;
                    var cidData = sessionStorage.getItem("contextId" + index);
                    if (!cidData) {
                        _this.mContextId = index;
                        sessionStorage.setItem(cid, cid);
                        _this.url.hash.contextId = index;
                        _this.syncUrl();
                        break;
                    }
                }
            }
            window.addEventListener("hashchange", function () {
                _this.hashUpdater.run(function () {
                    _this.url = new AtomUri_1.AtomUri(location.href);
                });
            });
            // registering font awesome
            _this.installStyleSheet("https://cdn.jsdelivr.net/npm/@fortawesome/fontawesome-free@5.9.0/css/all.css");
            _this.installStyleSheet({
                href: "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css",
                integrity: "sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T",
                crossOrigin: "anonymous"
            });
            return _this;
        }
        Object.defineProperty(WebApp.prototype, "parentElement", {
            get: function () {
                return document.body;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WebApp.prototype, "root", {
            get: function () {
                return this.mRoot;
            },
            set: function (v) {
                var old = this.mRoot;
                if (old) {
                    old.dispose();
                }
                this.mRoot = v;
                if (!v) {
                    return;
                }
                var pe = this.parentElement;
                var ce = new AtomUI_1.ChildEnumerator(pe);
                var de = [];
                while (ce.next()) {
                    de.push(ce.current);
                }
                for (var _i = 0, de_1 = de; _i < de_1.length; _i++) {
                    var iterator = de_1[_i];
                    iterator.remove();
                }
                pe.appendChild(v.element);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WebApp.prototype, "theme", {
            get: function () {
                return this.get(AtomStyleSheet_1.AtomStyleSheet);
            },
            set: function (v) {
                this.put(AtomTheme_1.AtomTheme, v);
                this.put(AtomStyleSheet_1.AtomStyleSheet, v);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(WebApp.prototype, "contextId", {
            get: function () {
                return "contextId_" + this.mContextId;
            },
            enumerable: false,
            configurable: true
        });
        WebApp.prototype.installStyleSheet = function (ssConfig) {
            if (typeof ssConfig !== "object") {
                ssConfig = { href: ssConfig };
            }
            ssConfig.href = UMD.resolvePath(ssConfig.href);
            var links = document.getElementsByTagName("link");
            // tslint:disable-next-line:prefer-for-of
            for (var index = 0; index < links.length; index++) {
                var element = links[index];
                var href = element.getAttribute("href");
                if (href === ssConfig.href) {
                    return;
                }
            }
            var ss = document.createElement("link");
            ss.rel = "stylesheet";
            ss.href = ssConfig.href;
            if (ssConfig.crossOrigin) {
                ss.crossOrigin = ssConfig.crossOrigin;
            }
            if (ssConfig.integrity) {
                ss.integrity = ssConfig.integrity;
            }
            document.head.appendChild(ss);
        };
        WebApp.prototype.installScript = function (location) {
            location = UMD.resolvePath(location);
            var links = document.getElementsByTagName("script");
            // tslint:disable-next-line:prefer-for-of
            for (var index = 0; index < links.length; index++) {
                var element = links[index];
                var href = element.getAttribute("src");
                if (href === location) {
                    return element.loaderPromise;
                }
            }
            var script = document.createElement("script");
            var p = new Promise(function (resolve, reject) {
                script.type = "text/javascript";
                script.src = location;
                var s = script;
                script.onload = s.onreadystatechange = function () {
                    if ((s.readyState && s.readyState !== "complete" && s.readyState !== "loaded")) {
                        return;
                    }
                    script.onload = s.onreadystatechange = null;
                    resolve();
                };
                document.body.appendChild(script);
            });
            script.loaderPromise = p;
            return p;
        };
        WebApp.prototype.updateDefaultStyle = function (textContent) {
            if (this.styleElement) {
                if (this.styleElement.textContent === textContent) {
                    return;
                }
            }
            var ss = document.createElement("style");
            ss.textContent = textContent;
            if (this.styleElement) {
                this.styleElement.remove();
            }
            document.head.appendChild(ss);
            this.styleElement = ss;
        };
        /**
         * Do not use this method
         */
        WebApp.prototype.syncUrl = function () {
            var _this = this;
            this.hashUpdater.run(function () {
                var currentUrl = new AtomUri_1.AtomUri(location.href);
                var sourceHash = _this.url.hash;
                var keyValues = [];
                var modified = false;
                for (var key in sourceHash) {
                    if (/^\_\$\_/.test(key)) {
                        continue;
                    }
                    if (sourceHash.hasOwnProperty(key)) {
                        var element = sourceHash[key];
                        var cv = currentUrl.hash[key];
                        if (element !== undefined) {
                            keyValues.push({ key: key, value: element });
                        }
                        if (cv === element) {
                            continue;
                        }
                        modified = true;
                    }
                }
                if (!modified) {
                    return;
                }
                var hash = keyValues.map(function (s) { return s.key + "=" + encodeURIComponent(s.value); }).join("&");
                location.hash = hash;
            });
        };
        WebApp.prototype.invokeReady = function () {
            var _this = this;
            if (document.readyState === "complete") {
                _super.prototype.invokeReady.call(this);
                return;
            }
            document.addEventListener("readystatechange", function (e) {
                _super.prototype.invokeReady.call(_this);
            });
        };
        return WebApp;
    }(App_1.App));
    exports.default = WebApp;
    // tslint:disable-next-line: only-arrow-functions
    (function () {
        if (typeof window.CustomEvent === "function") {
            return false;
        }
        function CustomEvent(event, params) {
            params = params || { bubbles: false, cancelable: false, detail: null };
            var evt = document.createEvent("CustomEvent");
            evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
            return evt;
        }
        window.CustomEvent = CustomEvent;
    })();
});
//# sourceMappingURL=WebApp.js.map

    AmdLoader.instance.setup("@web-atoms/core/dist/web/WebApp");

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "@web-atoms/core/dist/core/Bind", "@web-atoms/core/dist/core/XNode", "@web-atoms/core/dist/web/controls/AtomControl", "@web-atoms/samples/src/web/images/about-img.svg", "@web-atoms/samples/src/web/images/hero-img.svg", "@web-atoms/samples/src/web/images/logo.png", "../samples/web/form/FromDemo", "../view-models/IndexViewModel", "./styles/IndexStyle", "@web-atoms/core/dist/web/WebApp", "@web-atoms/core/dist/web/services/WindowService", "@web-atoms/samples/src/images/cs.png"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Bind_1 = require("@web-atoms/core/dist/core/Bind");
    var XNode_1 = require("@web-atoms/core/dist/core/XNode");
    var AtomControl_1 = require("@web-atoms/core/dist/web/controls/AtomControl");
    var about_img_svg_1 = require("@web-atoms/samples/src/web/images/about-img.svg");
    var hero_img_svg_1 = require("@web-atoms/samples/src/web/images/hero-img.svg");
    var logo_png_1 = require("@web-atoms/samples/src/web/images/logo.png");
    var FromDemo_1 = require("../samples/web/form/FromDemo");
    var IndexViewModel_1 = require("../view-models/IndexViewModel");
    var IndexStyle_1 = require("./styles/IndexStyle");
    var WebApp_1 = require("@web-atoms/core/dist/web/WebApp");
    exports.WebApp = WebApp_1.default;
    var WindowService_1 = require("@web-atoms/core/dist/web/services/WindowService");
    exports.W = WindowService_1.WindowService;
    var cs_png_1 = require("@web-atoms/samples/src/images/cs.png");
    // @web-atoms-pack: true
    /** XF Home Page */
    var Index = /** @class */ (function (_super) {
        __extends(Index, _super);
        function Index() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        Index.prototype.create = function () {
            var _this = this;
            this.defaultControlStyle = IndexStyle_1.default;
            this.viewModel = this.resolve(IndexViewModel_1.default);
            this.render(XNode_1.default.create("div", { styleClass: Bind_1.default.oneTime(function () { return _this.controlStyle.name; }), id: "page-top" },
                XNode_1.default.create("header", { id: "header", class: "fixed-top" },
                    XNode_1.default.create("div", { class: "container-fluid d-flex" },
                        XNode_1.default.create("div", { class: "logo mr-auto" },
                            XNode_1.default.create("h1", { class: "text-light" },
                                XNode_1.default.create("a", { href: "index.html" },
                                    XNode_1.default.create("span", null,
                                        XNode_1.default.create("img", { src: logo_png_1.default, width: "50", height: "50" }),
                                        "WebAtoms")))),
                        XNode_1.default.create("button", { type: "button", class: "mobile-nav-toggle d-lg-none", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }) },
                            XNode_1.default.create("i", { class: "fas fa-bars", styleClass: Bind_1.default.oneWay(function () { return _this.viewModel.collapsed ? "fas fa-times" : "fas fa-bars"; }), styleColor: Bind_1.default.oneWay(function () { return _this.viewModel.collapsed ? "White" : "#7a6960"; }) })),
                        XNode_1.default.create("nav", { styleClass: Bind_1.default.oneWay(function () {
                                return _this.viewModel.collapsed ? "mobile-nav d-lg-none" : "nav-menu d-none d-lg-block";
                            }) },
                            XNode_1.default.create("ul", null,
                                XNode_1.default.create("li", { class: "active" },
                                    XNode_1.default.create("a", { href: "#page-top", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }) }, "Home")),
                                XNode_1.default.create("li", null,
                                    XNode_1.default.create("a", { href: "#services", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }) }, "Features")),
                                XNode_1.default.create("li", null,
                                    XNode_1.default.create("a", { href: "/xf/samples.html", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }), target: "_blank" }, "XF Docs")),
                                XNode_1.default.create("li", null,
                                    XNode_1.default.create("a", { href: "/samples.html", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }), target: "_blank" }, "Web Docs")),
                                XNode_1.default.create("li", null,
                                    XNode_1.default.create("a", { href: "#team", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }) }, "Buy")),
                                XNode_1.default.create("li", null,
                                    XNode_1.default.create("a", { href: "#footer", eventClick: Bind_1.default.event(function (x) { return (x.viewModel).menuClick(); }) }, "Contact Us")),
                                XNode_1.default.create("li", { class: "get-started" },
                                    XNode_1.default.create("a", { href: "/play" }, "Play")),
                                XNode_1.default.create("li", { class: "get-started" },
                                    XNode_1.default.create("a", { href: "/account/licenses.html", target: "_tab" }, "Login")))),
                        XNode_1.default.create("div", { class: "mobile-nav-overly", styleDisplay: Bind_1.default.oneWay(function () { return _this.viewModel.collapsed ? "block" : "none"; }) }))),
                XNode_1.default.create("section", { id: "hero", class: "d-flex align-items-center" },
                    XNode_1.default.create("div", { class: "container" },
                        XNode_1.default.create("div", { class: "row" },
                            XNode_1.default.create("div", { class: "col-lg-6 pt-5 pt-lg-0 order-2 order-lg-1" },
                                XNode_1.default.create("h1", null,
                                    "JavaScript bridge for Xamarin.Forms",
                                    XNode_1.default.create("br", null)),
                                XNode_1.default.create("h2", null, "TypeScript + TSX for Xamarin.Forms, Hot Reload Your App in Production Environment"),
                                XNode_1.default.create("a", { href: "#download", class: "btn-get-started scrollto" }, "Get Started")),
                            XNode_1.default.create("div", { class: "col-lg-6 order-1 order-lg-2 hero-img" },
                                XNode_1.default.create("img", { src: hero_img_svg_1.default, class: "img-fluid animated", alt: "" }))))),
                XNode_1.default.create("main", { id: "main" },
                    XNode_1.default.create("section", { id: "about", class: "about" },
                        XNode_1.default.create("div", { class: "container" },
                            XNode_1.default.create("div", { class: "row justify-content-between" },
                                XNode_1.default.create("div", { class: "col-lg-5 d-flex align-items-center justify-content-center about-img" },
                                    XNode_1.default.create("img", { src: about_img_svg_1.default, class: "img-fluid", alt: "", "data-aos": "zoom-in" })),
                                XNode_1.default.create("div", { class: "col-lg-6 pt-5 pt-lg-0" },
                                    XNode_1.default.create("h3", { "data-aos": "fade-up" }, "MVVM Framework for Web and Xamarin.Forms"),
                                    XNode_1.default.create("p", { "data-aos": "fade-up", "data-aos-delay": "100" },
                                        "MVVM Pattern - ViewModel and Services in TypeScript for Web and Xamarin.Forms",
                                        XNode_1.default.create("br", null),
                                        "View in TSX (JSX) - for Web and Xamarin.Forms",
                                        XNode_1.default.create("br", null),
                                        "One time, One way, Two way Binding",
                                        XNode_1.default.create("br", null),
                                        "Simple Dependency Injection",
                                        XNode_1.default.create("br", null),
                                        "Simple Internationalization using Dependency Injection",
                                        XNode_1.default.create("br", null),
                                        "Simple Unit Tests",
                                        XNode_1.default.create("br", null),
                                        "Easy HTTP Rest API",
                                        XNode_1.default.create("br", null),
                                        "Design time mocks",
                                        XNode_1.default.create("br", null),
                                        "Use VS Code to Build Xamarin.Forms Apps"),
                                    XNode_1.default.create("div", { class: "row" }))))),
                    XNode_1.default.create("section", { id: "services", class: "services section-bg" },
                        XNode_1.default.create("div", { class: "container" },
                            XNode_1.default.create("div", { class: "section-title", "data-aos": "fade-up" },
                                XNode_1.default.create("h2", null, "Services"),
                                XNode_1.default.create("p", null, "Xamarin.Forms with TypeScript")),
                            XNode_1.default.create("div", { class: "row" },
                                XNode_1.default.create("div", { class: "col-md-6 col-lg-3 d-flex align-items-stretch", "data-aos": "zoom-in", "data-aos-delay": "100" },
                                    XNode_1.default.create("div", { class: "icon-box" },
                                        XNode_1.default.create("div", { class: "icon" },
                                            XNode_1.default.create("i", { class: "fas fa-4x fa-gem" })),
                                        XNode_1.default.create("h4", { class: "title" },
                                            XNode_1.default.create("a", { href: "" }, "Controls")),
                                        XNode_1.default.create("p", { class: "description" }, " Line of Business Controls for Web and Xamarin.Forms"))),
                                XNode_1.default.create("div", { class: "col-md-6 col-lg-3 d-flex align-items-stretch", "data-aos": "zoom-in", "data-aos-delay": "200" },
                                    XNode_1.default.create("div", { class: "icon-box" },
                                        XNode_1.default.create("div", { class: "icon" },
                                            XNode_1.default.create("i", { class: "fas fa-4x fa-laptop-code" })),
                                        XNode_1.default.create("h4", { class: "title" },
                                            XNode_1.default.create("a", { href: "" }, "Leverage TSX")),
                                        XNode_1.default.create("p", { class: "description" }, "All Views (for Web and Xamarin.Forms) can be written in TSX."))),
                                XNode_1.default.create("div", { class: "col-md-6 col-lg-3 d-flex align-items-stretch", "data-aos": "zoom-in", "data-aos-delay": "300" },
                                    XNode_1.default.create("div", { class: "icon-box" },
                                        XNode_1.default.create("div", { class: "icon" },
                                            XNode_1.default.create("i", { class: "fas fa-4x fa-globe" })),
                                        XNode_1.default.create("h4", { class: "title" },
                                            XNode_1.default.create("a", { href: "" }, "Live Hot Reload")),
                                        XNode_1.default.create("p", { class: "description" }, "Hot Reload Xamarin.Forms Applications from web server!"))),
                                XNode_1.default.create("div", { class: "col-md-6 col-lg-3 d-flex align-items-stretch", "data-aos": "zoom-in", "data-aos-delay": "400" },
                                    XNode_1.default.create("div", { class: "icon-box" },
                                        XNode_1.default.create("div", { class: "icon" },
                                            XNode_1.default.create("i", { class: "fas fa-4x fa-sun" })),
                                        XNode_1.default.create("h4", { class: "title" },
                                            XNode_1.default.create("a", { href: "" }, "Simple License")),
                                        XNode_1.default.create("p", { class: "description" }, "MIT License for Web, Single Commercial License for Single Xamarin.Forms App, with unlimited users.")))))),
                    XNode_1.default.create("section", { id: "portfolio", class: "portfolio" },
                        XNode_1.default.create("div", { class: "container" },
                            XNode_1.default.create("div", { class: "section-title", "data-aos": "fade-up" },
                                XNode_1.default.create("h2", null, "WebAtoms Sample"),
                                XNode_1.default.create("p", null, "Check out our WebAtoms sample")),
                            XNode_1.default.create("div", { class: "row text-left", style: "position: relative;\n\t\t\t\t\t\t\tmin-height: 600px;\n\t\t\t\t\t\t\twidth: 100%;\n\t\t\t\t\t\t\tmargin: 0;\n\t\t\t\t\t\t\toverflow: auto", styleMinHeight: Bind_1.default.oneTime(function () { return _this.app.screen.screenType === "mobile"
                                    ? "1120px" :
                                    "600px"; }) },
                                XNode_1.default.create(FromDemo_1.default, null)))),
                    XNode_1.default.create("section", { id: "download", class: "services section-bg" },
                        XNode_1.default.create("div", { class: "container" },
                            XNode_1.default.create("div", { class: "section-title", "data-aos": "fade-up" },
                                XNode_1.default.create("h2", null, "Try Now"),
                                XNode_1.default.create("p", null, "Xamarin.Forms with TypeScript")),
                            XNode_1.default.create("div", { class: "row", style: "background-color: black; color: white" },
                                XNode_1.default.create("pre", { style: "color: white" },
                                    XNode_1.default.create("ol", null,
                                        XNode_1.default.create("li", null,
                                            "Download github repo from",
                                            XNode_1.default.create("a", { href: "https://github.com/web-atoms/xf-samples", target: "_tab" }, "https://github.com/web-atoms/xf-samples")),
                                        XNode_1.default.create("li", null, "Open project in Visual Studio Code"),
                                        XNode_1.default.create("li", null,
                                            "Run ",
                                            XNode_1.default.create("code", null, "npm install -g @web-atoms/dev-server")),
                                        XNode_1.default.create("li", null,
                                            "Run ",
                                            XNode_1.default.create("code", null, "npm install")),
                                        XNode_1.default.create("li", null,
                                            "In VS Code, run Tasks, run ",
                                            XNode_1.default.create("code", null, "All Tasks")),
                                        XNode_1.default.create("li", null,
                                            "Open ",
                                            XNode_1.default.create("code", null, "XFDemo.sln"),
                                            " in Visual Studio (This needs to be done only once)"),
                                        XNode_1.default.create("li", null,
                                            "Go to file ",
                                            XNode_1.default.create("code", null, "App.xaml.cs")),
                                        XNode_1.default.create("li", null,
                                            "Change root to ",
                                            XNode_1.default.create("code", null, "http://.../"),
                                            " displayed in step 4"),
                                        XNode_1.default.create("li", null, "If you are unable to run it from there, you can still use CDN to run published samples")))))),
                    XNode_1.default.create("section", { id: "team", class: "team" },
                        XNode_1.default.create("div", { class: "container" },
                            XNode_1.default.create("div", { class: "section-title", "data-aos": "fade-up" },
                                XNode_1.default.create("h2", null, "Our Plan"),
                                XNode_1.default.create("p", null, "Choose the plan that's right for you")),
                            XNode_1.default.create("div", { class: "price-table" },
                                XNode_1.default.create("table", null,
                                    XNode_1.default.create("thead", null,
                                        XNode_1.default.create("tr", null,
                                            XNode_1.default.create("th", null, "PRODUCT"),
                                            XNode_1.default.create("th", null, "LICENCE"),
                                            XNode_1.default.create("th", null, " PRICE "),
                                            XNode_1.default.create("th", null, "  "))),
                                    XNode_1.default.create("tbody", null,
                                        XNode_1.default.create("tr", null,
                                            XNode_1.default.create("td", null, "WEB"),
                                            XNode_1.default.create("td", null, "MIT"),
                                            XNode_1.default.create("td", null, "Free"),
                                            XNode_1.default.create("td", null,
                                                XNode_1.default.create("p", { align: "center", style: "margin-bottom: 0" },
                                                    XNode_1.default.create("a", { href: "https://www.webatoms.in/samples.html", target: "_blank", class: "btn btn-block btn-primary text-uppercase" }, "Download")))),
                                        XNode_1.default.create("tr", null,
                                            XNode_1.default.create("td", null, "XF DROID"),
                                            XNode_1.default.create("td", null, "COMMERCIAL"),
                                            XNode_1.default.create("td", null, "$699"),
                                            XNode_1.default.create("td", null,
                                                XNode_1.default.create("p", { align: "center", style: "margin-bottom: 0" },
                                                    XNode_1.default.create("a", { href: "https://www.componentsource.com/product/web-atoms/prices", target: "_blank", class: "btn btn-block btn-primary text-uppercase" }, " BUY NOW")))),
                                        XNode_1.default.create("tr", null,
                                            XNode_1.default.create("td", null, "XF iOS"),
                                            XNode_1.default.create("td", null, "COMMERCIAL"),
                                            XNode_1.default.create("td", null, "$699"),
                                            XNode_1.default.create("td", null,
                                                XNode_1.default.create("p", { align: "center", style: "margin-bottom: 0" },
                                                    XNode_1.default.create("a", { href: "https://www.componentsource.com/product/web-atoms/prices", target: "_blank", class: "btn btn-block btn-primary text-uppercase" }, " BUY NOW")))),
                                        XNode_1.default.create("tr", null,
                                            XNode_1.default.create("td", null,
                                                "XF MOBILE ",
                                                XNode_1.default.create("br", null),
                                                "(ios + Droid)"),
                                            XNode_1.default.create("td", null, "COMMERCIAL"),
                                            XNode_1.default.create("td", null, "$999"),
                                            XNode_1.default.create("td", null,
                                                XNode_1.default.create("p", { align: "center", style: "margin-bottom: 0" },
                                                    XNode_1.default.create("a", { href: "https://www.componentsource.com/product/web-atoms/prices", target: "_blank", class: "btn btn-block btn-primary text-uppercase" }, " BUY NOW")))),
                                        XNode_1.default.create("tr", null,
                                            XNode_1.default.create("td", null,
                                                "XF MOBILE SOURCE ",
                                                XNode_1.default.create("br", null),
                                                "(ios + Droid)"),
                                            XNode_1.default.create("td", null, "COMMERCIAL"),
                                            XNode_1.default.create("td", null, "$9,999"),
                                            XNode_1.default.create("td", null,
                                                XNode_1.default.create("p", { align: "center", style: "margin-bottom: 0" },
                                                    XNode_1.default.create("a", { href: "https://www.componentsource.com/product/web-atoms/prices", target: "_blank", class: "btn btn-block btn-primary text-uppercase" }, " BUY NOW")))))),
                                XNode_1.default.create("div", null,
                                    XNode_1.default.create("h3", null, "Authorized Distributor"),
                                    XNode_1.default.create("a", { href: "https://www.componentsource.com/product/web-atoms", target: "_blank" },
                                        XNode_1.default.create("img", { src: cs_png_1.default })),
                                    XNode_1.default.create("a", { style: "margin-left:20px; font-size: x-large", href: "https://www.componentsource.com/help-support/about-us/contact", target: "_blank" },
                                        XNode_1.default.create("i", { class: "fas fa-phone-square-alt" }),
                                        "Sales Contact")))))),
                XNode_1.default.create("footer", { id: "footer" },
                    XNode_1.default.create("div", { class: "footer-top" },
                        XNode_1.default.create("div", { class: "container" },
                            XNode_1.default.create("div", { class: "row" },
                                XNode_1.default.create("div", { class: "col-lg-3 col-md-6 footer-contact", "data-aos": "fade-up", "data-aos-delay": "100" },
                                    XNode_1.default.create("h5", null, "NeuroSpeech Technologies Pvt Ltd"),
                                    XNode_1.default.create("p", null,
                                        "Unit 103, Building 3, ",
                                        XNode_1.default.create("br", null),
                                        "Sector 3, Millennium Business Park, ",
                                        XNode_1.default.create("br", null),
                                        "Mahape, Navi Mumbai",
                                        XNode_1.default.create("br", null),
                                        XNode_1.default.create("br", null),
                                        XNode_1.default.create("strong", null, "Phone:"),
                                        "+91 22 27781459",
                                        XNode_1.default.create("br", null),
                                        XNode_1.default.create("strong", null, "Email:"),
                                        " Support@neurospeech.com",
                                        XNode_1.default.create("br", null))),
                                XNode_1.default.create("div", { class: "col-lg-3 col-md-6 footer-links", "data-aos": "fade-up", "data-aos-delay": "200" },
                                    XNode_1.default.create("h4", null, "Useful Links"),
                                    XNode_1.default.create("ul", null,
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#" }, "Home")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#about" }, "About us")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            XNode_1.default.create("a", { href: "https://www.webatoms.in/xf/samples.html" }, "Xamarin.Forms  Docs")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "https://www.webatoms.in/samples.html" }, "Web Docs")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#team" }, "Buy")))),
                                XNode_1.default.create("div", { class: "col-lg-3 col-md-6 footer-links", "data-aos": "fade-up", "data-aos-delay": "300" },
                                    XNode_1.default.create("h4", null, "Our Services"),
                                    XNode_1.default.create("ul", null,
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#team" }, "Web Development")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#team" }, "XF Android")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#team" }, "XF iOS")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#team" }, "XF Mobile")),
                                        XNode_1.default.create("li", null,
                                            XNode_1.default.create("i", { class: "fas fa-chevron-right" }),
                                            " ",
                                            XNode_1.default.create("a", { href: "#team" }, "XF Mobile Source")))),
                                XNode_1.default.create("div", { class: "col-lg-3 col-md-6 footer-links", "data-aos": "fade-up", "data-aos-delay": "400" },
                                    XNode_1.default.create("h4", null, "Networks"),
                                    XNode_1.default.create("div", { class: "social-links mt-3" },
                                        XNode_1.default.create("a", { href: "https://twitter.com/WebAtoms", target: "_blank", class: "twitter" },
                                            XNode_1.default.create("i", { class: "fab fa-twitter" })),
                                        XNode_1.default.create("a", { eventClick: function () { return alert("Facebook? Seriously for development?"); }, class: "facebook" },
                                            XNode_1.default.create("i", { class: "fab fa-facebook-f" })),
                                        XNode_1.default.create("a", { href: "https://github.com/web-atoms", target: "_blank", class: "github" },
                                            XNode_1.default.create("i", { class: "fab fa-github" }))))))),
                    XNode_1.default.create("div", { class: "container py-4" },
                        XNode_1.default.create("div", { class: "copyright" }, "\u00A9 2018 NeuroSpeech Technologies Pvt Ltd (India). All Rights Reserved."),
                        XNode_1.default.create("a", { href: "/terms.html", target: "_blank", style: "margin-left: 40px" }, "Terms"),
                        XNode_1.default.create("a", { href: "/eula.html", target: "_blank", style: "margin-left: 40px" }, "End User License Agreement"))),
                XNode_1.default.create("a", { href: "#", class: "back-to-top" },
                    XNode_1.default.create("i", { class: "fas fa-angle-up" }))));
            // breaks seo waiting
            window.appReady = true;
        };
        return Index;
    }(AtomControl_1.AtomControl));
    exports.default = Index;
});
//# sourceMappingURL=Index.js.map

    AmdLoader.instance.setup("@web-atoms/samples/dist/web/Index");

        //# sourceMappingURL=Index.pack.js.map
        