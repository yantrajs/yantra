assert(Number.isNaN(Math.log()));
assert(Number.isNaN(Math.log(undefined)));
assert.strictEqual(Math.log(null), Number.NEGATIVE_INFINITY);
assert.strictEqual(Math.log(0), Number.NEGATIVE_INFINITY);
assert.strictEqual(Math.log(""), Number.NEGATIVE_INFINITY);
assert(Number.isNaN(Math.log("abcd")));
assert.doubleEqual(Math.log("1.2"), 0.1823215567939546);
assert.doubleEqual(Math.log(" 1.2"), 0.1823215567939546);
assert(Number.isNaN(Math.log("-1")));
assert.strictEqual(Math.log("1"), 0);
assert.strictEqual(Math.log(Infinity), Infinity);
assert.strictEqual(Math.log(8) / Math.log(2), 3);
assert.strictEqual(Math.log(625) / Math.log(5), 4);




