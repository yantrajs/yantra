assert.strictEqual(Math.clz32(), 32);
assert.strictEqual(Math.clz32(undefined), 32);
assert.strictEqual(Math.clz32(null), 32);
assert.strictEqual(Math.clz32(0), 32);
assert.strictEqual(Math.clz32(""), 32);
assert.strictEqual(Math.clz32("abcd"), 32);
assert.strictEqual(Math.clz32("1.2"), 31);
assert.strictEqual(Math.clz32(" 1.2"), 31);
// 00000000000000000000000000000001
assert.strictEqual(Math.clz32(1), 31);
// 00000000000000000000000000000100
assert.strictEqual(Math.clz32(4), 29);
// 00000000000000000000001111101000
assert.strictEqual(Math.clz32(1000), 22);
