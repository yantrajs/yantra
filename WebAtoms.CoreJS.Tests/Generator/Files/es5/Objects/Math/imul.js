assert.strictEqual(Math.imul(), 0);
assert.strictEqual(Math.imul(undefined), 0);
assert.strictEqual(Math.imul(null), 0);
assert.strictEqual(Math.imul(0), 0);
assert.strictEqual(Math.imul(""), 0);
assert.strictEqual(Math.imul("abcd"), 0);
assert.strictEqual(Math.imul("1.2"), 0);
assert.strictEqual(Math.imul(" 1.2"), 0);
assert.strictEqual(Math.imul(Infinity), 0);
assert.strictEqual(Math.imul(3,4), 12);
assert.strictEqual(Math.imul(-5,12), -60);
assert.strictEqual(Math.imul(0xffffffff, 5),-5);
assert.strictEqual(Math.imul(0xfffffffe, 5), -10);

