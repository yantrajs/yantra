function calcHypotenuse(a, b) {
    return (Math.sqrt((a * a) + (b * b)));
}
assert(Number.isNaN(Math.sqrt()));
assert(Number.isNaN(Math.sqrt(undefined)));
assert.strictEqual(Math.sqrt(null), 0);
assert.strictEqual(Math.sqrt(0), 0);
assert.strictEqual(Math.sqrt(""), 0);
assert(Number.isNaN(Math.sqrt("abcd")));
assert.doubleEqual(Math.sqrt("1.2"), 1.0954451150103321);
assert.doubleEqual(Math.sqrt(" 1.2"), 1.0954451150103321);
assert(Number.isNaN(Math.sqrt(-1)));
assert.strictEqual(Math.sqrt(1), 1);
assert.strictEqual(Math.sqrt(Infinity), Infinity);
assert.strictEqual(calcHypotenuse(3, 4), 5);
assert.strictEqual(calcHypotenuse(5, 12), 13);
assert.strictEqual(calcHypotenuse(0, 0), 0);
