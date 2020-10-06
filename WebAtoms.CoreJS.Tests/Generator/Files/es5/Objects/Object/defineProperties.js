var a = {};
var n = 1;

Object.defineProperties(a, {
    b: {
        value: 1
    },
    c: {
        set: function (v) {
            n = v;
        },
        get: function () {
            return n;
        }
    }
});
assert.strictEqual(a.b, 1);


assert.strictEqual(a.c, 1);

n = 2;

assert.strictEqual(a.c, 2);
a.c = 4;
assert.strictEqual(n, 4);
assert.strictEqual(a.c, 4);
