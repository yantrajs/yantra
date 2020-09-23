var a = {
    a: 1,
    b: 2
};

var entries = Object.entries(a);
assert(Array.isArray(entries));
assert.strictEqual(entries.length, 2);
assert.strictEqual(entries[0][0],  "a");
assert.strictEqual(entries[0][1], 1);
var b = entries[1];
assert.strictEqual(b[0], "b");
assert.strictEqual(b[1], 2);
