
assert(Number.isNaN(Math.cos()));
assert(Number.isNaN(Math.cos(undefined)));
assert.strictEqual(Math.cos(null), 1);
assert.strictEqual(Math.cos(0), 1);
assert.strictEqual(Math.cos(""), 1);
assert(isNaN(Math.cos("abcd")));
assert.strictEqual(Math.cos("1.2"), 0.3623577544766736);
assert.strictEqual(Math.cos(" 1.2"), 0.3623577544766736);
assert(Number.isNaN(Math.cos(Infinity)));
assert.strictEqual(Math.cos(1) * 10, 5.403023058681397);
assert.strictEqual(Math.cos(2) * 10, -4.161468365471424);
assert.strictEqual(Math.cos(Math.PI) * 10, -10);

