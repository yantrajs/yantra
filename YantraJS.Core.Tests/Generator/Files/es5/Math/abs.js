assert(Number.isNaN(Math.abs()));
assert(Number.isNaN(Math.abs(undefined)));
assert.strictEqual(Math.abs(null), 0);
assert.strictEqual(Math.abs(0), 0);
assert.strictEqual(Math.abs(""), 0);
assert(Number.isNaN(Math.abs("abcd")));
assert.strictEqual(Math.abs("1.2"), 1.2);
assert.strictEqual(Math.abs(" 1.2"), 1.2);
assert.strictEqual(Math.abs(3 - 5), 2);
assert.strictEqual(Math.abs(1.23456 - 7.89012), 6.6555599999999995);


