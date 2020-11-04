const array1 = [1, 3, 2];
assert.strictEqual(Math.max(), Number.NEGATIVE_INFINITY);
assert(Number.isNaN(Math.max(undefined)));
assert.strictEqual(Math.max(null), 0);
assert.strictEqual(Math.max(0), 0);
assert.strictEqual(Math.max(""), 0);
assert(Number.isNaN(Math.max("abcd")));
assert.strictEqual(Math.max("1.2"), 1.2);
assert.strictEqual(Math.max(" 1.2"), 1.2);
assert.strictEqual(Math.max(Infinity), Infinity);
assert.strictEqual(Math.max(-1,-3,-2), -1);
assert.strictEqual(Math.max(1, 3, 2), 3);

// ES6 not implemented yet : 9/30/2020
//assert.strictEqual(Math.max(...array1),3);
