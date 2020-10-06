assert(Number.isNaN(Math.pow()));
assert(Number.isNaN(Math.pow(undefined)));
assert(Number.isNaN(Math.pow(null)));
assert(Number.isNaN(Math.pow(0)));
assert(Number.isNaN(Math.pow("")));
assert(Number.isNaN(Math.pow("abcd")));
assert(Number.isNaN(Math.pow("1.2")));
assert(Number.isNaN(Math.pow(" 1.2")));
assert(Number.isNaN(Math.pow(Infinity)));
assert.strictEqual(Math.pow(7,3), 343);
assert.strictEqual(Math.pow(4, 0.5), 2);
assert.doubleEqual(Math.pow(7, -2), 0.02040816326530612);
assert(Number.isNaN(Math.pow(-7,0.5)));

