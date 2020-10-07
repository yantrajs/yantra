assert(Number.isNaN(Math.sign()));
assert(Number.isNaN(Math.sign(undefined)));
assert.strictEqual(Math.sign(null), 0);
assert.strictEqual(Math.sign(0), 0);
assert.strictEqual(Math.sign(-0), -0);
assert.strictEqual(Math.sign(""), 0);
assert(Number.isNaN(Math.sign("abcd")));
assert.strictEqual(Math.sign("1.2"), 1);
assert.doubleEqual(Math.sign(" 1.2"), 1);
assert.strictEqual(Math.sign(Infinity), 1);
assert.strictEqual(Math.sign(-1), -1);
assert.strictEqual(Math.sign(1), 1);

assert.strictEqual(Math.sign(3), 1);
assert.strictEqual(Math.sign(-3), -1);
assert.strictEqual(Math.sign('-3'), -1);

