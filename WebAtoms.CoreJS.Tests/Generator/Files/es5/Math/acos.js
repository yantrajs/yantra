assert(Number.isNaN(Math.acos()));
assert(Number.isNaN(Math.acos(undefined)));
assert.strictEqual(Math.acos(null), 1.5707963267948966);
assert.strictEqual(Math.acos(0), 1.5707963267948966);
assert.strictEqual(Math.acos(""), 1.5707963267948966);
assert(isNaN(Math.acos("abcd")));
assert(isNaN(Math.acos("1.2")));
assert(isNaN(Math.acos(" 1.2")));
assert.strictEqual(Math.acos(1), 0);
assert.strictEqual(Math.acos(-1), 3.141592653589793);
assert.strictEqual(Math.acos(8 / 10), 0.6435011087932843);
assert(isNaN(Math.acos(5 / 3)));

