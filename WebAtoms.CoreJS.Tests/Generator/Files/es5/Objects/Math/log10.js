assert(Number.isNaN(Math.log10()));
assert(Number.isNaN(Math.log10(undefined)));
assert.strictEqual(Math.log10(null), Number.NEGATIVE_INFINITY);
assert.strictEqual(Math.log10(0), Number.NEGATIVE_INFINITY);
assert.strictEqual(Math.log10(""), Number.NEGATIVE_INFINITY);
assert(Number.isNaN(Math.log10("abcd")));
assert.doubleEqual(Math.log10("1.2"), 0.07918124604762482);
assert.doubleEqual(Math.log10(" 1.2"), 0.07918124604762482);
assert(Number.isNaN(Math.log10("-1")));
assert.strictEqual(Math.log10("1"), 0);
assert.strictEqual(Math.log10(Infinity), Infinity);
assert.strictEqual(Math.log10(100000), 5);
assert.doubleEqual(Math.log10(2), 0.3010299956639812);
assert.strictEqual(Math.log10(1), 0);

