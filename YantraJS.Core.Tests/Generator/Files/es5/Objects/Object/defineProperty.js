var a = {};
Object.defineProperty(a, "b", {
    configurable: true,
    value: 1
});
assert.strictEqual(a.b, 1);

var n = 1;

// property..
Object.defineProperty(a, "c", {
    configurable: true,
    get: function () {
        return n;
    }
});
assert.strictEqual(a.c, 1);

n = 2;

assert.strictEqual(a.c, 2);
Object.defineProperty(a, "c", {
    set: function (v) {
        n = v;
    },
    get: function () {
        return n;
    }
});

a.c = 4;
assert.strictEqual(n, 4);
assert.strictEqual(a.c, 4);
