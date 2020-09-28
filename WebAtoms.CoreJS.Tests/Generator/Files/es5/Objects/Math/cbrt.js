assert(Number.isNaN(Math.cbrt()));
assert(Number.isNaN(Math.cbrt(undefined)));
assert.strictEqual(Math.cbrt(null), 0);
assert.strictEqual(Math.cbrt(0), 0);
assert.strictEqual(Math.cbrt(""), 0);
assert(Number.isNaN(Math.cbrt("abcd")));
assert.strictEqual(Math.cbrt("1.2"), 1.0626585691826111);
assert.strictEqual(Math.cbrt(" 1.2"), 1.0626585691826111);
assert.strictEqual(Math.cbrt(-1), -1);
assert.strictEqual(Math.cbrt(1), 1);
assert.strictEqual(Math.cbrt(Infinity), Infinity);
assert.strictEqual(Math.cbrt(64), 4);

