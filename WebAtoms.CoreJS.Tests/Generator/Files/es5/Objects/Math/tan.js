function getTanFromDegrees(degrees) {
    return Math.tan(degrees * Math.PI / 180);
}
assert(Number.isNaN(Math.tan()));
assert(Number.isNaN(Math.tan(undefined)));
assert.strictEqual(Math.tan(null), 0);
assert.strictEqual(Math.tan(0), 0);
assert.strictEqual(Math.tan(""), 0);
assert(isNaN(Math.tan("abcd")));
assert.doubleEqual(Math.tan("1.2"), 2.5721516221263188);
assert.doubleEqual(Math.tan(" 1.2"), 2.5721516221263188);
assert(isNaN(Math.tan(Infinity)));
assert.doubleEqual(Math.tan(-1), -1.5574077246549023);
assert.doubleEqual(Math.tan(0.5), 0.5463024898437905);
assert.strictEqual(Math.tan(1), 1.5574077246549023);
assert.strictEqual(getTanFromDegrees(0), 0);
assert.doubleEqual(getTanFromDegrees(45), 0.9999999999999999);
assert.doubleEqual(getTanFromDegrees(90), 16331239353195370);