assert(Number.isNaN(Math.floor()));
assert(Number.isNaN(Math.floor(undefined)));
assert.strictEqual(Math.floor(null), 0);
assert.strictEqual(Math.floor(0), 0);
assert.strictEqual(Math.floor(""), 0);
assert(Number.isNaN(Math.floor("abcd")));
assert.strictEqual(Math.floor("1.2"), 1);
assert.strictEqual(Math.floor(" 1.2"), 1);