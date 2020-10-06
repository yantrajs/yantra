const array1 = [2, 3, 1];
assert.strictEqual(Math.min(), Number.POSITIVE_INFINITY);
assert(Number.isNaN(Math.min(undefined)));
assert.strictEqual(Math.min(null), 0);
assert.strictEqual(Math.min(0), 0);
assert.strictEqual(Math.min(""), 0);
assert(Number.isNaN(Math.min("abcd")));
assert.strictEqual(Math.min("1.2"), 1.2);
assert.strictEqual(Math.min(" 1.2"), 1.2);
assert.strictEqual(Math.min(Infinity), Infinity);
assert.strictEqual(Math.min(-2, -3, -1), -3);
assert.strictEqual(Math.min(2, 3, 1), 1);

// ES6 not implemented yet : 9/30/2020
//assert.strictEqual(Math.min(...array1),1);
