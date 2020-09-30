assert(Number.isNaN(Math.log2()));
assert(Number.isNaN(Math.log2(undefined)));
assert.strictEqual(Math.log2(null), Number.NEGATIVE_INFINITY);
assert.strictEqual(Math.log2(0), Number.NEGATIVE_INFINITY);
assert.strictEqual(Math.log2(""), Number.NEGATIVE_INFINITY);
assert(Number.isNaN(Math.log2("abcd")));
assert.doubleEqual(Math.log2("1.2"), 0.2630344058337938);
assert.doubleEqual(Math.log2(" 1.2"), 0.2630344058337938);
assert(Number.isNaN(Math.log2(-1)));
assert.strictEqual(Math.log2("1"), 0);
assert.strictEqual(Math.log2(Infinity), Infinity);
assert.doubleEqual(Math.log2(3), 1.584962500721156);
assert.strictEqual(Math.log2(2), 1);


